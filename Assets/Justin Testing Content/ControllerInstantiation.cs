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

    public override void OnJoinedRoom()
    {
        InstantiatePCPlayer();
    }

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.PCPlayerCreationFailed:
                {
                    InstantiatePCPlayer();
                    break;
                }
            case ASLLocalEventManager.LocalEvents.PCPlayerCreationSucceeded:
                {
                    SucessfulCreationEventHandler();
                    break;
                }
            case ASLLocalEventManager.LocalEvents.PortalManagerPlayerSet:
                {
                    GameObject.Destroy(gameObject);
                    break;
                } 
            default:
                {
                    Debug.Log(this.name + ": Event not handled");
                    break;
                }
        }
    }

    private void SucessfulCreationEventHandler()
    {
        if (myPlayer.GetComponent<PhotonView>().isMine)
        {
            myPlayer.gameObject.name = "MyAvatar";
            myFPSCamera = myPlayer.GetComponentInChildren<Camera>(true).gameObject;
            myPlayer.GetComponentInChildren<Camera>(true).enabled = true;
            myPlayer.GetComponentInChildren<SmoothMouseLook>(true).enabled = true;
            myPlayer.GetComponentInChildren<PlayerController>(true).enabled = true;

            myPlayer.transform.Find("Cursor Canvas").gameObject.SetActive(true);
            myPlayer.transform.Find("World Space Canvas").gameObject.SetActive(true);

            myLabel = myPlayer.transform.GetComponentInChildren<BillboardText>(true);
            if (myLabel)
            {
                myLabel.enabled = true;
                myLabel.setCamera(myFPSCamera.GetComponent<Camera>());
            }


            SetPortalManagerPlayer();
        }
        else
        {
            // I don't think this will ever be called
            myPlayer.gameObject.name = "OtherAvatar";
            myPlayer.GetComponentInChildren<Camera>().enabled = false;
            myPlayer.GetComponentInChildren<SmoothMouseLook>().enabled = false;
            myPlayer.GetComponentInChildren<PlayerController>().enabled = false;

            transform.Find("Cursor Canvas").gameObject.SetActive(false);
            transform.Find("World Space Canvas").gameObject.SetActive(false);
        }
    }

    private void InstantiatePCPlayer()
    {
        myPlayer = mObjectInteractionManager.InstantiateOwnedObject("Player Avatar");
        if (myPlayer == null)
        {
            ASLLocalEventManager.Instance.Trigger(this, ASLLocalEventManager.LocalEvents.PCPlayerCreationFailed);
            return;
        }

        SetInitialProperties();

        ASLLocalEventManager.Instance.Trigger(this, ASLLocalEventManager.LocalEvents.PCPlayerCreationSucceeded);

    }

    private void SetInitialProperties()
    {
        myPlayer.GetComponent<MeshRenderer>().sharedMaterial = avatarMaterial;
        myPlayer.transform.position = initialPosition;
        myPlayer.transform.localScale = initialScale;
    }


    private void SetPortalManagerPlayer()
    {
        myFPSCamera.tag = "Local Primary Camera";
        GameObject.Find("PortalManager").GetComponent<PortalManager>().SetPlayer(myFPSCamera);
    }
}
