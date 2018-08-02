using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class ASLLocalEvents : MonoBehaviour
{   
    // Event Argument that contains a LocalEvent code (Enum)
    public class LocalEventArgs : EventArgs
    {
        public LocalEvents MyEvent { get; set; }
    }

    // Enum of supported events.
    public enum LocalEvents
    {
        PlayerInitialized
    }

    private static ASLLocalEvents _instance = null;

    public static ASLLocalEvents Instance { get { return _instance; } }
    public static event EventHandler<LocalEventArgs> LocalEventTriggered;


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