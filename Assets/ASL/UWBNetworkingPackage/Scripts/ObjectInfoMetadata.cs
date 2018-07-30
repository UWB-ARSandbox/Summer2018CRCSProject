using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Encapsulates metadata associated with an ASL GameObject. Used for easier
    /// management of data synchronization across clients.
    /// </summary>
    public class ObjectInfoMetadata
    {
        #region Fields
        /// <summary>
        /// The name of the GameObject being tracked.
        /// </summary>
        public string ObjectName;

        /// <summary>
        /// The position of the GameObject being tracked.
        /// </summary>
        public UnityEngine.Vector3 Position;

        /// <summary>
        /// The orientation of the GameObject being tracked.
        /// </summary>
        public UnityEngine.Quaternion Rotation;

        /// <summary>
        /// The Bounding Box associated with the GameObject being tracked.
        /// </summary>
        public UnityEngine.Bounds BoundingBox;

        #region PUN Stuff
        /// <summary>
        /// The PUN Owner ID of the object beign tracked.
        /// </summary>
        public int OwnerID;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Constructor that generates a metadata object given an ASL GameObject
        /// and its Owner's PUN ID.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="ownerID"></param>
        public ObjectInfoMetadata(GameObject go, int ownerID)
        {
            this.ObjectName = go.name;
            this.Position = go.transform.position;
            this.Rotation = go.transform.rotation;
            if (go.GetComponent<MeshFilter>() != null)
            {
                Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
                if (mesh == null)
                {
                    mesh = go.GetComponent<MeshFilter>().mesh;
                }

                if (mesh != null)
                {
                    this.BoundingBox = mesh.bounds;
                }
            }
            else
            {
                this.BoundingBox = new Bounds();
            }

            this.OwnerID = (ownerID < 1) ? 0 : ownerID; // associate object with scene
        }
        #endregion
    }
}