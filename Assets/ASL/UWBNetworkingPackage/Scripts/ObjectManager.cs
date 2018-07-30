using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// This class encapsulates all network communications that handle
    /// synchronization of anything relating to ASL objects. Though the
    /// Object Interaction Manager also exists, it exists as a wrapper 
    /// that funnels network control through this class.
    /// </summary>
    public class ObjectManager : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// A reference to the Resource folder for this project. Could be removed
        /// and integrated throughout the project.
        /// </summary>
        private string resourceFolderPath;

        /// <summary>
        /// A list of items that do not need to be synchronized across clients.
        /// This includes GameObjects that track exclusively local-state-dependent
        /// factors.
        /// </summary>
        private List<GameObject> nonSyncItems;
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when
        /// this class is enabled. Registers with the PhotonNetwork OnEventCall
        /// listener, sets the Resources folder path, and sets the non-synchronizing
        /// ASL items.
        /// </summary>
        public void Awake()
        {
            PhotonNetwork.OnEventCall += OnEvent;
            resourceFolderPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "ASL/Resources");
            
            SetNonAutoSyncItems();
        }

        /// <summary>
        /// Instantiate a prefab across all connected clients at a given position,
        /// orientation, and size.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab that exists in the Resources folder that will
        /// be instantiated.
        /// </param>
        /// <param name="position">
        /// The position to instantiate a prefab at.
        /// </param>
        /// <param name="rotation">
        /// The orientation to instantiate a prefab at.
        /// </param>
        /// <param name="scale">
        /// The size to instantiate a prefab at.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject that has been instantiated.
        /// </returns>
        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject go = Instantiate(prefabName);
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.localScale = scale;

            return go;
        }

        /// <summary>
        /// Instantiates a GameObject clone across all connected clients.
        /// 
        /// NOTE: This is likely not working as intended. Not recommended for
        /// current usage.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to clone and propagate across all connected clients.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject clone that was reinstantiated.
        /// </returns>
        public GameObject Instantiate(GameObject go)
        {
            go = HandleLocalLogic(go);
            RaiseInstantiateEventHandler(go);

            return go;
        }
        
        /// <summary>
        /// Instantiate a prefab across all connected clients. Emulates PUN object
        /// creation across the PUN network.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab to instantiate across all connected clients.
        /// Must exist in the Resources folder.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject instantiated.
        /// </returns>
        public GameObject Instantiate(string prefabName)
        {
            GameObject localObj = InstantiateLocally(prefabName);

            if (localObj != null)
            {
                RaiseInstantiateEventHandler(localObj);
            }
            return localObj;
        }

        /// <summary>
        /// Instantiate a prefab across all connected clients and immediately lock
        /// its ownership availability to this client.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab to instantiate.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject instantiated.
        /// </returns>
        public GameObject InstantiateOwnedObject(string prefabName)
        {
            GameObject localObj = InstantiateLocally(prefabName);

            if(localObj != null)
            {
                RaiseInstantiateEventHandler(localObj);
                int[] whiteListIDs = new int[1];
                whiteListIDs[0] = PhotonNetwork.player.ID;
                //UnityEngine.Debug.Log("About to raise restriction event.");
                OwnableObject ownershipManager = localObj.GetComponent<OwnableObject>();
                ownershipManager.SetRestrictions(true, whiteListIDs);
                RaiseObjectOwnershipRestrictionEventHandler(localObj, true, whiteListIDs);
            }
            return localObj;
        }

        /// <summary>
        /// Restrict ASL GameObject control access or release control access. Allows
        /// specificity of blacklisting / whitelisting of PUN IDs.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject whose control is being changed.
        /// </param>
        /// <param name="restricted">
        /// Do you want to restrict availability or open it to everyone?
        /// </param>
        /// <param name="whiteListIDs">
        /// The IDs to allow access to control request.
        /// </param>
        public void SetObjectRestrictions(GameObject go, bool restricted, List<int> whiteListIDs)
        {
            int[] ids = new int[whiteListIDs.Count];
            whiteListIDs.CopyTo(ids);

            RaiseObjectOwnershipRestrictionEventHandler(go, restricted, ids);
        }

        // This function exists to be called when a gameobject is destroyed 
        // since OnDestroy callbacks are local to the GameObject being destroyed
        /// <summary>
        /// This function exists to be called when an ASL GameObject is destroyed
        /// since OnDestroy callbacks are local to the GameObject being destroyed
        /// and (as far as I can tell) cannot be circumvented.
        /// </summary>
        /// 
        /// <param name="objectName">
        /// The name of the GameObject that is being destroyed.
        /// </param>
        /// <param name="viewID">
        /// The PUN viewID of the GameObject that is being destroyed.
        /// </param>
        public void DestroyHandler(string objectName, int viewID)
        {
            GameObject go = LocateObjectToDestroy(objectName, viewID);
            RaiseDestroyObjectEventHandler(objectName, viewID);
            HandleLocalDestroyLogic(go);
        }

        /// <summary>
        /// Forces another player to synchronize their scene with this client's.
        /// </summary>
        /// 
        /// <param name="otherPlayerID">
        /// The PUN ID of the player being forcibly synchronized.
        /// </param>
        public void ForceSyncScene(int otherPlayerID)
        {
            RaiseSyncSceneEventHandler(otherPlayerID, true);
        }
        
        #region Private Methods
