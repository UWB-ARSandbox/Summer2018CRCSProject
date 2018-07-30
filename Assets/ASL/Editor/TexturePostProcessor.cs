using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ASL.Adapters.Unity.Editor
{
    /// <summary>
    /// Handles texture processing upon import to the current project. Overrides 
    /// default and custom settings for Unity texture import. This class 
    /// overrides settings to get around Unity's default texture importer, which 
    /// blocks manipulation and availability of textures without extensive, 
    /// tedious manual manipulation.
    /// 
    /// NOTE: Though called "TexturePostProcessor", this class handles 
    /// preprocessing. Change of method name to more appropriate name not 
    /// tested, but advised for finalizing work.
    /// </summary>
    public class TexturePostProcessor : AssetPostprocessor
    {
#if UNITY_EDITOR
        /// <summary>
        /// Sets texture settings upon import to ASL.
        /// </summary>
        void OnPreprocessTexture()
        {
            TextureImporter textureImporter = assetImporter as TextureImporter;
            textureImporter.isReadable = true; // Make it so Unity can actually use a texture it imports (e.g. tex.GetPixels)
            //textureImporter.textureFormat = TextureImporterFormat.ARGB32; // Set the format of images to be unified

            // Set platform specific settings (set similar to "Standalone" setting)
            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
            textureImporter.GetPlatformTextureSettings("Standalone").CopyTo(platformSettings);
            platformSettings.overridden = true;
            platformSettings.format = TextureImporterFormat.RGBA32; // Set the format of images to be unified
            textureImporter.SetPlatformTextureSettings(platformSettings);
        }
#endif
    }
}