using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class RCBehavior_TCP : MonoBehaviour {
    public GameObject leftCamera; 
    public GameObject rightCamera;
    public GameObject QRReader;
    
    private const string rcAddress = "172.24.1.1";
    //private const string rcAddress = "127.0.0.1";
    private const short rcCPort = 1030;
    private const short MAX_MESSAGE_LENGTH = 16;
    private const float LERP_SLICE = 5f;
    private bool headingDirty;
    private bool distanceDirty;
    private bool isMoving;
    private bool connected;
    private bool connectionClosed;
    private bool isOwned;
    private bool isFirstPerson;
    private MeshRenderer carRend;
    private Transform xform;
    private TcpClient client;
    private IPEndPoint ep;
    private NetworkStream sock;
    private Byte[] rBuff;
    private Byte[] sBuff;
    private float lastHeading;
    private float headingOffset;
    private short lastCommand;


	/*
     * The Start method initializes variables used by the script, 
     * creates a new  TcpClient and connects the client to the host.
     */
	void Start () {
        isOwned = false;
        isFirstPerson = false;
        connected = false;
        connectionClosed = false;
        headingDirty = false;
        distanceDirty = false;
        isMoving = false;
        sBuff = new Byte[MAX_MESSAGE_LENGTH];
        rBuff = new Byte[MAX_MESSAGE_LENGTH];
        xform = this.GetComponent<Transform>();
        headingOffset = xform.rotation.eulerAngles.y;
        connectRemote();
    }
	
    void connectRemote() {
        client = new TcpClient();
        //ep = new IPEndPoint(IPAddress.Parse(rcAddress), rcCPort);
        ep = new IPEndPoint(IPAddress.Loopback, rcCPort);
        client.Connect(ep);
        sock = client.GetStream();
        connected = true;
        print("TcpClient is connected to the end point with address: " + 
        rcAddress + " and port: " + rcCPort);
    }
	/*
     * The update method checks if a key on the keyboard was pressed
     * during the current frame and sends the appropriate command to 
     * the remote control car if a control key was pressed.
     */
	void Update () {
        if(connected) {
            if(isOwned) {
                if (headingDirty) {
                    //print("Calling updateHeading()");
                    updateHeading();
                } 
                if(distanceDirty) {
                    //print("Calling updateDistance()");
                    updateDistance();
                }
                updateCommand();
            }
        }
        else {
            if(!connectionClosed) {
                connectionClosed = true;
                print("Closing the TCP connection to the remote control car");
                sock.Close();
                client.Close();
            }
        }
    }

    /*
        The updateHeading function attempts to retrieve a new heading
        using getData(), apply a new heading using setNewHeading(),
        and updates the headingDirty field.
        @return bool Returns true if a new heading is read and set
    */
    bool updateHeading() {
        float heading = getData();
        if(heading != -1) {
            setNewHeading(heading);
            headingDirty = false;      
        }
        else {
            print("A new heading could not be read from the stream");
            return false;
        }
        return true;
    }

    /*
        The getData function attempts to read and return a numeric
        value from sock, the NetworkStream associated with the current
        TcpClient. The function returns -1 if a value cannot be read from
        the stream. The function assumes that the value will be 
        preceded by the length of the value and the character 'l'.
        @return float The new value read from the stream
     */
    float getData() {
        int headLen = 0;
        string tempS = "";
        int temp = 0;
    
        while(temp != -1 && (char)temp != 'l') {
            temp = sock.ReadByte();
            if((char)temp != 'l')
                tempS += (char)temp;
        }
        headLen = Int32.Parse(tempS);
        int bRead = 0;
        if(sock.CanRead) {
            while(bRead < headLen) {
                bRead += sock.Read(rBuff, bRead, headLen);
            }
            return Single.Parse(Encoding.UTF8.GetString(rBuff, 0, bRead));
        }
        else
            return -1;
    }
    /*
        The setNewHeading method uses interpolation to rotate the 
        game object to the given target heading.
        @param target The new rotation for the game object to interpolate toward
     */
    void setNewHeading(float target) {
        //print("Setting New Heading: " + target + " with offset: " + headingOffset 
        //+ " Result: " + (target + headingOffset));
        Quaternion rQ = Quaternion.Euler(0, (target + headingOffset), 0);
        //xform.rotation = Quaternion.Slerp(xform.rotation, rQ, Time.deltaTime);
        xform.rotation = rQ;
        lastHeading = target;
    }

    /*
        The updateDistance function attempts to retrieve a new distance
        using getDistance(), apply a new distance using setTranslation(),
        and updates the distanceDirty field.
        @return bool Returns true if a new distance is read and set
    */
    bool updateDistance() {
        float dist = getData();
        // For Debug
        //print("Retrieved Distance: " + dist);
        // End Debug
        if(dist != -1) {
            setTranslation(dist);
            distanceDirty = false;
        }
        else {
            print("A new distance could not be read from the stream");
            return false;
        }
        return true;
    }

    /*
        The setTranslation function applies the given float
        to the transform, xform, of the game object. 
        @param trans The translation to be applied to the 
        current game object
    */
    void setTranslation(float trans) {
        // Note: This functionality is placed in its own function,
        //       because interpolation may be implemented in the future
        xform.Translate(Vector3.right * trans); 
    }
    /*
        The updateCommand function is called once per update to check
        for user input commands to control the car. If a command is given, 
        then sendCommand is called and dirty flags are updated appropriately
    */
    void updateCommand() {
        //Byte[] sBuff;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //Debug.Log("Going Foward");
            sBuff = Encoding.Default.GetBytes("F");
            lastCommand = 1;
            headingDirty = true;
            distanceDirty = true;
            sendCommand();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //Debug.Log("Going Backward");
            sBuff = Encoding.Default.GetBytes("B");
            lastCommand = 2;
            headingDirty = true;
            distanceDirty = true;
            sendCommand();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //Debug.Log("Turning Left");
            sBuff = Encoding.Default.GetBytes("L");
            lastCommand = 3;
            headingDirty = true;
            sendCommand();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //Debug.Log("Turning Right");
            sBuff = Encoding.Default.GetBytes("R");
            lastCommand = 4;
            headingDirty = true;
            sendCommand();
        }
        else if(Input.GetKey(KeyCode.E))
        {
            //Debug.Log("Exiting");
            sBuff = Encoding.Default.GetBytes("E");
            sendCommand();
            connected = false;
            headingDirty = false;
        }
        else
        {
            if(lastCommand > 0)
            {
                sBuff = Encoding.ASCII.GetBytes("S");
                lastCommand = 0;
                headingDirty = false;
                distanceDirty = false;
                sendCommand();
            }
        }    
    }

    /*
     * The sendCommand function writes the current contents of sBuff to
     * the socket when connected is true. If connected is false, then
     * no action is taken.
     */
    void sendCommand() {
        if(connected) {
            if(sock.CanWrite) {
                sock.Write(sBuff, 0, sBuff.Length);
                //print("Command Sent");
            }
        }
        else {
            print("The sendCommand() function cannot write to the socket, " + 
            " because the socket is not connected.");
        }   
    }

    /*
     * The overidden OnMouseDown method is called when a mouse down event
     * is triggered within the collider of the gameobject that owns the script.
     * The method disables the MeshRenderer for all children of the game object
     * and instantiates one instance of each camera feed prefab 
     * (leftCamera and rightCamera).
     */
    void OnMouseDown() {
        // print("In OnMouseDown()");
        if(!isOwned) {
            isOwned = true;
            // For Debug
            print("In RCBehavior_TCP.OnMouseDown() isOwned = true. Car is now owned!");
            // End Deubg
        }
        else {
            if(!isFirstPerson) {
                isFirstPerson = true;
                // For Debug
                print("In RCBehavior_TCP.OnMouseDown() isOwned = true " +
                " isFirstPerson = true. Instantiating cameras, becasue car is now first person!");
                // End Debug
                Instantiate(leftCamera);
                Instantiate(rightCamera);
                Instantiate(QRReader);
            }        
        }
    }

    /*
        The isCarOwned method returns true if the isOwned field is true.
        @return bool Returns false if the isOwned field is false.
    */
    public bool isCarOwned() {
        return isOwned;
    }

    /*
        The isCarFirstPerson method returns true if the isFirstPerson field
        is true.
        @return bool Returns false if the isFirstPerson field is false.
    */
    public bool isCarFirstPerson() {
        return isFirstPerson;
    }

}

/*
    --------------  OLD CODE  ----------------
    // Disables MeshRenderer to Make Car Invisible in First Person 
    MeshRenderer tempRend;
    for (int i = 0; i < xform.childCount - 1; i++)
    {
        tempRend = xform.GetChild(i).GetComponent<MeshRenderer>();
        tempRend.enabled = false;
    }
*/