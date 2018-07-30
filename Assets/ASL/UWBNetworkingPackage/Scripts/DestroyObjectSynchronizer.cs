using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// This class exists solely so that the networking component can trigger 
    /// necessary methods when an object in the scene is destroyed. This class
    /// is expected to be attached to all network objects that can be destroyed.
    /// </summary>
    public class DestroyObjectSynchronizer : MonoBehaviour
    {
        #region Fields
        #region Private Fields
        /// <summary>
        /// Provides a constant string for easy access to the NetworkManager 
        /// active in the scene.
        /// </summary>
        private const string NAME_NETWORKMANAGER = "NetworkManager";

        /// <summary>
        /// A reference to the active Object Manager in the scene.
        /// </summary>
        private ObjectManager objManager;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when 
        /// this class is enabled. Sets the ObjectManager to be the active 
        /// ObjectManager of the scene.
        /// </summary>
        public void Awake()
        {
            objManager = GameObject.Find(NAME_NETWORKMANAGER).GetComponent<ObjectManager>();
        }

        /// <summary>
        /// Handles logic for cleaning up the object's network presence when 
        /// Destroy is called on the gameObject for this script.
        /// </summary>
        public void OnDestroy()
        {
            string objName = this.gameObject.name;
            PhotonView view = gameObject.GetComponent<PhotonView>();
            if(view != null)
            {
                int viewID = view.viewID;
                objManager.DestroyHandler(this.name, viewID);
            }
        }
        #endregion
    }
}