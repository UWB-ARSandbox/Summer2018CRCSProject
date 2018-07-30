using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// A base class for Resource-type Config sub-classes (i.e. textures,  
    /// materials, etc.). Makes it easier to ensure you get the right strings 
    /// and pathways according to what you need for your platform.
    /// </summary>
    public class Config_Base_ResourceType
    {
        #region Fields/Properties
#if UNITY_WSA_10_0
        /// <summary>
        /// The full pathway to the assets folder. The Assets folder sits one level
        /// above the ASL folder at the time of this writing.
        /// </summary>
        private static string absoluteAssetRootFolder = Application.persistentDataPath;
#elif UNITY_ANDROID
        /// <summary>
        /// The full pathway to the assets folder. The Assets folder sits one level
        /// above the ASL folder at the time of this writing.
        /// </summary>
        private static string absoluteAssetRootFolder = "/data/data/" + Application.identifier;
        // Application.bundleIdentifier may be replaced by Application.identifier in Unity 5.6.0+
#elif UNITY_IOS
        /// <summary>
        /// The full pathway to the assets folder. The Assets folder sits one level
        /// above the ASL folder at the time of this writing.
        /// </summary>
        private static string absoluteAssetRootFolder = Path.Combine(Application.persistentDataPath, "Assets");
#else
        /// <summary>
        /// The full pathway to the assets folder. The Assets folder sits one level
        /// above the ASL folder at the time of this writing.
        /// </summary>
        private static string absoluteAssetRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
#endif
        /// <summary>
        /// The full pathway to the assets folder. The Assets folder sits one level
        /// above the ASL folder at the time of this writing.
        /// </summary>
        public static string AbsoluteAssetRootFolder
        {
            get
            {
                return absoluteAssetRootFolder;
            }
            set
            {
#if UNITY_WSA_10_0
                absoluteAssetRootFolder = Application.persistentDataPath;
#elif UNITY_ANDROID
                absoluteAssetRootFolder = value;
#else
                //absoluteAssetRootFolder = Application.dataPath;
                absoluteAssetRootFolder = value;
                // Put in logic for all node types
#endif
                UWB_Texturing.Config_Base.AbsoluteAssetRootFolder = absoluteAssetRootFolder;
            }
        }

        /// <summary>
        /// The relative path from the Assets folder to the Resources folder for
        /// the ASL project.
        /// </summary>
        private static string assetSubFolder = "ASL/Resources";
        /// <summary>
        /// The relative path from the Assets folder to the Resources folder for
        /// the ASL project.
        /// </summary>
        public static string AssetSubFolder
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
#endif
            }
        }

        //        private static string bundleSubFolder = AssetSubFolder + "/StreamingAssets";
        //        public static string BundleSubFolder
        //        {
        //            get
        //            {
        //                return bundleSubFolder;
        //            }
        //            set
        //            {
        //#if UNITY_WSA_10_0
        //#else
        //                bundleSubFolder = value;
        //#endif
        //            }
        //        }

        //        private static string roomResourceSubFolder = AssetSubFolder + "/Rooms";
        //        public static string RoomResourceSubFolder
        //        {
        //            get
        //            {
        //                return roomResourceSubFolder;
        //            }
        //            set
        //            {
        //#if UNITY_WSA_10_0
        //#else
        //                roomResourceSubFolder = value;
        //                UWB_Texturing.Config_Base.AssetSubFolder = roomResourceSubFolder;
        //#endif
        //            }
        //        }

#endregion

