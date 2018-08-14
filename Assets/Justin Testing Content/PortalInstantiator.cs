﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ASL.PortalSystem;
using ASL.Manipulation.Objects;

public class PortalInstantiator : LocalEventHandler
{

    public string portalName = "Portal";
    public bool setDestination = false;
    public string destinationName;
    public bool includeSelector = false;
    public Quaternion selectorDirection;
    public Vector3 selectorPosition;

    private PortalManager mPortalManager;
    private Portal mPortalInstance;
    private PortalSelector mPortalSelectorInstance;
    private Camera mPlayerCamera;
    private ObjectInteractionManager mObjectInteractionManager;

    private bool instantiated = false;
    public bool playerAvaliable = false;
    private bool registered = false;


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

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckForPlayer();
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
                if (mPortalManager.RequestRegisterPortal(mPortalInstance) && !registered)
                {
                    Debug.Log("Portal was registered");
                    if (!setDestination)
                    {
                        GameObject.Destroy(gameObject);
                    }
                    else
                    {
                        registered = true;
                        AttemptToLink();
                    }
                }
                else
                {
                    Debug.Log("Already registered, attempting to link");
                    AttemptToLink();
                }
            }
        }
    }

    private void AttemptToLink()
    {
        if (mPortalManager.RequestLinkPortal(portalName, destinationName))
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            Debug.Log("Unable to link portals");
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
            SelectorTranslation();
            mPortalSelectorInstance.Initialize(mPlayerCamera, mPortalInstance);
        }

        return true;

    }

    private void SelectorTranslation()
    {
        mPortalSelectorInstance.transform.parent = mPortalInstance.transform;
        mPortalSelectorInstance.transform.localPosition = selectorPosition;
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
            mPortalInstance.portalName = portalName;
            mPortalInstance.gameObject.name = portalName;
        }
    }

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        Debug.Log("Portal Instantiator has received an event: " + args.MyEvent.ToString());
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.PortalManagerPlayerSet:
                {
                    PlayerSetEventHandler();
                    break;
                }
            case ASLLocalEventManager.LocalEvents.TriggerPortalCreation:
                {
                    PortalCreationHandler();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void PlayerSetEventHandler()
    {
        Debug.Log("trying to find the Camera for the player");
        CheckForPlayer();
        if (mPlayerCamera)
        {
            Debug.Log("found the camera");
            playerAvaliable = true;
        }
    }

    private void PortalCreationHandler()
    {

    }

    private void CheckForPlayer()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Local Primary Camera");
        if (temp)
        {
            mPlayerCamera = temp.GetComponent<Camera>();
        }
        else
        {
            return;
        }

        if (mPlayerCamera != null)
        {
            playerAvaliable = true;
        }
    }
}
