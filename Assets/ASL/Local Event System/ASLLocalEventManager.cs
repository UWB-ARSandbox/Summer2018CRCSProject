using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Contains all classes relating to triggering and handling message passing
/// between local components in ASL.
/// </summary>
namespace ASL.LocalEventSystem
{

    /// <summary>
    /// This script implements a singleton pattern and maintains a public event
    /// that other scripts can subscribe to for using message passing, delegates,
    /// and events to synchronize the scene.
    /// </summary>
    public class ASLLocalEventManager : MonoBehaviour
    {
        private static ASLLocalEventManager _instance = null;

        /// <summary>
        /// Statically avaliable singleton instance
        /// </summary>
        public static ASLLocalEventManager Instance { get { return _instance; } }
        /// <summary>
        /// Event handler using LocalEventArgs, other scripts can use delegation to have an
        /// internal function invoked upon the local event being triggered elsewhere.
        /// </summary>
        public static event EventHandler<LocalEventArgs> LocalEventTriggered;

        /// <summary>
        /// Event Argument that contains a LocalEvent code (Enum)
        /// </summary>
        public class LocalEventArgs : EventArgs
        {
            public LocalEvents MyEvent { get; set; }
        }

        /// <summary>
        /// Enum of supported events.
        /// </summary>
        public enum LocalEvents
        {
            VRPlayerActivated,
            VRAvatarCreationSucceeded,
            VRAvatarCreationFailed,
            SimCameraRigCreationFailed,
            SimCameraRigCreationSucceeded,
            SimCameraRigActivated,
            SimulatorActivated,
            SteamVRActivated,
            PCPlayerActivated,
            PCPlayerCreationSucceeded,
            PCPlayerCreationFailed,
            PrimaryCameraSet,
            PortalManagerPlayerSet,
            PortalCreationSucceeded,
            PortalCreationFailed,
            TriggerPortalCreation,
        }

        void Awake()
        {
            if (Instance == null)
            {
                _instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// This function sends a message to ever script subscribed to the EventHandler delegate.
        /// </summary>
        /// <param name="sender">Object associated with triggering this event.</param>
        /// <param name="eventToTrigger">A LocalEvents enum value representing the event to be triggered.</param>
        /// <returns>boolean reflecting whether there are any observers for the event.</returns>
        public bool Trigger(object sender, LocalEvents eventToTrigger)
        {
            if (LocalEventTriggered != null)
            {
                LocalEventTriggered(sender, new LocalEventArgs { MyEvent = eventToTrigger });
                return true;
            }
            else
            {
                //Debug.Log("No subscribers to Local Events");
                return false;
            }
        }
    }

}