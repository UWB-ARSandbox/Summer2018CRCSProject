using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    /// <summary>
    /// Handles selection of different object as the "focus object" for
    /// manipulation.
    /// </summary>
    /// 
    /// <param name="e">
    /// Arguments associated with the object focus shift.
    /// </param>
    public delegate void ObjectSelectedEventHandler(ObjectSelectedEventArgs e);

    /// <summary>
    /// Holds information regarding the new object to be focused on, the object's
    /// current owner's Photon ID, and the ID of the person focusing on the 
    /// object.
    /// </summary>
    public class ObjectSelectedEventArgs : System.EventArgs
    {
        #region Fields
        /// <summary>
        /// ID of the current owner of the focus object.
        /// </summary>
        private int ownerID;

        /// <summary>
        /// The object being focused on for manipulation.
        /// </summary>
        private GameObject focusObject;

        /// <summary>
        /// ID of the network member focusing on the object.
        /// </summary>
        private int focuserID;
        #endregion

        #region Methods
        /// <summary>
        /// The constructor handling event argument generation. Sets the
        /// locally stored focus object, owner ID, and focuser ID.
        /// </summary>
        /// 
        /// <param name="focusObject">
        /// The object being focused on for manipulation.
        /// </param>
        /// 
        /// <param name="ownerID">
        /// ID of the object's current owner's ID.
        /// </param>
        /// 
        /// <param name="focuserID">
        /// ID of the network member focusing on the object.
        /// </param>
        public ObjectSelectedEventArgs(GameObject focusObject, int ownerID, int focuserID)
        {
            this.ownerID = ownerID;
            this.focusObject = focusObject;
            this.focuserID = focuserID;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Equivalent of a null ObjectSelectedEventArgs.
        /// </summary>
        public new ObjectSelectedEventArgs Empty
        {
            get
            {
                return new ObjectSelectedEventArgs(null, 0, 0);
            }
        }
        /// <summary>
        /// ID of the focus object's current owner.
        /// </summary>
        public int OwnerID
        {
            get
            {
                return ownerID;
            }
        }
        /// <summary>
        /// Object focused on for manipulation.
        /// </summary>
        public GameObject FocusObject
        {
            get
            {
                return focusObject;
            }
        }
        /// <summary>
        /// ID of the network member focusing on the object.
        /// </summary>
        public int FocuserID
        {
            get
            {
                return focuserID;
            }
        }
        #endregion
    }
}