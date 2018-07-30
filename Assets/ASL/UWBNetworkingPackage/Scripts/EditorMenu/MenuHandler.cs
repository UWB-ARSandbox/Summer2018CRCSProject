using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Handles the logic between the Menu class and the methods that are used 
    /// to handle asset bundles for the Hololens Scanned Rooms.
    /// </summary>
    public class MenuHandler : MonoBehaviour
    {
//        private static PhotonView photonView;

//        public void Start()
//        {
//            //photonView = PhotonView.Get(GameObject.Find("NetworkManager").GetComponent<PhotonView>());
//            photonView = PhotonView.Find(1);
//            if (photonView == null)
//                Debug.Log("Wrong photon view id given");
//            // ERROR TESTING
//        }

//#if UNITY_EDITOR
//        public static void PackRawResourcesBundle()
//        {
//            //string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();
//            //UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
//            UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(BuildTarget.StandaloneWindows);
//        }

//        public static void PackRoomBundle()
//        {
//            //string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();
//            //UWB_Texturing.BundleHandler.PackFinalRoomBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
//            UWB_Texturing.BundleHandler.PackFinalRoomBundle(BuildTarget.StandaloneWindows);
//        }
//#endif

//        public static void ExportRawResources(int targetID)
//        {
//            //string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
//            string filepath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename());
//            int rawRoomPort = Config.Ports.RoomResourceBundle_ClientToServer;
//#if !UNITY_WSA_10_0
//            SocketClient_PC.SendFile(ServerFinder.serverIP, rawRoomPort, filepath);
//            Debug.Log("Exporting raw room resources not currently implemented correctly! Doesn't consider target ID and just sends to master");
//#elif !UNITY_EDITOR && UNITY_WSA_10_0
//            SocketClient_Hololens.SendFile(ServerFinder_Hololens.serverIP, rawRoomPort, filepath);
//#endif
//        }


        //public static void ProcessRoomResources()
        //{
        //    string roomName = UWB_Texturing.Config.RoomObject.GameObjectName;
        //    //string customOrientationFilepath = Config.AssetBundle.Current.CompileAbsoluteAssetPath(UWB_Texturing.Config.CustomOrientation.CompileFilename());
        //    string customOrientationFilepath = UWB_Texturing.Config.CustomOrientation.CompileAbsoluteAssetPath(UWB_Texturing.Config.CustomOrientation.CompileFilename(), roomName);
        //    string unityMeshesRelativeDirectory = Config.AssetBundle.Current.AssetSubFolder;
        //    string materialsRelativeDirectory = Config.AssetBundle.Current.AssetSubFolder;
        //    if (File.Exists(customOrientationFilepath))
        //    {
        //        // Build room object
        //        string[] customOrientationFileLines = File.ReadAllLines(customOrientationFilepath);
        //        UWB_Texturing.RoomModel.BuildRoomObject(roomName, customOrientationFileLines, unityMeshesRelativeDirectory, materialsRelativeDirectory);
        //    }
        //    else
        //    {
        //        Debug.Log("Unable to build room!");
        //    }
        //    //UWB_Texturing.RoomModel.BuildRoomObject(File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename())));
        //}


        #region Instantiate
        /// <summary>
        /// Instantiates a Hololens Scanned Room specified by the RoomManager.
        /// </summary>
        public static void InstantiateRoom()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            string rawResourceBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename(), roomName);

            UWB_Texturing.BundleHandler.InstantiateRoom(rawResourceBundlePath);
        }

        /// <summary>
        /// Instantiates all Hololens Scanned Rooms stored on this node.
        /// </summary>
        public static void InstantiateAllRooms()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.InstantiateAllRooms();
        }
        #endregion

        #region Prefabs
        /// <summary>
        /// Creates a prefab for the Hololens Scanned Room specified by the
        /// Room Manager.
        /// </summary>
        public static void InstantiateRoomPrefab()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            GameObject room = GameObject.Find(roomName);
            UWB_Texturing.PrefabHandler.CreateRoomPrefab(room);
        }

        /// <summary>
        /// Creates prefabs for all Hololens Scanned Rooms stored on this
        /// node.
        /// </summary>
        public static void InstantiateAllRoomPrefabs()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.PrefabHandler.CreateAllRoomPrefabs();
        }
        #endregion

        #region Delete
        /// <summary>
        /// Destroys a GameObject representing the Hololens Scanned Room
        /// specified by the RoomManager.
        /// </summary>
        public static void RemoveRoomObject()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveRoomObject(roomName);
        }

        /// <summary>
        /// Destroys all GameObjects representing Hololens Scanned Rooms
        /// in the current scene.
        /// </summary>
        public static void RemoveAllRoomObjects()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveAllRoomObjects();
        }

        /// <summary>
        /// Removes the prefab of the Hololens Scanned Room specified by the
        /// RoomManager from this ASL node.
        /// </summary>
        public static void RemoveRoomPrefab()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.PrefabHandler.DeletePrefab(roomName);
        }

        /// <summary>
        /// Removes all prefabs of Hololens Scanned Rooms from this ASL node.
        /// </summary>
        public static void RemoveAllRoomPrefabs()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.PrefabHandler.DeleteAllPrefabs();
        }

        /// <summary>
        /// Removes all resources for the Hololens Scanned Room specified by
        /// the RoomManager from this ASL node. (Textures, materials, meshes.)
        /// Does not destroy the text files used to generate the resources.
        /// </summary>
        public static void RemoveRoomResources()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveRoomResources(roomName);
        }

        /// <summary>
        /// Removes all resources for all Hololens Scanned Rooms for this ASL
        /// node. (Textures, materials, meshes.) Does not destroy the text
        /// files used to generate the resources.
        /// </summary>
        public static void RemoveAllRoomResources()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveAllRoomResources();
        }

        /// <summary>
        /// Removes all text files for the Hololens Scanned Rooms specified by
        /// the RoomManager for this ASL node. Does not destroy the resources
        /// for the Room.
        /// </summary>
        public static void RemoveRoomRawInfo()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveRawInfo(roomName);
        }

        /// <summary>
        /// Removes all text files for all Hololens Scanned Rooms. Does not
        /// destroy the resources for the rooms.
        /// </summary>
        public static void RemoveAllRoomRawInfo()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveAllRawInfo();
        }

        /// <summary>
        /// Removes everything related to the Hololens Scanned Room specified
        /// by the Room Manager from this ASL node.
        /// </summary>
        public static void RemoveEverything()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveRoomObject(roomName);
            UWB_Texturing.BundleHandler.RemoveRoomResources(roomName);
            UWB_Texturing.BundleHandler.RemoveRawInfo(roomName);
        }

        /// <summary>
        /// Remove everything related to all Hololens Scanned Rooms from this
        /// ASL node.
        /// </summary>
        public static void RemoveAllEverything()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.RemoveAllRoomObjects();
            UWB_Texturing.BundleHandler.RemoveAllRoomResources();
            UWB_Texturing.BundleHandler.RemoveAllRawInfo();
        }
        #endregion

        #region Bundle
