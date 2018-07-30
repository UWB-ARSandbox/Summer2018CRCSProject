﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// The types of platforms that ASL anticipates and attempts to provide
    /// for.
    /// </summary>
    public enum NodeType
    {
        Android,
        Hololens,
        Kinect,
        Oculus,
        Vive,
        PC,
        Tango, // The Google Tango AR Phone - an Android
        iOS
    };

    /// <summary>
    /// Config classes should inherit from this class. This class provides 
    /// the paths, names, and methods to retrieve those paths and names that 
    /// serve as a baseline. Platforms may deviate from the logic (and thus 
    /// require customization in their own classes), but this should serve 
    /// as a good baseline for every platform.
    /// </summary>
    public class Config_Base
    {
        #region Fields/Properties

        public static class Messages
        {
            public static string PlatformNotFound = "Platform not found. Please reference NodeType enum in Config_Base file.";
        }

        private static NodeType nodeType = NodeType.PC;
        public static NodeType NodeType
        {
            get
            {
                return nodeType;
            }
            set
            {
                nodeType = value;
            }
        }

        #endregion

        #region Methods
        public static void Start(NodeType platform)
        {
            NodeType = platform;
        }
        #endregion

        /// <summary>
        /// Handles everything that should be associated with port configuration.
        /// This information should be the same across all platforms for all nodes
        /// so that the nodes are able to communicate with each other.
        /// </summary>
        public static class Ports
        {
            /// <summary>
            /// The types of inter-node behavior we want to account for. Add here
            /// if you would like to customize your own inter-node behavior.
            /// </summary>
            public enum Types
            {
                Bundle,
                Bundle_ClientToServer,
                RoomResourceBundle,
                RoomResourceBundle_ClientToServer,
                RoomBundle,
                RoomBundle_ClientToServer,
                AndroidBundle,
                AndroidBundle_ClientToServer,
                AndroidRoomResourceBundle,
                AndroidRoomResourceBundle_ClientToServer,
                AndroidRoomBundle,
                AndroidRoomBundle_ClientToServer,
                ClientServerConnection,
                FindServer
            }

            /// <summary>
            /// Translates a port type to an actual port number.
            /// </summary>
            /// 
            /// <param name="portType">
            /// The type of port behavior you want to get the port for.
            /// </param>
            /// 
            /// <returns>
            /// The port.
            /// </returns>
            public static int GetPort(Types portType)
            {
                switch (portType)
                {
                    case Types.Bundle:
                        return Bundle;
                    case Types.Bundle_ClientToServer:
                        return Bundle_ClientToServer;
                    case Types.RoomResourceBundle:
                        return RoomResourceBundle;
                    case Types.RoomResourceBundle_ClientToServer:
                        return RoomResourceBundle_ClientToServer;
                    case Types.RoomBundle:
                        return RoomBundle;
                    case Types.RoomBundle_ClientToServer:
                        return RoomBundle_ClientToServer;
                    case Types.ClientServerConnection:
                        return ClientServerConnection;
                    case Types.FindServer:
                        return FindServer;
                    case Types.AndroidBundle:
                        return AndroidBundle;
                    case Types.AndroidBundle_ClientToServer:
                        return AndroidBundle_ClientToServer;
                    case Types.AndroidRoomResourceBundle:
                        return AndroidRoomResourceBundle;
                    case Types.AndroidRoomResourceBundle_ClientToServer:
                        return AndroidRoomResourceBundle_ClientToServer;
                    case Types.AndroidRoomBundle:
                        return AndroidRoomBundle;
                    case Types.AndroidRoomBundle_ClientToServer:
                        return AndroidRoomBundle_ClientToServer;
                }

                return Base;
            }

            /// <summary>
            /// Translates a port number into a port type. Allows insight into
            /// what kind of code or behavior may be associated with a given port.
            /// </summary>
            /// 
            /// <param name="port">
            /// The port being investigated.
            /// </param>
            /// 
            /// <returns>
            /// The Port Type. (Behavior or target of the port)
            /// </returns>
            public static Types GetPortType(int port)
            {
                if (port == Bundle)
                    return Types.Bundle;
                else if (port == Bundle_ClientToServer)
                    return Types.Bundle_ClientToServer;
                else if (port == RoomResourceBundle)
                    return Types.RoomResourceBundle;
                else if (port == RoomResourceBundle_ClientToServer)
                    return Types.RoomResourceBundle_ClientToServer;
                else if (port == RoomBundle)
                    return Types.RoomBundle;
                else if (port == RoomBundle_ClientToServer)
                    return Types.RoomBundle_ClientToServer;
                else if (port == ClientServerConnection)
                    return Types.ClientServerConnection;
                else if (port == FindServer)
                    return Types.FindServer;
                else if (port == AndroidBundle)
                    return Types.AndroidBundle;
                else if (port == AndroidBundle_ClientToServer)
                    return Types.AndroidBundle_ClientToServer;
                else if (port == AndroidRoomResourceBundle)
                    return Types.AndroidRoomResourceBundle;
                else if (port == AndroidRoomResourceBundle_ClientToServer)
                    return Types.AndroidRoomResourceBundle_ClientToServer;
                else if (port == AndroidRoomBundle)
                    return Types.AndroidRoomBundle;
                else if (port == AndroidRoomBundle_ClientToServer)
                    return Types.AndroidRoomBundle_ClientToServer;
                else
                {
                    Debug.LogError("Port type not found for port " + port);
                    throw new System.Exception();
                }
            }

            /// <summary>
            /// The base port that is associated with this ASL network. This is
            /// currently typed in manually through the NetworkManager.
            /// </summary>
            private static int port = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().Port;
            /// <summary>
            /// Returns the base port associated with this ASL network.
            /// </summary>
            public static int Base
            {
                get
                {
                    return port;
                }
                set
                {
                    if (value < 64000 && value > 20000)
                    {
                        port = value;
                    }
                    else
                    {
                        Debug.Log("Invalid port chosen. Please select a port between 20000 and 64000");
                    }
                }
            }
            /// <summary>
            /// Returns the port associated with finding a server.
            /// </summary>
            public static int FindServer
            {
                get
                {
                    return Base + 1;
                }
            }
            /// <summary>
            /// Returns the port associated with client-server connections.
            /// </summary>
            public static int ClientServerConnection
            {
                get
                {
                    return Base + 2;
                }
            }
            /// <summary>
            /// Returns the port associated with transferring generic asset 
            /// bundles.
            /// </summary>
            public static int Bundle
            {
                get
                {
                    return Base + 3;
                }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle from
            /// the client to the server.
            /// </summary>
            public static int Bundle_ClientToServer
            {
                get
                {
                    return Base + 4;
                }
            }
            /// <summary>
            /// Returns the port associated with sending a Room Resource asset
            /// bundle (textures and files associated with a Hololens scanned
            /// room).
            /// </summary>
            public static int RoomResourceBundle
            {
                get
                {
                    return Base + 5;
                }
            }
            /// <summary>
            /// Returns the port associated with sending a Room Resource asset
            /// bundle (textures and files associated with a Hololens scanned
            /// room) from the client to a server.
            /// </summary>
            public static int RoomResourceBundle_ClientToServer
            {
                get
                {
                    return Base + 6;
                }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle containing
            /// a Hololens scanned room.
            /// </summary>
            public static int RoomBundle
            {
                get
                {
                    return Base + 7;
                }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle containing
            /// a Hololens scanned room from a client to a server.
            /// </summary>
            public static int RoomBundle_ClientToServer
            {
                get
                {
                    return Base + 8;
                }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle for the
            /// Android platform.
            /// </summary>
            public static int AndroidBundle
            {
                get { return Base + 9; }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle for the 
            /// Android platform from a client to the server.
            /// </summary>
            public static int AndroidBundle_ClientToServer
            {
                get { return Base + 10; }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle for the
            /// Android platform containing resources like textures for a Hololens 
            /// scanned room.
            /// </summary>
            public static int AndroidRoomResourceBundle
            {
                get { return Base + 11; }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle for the
            /// Android platform containing resources like textures for a Hololens 
            /// scanned room from a client to a server.
            /// </summary>
            public static int AndroidRoomResourceBundle_ClientToServer
            {
                get { return Base + 12; }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle for the
            /// Android platform containing a Hololens scanned room.
            /// </summary>
            public static int AndroidRoomBundle
            {
                get { return Base + 13; }
            }
            /// <summary>
            /// Returns the port associated with sending an asset bundle for the
            /// Android platform containing a Hololens scanned room from a client
            /// to a server.
            /// </summary>
            public static int AndroidRoomBundle_ClientToServer
            {
                get { return Base + 14; }
            }
        }
    }
}
