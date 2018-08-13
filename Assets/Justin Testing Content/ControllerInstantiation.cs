using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;
using ASL.PortalSystem;

public class ControllerInstantiation : LocalEventHandler
{

    public Material avatarMaterial;
    public GameObject prefabReference;
    public Vector3 initialPosition;
    public Vector3 initialScale;

    private ObjectInteractionManager mObjectInteractionManager;
    private GameObject myController;
    private bool instantiated = false;
    private GameObject myPlayer;
    private GameObject myFPSCamera;
    private BillboardText myLabel;

    void Awake()
    {
        mObjectInteractionManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
        Debug.Log("Controller Instantiation script initialized");
    }

    private void OnDisable()
    {
        ASLLocalEventManager.Instance.Trigger(myFPSCamera, ASLLocalEventManager.LocalEvents.PlayerInstanceActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.inRoom && !instantiated)
        {
            Debug.Log("Trying to create components for first person controller");
            instantiated = true;

            myPlayer = mObjectInteractionManager.InstantiateOwnedObject("Player Avatar");
            if (myPlayer != null)
            {
                Debug.Log("The player avatar component was created");

            }
            else
            {
                Debug.Log("The player capsule component of the avatar was not created properly");
                instantiated = false;
            }

            Debug.Log("Material set");
            myPlayer.GetComponent<MeshRenderer>().sharedMaterial = avatarMaterial;

            //Debug.Log("Creating the mouse control/local components");
            //myFPSCamera = Instantiate(prefabReference, myPlayer.transform);
            //createLocalComponents();
            //myFPSCamera.transform.parent = myPlayer.transform;
            myPlayer.transform.parent = transform;
            transform.GetComponentInChildren<Camera>().enabled = true;
            myLabel = transform.GetComponentInChildren<BillboardText>();
            myLabel.setCamera(transform.GetComponentInChildren<Camera>());

            myPlayer.transform.position = initialPosition;
            myPlayer.transform.localScale = initialScale;

            myFPSCamera = transform.GetComponentInChildren<Camera>().gameObject;

            setPortalManagerPlayer(myFPSCamera);


            if (!instantiated)
            {
                cleanUp();
            }
            else
            {
                Debug.Log("successfully created, disabling instantiation script");
                gameObject.GetComponent<ControllerInstantiation>().enabled = false;
            }

        }
    }

    private void createLocalComponents()
    {
        myPlayer.AddComponent<PlayerController>();
        myPlayer.AddComponent<SmoothMouseLook>();
    }

    private void cleanUp()
    {
        mObjectInteractionManager.Destroy(myPlayer);
    }


    private void setPortalManagerPlayer(GameObject go)
    {
        go.tag = "Local Primary Camera";
        GameObject.Find("PortalManager").GetComponent<PortalManager>().player = go;
    }
}
