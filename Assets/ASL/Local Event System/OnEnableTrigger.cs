using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL
{
    namespace LocalEventSystem
    {
        /// <summary>
        /// Utility script using the local event system. Provides ability to trigger any local event
        /// when this game object is enabled.
        /// </summary>
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
    }
}
