using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL
{

    /// <summary>
    /// Simple script that will activate a list of GameObjects upon joining a PUN room.
    /// </summary>
    public class OnConnectScript : Photon.PunBehaviour
    {
        /// <summary>
        /// GameObjects to activate after joining a room.
        /// </summary>
        public GameObject[] networkedObjects;

        public override void OnJoinedRoom()
        {
            foreach (GameObject go in networkedObjects)
            {
                go.SetActive(true);
            }
        }
    }
}