using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects.Android
{
    /// <summary>
    /// Handles behaviors associated with the phenomena of "zooming".
    /// </summary>
    public class ZoomBehavior : MonoBehaviour
    {
        #region Fields
        #region Private Fields
        /// <summary>
        /// A reference to the active Object Interaction Manager of the scene.
        /// </summary>
        private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
        #endregion

        #region Public Fields
        /// <summary>
        /// The multiplicative rate at which zoom occurs for a perspective camera 
        /// view (i.e. for a traditional camera view).
        /// </summary>
        public float perspectiveZoomSpeed = 0.25f;

        /// <summary>
        /// The multiplicative rate at which zoom occurs for an orthogonal camera
        /// view.
        /// </summary>
        public float orthographicZoomSpeed = 0.5f;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Handles the logic associated with zooming by measuring the change in
        /// distance magnitude between touches.
        /// </summary>
        /// <param name="touchInfos"></param>
        public void Zoom(Touch[] touchInfos)
        {
            if(touchInfos.Length == 2)
            {
                Touch touchZero = touchInfos[0];
                Touch touchOne = touchInfos[1];

                Vector2 touchZeroPrev = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrev = touchOne.position - touchOne.deltaPosition;

                float startingMagnitude = (touchZeroPrev - touchOnePrev).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float delta = startingMagnitude - currentMagnitude;

                Camera cam = GameObject.FindObjectOfType<Camera>();
                if(cam != null)
                {
                    cam.fieldOfView += delta * perspectiveZoomSpeed;
                    cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, .1f, 179.9f);
                }
            }
        }
        #endregion
    }
}