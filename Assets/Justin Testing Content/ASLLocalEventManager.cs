using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class ASLLocalEventManager : MonoBehaviour
{   
    private static ASLLocalEventManager _instance = null;

    public static ASLLocalEventManager Instance { get { return _instance; } }
    public static event EventHandler<LocalEventArgs> LocalEventTriggered;

    // Event Argument that contains a LocalEvent code (Enum)
    public class LocalEventArgs : EventArgs
    {
        public LocalEvents MyEvent { get; set; }
    }

    // Enum of supported events.
    public enum LocalEvents
    {
        VRPlayerActivated,
        PCPlayerActivated,
        PCPlayerCreationSucceeded,
        PCPlayerCreationFailed,
        PrimaryCameraSet,
        PortalManagerPlayerSet
    }

    void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }


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

