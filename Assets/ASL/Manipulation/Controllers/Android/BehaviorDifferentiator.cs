using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Controllers.Android
{
    /// <summary>
    /// Serves as a fork for logic that operates on touchscreen pinch behavior 
    /// and tap behavior.
    /// 
    /// NOTE: Additional behaviors may be added as desired, and it is 
    /// recommended that different classes handle given categories of behaviors 
    /// for easy adjustment later.
    /// </summary>
    public class BehaviorDifferentiator : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_EDITOR
        #region Fields
        private ASL.Manipulation.Objects.ObjectInteractionManager objManager;

        #region Android-Specific
        private TapBehavior tapBehavior;
        private PinchBehavior pinchBehavior;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when 
        /// this class is enabled. Sets the Object Interaction Manager 
        /// dynamically and generates Tap and Pinch Behavior classes to 
        /// facilitate manipulation of touchscreen behavior.
        /// </summary>
        public void Awake()
        {
            objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ASL.Manipulation.Objects.ObjectInteractionManager>();
            gameObject.AddComponent<TapBehavior>();
            gameObject.AddComponent<PinchBehavior>();

            // Remove the following AddComponent, separate general PUN creation/synchronization behavior in a different file, and then make sure that objectinteractionmanager script attaches it automatically
            //gameObject.AddComponent<ASL.Manipulation.Objects.CreateObject>();

        }

        /// <summary>
        /// Triggers repeatedly to call for certain behavior to occur when 
        /// taps or pinches are registered on screen.
        /// </summary>
        public void FixedUpdate()
        {
            Touch[] touches = Input.touches;
            if (touches.Length == 1)
            {
                tapBehavior.Handle(touches[0]);
            }
            if (touches.Length == 2)
            {
                pinchBehavior.Handle(touches);
            }
        }
        #endregion
#endif
    }
}