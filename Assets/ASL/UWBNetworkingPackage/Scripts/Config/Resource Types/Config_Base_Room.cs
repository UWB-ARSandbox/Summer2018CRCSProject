using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// A base class for Config sub-classes related to scanned rooms (i.e. the
    /// Tango and Hololens scanned rooms).
    /// </summary>
    public class Config_Base_Room : Config_Base_ResourceType
    {
        /// <summary>
        /// Unity method that is called prior to runtime. Triggers before "Awake"
        /// methods. Adjusts cached folder pathways for better readability in the
        /// class and creates the Resources directory associated if it does not
        /// yet exist.
        /// </summary>
        public static void Start()
        {
            SetFolders();
        }

        /// <summary>
        /// Adjusts cached folder pathways for better readability in the class and
        /// creates the Resources directory associated if it does not yet exist.
        /// </summary>
        public static void SetFolders()
        {
            Directory.CreateDirectory(CompileAbsoluteAssetDirectory());
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            UWB_Texturing.Config_Base.AbsoluteAssetRootFolder = AbsoluteAssetRootFolder;
            UWB_Texturing.Config_Base.AssetSubFolder = AssetSubFolder;
        }
        
        /// <summary>
        /// "ASL/Resources/Rooms". Refers to the main directory where scanned 
        /// room folders are kept.
        /// </summary>
        private static string assetSubFolder = Config_Base_ResourceType.AssetSubFolder + "/Rooms";
        /// <summary>
        /// Refers to the main directory where scanned room folders are kept.
        /// </summary>
        public static new string AssetSubFolder
        {
            get
            {
                return assetSubFolder;
            }
            set
            {
#if UNITY_WSA_10_0
#else
                assetSubFolder = value;
                UWB_Texturing.Config_Base.AssetSubFolder = assetSubFolder;
#endif
            }
        }

        /// <summary>
        /// The name of the file that informs a room parser what platform is
        /// associated with a scanned room.
        /// </summary>
        private static string tagFilename = "ScannerPlatform";
        /// <summary>
        /// The file extension of the file that informs a room parser what
        /// platform is associated with a scanned room.
        /// </summary>
        private static string tagFileExtension = ".txt";
        /// <summary>
        /// The name of the file that informs a room parser what platform is
        /// associated with a scanned room.
        /// </summary>
        public static string TagFilename
        {
            get
            {
                return tagFilename + tagFileExtension;
            }
            set
            {
#if UNITY_WSA_10_0
#elif UNITY_ANDROID
#else
                string[] components = value.Split('.');
                tagFilename = components[0];
                if(components.Length > 1)
                {
                    tagFileExtension = components[1];
                }
#endif
            }
        }

        /// <summary>
        /// The custom file extension for a Tango scanned room file.
        /// </summary>
        private static string tangoFileExtension = ".tngrm";
        /// <summary>
        /// The custom file extension for a Tango scanned room file.
        /// </summary>
        public static string TangoFileExtension
        {
            get
            {
                return tangoFileExtension;
            }
            set
            {
                if (value.Contains("."))
                {
                    tangoFileExtension = value;
                }
                else if (value.Length < 7)
                {
                    tangoFileExtension = "." + value;
                }
                else
                {
                    Debug.LogError("Tango file extension too long. Please shorten to less than 7 characters.");
                }
            }
        }

        /// <summary>
        /// Compiles the full directory pathway for the Scanned Room folders.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the full directory pathway for the Scanned
        /// Room folders.
        /// </returns>
        public static new string CompileAbsoluteAssetDirectory()
        {
            //string roomName = RoomManager.GetAllRoomNames()[0];
            //Debug.Error("Defaulting to room " + roomName);
            //return CompileAbsoluteAssetDirectory(roomName);
            return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
        }

        /// <summary>
        /// Compiles the full directory pathway for a specific Scanned Room
        /// folder.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the scanned room you want the directory for.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the full directory pathway for a specific
        /// Scanned Room folder.
        /// </returns>
        public static string CompileAbsoluteAssetDirectory(string roomName)
        {
            return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder) + '/' + roomName;
        }
        
        /// <summary>
        /// Compiles the full filepath for a specific Scanned Room. (Used for
        /// Hololens scanned room only at the moment.)
        /// </summary>
        /// 
        /// <param name="filename">
        /// The name of the file to load (with extension).
        /// </param>
        /// 
        /// <returns>
        /// A string representing the full filepath for a specific Scanned Room.
        /// </returns>
        public static new string CompileAbsoluteAssetPath(string filename)
        {
            //string roomName = RoomManager.GetAllRoomNames()[0];
            //Debug.Error("Defaulting to room " + roomName);
            //return CompileAbsoluteAssetPath(roomName, filename);
            return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
        }

        /// <summary>
        /// Compiles the full filepath for a specific Scanned Room given that
        /// room's name and the name of the file you want.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the room you want the file for.
        /// </param>
        /// <param name="filename">
        /// The name of the file you want the path for.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the full filepath for a specific Scanned Room.
        /// </returns>
        public static string CompileAbsoluteAssetPath(string roomName, string filename)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(roomName), filename);
        }

        /// <summary>
        /// Compiles the relative directory pathway for the scanned rooms folder.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the relative directory pathway for the scanned
        /// rooms folder.
        /// </returns>
        public static new string CompileUnityAssetDirectory()
        {
            //string roomName = RoomManager.GetAllRoomNames()[0];
            //Debug.Error("Defaulting to room " + roomName);

            //return CompileUnityAssetDirectory(roomName);

            return "Assets/" + AssetSubFolder;
        }

        /// <summary>
        /// Compiles the relative directory pathway for a specific Scanned Room
        /// in the scanned rooms folder.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the room you want the folder for.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the relative directory pathway for a specific
        /// Scanned Room.
        /// </returns>
        public static string CompileUnityAssetDirectory(string roomName)
        {
            return "Assets/" + AssetSubFolder + '/' + roomName;
        }

        /// <summary>
        /// Compiles the relative filepath for a specific file. (Not currently used
        /// for either Hololens or Tango scanned room directories.)
        /// </summary>
        /// 
        /// <param name="filename">
        /// The name of the file to load.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the relative filepath for a specific file.
        /// </returns>
        public static new string CompileUnityAssetPath(string filename)
        {
            //string roomName = RoomManager.GetAllRoomNames()[0];
            //Debug.Error("Defaulting to room " + roomName);

            //return CompileUnityAssetPath(roomName, filename);

            return CompileUnityAssetDirectory() + '/' + filename;
        }

        /// <summary>
        /// Compiles the relative filepath for a filename for a specific Scanned
        /// Room.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the room for which you want the file for.
        /// </param>
        /// <param name="filename">
        /// The name of the file.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the relative filepath for a filename for a 
        /// specific Scanned Room.
        /// </returns>
        public static string CompileUnityAssetPath(string roomName, string filename)
        {
            return CompileUnityAssetDirectory(roomName) + '/' + filename;
        }
        
        /// <summary>
        /// Compiles a Resources.Load compatible filepath for a Scanned Room file.
        /// (Not currently used by Hololens or Tango.)
        /// </summary>
        /// 
        /// <param name="assetNameWithoutExtension">
        /// The name of the file (without extension).
        /// </param>
        /// 
        /// <returns>
        /// A string representing a Resources.Load compatible filepath for a Scanned
        /// Room file.
        /// </returns>
        public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            string roomName = RoomManager.GetAllRoomNames()[0];
            Debug.LogError("Defaulting to room " + roomName);

            return CompileResourcesLoadPath(roomName, assetNameWithoutExtension);
        }

        /// <summary>
        /// Compiles a Resources.Load compatible filepath for a Scanned Room file
        /// for a specific Scanned Room.
        /// </summary>
        /// 
        /// <param name="roomName">
        /// The name of the Scanned Room you want the file for.
        /// </param>
        /// <param name="assetNameWithoutExtension">
        /// The name of the file (without extension).
        /// </param>
        /// 
        /// <returns>
        /// A string representing a Resources.Load compatible filepath for a
        /// Scanned Room file for a specific Scanned Room.
        /// </returns>
        public static new string CompileResourcesLoadPath(string roomName, string assetNameWithoutExtension)
        {
            string directory = CompileUnityAssetDirectory(roomName);
            
            return directory.Substring(directory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }
    }
}