using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {


	void OnEnable()
    {

        ASLLocalEventManager.LocalEventTriggered += ReactToEvent;
    }

    void OnDisable()
    {
        ASLLocalEventManager.LocalEventTriggered -= ReactToEvent;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Trying to trigger Event Handler");
            ASLLocalEventManager.Instance.Trigger(this, ASLLocalEventManager.LocalEvents.PlayerInitialized);
        }
    }

    public void ReactToEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch(args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.PlayerInitialized:
            {
                Debug.Log("reacting to player initilization event");
                break;
            }
            default:
            {
                Debug.Log("Event not handled");
                break;
            }

        }
    }
}