#region Non Sync Items
        /// <summary>
        /// Sets the items that will not be synchronized between clients (i.e.
        /// ASL objects that handle local logic or platform-specific stuff).
        /// </summary>
        private void SetNonAutoSyncItems()
        {
            nonSyncItems = new List<GameObject>();
            List<GameObject> nonSyncGOs = new List<GameObject>();

            foreach(RoomManager roomManager in GameObject.FindObjectsOfType<RoomManager>())
            {
                nonSyncGOs.Add(roomManager.gameObject);
            }
            foreach(ASL.Manipulation.Objects.ObjectInteractionManager objInteractionManager in GameObject.FindObjectsOfType<ASL.Manipulation.Objects.ObjectInteractionManager>())
            {
                nonSyncGOs.Add(objInteractionManager.gameObject);
            }
            foreach(NetworkManager networkManager in GameObject.FindObjectsOfType<NetworkManager>())
            {
                nonSyncGOs.Add(networkManager.gameObject);
            }
            foreach(ASL.Adapters.PUN.RPCManager rpcManager in GameObject.FindObjectsOfType<ASL.Adapters.PUN.RPCManager>())
            {
                nonSyncGOs.Add(rpcManager.gameObject);
            }
            foreach(ObjectManager objManager in GameObject.FindObjectsOfType<ObjectManager>())
            {
                nonSyncGOs.Add(objManager.gameObject);
            }
            foreach(Camera cam in GameObject.FindObjectsOfType<Camera>())
            {
                nonSyncGOs.Add(cam.gameObject);
            }

            nonSyncItems = RefineNonSyncGOList(nonSyncGOs);
        }

        /// <summary>
        /// Essentially just converts the non synchronizing item list to a hash to
        /// weed out redundancies.
        /// </summary>
        /// <param name="goList"></param>
        /// <returns></returns>
        private List<GameObject> RefineNonSyncGOList(IEnumerable<GameObject> goList)
        {
            List<GameObject> refinedGOList = new List<GameObject>();

            foreach(GameObject go in goList)
            {
                if (!refinedGOList.Contains(go))
                {
                    refinedGOList.Add(go);
                }
            }

            return refinedGOList;
        }
#endregion

        /// <summary>
        /// Instantiates a prefab locally and handles logic for attaching necessary
        /// scripts and handling PUN interactions.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab to instantiate.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject instantiated.
        /// </returns>
        private GameObject InstantiateLocally(string prefabName)
        {
            bool connected = PhotonNetwork.connectedAndReady;
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            // safeguard
            if (!connected)
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + PhotonNetwork.connectionStateDetailed);
                return null;
            }

            // retrieve PUN object from cache
            GameObject prefabGo;
            if (!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ".");
                return null;
            }
#if UNITY_EDITOR
            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(prefabGo) as GameObject;
#else
            GameObject go = GameObject.Instantiate(prefabGo);
#endif
            go.name = prefabGo.name;

            HandleLocalLogic(go);
            RegisterObjectCreation(go, prefabName);

            return go;
        }

        /// <summary>
        /// A copy of the PUN method that determines if a prefab is stored in the 
        /// PUN cache. If the prefab has been instantiated and cached, it should
        /// theoretically grab the prefab from the cache before searching the 
        /// Resources folder for the prefab again. Currently not quite implemented
        /// as intended due to worries of parsing and verifying availability of
        /// PUN PrefabCache.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="prefabGo"></param>
        /// <returns></returns>
        private bool RetrieveFromPUNCache(string prefabName, out GameObject prefabGo)
        {
            bool UsePrefabCache = true;

            if (!UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out prefabGo))
            {
                //List<string> prefabFolderPossibilities = new List<string>();
                prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));
                if (prefabGo == null)
                {
                    string directory = resourceFolderPath;
                    directory = string.Join("/", directory.Split(new char[1] { '/' }));
                    //directory = ConvertToResourcePath(directory);
                    prefabGo = ResourceDive(prefabName, directory);
                }
                if (UsePrefabCache)
                {
                    PhotonNetwork.PrefabCache.Add(prefabName, prefabGo);
                }
            }

            return prefabGo != null;
        }

        /// <summary>
        /// Handles the local client logic associated with turning a GameObject
        /// into an ASL synchronized GameObject.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private GameObject HandleLocalLogic(GameObject go)
        {
            go = HandlePUNStuff(go);
            go = SynchCustomScripts(go);

            return go;
        }

        /// <summary>
        /// Handles the logic associated with overriding Photon Unity Networking
        /// (PUN) protocols and limitations to allow ASL to properly synchronize
        /// between clients.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to have PUN stuff handled for.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject after it has been modified.
        /// </returns>
        private GameObject HandlePUNStuff(GameObject go)
        {
            return HandlePUNStuff(go, null);
        }

        /// <summary>
        /// Handles the logic associated with overriding Photon Unity Networking
        /// (PUN) protocols and limitations to allow ASL to properly synchronize
        /// between clients. Manually overrides and sets the view ID of the 
        /// object and its children in a first-come-first-serve manner.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to be modified.
        /// </param>
        /// <param name="viewIDs">
        /// The PUN object ViewIDs to associate with the GameObject and its 
        /// children.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject after it has been modified.
        /// </returns>
        private GameObject HandlePUNStuff(GameObject go, int[] viewIDs)
        {
            go = AttachPhotonViews(go);
            go = AttachPhotonTransformViews(go);
            if(viewIDs == null)
            {
                SetViewIDs(ref go);
            }
            else
            {
                if(go.GetPhotonViewsInChildren().Length != viewIDs.Length)
                {
                    UnityEngine.Debug.LogError("Prefab mismatch encountered! Number of children encountered differs between nodes.");
                }
                SynchViewIDs(go, viewIDs);
            }

            AddressFinalPUNSynch(go);

            return go;
        }

