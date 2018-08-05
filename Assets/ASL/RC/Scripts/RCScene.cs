using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	The RCScene class is the scene manager for the RC_PC_Demo_3 scene 
*/
public class RCScene : MonoBehaviour {
	private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
	private bool punConnected;
	private bool objsInstantiated;
	// Use this for initialization
	void Start () {
		print("In RCScene.Start()");
		punConnected = objsInstantiated = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!objsInstantiated) {
			if(PhotonNetwork.connectedAndReady) {
				objsInstantiated = true;
				instantiateRobot();
			}
		}
	}

	void instantiateRobot() {
		print("Instantiating a ROBOT!");
		objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ASL.Manipulation.Objects.ObjectInteractionManager>();
		objManager.InstantiateOwnedObject("Robot");
	}
}
