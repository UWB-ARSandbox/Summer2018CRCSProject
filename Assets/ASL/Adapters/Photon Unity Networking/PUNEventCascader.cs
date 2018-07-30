using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Adapters.PUN
{
    /// <summary>
    /// Handles PUN events that must be triggered by all clients upon startup.
    /// </summary>
    public static class PUNEventCascader
    {
        /// <summary>
        /// Dynamically sets the Photon network settings for handling events, 
        /// then sends an event trigger to all connected clients to properly 
        /// synchronize their scenes with this client by adding appropriate
        /// objects.
        /// 
        /// This logic should be called whenever a client joins an existing
        /// Photon room.
        /// </summary>
        public static void Join()
        {
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(UWBNetworkingPackage.ASLEventCode.EV_JOIN, null, true, options);
        }
    }
}