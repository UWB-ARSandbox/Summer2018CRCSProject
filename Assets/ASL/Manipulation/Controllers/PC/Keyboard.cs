using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;

namespace ASL.Manipulation.Controllers.PC
{
    /// <summary>
    /// Handles controller input for a traditional computer keyboard.
    /// </summary>
    public class Keyboard : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// Reference to the MoveBehavior associated with this controller.
        /// </summary>
        private MoveBehavior _moveBehavior;

        /// <summary>
        /// Reference to the ObjectInteractionManager associated with this controller.
        /// </summary>
        private ObjectInteractionManager objManager;
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when 
        /// this class is enabled. Sets Object Interaction Manager to active 
        /// Object Interaction Manager in scene. Initializes and assigns 
        /// MoveBehavior script.
        /// </summary>
        private void Awake()
        {
            objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
            _moveBehavior = objManager.RegisterBehavior<MoveBehavior>();
        }

        /// <summary>
        /// Unity method called every frame. Grabs actively pressed keys and 
        /// calls appropriate behavior.
        /// </summary>
        void Update()
        {
            //if (Input.GetKey(KeyCode.DownArrow)
            //    || Input.GetKey(KeyCode.S))
            //{
            //    MoveBehavior.Down();
            //}
            //if(Input.GetKey(KeyCode.UpArrow)
            //    || Input.GetKey(KeyCode.W))
            //{
            //    MoveBehavior.Up();
            //}
            //if(Input.GetKey(KeyCode.LeftArrow)
            //    || Input.GetKey(KeyCode.A))
            //{
            //    MoveBehavior.Left();
            //}
            //if(Input.GetKey(KeyCode.RightArrow)
            //    || Input.GetKey(KeyCode.D))
            //{
            //    MoveBehavior.Right();
            //}
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    MoveBehavior.RotateClockwise();
            //}
            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    MoveBehavior.RotateCounterClockwise();
            //}
            
            //if (Input.GetKey(KeyCode.R))
            //{
            //    //gameObject.GetComponent<CreateObject>().CreatePUNObject("Rooms/Room2/Room2");
            //    string prefabName = "Rooms/Room2/Room2";
            //    //objManager.Instantiate(prefabName);
            //    objManager.InstantiateOwnedObject(prefabName);
            //}

            //if (Input.GetKey(KeyCode.P))
            //{
            //    GameObject go = GameObject.Find("Sphere");
            //    UWBNetworkingPackage.NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<UWBNetworkingPackage.NetworkManager>();
            //    nm.UnRestrictOwnership(go);
            //}

            //if (Input.GetKey(KeyCode.O))
            //{
            //    List<int> IDsToAdd = new List<int>();
            //    IDsToAdd.Add(2);
            //    UWBNetworkingPackage.NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<UWBNetworkingPackage.NetworkManager>();
            //    GameObject go = GameObject.Find("Sphere");
            //    nm.WhiteListOwnership(go, IDsToAdd);
            //}
        }

        #region Properties
        /// <summary>
        /// MoveBehavior associated with this object.
        /// </summary>
        public MoveBehavior MoveBehavior
        {
            get
            {
                return _moveBehavior;
            }
            set
            {
                if(value != null)
                {
                    _moveBehavior = value;
                }
            }
        }
        #endregion
        #endregion
    }
}