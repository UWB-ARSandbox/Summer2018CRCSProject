using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects.Android;

namespace ASL.Manipulation.Controllers.Android
{
    /// <summary>
    /// Handles logic translation of a tap on an Android touchscreen.
    /// </summary>
    public class TapBehavior : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_EDITOR
        #region Fields
        private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
        private SelectObject selectBehavior;
        private MoveBehavior moveBehavior;

        private Vector3 touchBeginPosition;
        private Vector3 touchEndPosition;
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when 
        /// this class is enabled. Initializes and sets the associated Object Interaction
        /// Manager and SelectBehavior objects for this class.
        /// </summary>
        public void Awake()
        {
            objManager = gameObject.GetComponent<ASL.Manipulation.Objects.ObjectInteractionManager>();
            selectBehavior = gameObject.GetComponent<SelectObject>();
        }

        /// <summary>
        /// Receives information associated with the touch of a tap in order to 
        /// determine what kind of behavior should be triggered.
        /// 
        /// NOTE: This is where logic branches for custom project stuff.
        /// </summary>
        /// <param name="touchInfo"></param>
        public void Handle(Touch touchInfo)
        {
            switch (touchInfo.phase)
            {
                case TouchPhase.Began:
                    touchBeginPosition = touchInfo.position;
                    break;
                case TouchPhase.Stationary:
                    GameObject go = selectBehavior.Select(touchInfo);
                    objManager.RequestOwnership(go);
                    break;
                case TouchPhase.Moved:
                    moveBehavior.Drag(touchInfo.deltaPosition);
                    break;
                case TouchPhase.Ended:
                    touchEndPosition = touchInfo.position;
                    selectBehavior.Unselect(touchInfo);
                    break;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Where on-screen an ongoing Tap began.
        /// </summary>
        public Vector3 TouchBegin
        {
            get
            {
                return touchBeginPosition;
            }
        }
        /// <summary>
        /// Where on-screen the last ongoing Tap ended.
        /// </summary>
        public Vector3 TouchEnd
        {
            get
            {
                return touchEndPosition;
            }
        }
#endregion
#endif
    }
}