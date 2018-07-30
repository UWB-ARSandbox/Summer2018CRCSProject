using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;
using System;
using ASL.PortalSystem;

public class PortalInstiantationScript : MonoBehaviour
{
    public PortalManager mPortalManager;
    public GameObject loc;
    private bool instantiated = false;
    private HubPortalScript hub;

    void Awake()
    {
        mPortalManager = GameObject.Find("PortalManager").GetComponent<PortalManager>();
        loc = this.transform.Find("Portal Placeholder").gameObject;
        hub = GameObject.Find("HUB").GetComponent<HubPortalScript>();
    }

    void Update()
    {
        if (PhotonNetwork.inRoom && !instantiated)
        {
            instantiated = instantiatePortal();
        }

    }

    private bool instantiatePortal()
    {
        Portal p = mPortalManager.MakePortal(loc.transform.position, loc.transform.forward, loc.transform.up, Portal.ViewType.VIRTUAL, "Portal");
        loc.SetActive(false);
        instantiated = true;

        p.gameObject.transform.SetParent(transform);
        p.gameObject.transform.Translate(new Vector3(0, -1, 0));
        bool results = mPortalManager.RequestRegisterPortal(p);
        if (results)
        {
            hub.addPortal(transform.parent.name);
        }

        return results;
    }
}
