using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;
using ASL.PortalSystem;
using ASL.LocalEventSystem;

namespace ASL
{
    /// <summary>
    /// This script handles the creation of a PC player. Having an instance of this script on a GameObject will instantiate a PC player
    /// and then delete the GameObject afterwards. Primarily intended to be used through ASL Player prefab.
    /// </summary>
    public class ControllerInstantiation : LocalEventHandler
    {

        /// <summary>
        /// Material to be used locally for the PC Player avatar. Could be
        /// reworked to the photon event system to better sync between clients.
        /// </summary>
        public Material avatarMaterial;

        /// <summary>
        /// Initial position the PC player will be moved to.
        /// </summary>
        public Vector3 initialPosition;

        /// <summary>
        /// Initial scale the PC player will be set to.
        /// </summary>
        public Vector3 initialScale;

        private ObjectInteractionManager mObjectInteractionManager;
        private GameObject myPlayer;
        private GameObject myFPSCamera;

        // Working implementation for the billboard text labels was dropped to focus on supporting different
        // VR devices.
        // Future Development:  Fix the orientation of the text.
        private BillboardText myLabel;

        void Awake()
        {
            mObjectInteractionManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
            //Debug.Log("Controller Instantiation script initialized");
        }

        public override void OnJoinedRoom()
        {
            InstantiatePCPlayer();
        }

        /// <summary>
        /// Event handler for local events relating to the creation of a PC player.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="args">The event being triggered</param>
        /// <event>PCPlayerCreationFailed</event>
        /// <description>This event is handled by attempting to create an avatar until successful.</description>
        /// <event>PCPlayerCreationSucceeded</event>
        /// <description>This event is triggered after the PC player is sucessfully created across PUN.
        /// Components are enabled and disabled depending on owner to prevent camera and control inputs.</description>
        /// <event>PortalManagerPlayerSet</event>
        /// <description>This event is triggered after the Portal manager has a valid player reference. This script does
        /// will have served its purpose of creating the PC player and can be removed from the scene.</description>
        protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
        {
            switch (args.MyEvent)
            {
                case ASLLocalEventManager.LocalEvents.PCPlayerCreationFailed:
                    {
                        InstantiatePCPlayer();
                        break;
                    }
                case ASLLocalEventManager.LocalEvents.PCPlayerCreationSucceeded:
                    {
                        SucessfulCreationEventHandler();
                        break;
                    }
                case ASLLocalEventManager.LocalEvents.PortalManagerPlayerSet:
                    {
                        GameObject.Destroy(gameObject);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// This function enables or disables components of a PC player depending on the owner status of the
        /// PC player. If local user owns the Player Avatar then control scripts will be enabled, if not then
        /// they will be disabled.
        /// </summary>
        private void SucessfulCreationEventHandler()
        {
            if (myPlayer.GetComponent<PhotonView>().isMine)
            {
                myPlayer.gameObject.name = "MyAvatar";
                myFPSCamera = myPlayer.GetComponentInChildren<Camera>(true).gameObject;
                myPlayer.GetComponentInChildren<Camera>(true).enabled = true;
                myPlayer.GetComponentInChildren<SmoothMouseLook>(true).enabled = true;
                myPlayer.GetComponentInChildren<PlayerController>(true).enabled = true;

                myPlayer.transform.Find("Cursor Canvas").gameObject.SetActive(true);
                myPlayer.transform.Find("World Space Canvas").gameObject.SetActive(true);

                myLabel = myPlayer.transform.GetComponentInChildren<BillboardText>(true);
                if (myLabel)
                {
                    myLabel.enabled = true;
                    myLabel.setCamera(myFPSCamera.GetComponent<Camera>());
                }
                SetPortalManagerPlayer();
            }
            else
            {
                myPlayer.gameObject.name = "OtherAvatar";
                myPlayer.GetComponentInChildren<Camera>().enabled = false;
                myPlayer.GetComponentInChildren<SmoothMouseLook>().enabled = false;
                myPlayer.GetComponentInChildren<PlayerController>().enabled = false;

                transform.Find("Cursor Canvas").gameObject.SetActive(false);
                transform.Find("World Space Canvas").gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// This function instantiates a PC player through PUN. Raises local event reflecting status of the instantation.
        /// </summary>
        private void InstantiatePCPlayer()
        {
            myPlayer = mObjectInteractionManager.InstantiateOwnedObject("Player Avatar");
            if (myPlayer == null)
            {
                ASLLocalEventManager.Instance.Trigger(this, ASLLocalEventManager.LocalEvents.PCPlayerCreationFailed);
                return;
            }

            SetInitialProperties();

            ASLLocalEventManager.Instance.Trigger(this, ASLLocalEventManager.LocalEvents.PCPlayerCreationSucceeded);
        }

        /// <summary>
        /// Uses public fields to assign properties and initial values to the PC player.
        /// </summary>    
        /// Future Development: Material could be replaced with a PUN event to synchronize.
        private void SetInitialProperties()
        {
            myPlayer.GetComponent<MeshRenderer>().sharedMaterial = avatarMaterial;
            myPlayer.transform.position = initialPosition;
            myPlayer.transform.localScale = initialScale;
        }

        /// <summary>
        /// This function sets a tag that is used by portals for synchronizing the copy camera with this player.
        /// Should only be called after a networked instance of the PC player has been instantiated and activated.
        /// </summary>
        private void SetPortalManagerPlayer()
        {
            myFPSCamera.tag = "Local Primary Camera";
            GameObject.Find("PortalManager").GetComponent<PortalManager>().SetPlayer(myFPSCamera);
        }
    }
}