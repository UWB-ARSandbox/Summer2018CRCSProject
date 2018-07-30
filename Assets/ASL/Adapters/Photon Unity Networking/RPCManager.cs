using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASL.Adapters.PUN
{
    // Reference PhotonEditor.cs for the stuff that this class derives private
    // methods from
    /// <summary>
    /// Handles Photon Unity Networking (PUN) RPC (Remote Procedure Call) 
    /// interactions and display in the Unity Editor. PUN RPCs are managed
    /// through the PhotonServerSettings class. This class acts as a medium
    /// outside of the PUN library that allows us access and methods to
    /// better interact with that class for proper PUN RPC manipulation.
    /// </summary>
    public class RPCManager : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// The list of RPCs currently set in the PhotonServerSettings file, 
        /// which should represent the RPCs currently registered for this client.
        /// </summary>
        public List<string> RPCList;

        /// <summary>
        /// Encapsulation of messages used by this class. Includes error and 
        /// warning messages.
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// "The passed RPC name is not an RPC."
            /// </summary>
            public static string IsNotRPC = "The passed RPC name is not an RPC.";

            /// <summary>
            /// Handles messages related to titles.
            /// </summary>
            public static class Titles
            {
                /// <summary>
                /// "PUN RPC List"
                /// </summary>
                public static string RefreshRPCList = "PUN RPC List";
            }

            /// <summary>
            /// Handles generic messages.
            /// </summary>
            public static class Messages
            {
                /// <summary>
                /// "Refresh RPC List?"
                /// </summary>
                public static string RefreshRPCList = "Refresh RPC List?";
            }

            /// <summary>
            /// Handles confirmation messages.
            /// </summary>
            public static class OK
            {
                /// <summary>
                /// "Yes"
                /// </summary>
                public static string RefreshRPCList = "Yes";
            }

            /// <summary>
            /// Handles cancellation messages.
            /// </summary>
            public static class Cancel
            {
                /// <summary>
                /// "No"
                /// </summary>
                public static string RefreshRPCList = "No";
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when 
        /// this class is enabled. Initializes the displayed PUN RPC list.
        /// </summary>
        public void Awake()
        {
            updateStoredRPCList();
        }

        /// <summary>
        /// While in Editor mode, repeatedly pulls the PUN RPC list from the 
        /// PhotonServerSettings file and updates the displayed list. Assumes 
        /// the Editor is not in Play mode, because synchronization problems 
        /// could occur if connected to other clients during PUN RPC list update.
        /// </summary>
        public void FixedUpdate()
        {
#if UNITY_EDITOR
            // Make sure not to dynamically change RPC methods while any of 
            // the clients are running! Will cause a desync issue, most likely
            if (!EditorApplication.isPlaying)
            {
                RefreshRPCList();
                updateStoredRPCList();
            }
#endif
        }


        /// <summary>
        /// Pulls the PUN RPC list from the PhotonServerSettings file and updates
        /// the locally stored copy of PUN RPC methods.
        /// 
        /// NOTE: Cannot reference PhotonEditor class (may be due to assembly 
        /// linkage?) -> If you can figure out a way to reference this class, 
        /// USE THEIR METHODS INSTEAD OF THE LOGIC WHICH WAS COPY PASTED
        /// </summary>
        public static void RefreshRPCList()
        {
#if UNITY_EDITOR
            //if(EditorUtility.DisplayDialog(Text.Titles.RefreshRPCList, Text.Messages.RefreshRPCList, Text.OK.RefreshRPCList))
            //{
                clearPUNRPCFileRpcList();
                triggerPUNRPCFileRefresh();
            //}
#else
            // commenting out to test build.
            //ClearRpcList();
            //UpdateRpcList();
#endif
        }

        /// <summary>
        /// Checks whether a given string matches any RPC methods. Expects 
        /// proper capitalization. Displays warning message if RPC is not 
        /// encountered.
        /// </summary>
        /// 
        /// <param name="RPCName">
        /// The name of a method you want to check.
        /// </param>
        /// 
        /// <returns>
        /// True if this exists as an RPC in the PhotonServerSettings 
        /// file with the exact same name (i.e. capitalization). False otherwise.
        /// </returns>
        public static bool IsAnRPC(string RPCName)
        {
            if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(RPCName))
            {
                return true;
            }
            else
            {
                Debug.LogWarning(Text.IsNotRPC + " (" + RPCName + ")");
                return false;
            }
        }

        #region Private Methods
        
        /// <summary>
        /// Refreshes the locally stored copy of PUN RPCs found in 
        /// PhotonServerSettings and then refreshes the list of PUN RPCs 
        /// displayed in the Editor to match.
        /// </summary>
        private void updateStoredRPCList()
        {
            string[] rpcArray = new string[PhotonNetwork.PhotonServerSettings.RpcList.Count];
            PhotonNetwork.PhotonServerSettings.RpcList.CopyTo(rpcArray);
            RPCList = new List<string>(rpcArray);
        }

        #region Reflection
        /// <summary>
        /// Helper reflection method designed to get all classes that inherit 
        /// from a given base class. This allows you to parse through classes 
        /// at a given hierarchical level.
        /// </summary>
        /// 
        /// <param name="aBaseClass">
        /// The type of the class you want to set as the parent of the returned 
        /// classes (e.g. typeof(MonoBehavior)).
        /// </param>
        /// 
        /// <returns>
        /// An array of class types that inherit from the type specified by 
        /// aBaseClass. Returns an empty, non-null array if no classes inherit
        /// from the class specified by aBaseClass.
        /// </returns>
        private static System.Type[] getAllSubTypesInScripts(System.Type aBaseClass)
        {
            var result = new System.Collections.Generic.List<System.Type>();
            System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var A in AS)
            {
                // this skips all but the Unity-scripted assemblies for RPC-list creation. You could remove this to search all assemblies in project
                if (!A.FullName.StartsWith("Assembly-"))
                {
                    // Debug.Log("Skipping Assembly: " + A);
                    continue;
                }

                //Debug.Log("Assembly: " + A.FullName);
                System.Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(aBaseClass))
                    {
                        result.Add(T);
                    }
                }
            }
            return result.ToArray();
        }
        #endregion

        #region PhotonServerSettingsAdapter
        #region PhotonServerSettings Negotiation
        /// <summary>
        /// Dynamically searches for PUN RPCs that exist in the current 
        /// implementation on the current client, clears the PUN RPC list in 
        /// the PhotonServerSettings file, and updates the PUN RPC list in the 
        /// PhotonServerSettings file. This does not directly affect this class, 
        /// but allows for access to the class that is otherwise restricted by 
        /// the imported PUN library.
        /// </summary>
        private static void triggerPUNRPCFileRefresh()
        {
            List<string> additionalRpcs = new List<string>();
            HashSet<string> currentRpcs = new HashSet<string>();

            var types = getAllSubTypesInScripts(typeof(MonoBehaviour));

            int countOldRpcs = 0;
            foreach (var mono in types)
            {
                MethodInfo[] methods = mono.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (MethodInfo method in methods)
                {
                    bool isOldRpc = false;
#pragma warning disable 618
                    // we let the Editor check for outdated RPC attributes in code. that should not cause a compile warning
                    if (method.IsDefined(typeof(RPC), false))
                    {
                        countOldRpcs++;
                        isOldRpc = true;
                    }
#pragma warning restore 618

                    if (isOldRpc || method.IsDefined(typeof(PunRPC), false))
                    {
                        currentRpcs.Add(method.Name);

                        if (!additionalRpcs.Contains(method.Name) && !PhotonNetwork.PhotonServerSettings.RpcList.Contains(method.Name))
                        {
                            additionalRpcs.Add(method.Name);
                        }
                    }
                }
            }

            if (additionalRpcs.Count > 0)
            {
                //// LIMITS RPC COUNT
                //if (additionalRpcs.Count + PhotonNetwork.PhotonServerSettings.RpcList.Count >= byte.MaxValue)
                //{
                //    if (currentRpcs.Count <= byte.MaxValue)
                //    {
                //        bool clearList = EditorUtility.DisplayDialog(CurrentLang.IncorrectRPCListTitle, CurrentLang.IncorrectRPCListLabel, CurrentLang.RemoveOutdatedRPCsLabel, CurrentLang.CancelButton);
                //        if (clearList)
                //        {
                //            PhotonNetwork.PhotonServerSettings.RpcList.Clear();
                //            PhotonNetwork.PhotonServerSettings.RpcList.AddRange(currentRpcs);
                //        }
                //        else
                //        {
                //            return;
                //        }
                //    }
                //    else
                //    {
                //        EditorUtility.DisplayDialog(CurrentLang.FullRPCListTitle, CurrentLang.FullRPCListLabel, CurrentLang.SkipRPCListUpdateLabel);
                //        return;
                //    }
                //}
#if UNITY_EDITOR
                additionalRpcs.Sort();
                Undo.RecordObject(PhotonNetwork.PhotonServerSettings, "Update PUN RPC-list");
                PhotonNetwork.PhotonServerSettings.RpcList.AddRange(additionalRpcs);
                savePUNRPCFileSettings();
#endif
            }

            if (countOldRpcs > 0)
            {
                convertRpcAttributesInFilesIn("");

                //bool convertRPCs = EditorUtility.DisplayDialog(CurrentLang.RpcFoundDialogTitle, CurrentLang.RpcFoundMessage, CurrentLang.RpcReplaceButton, CurrentLang.RpcSkipReplace);
                //if (convertRPCs)
                //{
                //    PhotonConverter.ConvertRpcAttribute("");
                //}
            }
        }

        /// <summary>
        /// Clears the PUN RPC list stored in the PhotonServerSettings file.
        /// </summary>
        private static void clearPUNRPCFileRpcList()
        {
            PhotonNetwork.PhotonServerSettings.RpcList.Clear();

            //bool clearList = EditorUtility.DisplayDialog(CurrentLang.PUNNameReplaceTitle, CurrentLang.PUNNameReplaceLabel, CurrentLang.RPCListCleared, CurrentLang.CancelButton);
            //if (clearList)
            //{
            //    PhotonNetwork.PhotonServerSettings.RpcList.Clear();
            //    Debug.LogWarning(CurrentLang.ServerSettingsCleanedWarning);
            //}
        }

        /// <summary>
        /// Saves changes to the PhotonServerSettings file.
        /// </summary>
        private static void savePUNRPCFileSettings()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(PhotonNetwork.PhotonServerSettings);
