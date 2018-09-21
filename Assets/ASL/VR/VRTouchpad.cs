using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTouchpad : MonoBehaviour
{

    public TrackpadComs obj;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;
    private bool RCActive;
    public double x;
    public double y;

	// Use this for initialization
	void Start ()
    {
        RCActive = false;
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        x = 0;
        y = 0;
	}

    public void activate ()
    {
        RCActive = true;
    }
	
	// Update is called once per frame
	void Update ()
    { 
        device = SteamVR_Controller.Input((int) trackedObject.index);
        x = device.GetAxis().x;
        y = device.GetAxis().y;
        obj.x = x;
        obj.y = y;

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {            
            Debug.Log("Grip Pressed");
            obj.Grip_Pressed = true;
        }

        if (false)
        {
            //if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            Debug.Log(device.GetAxis().x + " " + device.GetAxis().y);
            
           
        }
        
	}   
}
