using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
#endif

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Handles logic related to grabbing and interpreting IP addresses.
    /// </summary>
    public static class IPManager
    {
        //public static IPAddress GetLocalIpAddress()
        //{
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (IPAddress ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily.ToString() == "InterNetwork")
        //        {
        //            return ip;
        //        }
        //    }
        //    return null;
        //}

        /// <summary>
        /// Compiles a string representing the IP address of this client node.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the local IP address of this ASL client node.
        /// </returns>
        public static string GetLocalIpAddress()
        {
#if !UNITY_EDITOR && UNITY_WSA_10_0
            string ip = null;

            // IPInformation is null if not HostName is not a local IPv4 or
            // IPv6 address retrieved from GetHostNames()
            foreach (HostName localHostName in NetworkInformation.GetHostNames())
            {
                if (localHostName.IPInformation != null)
                {
                    if (localHostName.Type == HostNameType.Ipv4)
                    {
                        localHostName.ToString(); // Get the IP address from the host name
                        break;
                    }
                }
            }

            return ip;
#else
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    return ip.ToString();
                }
            }
            return null;
#endif
        }

        /// <summary>
        /// Compiles a string representing the IP address of this ASL node and
        /// a specified port. The two are separated by a colon. Used for joining
        /// and sending communication endpoint information.
        /// (ex: "192.168.1.1:17652").
        /// </summary>
        /// 
        /// <param name="port">
        /// The port to include in the string.
        /// </param>
        /// 
        /// <returns>
        /// A string with the IP address and port. (ex.: "192.168.1.1:17652")
        /// </returns>
        public static string CompileNetworkConfigString(int port)
        {
            return GetLocalIpAddress() + ":" + port;
        }

        /// <summary>
        /// Extracts the IP address from a network config string.
        /// </summary>
        /// 
        /// <param name="networkConfigString">
        /// The network configuration string also from this class.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the IP address.
        /// </returns>
        public static string ExtractIPAddress(string networkConfigString)
        {
            return networkConfigString.Split(':')[0];
        }

        /// <summary>
        /// Extracts the port from a network configuration string.
        /// </summary>
        /// 
        /// <param name="networkConfigString">
        /// The network configuration string also from this class.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the port.
        /// </returns>
        public static string ExtractPort(string networkConfigString)
        {
            return networkConfigString.Split(':')[1];
        }

        /// <summary>
        /// Constant representing the broadcast IP address.
        /// </summary>
        public static string BroadcastIP
        {
            get
            {
                return "255.255.255.255";
            }
        }
    }
}