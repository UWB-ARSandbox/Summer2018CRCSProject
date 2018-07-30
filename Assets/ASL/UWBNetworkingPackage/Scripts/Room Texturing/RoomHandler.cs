using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Handles logic associated with generating Hololens Scanned Rooms from
    /// text files and resources. Interacts with the RoomManager script to
    /// specify which room to work with.
    /// </summary>
    public class RoomHandler : MonoBehaviour
    {
        /// <summary>
        /// Unity method that is called prior to runtime. Triggers before "Awake"
        /// methods. Registers the GenerateRoomFolder method to the 
        /// RoomObject.Changed event.
        /// </summary>
        public static void Start()
        {
            UWB_Texturing.Config.RoomObject.Changed += GenerateRoomFolder;
        }

        /// <summary>
        /// Triggers the deciphering of the text files used to generate the
        /// resources required to generate all Hololens Scanned Rooms. Also
        /// uses those resources to generate GameObjects and prefabs for
        /// the Scanned Rooms.
        /// </summary>
        public static void ProcessAllRooms()
        {
            string[] roomNames = GetRoomNames();

            for (int i = 0; i < roomNames.Length; i++)
            {
                string roomName = roomNames[i];
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

                CreateRoomResources(roomName);
                InstantiateRoom(roomName);
            }
        }

        /// <summary>
        /// Get the Hololens Scanned Room names that are stored on this ASL node.
        /// </summary>
        /// 
        /// <returns>
        /// An array of strings representing the Hololens Scanned Rooms.
        /// </returns>
        public static string[] GetRoomNames()
        {
            //string[] roomNames = Directory.GetDirectories(Config_Base.CompileAbsoluteRoomDirectory());
            string[] roomNames = Directory.GetDirectories(Config.Current.Room.CompileAbsoluteAssetDirectory());
            for (int i = 0; i < roomNames.Length; i++)
            {
                string pass1 = roomNames[i].Split('/')[roomNames[i].Split('/').Length];
                string roomName = pass1.Split('\\')[pass1.Split('\\').Length];
                roomNames[i] = roomName;
            }
            return roomNames;
        }

        /// <summary>
        /// Wrapper method for creating resources from text files for a Hololens
        /// Scanned Room.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the Hololens Scanned Room to generate resources for.
        /// </param>
        public static void CreateRoomResources(string roomName)
        {
            //string matrixArrayFilepath = Config.AssetBundle.Current.CompileAbsoluteRoomPath(UWB_Texturing.Config.MatrixArray.CompileFilename(), roomName);
            //string materialsDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
            //string meshesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
            //string texturesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
            //string imagesDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
            UWB_Texturing.BundleHandler.CreateRoomResources();
        }

        /// <summary>
        /// Wrapper method for creating the Hololens Scanned Room 
        /// GameObject from the associated resources.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the Hololens Scanned Room to instantiate.
        /// </param>
        public static void InstantiateRoom(string roomName)
        {
            //string rawResourceBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
            string rawResourceBundlePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
            UWB_Texturing.BundleHandler.InstantiateRoom(rawResourceBundlePath);
        }

        //public static void InstantiateRoomFromResources(string roomName)
        //{
        //    string matrixArrayFilepath = Config.AssetBundle.Current.CompileAbsoluteRoomPath(UWB_Texturing.Config.MatrixArray.CompileFilename(), roomName);
        //    string rawRoomBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
        //    string customMatricesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
        //    string customOrientationDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
        //    string customMeshesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
        //    string textureImagesDestinationDirectory = Config.AssetBundle.Current.CompileAbsoluteRoomDirectory(roomName);
        //    UWB_Texturing.BundleHandler.InstantiateRoomFromResources(roomName, rawRoomBundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, textureImagesDestinationDirectory, matrixArrayFilepath);
        //}

        /// <summary>
        /// Creates the directory for a Hololens Scanned Room if it does not 
        /// already exist in the Resources folder.
        /// </summary>
        /// 
        /// <param name="e">
        /// Information associated with the event associated with the room name
        /// changing in the RoomManager object of the scene.
        /// </param>
        public static void GenerateRoomFolder(UWB_Texturing.RoomNameChangedEventArgs e)
        {
            string roomName = e.NewName;
            //string roomDirectory = Config.Room.CompileAbsoluteRoomDirectory(roomName);
            string roomDirectory = Config.Current.Room.CompileAbsoluteAssetDirectory(roomName);
            if (!Directory.Exists(roomDirectory))
            {
                AbnormalDirectoryHandler.CreateDirectory(roomDirectory);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Packs an asset bundle for a Hololens Scanned Room that contains its
        /// prefab.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the Hololens Scanned Room to pack an asset bundle for.
        /// </param>
        public static void PackRoomBundle(string roomName)
        {
            // Have to generate Android bundle first or the PC one will be overwritten
            // with the Android one (i.e. deleted) because of how this hooks into the 
            // RoomTexture module
            //string roomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            RoomManager.UpdateRoomBundle(roomName);
            UWB_Texturing.BundleHandler.PackFinalRoomBundle(BuildTarget.Android);
            string AndroidBundlePath = Config.Android.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename(), roomName);
            if (File.Exists(AndroidBundlePath))
            {
                File.Delete(AndroidBundlePath);
            }
            File.Copy(GeneratedBundlePath, AndroidBundlePath);
            UWB_Texturing.BundleHandler.PackFinalRoomBundle(BuildTarget.StandaloneWindows);
        }

        /// <summary>
        /// Pack asset bundles for all Hololens Scanned Rooms on this ASL node.
        /// </summary>
        public static void PackAllRoomBundles()
        {
            string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;

            string[] roomNames = RoomManager.GetAllRoomNames();
            foreach(string roomName in roomNames)
            {
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
                PackRoomBundle(roomName);
            }

            UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
        }

        //public static void PackRoomBundle(BuildTarget targetPlatform)
        //{
        //    //string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();
        //    string destinationDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
        //    //UWB_Texturing.BundleHandler.PackFinalRoomBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
        //    UWB_Texturing.BundleHandler.PackFinalRoomBundle(targetPlatform);
        //}

        /// <summary>
        /// Pack asset bundle for a Hololens Scanned Room that contains the 
        /// resources associated with the room. This includes textures,
        /// meshes, and materials. Does not include the raw text files from
        /// which the resources are derived from.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the Hololens Scanned Room to pack the asset bundle for.
        /// </param>
        public static void PackRawRoomResourceBundle(string roomName)
        {
            string roomNAme = UWB_Texturing.Config.RoomObject.GameObjectName;
            RoomManager.UpdateRawRoomBundle(roomName);
            UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(BuildTarget.Android);
            string AndroidBundlePath = Config.Android.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename(), roomName);
            if (File.Exists(AndroidBundlePath))
            {
                File.Delete(AndroidBundlePath);
            }
            File.Copy(GeneratedBundlePath, AndroidBundlePath);
            UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(BuildTarget.StandaloneWindows);
        }

        /// <summary>
        /// Pack asset bundles for all Hololens Scanned Rooms that contains the
        /// resources associated with each room. This includes textures, meshes,
        /// and materials. Does not include the raw text files from which the
        /// resources are derived from.
        /// </summary>
        public static void PackAllRawRoomResourceBundles()
        {
            string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;

            string[] roomNames = RoomManager.GetAllRoomNames();
            foreach (string roomName in roomNames)
            {
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
                PackRawRoomResourceBundle(roomName);
            }

            UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
        }
#endif
    }
}