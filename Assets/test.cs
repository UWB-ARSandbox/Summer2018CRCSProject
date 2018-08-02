using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {


	void OnEnable()
    {

        ASLLocalEvents.LocalEventTriggered += ReactToEvent;
    }

    void OnDisable()
    {
        ASLLocalEvents.LocalEventTriggered -= ReactToEvent;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Trying to trigger Event Handler");
            ASLLocalEvents.Instance.Trigger(this, ASLLocalEvents.LocalEvents.PlayerInitialized);
        }
    }

    public void ReactToEvent(object sender, ASLLocalEvents.LocalEventArgs args)
    {
        switch(args.MyEvent)
        {
            case ASLLocalEvents.LocalEvents.PlayerInitialized:
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
