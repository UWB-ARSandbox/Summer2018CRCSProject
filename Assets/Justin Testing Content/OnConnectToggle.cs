using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnConnectToggle : Photon.PunBehaviour
{

    public MonoBehaviour[] ScriptsToActivate;
    public MonoBehaviour[] ScriptsToDeactivate;
    public GameObject[] GameObjectsToActivate;
    public GameObject[] GameObjectsToDeactivate;
    public Camera[] CamerasToActivate;
    public Camera[] CamerasToDeactivate;

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log(transform.name + " has been instantiated");
        if (transform.GetComponent<PhotonView>().isMine)
        {
            foreach (MonoBehaviour script in ScriptsToDeactivate)
            {
                script.enabled = false;
            }

            foreach (MonoBehaviour script in ScriptsToActivate)
            {
                script.enabled = true;
            }

            foreach (Camera c in CamerasToActivate)
            {
                c.enabled = true;
            }


            foreach (Camera c in CamerasToDeactivate)
            {
                c.enabled = false;
            }

            foreach (GameObject go in GameObjectsToActivate)
            {
                go.SetActive(true);
            }

            foreach (GameObject go in GameObjectsToDeactivate)
            {
                go.SetActive(false);
            }
        }
    }
}
