﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;

public class WebStream : MonoBehaviour {
    // Opentopia.com Webcam IP Adress
    // private const string sourceURL = "http://194.103.218.15/mjpg/video.mjpg";
    // UW Seattle Webcam IP Address
    //private const string sourceURL = "http://128.208.252.2/mjpg/video.mjpg";
    // RC Car IP Address
    public bool left;
    private string sourceURL;
    private Texture2D texture;
    private MeshRenderer frame;
    private Stream stream;
   
    /*
     * The Start method, called when the object is instantiated, initializes
     * variables for the script, assigns the appropriate sorting layer to 
     * the MeshRenderer for the object, and calls the GetStream method.
     */ 
    void Start()
    {
        if(left)
            sourceURL = "http://194.103.218.15/mjpg/video.mjpg";
        else
            sourceURL = "http://194.103.218.15/mjpg/video.mjpg";
        frame = this.GetComponent<MeshRenderer>();
        texture = new Texture2D(2, 2);
        GetStream();
    }

    /*
     * The GetStream method creates and sends an HTTP GET request
     * to the sourceURL, receives the response from the address, 
     * and starts the FillFrame coroutine.
     */
    void GetStream() {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sourceURL);
        WebResponse response = request.GetResponse();
        //  - For Debug -
        /*
        if(left)
            print("Left: Received response with Content Length: " + response.ContentLength);
        else
            print("Received response with Content Length: " + response.ContentLength);
        */
        // - End Debug -
        stream = response.GetResponseStream();
        // - For Debug -
        /* 
        print("Stream created with Content Length: " + response.ContentLength);
        */
        // -End Debug -
        StartCoroutine(FillFrame());
    }

    /*
     * The FillFrame method uses a MemoryStream created from reading
     * the bytes in the stream to load the image into a texture and 
     * attach it to the MeshRenderer. The StreamLength() method is 
     * called to determine the number of bytes to read.
     * @return IEnumerator The IEnumerator that determines How long 
     * the coroutine will yield. In most cases the coroutine will
     * start again on the next update.
     */
    IEnumerator FillFrame() {
        // - For Debug -
        // print("Starting Coroutine");
        // - End Debug -
        Byte[] imageData = new Byte[150000];
        while(true) {
            int totalBytes = StreamLength(stream);
            // - For Debug -
            /*
            if (left)
                print("Left: Stream Length: " + totalBytes);
            else
                print("Stream Length: " + totalBytes);
            */
            // - End Debug -
            if (totalBytes == -1)
            {
                // - For Debug -
                // print("Yielding Coroutine, because there are no bytes to read from the stream");
                // - End Debug -
                yield break;
            }
            int remainingBytes = totalBytes;
            while(remainingBytes > 0)
            {
                // - For Debug -
                /*
                if (left)
                    print("Left: In Read Loop ... Remaining: " + remainingBytes);
                else
                    print("In Read Loop ... Remaining: " + remainingBytes);
                */
                // - End Debug -
                remainingBytes -= stream.Read(imageData, totalBytes - remainingBytes, remainingBytes);
                // - For Debug -
                // print("Yielding for one update ..");
                // - End Debug -
                yield return null;
            }
            // - For Debug -
            /*
            if(left)
                print("Left: Loading Image and Exiting Coroutine");
            else
                print("Loading Image and Exiting Coroutine");
            */
            // - End Debug -
            MemoryStream memStream = new MemoryStream(imageData, 0, totalBytes, false, true);
            texture.LoadImage(memStream.GetBuffer());
            frame.material.mainTexture = texture;
            stream.ReadByte();
            stream.ReadByte();
        }
    }

    /*
     * The StreamLength method returns the total number of bytes in
     * the stream excluding header and metadata information. 
     */
    int StreamLength(Stream s) {
        int b;
        string line = "";
        int result = -1;
        bool atEOL = false;
        while ((b = stream.ReadByte()) != -1)
        {
            if (b == 10) continue; 
            if (b == 13)
            { 
                if (atEOL)
                {
                    stream.ReadByte(); 
                    return result;
                }
                if (line.StartsWith("Content-Length:"))
                {
                    result = Convert.ToInt32(line.Substring("Content-Length:".Length).Trim());
                    line = "";
                }
                else
                {
                    line = "";
                }
                atEOL = true;
            }
            else
            {
                atEOL = false;
                line += (char)b;
            }
        }
        return -1;
    }
}