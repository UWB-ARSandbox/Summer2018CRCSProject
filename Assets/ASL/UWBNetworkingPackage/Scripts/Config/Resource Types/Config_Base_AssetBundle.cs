using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// The base class for the subclass associated with Unity Asset Bundles in
    /// the Config class. Since Asset Bundles are platform-dependent on compilation,
    /// a base class for the Asset Bundle config sub-class will allow you to save
    /// a lot of repetitive work.
    /// </summary>
    public class Config_Base_AssetBundle : Config_Base_ResourceType
    {
        /// <summary>
        /// "ASL/Resources/StreamingAssets". The relative folder from the base
        /// Unity project folder that tells ASL where the asset bundles are stored.
        /// </summary>
        private static string assetSubFolder = Config_Base_ResourceType.AssetSubFolder + "/StreamingAssets";
        /// <summary>
        /// "ASL/Resources/StreamingAssets". The relative folder from the base
        /// Unity project folder that tells ASL where the asset bundles are stored.
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
#endif
            }
        }
        
        /// <summary>
        /// Compiles the full directory pathway for the asset bundle directory 
        /// (ex. "C://...")
        /// </summary>
        /// 
        /// <returns>
        /// Returns a string representing the full directory pathway for the asset
        /// bundle directory.
        /// </returns>
        public static new string CompileAbsoluteAssetDirectory()
        {
            // Find the asset directory based off of the nodetype passed in
            return Path.Combine(Directory.GetCurrentDirectory(), AssetSubFolder);
        }

        /// <summary>
        /// Compiles the full filepath for a given asset bundle (ex. "C://...")
        /// </summary>
        /// 
        /// <param name="filename">
        /// The name of the asset bundle.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the full filepath for a given asset bundle.
        /// </returns>
        public static new string CompileAbsoluteAssetPath(string filename)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
        }

        /// <summary>
        /// Compiles the relative directory pathway for the asset bundle directory.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the relative directory pathway for the asset
        /// bundle directory.
        /// </returns>
        public static new string CompileUnityAssetDirectory()
        {
            return "Assets/" + AssetSubFolder;
        }

        /// <summary>
        /// Compiles the relative filepath for a given asset bundle.
        /// </summary>
        /// 
        /// <param name="filename">
        /// The name of the asset bundle.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the relative filepath for a given asset bundle.
        /// </returns>
        public static new string CompileUnityAssetPath(string filename)
        {
            return CompileUnityAssetDirectory() + '/' + filename;
        }

        /// <summary>
        /// Compiles a string representing a Resources.Load compatible pathway
        /// for an asset bundle.
        /// </summary>
        /// 
        /// <param name="assetNameWithoutExtension">
        /// The name of the asset bundle (without the extension).
        /// </param>
        /// 
        /// <returns>
        /// A string representing a Resources.Load compatible pathway for an
        /// asset bundle.
        /// </returns>
        public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }

        /// <summary>
        /// Compile a Resources.Load compatible file pathway for an asset bundle that
        /// doesn't sit in the top level of the asset bundle subfolder.
        /// </summary>
        /// 
        /// <param name="assetSubDirectory">
        /// The subdirectory to sandwich between the asset bundle default pathway and
        /// the name of the asset bundle.
        /// </param>
        /// <param name="assetNameWithoutExtension">
        /// The name of the asset bundle without an extension.
        /// </param>
        /// 
        /// <returns>
        /// A string representing a Resources.Load compatible file pathway for an asset
        /// bundle with a semi-custom asset bundle pathway.
        /// </returns>
        public static new string CompileResourcesLoadPath(string assetSubDirectory, string assetNameWithoutExtension)
        {
            return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
        }

        //public static string Extension = ".asset";
        /// <summary>
        /// Compiles a filename for an asset bundle.
        /// </summary>
        /// 
        /// <param name="bundleName">
        /// The name of the asset bundle (without extension).
        /// </param>
        /// 
        /// <returns>
        /// A string representing the asset bundle name with an extension.
        /// </returns>
        public static string CompileFilename(string bundleName)
        {
            return bundleName; //+ Extension;
        }
    }
}