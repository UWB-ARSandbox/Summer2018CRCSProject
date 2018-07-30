using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// A class that handles generation of a UDP server that broadcasts connection
    /// information for a better TCP connection. Also handles the logic of finding
    /// this server on the server node for receiving clients sitting on other ASL
    /// nodes on a local area network.
    /// </summary>
    public class ServerFinder
    {
        #region Fields
        /// <summary>
        /// A variable to hold the server's IP address when it's generated (for 
        /// the server) or retrieved (for the clients).
        /// </summary>
        public static string serverIP;

        /// <summary>
        /// The thread associated with repeatedly accepting client requests for
        /// information.
        /// </summary>
        private static Thread thread_AcceptClient;
        #endregion

#if !UNITY_WSA_10_0
        /// <summary>
        /// The listener socket for receiving requests from clients attempting
        /// to gather server connection information.
        /// </summary>
        public static UdpClient listener;

        #region Methods
        /// <summary>
        /// Starts up a server on this ASL node that continually broadcasts
        /// information that lets other nodes know how to contact this server
        /// node for TCP connections.
        /// </summary>
        public static void ServerStart()
        {
            int listenerPort = Config.Ports.FindServer;
            listener = new UdpClient(listenerPort);
            serverIP = IPManager.GetLocalIpAddress().ToString();

            AcceptClient();
        }

        /// <summary>
        /// Processes logic for accepting information from a communication from
        /// an ASL node and responds with TCP connection information for this
        /// server node.
        /// </summary>
        public static void AcceptClient()
        {
            byte[] serverIPBytes = Encoding.UTF8.GetBytes(serverIP);

            if(thread_AcceptClient != null
                && thread_AcceptClient.IsAlive)
            {
                thread_AcceptClient.Abort();
            }

            thread_AcceptClient = new Thread(() =>
            {
                while (true)
                {
                    IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    // ERROR TESTING - NEED TO ASSIGN THE SYSTEM A PORT # THAT WORKS, INSTEAD OF 0 ABOVE
                    byte[] clientIPBytes = listener.Receive(ref clientEndpoint);
                    //string clientIPString = Encoding.UTF8.GetString(clientIPBytes);
                    listener.Send(serverIPBytes, serverIPBytes.Length, clientEndpoint);
                }
            });

            thread_AcceptClient.Start();
        }

        // IPAddress string
        /// <summary>
        /// Processes on the client nodes (i.e. not server nodes). Allows the 
        /// client to search for broadcast signals on the local area network to
        /// connect to them and request TCP connection information.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the server's IP address.
        /// </returns>
        public static string FindServer()
        {
            string IPString = string.Empty;

            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            int findServerPort = Config.Ports.FindServer;
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Broadcast, findServerPort);
            byte[] clientIPBytes = Encoding.UTF8.GetBytes(IPManager.GetLocalIpAddress().ToString());
            client.Send(clientIPBytes, clientIPBytes.Length, serverEndpoint);
            byte[] serverIPBytes = client.Receive(ref serverEndpoint);

            IPString = Encoding.UTF8.GetString(serverIPBytes);
            serverIP = IPString;
            return IPString;
        }

        /// <summary>
        /// Handles cleanup logic for threads associated with this class (Causes
        /// freeze in Unity otherwise).
        /// </summary>
        /// 
        /// <returns>
        /// True if the thread was closed, has been closed, or doesn't exist. 
        /// False otherwise.
        /// </returns>
        public static bool KillThreads()
        {
            if(thread_AcceptClient != null
                && thread_AcceptClient.IsAlive)
            {
                if(listener != null)
                {
                    listener.Close();
                    listener = null;
                }
                thread_AcceptClient.Abort();
                return !thread_AcceptClient.IsAlive;
            }

            return true;
        }
        #endregion
#endif
    }
}