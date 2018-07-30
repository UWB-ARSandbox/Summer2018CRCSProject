using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UWBNetworkingPackage
{
    /// <summary>
    /// Class used to encapsulate all information regarding a Tango scanned room.
    /// </summary>
    public class TangoRoom : MonoBehaviour
    {
        /// <summary>
        /// When a TangoRoom is delete, remove it from the list in TangoDatabase.cs
        /// </summary>
        private void OnDestroy()
        {
            TangoDatabase.TangoRoom T = TangoDatabase.GetRoomByName(this.gameObject.name);

            TangoDatabase.DeleteRoom(T);
        }
    }
}