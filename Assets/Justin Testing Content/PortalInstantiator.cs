using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ASL.PortalSystem;
using ASL.Manipulation.Objects;

public class PortalInstantiator : MonoBehaviour
{


    public bool includeSelector = false;
    public Quaternion selectorDirection;
    public Vector3 selectorPosition;

    private PortalManager mPortalManager;
    private Portal mPortalInstance;
    private PortalSelector mPortalSelectorInstance;
    private ObjectInteractionManager mObjectInteractionManager;

    private bool instantiated = false;


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

        if (PhotonNetwork.inRoom)
        {
            if (!instantiated)
            {

                if (includeSelector)
                {
                    instantiated = instantiateWithSelector();
                }
                else
                {
                    instantiated = instantiateWithoutSelector();
                }
                mPortalInstance = mPortalManager.MakePortal(transform.position, transform.forward, transform.up, Portal.ViewType.VIRTUAL, "Portal");
                

                if (mPortalInstance != null)
                {
                    instantiated = true;
                }
            }
            else
            {
                if (mPortalManager.RequestRegisterPortal(mPortalInstance))
                {
                    GameObject selector = mObjectInteractionManager.InstantiateOwnedObject("PortalSelector");
                    Debug.Log("Creating portal selector");
                    if (selector != null)
                    {
                        Debug.Log("Selector Created");
                        selector.transform.parent = mPortalInstance.transform;
                        selector.transform.localPosition = selectorPosition;
                        selector.transform.rotation = selectorDirection;
                        selector.GetComponent<PortalSelector>().Initialize(mPortalManager.player.GetComponentInChildren<Camera>(), mPortalInstance);
                        GameObject.Destroy(gameObject);

                    }
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

        mPortalSelectorInstance = mObjectInteractionManager.Instantiate("Portal Selector").GetComponent<PortalSelector>();
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
}
