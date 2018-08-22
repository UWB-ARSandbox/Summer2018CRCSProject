using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.LocalEventSystem;

namespace ASL
{
    /// <summary>
    /// Enables, disables, or triggers various events and components
    /// upon networked instantation. Dependent on having ownership
    /// of the object.
    /// </summary>
    public class OnConnectToggle : Photon.PunBehaviour
    {

        public MonoBehaviour[] ScriptsToActivate;
        public MonoBehaviour[] ScriptsToDeactivate;
        public GameObject[] GameObjectsToActivate;
        public GameObject[] GameObjectsToDeactivate;
        public ASLLocalEventManager.LocalEvents[] EventsToTrigger;
        public Camera[] CamerasToActivate;
        public Camera[] CamerasToDeactivate;

        /// <summary>
        /// Photon event handler that will trigger everything from the public fields.
        /// </summary>
        /// <param name="info"></param>
        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            //Debug.Log(transform.name + " has been instantiated");
            if (transform.GetComponent<PhotonView>().isMine)
            {
                //Debug.Log("I own " + transform.name + " handling the activations and deactivations");

                HandleOwnedGameObjects();

                HandleOwnedScripts();

                HandleOwnedCameras();

                TriggerEvents();
            }
        }

        private void HandleOwnedScripts()
        {
            foreach (MonoBehaviour script in ScriptsToDeactivate)
            {
                Debug.Log("Disabling Script: " + script.name);
                script.enabled = false;
            }

            foreach (MonoBehaviour script in ScriptsToActivate)
            {
                Debug.Log("Enabling Script: " + script.name);
                script.enabled = true;
            }
        }

        private void HandleOwnedCameras()
        {
            foreach (Camera c in CamerasToActivate)
            {
                c.enabled = true;
            }


            foreach (Camera c in CamerasToDeactivate)
            {
                c.enabled = false;
            }
        }

        private void HandleOwnedGameObjects()
        {
            foreach (GameObject go in GameObjectsToActivate)
            {
                Debug.Log("Activating Game Object: " + go.name);
                go.SetActive(true);
            }

            foreach (GameObject go in GameObjectsToDeactivate)
            {
                Debug.Log("Deactivating Game Object: " + go.name);
                go.SetActive(false);
            }
        }

        private void TriggerEvents()
        {
            foreach (ASLLocalEventManager.LocalEvents ev in EventsToTrigger)
            {
                Debug.Log("Triggering event " + ev.ToString());
                ASLLocalEventManager.Instance.Trigger(gameObject, ev);
            }
        }
    }
}