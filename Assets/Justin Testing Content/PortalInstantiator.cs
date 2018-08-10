using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ASL.PortalSystem;
using ASL.Manipulation.Objects;

public class PortalInstantiator : LocalEventHandler
{


    public bool includeSelector = false;
    public Quaternion selectorDirection;
    public Vector3 selectorPosition;

    private PortalManager mPortalManager;
    private Portal mPortalInstance;
    private PortalSelector mPortalSelectorInstance;
    private ObjectInteractionManager mObjectInteractionManager;

    private bool instantiated = false;
    private bool playerAvaliable = false;


    void Awake()
    {
        mPortalManager = GameObject.Find("PortalManager").GetComponent<PortalManager>();
        mObjectInteractionManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();

        // Destroy the postion placeholder for the 
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (playerAvaliable)
        {
            if (!instantiated)
            {
                Debug.Log("Instantiating a portal");

                if (includeSelector)
                {
                    Debug.Log("Creating with a selector");
                    instantiated = instantiateWithSelector();
                }
                else
                {
                    Debug.Log("Creating without a selector");
                    instantiated = instantiateWithoutSelector();
                }
            }
            else
            {
            Debug.Log("Portal was created, attempting to register");
                if (mPortalManager.RequestRegisterPortal(mPortalInstance))
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }


    }

    private bool instantiateWithSelector()
    {
        instantiatePortal();

        if (mPortalInstance == null)
        {
            return false;
        }
        GameObject temp = mObjectInteractionManager.Instantiate("PortalSelector");
        mPortalSelectorInstance = temp.GetComponent<PortalSelector>();
        if (mPortalSelectorInstance == null)
        {
            return false;
        }
        else
        {
            mPortalSelectorInstance.transform.parent = mPortalInstance.transform;
            mPortalSelectorInstance.Initialize(mPortalManager.player.GetComponentInChildren<Camera>(), mPortalInstance);
        }

        return true;

    }

    private bool instantiateWithoutSelector()
    {
        instantiatePortal();

        if (mPortalInstance == null)
        {
            return false;
        }

        return true;
    }

    private void instantiatePortal()
    {
        if (mPortalInstance == null)
        {
            mPortalInstance = mPortalManager.MakePortal(transform.position, transform.forward, transform.up, Portal.ViewType.VIRTUAL, "Portal");
        }
    }

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.PlayerInitialized:
                {
                    playerAvaliable = true;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
