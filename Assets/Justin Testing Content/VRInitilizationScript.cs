using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;
using System;
using ASL.PortalSystem;
using ASL.LocalEventSystem;

namespace ASL
{
    namespace VirtualReality
    {
        /// <summary>
        /// This script is responsible for initilizing all required components for using a VR user
        /// in ASL with a corresponding networked representation.
        /// </summary>
        public class VRInitilizationScript : LocalEventHandler
        {
            // Enum of supported events.
            // Must match order in VRTK_SDKManager
            public enum SupportedDevices
            {
                SteamVR,
                Simulator
            }

            // references to the GameObjects within the active camera rig corresponding to the networked
            // VR avatar for this VR user.
            GameObject cameraRigReference;
            GameObject headMountedDisplayReference;
            GameObject leftControllerReference;
            GameObject rightControllerReference;
            GameObject capsuleBodyReference;

            VRTK.VRTK_SDKManager sdkManager;

            public SupportedDevices DeviceToUse = SupportedDevices.Simulator; // Set for VR SDK to use
            public Vector3 origin; // Starting point VR user

            // References to GameObjects containing the SDK Setup scripts.
            public GameObject simulator;
            public GameObject steamVR;

            // Networked visual representation for this VR user.
            GameObject myVRAvatar;

            private ObjectInteractionManager objectInteractionManager;

            private void Awake()
            {
                objectInteractionManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
                sdkManager = transform.GetComponent<VRTK.VRTK_SDKManager>();
            }

            /// <summary>
            /// This function will attempt to call for the creation of networked representation of a VR player on connecting to a room. 
            /// </summary>
            public override void OnJoinedRoom()
            {
                Debug.Log("VR Initilization script has joined the room");
                InstantiateAvatar();
            }

            /// <summary>
            /// Instantiates an owned networked VR avatar, uses local event manager to signal success or failure for other scripts.
            /// </summary>
            private void InstantiateAvatar()
            {
                Debug.Log("Trying to create an avatar");
                myVRAvatar = objectInteractionManager.InstantiateOwnedObject("Networked VR Avatar");

                if (myVRAvatar)
                {
                    Debug.Log("successfully created");
                    ASLLocalEventManager.Instance.Trigger(myVRAvatar, ASLLocalEventManager.LocalEvents.VRAvatarCreationSucceeded);
                }
                else
                {
                    ASLLocalEventManager.Instance.Trigger(myVRAvatar, ASLLocalEventManager.LocalEvents.VRAvatarCreationFailed);
                }
            }

            protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
            {
                switch (args.MyEvent)
                {
                    // Case to catch unsuccessful creation of a networked avatar
                    case ASLLocalEventManager.LocalEvents.VRAvatarCreationFailed:
                        {
                            InstantiateAvatar();
                            break;
                        }
                    case ASLLocalEventManager.LocalEvents.VRAvatarCreationSucceeded:
                        {
                            transform.GetComponent<VRTK.VRTK_SDKManager>().enabled = true;
                            sdkManager.TryLoadSDKSetup((int)DeviceToUse, false, sdkManager.setups);
                            break;
                        }
                    case ASLLocalEventManager.LocalEvents.SimulatorActivated:
                        {
                            InitializeAvatar();
                            break;
                        }
                    case ASLLocalEventManager.LocalEvents.SteamVRActivated:
                        {
                            InitializeAvatar();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            // Gets the appropraite GameObject references for setting up the VR Avatar and passes the references
            // to the avatar.
            private void InitializeAvatar()
            {
                Debug.Log("initilizing the follow scripts for the case: " + DeviceToUse.ToString());

                GetSimulatorReferencePoints();

                switch (DeviceToUse)
                {
                    case SupportedDevices.Simulator:
                        {
                            myVRAvatar.GetComponent<VRAvatarInitialization>().Initialize(VRAvatarInitialization.VRDevice.Simulator,
                                cameraRigReference, headMountedDisplayReference, leftControllerReference, rightControllerReference, capsuleBodyReference);
                            
                            break;
                        }
                    case SupportedDevices.SteamVR:
                        {
                            myVRAvatar.GetComponent<VRAvatarInitialization>().Initialize(VRAvatarInitialization.VRDevice.WindowsMixedReality,
            cameraRigReference, headMountedDisplayReference, leftControllerReference, rightControllerReference, capsuleBodyReference);

                            break;
                        }
                }
                GameObject.Find("PortalManager").GetComponent<PortalManager>().SetPlayer(GameObject.FindGameObjectWithTag("Local Primary Camera"));
            }

            // This function is called in response to the VR SDK Setup script being enabled.
            // Once enabled the VR avatar reference objects can be located for the purpose of the
            // VRAvatarInitialization.Initialize function.
            private void GetSimulatorReferencePoints()
            {
                switch (DeviceToUse)
                {
                    case SupportedDevices.Simulator:
                        {
                            cameraRigReference = GameObject.FindGameObjectWithTag("Simulator Camera Rig");
                            headMountedDisplayReference = GameObject.FindGameObjectWithTag("Simulator HMD");
                            leftControllerReference = GameObject.FindGameObjectWithTag("Simulator Left Controller");
                            rightControllerReference = GameObject.FindGameObjectWithTag("Simulator Right Controller");
                            capsuleBodyReference = GameObject.FindGameObjectWithTag("Simulator Capsule");
                            return;
                        }
                    case SupportedDevices.SteamVR:
                        {
                            cameraRigReference = GameObject.FindGameObjectWithTag("SteamVR Camera Rig");
                            headMountedDisplayReference = GameObject.FindGameObjectWithTag("StreamVR HMD");
                            leftControllerReference = GameObject.FindGameObjectWithTag("SteamVR Left Controller");
                            rightControllerReference = GameObject.FindGameObjectWithTag("SteamVR Right Controller");
                            capsuleBodyReference = GameObject.FindGameObjectWithTag("SteamVR Capsule");
                            return;
                        }
                }
            }
        }
    }
}

