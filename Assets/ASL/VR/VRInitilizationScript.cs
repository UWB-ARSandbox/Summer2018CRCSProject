using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;
using System;
using ASL.PortalSystem;
using ASL.LocalEventSystem;

/// <summary>
/// Contains all classes relating the creation and behaviour of VR players & objects in ASL.
/// </summary>
namespace ASL.VirtualReality
{

    /// <summary>
    /// This script is responsible for initilizing all required components for using a VR user
    /// in ASL with a corresponding networked representation.
    /// </summary>
    public class VRInitilizationScript : LocalEventHandler
    {
        /// <summary>Enum of supported events.</summary>
        /// <important>Must match order in VRTK_SDKManager</important>
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

        /// <summary>
        /// The VR device to be used by the player.
        /// </summary>
        public SupportedDevices DeviceToUse = SupportedDevices.Simulator; // Set for VR SDK to use
        /// <summary>
        /// Starting position for the VR user
        /// </summary>
        public Vector3 origin; // Starting point VR user

        // References to GameObjects containing the SDK Setup scripts.
        /// <summary>
        /// Reference to the Simulator SDK setup
        /// </summary>
        public GameObject simulator;
        /// <summary>
        /// Reference to the SteamVR SDK setup
        /// </summary>
        public GameObject steamVR;

        // Networked visual representation for this VR user.
        GameObject myVRAvatar;

        private ObjectInteractionManager objectInteractionManager;

        /// <summary>
        /// Getting initial required references.
        /// </summary>
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


        /// <summary>
        /// Event handler for local events relating to the creation of a VR player.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="args">The event being triggered</param>
        /// <event>VRAvatarCreationFailed</event>
        /// <description>This event is handled by attempting to create an avatar until successful.</description>
        /// <event>VRAvatarCreationSucceeded</event>
        /// <description>This event is triggered after the VR avatar is sucessfully created across PUN.
        /// Once created, the VRTK scripts will be enabled the appropriate SDK loaded.</description>
        /// <event>SimulatorActivated</event>
        /// <description>This event is triggered after the simulator has been activated, triggers the creation of
        /// a VR player avatar.</description>
        /// <event>SteamVRActivated</event>
        /// <description>This event is triggered after the simulator has been activated, triggers the creation of
        /// a VR player avatar.</description>
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

        /// <summary>
        /// Gets the appropriate GameObject references for setting up the VR Avatar and passes the references
        /// to the avatar.
        /// </summary>
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

            cameraRigReference.transform.position = origin;
            GameObject.Find("PortalManager").GetComponent<PortalManager>().SetPlayer(GameObject.FindGameObjectWithTag("Local Primary Camera"));
        }

        /// <summary>
        /// This function is called in response to the VR SDK Setup script being enabled.
        /// Once enabled the VR avatar reference objects can be located for the purpose of the
        /// VRAvatarInitialization.Initialize function.
        /// </summary>
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

