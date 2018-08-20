using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.LocalEventSystem;

public class PlayerCameraEventHandler : LocalEventHandler
{
    public Camera playerCamera;

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        Debug.Log("Player Camera Event Handler received event: " + args.MyEvent.ToString());
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.VRPlayerActivated:
                {
                    SetPrimaryCameraTag();
                    GameObject.Find("PortalManager").GetComponent<ASL.PortalSystem.PortalManager>().SetPlayer(playerCamera.gameObject);
                    break;
                }
            default:
                {
                    Debug.Log("Event not handled");
                    break;
                }
        }
    }



    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {

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
