using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.Security.Cryptography;
using Windows.Networking; // HostName
using Windows.Networking.Sockets;
using System.Runtime.InteropServices.WindowsRuntime; // Used for ToArray extension method for IBuffers (to convert them to byte arrays)
using Windows.Storage.Streams; // DataWriter/DataReader
#endif

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Modification of ServerFinder class to adapt to Hololens requirements.
    /// Used to find and connect to a local broadcast server housed by the
    /// ASL master client for legitimate data transferral of digital objects.
    /// </summary>
    public class ServerFinder_Hololens
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        #region Fields
        /// <summary>
        /// The IP address of the server that is found. Starts empty but gets
        /// filled when the server is found.
        /// </summary>
        public static string serverIP;

        /// <summary>
        /// The UDP socket used to receive information from the broadcaster that
        /// tells this client what the IPAddress of the content server (i.e. 
        /// master client host IP) is.
        /// </summary>
        public static DatagramSocket listener;
        #endregion

        #region Methods
        /// <summary>
        /// Starts up a UDP broadcast server that sends out information describing
        /// this node's IP address and connection port.
        /// </summary>
        public static async void ServerStartAsync()
    {
        // Get information for connecting or for later reference
        int listenerPort = Config.Ports.FindServer;
        serverIP = IPManager.GetLocalIpAddress();

        // Generate the listener server socket
        listener = new DatagramSocket();
        listener.MessageReceived += AcceptClient;
        // Assumes default inbound buffer size
        await listener.BindEndpointAsync(new HostName(IPManager.GetLocalIpAddress()), Config.Ports.FindServer.ToString());
    }
    
    /// <summary>
    /// Method for when the server receives a request from the client for connection
    /// information. Sends server IP address and connection port to client.
    /// </summary>
    /// 
    /// <param name="listener">
    /// The server socket.
    /// </param>
    /// <param name="args">
    /// The DatagramSocketMessageReceivedEventArgs associated with the client 
    /// attempting to talk to the server.
    /// </param>
    public static void AcceptClient(DatagramSocket listener, DatagramSocketMessageReceivedEventArgs args)
    {
        new Task(async () =>
        {
            // Retrieve client IP info
            byte[] serverIPBytes = CryptographicBuffer.ConvertStringToBinary(serverIP, BinaryStringEncoding.Utf8).ToArray();
            //HostName clientIP = args.RemoteAddress;
            //string clientPort = args.RemotePort;
            DataReader reader = args.GetDataReader();
            uint numBytesLoaded = await reader.LoadAsync(1024);
            //byte[] buffer = new byte[reader.UnconsumedBufferLength];
            //reader.ReadBytes(buffer);

            string clientIP = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, reader.ReadBuffer(reader.UnconsumedBufferLength));

            DataWriter writer = new DataWriter(await listener.GetOutputStreamAsync(new HostName(clientIP), Config.Ports.FindServer.ToString()));

            writer.WriteBytes(serverIPBytes);
            await writer.StoreAsync(); // necessary??

            // Reset listening status
            await listener.ConnectAsync(new HostName("0.0.0.0"), Config.Ports.FindServer.ToString());

        }).Start();
    }
        
    /// <summary>
    /// Begins client protocol for finding an ASL content server (i.e. Master client
    /// host) on the local area network.
    /// </summary>
    public static async void FindServerAsync()
    {
        string IPString = string.Empty;

        DatagramSocket clientSocket = new DatagramSocket();
        string clientIP = IPManager.GetLocalIpAddress();

        clientSocket.MessageReceived += FoundServer;

        //await clientSocket.BindEndpointAsync(new HostName(IPManager.GetLocalIpAddress()), Config.Ports.FindServer.ToString());
        await clientSocket.ConnectAsync(new HostName(IPManager.BroadcastIP), Config.Ports.FindServer.ToString());
        DataWriter writer = new DataWriter(await clientSocket.GetOutputStreamAsync(new HostName(IPManager.BroadcastIP), Config.Ports.FindServer.ToString()));
        byte[] clientIPBytes = CryptographicBuffer.ConvertStringToBinary(clientIP, BinaryStringEncoding.Utf8).ToArray();
        writer.WriteBytes(clientIPBytes);
    }

    /// <summary>
    /// Logic to handle when a server responds with its connection information.
    /// </summary>
    /// 
    /// <param name="clientSocket">
    /// The client socket for receiving information from the server.
    /// </param>
    /// <param name="args">
    /// The DatagramSocketMessageReceivedEventArgs associated with the server
    /// attempting to talk to the client.
    /// </param>
    public static void FoundServer(DatagramSocket clientSocket, DatagramSocketMessageReceivedEventArgs args)
    {
        new Task(async () =>
        {
            DataReader reader = args.GetDataReader();
            await reader.LoadAsync(1024);
            serverIP = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, reader.ReadBuffer(reader.UnconsumedBufferLength));
        }).Start();
    }

    /// <summary>
    /// Handles cleanup for all threads associated with the ServerFinder class.
    /// </summary>
    /// 
    /// <returns>
    /// Returns true if the threads were handled properly.
    /// </returns>
    public static bool KillThreads()
        {
            listener.Dispose();
        }
        #endregion
#endif
    }
}