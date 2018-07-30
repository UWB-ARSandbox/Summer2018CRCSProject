using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using Windows.System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams; // DataReader/DataWriter & Streams
using Windows.Security.Cryptography; // Convert string to bytes
#endif

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Hololens modification of the Socket Base class specific to clients.
    /// </summary>
    public class SocketClient_Hololens : Socket_Base_Hololens
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        /// <summary>
        /// Used to request files from the server.
        /// </summary>
        /// 
        /// <param name="port">
        /// The port associated with the content being requested.
        /// </param>
        /// <param name="receiveDirectory">
        /// The directory to store the received files.
        /// </param>
        public static void RequestFiles(int port, string receiveDirectory)
        {
            RequestFiles(ServerFinder.serverIP, port, receiveDirectory);
        }

        /// <summary>
        /// Request files from a given IP address.
        /// </summary>
        /// 
        /// <param name="serverIP">
        /// The IP address of the server being addressed.
        /// </param>
        /// <param name="port">
        /// The port associated with the content being requested.
        /// </param>
        /// <param name="receiveDirectory">
        /// The directory to store the received files.
        /// </param>
        public static void RequestFiles(string serverIP, int port, string receiveDirectory)
        {
            new Task(async () =>
            {
                // Generate the socket and connect to the server
                StreamSocket socket = new StreamSocket();
                
                int serverPort = Config.Ports.ClientServerConnection;
                EndpointPair endpointPair = new EndpointPair(new HostName(IPManager.GetLocalIpAddress()), port.ToString(), new HostName(serverIP), serverPort.ToString());
                await socket.ConnectAsync(endpointPair);

                // After awaiting the connection, receive data appropriately
                ReceiveFilesAsync(socket, receiveDirectory);

                socket.Dispose();
            }).Start();
        }

        /// <summary>
        /// Send a file to the server.
        /// </summary>
        /// 
        /// <param name="remoteIP">
        /// The IP address to send to.
        /// </param>
        /// <param name="port">
        /// The port associated with the content being sent out.
        /// </param>
        /// <param name="filepath">
        /// The filepath associated with the file you want to send out.
        /// </param>
        public static void SendFile(string remoteIP, int port, string filepath)
        {
            SendFiles(remoteIP, port, new string[1] { filepath });
        }

        /// <summary>
        /// Send multiple files to the server.
        /// </summary>
        /// 
        /// <param name="remoteIP">
        /// The IP address to send to.
        /// </param>
        /// <param name="port">
        /// The port associated with the content being sent out.
        /// </param>
        /// <param name="filepaths">
        /// The filepaths associated with the files you want to send out.
        /// </param>
        public static void SendFiles(string remoteIP, int port, string[] filepaths)
        {
            new Task(async () =>
            {
                int serverPort = Config.Ports.ClientServerConnection;
                EndpointPair endpointPair = new EndpointPair(new HostName(IPManager.GetLocalIpAddress()), port.ToString(), new HostName(remoteIP), serverPort.ToString());
                StreamSocket socket = new StreamSocket();

                await socket.ConnectAsync(endpointPair);
                Socket_Base_Hololens.SendFilesAsync(filepaths, socket);

                socket.Dispose();
            }).Start();
        }
#endif
    }
}