using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// A script that gets attached to all ASL PUN objects that manages ownership
    /// behaviors and restrictions for the attached object.
    /// </summary>
    public class OwnableObject : Photon.PunBehaviour
    {
        #region Fields
        /// <summary>
        /// When ownership of an object defaults, it defaults to a PUN ID of '0',
        /// which associates with being owned by the scene.
        /// </summary>
        private int SCENE_VALUE = 0;    // Hidden feature: assigning ownership to '0' makes object a scene object

        /// <summary>
        /// Simple boolean determining if the object's ownership is restricted to
        /// some ASL PUN PhotonPlayer IDs.
        /// </summary>
        private bool isRestricted = false;

        /// <summary>
        /// A list of whitelisted IDs.
        /// </summary>
        private List<int> restrictedIDs;
        #endregion

        #region Methods
        // Add object behavior components
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when this
        /// class is enabled. Initializes the restricted ID list and sets the
        /// Ownership methodology to "Takeover" which allows nonconsensual
        /// ownership transfer.
        /// </summary>
        protected void Awake()
        {
            //gameObject.AddComponent<ASL.UI.Mouse.OwnershipTransfer>();
            restrictedIDs = new List<int>();
            PhotonView pv = gameObject.GetPhotonView();
            pv.ownershipTransfer = OwnershipOption.Takeover;
        }

        // Fire an event when instantiated
        // Automatically transfer ownership of objects to default scene.
        /// <summary>
        /// Automatically transfer ownership of objects to default (scene)
        /// upon initialization.
        /// </summary>
        /// <param name="info"></param>
        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            base.OnPhotonInstantiate(info);
            //this.gameObject.GetPhotonView().TransferOwnership(SCENE_VALUE);
        }

        /// <summary>
        /// Method to grab ownership of an object.
        /// </summary>
        /// 
        /// <param name="info">
        /// Information associated with the grab attempt.
        /// </param>
        [PunRPC]
        public void Grab(PhotonMessageInfo info)
        {
            this.RelinquishOwnership(info.sender.ID);

            //int grabbed = (int)OWNERSHIPSTATE.FAIL;

            //if (this.RequestOwnership(info) == 0)
            //    grabbed = (int)OWNERSHIPSTATE.NOW;

            //return grabbed;
        }

        /// <summary>
        /// Method to determine if a player has ownership of this ASL GameObject.
        /// </summary>
        /// 
        /// <param name="player">
        /// The player who may own this ASL GameObject.
        /// </param>
        /// 
        /// <returns>
        /// True if this GameObject has a PhotonView and the queried player
        /// matches the specified owner of this GameObject.
        /// </returns>
        public bool HasOwnership(PhotonPlayer player)
        {
            if (gameObject.GetComponent<PhotonView>() != null)
            {
                return player.Equals(gameObject.GetComponent<PhotonView>().owner);
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Releases ownership of this GameObject to the specified ASL PUN Player
        /// ID.
        /// </summary>
        /// 
        /// <param name="newPlayerID">
        /// The PUN ID of the player who will be taking over ownership.
        /// </param>
        public void RelinquishOwnership(int newPlayerID)
        {
            // Ignore all items tagged with "room" tag
            if (this.tag.ToUpper().CompareTo("ROOM") != 0)
            {
                if (gameObject.GetPhotonView().owner.Equals(PhotonNetwork.player))
                {
                    photonView.TransferOwnership(newPlayerID);
                }
                else if (gameObject.GetPhotonView().owner.Equals(SCENE_VALUE))
                {
                    photonView.RequestOwnership();
                    photonView.TransferOwnership(newPlayerID);
                }
                gameObject.GetPhotonView().ownerId = newPlayerID;
            }
        }

        /// <summary>
        /// Determines if the caller ASL node can take ownership of this object.
        /// </summary>
        /// 
        /// <returns>
        /// True if the object's ownership lies with the scene, if the ownership
        /// is not currently restricted, or if ownership is restricted and this
        /// ASL node ID is whitelisted.
        /// </returns>
        public bool CanTake()
        {
            if (StoredInScene || !isRestricted || (isRestricted && restrictedIDs.Contains(PhotonNetwork.player.ID)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Grabs ownership of this object if it can.
        /// </summary>
        /// 
        /// <returns>
        /// True if object ownership rests with the scene or if ownership can
        /// otherwise be transferred and is not currently owned.
        /// </returns>
        public bool Take()
        {
            PhotonView pv = gameObject.GetPhotonView();
            if (gameObject.GetPhotonView() != null)
            {
                bool owned = HasOwnership(PhotonNetwork.player);
                if (StoredInScene)
                {
                    pv.RequestOwnership();
                    pv.ownerId = PhotonNetwork.player.ID;
                }
                else if (CanTake() && !owned)
                {
                    pv.RPC("Grab", PhotonTargets.Others);
                }

                if (HasOwnership(PhotonNetwork.player))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        // restrict and add yourself to the list of ownable IDs
        // returns list of ownable IDs
        /// <summary>
        /// Restrict and whitelist yourself.
        /// </summary>
        /// 
        /// <returns>
        /// List of whitelisted PUN IDs.
        /// </returns>
        public List<int> Restrict()
        {
            if (!isRestricted && Take())
            {
                isRestricted = true;
                if (!restrictedIDs.Contains(PhotonNetwork.player.ID))
                {
                    restrictedIDs.Add(PhotonNetwork.player.ID);
                }

                // propagate restriction event
                ObjectManager objManager = GameObject.FindObjectOfType<ObjectManager>();
                if(objManager != null)
                {
                    objManager.SetObjectRestrictions(gameObject, true, restrictedIDs);
                }

                PhotonView pv = gameObject.GetPhotonView();
                pv.ownershipTransfer = OwnershipOption.Fixed;

                return OwnablePlayerIDs;
            }
            else if (isRestricted)
            {
                return OwnablePlayerIDs;
            }
            else
            {
                Debug.LogWarning("Ownership restriction failed.");
                return null;
            }
        }

        /// <summary>
        /// Restrict ownership access to this object to only yourself.
        /// </summary>
        /// 
        /// <returns>
        /// A list of whitelisted IDs (i.e. your ID).
        /// </returns>
        public List<int> RestrictToYourself()
        {
            if (!isRestricted && Take())
            {
                ClearOwnablePlayerIDs();
                return Restrict();
            }
            else if (isRestricted)
            {
                return OwnablePlayerIDs;
            }
            else
            {
                Debug.LogWarning("Ownership restriction failed.");
                return null;
            }
        }

        /// <summary>
        /// Remove restrictions regarding ownership transfer access.
        /// </summary>
        /// 
        /// <returns>
        /// True if you can take ownership of the object or if the GameObject's
        /// ownership transfer access is already unrestricted.
        /// </returns>
        public bool UnRestrict()
        {
            if(isRestricted && Take())
            {
                isRestricted = false;
                
                // propagate restriction event
                ObjectManager objManager = GameObject.FindObjectOfType<ObjectManager>();
                if (objManager != null)
                {
                    objManager.SetObjectRestrictions(gameObject, false, restrictedIDs);
                }

                PhotonView pv = gameObject.GetPhotonView();
                pv.ownershipTransfer = OwnershipOption.Takeover;

                return true;
            }
            else if (!isRestricted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Helper method to handle the logic of setting restrictions for 
        /// GameObject ownership access (or relaxes them).
        /// </summary>
        /// 
        /// <param name="restricted">
        /// True if you want to restrict access to the object's ownership access.
        /// False if you want to open access to the object's ownership access.
        /// </param>
        /// <param name="ownableIDs">
        /// List of whitelisted PUN IDs that can later request access to the
        /// ownership of an object.
        /// </param>
        public void SetRestrictions(bool restricted, int[] ownableIDs)
        {
            isRestricted = restricted;
            if (isRestricted)
            {
                PhotonView pv = gameObject.GetPhotonView();
                pv.ownershipTransfer = OwnershipOption.Fixed;
            }
            else
            {
                PhotonView pv = gameObject.GetPhotonView();
                pv.ownershipTransfer = OwnershipOption.Takeover;
            }

            ClearOwnablePlayerIDs();
            bool ownable = false;
            if (ownableIDs != null)
            {
                foreach (int id in ownableIDs)
                {
                    restrictedIDs.Add(id);
                    if (id == PhotonNetwork.player.ID)
                    {
                        ownable = true;
                    }
                }
            }

            // Handle edge case where you're ripping ownership away from someone who owns the object
            if(HasOwnership(PhotonNetwork.player) && !ownable)
            {
                PhotonView pv = gameObject.GetPhotonView();
                if(pv != null)
                {
                    pv.TransferOwnership(0);
                }
            }
        }

        /// <summary>
        /// Whitelists given PUN PhotonPlayer IDs for access to GameObject's
        /// ownership.
        /// </summary>
        /// 
        /// <param name="playerIDs">
        /// A list of ints representing the PhotonPlayer IDs associated with
        /// each player who should be given rights to request ownership for
        /// the GameObject.
        /// </param>
        public void WhiteListPlayerID(List<int> playerIDs)
        {
            bool changed = false;

            if (playerIDs != null)
            {
                foreach (int id in playerIDs)
                {
                    if (!restrictedIDs.Contains(id))
                    {
                        restrictedIDs.Add(id);
                        changed = true;
                    }
                }
            }

            // propagate restriction event
            if (changed)
            {
                ObjectManager objManager = GameObject.FindObjectOfType<ObjectManager>();
                if (objManager != null)
                {
                    objManager.SetObjectRestrictions(gameObject, isRestricted, restrictedIDs);
                }
            }
        }

        /// <summary>
        /// Blacklists given PUN PhotonPlayer IDs so they cannot request
        /// ownership to a GameObject.
        /// </summary>
        /// 
        /// <param name="playerIDs">
        /// A list of ints representing the PhotonPlayer IDs associated with
        /// each player who should be restricted from being allowed to
        /// request ownership for the GameObject.
        /// </param>
        public void BlackListPlayerID(List<int> playerIDs)
        {
            bool changed = false;

            if (playerIDs != null)
            {
                foreach (int id in playerIDs)
                {
                    if (restrictedIDs.Contains(id))
                    {
                        restrictedIDs.Remove(id);
                        changed = true;
                    }
                }
            }

            // propagate restriction event
            if (changed)
            {
                ObjectManager objManager = GameObject.FindObjectOfType<ObjectManager>();
                if (objManager != null)
                {
                    objManager.SetObjectRestrictions(gameObject, isRestricted, restrictedIDs);
                }
            }
        }

        /// <summary>
        /// Clears and resets whitelist of potential owner IDs.
        /// </summary>
        public void ClearOwnablePlayerIDs()
        {
            restrictedIDs.Clear();
        }
#endregion

        #region Properties
        /// <summary>
        /// Determines if the ownership of this object is restricted to some ASL
        /// nodes.
        /// 
        /// NOTE: Not currently working as intended.
        /// </summary>
        public bool IsOwnershipRestricted
        {
            get
            {
                return isRestricted;
            }
        }

        /// <summary>
        /// Not very fast. Returns a safe copy of the restricted IDs. Use "CanTake" method to determine if available in this list.
        /// </summary>
        public List<int> OwnablePlayerIDs
        {
            get
            {
                List<int> copy = new List<int>();
                foreach(int id in restrictedIDs)
                {
                    copy.Add(id);
                }

                return copy;
            }
        }

        /// <summary>
        /// The PUN ID of the current owner of this GameObject.
        /// </summary>
        public int OwnerID
        {
            get
            {
                return gameObject.GetPhotonView().ownerId;
            }
        }

        /// <summary>
        /// The PhotonPlayer who is the current owner of this GameObject.
        /// </summary>
        public PhotonPlayer Owner
        {
            get
            {
                if (gameObject.GetComponent<PhotonView>() != null)
                {
                    return gameObject.GetComponent<PhotonView>().owner;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Determines if the owner has defaulted to be the scene and is
        /// available to be grabbed by anyone.
        /// </summary>
        public bool StoredInScene
        {
            get
            {
                return OwnerID == SCENE_VALUE;
            }
        }
        #endregion
    }
}