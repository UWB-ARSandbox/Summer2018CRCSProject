using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.PortalSystem;

public class HubPortalScript : MonoBehaviour {

    public PortalManager mPortalManager;
    public int portalID = -1;
    public int totalPortals = 0;
    public Dictionary<string, int> portalKey;
    public string[] portalNames;
    public int currentCycle = 0;
    public int swapCycle = 240;
    public int targetPortalIndex = 0;

    void Start()
    {
        portalKey = new Dictionary<string, int>();
        portalNames = new string[4];
        portalNames[0] = "HUB";
        portalNames[1] = "Room1";
        portalNames[2] = "Room2";
        portalNames[3] = "Room3";

        for (int i = 0; i < 4; i++)
        {
            portalKey.Add(portalNames[i], -1);
        }
    }

    public bool addPortal(string target)
    {
        GameObject o = GameObject.Find(target);
        int id = -1;
        if (o.GetComponentInChildren<PhotonView>() != null)
        {
            id = o.GetComponentInChildren<PhotonView>().viewID;
        }
        if (portalKey.ContainsValue(id))
        {
            return false;
        }
        else
        {
            if (id > 0)
            {
                portalKey[target] = id;
                totalPortals++;
                if (target == "HUB")
                {
                    portalID = id;
                }
                Debug.Log("Successfully added ID associated with " + target);
                return true;
            }
            return false;
        }
    }

    public int getPortalID(string name)
    {
        if (portalKey.ContainsKey(name))
        {
            return portalKey[name];
        }
        return -1;
    }

    void Update ()
    {
        currentCycle++;
        if (currentCycle == swapCycle)
        {
            targetPortalIndex++;
            targetPortalIndex = targetPortalIndex % 4;
            currentCycle = currentCycle % swapCycle;

            int srcID = portalID;
            int dstID = portalKey[portalNames[targetPortalIndex]];

            mPortalManager.RequestLinkPortal(srcID, dstID);
        }

        if (portalID < 0)
        {
            GameObject o = transform.Find("Portal").gameObject;
            if (o.GetComponentInChildren<PhotonView>() != null && o != null)
            {
                portalID = o.GetComponentInChildren<PhotonView>().viewID;
            }
        }

    }
}