#region PUN Stuff
        /// <summary>
        /// Handles parsing of the Resources folder to find a specified prefab.
        /// Performs a recursive depth-first search.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab to be instantiated.
        /// </param>
        /// <param name="directory">
        /// The directory to be searching in. Initially this will be the Resources
        /// folder, but can be any of the subfolders since it is a recursive 
        /// search.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject that is instantiated from a prefab found in the dive.
        /// Null if nothing is found.
        /// </returns>
        private GameObject ResourceDive(string prefabName, string directory)
        {
            string resourcePath = ConvertToResourcePath(directory, prefabName);
            GameObject prefabGo = (GameObject)Resources.Load(resourcePath, typeof(GameObject));

            if (prefabGo == null)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string dir in subdirectories)
                {
                    prefabGo = ResourceDive(prefabName, dir);
                    if (prefabGo != null)
                    {
                        break;
                    }
                }
            }

            return prefabGo;
        }

        /// <summary>
        /// Due to the peculiarities of the Unity Resources.Load method, a
        /// conversion is required from a string of a normal file / folder path 
        /// to one that can be read by Resources.Load.
        /// </summary>
        /// 
        /// <param name="directory">
        /// The directory to convert.
        /// </param>
        /// <param name="prefabName">
        /// The name of the prefab to search for in the directory.
        /// </param>
        /// 
        /// <returns>
        /// A string that can be plugged into Resources.Load.
        /// </returns>
        private string ConvertToResourcePath(string directory, string prefabName)
        {
            string resourcePath = directory.Substring(directory.IndexOf("Resources") + "Resources".Length);
            if (resourcePath.Length > 0)
            {
                resourcePath = resourcePath.Substring(1) + '/' + prefabName;
                resourcePath.Replace('\\', '/');
            }
            else
            {
                resourcePath = prefabName;
            }

            return resourcePath;
            //return string.Join("/", directory.Split('\\')) + "/" + prefabName;
        }

        /// <summary>
        /// Handles attachment of PhotonView class script to the GameObject
        /// specified.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to have the PhotonView attached to.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject after it's been modified.
        /// </returns>
        private GameObject AttachPhotonViews(GameObject go)
        {
            PhotonView pv = go.GetComponent<PhotonView>();
            if(pv == null)
            {
                // Attach photon view
                pv = go.AddComponent<PhotonView>();
            }
            pv.synchronization = ViewSynchronization.UnreliableOnChange;

            for(int i = 0; i < go.transform.childCount; i++)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                AttachPhotonViews(child);
            }

            return go;
        }

        /// <summary>
        /// Handles attachment of PhotonTransformView class scripts to the
        /// GameObject specified.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to have the PhotonTransformView attached to.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject after it's been modified.
        /// </returns>
        private GameObject AttachPhotonTransformViews(GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            UWBPhotonTransformView ptv = go.GetComponent<UWBPhotonTransformView>();

            if(ptv == null)
            {
                ptv = go.AddComponent<UWBPhotonTransformView>();
            }

            ptv.enableSyncPos();
            ptv.enableSyncRot();
            ptv.enableSyncScale();

            PhotonView view = go.GetComponent<PhotonView>();
            if(view == null)
            {
                UnityEngine.Debug.LogError("Encountered invalid Photon view state when attaching photon transform view. Aborting");
                return null;
            }

            if (view.ObservedComponents == null)
            {
                view.ObservedComponents = new List<Component>();
            }
            view.ObservedComponents.Add(ptv);

            for(int i = 0; i < go.transform.childCount; i++)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                AttachPhotonTransformViews(child);
            }

            return go;
        }

        /// <summary>
        /// Sets the Photon View IDs of new ASL objects. Utilizes the PhotonNetwork's
        /// ability to dynamically allocate view IDs and register them.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to allocate Photon ViewIDs for.
        /// </param>
        /// 
        /// <returns>
        /// The modified GameObject.
        /// </returns>
        private GameObject SetViewIDs(ref GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            PhotonView[] views = go.GetPhotonViewsInChildren();
            
            //Debug.Log("Found " + views.Length + " photon views in " + go.name + " object and its children");
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++) // ignore the main gameobject
            {
                //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
                viewIDs[i] = PhotonNetwork.AllocateViewID();
                //Debug.Log("Allocated an id of " + viewIDs[i]);
                views[i].viewID = viewIDs[i];
                views[i].instantiationId = viewIDs[i];
                //Debug.Log("Assigning view id of " + viewIDs[i] + ", so now the view id is " + go.GetPhotonView().viewID + " for gameobject " + go.name);
                networkingPeer.RegisterPhotonView(views[i]);
            }

            return go;
        }

        /// <summary>
        /// Sets the Photon ViewIDs of a GameObject and its children to manually
        /// passed in ViewIDs. This is used explicitly by clients who did not
        /// instantiate an object, but instead are fulfilling the request to
        /// instantiate an object from other clients.
        /// </summary>
        /// 
        /// <param name="go">
        /// GameObject that is having its ViewIDs manually set.
        /// </param>
        /// <param name="viewIDs">
        /// The ViewIDs to use to manually set.
        /// </param>
        private void SynchViewIDs(GameObject go, int[] viewIDs)
        {
            PhotonView[] PVs = go.GetPhotonViewsInChildren();
            for (int i = 0; i < PVs.Length; i++)
            {
                PVs[i].viewID = viewIDs[i];
                PVs[i].instantiationId = viewIDs[i];
            }
        }
        
        /// <summary>
        /// This is a copy of a step in the PUN process for object synchronization
        /// on their networks. Not explored thoroughly and just kept here to ensure
        /// functionality. Might be removable, not tested.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject being synchronized.
        /// </param>
        private void AddressFinalPUNSynch(GameObject go)
        {
            NetworkingPeer networkingPeer = PhotonNetwork.networkingPeer;

            // Send to others, create info
            //Hashtable instantiateEvent = networkingPeer.SendInstantiate(prefabName, position, rotation, group, viewIDs, data, false);
            RaiseEventOptions options = new RaiseEventOptions();
            bool isGlobalObject = false;
            options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            for (int i = 0; i < photonViews.Length; i++)
            {
                photonViews[i].didAwake = false;
                //photonViews[i].viewID = 0; // why is this included in the original?

                //photonViews[i].prefix = objLevelPrefix;
                photonViews[i].prefix = networkingPeer.currentLevelPrefix;
                //photonViews[i].instantiationId = instantiationId;
                photonViews[i].instantiationId = go.GetComponent<PhotonView>().viewID;
                photonViews[i].isRuntimeInstantiated = true;
                //photonViews[i].instantiationDataField = incomingInstantiationData;
                photonViews[i].instantiationDataField = null;

                photonViews[i].didAwake = true;
                //photonViews[i].viewID = viewsIDs[i];    // with didAwake true and viewID == 0, this will also register the view
                //photonViews[i].viewID = viewIDs[i];
                photonViews[i].viewID = go.GetPhotonViewsInChildren()[i].viewID;
            }

            // Send OnPhotonInstantiate callback to newly created GO.
            // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
            go.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(PhotonNetwork.player, PhotonNetwork.ServerTimestamp, null), SendMessageOptions.DontRequireReceiver);

            // Instantiate the GO locally (but the same way as if it was done via event). This will also cache the instantiationId
            //return networkingPeer.DoInstantiate(instantiateEvent, networkingPeer.LocalPlayer, prefabGo);

            for (int i = 0; i < go.transform.childCount; i++)
            {
                // Trigger OnPhotonInstantiate event on all children of the object
                GameObject child = go.transform.GetChild(i).gameObject;
                child.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(PhotonNetwork.player, PhotonNetwork.ServerTimestamp, null), SendMessageOptions.DontRequireReceiver);
            }
        }
