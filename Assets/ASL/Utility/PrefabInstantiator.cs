using UnityEngine;
using ASL.Manipulation.Objects;

namespace ASL.Manipulation.Objects
{
    /// <summary>
    /// This class is used to create sychronized & owned ASL objects
    /// before runtime.
    /// </summary>
    public class PrefabInstantiator : MonoBehaviour
    {
        /// <summary>
        /// The prefab to be created
        /// </summary>
        public GameObject prefabReference;
        /// <summary>
        /// A GameObject to anchor as a parent in the heirarchy
        /// </summary>
        public GameObject parent = null;
        /// <summary>
        /// Toggle for instantiating the prefab locally or over the network
        /// </summary>
        public bool LocalOnly = false;


        private GameObject prefabInstance;
        private ObjectInteractionManager mObjectInteractionManager;

        private bool instantiated = false;



        void Awake()
        {
            mObjectInteractionManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
            transform.GetComponent<MeshRenderer>().enabled = false;

        }

        // Update polls to see if the prefab has been created yet and waits to be connected to the room.
        // Should be refactored to use OnPhotonInstantiate or OnJoinedRoom in the case of networked instantation.
        void Update()
        {
            if (prefabReference != null)
            {
                if (!instantiated && PhotonNetwork.inRoom)
                {
                    if (!LocalOnly)
                    {
                        prefabInstance = instantiatePrefab(prefabReference);
                    }
                    else
                    {
                        prefabInstance = (GameObject)Instantiate(prefabReference);
                    }

                    if (prefabInstance != null)
                    {
                        instantiated = true;
                        transform.GetComponent<PrefabInstantiator>().enabled = false;
                        translateInstance();
                        if (parent != null)
                        {
                            prefabInstance.transform.parent = parent.transform;
                            Debug.Log("Changed parent");
                        }
                        GameObject.Destroy(gameObject);
                    }
                }
            }

        }

        /// <summary>
        /// This function can be used by other classes to instantiate a prefab over ASL.
        /// </summary>
        /// <param name="prefab">Reference to a prefab to be created</param>
        /// <returns>The instance of the prefab created</returns>
        public GameObject instantiatePrefab(GameObject prefab)
        {
            return mObjectInteractionManager.InstantiateOwnedObject(prefab.transform.name);
        }

        private void translateInstance()
        {
            prefabInstance.transform.position = transform.position;
            prefabInstance.transform.rotation = transform.rotation;
            prefabInstance.transform.localScale = transform.localScale;
        }
    }
}
