using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.PortalSystem;

public class RoomPortalScript : MonoBehaviour {

    public PortalManager mPortalManager;
    public HubPortalScript hub;
    public bool registered = false;
    public bool linked = false;
	
	// Update is called once per frame
	void Update ()
    {
		if(!registered)
        {
            registered = hub.addPortal(name);
        }
        if (!linked)
        {
            Debug.Log("Trying to link portal to HUB");
            linked = linkToHub();
        }
	}

    bool linkToHub()
    {
        Debug.Log("Trying to link " + name + " to the HUB");
        int srcID = hub.getPortalID(name);
        int hubID = hub.portalID;
        Debug.Log(name + " ID: " + srcID + "\t" + "HUB ID: " + hubID);
        return mPortalManager.RequestLinkPortal(srcID, hubID);
    }
}
