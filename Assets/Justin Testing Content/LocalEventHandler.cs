using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalEventHandler : MonoBehaviour {

    protected virtual void OnEnable()
    {
        ASLLocalEventManager.LocalEventTriggered += OnLocalEvent;
    }

    protected virtual void OnDestroy()
    {
        ASLLocalEventManager.LocalEventTriggered += OnLocalEvent;
    }

    protected virtual void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch (args.MyEvent)
        {
            default:
                {
                    Debug.Log("Event not handled");
                    break;
                }
        }
    }
}