#region Methods

        //        public static string CompileUnityRoomDirectory()
        //        {
        //            return RoomResourceSubFolder;
        //        }
        //        public static string CompileUnityRoomDirectory(string roomName)
        //        {
        //            return RoomResourceSubFolder + '/' + roomName;
        //        }
        //        public static string CompileUnityRoomPath(string filename, string roomName)
        //        {
        //            return CompileUnityRoomDirectory(roomName) + '/' + filename;
        //        }
        //        public static string CompileAbsoluteRoomDirectory()
        //        {
        //#if UNITY_WSA_10_0
        //            return AbsoluteAssetRootFolder;
        //#else
        //            return Path.Combine(AbsoluteAssetRootFolder, RoomResourceSubFolder);
        //#endif
        //        }
        //        public static string CompileAbsoluteRoomDirectory(string roomName)
        //        {
        //#if UNITY_WSA_10_0
        //            return AbsoluteAssetRootFolder;
        //#else
        //            return Path.Combine(AbsoluteAssetRootFolder, Path.Combine(RoomResourceSubFolder, roomName));
        //#endif
        //        }
        //        public static string CompileAbsoluteRoomPath(string filename, string roomName)
        //        {
        //            return Path.Combine(CompileAbsoluteRoomDirectory(roomName), filename);
        //        }








        /// <summary>
        /// Compiles the full directory pathway for the Resources folder for 
        /// the ASL project.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the full directory pathway for the Resources
        /// folder for the ASL project.
        /// </returns>
        public static string CompileAbsoluteAssetDirectory()
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
#endif
        }
        /// <summary>
        /// Compiles the full filepath for a file in the Resources folder for
        /// the ASL project. If looking for something not in the top level of
        /// the Resources folder, you have to provide the relative path from
        /// the Resources folder.
        /// </summary>
        /// 
        /// <param name="filename">
        /// The file name or relative path from the Resources folder.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the full filepath for a file in the Resources
        /// folder for the ASL project.
        /// </returns>
        public static string CompileAbsoluteAssetPath(string filename)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
        }
        /// <summary>
        /// Compiles the relative directory pathway for the Resources folder for
        /// the ASL project.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the relative directory pathway for the Resources 
        /// folder for the ASL project.
        /// </returns>
        public static string CompileUnityAssetDirectory()
        {
            //return "Assets/" + AssetSubFolder;
            return AssetSubFolder;
        }
        /// <summary>
        /// Compiles the relative filepath for a file in the Resources folder
        /// for the ASL project. If looking for something not in the top level 
        /// of the Resources folder, you have to provide the relative path from
        /// the Resources folder.
        /// </summary>
        /// 
        /// <param name="filename">
        /// The file name or relative path from the Resources folder.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the relative filepath for a file in the Resources
        /// folder for the ASL project.
        /// </returns>
        public static string CompileUnityAssetPath(string filename)
        {
            return CompileUnityAssetDirectory() + '/' + filename;
        }
        //public static string CompileRoomResourcesLoadPath(string assetNameWithoutExtension, string roomName)
        //{
        //    return RoomResourceSubFolder.Substring(RoomResourceSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        //}
        /// <summary>
        /// Compiles a Resources.Load compatible pathway to a file in the Resources
        /// folder. The file name should have no extension.
        /// </summary>
        /// 
        /// <param name="assetNameWithoutExtension">
        /// The name of the file without the extension.
        /// </param>
        /// 
        /// <returns>
        /// A string representing a Resources.Load compatible pathway to a file in
        /// the Resources folder.
        /// </returns>
        public static string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
            //return ResourcesSubFolder + '/' + assetNameWithoutExtension;
        }
        /// <summary>
        /// Compiles a Resources.Load compatible pathway to a file in the Resources
        /// folder. Allows you to specify a subfolder in the Resources folder that
        /// the file resides in.
        /// </summary>
        /// 
        /// <param name="assetSubDirectory">
        /// The subfolder of the Resources folder that the file resides in.
        /// </param>
        /// <param name="assetNameWithoutExtension">
        /// The name of the file without the file extension.
        /// </param>
        /// 
        /// <returns>
        /// A string representing a Resources.Load compatible pathway to a file
        /// in a sub-folder of the Resources folder.
        /// </returns>
        public static string CompileResourcesLoadPath(string assetSubDirectory, string assetNameWithoutExtension)
        {
            return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }














//        public static string CompileUnityBundleDirectory()
//        {
//            return "Assets/" + BundleSubFolder;
//            //return BundleSubFolder;
//        }
//        public static string CompileUnityBundlePath(string filename)
//        {
//            //return CompileUnityBundleDirectory() + '/' + filename;
//            return Path.Combine(CompileUnityBundleDirectory(), filename);
//        }
//        public static string CompileAbsoluteBundleDirectory()
//        {
//#if UNITY_WSA_10_0
//            return AbsoluteAssetRootFolder;
//#else
//            return Path.Combine(AbsoluteAssetRootFolder, BundleSubFolder);
//#endif
//        }
//        public static string CompileAbsoluteBundlePath(string filename)
//        {
//            return Path.Combine(CompileAbsoluteBundleDirectory(), filename);
//        }

#endregion
    }
}
