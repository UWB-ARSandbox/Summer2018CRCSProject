using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnConnectScript : Photon.PunBehaviour
{

    public GameObject[] networkedObjects;

    public override void OnJoinedRoom()
    {
        foreach (GameObject go in networkedObjects)
        {
            go.SetActive(true);
        }
    }

}
