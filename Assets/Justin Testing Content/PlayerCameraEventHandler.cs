using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraEventHandler : LocalEventHandler
{
    public Camera playerCamera;

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.PlayerInstanceActive:
                {
                    break;
                }
            default:
                {
                    Debug.Log("Event not handled");
                    break;
                }
        }
    }

    private void SetPrimaryCameraTag()
    {
        if (playerCamera == null)
        {
            FindCamera();
        }
        playerCamera.gameObject.tag = "Local Primary Camera";
    }

    private void FindCamera()
    {
        playerCamera = transform.GetComponentInChildren<Camera>(true);
    }
}