#endif
        }
        #endregion

        /// <summary>
        /// Converts all native Unity RPC attributes to PUNRPC attributes by
        /// reading all file contents for a given folder path, parsing and replacing
        /// any encountered [RPC] or @RPC tags with [PunRPC] and @PunRPC 
        /// respectively, then saves and overwrites the original file.
        /// 
        /// NOTE: "path" parameter defaults to "Assets" if passed an empty or 
        /// null string.
        /// </summary>
        /// 
        /// <param name="path">
        /// The path name should be the relative or absolute path for a directory, 
        /// not a file. This string does not have to be case-sensitive.
        /// 
        /// If left null or empty, path defaults to "Assets".
        /// </param>
        private static void convertRpcAttributesInFilesIn(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }

            List<string> scripts = getScriptsInFolder(path);
            foreach (string file in scripts)
            {
                string text = File.ReadAllText(file);
                string textCopy = text;
                if (file.EndsWith("PhotonConverter.cs"))
                {
                    continue;
                }

                text = text.Replace("[RPC]", "[PunRPC]");
                text = text.Replace("@RPC", "@PunRPC");

                if (!text.Equals(textCopy))
                {
                    File.WriteAllText(file, text);
                    Debug.Log("Converted RPC to PunRPC in: " + file);
                }
            }
        }

        /// <summary>
        /// Gathers all file names for a given folder that end in ".cs", ".js", 
        /// or ".boo".
        /// 
        /// Throws a generic System.Exception if the folder is invalid.
        /// </summary>
        /// 
        /// <param name="folder">
        /// A path name which should be the relative or absolute path for a directory, 
        /// not a file. This string does not have to be case-sensitive.
        /// </param>
        /// 
        /// <returns>
        /// A list of full file names with extensions.
        /// </returns>
        private static List<string> getScriptsInFolder(string folder)
        {
            List<string> scripts = new List<string>();

            try
            {
                scripts.AddRange(Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories));
                scripts.AddRange(Directory.GetFiles(folder, "*.js", SearchOption.AllDirectories));
                scripts.AddRange(Directory.GetFiles(folder, "*.boo", SearchOption.AllDirectories));
            }
            catch (System.Exception ex)
            {
                Debug.Log("Getting script list from folder " + folder + " failed. Exception:\n" + ex.ToString());
            }

            return scripts;
        }
        #endregion
        #endregion
        #endregion
    }
}