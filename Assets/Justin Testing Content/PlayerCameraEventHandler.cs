using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraEventHandler : LocalEventHandler
{
    public Camera playerCamera;

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        Debug.Log("Player Camera Event Handler received event: " + args.MyEvent.ToString());
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.SimulatorCameraRigInstantiated:
                {
                    SetPrimaryCameraTag();
                    GameObject.Find("PortalManager").GetComponent<ASL.PortalSystem.PortalManager>().player = playerCamera.gameObject;
                    ASLLocalEventManager.Instance.Trigger(gameObject, ASLLocalEventManager.LocalEvents.PlayerInitialized);
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
        Debug.Log("Changing camera tag");
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
