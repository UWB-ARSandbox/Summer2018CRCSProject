using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.UI.Menus.Networking
{
    /// <summary>
    /// A DontDestroyOnLoad object that is a simple container for global 
    /// variables between scene transitions.
    /// </summary>
    public class SceneVariableSetter : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// Boolean determining if this client node is the current master 
        /// client or not.
        /// </summary>
        public bool isMasterClient = false;

        /// <summary>
        /// The platform type that this client node is supposed to be (i.e. PC,
        /// Android, etc.).
        /// </summary>
        public UWBNetworkingPackage.NodeType platform;
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or when
        /// this class is enabled. Just sets the containing GameObject to not
        /// be destroyed on a scene shift.
        /// </summary>
        public void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }
        #endregion
    }
}