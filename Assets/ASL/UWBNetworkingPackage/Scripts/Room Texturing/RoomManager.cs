using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// A class that allows the Hololens Room library to manage multiple rooms. The
    /// room pointed to by this class is used as the main Hololens room of focus
    /// for any method that modifies information related to a specific Hololens
    /// room.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        /// <summary>
        /// The public Editor-accessible string representing the name of the 
        /// Hololens Scanned Room to focus on.
        /// </summary>
        public string RoomName;

        /// <summary>
        /// A class that manages messages for this class.
        /// </summary>
        public static class Messages
        {
            /// <summary>
            /// A sub-class that manages error messages related to this class.
            /// </summary>
            public static class Errors
            {
                /// <summary>
                /// "The raw room bundle (raw resources used to generate bundle) 
                /// is unavaiable. Please generate it through the appropriate 
                /// means and ensure it is in the correct folder."
                /// </summary>
                public static string RawRoomBundleNotAvailable = "The raw room bundle (raw resources used to generate bundle) is unavailable. Please generate it through the appropriate means and ensure it is in the correct folder.";

                /// <summary>
                /// The final room bundle is unavailable. Please generate it 
                /// through the appropriate means and ensure it is in the 
                /// correct folder.
                /// </summary>
                public static string RoomBundleNotAvailable = "The final room bundle is unavailable. Please generate it through the appropriate means and ensure it is in the correct folder.";
            }
        }

        /// <summary>
        /// Unity method that is called prior to runtime. Triggers before "Awake"
        /// method. Sets room name to default for startup and triggers 
        /// UpdateRoomBundles method every 3 seconds.
        /// </summary>
        void Start()
        {
            if (string.IsNullOrEmpty(RoomName))
            {
                RoomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            }

            InvokeRepeating("UpdateRoomBundles", 0, 3);
        }

        /// <summary>
        /// Unity method that is called every few frames. Synchronizes room name 
        /// with appropriate variables elsewhere and creates a directory for the 
        /// room if necessary.
        /// </summary>
        void FixedUpdate()
        {
            if (!RoomName.Equals(UWB_Texturing.Config.RoomObject.GameObjectName))
            {
                UWB_Texturing.Config.RoomObject.GameObjectName = RoomName;
                // Make the directory for this room
                //string directoryPath = Config_Base.CompileAbsoluteRoomDirectory(RoomName);
                string directoryPath = Config.Current.Room.CompileAbsoluteAssetDirectory(RoomName);
                //string directoryPath = UWB_Texturing.Config.RoomObject.CompileAbsoluteAssetDirectory(RoomName);
                AbnormalDirectoryHandler.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// Synchronizes the locally cached Hololens Scanned Room name to the 
        /// Room Name used by the Config file.
        /// </summary>
        public void SyncDisplayedRoomName()
        {
            RoomName = UWB_Texturing.Config.RoomObject.GameObjectName;
        }

        /// <summary>
        /// Synchronizes the Config file's Hololens Scanned Room name of focus
        /// to the locally cached Hololens Scanned Room name.
        /// </summary>
        /// <returns></returns>
        public static string SyncRoomName()
        {
            string roomName = GameObject.Find("RoomManager").GetComponent<RoomManager>().RoomName;
            UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
            return roomName;
        }

        // ERROR TESTING - Revisit when we establish a good way to identify 
        /// <summary>
        /// Gathers all Hololens Scanned Room names on this ASL node. Assumes that
        /// all Hololens Scanned Rooms exist in the "Rooms" folder of the "Resources"
        /// folder of ASL.
        /// </summary>
        /// 
        /// <returns>
        /// An array of strings representing the names of the Hololens Scanned
        /// Rooms on this ASL node.
        /// </returns>
        public static string[] GetAllRoomNames()
        {
            List<string> roomNames = new List<string>();

            //foreach(string folderPath in Directory.GetDirectories(Config_Base.CompileAbsoluteAssetDirectory()))
            foreach(string folderPath in Directory.GetDirectories(Path.Combine(Config.Current.Room.AbsoluteAssetRootFolder, Config.Current.Room.AssetSubFolder)))
            //foreach (string folderPath in Directory.GetDirectories(Path.Combine(UWB_Texturing.Config_Base.AbsoluteAssetRootFolder, Config_Base.AssetSubFolder)))
            {
                string[] pass1 = folderPath.Split('/');
                string[] pass2 = pass1[pass1.Length - 1].Split('\\');

                string directoryName = pass2[pass2.Length - 1];
                if (!directoryName.StartsWith("_"))
                {
                    roomNames.Add(directoryName);
                    Debug.Log("Room resource folder found!: " + directoryName);
                }
            }

            return roomNames.ToArray();
        }

        /// <summary>
        /// Updates the asset bundles associated with all Hololens Scanned Rooms.
        /// </summary>
        public void UpdateRoomBundles()
        {
            string[] roomNames = GetAllRoomNames();
            foreach(string roomName in roomNames)
            {
                Debug.Log("Debugging " + roomName + " UpdateRawRoomBundle");
                UpdateRawRoomBundle(roomName);
                Debug.Log("Debugging " + roomName + " UpdateRoomBundle");
                UpdateRoomBundle(roomName);
            }
        }

        /// <summary>
        /// Determines whether a downloaded / cached asset bundle concerning the 
        /// text files / textures regarding the Hololens Scanned Room specified
        /// is newer than the locally stored version. If so, the locally stored
        /// version is replaced with the downloaded version (i.e. the newer one).
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the Hololens Scanned Room whose asset bundle you want 
        /// updated.
        /// </param>
        public static void UpdateRawRoomBundle(string roomName)
        {
            string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
            
            string bundleName = UWB_Texturing.Config.AssetBundle.RawPackage.CompileFilename();
            //string ASLBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(bundleName);
            string ASLBundlePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(bundleName);
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(bundleName, roomName);
            
            string ASLBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            string GeneratedBundleDirectory = UWB_Texturing.Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(ASLBundleDirectory))
                Directory.CreateDirectory(ASLBundleDirectory);
            if (Directory.Exists(GeneratedBundleDirectory))
                Directory.CreateDirectory(GeneratedBundleDirectory);

            if (File.Exists(ASLBundlePath)
                && File.Exists(GeneratedBundlePath))
            {
                DateTime ASLDateTime = File.GetLastWriteTime(ASLBundlePath);
                DateTime RoomTextureDateTime = File.GetLastWriteTime(GeneratedBundlePath);

                if (DateTime.Compare(ASLDateTime, RoomTextureDateTime) < 0)
                {
                    if (File.Exists(ASLBundlePath))
                    {
                        File.Delete(ASLBundlePath);
                    }
                    File.Copy(GeneratedBundlePath, ASLBundlePath);
                }
                else if (DateTime.Compare(RoomTextureDateTime, ASLDateTime) < 0)
                {
                    if (File.Exists(GeneratedBundlePath))
                    {
                        File.Delete(GeneratedBundlePath);
                    }
                    File.Copy(ASLBundlePath, GeneratedBundlePath);
                }
            }
            else if (File.Exists(GeneratedBundlePath)
                && !File.Exists(ASLBundlePath))
            {
                File.Copy(GeneratedBundlePath, ASLBundlePath);
            }
            else if (File.Exists(ASLBundlePath)
                && !File.Exists(GeneratedBundlePath))
            {
                File.Copy(ASLBundlePath, GeneratedBundlePath);
            }
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
        }

        /// <summary>
        /// Determines whether a downloaded / cached asset bundle concerning the
        /// prefab for the Hololens Scanned Room specified is newer than the 
        /// locally stored version. If so, the locally stored version is replaced
        /// with the downloaded version (i.e. the newer one).
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the Hololens Scanned Room whose asset bundle you want 
        /// updated.
        /// </param>
        public static void UpdateRoomBundle(string roomName)
        {
            string originalRoomName = UWB_Texturing.Config.RoomObject.GameObjectName;
            UWB_Texturing.Config.RoomObject.GameObjectName = roomName;

            string bundleName = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename();
            //string ASLBundlePath = Config.AssetBundle.Current.CompileAbsoluteBundlePath(Config.AssetBundle.Current.CompileFilename(bundleName));
            string ASLBundlePath = Config.Current.AssetBundle.CompileAbsoluteAssetPath(Config.Current.AssetBundle.CompileFilename(bundleName));
            string GeneratedBundlePath = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(bundleName, roomName);
            //string GeneratedBundlePath = Config.AssetBundle.PC.CompileAbsoluteAssetPath(Config.AssetBundle.PC.CompileFilename(bundleName));
            
            Debug.Log("Bundle name = " + bundleName);
            Debug.Log("Current directory = " + Directory.GetCurrentDirectory());
            Debug.Log("ASL Bundle path = " + ASLBundlePath);
            Debug.Log("Generated Bundle Path = " + GeneratedBundlePath);

            string ASLBundleDirectory = Config.Current.AssetBundle.CompileAbsoluteAssetDirectory();
            string GeneratedBundleDirectory = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(ASLBundleDirectory))
                Directory.CreateDirectory(ASLBundleDirectory);
            if (Directory.Exists(GeneratedBundleDirectory))
                Directory.CreateDirectory(GeneratedBundleDirectory);
            
            if (File.Exists(ASLBundlePath)
                && File.Exists(GeneratedBundlePath))
            {
                DateTime ASLDateTime = File.GetLastWriteTime(ASLBundlePath);
                DateTime RoomTextureDateTime = File.GetLastWriteTime(GeneratedBundlePath);

                if (DateTime.Compare(ASLDateTime, RoomTextureDateTime) < 0)
                {
                    if (File.Exists(ASLBundlePath))
                    {
                        File.Delete(ASLBundlePath);
                    }
                    File.Copy(GeneratedBundlePath, ASLBundlePath);
                }
                else if (DateTime.Compare(RoomTextureDateTime, ASLDateTime) < 0)
                {
                    if (File.Exists(GeneratedBundlePath))
                    {
                        File.Delete(GeneratedBundlePath);
                    }
                    File.Copy(ASLBundlePath, GeneratedBundlePath);
                }
            }
            else if (File.Exists(GeneratedBundlePath)
                && !File.Exists(ASLBundlePath))
            {
                File.Copy(GeneratedBundlePath, ASLBundlePath);
            }
            else if (File.Exists(ASLBundlePath)
                && !File.Exists(GeneratedBundlePath))
            {
                File.Copy(ASLBundlePath, GeneratedBundlePath);
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            UWB_Texturing.Config.RoomObject.GameObjectName = originalRoomName;
        }
    }
}