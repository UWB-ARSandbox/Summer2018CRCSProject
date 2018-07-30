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
    /// Handles logic for displaying menus in the Unity Editor menu bar (top of 
    /// the screen (ex. File, Edit, Assets, etc.)).
    /// 
    /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
    /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
    /// this menu class and resultant menu items should be ignored.
    /// </summary>
    public static class Menu {
#if UNITY_EDITOR && !UNITY_WSA_10_0
        #region Instantiate
        /// <summary>
        /// Instantiate a Hololens Room from the currently selected room in the
        /// Room Manager.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Instantiate/Current Room/GameObject", false, 0)]
        public static void InstantiateRoom()
        {
            //UWB_Texturing.Menu.InstantiateRoom();
            MenuHandler.InstantiateRoom();
        }

        /// <summary>
        /// Instantiate all Hololens Rooms currently in the Rooms directory.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Instantiate/All Rooms/GameObjects", false, 0)]
        public static void InstantiateAllRooms()
        {
            //UWB_Texturing.Menu.InstantiateAllRooms();
            MenuHandler.InstantiateAllRooms();
        }
        #endregion

        #region Prefabs
        /// <summary>
        /// Instantiate a Hololens Room prefab (not from components).
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Instantiate/Current Room/Prefab", false, 0)]
        public static void InstantiateRoomPrefab()
        {
            //UWB_Texturing.Menu.InstantiateRoomPrefab();
            MenuHandler.InstantiateRoomPrefab();
        }

        /// <summary>
        /// Instantiate all Hololens Room prefabs (not from components).
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Instantiate/All Rooms/Prefabs", false, 0)]
        public static void InstantiateAllRoomPrefabs()
        {
            //UWB_Texturing.Menu.InstantiateAllRoomPrefabs();
            MenuHandler.InstantiateAllRoomPrefabs();
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete the GameObject in the scene representing the Hololens Room
        /// currently specified by the RoomManager.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/GameObject", false, 0)]
        public static void RemoveRoomObject()
        {
            //UWB_Texturing.Menu.RemoveRoomObject();
            MenuHandler.RemoveRoomObject();
        }

        /// <summary>
        /// Delete all GameObjects in the scene representing Hololens Rooms.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/GameObject", false, 0)]
        public static void RemoveAllRoomObjects()
        {
            //UWB_Texturing.Menu.RemoveAllRoomObjects();
            MenuHandler.RemoveAllRoomObjects();
        }

        /// <summary>
        /// Delete the prefab of the Hololens Scanned Room specified by
        /// RoomManager stored in the ASL project.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/Prefab", false, 0)]
        public static void RemoveRoomPrefab()
        {
            //UWB_Texturing.Menu.RemoveRoomPrefab();
            MenuHandler.RemoveRoomPrefab();
        }

        /// <summary>
        /// Delete all prefabs of Hololens Scanned Rooms stored in the ASL
        /// project.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/Prefab", false, 0)]
        public static void RemoveAllRoomPrefabs()
        {
            //UWB_Texturing.Menu.RemoveAllRoomPrefabs();
            MenuHandler.RemoveAllRoomPrefabs();
        }

        /// <summary>
        /// Delete Resources (i.e. textures, meshes) of the Hololens Scanned
        /// Room specified by RoomManager. This does not delete the text files
        /// that the resources are derived from.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/Resources", false, 0)]
        public static void RemoveRoomResources()
        {
            //UWB_Texturing.Menu.RemoveRoomResources();
            MenuHandler.RemoveRoomResources();
        }

        /// <summary>
        /// Delete Resources (i.e. textures, meshes) of all Hololens Scanned
        /// Rooms. This does not delete the text files that the resources are derived from.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/Resources", false, 0)]
        public static void RemoveAllRoomResources()
        {
            //UWB_Texturing.Menu.RemoveAllRoomResources();
            MenuHandler.RemoveAllRoomResources();
        }

        /// <summary>
        /// Delete the text files that are used to derive resources for the Hololens
        /// Scanned Room specified by the RoomManager. This does not delete the
        /// resources themselves.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/Raw Info", false, 0)]
        public static void RemoveRoomRawInfo()
        {
            //UWB_Texturing.Menu.RemoveRoomRawInfo();
            MenuHandler.RemoveRoomRawInfo();
        }

        /// <summary>
        /// Delete the text files that are used to derive resources for all Hololens
        /// Scanned Rooms. This does not delete the resources themselves.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/Raw Info", false, 0)]
        public static void RemoveAllRoomRawInfo()
        {
            //UWB_Texturing.Menu.RemoveAllRoomRawInfo();
            MenuHandler.RemoveAllRoomRawInfo();
        }

        /// <summary>
        /// Delete all items associated with the Hololens Scanned Room specified
        /// by the RoomManager.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/Current Room/Everything", false, 0)]
        public static void RemoveEverything()
        {
            //UWB_Texturing.Menu.RemoveEverything();
            MenuHandler.RemoveEverything();
        }

        /// <summary>
        /// Delete all items associated with all Hololens Scanned Rooms.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Delete/All Rooms/Everything", false, 0)]
        public static void RemoveAllEverything()
        {
            //UWB_Texturing.Menu.RemoveAllEverything();
            MenuHandler.RemoveAllEverything();
        }
        #endregion

        #region Bundle
        /// <summary>
        /// Construct an asset bundle for the Hololens Scanned Room specified by
        /// the RoomManager. This asset bundle will hold the resources associated
        /// with the room.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Resource Bundle/Pack/Current Room", false, 0)]
        public static void PackRoomResourceBundle()
        {
            //UWB_Texturing.Menu.PackRoomResourceBundle();
            MenuHandler.PackRoomResourceBundle();
        }

        /// <summary>
        /// Construct asset bundles for all Hololens Scanned Rooms. These asset
        /// bundles will hold the resources associated with the rooms.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Resource Bundle/Pack/All Rooms", false, 0)]
        public static void PackAllRoomResourceBundles()
        {
            //UWB_Texturing.Menu.PackAllRoomResourceBundles();
            MenuHandler.PackAllRoomResourceBundles();
        }

        /// <summary>
        /// Construct an asset bundle for a Hololens Scanned Room specified 
        /// by the RoomManager. This asset bundle will hold the prefab of a 
        /// Hololens Scanned Room GameObject.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Pack/Current Room", false, 0)]
        public static void PackRoomBundle()
        {
            //UWB_Texturing.Menu.PackRoomBundle();
            MenuHandler.PackRoomBundle();
        }

        /// <summary>
        /// Construct an asset bundle for all Hololens Scanned Rooms. These asset
        /// bundles will hold the prefabs of the Hololens Scanned Room GameObjects.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Pack/All Rooms", false, 0)]
        public static void PackAllRoomBundles()
        {
            //UWB_Texturing.Menu.PackAllRoomBundles();
            MenuHandler.PackAllRoomBundles();
        }

        /// <summary>
        /// Decipher and unpack a resource asset bundle for a Hololens Scanned 
        /// Room specified by the RoomManager.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Resource Bundle/Unpack/Current Room", false, 0)]
        public static void UnpackRoomResourceBundle()
        {
            //UWB_Texturing.Menu.UnpackRoomResourceBundle();
            MenuHandler.UnpackRoomResourceBundle();
        }

        /// <summary>
        /// Decipher and unpack all resource asset bundles cached for Hololens
        /// Scanned Rooms.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Resource Bundle/Unpack/All Rooms", false, 0)]
        public static void UnpackAllRoomResourceBundles()
        {
            //UWB_Texturing.Menu.UnpackAllRoomResourceBundles();
            MenuHandler.UnpackAllRoomResourceBundles();
        }

        /// <summary>
        /// Decipher and unpack an asset bundle containing a prefab for a
        /// Hololens Scanned Room specified by the RoomManager.
        /// 
        /// NOTE: Hololens Scanned Rooms have experienced several bugs as ASL 
        /// has matured. Coupled with the deprecation of the Microsoft Hololens, 
        /// this menu option should be ignored.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Unpack/Current Room", false, 0)]
        public static void UnpackRoomBundle()
        {
            //UWB_Texturing.Menu.UnpackRoomBundle();
            MenuHandler.UnpackRoomBundle();
        }

        /// <summary>
        /// Decipher and unpack an asset bundle containing a prefab for all
        /// Hololens Scanned Rooms.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Unpack/All Rooms", false, 0)]
        public static void UnpackAllRoomBundles()
        {
            //UWB_Texturing.Menu.UnpackAllRoomBundles();
            MenuHandler.UnpackAllRoomBundles();
        }

        //[UnityEditor.MenuItem("ASL/Room Texture/Bundle/Room Bundle/Unpack/All Rooms", false, 0)]
        //public static void ExportRoomBundleToMasterClient()
        //{
            
        //}
        #endregion











        //[UnityEditor.MenuItem("ASL/Room Texture/Pack Raw Resources (Offline)", false, 0)]
        //public static void PackRawResourcesBundle()
        //{
        //    MenuHandler.PackRawResourcesBundle();
        //}

        //[UnityEditor.MenuItem("ASL/Room Texture/Pack Room Bundle (Offline)", false, 0)]
        //public static void PackRoomBundle()
        //{
        //    MenuHandler.PackRoomBundle();
        //}

        ///// <summary>
        ///// Exports to master client.
        ///// </summary>
        //[UnityEditor.MenuItem("ASL/Room Texture/Export Room Resources", false, 0)]
        //public static void ExportRawResources() {
        //    MenuHandler.ExportRawResources(PhotonNetwork.masterClient.ID);
        //}

        /////// <summary>
        /////// Processes room resources to generate final room, room prefab, and appropraite bundle.
        /////// </summary>
        ////[UnityEditor.MenuItem("ASL/Room Texture/Generate Room", false, 0)]
        ////public static void ProcessRoomResources()
        ////{
        ////    MenuHandler.ProcessRoomResources();
        ////}

        //[UnityEditor.MenuItem("ASL/Room Texture/Export Room", false, 0)]
        //public static void ExportFinalRoom() {
        //    MenuHandler.ExportRoom(PhotonNetwork.masterClient.ID);
        //}

        #region Verified
        /// <summary>
        /// Triggers the deciphering of the text files used to generate the
        /// resources required to generate all Hololens Scanned Rooms. Also
        /// uses those resources to generate GameObjects and prefabs for
        /// the Scanned Rooms.
        /// </summary>
        [UnityEditor.MenuItem("ASL/Room Texture/Process All Rooms")]
        public static void ProcessAllRooms()
        {
            MenuHandler.ProcessAllRooms();
        }
#endregion
#endif
    }
}
