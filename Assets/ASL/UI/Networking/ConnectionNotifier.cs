using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.UI.Networking
{
    /// <summary>
    /// A class that encapsulates the UI logic for displaying this client node
    /// 's current connection status to the ASL / PUN network.
    /// </summary>
    public class ConnectionNotifier : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// The text to display on-screen as a visual representation of the 
        /// connection.
        /// </summary>
        private UnityEngine.UI.Text displayText;
        #endregion

        #region Methods
        /// <summary>
        /// Triggers after all "Start" methods are successfully called or 
        /// when this class is enabled. Sets the displayed text to be the 
        /// same as the initialized value of the UI text.
        /// </summary>
        public void Awake()
        {
            //displayText = GameObject.Find("NetworkManager").GetComponentInChildren<UnityEngine.UI.Text>();
            displayText = gameObject.GetComponent<UnityEngine.UI.Text>();
        }

        /// <summary>
        /// Unity method that is called every frame. Sets the status text 
        /// to display the connection state and sets the text color 
        /// appropriately.
        /// </summary>
        public void Update()
        {
            ConnectionState status = PhotonNetwork.connectionState;
            switch (status)
            {
                case ConnectionState.Connected:
                    displayText.color = Color.green;
                    break;
                case ConnectionState.Connecting:
                    displayText.color = Color.yellow;
                    break;
                case ConnectionState.Disconnecting:
                case ConnectionState.Disconnected:
                    displayText.color = Color.red;
                    break;
                case ConnectionState.InitializingApplication:
                    displayText.color = Color.gray;
                    break;
            }

            displayText.text = PhotonNetwork.connectionState.ToString();
            displayText.text += "\n" + PhotonNetwork.room;
            displayText.text += "\n" + PhotonNetwork.lobby;
        }
        #endregion
    }
}