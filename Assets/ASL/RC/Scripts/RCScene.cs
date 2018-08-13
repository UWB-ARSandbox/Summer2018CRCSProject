using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	The RCScene class is the scene manager for the RC_PC_Demo_3 scene 
*/
public class RCScene : MonoBehaviour {
	private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
	private bool playerOwnsCar;
	private bool objsInstantiated;
	private RCBehavior_TCP car;
	//private GameObject player;
	//private PlayerController playerControl;

	// NOTE: Change to the Awake() method instead of using a bool and polling in update()
	// Use this for initialization
	void Start () {
		print("In RCScene.Start() initializing class fields");
		objsInstantiated = false;
		playerOwnsCar = false;
	}
	
	/*
		The Awake method is called for the RCScene after all Start
		methods have completed to instantiate game objects for the
		scene across the Photon Unity Network.
	*/
	/* 
	void Awake() {
		// For Debug
		print("In RCScene.Awake instantiating game objects for the scene across PUN");
		// End Debug
		playerIsCarView = playerOwnsCar = false;
		instantiateSceneObjects();
	}
	*/
	/*
		The update function for the RCScene class updates members
		and functions for the scene that must be addressed once per frame.
	*/
	void Update () {
		 
		if(!objsInstantiated) {
			if(PhotonNetwork.connectedAndReady) {
				instantiateSceneObjects();
			}
		}
		
		
		if(!playerOwnsCar) {
            if(Input.GetMouseButtonDown(0))
            {
				// For Debug
				print("In RCScene.update() MouseButtonDown = true and playerOwnsCar = false");
				// End Debug
				if(car.isCarOwned()) {
					playerOwnsCar = true;
					playerCarTransition();
				}
			}
        }
	}

	void instantiateSceneObjects() {
		objsInstantiated = true;
		objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ASL.Manipulation.Objects.ObjectInteractionManager>();
		print("Instantiating a ROBOT!");
		objManager.InstantiateOwnedObject("BlueCar");
		objManager.InstantiateOwnedObject("ASL Player");
		car = GameObject.Find("BlueCar").GetComponent<RCBehavior_TCP>();
		// For Debug 
		if(car == null)
			print("In RCScene.instantiateSceneObjects() with car == null");
		else
			car.enabled = true;
		// End Debug
		//player = GameObject.Find("Player Avatar");
	}

	/*
		The disablePlayerAvatar method makes the Player Avatar 
		for the ASL Player invisible by disabling the MeshRenderer
		for each of the avatar's children.
	*/
	void playerCarTransition() {
		GameObject player = GameObject.Find("Player Avatar");
		if(player != null) {
			player.GetComponent<PlayerController>().setTransEnabled(false);
			player.GetComponent<SmoothMouseLook>().enabled = false;
			
			MeshRenderer tempRend;
			Transform xform = GameObject.Find("Player Avatar").GetComponent<Transform>();
			for (int i = 0; i < xform.childCount - 1; i++)
			{
				tempRend = xform.GetChild(i).GetComponent<MeshRenderer>();
				if(tempRend != null)
					tempRend.enabled = false;
			}
		}
		else {
			print("Error: Game Object: 'Player Avatar' could not be located in the scene." +
			" RCScene.playerCarTransition() line 101");
		}
	}
}
