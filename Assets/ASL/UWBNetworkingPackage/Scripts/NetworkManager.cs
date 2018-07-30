using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;
//using UnityEditor;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// NetworkManager adds the correct Launcher script based on the user 
    /// selected device (in the Unity Inspector).
    /// 
    /// NOTE: For future improvement, this class should:
    /// a) Detect the device and add the launcher automatically
    /// b) Only allow user to select one device
    /// </summary>
    [System.Serializable]
    public class NetworkManager : PunBehaviour
    {
        #region Fields
        #region Public Fields
        /// <summary>
        /// Simple boolean determining if this node is to be the Master Client.
        /// Used to determine which logic branch to take.
        /// 
        /// NOTE: Should be replaced so that people don't have to manually say
        /// whether or not they're the master client.
        /// </summary>
        public bool MasterClient = true;
        
        /// <summary>
        /// Port that is used to send Room Meshes to other clients.
        /// </summary>
        [Tooltip("A port number for devices to communicate through. The port number should be the same for each set of projects that need to connect to each other and share the same Room Mesh.")]
        public int Port;

        // Needed for Photon 
        /// <summary>
        /// The PUN Room name for the ASL network.
        /// </summary>
        [Tooltip("The name of the room that this project will attempt to connect to. This room must be created by a \"Master Client\".")]
        public string RoomName;
        #endregion

        #region Private Fields
        /// <summary>
        /// Constant string for easy access to the Scene Loading object.
        /// </summary>
        private const string SCENE_LOADER_NAME = "SceneLoaderObject";

        /// <summary>
        /// The Global Variables used for the ASL client that don't get washed
        /// away between scene transitions.
        /// </summary>
        private ASL.UI.Menus.Networking.SceneVariableSetter globalVariables;

        /// <summary>
        /// A reference to the ObjectManager used for this class.
        /// </summary>
        private ObjectManager objManager;
        #endregion
        #endregion

        /// <summary>
        /// When Awake, NetworkManager will add the correct Launcher script.
        /// 
        /// NOTE: This shouldn't trigger only in the Awake phase, but should be
        /// handling logic whenever problems with the PUN network occur. Also,
        /// try to move away from idea of attaching a script that separates behavior
        /// based on a variable at startup.
        /// </summary>
        void Awake()
        {
            GameObject MenuUI = GameObject.Find(SCENE_LOADER_NAME);
            if (MenuUI != null)
            {
                globalVariables = MenuUI.GetComponent<ASL.UI.Menus.Networking.SceneVariableSetter>();
                MasterClient = globalVariables.isMasterClient;
                UWBNetworkingPackage.NodeType platform = globalVariables.platform;
                Config.Start(platform);
                //MasterClient = MenuUI.GetComponent<ASL.UI.Menus.Networking.SceneVariableSetter>().isMasterClient;
            }
            else
            {
#if !UNITY_EDITOR && UNITY_WSA_10_0
                Config.Start(NodeType.Hololens);
#elif !UNITY_EDITOR && UNITY_ANDROID
                Config.Start(NodeType.Tango);
#elif !UNITY_EDITOR && UNITY_IOS
                Config.Start(NodeType.iOS);
#else
                Config.Start(NodeType.PC);
#endif
            }

            objManager = gameObject.AddComponent<ObjectManager>();
            
            //Preprocessor directives to choose which component is added.  Note, master client still has to be hard coded
            //Haven't yet found a better solution for this

#if !UNITY_WSA_10_0 && !UNITY_ANDROID
            RoomHandler.Start();

            if (MasterClient)
            {
                gameObject.AddComponent<MasterClientLauncher_PC>();
                //new Config.AssetBundle.Current(); // Sets some items
            }
            else
            {
                gameObject.AddComponent<ReceivingClientLauncher_PC>();
                // get logic for setting nodetype appropriately

                // new Config.AssetBundle.Current(); // Sets some items
            }
#elif !UNITY_EDITOR && UNITY_WSA_10_0
            RoomHandler.Start();

            if (MasterClient)
            {
                gameObject.AddComponent<MasterClientLauncher_Hololens>();
            }
            else
            {
                gameObject.AddComponent<ReceivingClientLauncher_Hololens>();
            }
            //gameObject.AddComponent<HoloLensLauncher>();

            //UWB_Texturing.TextManager.Start();

            //// ERROR TESTING REMOVE
            //string[] filelines = new string[4];
            //filelines[0] = "Absolute asset root folder = " + Config_Base.AbsoluteAssetRootFolder;
            //filelines[1] = "Private absolute asset root folder = " + Config_Base.absoluteAssetRootFolder;
            //filelines[2] = "Absolute asset directory = " + Config.AssetBundle.Current.CompileAbsoluteAssetDirectory();
            //filelines[3] = "Absolute bundle directory = " + Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();

            //string filepath = System.IO.Path.Combine(Application.persistentDataPath, "debugfile.txt");
            //System.IO.File.WriteAllLines(filepath, filelines);
#elif UNITY_ANDROID
            bool isTango = true;
            if (isTango)
            {
                //Config.Start(NodeType.Tango);
                RoomHandler.Start();
                if (MasterClient)
                {
                    throw new System.Exception("Tango master client not yet implemented! If it is, then update NetworkManager where you see this error message.");
                }
                else { 
                    gameObject.AddComponent<ReceivingClientLauncher_Tango>();
                }
            }
            else
            {
               // Config.Start(NodeType.Android);
                RoomHandler.Start();
                if (MasterClient)
                {
                    throw new System.Exception("Android master client not yet implemented! If it is, then update NetworkManager where you see this error message.");
                }
                else
                {
                    gameObject.AddComponent<ReceivingClientLauncher_Android>();
                }
            }
#else
            RoomHandler.Start();

            if (MasterClient)
            {
                gameObject.AddComponent<MasterClientLauncher_PC>();
                //new Config.AssetBundle.Current(); // Sets some items
            }
            else
            {
                gameObject.AddComponent<ReceivingClientLauncher_PC>();
                // get logic for setting nodetype appropriately

                // new Config.AssetBundle.Current(); // Sets some items
            }
#endif
        }

        /// <summary>
        /// Cleans up threads related to the backend TCP server used for data
        /// transferral.
        /// </summary>
        protected void OnApplicationQuit()
        {
#if UNITY_WSA_10_0 && !UNITY_EDITOR
            ServerFinder_Hololens.KillThreads();
#else
            ServerFinder.KillThreads();
#endif
        }

        /// <summary>
        /// Instantiate an object through the ObjectManager for all connected 
        /// clients. Should only be called through ObjectInteractionManager (if it
        /// still exists).
        /// 
        /// NOTE: Not currently used, working, or implemented as intended. Do not
        /// use.
        /// </summary>
        /// 
        /// <param name="go">
        /// The object to be instantiated.
        /// </param>
        /// 
        /// <returns>
        /// A clone of the GameObject created across the network.
        /// </returns>
        public GameObject Instantiate(GameObject go)
        {
            if(objManager != null)
            {
                return objManager.Instantiate(go);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Instantiate a prefab through the ObjectManager for all connected
        /// clients. Should only be called through ObjectInteractionManager (if it
        /// still exists).
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab existing in the Resources folder that you
        /// want to instantiate on all connected clients.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject instantiated.
        /// </returns>
        public GameObject Instantiate(string prefabName)
        {
            if(objManager != null)
            {
                return objManager.Instantiate(prefabName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Instantiate a prefab through the ObjectManager for all connected
        /// clients. Should only be called through ObjectInteractionManager (if it 
        /// still exists). This object is shifted to the specified position,
        /// orientation, and size.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab existing in the Resources folder that you 
        /// want to instantiate on all connected clients.
        /// </param>
        /// <param name="position">
        /// The position of the created object.
        /// </param>
        /// <param name="rotation">
        /// The orientation of the created object.
        /// </param>
        /// <param name="scale">
        /// The size of the created object.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject instantiated.
        /// </returns>
        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if(objManager != null)
            {
                return objManager.Instantiate(prefabName, position, rotation, scale);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Instantiate a prefab through the ObjectManager for all connected
        /// clients. Should only be called through ObjectInteractionManager (if it 
        /// still exists). This object has its ownership locked to this client.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab to be instantiated on all clients.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject instantiated.
        /// </returns>
        public GameObject InstantiateOwnedObject(string prefabName)
        {
            if(objManager != null)
            {
                return objManager.InstantiateOwnedObject(prefabName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Destroys an instantiated network object across all clients.
        /// </summary>
        /// 
        /// <param name="go">
        /// The name of the GameObject to be destroyed across all clients.
        /// </param>
        /// 
        /// <returns>
        /// True if the GameObject was found and destroyed on at least this 
        /// client. False otherwise.
        /// </returns>
        public bool Destroy(GameObject go)
        {
            if (go == null)
            {
                return true;
            }
            else
            {
                OwnableObject ownershipManager = go.GetComponent<OwnableObject>();
                if (ownershipManager.Take())
                {
                    UnityEngine.GameObject.Destroy(go);
                    // Calling Unity's Destroy mechanism kills the object by triggering an OnDestroy call in the ObjectManager
                }

                return true;
            }
        }

        /// <summary>
        /// Forcibly takes ownership of an object.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The GameObject whose ownership you would like to have.
        /// </param>
        /// 
        /// <returns>
        /// True if the object can be owned. False otherwise.
        /// </returns>
        public bool RequestOwnership(GameObject obj)
        {
            OwnableObject ownershipManager = obj.GetComponent<OwnableObject>();
            if(ownershipManager != null)
            {
                return ownershipManager.Take();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Restricts ownership of an ownable ASL GameObject to certain ASL 
        /// PUN IDs.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The GameObject whose ownership availability is being restricted.
        /// </param>
        /// <param name="whiteListIDs">
        /// The list of ASL PUN IDs of members that are allowed access to 
        /// requesting GameObject ownership.
        /// </param>
        /// 
        /// <returns>
        /// True if the whitelist can be processed and the object can be owned.
        /// False otherwise.
        /// </returns>
        public bool RestrictOwnership(GameObject obj, List<int> whiteListIDs)
        {
            OwnableObject ownershipManager = obj.GetComponent<OwnableObject>();
            List<int> returnValue = ownershipManager.RestrictToYourself();
            if (returnValue != null)
            {
                if (whiteListIDs != null)
                {
                    ownershipManager.WhiteListPlayerID(whiteListIDs);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Relieves ownership restrictions related to a GameObject.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The GameObject whose ownership is to be opened to all clients.
        /// </param>
        /// 
        /// <returns>
        /// True if the GameObject's restrictions were lifted. False otherwise.
        /// </returns>
        public bool UnRestrictOwnership(GameObject obj)
        {
            OwnableObject ownershipManager = obj.GetComponent<OwnableObject>();
            return ownershipManager.UnRestrict();
        }

        /// <summary>
        /// Whitelists additional IDS as other client IDs that can also request 
        /// ownership of a GameObject. Doesn't work if your ID isn't whitelisted 
        /// or if object ownership is not restricted.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The GameObject whose whitelist is being affected.
        /// </param>
        /// <param name="playerIDs">
        /// A list that holds the intended whitelisted player IDs. IDs are appended
        /// to this list.
        /// </param>
        public void WhiteListOwnership(GameObject obj, List<int> playerIDs)
        {
            obj.GetComponent<OwnableObject>().WhiteListPlayerID(playerIDs);
        }

        // Ownership must be restricted before blacklisting can take effect
        /// <summary>
        /// Blacklists additional IDs as other client IDs that can't request 
        /// ownership of a GameObject. Doesn't work if your ID isn't whitelisted
        /// or if object ownership is not restricted.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The GameObject whose blacklist is getting updated.
        /// </param>
        /// <param name="playerIDs">
        /// The player IDs that will be appended to the GameObject's blacklist
        /// for object ownership requests.
        /// </param>
        public void BlackListOwnership(GameObject obj, List<int> playerIDs)
        {
            obj.GetComponent<OwnableObject>().BlackListPlayerID(playerIDs);
        }

        /// <summary>
        /// Sends Tango Mesh.
        /// 
        /// NOTE: Should not be in the NetworkManager.
        /// </summary>
        public void SendTangoMesh()
        {
            gameObject.GetComponent<ReceivingClientLauncher_Tango>().SendTangoMesh();
        }

        //-----------------------------------------------------------------------------
        // Legacy Code:

        ///// <summary>
        ///// This is a HoloLens specific method
        ///// This method allows a HoloLens developer to send a Room Mesh when triggered by an event
        ///// This is here because HoloLensLauncher is applied at runtime
        ///// In the HoloLensDemo, this method is called when the phrase "Send Mesh" is spoken and heard by the HoloLens
        ///// </summary>
        //#if UNITY_WSA_10_0
        //        public void HoloSendMesh()
        //        { 
        //            gameObject.GetComponent<MasterClientLauncher_Hololens>().SendMesh();

        //        }
        //#endif
    }
}
