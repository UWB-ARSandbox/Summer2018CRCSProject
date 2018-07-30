using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using ASL.Manipulation.Objects;

using ASL.Avatars.Vive;

namespace ASL.Manipulation.Controllers.Vive
{
    /// <summary>
    /// NOTE: Implemented by previous team before large-scale changes to ASL. 
    /// Applicability of class may be null due to changes in 3rd Party Vive 
    /// library (VRToolkit). Reference Vive library before utilizing this class.
    /// 
    /// Implemented by previous ASL team. Not reviewed and only present for 
    /// archiving purposes.
    /// </summary>
    public class TipTracker : VRTK_ControllerEvents
    {
        #region Fields
        #region Private Fields
        //private ASL.Manipulation.Objects.CreateObject objManager;

        /// <summary>
        /// A reference to the Object Interaction Manager active in the scene.
        /// </summary>
        private ObjectInteractionManager objManager;

        /// <summary>
        /// A reference to the ViveHead active in the scene.
        /// </summary>
        private ViveHead viveHead;

        /// <summary>
        /// A reference to the left hand prefab for the Vive active in the scene.
        /// </summary>
        private ViveLeftHand leftHand;

        /// <summary>
        /// A reference to the right hand prefab for the Vive active in the scene.
        /// </summary>
        private ViveRightHand rightHand;
        #endregion

        #region Public Fields
        /// <summary>
        /// A reference to the object currently selected by the left hand prefab.
        /// </summary>
        public GameObject leftSelectedObject;

        /// <summary>
        /// A reference to the object currently selected by the right hand prefab.
        /// </summary>
        public GameObject rightSelectedObject;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Unity method that is called prior to runtime. Triggers before "Awake" 
        /// methods. Sets Object Interaction Manager to active Object Interaction 
        /// Manager in scene. Associates methods with the TriggerPressed and 
        /// TriggerReleased events. Sets the Vive avatar prefabs to the objects 
        /// currently active in the scene.
        /// </summary>
        public void Start()
        {
            objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();

            TriggerPressed += Select;
            TriggerReleased += Unselect;

            GameObject avatar = GameObject.Find("ViveAvatar");
            if(avatar == null)
            {
                Debug.LogError("Cannot find Vive avatar. Does it exist in the scene?");
            }

            viveHead = avatar.GetComponent<ViveHead>();
            leftHand = avatar.GetComponent<ViveLeftHand>();
            rightHand = avatar.GetComponent<ViveRightHand>();

            leftSelectedObject = null;
            rightSelectedObject = null;
        }

        /// <summary>
        /// Overridden Unity method that is called every frame. Locks selected objects
        /// to the transforms of associated hand prefabs.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if(leftSelectedObject != null)
            {
                Vector3 translation = leftHand.LeftHandPosition + leftHand.LeftHandDirection * 2;
                leftSelectedObject.transform.Translate(translation);
            }
            if(rightSelectedObject != null)
            {
                Vector3 translation = rightHand.RightHandPosition + rightHand.RightHandDirection * 2;
                rightSelectedObject.transform.Translate(translation);
            }
        }

        /// <summary>
        /// Selects an object and locks ownership.
        /// </summary>
        /// <param name="sender">?</param>
        /// <param name="e">?</param>
        public void Select(object sender, ControllerInteractionEventArgs e)
        {
            bool isLeftController;
            GameObject controllerAvatar = GetViveControllerAvatar(e.controllerReference, out isLeftController);

            Transform controllerTip = SetTipTransform(controllerAvatar);
            Vector3 tipPos = controllerTip.position;
            Ray tipRay = new Ray(tipPos, controllerTip.forward);
            RaycastHit hit;
            Physics.Raycast(tipRay, out hit);
            if (hit.collider != null)
            {
                if (isLeftController)
                {
                    leftSelectedObject = hit.collider.gameObject;
                }
                else
                {
                    rightSelectedObject = hit.collider.gameObject;
                }
            }

            //objManager.GetComponent<ObjectInteractionManager>().RequestOwnership(hit.collider.gameObject, PhotonNetwork.player.ID);
            objManager.RequestOwnership(hit.collider.gameObject);
        }

        /// <summary>
        /// Releases ownership of a locked object.
        /// </summary>
        /// <param name="sender">?</param>
        /// <param name="e">?</param>
        public void Unselect(object sender, ControllerInteractionEventArgs e)
        {
            bool isLeftController;
            GameObject controllerAvatar = GetViveControllerAvatar(e.controllerReference, out isLeftController);
            //objManager.GetComponent<ObjectInteractionManager>().Focus(null, PhotonNetwork.player.ID);
            objManager.Focus(null);
            if (isLeftController)
            {
                leftSelectedObject = null;
            }
            else
            {
                rightSelectedObject = null;
            }
        }

        /// <summary>
        /// Returns a GameObject associated with the controller with the given 
        /// controller reference.
        /// </summary>
        /// 
        /// <param name="controllerReference">
        /// ?
        /// </param>
        /// <param name="isLeftController">
        /// Whether you're looking to grab the left controller or the right 
        /// controller.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject used as a prefab for the given controller. Returns 
        /// null if no GameObject is instantiated for that controller.
        /// </returns>
        public GameObject GetViveControllerAvatar(VRTK_ControllerReference controllerReference, out bool isLeftController)
        {
            uint controllerIndex = controllerReference.index;
            GameObject controllerAvatar = null;
            isLeftController = false;
            if (GameObject.Find("ViveLeftHand").GetComponent<ViveLeftHand>().ControllerID == controllerIndex)
            {
                isLeftController = true;
                controllerAvatar = GameObject.Find("ViveLeftHand");
            }
            else if (GameObject.Find("ViveRightHand").GetComponent<ViveLeftHand>().ControllerID == controllerIndex)
            {
                isLeftController = false;
                controllerAvatar = GameObject.Find("ViveRightHand");
            }

            return controllerAvatar;
        }

        /// <summary>
        /// Gets the transform for a Vive controller prefab object by referencing 
        /// the transform of that controller's "Model/tip/attach" object.
        /// </summary>
        /// 
        /// <param name="controllerObj">
        /// The GameObject representing a Vive controller prefab.
        /// </param>
        /// 
        /// <returns>
        /// Returns the transform associated with the controller's tip.
        /// </returns>
        public Transform SetTipTransform(GameObject controllerObj)
        {
            return controllerObj.transform.Find("Model/tip/attach");
        }
        #endregion
    }
}