using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Custom debug class used to manipulate debugging behavior and avoid 
    /// flooding the project with debugs. Allows user to flip a debugging 
    /// switch that turns debugging messages on and off.
    /// </summary>
    public class Debug : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// Flag that determines whether to display debug messages at all.
        /// </summary>
        public static bool debugging = false;
        #endregion

        #region Methods
        /// <summary>
        /// Similar to Debug.Log except that you can shut off the output of 
        /// these Debug calls.
        /// </summary>
        /// 
        /// <param name="message">
        /// The message to output to the Unity Editor Debug window.
        /// </param>
        public static void Log(string message)
        {
#if !UNITY_EDITOR && UNITY_WSA_10_0
            if (debugging)
            {
                if (UWB_Texturing.TextManager.IsActive)
                {
                    UWB_Texturing.TextManager.SetText(message);
                }
                UnityEngine.Debug.Log(message);
            }
#else
            if(debugging)
                UnityEngine.Debug.Log(message);
#endif
        }

        /// <summary>
        /// Logs an error message as a regular debug message with the "ERROR" 
        /// prefix.
        /// </summary>
        /// 
        /// <param name="message">
        /// The message to output to the Unity Editor Debug window (without the 
        /// "ERROR" prefix).
        /// </param>
        public static void LogError(string message)
        {
            UnityEngine.Debug.Log("ERROR: " + message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// 
        /// <param name="message">
        /// The message to output to the Unity Editor Debug window.
        /// </param>
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
        #endregion
    }
}