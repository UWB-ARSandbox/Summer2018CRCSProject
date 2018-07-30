using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects.Android;

namespace ASL.Manipulation.Controllers.Android
{
    /// <summary>
    /// Handles logic translation of a pinch on an Android touchscreen.
    /// </summary>
    public class PinchBehavior : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_EDITOR
        #region Fields
        private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
        private ZoomBehavior zoomBehavior;
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or 
        /// when this class is enabled. Initializes and sets the associated 
        /// ZoomBehavior object for this class.
        /// </summary>
        public void Awake()
        {
            zoomBehavior = gameObject.GetComponent<ZoomBehavior>();
        }

        /// <summary>
        /// Receives information associated with the touches of a pinch in order
        /// to determine what kind of behavior should be triggered.
        /// 
        /// NOTE: This is where logic branches for custom project stuff.
        /// </summary>
        /// <param name="touchInfos"></param>
        public void Handle(Touch[] touchInfos)
        {
            zoomBehavior.Zoom(touchInfos);
        }
        #endregion
#endif
    }
}