#if UNITY_EDITOR
        /// <summary>
        /// Packs the resources associated with a Hololens Scanned Room specified
        /// by the RoomManager into an asset bundle.
        /// </summary>
        public static void PackRoomResourceBundle()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            //UWB_Texturing.BundleHandler.PackRawRoomTextureBundle(BuildTarget.StandaloneWindows);
            RoomHandler.PackRawRoomResourceBundle(UWB_Texturing.Config.RoomObject.GameObjectName);
        }

        /// <summary>
        /// Packs the resources associated with all Hololens Scanned Rooms into
        /// separate asset bundles.
        /// </summary>
        public static void PackAllRoomResourceBundles()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            //UWB_Texturing.BundleHandler.PackAllRawRoomTextureBundles(BuildTarget.StandaloneWindows);
            RoomHandler.PackAllRawRoomResourceBundles();
        }

        /// <summary>
        /// Packs the prefab of a Hololens Scanned Room specified by the RoomManager
        /// into an asset bundle.
        /// </summary>
        public static void PackRoomBundle()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            //UWB_Texturing.BundleHandler.PackFinalRoomBundle(BuildTarget.StandaloneWindows);
            RoomHandler.PackRoomBundle(UWB_Texturing.Config.RoomObject.GameObjectName);
        }

        /// <summary>
        /// Packs the prefabs of all Hololens Scanned Rooms into separate asset
        /// bundles.
        /// </summary>
        public static void PackAllRoomBundles()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            //UWB_Texturing.BundleHandler.PackAllFinalRoomBundles(BuildTarget.StandaloneWindows);
            RoomHandler.PackAllRoomBundles();
        }
