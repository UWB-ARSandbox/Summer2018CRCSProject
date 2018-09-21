using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackpadComs : MonoBehaviour {

    public double x;
    public double y;
    public bool Grip_Pressed;

    // Use this for initialization
    void Start () {
        x = 0;
        y = 0;
        Grip_Pressed = false;
}
	
	// Update is called once per frame
	void Update () {
		
	}
}
