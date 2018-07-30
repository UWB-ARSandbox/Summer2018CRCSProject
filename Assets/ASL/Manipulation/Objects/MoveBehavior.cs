using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    /// <summary>
    /// Encapsulates behavior associated with any kind of movement.
    /// </summary>
    public class MoveBehavior : MonoBehaviour
    {
        #region Fields
        public GameObject focusObject;
        private float moveScale = 0.10f;
        private float rotateScale = 15.0f;
        #endregion

        #region Methods
        #region Public Methods
        /// <summary>
        /// Triggers when explicitly called. Sets ObjectInteractionManager for 
        /// this class to the active ObjectInteractionManager in the scene.
        /// </summary>
        public virtual void Awake()
        {
            GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>().FocusObjectChangedEvent += SetObject;
        }
        
        /// <summary>
        /// Generic behavior for moving the focus object up.
        /// </summary>
        public virtual void Up()
        {
            if(focusObject != null)
            {
                focusObject.transform.Translate(Vector3.up * MoveScale);
            }
        }

        /// <summary>
        /// Generic behavior for moving the focus object down.
        /// </summary>
        public virtual void Down()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.down * MoveScale);
            }
        }

        /// <summary>
        /// Generic behavior for moving the focus object left.
        /// </summary>
        public virtual void Left()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.left * MoveScale);
            }
        }

        /// <summary>
        /// Generic behavior for moving the focus object right.
        /// </summary>
        public virtual void Right()
        {
            if (focusObject != null)
            {
                focusObject.transform.Translate(Vector3.right * MoveScale);
            }
        }

        /// <summary>
        /// Generic behavior for rotating focus object clockwise.
        /// </summary>
        public virtual void RotateClockwise()
        {
            if(focusObject != null)
            {
                focusObject.transform.Rotate(Vector3.up, RotateScale);
            }
        }

        /// <summary>
        /// Generic behavior for rotating focus object counterclockwise.
        /// </summary>
        public virtual void RotateCounterClockwise()
        {
            if(focusObject != null)
            {
                focusObject.transform.Rotate(Vector3.up, RotateScale * -1);
            }
        }

        /// <summary>
        /// Generic behavior for dragging a focus object to a specified position.
        /// </summary>
        /// <param name="deltaPosition"></param>
        public virtual void Drag(Vector3 deltaPosition)
        {
            if(focusObject != null)
            {
                focusObject.transform.Translate(deltaPosition);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the object that is getting moved by this script.
        /// </summary>
        /// <param name="e"></param>
        private void SetObject(ObjectSelectedEventArgs e)
        {
            focusObject = e.FocusObject;
        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// The multiplicative factor by which movement is amplified.
        /// </summary>
        public float MoveScale
        {
            get
            {
                return moveScale;
            }
            set
            {
                if (value > 0.0f)
                {
                    moveScale = value;
                }
            }
        }

        /// <summary>
        /// The multiplicative factor by which rotation is amplified.
        /// </summary>
        public float RotateScale
        {
            get
            {
                return rotateScale;
            }
            set
            {
                if(value > 0.0f)
                {
                    rotateScale = value;
                }
            }
        }
#endregion
    }
}