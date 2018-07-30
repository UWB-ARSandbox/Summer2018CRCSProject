using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASL.Adapters.Unity
{
#if UNITY_EDITOR
    /// <summary>
    /// Handles startup procedure for ensuring that proper handling of 
    /// Unity-specific shortcomings is addressed for proper usage of ASL 
    /// without needing input from the user.
    /// 
    /// Only triggers in Unity Editor mode. (Might have to be adjusted to work 
    /// properly for built applications.)
    /// </summary>
    [InitializeOnLoad]
    public class Startup
    {
        #region Methods
        /// <summary>
        /// Static constructor. Adds Unity tags and layers to the Unity editor 
        /// as needed for proper ASL usage.
        /// </summary>
        static Startup()
        {
            Debug.Log("ASL Booting...");

            AddTags();
            AddLayers();
        }

        /// <summary>
        /// Adds predefined tags to the Unity tag manager.
        /// </summary>
        private static void AddTags()
        {
            string[] tags =
            {
                "Room"
            };

            foreach(string tag in tags)
            {
                TagsAndLayers.AddTag(tag);
            }
        }

        /// <summary>
        /// Adds predefined layers to the Unity layer manager.
        /// </summary>
        private static void AddLayers()
        {
            string[] layers =
            {
                // Add layers as appropriate / needed here
            };

            foreach(string layer in layers)
            {
                TagsAndLayers.AddTag(layer);
            }
        }
        #endregion
    }
#endif
}