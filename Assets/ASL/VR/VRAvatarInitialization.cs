using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all classes relating the creation and behaviour of VR players & objects in ASL.
/// </summary>
namespace ASL.VirtualReality
{
    /// <summary>
    /// This script is used to set references between the VR camera rigs and the ASL networked VR avatar.
    /// Uses VRTK follow scripts to synchronize the location of the VR user and their networked representation.
    /// </summary>
    public class VRAvatarInitialization : MonoBehaviour
    {

        /// <summary>
        /// VR Device this avatar will represent.
        /// </summary>
        VRDevice device;

        // References to networked avatar components
        List<GameObject> avatarComponents;

        // The components of the networked VR avatar with a follow script
        private enum componentIndex
        {
            cameraRig,
            HMD,
            LeftController,
            RightController,
            Capsule
        }

        // Not currently used, intended for differentiating between VR user types
        public enum VRDevice
        {
            Vive,
            Simulator,
            WindowsMixedReality,
            Rift
        }

        /// <summary>
        /// Saves references to the VR Avatar components into the list, avatarComponents.
        /// </summary>
        private void Awake()
        {
            avatarComponents = new List<GameObject>();
            avatarComponents.Add(gameObject);
            avatarComponents.Add(transform.Find("Head").gameObject);
            avatarComponents.Add(transform.Find("Left Controller").gameObject);
            avatarComponents.Add(transform.Find("Right Controller").gameObject);
            avatarComponents.Add(transform.Find("Body").gameObject);
        }

        /// <summary>
        /// A Function that links the visible networked avatar for a VR user with its corresponding
        /// components in appropriate camera rig. Should only be run for the owner of the avatar.
        /// </summary>
        /// <param name="d">VR device being used, not currently used in a meaningful manner.</param>
        /// <param name="cameraRig">The highest level component defining the play area of a VR user</param>
        /// <param name="hmd">Gameobject inheriting from camera manipulated by the head mounted display.</param>
        /// <param name="leftController">Game object tracking the world space of the left controller.</param>
        /// <param name="rightController">Game object tracking the wrold space of the right controller.</param>
        /// <param name="body">Game Object tracking the center of the camera rig or below the HMD.</param>
        /// <returns> boolean value indicating whether each follow script was successfully set</returns>
        public bool Initialize(VRDevice d, GameObject cameraRig, GameObject hmd, GameObject leftController, GameObject rightController, GameObject body)
        {
            device = d;

            // only the VR user will have reference to local VR components, photon will handle synchronization for non owners.
            // Possible issue could arise if use case requiring a different user to manipulate the location of the local/owned user.
            if (transform.GetComponent<PhotonView>().isMine)
            {
                GameObject[] vrComponents = { cameraRig, hmd, leftController, rightController, body };

                for (int i = 0; i < vrComponents.Length; i++)
                {
                    //Debug.Log("Setting object to follow: " + vrComponents[i].ToString());
                    avatarComponents[i].GetComponent<VRTK.VRTK_TransformFollow>().gameObjectToFollow = vrComponents[i];
                }

                foreach (GameObject component in avatarComponents)
                {
                    if (component.GetComponent<VRTK.VRTK_TransformFollow>().gameObjectToFollow == null)
                    {
                        return false;
                    }
                    component.GetComponent<VRTK.VRTK_TransformFollow>().enabled = true;
                }
            }
            return true;
        }
    }
}
