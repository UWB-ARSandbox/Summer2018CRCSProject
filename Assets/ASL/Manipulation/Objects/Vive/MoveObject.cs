using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects.Vive
{
    /// <summary>
    /// Handles Vive-specific move behavior logic.
    /// </summary>
    public class MoveObject : ASL.Manipulation.Objects.MoveBehavior
    {
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when 
        /// this class is enabled. Sets the movement rate.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            MoveScale = 0.25f;
        }
    }
}