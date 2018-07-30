using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Receiving client launcher functionality customized for use with the 
    /// Microsoft Hololens.
    /// </summary>
    public class ReceivingClientLauncher_Hololens : ReceivingClientLauncher
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        // Insert overrides here
        /// <summary>
        /// Unity method that is called prior to runtime. Triggers before 
        /// "Awake" methods. Finds and connects to master client content server.
        /// </summary>
        public override void Start()
        {
            base.Start();
            ServerFinder_Hololens.FindServerAsync();
            SocketServer_Hololens.StartAsync();
        }

        /// <summary>
        /// NOTE: Currently commented out due to issues with server connection,
        /// but was used primarily to synchronize asset bundles containing
        /// Hololens-scanned rooms. Retain and revisit if the Hololens becomes 
        /// supported again.
        /// </summary>
        public override void OnJoinedRoom()
        {
            //Debug.debugging = true;
            //string roomBundleDirectory = Config.AssetBundle.Current.CompileAbsoluteBundleDirectory();
            //int roomBundlePort = Config.Ports.RoomBundle;

            //Debug.Log("directory = " + roomBundleDirectory + "; port = " + roomBundlePort.ToString());

            //SocketClient_Hololens.RequestFiles(roomBundlePort, roomBundleDirectory);
            //Debug.debugging = false;
        }
#endif
    }
}