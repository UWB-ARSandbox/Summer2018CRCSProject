using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;
using ASL.PortalSystem;
using ASL.LocalEventSystem;

namespace ASL
{
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
        // VR devices. Future work could be put into fixing the orientation of the text.
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
                        Debug.Log(this.name + ": Event not handled");
                        break;
                    }
            }
        }

        // Event handler that will toggle components that should only be run by the owner of this player.
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

        // Instantiation function that uses the local event manager
        // to signal success and to trigger time sensitive code execution.
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

        // Uses public fields to assign properties to the PC player.
        private void SetInitialProperties()
        {
            myPlayer.GetComponent<MeshRenderer>().sharedMaterial = avatarMaterial;
            myPlayer.transform.position = initialPosition;
            myPlayer.transform.localScale = initialScale;
        }


        private void SetPortalManagerPlayer()
        {
            myFPSCamera.tag = "Local Primary Camera";
            GameObject.Find("PortalManager").GetComponent<PortalManager>().SetPlayer(myFPSCamera);
        }
    }
}