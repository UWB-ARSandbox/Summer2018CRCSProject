using UnityEngine;
using ASL.Manipulation.Objects;

public class PrefabInstantiator : MonoBehaviour {

    public GameObject prefabReference;
    public GameObject parent = null;


    private GameObject prefabInstance;
    private ObjectInteractionManager mObjectInteractionManager;

    private bool instantiated = false;



	void Awake ()
    {
        mObjectInteractionManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
        transform.GetComponent<MeshRenderer>().enabled = false;
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (prefabReference != null)
        {
            if (!instantiated && PhotonNetwork.inRoom)
            {
                prefabInstance = instantiatePrefab(prefabReference);
                
                if (prefabInstance != null)
                {
                    instantiated = true;
                    transform.GetComponent<PrefabInstantiator>().enabled = false;
                    translateInstance();
                    if(parent != null)
                    {
                        prefabInstance.transform.parent = parent.transform;
                        Debug.Log("Changed parent");
                    }
                    GameObject.Destroy(gameObject);
                }
            }
        }
		
	}

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