#endregion

#region PUN Event Stuff
        /// <summary>
        /// Raises a PUN network event which tells clients to instantiate an 
        /// object.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject being instantiated on other clients.
        /// </param>
        private void RaiseInstantiateEventHandler(GameObject go)
        {
            //Debug.Log("Attempting to raise event for instantiation");

            NetworkingPeer peer = PhotonNetwork.networkingPeer;

            byte[] content = new byte[2];
            ExitGames.Client.Photon.Hashtable instantiateEvent = new ExitGames.Client.Photon.Hashtable();
#if UNITY_EDITOR
            string prefabName = UnityEditor.PrefabUtility.GetPrefabParent(go).name;
#else
            //string prefabName = go.name;
            string prefabName = ObjectInstantiationDatabase.GetPrefabName(go);
#endif
            instantiateEvent[(byte)0] = prefabName;

            if (go.transform.position != Vector3.zero)
            {
                instantiateEvent[(byte)1] = go.transform.position;
            }

            if (go.transform.rotation != Quaternion.identity)
            {
                instantiateEvent[(byte)2] = go.transform.rotation;
            }

            instantiateEvent[(byte)3] = go.transform.localScale;
            
            int[] viewIDs = ExtractPhotonViewIDs(go);
            instantiateEvent[(byte)4] = viewIDs;

            if (peer.currentLevelPrefix > 0)
            {
                instantiateEvent[(byte)5] = peer.currentLevelPrefix;
            }

            instantiateEvent[(byte)6] = PhotonNetwork.ServerTimestamp;
            instantiateEvent[(byte)7] = go.GetPhotonView().instantiationId;

            //RaiseEventOptions options = new RaiseEventOptions();
            //options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

            //Debug.Log("All items packed. Attempting to literally raise event now.");

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.Others;
            PhotonNetwork.RaiseEvent(ASLEventCode.EV_INSTANTIATE, instantiateEvent, true, options);

            //peer.OpRaiseEvent(EV_INSTANTIATE, instantiateEvent, true, null);
        }
        
        /// <summary>
        /// Raises a PUN network event which tells clients to synchronize with
        /// information passed to them through the event (i.e. adds the items
        /// requested). This will only work if it is the master client telling a
        /// master client to synchronize or if the client forces the 
        /// synchronization.
        /// 
        /// Currently, this is a minimal synchronization and doesn't delete
        /// objects or modify them, though this could be easily modified 
        /// using ObjectInstantiationDatabase and ObjectInfoDatabase.
        /// </summary>
        /// 
        /// <param name="otherPlayerID">
        /// The ID of the other player to have synchronize.
        /// </param>
        /// <param name="forceSync">
        /// If not a master client, this client can still force synchronization
        /// by setting this to "true".
        /// </param>
        private void RaiseSyncSceneEventHandler(int otherPlayerID, bool forceSync)
        {
            if (PhotonNetwork.isMasterClient || forceSync)
            {
                List<GameObject> ASLObjectList = GrabAllASLObjects();
                HashSet<GameObject> ASLObjectSet = new HashSet<GameObject>();
                foreach(var go in ASLObjectList)
                {
                    //GameObject root= go.transform.root.gameObject;
                    if(go.transform.parent!= null && go.transform.parent.gameObject.GetComponent<PhotonView>() != null)
                    {
                        continue;
                    }
                    else
                    {
                        ASLObjectSet.Add(go);
                    }
                }
                
                NetworkingPeer peer = PhotonNetwork.networkingPeer;
                foreach (GameObject go in ASLObjectSet)
                {
                    if (nonSyncItems.Contains(go))
                    {
                        continue;
                    }

                    ExitGames.Client.Photon.Hashtable syncSceneData = new ExitGames.Client.Photon.Hashtable();

                    syncSceneData[(byte)0] = otherPlayerID;

#if UNITY_EDITOR
                    string prefabName = UnityEditor.PrefabUtility.GetPrefabParent(go).name;
#else
                    //string prefabName = go.name;
                    string prefabName = ObjectInstantiationDatabase.GetPrefabName(go);
#endif
                    //UnityEngine.Debug.Log("Prefab name = " + prefabName);
                    syncSceneData[(byte)1] = prefabName;

                    if (go.transform.position != Vector3.zero)
                    {
                        syncSceneData[(byte)2] = go.transform.position;
                    }

                    if (go.transform.rotation != Quaternion.identity)
                    {
                        syncSceneData[(byte)3] = go.transform.rotation;
                    }
                    syncSceneData[(byte)4] = go.transform.localScale;

                    int[] viewIDs = ExtractPhotonViewIDs(go);
                    syncSceneData[(byte)5] = viewIDs;

                    if (peer.currentLevelPrefix > 0)
                    {
                        syncSceneData[(byte)6] = peer.currentLevelPrefix;
                    }

                    syncSceneData[(byte)7] = PhotonNetwork.ServerTimestamp;
                    syncSceneData[(byte)8] = go.GetPhotonView().instantiationId;

                    OwnableObject ownershipManager = go.GetComponent<OwnableObject>();
                    syncSceneData[(byte)9] = ownershipManager.IsOwnershipRestricted;
                    List<int> whiteListIDs = ownershipManager.OwnablePlayerIDs;
                    int[] idList = new int[whiteListIDs.Count];
                    whiteListIDs.CopyTo(idList);
                    syncSceneData[(byte)10] = idList;

                    //int numChildren = go.transform.childCount;
                    //syncSceneData[(byte)11] = numChildren;

                    //int childIndex = 0;
                    //for(int i = 12; i< numChildren + 12; i++)
                    //{
                    //    syncSceneData[(byte)i] = go.transform.GetChild(childIndex).gameObject.GetPhotonView().instantiationId;
                    //    childIndex++;
                    //}

                    //RaiseEventOptions options = new RaiseEventOptions();
                    //options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

                    //Debug.Log("All items packed. Attempting to literally raise event now.");

                    RaiseEventOptions options = new RaiseEventOptions();
                    options.Receivers = ReceiverGroup.Others;
                    PhotonNetwork.RaiseEvent(ASLEventCode.EV_SYNCSCENE, syncSceneData, true, options);
                }
            }
        }

        /// <summary>
        /// Raises a PUN event to trigger synchronized destruction of an ASL 
        /// GameObject.
        /// </summary>
        /// 
        /// <param name="objectName">
        /// The name of an ASL GameObject to destroy.
        /// </param>
        /// <param name="viewID">
        /// The Photon ViewID of the GameObject to be destroyed. (Serves as a check
        /// for the correct item since multiple GameObjects can have the same
        /// name.)
        /// </param>
        private void RaiseDestroyObjectEventHandler(string objectName, int viewID)
        {
            //Debug.Log("Attempting to raise event for destruction of object");

            NetworkingPeer peer = PhotonNetwork.networkingPeer;
            
            ExitGames.Client.Photon.Hashtable destroyObjectEvent = new ExitGames.Client.Photon.Hashtable();
            //string objectName = go.name;
            destroyObjectEvent[(byte)0] = objectName;

            // need the viewID
            
            destroyObjectEvent[(byte)1] = viewID;

            destroyObjectEvent[(byte)2] = PhotonNetwork.ServerTimestamp;

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.Others;
            PhotonNetwork.RaiseEvent(ASLEventCode.EV_DESTROYOBJECT, destroyObjectEvent, true, options);
        }

        /// <summary>
        /// Raises a PUN event to force ownership restriction of an ASL GameObject.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to have its ownership restricted.
        /// </param>
        /// <param name="restricted">
        /// Whether to restrict the ownership or to open it up to everyone.
        /// </param>
        /// <param name="ownableIDs">
        /// The IDs that are allowed access to request ownership of an ASL 
        /// object.
        /// </param>
        private void RaiseObjectOwnershipRestrictionEventHandler(GameObject go, bool restricted, int[] ownableIDs)
        {
            NetworkingPeer peer = PhotonNetwork.networkingPeer;

            //UnityEngine.Debug.Log("restricted = " + ((restricted) ? "true" : "false"));

            ExitGames.Client.Photon.Hashtable restrictionEvent = new ExitGames.Client.Photon.Hashtable();
            PhotonView pv = go.GetComponent<PhotonView>();
            restrictionEvent[(byte)0] = go.name;
            restrictionEvent[(byte)1] = (pv == null) ? 0 : pv.viewID;
            restrictionEvent[(byte)2] = restricted;
            restrictionEvent[(byte)3] = ownableIDs;
            restrictionEvent[(byte)4] = PhotonNetwork.ServerTimestamp;

            //foreach(int id in ownableIDs)
            //{
            //    UnityEngine.Debug.Log("Sending ownableID of " + id);
            //}

            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.All;
            
            PhotonNetwork.RaiseEvent(ASLEventCode.EV_SYNC_OBJECT_OWNERSHIP_RESTRICTION, restrictionEvent, true, options);
        }
        
        /// <summary>
        /// A hook into the Photon Event listener manager that lets me 
        /// listen for specific events and branch logic the way ASL needs it.
        /// </summary>
        /// 
        /// <param name="eventCode">
        /// The event code associated with the event caught. Custom event codes
        /// can be found in the ASLEventCode class.
        /// </param>
        /// <param name="content">
        /// The information associated with the event.
        /// </param>
        /// <param name="senderID">
        /// The PUN ID of the client who triggered the event.
        /// </param>
        private void OnEvent(byte eventCode, object content, int senderID)
        {
            Debug.Log("OnEvent method triggered.");

            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.Log(string.Format("Custom OnEvent for CreateObject: {0}", eventCode.ToString()));
            }

            if (eventCode.Equals(ASLEventCode.EV_INSTANTIATE))
            {
                RemoteInstantiate((ExitGames.Client.Photon.Hashtable)content);
            }
            else if (eventCode.Equals(ASLEventCode.EV_DESTROYOBJECT))
            {
                RemoteDestroyObject((ExitGames.Client.Photon.Hashtable)content);
            }
            else if (eventCode.Equals(ASLEventCode.EV_JOIN))
            {
                RaiseSyncSceneEventHandler(senderID, false);
            }
            else if (eventCode.Equals(ASLEventCode.EV_SYNCSCENE))
            {
                HandleSyncSceneEvent((ExitGames.Client.Photon.Hashtable)content);
            }
            else if (eventCode.Equals(ASLEventCode.EV_SYNC_OBJECT_OWNERSHIP_RESTRICTION))
            {
                HandleSyncObjectOwnershipRestriction((ExitGames.Client.Photon.Hashtable)content);
            }
        }

        /// <summary>
        /// Handles the unpacking of information associated with a remote ASL
        /// GameObject instantiation request. The event data is parsed and the 
        /// object is reconstructed from the information.
        /// </summary>
        /// 
        /// <param name="eventData">
        /// Information describing the ASL GameObject and its state.
        /// </param>
        private void RemoteInstantiate(ExitGames.Client.Photon.Hashtable eventData)
        {
            string prefabName = (string)eventData[(byte)0];
            Vector3 position = Vector3.zero;
            Vector3 scale = Vector3.one;
            if (eventData.ContainsKey((byte)1))
            {
                position = (Vector3)eventData[(byte)1];
            }
            Quaternion rotation = Quaternion.identity;
            if (eventData.ContainsKey((byte)2))
            {
                rotation = (Quaternion)eventData[(byte)2];
            }

            scale = (Vector3)eventData[(byte)3];

            int[] viewIDs = (int[])eventData[(byte)4];
            if (eventData.ContainsKey((byte)5))
            {
                uint currentLevelPrefix = (uint)eventData[(byte)5];
            }

            int serverTimeStamp = (int)eventData[(byte)6];
            int instantiationID = (int)eventData[(byte)7];

            InstantiateLocally(prefabName, viewIDs, position, rotation, scale);
        }
        
        /// <summary>
        /// Handles local instantiation of an ASL GameObject.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// The name of the prefab that is being instantiated.
        /// </param>
        /// <param name="viewIDs">
        /// The Photon ViewIDs to associate with the prefab and its children upon
        /// instantiation.
        /// </param>
        /// <param name="position">
        /// The position of the GameObject once it's been instantiated.
        /// </param>
        /// <param name="rotation">
        /// The orientation of the GameObject once it's been instantiated.
        /// </param>
        /// <param name="scale">
        /// The size of the GameObject once it's been instantiated.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject that was instantiated. Null if not instantiable (i.e.
        /// the prefab could not be found in the Resources folder).
        /// </returns>
        private GameObject InstantiateLocally(string prefabName, int[] viewIDs, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject prefabGo;
            if (!RetrieveFromPUNCache(prefabName, out prefabGo))
            {
                Debug.LogError("Failed to Instantiate prefab: " + prefabName + ".");
                return null;
            }

#if UNITY_EDITOR
            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(prefabGo) as GameObject;
#else
            GameObject go = GameObject.Instantiate(prefabGo);
#endif
            go.name = prefabGo.name;

            HandleLocalLogic(go, viewIDs);
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.localScale = scale;

            RegisterObjectCreation(go, prefabName);

            return go;
        }

        /// <summary>
        /// Handle the local logic associated with attaching scripts to an
        /// instantiated GameObject to make sure that the generated GameObject
        /// is synchronized possibly.
        /// 
        /// NOTE: This version of the method is to be used on the client that is
        /// receiving the object instantiation event.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to be modified.
        /// </param>
        /// <param name="viewIDs">
        /// The Photon ViewIDs to be manually set for the GameObject and its
        /// children.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject that is being modified.
        /// </returns>
        private GameObject HandleLocalLogic(GameObject go, int[] viewIDs)
        {
            go = HandlePUNStuff(go, viewIDs);
            go = SynchCustomScripts(go);

            return go;
        }

        /// <summary>
        /// Handles the processing of destroying an ASL GameObject after receiving
        /// that event.
        /// </summary>
        /// 
        /// <param name="eventData">
        /// The information specifying the GameObject to destroy.
        /// </param>
        private void RemoteDestroyObject(ExitGames.Client.Photon.Hashtable eventData)
        {
            string objectName = (string)eventData[(byte)0];
            int viewID = (int)eventData[(byte)1];
            int timeStamp = (int)eventData[(byte)2];

            // Handle destroy logic
            GameObject go = LocateObjectToDestroy(objectName, viewID);
            HandleLocalDestroyLogic(go);
        }
        
        /// <summary>
        /// Handles all logic associated with destroying and nulling components
        /// for an ASL GameObject.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject being destroyed.
        /// </param>
        private void HandleLocalDestroyLogic(GameObject go)
        {
            if (go != null)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    HandleLocalDestroyLogic(go.transform.GetChild(i).gameObject);
                }

                PhotonView view = go.GetComponent<PhotonView>();
                if (view != null)
                {
                    // Clear up the local view and delete it from the registration list
                    //PhotonNetwork.networkingPeer.LocalCleanPhotonView(view);

                    view.removedFromLocalViewList = true;
                    int viewID = view.viewID;
                    PhotonNetwork.networkingPeer.photonViewList.Remove(viewID);
                    PhotonNetwork.UnAllocateViewID(viewID);
                }

                GameObject.Destroy(go);

                RegisterObjectDeletion(go, go.name);
            }
        }

        /// <summary>
        /// Handles the unpacking of information associated with a remote ASL
        /// GameObject scene synchronization request. The event data is parsed 
        /// and the scene objects are reconstructed from the information.
        /// </summary>
        /// <param name="eventData"></param>
        private void HandleSyncSceneEvent(ExitGames.Client.Photon.Hashtable eventData)
        {
            int targetPlayerID = (int)eventData[(byte)0];
            if (PhotonNetwork.player.ID == targetPlayerID)
            {
                string prefabName = (string)eventData[(byte)1];
                Vector3 position = Vector3.zero;
                Vector3 scale = Vector3.one;
                if (eventData.ContainsKey((byte)2))
                {
                    position = (Vector3)eventData[(byte)2];
                }
                Quaternion rotation = Quaternion.identity;
                if (eventData.ContainsKey((byte)3))
                {
                    rotation = (Quaternion)eventData[(byte)3];
                }
                scale = (Vector3)eventData[(byte)4];

                int[] viewIDs = (int[])eventData[(byte)5];
                if (eventData.ContainsKey((byte)6))
                {
                    uint currentLevelPrefix = (uint)eventData[(byte)6];
                }

                int serverTimeStamp = (int)eventData[(byte)7];
                int instantiationID = (int)eventData[(byte)8];

                GameObject go = InstantiateLocally(prefabName, viewIDs, position, rotation, scale);

                OwnableObject ownershipManager = go.GetComponent<OwnableObject>();
                bool isOwnershipRestricted = (bool)eventData[(byte)9];
                int[] whiteListIDs = (int[])eventData[(byte)10];
                ownershipManager.SetRestrictions(isOwnershipRestricted, whiteListIDs);

                //// May present issues with theownership settings of prefab children, but that can be remedied in the future
                //int numChildren = (int)eventData[(byte)11];
                //int[] childrenViewIDs = new int[numChildren];
                //int idIndex = 0;
                //for(int i = 12; i < numChildren + 12; i++)
                //{
                //    childrenViewIDs[idIndex] = (int)eventData[(byte)i];
                //    // Attach the ownership script to the children and try to ensure that the PhotonView gets set correctly
                //    GameObject child = go.transform.GetChild(idIndex).gameObject;
                //    var ownership = child.AddComponent<OwnableObject>();
                //    var pv = child.AddComponent<PhotonView>();
                //    pv.
                //    go.transform.GetChild(idIndex).gameObject.AddComponent<OwnableObject>();
                //    go.transform.GetChild(idIndex).gameObject.AddComponent<PhotonView>();
                //    ++idIndex;
                //}
            }
        }

        /// <summary>
        /// Handles the unpacking of information associated with a remote ASL
        /// GameObject object ownership synchronization request. The event data 
        /// is parsed and the object is ownership settings are reconstructed
        /// from the information.
        /// </summary>
        /// 
        /// <param name="eventData">
        /// The information associated with the event.
        /// </param>
        private void HandleSyncObjectOwnershipRestriction(ExitGames.Client.Photon.Hashtable eventData)
        {
            string objectName = (string)eventData[(byte)0];
            int viewID = (int)eventData[(byte)1];
            bool restricted = (bool)eventData[(byte)2];
            int[] ownableIDs = (int[])eventData[(byte)3];
            int serverTimeStamp = (int)eventData[(byte)4];
            
            //UnityEngine.Debug.Log("restricted = " + ((restricted) ? "true" : "false"));

            GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
            GameObject go = null;
            foreach(GameObject obj in objs)
            {
                if (obj.name.Equals(objectName))
                {
                    PhotonView pv = obj.GetComponent<PhotonView>();
                    if(pv == null)
                    {
                        continue;
                    }
                    else if(pv.viewID == viewID)
                    {
                        go = obj;
                        break;
                    }
                }
            }

            if(go != null)
            {
                OwnableObject ownershipManager = go.GetComponent<OwnableObject>();
                ownershipManager.SetRestrictions(restricted, ownableIDs);
            }
        }
