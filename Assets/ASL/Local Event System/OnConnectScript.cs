using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that will activate a list of GameObjects upon joining a PUN room.
/// </summary>
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
