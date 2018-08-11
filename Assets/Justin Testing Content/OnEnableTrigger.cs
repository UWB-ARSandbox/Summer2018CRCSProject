using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableTrigger : MonoBehaviour
{

    public ASLLocalEventManager.LocalEvents[] EventsToTrigger;

    void OnEnable()
    {
        foreach (ASLLocalEventManager.LocalEvents ev in EventsToTrigger)
        {
            ASLLocalEventManager.Instance.Trigger(gameObject, ev);
        }
    }

}
