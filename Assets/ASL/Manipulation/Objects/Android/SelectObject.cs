using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects.Android
{
    /// <summary>
    /// Intended to house the behavior of all object manipulation given an 
    /// Android's control schemes. Should be removed and reworked along with 
    /// overall object control scheme.
    /// </summary>
    public class SelectObject : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// A reference to the active Object Interaction Manager in the scene.
        /// </summary>
        private ObjectInteractionManager objManager;
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when 
        /// this class is enabled. Sets the Object Interaction Manager reference
        /// to the currently active Object Interaction Manager in the scene.
        /// </summary>
        public void Awake()
        {
            objManager = gameObject.GetComponent<ObjectInteractionManager>();
        }

        /// <summary>
        /// Handles selection of an object based on the tap position of a finger.
        /// Translates screen space to a ray that intersects with objects and
        /// returns the first object intersected with.
        /// </summary>
        /// 
        /// <param name="touchInfo">
        /// Information associated with the touch of a tap.
        /// </param>
        /// 
        /// <returns>
        /// The first object intersected with. Null if no objects are 
        /// intersected.
        /// </returns>
        public GameObject Select(Touch touchInfo)
        {
            Camera cam = GameObject.FindObjectOfType<Camera>();
            Vector3 tapPos = touchInfo.position;
            Ray ray = cam.ScreenPointToRay(tapPos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            else
            {
                GameObject camera = GameObject.Find("Main Camera");
                if (camera != null)
                {
                    return camera;
                }
                else
                {
                    Debug.LogError("Cannot find camera object. Selecting null object.");
                    return null;
                }
            }
        }

        /// <summary>
        /// Makes the focus object for manipulation null.
        /// </summary>
        /// <param name="touchInfo"></param>
        public void Unselect(Touch touchInfo)
        {
            objManager.Focus(null);
        }
        #endregion
    }
}