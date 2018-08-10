using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVRReferenceSetter : LocalEventHandler
{

    public string followObjectName;

    private VRTK.VRTK_ObjectFollow myFollowScript;

    void Start ()
    {
        myFollowScript = transform.GetComponent<VRTK.VRTK_ObjectFollow>();
    }

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.ViveCameraRigInstantiated:
                {
                    ViveCameraRigEventHandler();
                    break;
                }
            default:
                {
                    Debug.Log("Event not handled");
                    break;
                }
        }
    }

    private void ViveCameraRigEventHandler()
    {
        PhotonView pv = transform.GetComponent<PhotonView>();
        while(!FindFollowScript())
        {
            Debug.Log("Follow script is null: " + transform.gameObject.name);
        }
        if (pv == null)
        {
            myFollowScript.enabled = true;
            if (!SetFollowObject())
            {
                Debug.Log("Unable to find follow object");
            }
            return;
        }
        if (!pv.isMine)
        {
            myFollowScript.enabled = false;
        }
        else
        {
            myFollowScript.enabled = true;
            if (!SetFollowObject())
            {
                Debug.Log("Unable to find follow object");
            }
        }
    }

    private bool SetFollowObject()
    {
        if (myFollowScript.gameObjectToFollow == null)
        {
            myFollowScript.gameObjectToFollow = GameObject.Find(followObjectName);

            return myFollowScript.gameObjectToFollow == null;
        }
        return true;
    }

    private bool FindFollowScript()
    {
        myFollowScript = transform.GetComponent<VRTK.VRTK_ObjectFollow>();
        return myFollowScript != null;
    }
}
