using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;

namespace ASL.Manipulation.Controllers.PC
{
    /// <summary>
    /// Handles controller input for a traditional computer mouse.
    /// </summary>
    public class Mouse : MonoBehaviour
    {
        #region Fields
        private ObjectInteractionManager objManager;
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or 
        /// when this class is enabled. Sets Object Interaction Manager to 
        /// the active Object Interaction Manager in the scene.
        /// </summary>
        public void Awake()
        {
            objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
        }

        /// <summary>
        /// Unity method called every frame. Interprets mouse button presses 
        /// and triggers appropriate behavior.
        /// </summary>
        public void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    GameObject selectedObject = Select();
            //    objManager.RequestOwnership(selectedObject);
            //}
            //if (Input.GetMouseButtonDown(1))
            //{
            //    string prefabName = "Sphere";
            //    Vector3 position = new Vector3(0, 0, 2);
            //    Quaternion rotation = Quaternion.identity;
            //    Vector3 scale = Vector3.one;
            //    //objManager.Instantiate(prefabName, position, rotation, scale);
            //    //GameObject go = objManager.Instantiate(prefabName);
            //    GameObject go = objManager.InstantiateOwnedObject(prefabName);
            //    go.transform.Translate(position);

            //    UWBNetworkingPackage.NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<UWBNetworkingPackage.NetworkManager>();
            //    List<int> IDsToAdd = new List<int>();
            //    IDsToAdd.Add(2);
            //    nm.WhiteListOwnership(go, IDsToAdd);
            //    //nm.RestrictOwnership(go, IDsToAdd);
            //}
        }

        /// <summary>
        /// Selection behavior. Should be moved and implemented inside of a 
        /// behavior class.
        /// </summary>
        /// <returns></returns>
        public GameObject Select()
        {
            Camera cam = GameObject.FindObjectOfType<Camera>();
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseRay = cam.ScreenToWorldPoint(mousePos);
            RaycastHit hit;
            Physics.Raycast(cam.ScreenPointToRay(mousePos), out hit);

            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            else
            {
                GameObject camera = GameObject.Find("Main Camera");
                if(camera != null)
                {
                    return camera;
                }
                else
                {
                    Debug.LogError("Cannot find camera object. Selecting null object.");
                    return null;
                }
            }
        }
        #endregion
    }
}