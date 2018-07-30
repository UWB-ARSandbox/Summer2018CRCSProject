using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Metadata associated with the instantiation and prefabs of an ASL GameObject.
    /// Used for easier synchronization of scenes between ASL network nodes.
    /// </summary>
    public class ObjectInstantiationMetadata
    {
        #region Fields
        /// <summary>
        /// Metadata associated with an object, not just the instantiation
        /// information. This includes the GameObject's name, the GameObject's 
        /// position, orientation, Bounding Box, and PUN Owner ID.
        /// </summary>
        public ObjectInfoMetadata ObjectInfo;

        /// <summary>
        /// The prefab string associated with the tracked GameObject.
        /// </summary>
        public string PrefabName;

        /// <summary>
        /// The instantiation time of the object. Useful for determining what 
        /// needs to be updated on other clients.
        /// </summary>
        public System.DateTime InstantiationTime;
        #endregion

        /// <summary>
        /// Constructor for generating metadata from an ObjectInfoMetadata and the
        /// associated prefab name.
        /// </summary>
        /// 
        /// <param name="objectInfo">
        /// ObjectInfoMetadata associated with the ASL GameObject associated with
        /// this metadata.
        /// </param>
        /// <param name="prefabName">
        /// The prefab string to associate with this GameObject.
        /// </param>
        /// <param name="currentTime">
        /// The current time to be associated with the instantiation of the ASL
        /// GameObject.
        /// </param>
        public ObjectInstantiationMetadata(ObjectInfoMetadata objectInfo, string prefabName, System.DateTime currentTime)
        {
            this.ObjectInfo = objectInfo;
            this.PrefabName = prefabName;
            this.InstantiationTime = currentTime;
        }
    }
}