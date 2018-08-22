using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/*
    Instances of the RCBehavior_TCP class establish a TCP connection
    to the RC car, listen for control commands and pass them to the car,
    receive yaw and translation data and update the virtual car, and
    respond to mouse clicks to take control of the car.
*/
public class RCBehavior_TCP : MonoBehaviour {
    public GameObject leftCamera; 
    public GameObject rightCamera;
    public GameObject QRReader;

    private const float CAM_H = 6f;  
    private const float CAM_Z = 30.5f; 
    private const float CAM_ASPECT = 1.33f; 
    private const string rcAddress = "172.24.1.1";
    //private const string rcAddress = "127.0.0.1";
    private const short rcCPort = 1070;
    private const short MAX_MESSAGE_LENGTH = 16;
    private const float LERP_SLICE = 5f;
    private bool headingDirty;
    private bool distanceDirty;
    private bool isMoving;
    private bool connected;
    private bool connectionClosed;
    private bool isOwned;
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
    void Start() {
        print("In RCBehavior.Awake()");
        isOwned = false;
        connected = false;
        connectionClosed = true;
        headingDirty = false;
        distanceDirty = false;
        isMoving = false;
        sBuff = new Byte[MAX_MESSAGE_LENGTH];
        rBuff = new Byte[MAX_MESSAGE_LENGTH];
        xform = this.GetComponent<Transform>();
        headingOffset = xform.rotation.eulerAngles.y;
        connectRemote();
    }
    
    /*
        The connectRemote method establishes a TCP connection
        to rcAddress and rcPort and updates connection flags
    */
    void connectRemote() {
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
	
    /*
     * The update method checks if keyboard input corresponding to a car 
     * control command was given and sends the command to the car. The function
     * also makes calls to updateHeading() and updateDistance() and closes
     * the TCP connection when the connectionClosed flag is set.
     */
	void Update () {
        if(connected) {
            if(isOwned) {
                if (headingDirty) 
                    updateHeading();
                if(distanceDirty) 
                    updateDistance();
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
        Quaternion rQ = Quaternion.Euler(0, (target + headingOffset), 0);
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
        else if(Input.GetKey(KeyCode.E))
        {
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
        if(!isOwned) {
            isOwned = true;
            startCarFirstPerson();
            //startQR();
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
        The startCarFirstPerson method locally instantiates instances of the 
        LeftCam, RightCam, and QRReader prefabs. The method also scales and
        translates the LeftCam and RightCam instances to completely cover the
        main camera. 
    */
    void startCarFirstPerson() {
        Camera cam = Camera.main;
        Instantiate(leftCamera);
        Instantiate(rightCamera);
        GameObject temp = GameObject.Find("LeftCam(Clone)");
        if(temp != null) {
            float camWidth = CAM_ASPECT * CAM_H;
            temp.transform.localScale = new Vector3(camWidth, CAM_H, 1f);
            temp.transform.position = new Vector3(cam.transform.position.x + (0.5f * camWidth), cam.transform.position.y, CAM_Z) ;
        }
        else
            print("Error: Game Object: LeftCam not found in scene. RCBehavior.OnMouseDown() line 306");
        temp = GameObject.Find("RightCam(Clone)");
        if(temp != null) {
            float camWidth = CAM_ASPECT * CAM_H;
            temp.transform.localScale = new Vector3(camWidth, CAM_H, 1);
            temp.transform.position = new Vector3(cam.transform.position.x - (0.5f * camWidth), cam.transform.position.y, CAM_Z) ;
        }
        else
            print("Error: Game Object: RightCam not found in scene. RCBehavior.OnMouseDown() line 311");
    }

    /*
        The startQR method locally instantiates an instance of
        the QRReader prefab to be used by the RC car for scanning
        QR codes and syncing the virtual position to absolute.
        TO DO: The QR Scanning feature has not been fully integrated
            and requires further testing. 
    */
    void startQR() {
        Instantiate(QRReader);
    }
}