#endif

        /// <summary>
        /// Unpacks the cached asset bundle associated with a Hololens Scanned
        /// Room's resources specified by the RoomManager.
        /// </summary>
        public static void UnpackRoomResourceBundle()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            string rawRoomBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename(), roomName);

            UWB_Texturing.BundleHandler.UnpackRawResourceTextureBundle(rawRoomBundlePath);
        }

        /// <summary>
        /// Unpacks the cached asset bundles associated with all Hololens Scanned
        /// Rooms' resources.
        /// </summary>
        public static void UnpackAllRoomResourceBundles()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.UnpackAllRawResourceTextureBundles();
        }

        /// <summary>
        /// Unpacks the cached asset bundle associated with a Hololens Scanned
        /// Room's prefab specified by the RoomManager.
        /// </summary>
        public static void UnpackRoomBundle()
        {
            Config.Current.Room.SetFolders();
            string roomName = RoomManager.SyncRoomName();
            string roomBundlePath = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename(), roomName);

            UWB_Texturing.BundleHandler.UnpackFinalRoomTextureBundle(roomBundlePath);
        }

        /// <summary>
        /// Unpacks all cached asset bundles associated with all Hololens Scanned
        /// Rooms' prefabs.
        /// </summary>
        public static void UnpackAllRoomBundles()
        {
            Config.Current.Room.SetFolders();
            RoomManager.SyncRoomName();
            UWB_Texturing.BundleHandler.UnpackAllFinalRoomTextureBundles();
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Sends the asset bundle associated with the Hololens Scanned Room
        /// specified by the RoomManager to the ASL node with the given
        /// targetID PUN ID.
        /// </summary>
        /// 
        /// <param name="targetID">
        /// The PUN ID of the ASL node that you want to send the asset bundle
        /// to.
        /// </param>
        public static void ExportRoom(int targetID)
        {
            //Debug.Log("Export Room entered");
            ////string destinationDirectory = Config.AssetBundle.PC.CompileAbsoluteBundleDirectory();

            ////UWB_Texturing.BundleHandler.PackFinalRoomBundle(destinationDirectory, BuildTarget.StandaloneWindows);  // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE
            //string bundleName = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename();
            //string bundlePath = Config.AssetBundle.PC.CompileAbsoluteBundlePath(Config.AssetBundle.PC.CompileFilename(bundleName)); // MUST INCORPORATE CODE THAT WILL ANALYZE TARGET ID/TARGET AND SET CORRECT BUILDTARGET FOR PACKING AND SENDING ASSET BUNDLE

            //Debug.Log("bundlename = " + bundleName);
            //Debug.Log("bundle path = " + bundlePath);

            //int finalRoomBundlePort = Config.Ports.RoomBundle;
            //////Launcher.SendAssetBundle(targetID, bundlePath, finalRoomBundlePort);
            ////Launcher launcher = Launcher.GetLauncherInstance();
            ////launcher.SendRoomModel(targetID);

            //////Debug.Log("bundle sent");

            //////PhotonPlayer.Find(targetID);

            //////Debug.Log("Photon Player found");

            //////photonView.RPC("ReceiveRoomModel", PhotonPlayer.Find(targetID), IPManager.CompileNetworkConfigString(finalRoomBundlePort));


            //string filepath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            string filepath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            int roomBundlePort = Config.Ports.RoomBundle_ClientToServer;
#if !UNITY_WSA_10_0
            SocketClient_PC.SendFile(ServerFinder.serverIP, roomBundlePort, filepath);
            Debug.Log("Exporting raw room resources not currently implemented correctly! Doesn't consider target ID and just sends to master");
#elif !UNITY_EDITOR && UNITY_WSA_10_0
            SocketClient_Hololens.SendFile(ServerFinder.serverIP, roomBundlePort, filepath);
#endif
        }
        
        /// <summary>
        /// Generates Hololens Scanned Room resources from associated text files,
        /// generates a Hololens Scanned Room GameObject for all rooms, and 
        /// generates a prefab for all rooms.
        /// </summary>
        public static void ProcessAllRooms()
        {
            RoomHandler.ProcessAllRooms();
        }
#endregion
    }
}