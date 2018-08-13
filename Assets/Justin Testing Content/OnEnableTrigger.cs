using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableTrigger : MonoBehaviour
{

    public ASLLocalEventManager.LocalEvents[] EventsToTrigger;

    void OnEnable()
    {
        Debug.Log(gameObject.name + " is triggering events");
        foreach (ASLLocalEventManager.LocalEvents ev in EventsToTrigger)
        {
            Debug.Log("Triggering on enable event: " + ev.ToString());
            ASLLocalEventManager.Instance.Trigger(gameObject, ev);
        }
    }

}
