using System;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization;
#if !UNITY_WSA_10_0
    using System.Runtime.Serialization.Formatters.Binary;
#endif
//using HoloToolkit.Unity;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// MasterClientLauncher implements launcher functionality specific to the 
    /// MasterClient. This can be inherited by platform-specific versions to
    /// adapt to platform-specific issues.
    /// 
    /// NOTE: Due to the awkwardness of the master client launcher and the
    /// frequency with which the network encounters disconnection issues,
    /// the MasterClientLauncher should be phased out in lieu of a more
    /// streamlined master client handler.
    /// </summary>
    public abstract class MasterClientLauncher : Launcher
    {
        #region Fields
        #region Private Fields
        /// <summary>
        /// Timestamp of the last Hololens room update. Used to determine 
        /// whether an update is appropriate.
        /// </summary>
        private DateTime lastRoomUpdate = DateTime.MinValue;

        /// <summary>
        /// Timestamp of the last update. Used to determine whether an update
        /// is appropriate.
        /// </summary>
        private DateTime _lastUpdate = DateTime.MinValue;

        /// <summary>
        /// List of threads used by previous ASL team. Used to track ongoing threads
        /// for TCP connection maintenance.
        /// </summary>
        List<Thread> threads = new List<Thread>(); //list of threads created for TCP connections

        /// <summary>
        /// Time delta between updates.
        /// </summary>
        private float updateTime = 5f; //time tracker to send a new room list to all clients
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Unity method called once per frame. 
        /// 
        /// Handle TangoDatabase updating for ensuring Tango Room caching is
        /// accurate and synchronized across all clients.
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            //    //AssetBundle object instantiation for testing purposes
            //    if (Input.GetKeyDown("i"))
            //    {
            //        int id1 = PhotonNetwork.AllocateViewID();

            //        photonView.RPC("SpawnNetworkObject", PhotonTargets.AllBuffered, transform.position, transform.rotation, id1, "Cube");
            //    }
            //}

            //Check to see if a new Tango Room Mesh has been recieved and create a gameobject and render the mesh in gamespace
            if (TangoDatabase.LastUpdate != DateTime.MinValue && DateTime.Compare(_lastUpdate, TangoDatabase.LastUpdate) < 0)
            {
                for (int i = 0; i < TangoDatabase.Rooms.Count; i++)
                {
                    TangoDatabase.TangoRoom T = TangoDatabase.GetRoom(i);

                    if (TangoDatabase.ID < T.ID)
                    {
                        TangoDatabase.ID = T.ID;
                        //    Create a material to apply to the mesh
                        Material meshMaterial = new Material(Shader.Find("Diffuse"));

                        //    grab the meshes in the database
                        IEnumerable<Mesh> temp = new List<Mesh>(TangoDatabase.GetMeshAsList(T));

                        foreach (var mesh in temp)
                        {
                            //        for each mesh in the database, create a game object to represent
                            //        and display the mesh in the scene
                            GameObject obj1 = new GameObject(T.name);    

                            //        add a mesh filter to the object and assign it the mesh
                            MeshFilter filter = obj1.AddComponent<MeshFilter>();
                            filter.mesh = mesh;

                            //        add a mesh rendererer and add a material to it
                            MeshRenderer rend1 = obj1.AddComponent<MeshRenderer>();
                            rend1.material = meshMaterial;

                            rend1.material.shader = Shader.Find("Custom/UnlitVertexColor");

                            obj1.tag = "Room";
                            obj1.AddComponent<TangoRoom>();

                            PhotonView parentView = PhotonView.Find(T.parentID);
                            if (parentView != null)
                                obj1.transform.SetParent(parentView.transform, false);
                        }
                    }
                }

                foreach (PhotonPlayer p in PhotonNetwork.otherPlayers)
                {
                    SendTangoRoomInfo(p.ID);
                }

                _lastUpdate = TangoDatabase.LastUpdate;
            }

            //Check if the Thread has stopped and join if so
            Thread[] TL = threads.ToArray();
            foreach (Thread T in TL)
            {
                if (T.ThreadState == ThreadState.Stopped)
                {
                    T.Join();
                    //threads.Remove(T);
                }
            }

            //decrement updateTime which controls the TangoRoomInfo sending between clients
            updateTime -= Time.deltaTime;
            if (updateTime <= 0)
            {
                SendTangoRoomInfoAll();
                updateTime = 5f;
            }
        }

        /// <summary>
        /// Handles triggering of the Hololens room caching process.
        /// 
        /// NOTE: This does not belong inside of the Launchers, as it is related
        /// solely to the scanning process.
        /// </summary>
        public void BeginRoomRefreshCycle()
        {
            float recommendedRefreshTime = 25.0f;
            InvokeRepeating("UpdateRoom", 0.0f, recommendedRefreshTime);
        }

        /// <summary>
        /// Handles updating of the Hololens scanned rooms.
        /// 
        /// NOTE: This does not belong inside of the Launchers, as it is related 
        /// solely to the scanning process.
        /// </summary>
        public void UpdateRoom()
        {
            //string path = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            //string path = Config.AssetBundle.Current.CompileAbsoluteBundlePath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            string path = Config.Current.AssetBundle.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
            Debug.Log("Asset path when updating room = " + path);
            if (File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                if ((int)f.LastWriteTime.CompareTo(lastRoomUpdate) > 0)
                {
                    // Update to room bundle has occurred
                    lastRoomUpdate = f.LastWriteTime;
                    for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
                    {
                        //SendRoomModel(PhotonNetwork.otherPlayers[i].ID);
                        PhotonNetwork.RPC(photonView, "RequestRoomModel", PhotonTargets.Others, false);
                    }
                }
            }
            else
            {
                Debug.Log("Room model not found at path " + path);
                PhotonNetwork.RPC(photonView, "RequestRoomModel", PhotonTargets.Others, false);
            }
        }

        /// <summary>
        /// After connecting to the Master Server, create a room using the 
        /// specified room name.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.CreateRoom(RoomName);
            //foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
            //{
            //    // ERROR TESTING - MUST APPROPRIATELY GET THE NODE TYPE OF THE OTHER PLAYER (HAS TO BE SET IN CUSTOM PROPERTIES)
            //    // LOOK AT UPDATE() IN LAUNCHER, AND SET APPROPRIATELY
            //    // NODETYPE.PC USED AS STOPGAP
            //    SendAssetBundles(player.ID);
            //}
            BeginRoomRefreshCycle();
        }

        /// <summary>
        /// Once a new player connects, the Master Client will send Tango Room 
        /// information to that new player.
        /// </summary>
        /// 
        /// <param name="newPlayer">
        /// The PhotonPlayer that just joined the ASL PUN network.
        /// </param>
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            SendTangoRoomInfo(newPlayer.ID);

            //UnityEngine.Debug.Log("Player Connected, Send Tango Mesh");

            base.OnPhotonPlayerConnected(newPlayer);
        }

        /// <summary>
        /// Sends a list of all Tango Rooms currently being stored in the 
        /// TangoDatabase to the PhotonPlayer ID. Clients can also request 
        /// updates directly.
        /// </summary>
        /// 
        /// <param name="id">
        /// The ID of the ASL PUN member that is receiving this Tango room info.
        /// </param>
        [PunRPC]
        public override void SendTangoRoomInfo(int id)
        {
            string data = "";

            if (TangoDatabase.Rooms.Count > 0)
            {
                data = TangoDatabase.GetAllRooms();
            }

            photonView.RPC("RecieveTangoRoomInfo", PhotonPlayer.Find(id), data);

        }

        /// <summary>
        /// Sends a list of all Tango Rooms currently being stored in 
        /// the TangoDatabase to all Players.
        /// </summary>
        public void SendTangoRoomInfoAll()
        {
            string data = "";

            if (TangoDatabase.Rooms.Count > 0)
            {
                data = TangoDatabase.GetAllRooms();
            }

            photonView.RPC("RecieveTangoRoomInfo", PhotonTargets.All, data);

        }

        /// <summary>
        /// PunRPC call that is sent by a client requesting a specific room mesh
        /// Creates a Thread that handles sending the byte stream to the client
        /// </summary>
        /// 
        /// <param name="networkConfig">
        /// A string representing the IP address and port of the client
        /// receiving Tango Mesh.
        /// </param>
        [PunRPC]
        public override void SendTangoMeshByName(string networkConfig)
        {
            Thread T = new Thread(() => SendTangoMeshThread(networkConfig));
            T.IsBackground = true;
            T.Start();
            threads.Add(T);
        }

        /// <summary>
        /// Parses the networkConfig string for the clients TCP info and room name to send to the client
        /// Retrieves the room from TangoDatabase based on it's name
        /// Establishes a TCP connection with the client and sends the byte data for the room
        /// </summary>
        /// 
        /// <param name="networkConfig">
        /// A string representing the IP address and port of the client receiving
        /// the Tango mesh.
        /// </param>
        void SendTangoMeshThread(string networkConfig)
        {
            var networkConfigArray = networkConfig.Split('~');
            string name = networkConfigArray[2];

            UnityEngine.Debug.Log("Sending Tango Mesh Master Client " + name);

            TcpClient client = new TcpClient();

            client.Connect(IPAddress.Parse(networkConfigArray[0]), System.Int32.Parse(networkConfigArray[1]));

            using (NetworkStream stream = client.GetStream())
            {
                TangoDatabase.TangoRoom T = TangoDatabase.GetRoomByName(name);

                //write the parent id
                var parentData = BitConverter.GetBytes(T.parentID);
                stream.Write(parentData, 0, parentData.Length);

                var data = T._meshes;
                stream.Write(data, 0, data.Length);
                UnityEngine.Debug.Log("Mesh sent: mesh size = " + data.Length);
            }
            client.Close();

            Thread.CurrentThread.Join();

        }

        /// <summary>
        /// Recieves a PunRPC request to recieve a new Tango Room Mesh
        /// Creates a thread to handle the TCP connection
        /// Sends an RPC call back to for the client to send the mesh
        /// </summary>
        /// 
        /// <param name="id">
        /// ID of the ASL PUN player.
        /// </param>
        /// <param name="size">
        /// Size of the byte array representing the mesh.
        /// </param>
        [PunRPC]
        public override void ReceiveTangoMesh(int id, int size)
        {
            // Setup TCPListener to wait and receive mesh
            //this.DeleteLocalMesh();
            UnityEngine.Debug.Log("RecieveMesh Create Thread");
            Thread T = new Thread(() => RecieveTangoMeshThread(id, size));
            T.IsBackground = true;
            T.Start();
            threads.Add(T);

            UnityEngine.Debug.Log("Thread Created");

            photonView.RPC("SendTangoMesh", PhotonPlayer.Find(id), GetLocalIpAddress() + ":" + (Port + 1));
        }

        /// <summary>
        /// Establishes a TCPListener to recieve a connection from the client
        /// Reads the byte stream and updates the mesh in the TangoDatabase.cs
        /// Terminates the Thread once finished
        /// </summary>
        /// 
        /// <param name="id">
        /// ID of the client sending the Tango Mesh.
        /// </param>
        /// <param name="size">
        /// Size of the byte array representing tango mesh.
        /// </param>
        void RecieveTangoMeshThread(int id, int size)
        {
            TcpListener receiveTcpListener = new TcpListener(IPAddress.Any, Port + 1);
            receiveTcpListener.Start();
            var client = receiveTcpListener.AcceptTcpClient();
            using (var stream = client.GetStream())
            {
                byte[] data = new byte[size];

                Debug.Log("Start receiving mesh");
                using (MemoryStream ms = new MemoryStream())
                {
                    int numBytesRead;
                    while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
                    {
                        ms.Write(data, 0, numBytesRead);
                    }
                    Debug.Log("finish receiving mesh: size = " + ms.Length);
                    client.Close();
                    TangoDatabase.UpdateMesh(ms.ToArray(), id);
                }
            }
            client.Close();
            receiveTcpListener.Stop();

            Thread.CurrentThread.Join();
        }

        ///// <summary>
        ///// After creating a room, set up a multi-threading tcp listener to listen on the specified port
        ///// Once someone connects to the port, send the currently saved (in Database) Room Mesh
        ///// It creates a second tcp listener to send asset bundles across Port+1
        ///// </summary>
        //public override void OnCreatedRoom()
        //{
        //    TcpListener server = new TcpListener(IPAddress.Any, Port);
        //    server.Start();
        //    new Thread(() =>
        //    {
        //        Debug.Log("MasterClient start listening for new connection");
        //        while (true)
        //        {
        //            SendRoomPrefabBundle();
        //            //TcpClient client = server.AcceptTcpClient();
        //            //Debug.Log("New connection established");
        //            //new Thread(() =>
        //            //{
        //            //    //using (NetworkStream stream = client.GetStream())
        //            //    //{
        //            //    //    var data = Database.GetMeshAsBytes();
        //            //    //    stream.Write(data, 0, data.Length);
        //            //    //    Debug.Log("Mesh sent: mesh size = " + data.Length);
        //            //    //}
        //            //    client.Close();
        //            //}).Start();

        //        }
        //    }).Start();
        //}

        /// <summary>
        /// This returns local IP address
        /// </summary>
        /// <returns>Local IP address of the machine running as the Master Client</returns>
        private IPAddress GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    return ip;
                }
            }
            return null;
        }

        ///// <summary>
        ///// Performs the actual sending of bundles.  The path is determined
        ///// by the calling function which is dependent upon platform
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="path"></param>
        ///// <param name="port"></param>
        //private void SendBundle(int id, string path, int port)
        //{
        //    TcpListener bundleListener = new TcpListener(IPAddress.Any, port);

        //    bundleListener.Start();
        //            new Thread(() =>
        //                {
        //                    var client = bundleListener.AcceptTcpClient();

        //                    using (var stream = client.GetStream())
        //                    {
        //                        //needs to be changed back
        //                        byte[] data = File.ReadAllBytes(path);
        //                        stream.Write(data, 0, data.Length);
        //                        client.Close();
        //                    }

        //                    bundleListener.Stop();
        //                }).Start();
        //}

        //[PunRPC]
        //public void SendRoomPrefabBundle(int id)
        //{
        //    string path = UWB_Texturing.Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(UWB_Texturing.Config.AssetBundle.RoomPackage.CompileFilename());
        //    if (File.Exists(path))
        //    {
        //        int roomBundlePort = Port + 8;
        //        SendBundle(id, path, roomBundlePort);
        //        photonView.RPC("ReceiveRoomPrefabBundle", PhotonPlayer.Find(id), GetLocalIpAddress() + ":" + roomBundlePort);
        //    }
        //}

        //#region RPC Method

        ///// <summary>
        ///// Send mesh to a host specified by PhotonNetwork.Player.ID 
        ///// This is a RPC method that will be called by ReceivingClient
        ///// </summary>
        ///// <param name="id">The player id that will sent the mesh</param>
        //[PunRPC]
        //public override void SendRoomModel(int id)
        //{
        //    if (Database.GetMeshAsBytes() != null)
        //    {
        //        photonView.RPC("ReceiveMesh", PhotonPlayer.Find(id), GetLocalIpAddress() + ":" + Port);
        //    }
        //}


        ///// <summary>
        ///// Send bundles for PC
        ///// </summary>
        ///// <param name="id"></param>
        //[PunRPC]
        //public void SendPCBundles(int id)
        //{
        //    string path = Application.dataPath + "/ASL/StreamingAssets/AssetBundlesPC";
        //    foreach (string file in System.IO.Directory.GetFiles(path))
        //    {
        //        if (file.Contains("networkbundle") && !file.Contains("manifest") && !file.Contains("meta"))
        //        {
        //            int bundlePort_PC = Port + 5;
        //            SendBundle(id, file, bundlePort_PC);
        //            photonView.RPC("ReceiveBundles", PhotonPlayer.Find(id), GetLocalIpAddress() + ":" + bundlePort_PC);
        //        }
        //    }
        //}        

        ///// <summary>
        ///// Send bundles for Android
        ///// </summary>
        ///// <param name="id"></param>
        //[PunRPC]
        //public void SendAndroidBundles(int id)
        //{
        //    string path = Application.dataPath + "/ASL/StreamingAssets/AssetBundlesAndroid";
        //    foreach (string file in System.IO.Directory.GetFiles(path))
        //    {
        //        if (file.Contains("networkbundle") && !file.Contains("manifest") && !file.Contains("meta"))
        //        {
        //            int bundlePort_Android = Port + 2;
        //            SendBundle(id, file, bundlePort_Android);
        //            photonView.RPC("ReceiveBundles", PhotonPlayer.Find(id), GetLocalIpAddress() + ":" + bundlePort_Android);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Send bundles for hololens
        ///// </summary>
        ///// <param name="id"></param>
        //[PunRPC]
        //public void SendHololensBundles(int id)
        //{
        //    string path = Application.dataPath + "/ASL/StreamingAssets/AssetBundlesHololens";
        //    foreach (string file in System.IO.Directory.GetFiles(path))
        //    {
        //        if (file.Contains("networkbundle") && !file.Contains("manifest") && !file.Contains("meta"))
        //        {
        //            int bundlePort_Hololens = Port + 3;
        //            SendBundle(id, file, bundlePort_Hololens);
        //            photonView.RPC("ReceiveBundles", PhotonPlayer.Find(id), GetLocalIpAddress() + ":" + bundlePort_Hololens);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Receive room mesh from specifed PhotonNetwork.Player.ID
        ///// This is a RPC method this will be called by HoloLens
        ///// </summary>
        ///// <param name="id">The player id that will receive mesh</param>
        //[PunRPC]
        //public override void ReceiveMesh(int id)
        //{
        //    // Setup TCPListener to wait and receive mesh
        //    this.DeleteLocalRoomModelInfo();
        //    TcpListener receiveTcpListener = new TcpListener(IPAddress.Any, Port + 1);
        //    receiveTcpListener.Start();
        //    new Thread(() =>
        //    {
        //        var client = receiveTcpListener.AcceptTcpClient();
        //        using (var stream = client.GetStream())
        //        {
        //            byte[] data = new byte[1024];

        //            Debug.Log("Start receiving mesh");
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                int numBytesRead;
        //                while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
        //                {
        //                    ms.Write(data, 0, numBytesRead);
        //                }
        //                Debug.Log("finish receiving mesh: size = " + ms.Length);
        //                client.Close();
        //                Database.UpdateMesh(ms.ToArray());
        //            }
        //        }
        //        client.Close();
        //        receiveTcpListener.Stop();
        //        photonView.RPC("ReceiveMesh", PhotonTargets.Others, GetLocalIpAddress() + ":" + Port);
        //    }).Start();

        //    photonView.RPC("SendMesh", PhotonPlayer.Find(id), GetLocalIpAddress() + ":" + (Port + 1));
        //}

        ///// <summary>
        ///// This will send a call to delete all meshes held by the clients
        ///// This is a RPC method that will be called by ReceivingClient
        ///// </summary>
        //[PunRPC]
        //public override void DeleteRoomInfo()
        //{
        //    this.DeleteLocalRoomModelInfo();
        //    photonView.RPC("DeleteLocalRoomInfo", PhotonTargets.Others);
        //    //if (Database.GetMeshAsBytes() != null)
        //    //{
        //    //    photonView.RPC("DeleteLocalMesh", PhotonTargets.Others);
        //    //}
        //    //this.DeleteLocalMesh();
        //}

        ///// <summary>
        ///// Initiates the sending of a Mesh to add
        ///// </summary>
        //public override void SendAddMesh()
        //{
        //    if (Database.GetMeshAsBytes() != null)
        //    {
        //        photonView.RPC("ReceiveAddMesh", PhotonTargets.Others, GetLocalIpAddress() + ":" + Port);
        //    }
        //}

        ///// <summary>
        ///// Receive room mesh from specifed PhotonNetwork.Player.ID. 
        ///// and add it to the total roommesh
        ///// </summary>
        ///// <param name="networkConfig">The player id that will receive mesh</param>
        ///// <param name="networkOrigin">The origin of the sent mesh</param>
        //[PunRPC]
        //public override void ReceiveAddMesh(int id)
        //{
        //    // Setup TCPListener to wait and receive mesh
        //    TcpListener receiveTcpListener = new TcpListener(IPAddress.Any, Port + 4);
        //    receiveTcpListener.Start();
        //    new Thread(() =>
        //    {
        //        var client = receiveTcpListener.AcceptTcpClient();
        //        using (var stream = client.GetStream())
        //        {
        //            byte[] data = new byte[1024];

        //            Debug.Log("Start receiving mesh");
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                int numBytesRead;
        //                while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
        //                {
        //                    ms.Write(data, 0, numBytesRead);
        //                }
        //                Debug.Log("finish receiving mesh: size = " + ms.Length);
        //                client.Close();
        //                Database.AddToMesh(ms.ToArray());
        //            }
        //        }
        //        client.Close();
        //        receiveTcpListener.Stop();
        //        photonView.RPC("ReceiveMesh", PhotonTargets.Others, GetLocalIpAddress() + ":" + Port);
        //    }).Start();

        //    photonView.RPC("SendAddMesh", PhotonPlayer.Find(id), GetLocalIpAddress() + ":" + (Port + 4));
        //}

        //#endregion
        #endregion
    }
}

