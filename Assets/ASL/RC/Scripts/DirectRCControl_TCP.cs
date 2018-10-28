using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class DirectRCControl_TCP : MonoBehaviour
{
    private const string rcAddress = "172.24.1.1";
    private const short rcCPort = 1070;
    private const short MAX_MESSAGE_LENGTH = 16;

    private TcpClient client;
    private IPEndPoint ep;
    private NetworkStream sock;

    private bool connected;
    private bool connectionClosed;

    private Byte[] rBuff;
    private Byte[] sBuff;

    private bool headingDirty;
    private bool distanceDirty;
    private bool isMoving;

    private float lastHeading;
    private float headingOffset;
    private short lastCommand;

    void Start() {
        connected = false;
        connectionClosed = true;
        headingDirty = false;
        distanceDirty = false;
        isMoving = false;
        sBuff = new Byte[MAX_MESSAGE_LENGTH];
        rBuff = new Byte[MAX_MESSAGE_LENGTH];

        connectRemote();
    }

    void Update() {
        if (connected) {
            if (headingDirty) {
                updateHeading();
            }

            if (distanceDirty) {
                updateDistance();
            }

            updateCommand();
        } else {
            if (!connectionClosed) {
                connectionClosed = true;
                print("Closing the TCP connection to the remote control car");
                sock.Close();
                client.Close();
            }
        }
    }

    void connectRemote()
    {
        client = new TcpClient();
        ep = new IPEndPoint(IPAddress.Parse(rcAddress), rcCPort);
        //ep = new IPEndPoint(IPAddress.Loopback, rcCPort);
        client.Connect(ep);
        sock = client.GetStream();
        connected = true;
        connectionClosed = false;
        print("TcpClient is connected to the end point with address: " +
        rcAddress + " and port: " + rcCPort);
    }

    bool updateHeading()
    {
        float heading = getData();
        if (heading != -1)
        {
            setNewHeading(heading);
            headingDirty = false;
        }
        else
        {
            print("A new heading could not be read from the stream");
            return false;
        }
        return true;
    }

    float getData()
    {
        int headLen = 0;
        string tempS = "";
        int temp = 0;
        while (temp != -1 && (char)temp != 'l')
        {
            temp = sock.ReadByte();
            if ((char)temp != 'l')
                tempS += (char)temp;
        }
        headLen = Int32.Parse(tempS);
        int bRead = 0;
        if (sock.CanRead)
        {
            while (bRead < headLen)
            {
                bRead += sock.Read(rBuff, bRead, headLen);
            }
            return Single.Parse(Encoding.UTF8.GetString(rBuff, 0, bRead));
        }
        else
            return -1;
    }

    void setNewHeading(float target)
    {
        // Quaternion rQ = Quaternion.Euler(0, (target + headingOffset), 0);
        // xform.rotation = rQ;
        lastHeading = target;
    }

    bool updateDistance()
    {
        float dist = getData();
        if (dist != -1)
        {
            // setTranslation(dist);
            distanceDirty = false;
        }
        else
        {
            print("A new distance could not be read from the stream");
            return false;
        }
        return true;
    }

    void updateCommand()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            sBuff = Encoding.Default.GetBytes("F");
            lastCommand = 1;
            headingDirty = true;
            distanceDirty = true;
            sendCommand();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            sBuff = Encoding.Default.GetBytes("B");
            lastCommand = 2;
            headingDirty = true;
            distanceDirty = true;
            sendCommand();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            sBuff = Encoding.Default.GetBytes("L");
            lastCommand = 3;
            headingDirty = true;
            sendCommand();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            sBuff = Encoding.Default.GetBytes("R");
            lastCommand = 4;
            headingDirty = true;
            sendCommand();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            sBuff = Encoding.Default.GetBytes("E");
            sendCommand();
            connected = false;
            headingDirty = false;
        }
        else
        {
            if (lastCommand > 0)
            {
                sBuff = Encoding.ASCII.GetBytes("S");
                lastCommand = 0;
                headingDirty = false;
                distanceDirty = false;
                sendCommand();
            }
        }
    }

    void sendCommand()
    {
        if (connected)
        {
            if (sock.CanWrite)
            {
                sock.Write(sBuff, 0, sBuff.Length);
            }
        }
        else
        {
            print("The sendCommand() function cannot write to the socket, " +
            " because the socket is not connected.");
        }
    }
}
