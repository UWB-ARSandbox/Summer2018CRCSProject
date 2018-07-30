using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Contains and manages information related to an ASL GameObject's metadata.
    /// This includes the object's name, position, orientation, bounding box, and
    /// PUN owner ID.
    /// 
    /// This class is primarily used for easier synchronization across different
    /// clients connected to the ASL network.
    /// </summary>
    public static class ObjectInfoDatabase
    {
        #region Fields
        #region Public Fields
        /// <summary>
        /// A map associating object names with their ObjectInfoMetadatas. The
        /// metadata stored tracks an object's name, position, orientation, 
        /// bounding box, and PUN owner ID.
        /// </summary>
        public static Dictionary<string, ObjectInfoMetadata> ObjectDatabase;
        #endregion

        #region Private Fields
        /// <summary>
        /// Timestamp tracking the last update to the database. Used to determine
        /// whether an update is appropriate.
        /// </summary>
        private static System.DateTime lastUpdate;
        #endregion
        #endregion

        #region Methods
        #region Static Methods
        /// <summary>
        /// Initializes an ObjectInfoDatabase by initializing the underlying map
        /// and setting the update timestamp.
        /// </summary>
        static ObjectInfoDatabase()
        {
            ObjectDatabase = new Dictionary<string, ObjectInfoMetadata>();
            lastUpdate = System.DateTime.MinValue;
        }

        /// <summary>
        /// Adds a GameObject and its associated metadata to the underlying map.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject that is being added to the underlying map.
        /// </param>
        public static void Add(GameObject go)
        {
            string objectName = go.name;

            int ownerID = 0;
            PhotonView view = go.GetComponent<PhotonView>();
            if(view != null)
            {
                ownerID = view.ownerId;
            }
            ObjectInfoMetadata objectInfo = new ObjectInfoMetadata(go, ownerID);

            if (ObjectDatabase.ContainsKey(objectName))
            {
                ObjectDatabase.Remove(objectName);
            }
            ObjectDatabase.Add(objectName, objectInfo);

            lastUpdate = System.DateTime.Now;
        }

        /// <summary>
        /// Removes a GameObject and its associated metadata from the underlying 
        /// map.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to remove from the underlying map.
        /// </param>
        public static void Remove(GameObject go)
        {
            if (ObjectDatabase.ContainsKey(go.name))
            {
                ObjectDatabase.Remove(go.name);
                lastUpdate = System.DateTime.Now;
            }
        }

        /// <summary>
        /// Retrieve the ObjectInfoMetadata associated with a GameObject by
        /// passing in its name.
        /// </summary>
        /// 
        /// <param name="objectName">
        /// The name of the GameObject whose metadata you want.
        /// </param>
        /// 
        /// <returns>
        /// The ObjectInfoMetadata associated with the GameObject specified.
        /// Returns null if the Object is not stored.
        /// </returns>
        public static ObjectInfoMetadata Get(string objectName)
        {
            if (ObjectDatabase.ContainsKey(objectName))
            {
                return ObjectDatabase[objectName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines if the underlying map contains information on a GameObject 
        /// by the name specified.
        /// </summary>
        /// 
        /// <param name="objectName">
        /// The name of the GameObject you are checking for.
        /// </param>
        /// 
        /// <returns>
        /// True if the GameObject and its metadata are contained in the underlying
        /// map. False otherwise.
        /// </returns>
        public static bool Contains(string objectName)
        {
            return ObjectDatabase.ContainsKey(objectName);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The timestamp of the last update applied to the database.
        /// </summary>
        public static System.DateTime UpdateTime
        {
            get
            {
                return lastUpdate;
            }
        }

        /// <summary>
        /// Boolean if the underlying map is empty.
        /// </summary>
        public static bool Empty
        {
            get
            {
                return ObjectDatabase.Count < 1;
            }
        }

        /// <summary>
        /// The number of objects stored.
        /// </summary>
        public static int Count
        {
            get
            {
                return ObjectDatabase.Count;
            }
        }
        #endregion
        #endregion
    }
}