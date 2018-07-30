using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// A database tracking ASL object generation and destruction to keep track
    /// of which items should be synchronized to new clients. Though not extended
    /// properly to do so, this class can easily be used to synchronize players 
    /// that connect at different points in time.
    /// </summary>
    public static class ObjectInstantiationDatabase
    {
        #region Fields
        #region Public Fields
        /// <summary>
        /// Maps an ASL GameObject's name to a tuple pairing that GameObject 
        /// with its metadata. The pairing is necessary because multiple 
        /// GameObjects can have the same name.
        /// </summary>
        public static Dictionary<string, List<KeyValuePair<ObjectInstantiationMetadata, GameObject>>> ObjectDatabase;

        /// <summary>
        /// Maps GameObjects to their prefab names. This allows for easy reference
        /// when trying to generate copies of that GameObject from an existing
        /// object in the scene.
        /// </summary>
        public static Dictionary<GameObject, string> PrefabLookupTable;
        #endregion

        #region Private Fields
        /// <summary>
        /// Timestamp keeping track of the last update applied to the database.
        /// Used to determine whether an update is appropriate.
        /// </summary>
        private static System.DateTime lastUpdate;
        #endregion
        #endregion

        #region Methods
        #region Static Methods
        /// <summary>
        /// Constructor that initializes underlying items.
        /// </summary>
        static ObjectInstantiationDatabase()
        {
            //ObjectDatabase = new Dictionary<string, ObjectInstantiationMetadata>();
            ObjectDatabase = new Dictionary<string, List<KeyValuePair<ObjectInstantiationMetadata, GameObject>>>();
            PrefabLookupTable = new Dictionary<GameObject, string>();
            lastUpdate = System.DateTime.MinValue;
        }

        /// <summary>
        /// Associate an ASL GameObject with a given prefab name.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The prefab name to associate with the GameObject being tracked.
        /// </param>
        /// <param name="go">
        /// The GameObject to track.
        /// </param>
        public static void Add(string prefabName, GameObject go)
        {
            string objectName = go.name;

            int ownerID = 0;
            PhotonView view = go.GetComponent<PhotonView>();
            if (view != null)
            {
                ownerID = view.ownerId;
            }
            ObjectInfoMetadata objectInfo = new ObjectInfoMetadata(go, ownerID);

            System.DateTime currentTime = System.DateTime.Now;

            ObjectInstantiationMetadata instantiationMetadata = new ObjectInstantiationMetadata(objectInfo, prefabName, currentTime);
            
            if (!ObjectDatabase.ContainsKey(objectName))
            {
                ObjectDatabase.Add(objectName, new List<KeyValuePair<ObjectInstantiationMetadata, GameObject>>());
            }

            ObjectDatabase[objectName].Add(new KeyValuePair<ObjectInstantiationMetadata, GameObject>(instantiationMetadata, go));
            PrefabLookupTable.Add(go, prefabName);

            lastUpdate = System.DateTime.Now;
        }

        /// <summary>
        /// Remove an ASL GameObject from being tracked. This should be called 
        /// when the ASL GameObject is no longer relevant to the project and doesn't
        /// need to be synchronized across clients.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to be removed from tracking.
        /// </param>
        /// 
        /// <returns>
        /// True if the GameObject was being tracked and is no longer being 
        /// tracked. Also returns true if the GameObject was not being tracked
        /// anyways. Returns false otherwise.
        /// </returns>
        public static bool Remove(GameObject go)
        {
            if(go == null)
            {
                return false;
            }
            else if (ObjectDatabase.ContainsKey(go.name))
            {
                bool removed = false;

                string objectName = go.name;
                List<KeyValuePair<ObjectInstantiationMetadata, GameObject>> valuePairList = ObjectDatabase[objectName];
                foreach(KeyValuePair<ObjectInstantiationMetadata, GameObject> valuePair in valuePairList)
                {
                    if (valuePair.Value.Equals(go))
                    {
                        ObjectDatabase[objectName].Remove(valuePair);
                        removed = true;
                        if (PrefabLookupTable.ContainsKey(go))
                        {
                            PrefabLookupTable.Remove(go);
                        }
                        lastUpdate = System.DateTime.Now;
                        break;
                    }
                }

                return removed;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Remove an ASL GameObject from being tracked. This differs from
        /// the standard Remove method in that it searches for an object with
        /// the same name if the GameObject requested for removal doesn't
        /// exist in the underlying tracking structures.
        /// 
        /// Used to safeguard against having to deal with GameObjects that
        /// are destroyed or nulled out before they're properly dealt with by
        /// this class.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to remove.
        /// </param>
        /// <param name="goName">
        /// The name of the GameObject being used as backup.
        /// </param>
        /// 
        /// <returns>
        /// True if the GameObject was being tracked and is no longer being
        /// tracked. Also returns true if the GameObject was not being tracked
        /// anyways. Also returns true if a GameObject with the matching name was
        /// removed. Returns false otherwise.
        /// 
        /// </returns>
        public static bool Remove(GameObject go, string goName)
        {
            if (string.IsNullOrEmpty(goName))
            {
                return false;
            }
            else if(go != null)
            {
                return Remove(go);
            }
            else if (ObjectDatabase.ContainsKey(goName))
            {
                bool removed = false;
                List<KeyValuePair<ObjectInstantiationMetadata, GameObject>> valuePairList = ObjectDatabase[goName];
                foreach (KeyValuePair<ObjectInstantiationMetadata, GameObject> valuePair in valuePairList)
                {
                    if (valuePair.Value == null)
                    {
                        ObjectDatabase[goName].Remove(valuePair);
                        removed = true;
                        lastUpdate = System.DateTime.Now;
                    }
                }

                if (PrefabLookupTable.ContainsKey(null))
                {
                    PrefabLookupTable.Remove(null);
                }

                return removed;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Get the ObjectInstantiationMetadata associated with a GameObject, if it's
        /// being tracked.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to retrieve metadata for.
        /// </param>
        /// 
        /// <returns>
        /// The ObjectInstantiationMetadata associated with the ASL GameObject (if
        /// the ASL GameObject is being tracked).
        /// </returns>
        public static ObjectInstantiationMetadata Get(GameObject go)
        {
            if (go != null)
            {
                string objectDatabaseKey = go.name;
                if (ObjectDatabase.ContainsKey(objectDatabaseKey))
                {
                    List<KeyValuePair<ObjectInstantiationMetadata, GameObject>> pairList = ObjectDatabase[objectDatabaseKey];
                    foreach (KeyValuePair<ObjectInstantiationMetadata, GameObject> pair in pairList)
                    {
                        if (pair.Value.Equals(go))
                        {
                            return pair.Key;
                        }
                    }
                }

                UnityEngine.Debug.LogError("Object Instantiation metadata not found!");
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get all ObjectInstantiationMetadatas currently tracked by this class
        /// that are associated with a GameObject sharing the name used for the 
        /// request.
        /// </summary>
        /// 
        /// <param name="objectName">
        /// The name of the ASL GameObject for which you want all instantiation
        /// metadata.
        /// </param>
        /// 
        /// <returns>
        /// A list of ObjectInstantiationMetadata for all GameObjects tracked that
        /// possess a name that is the same as the one passed in.
        /// </returns>
        public static List<ObjectInstantiationMetadata> GetAll(string objectName)
        {
            List<ObjectInstantiationMetadata> metadataList = new List<ObjectInstantiationMetadata>();
            if (ObjectDatabase.ContainsKey(objectName))
            {
                List<KeyValuePair<ObjectInstantiationMetadata, GameObject>> pairList = ObjectDatabase[objectName];
                foreach(KeyValuePair<ObjectInstantiationMetadata, GameObject> pair in pairList)
                {
                    metadataList.Add(pair.Key);
                }
            }

            return metadataList;
        }

        /// <summary>
        /// Get the prefab name associated with an ASL GameObject.
        /// </summary>
        /// 
        /// <param name="go">
        /// The ASL GameObject used for the query.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the prefab name associated with the ASL
        /// GameObject.
        /// </returns>
        public static string GetPrefabName(GameObject go)
        {
            if(go != null)
            {
                if (PrefabLookupTable.ContainsKey(go))
                {
                    return PrefabLookupTable[go];
                }
            }

            UnityEngine.Debug.LogError("GameObject prefab name not found.");
            return null;
        }

        /// <summary>
        /// Determine if an ASL GameObject is tracked that has the name passed
        /// in.
        /// </summary>
        /// 
        /// <param name="objectName">
        /// The name to search for.
        /// </param>
        /// 
        /// <returns>
        /// True if an ASL GameObject is being tracked that has the name queried.
        /// False otherwise.
        /// </returns>
        public static bool Contains(string objectName)
        {
            return ObjectDatabase.ContainsKey(objectName);
        }

        #region Properties
        /// <summary>
        /// Timestamp for determining the most recent update of the database.
        /// </summary>
        public static System.DateTime UpdateTime
        {
            get
            {
                return lastUpdate;
            }
        }

        /// <summary>
        /// Determines if the count of tracked ASL GameObjects is zero.
        /// </summary>
        public static bool Empty
        {
            get
            {
                return ObjectDatabase.Count < 1;
            }
        }
        #endregion
        #endregion
        #endregion
    }
}