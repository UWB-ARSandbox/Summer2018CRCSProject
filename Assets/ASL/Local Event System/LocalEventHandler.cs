using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL
{
    namespace LocalEventSystem
    {
        /// <summary>
        /// This class provides auto subscription to the local event manager events and provides functions
        /// handling local events.
        /// </summary>
        public abstract class LocalEventHandler : Photon.PunBehaviour
        {

            /// <summary>
            /// Subscribes to Local event system, when a local event is trigger the OnLocalEvent function will be called.
            /// </summary>
            protected virtual void OnEnable()
            {
                ASLLocalEventManager.LocalEventTriggered += OnLocalEvent;
            }

            /// <summary>
            /// Unsubscribes from the lcal events system to prevent a memory leak.
            /// </summary>
            protected virtual void OnDestroy()
            {
                ASLLocalEventManager.LocalEventTriggered -= OnLocalEvent;
            }

            /// <summary>
            /// Callback function that is triggered when an object is created via photon.
            /// </summary>
            /// <param name="info">Struct containing information about a photon instantation.</param>
            public override void OnPhotonInstantiate(PhotonMessageInfo info) { base.OnPhotonInstantiate(info); }

            /// <summary>
            /// Callback function that is triggered when this user connects and joins a Photon room. Useful for 
            /// making instantiation calls through Photon.
            /// </summary>
            public override void OnJoinedRoom(){ base.OnJoinedRoom(); }

            /// <summary>
            /// Function to be implemented by derived classes. Should operate with a switch statement using the appropriate
            /// LocalEvents values from the Enum in ASLLocalEventManager.
            /// </summary>
            /// <param name="sender">The object linked with triggering this event.</param>
            /// <param name="args">A LocalEvents enum value corresponding to the event triggered.</param>
            protected abstract void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args);
        }
    }
}
