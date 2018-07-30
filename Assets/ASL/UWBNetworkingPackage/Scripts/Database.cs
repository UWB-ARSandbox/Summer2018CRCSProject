using System;
using UnityEngine;
using System.Collections.Generic;
//using HoloToolkit.Unity;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Implemented by previous ASL team. Historically stored room meshes 
    /// transferred from the Hololens which consisted of texture-less meshes 
    /// with vertices, triangles, and vertex normals. Stored only for 
    /// archival purposes and is not currently used.
    /// 
    /// Database is a static class that stores the most recently sent Room Mesh (the Room Mesh is created by a HoloLens),
    /// and allows any classes in the UWBNetworkingPackage to access it
    /// </summary>
    public class Database
    {
        #region Fields
        #region Private Fields'
        /// <summary>
        /// An array that stores the bytes of stored meshes.
        /// </summary>
        private static byte[] _meshes;  // Stores the current Room Mesh data as a serialized byte array
        #endregion

        #region Public Fields
        /// <summary>
        /// Keeps track of the timestamp of the last update for checking when
        /// updates are needed.
        /// </summary>
        public static DateTime LastUpdate = DateTime.MinValue;  // Used for keeping the Room Map up-to-date
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Retrieves the Room Mesh as a deserialized list
        /// </summary>
        /// 
        /// <returns>
        /// An enumerable of Unity meshes. These are deserialized Room Meshes.
        /// </returns>
        public static IEnumerable<Mesh> GetMeshAsList()
        {
            return NetworkingPackage.SimpleMeshSerializer.Deserialize(_meshes);
        }

        /// <summary>
        /// Retrieves the Room Mesh as a serialized byte array
        /// </summary>
        /// <returns>Serialized Room Mesh</returns>
        public static byte[] GetMeshAsBytes()
        {
            return _meshes;
        }

        /// <summary>
        /// Update the currently saved mesh to be the given deserialized Room Mesh
        /// This method will also update the LastUpdate time
        /// </summary>
        /// <param name="newMesh">Deserialized Room Mesh stored in a list</param>
        public static void UpdateMesh(IEnumerable<Mesh> newMesh)
        {
            _meshes = NetworkingPackage.SimpleMeshSerializer.Serialize(newMesh);
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Update the currently saved mesh to be the given serialized Room Mesh
        /// This method will also update the LastUpdate time
        /// </summary>
        /// <param name="newMesh">Serialized Room Mesh stored in a byte array</param>
        public static void UpdateMesh(byte[] newMesh)
        {
            _meshes = newMesh;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Delete the currently held Room Mesh
        /// This method will also update the LastUpdate time
        /// </summary>
        public static void DeleteMesh()
        {
            Debug.Log("Deleting Mesh");
            _meshes = null;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Update the currently saved mesh to add the new mesh
        /// This method will also update the LastUpdate time
        /// </summary>
        /// <param name="newMesh">Serialized Room Mesh stored in a byte array</param>
        public static void AddToMesh(byte[] newMesh)
        {
            int length = newMesh.Length + _meshes.Length;
            byte[] totalMesh = new byte[length];            
            Buffer.BlockCopy(_meshes, 0, totalMesh, 0, _meshes.Length);          
            Buffer.BlockCopy(newMesh, 0, totalMesh, _meshes.Length, newMesh.Length);
            LastUpdate = DateTime.Now;
        }
        #endregion
    }
}