#endregion
        
        /// <summary>
        /// Helper method to locate the ASL GameObject to destroy using its name
        /// and Photon ViewID. This is because objects hashes will differ
        /// between nodes and also because the object may already be destroyed and
        /// null on the client initiating synchronized object destruction.
        /// </summary>
        /// 
        /// <param name="objectName">
        /// The name of the ASL GameObject to destroy.
        /// </param>
        /// <param name="viewID">
        /// The Photon ViewID of the ASL GameObject to destroy.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject that is to be destroyed. Null if object does not 
        /// exist or can't be located in the scene.
        /// </returns>
        private GameObject LocateObjectToDestroy(string objectName, int viewID)
        {
            GameObject objectToDestroy = null;
            GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
            foreach(GameObject go in goArray)
            {
                if (go.name.Equals(objectName))
                {
                    PhotonView view = go.GetComponent<PhotonView>();
                    if(view != null)
                    {
                        if(viewID == view.viewID)
                        {
                            objectToDestroy = go;
                            break;
                        }
                    }
                }
            }

            return objectToDestroy;
        }

        /// <summary>
        /// Helper method to synchronize the custom scripts that should be attached
        /// to a GameObject after it's instantiated in response to a PUN event
        /// resolution.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to attach scripts to and synchronize.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject after it's been modified.
        /// </returns>
        private GameObject SynchCustomScripts(GameObject go)
        {
            go.AddComponent<UWBNetworkingPackage.OwnableObject>();
            go.AddComponent<UWBNetworkingPackage.DestroyObjectSynchronizer>();

            for(int i = 0; i < go.transform.childCount; i++)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                SynchCustomScripts(child);
            }

            return go;
        }

        /// <summary>
        /// Helper method to grab the Photon ViewIDs from a GameObject and 
        /// its children.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject to grab PhotonViews from.
        /// </param>
        /// 
        /// <returns>
        /// An array of ints, which is a COPY of the Photon ViewIDs from the 
        /// ASL GameObject.
        /// </returns>
        private int[] ExtractPhotonViewIDs(GameObject go)
        {
            PhotonView[] views = go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[views.Length];
            for (int i = 0; i < viewIDs.Length; i++)
            {
                viewIDs[i] = views[i].viewID;
            }

            return viewIDs;
        }
        
        /// <summary>
        /// Helper method to find all ASL objects in the scene.
        /// </summary>
        /// 
        /// <returns>
        /// A list of all GameObjects that qualify as ASL GameObjects in the 
        /// current scene.
        /// </returns>
        private List<GameObject> GrabAllASLObjects()
        {
            List<GameObject> goList = new List<GameObject>();

            PhotonView[] views = GameObject.FindObjectsOfType<PhotonView>();
            foreach (PhotonView view in views)
            {
                GameObject go = view.gameObject;
                if (!goList.Contains(go))
                {
                    goList.Add(go);
                }
            }

            return goList;
        }

        #region Instantiation Database
        /// <summary>
        /// Helper method to ensure that the local databases are updated 
        /// appropriately after items are created or destroyed due to event
        /// resolutions.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject whose creation is to be registered.
        /// </param>
        /// <param name="prefabName">
        /// The string representing the name of the prefab the GameObject is
        /// instantiated from.
        /// </param>
        private void RegisterObjectCreation(GameObject go, string prefabName)
        {
            ObjectInstantiationDatabase.Add(prefabName, go);
        }

        /// <summary>
        /// Helper method to ensure that the local databases are updated 
        /// appropriately after items are created or destroyed due to event 
        /// resolutions.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject whose deletion is to be registered.
        /// </param>
        /// <param name="goName">
        /// The string representing the name of the GameObject that is
        /// to be destroyed.
        /// </param>
        private void RegisterObjectDeletion(GameObject go, string goName)
        {
            ObjectInstantiationDatabase.Remove(go, goName);
        }
#endregion
        #endregion
        #endregion
    }
}