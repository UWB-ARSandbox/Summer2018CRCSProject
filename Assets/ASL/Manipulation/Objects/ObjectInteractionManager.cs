using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects
{
    /// <summary>
    /// Hierarchical head of all object interaction (movement, selection, etc.).
    /// Called by ObjectManager to handle updates of interaction without 
    /// exposing too much complexity to end-users.
    /// 
    /// NOTE: The setup for this class assumes a singular object manipulation, 
    /// which should be not be assumed as it's extremely limiting. Future work 
    /// should rework this class so that it only handles network communications 
    /// for PUN protocols on object updating.
    /// </summary>
    public class ObjectInteractionManager : MonoBehaviour
    {
        #region Fields
        #region Private Fields
        /// <summary>
        /// The type of node this client exists on (PC, Android, etc.).
        /// </summary>
        private UWBNetworkingPackage.NodeType platform;

        /// <summary>
        /// A reference to the Network Manager active in the scene.
        /// </summary>
        private UWBNetworkingPackage.NetworkManager networkManager;
        #endregion

        #region Public Fields
        /// <summary>
        /// A reference to the currently selected object by this user in the scene.
        /// </summary>
        public event ObjectSelectedEventHandler FocusObjectChangedEvent;
        #endregion
        #endregion

        #region Methods
        #region Public Methods
        /// <summary>
        /// Handles the requesting of ownership so that concurrent multi-user 
        /// manipulation doesn't screw up synchronization or state of an object.
        /// </summary>
        /// <param name="obj"></param>
        public void RequestOwnership(GameObject obj)
        {
            OnObjectSelected(obj);
            networkManager.RequestOwnership(obj);
        }

        /// <summary>
        /// Selects an object as the "focus object" for which all interactions 
        /// will center on.
        /// 
        /// NOTE: This has a bad flow and should be heavily reworked so that 
        /// manipulation isn't limited to a single object at a time.
        /// </summary>
        /// 
        /// <param name="obj"></param>
        public void Focus(GameObject obj)
        {
            OnObjectSelected(obj);
        }

        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when 
        /// this class is enabled. Sets the NetworkManager to the active 
        /// NetworkManager in the scene. Sets the platform to be the platform 
        /// type of this client node. Dynamically attaches controller scripts 
        /// (e.g. Mouse & Keyboard) based off of platform type.
        /// </summary>
        public void Awake()
        {
            networkManager = GameObject.Find("NetworkManager").GetComponent<UWBNetworkingPackage.NetworkManager>();
#if UNITY_WSA_10_0
#elif UNITY_ANDROID
            gameObject.AddComponent<ASL.Manipulation.Controllers.Android.BehaviorDifferentiator>();
#else
            UWBNetworkingPackage.NodeType platform = UWBNetworkingPackage.Config.NodeType;

            switch (platform)
            {
                case UWBNetworkingPackage.NodeType.PC:
                    gameObject.AddComponent<ASL.Manipulation.Controllers.PC.Mouse>();
                    gameObject.AddComponent<ASL.Manipulation.Controllers.PC.Keyboard>();
                    break;
                case UWBNetworkingPackage.NodeType.Kinect:
                    break;
                case UWBNetworkingPackage.NodeType.Vive:
                    //gameObject.AddComponent<ASL.Manipulation.Controllers.Vive.ControllerUIManager>();
                    break;
                case UWBNetworkingPackage.NodeType.Oculus:
                    break;
                default:
                    Debug.LogWarning("Unsupported platform encountered");
                    break;
            }
#endif
        }

        /// <summary>
        /// Unity method that is called every few frames. Forces a NetworkManager 
        /// reset if the platform is updated after initialization.
        /// </summary>
        public void FixedUpdate()
        {
            // reset manager if platform is too quick to update properly at startup
            if (platform != UWBNetworkingPackage.Config.NodeType)
            {
                Resources.Load("Prefabs/ObjectInteractionManager");
                //GameObject.Destroy(gameObject);
            }
        }

        /// <summary>
        /// Instantiates an object across the network for all connected clients.
        /// 
        /// NOTE: Currently not tested, used, or properly implemented. Please use 
        /// the other Instantiate methods.
        /// </summary>
        /// 
        /// <param name="go">
        /// The GameObject you want to have replicated and instantiated on all
        /// connected clients.
        /// </param>
        /// 
        /// <returns>
        /// The GameObject clone of the one passed in.
        /// </returns>
        public GameObject Instantiate(GameObject go)
        {
            return networkManager.Instantiate(go);
        }

        /// <summary>
        /// Instantiates a prefab across the network for all connected clients.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// Name of the prefab somewhere in the Resources folder you want to 
        /// instantiate. This name is case-sensitive. Will instantiate the 
        /// first prefab found with a matching name based on a depth-first 
        /// traversal of the Resources folder.
        /// </param>
        /// 
        /// <returns>
        /// The instantiated prefab.
        /// </returns>
        public GameObject Instantiate(string prefabName)
        {
            return networkManager.Instantiate(prefabName);
        }

        /// <summary>
        /// Instantiates a prefab across the network at a specified position 
        /// and orientation.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// Name of the prefab somewhere in the Resources folder you want to
        /// instantiate. This name is case-sensitive. Will instantiate the
        /// first prefab found with a matching name based on a depth-first
        /// traversal of the Resources folder.
        /// </param>
        /// 
        /// <param name="position">
        /// The desired position of the instantiated object.
        /// </param>
        /// 
        /// <param name="rotation">
        /// The desired orientation of the instantiated object.
        /// </param>
        /// 
        /// <param name="scale">
        /// The desired scale of the instantiated object.
        /// </param>
        /// 
        /// <returns>
        /// The instantiated prefab.
        /// </returns>
        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return networkManager.Instantiate(prefabName, position, rotation, scale);
        }

        /// <summary>
        /// Instantiate an object and immediately request ownership. Acts the
        /// same as Instantiate but ensures others cannot grab the object if they
        /// are not allowed control.
        /// </summary>
        /// 
        /// <param name="prefabName">
        /// Name of the prefab somewhere in the Resources folder you want to
        /// instantiate. This name is case-sensitive. Will instantiate the
        /// first prefab found with a matching name based on a depth-first
        /// traversal of the Resources folder.
        /// </param>
        /// 
        /// <returns>
        /// The instantiated prefab.
        /// </returns>
        public GameObject InstantiateOwnedObject(string prefabName)
        {
            return networkManager.InstantiateOwnedObject(prefabName);
        }

        /// <summary>
        /// Destroy an object across the network. Unconnected clients will 
        /// not have their object destroyed upon reconnection through this
        /// method, but may have the object destroyed through synchronization.
        /// 
        /// Ownership of an object is required for an object to be destroyed.
        /// </summary>
        /// 
        /// <param name="go">
        /// The ASL GameObject to be destroyed.
        /// </param>
        /// 
        /// <returns>
        /// True if the object exists and was destroyed on at least this client.
        /// </returns>
        public bool Destroy(GameObject go)
        {
            return networkManager.Destroy(go);
        }
        
        /// <summary>
        /// Associates a behavior with this class for easy reference by 
        /// behavior collector classes.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of behavior (i.e. its class).
        /// </typeparam>
        /// 
        /// <returns>
        /// The generated behavior script that is attached and associated with 
        /// this object.
        /// </returns>
        public T RegisterBehavior<T>()
        {
            if(gameObject.GetComponent<T>() != null)
            {
                Debug.Log("ObjectInteractionManager already has behavior attached. Ignoring request to reattach behavior script to avoid missing proper startup logic calls.");
            }
            else
            {
                gameObject.AddComponent(typeof(T));
            }

            return gameObject.GetComponent<T>();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Updates the stored "focus object" to be this focus object. Calls the
        /// FocusObjectChangedEvent to perform this update.
        /// 
        /// NOTE: Should be left as a protected method to ensure that if any other
        /// backend changes are needed for custom behavior, that they are 
        /// realized through this method.
        /// </summary>
        /// <param name="obj"></param>
        protected void OnObjectSelected(GameObject obj)
        {
            int focuserID = PhotonNetwork.player.ID;
            //Debug.Log("About to trigger On Object Selected event");
            if (obj != null)
            {
                if (obj.GetPhotonView() != null)
                {
                    FocusObjectChangedEvent(new ObjectSelectedEventArgs(obj, obj.GetPhotonView().ownerId, focuserID));
                }
                else
                {
                    FocusObjectChangedEvent(new ObjectSelectedEventArgs(obj, 0, focuserID));
                }
            }
            else
            {
                FocusObjectChangedEvent(new ObjectSelectedEventArgs(obj, 0, focuserID));
            }
            //Debug.Log("Event triggered");
        }
        #endregion
        #endregion
    }
}