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
	private Vector3 clickPosition;
	private Vector3 firstPersonCam;
	private RCBehavior_TCP car;
	//private GameObject player;
	//private PlayerController playerControl;

	// NOTE: Change to the Awake() method instead of using a bool and polling in update()
	// Use this for initialization
	/* 
	void Start () {
		print("In RCScene.Start() initializing class fields");
		objsInstantiated = false;
		playerOwnsCar = false;
		clickPosition = new Vector3(0, 0, 0);
	}
	*/
	/*
		The Awake method is called for the RCScene after all Start
		methods have completed to instantiate game objects for the
		scene across the Photon Unity Network.
	*/
	 
	void Awake() {
		// For Debug
		print("In RCScene.Awake instantiating game objects for the scene across PUN");
		// End Debug
		objsInstantiated = false;
		playerOwnsCar = false;
		clickPosition = new Vector3(0, 0, 0);
		firstPersonCam = new Vector3(0, 0, 40);
		//instantiateSceneObjects();
	}
	
	/*
		The update function for the RCScene class updates members
		and functions for the scene that must be addressed once per frame.
	*/
	void Update () {
		 
		if(!objsInstantiated) {
			if(PhotonNetwork.inRoom) {
				instantiateSceneObjects();
			}
		}
		else {
			if(!playerOwnsCar) {
				if(Input.GetMouseButtonDown(0)) {
					if(car.isCarOwned()) {
						playerOwnsCar = true;
						playerCarTransition();
					}
				}
        	}
		}
		
		
	}

	/*
		The instantiateSceneObjects method instantiates all networked
		objects for the RCScene across PUN using the ASL object interaction
		and network managers. The method instantiates the ASL Player and
		Blue Car prefabs and enables the RCBehavior_TCP component of the 
		Blue Car prefab.
	*/
	void instantiateSceneObjects() {
		objsInstantiated = true;
		objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ASL.Manipulation.Objects.ObjectInteractionManager>();
		objManager.InstantiateOwnedObject("ASL Player");
		car = objManager.InstantiateOwnedObject("BlueCar").GetComponent<RCBehavior_TCP>();
		// For Debug 
		if(car == null)
			print("In RCScene.instantiateSceneObjects() with car == null");
		else
			car.GetComponent<RCBehavior_TCP>().enabled = true;
		
	}

	/*
		The playerCarTransition method assigns the current position
		of the ASL Player to the main camera, destroys the player 
		across PUN using the ASL object interaction manager, and 
		activates the main camera.
	*/
	void playerCarTransition() {
		//GameObject playAvatar = GameObject.Find("Player Avatar");
		GameObject aslPlayer = GameObject.Find("ASL Player");
		GameObject cam = GameObject.Find("Main Camera");
		if(aslPlayer != null && cam != null) {
			// Determine the position of the Player Avatar and assign it to the camera
			cam.transform.position = firstPersonCam;
			// Activate the Main Camera in the hierarchy
			cam.SetActive(true);
			// Destroy the ASL Player
			objManager.Destroy(aslPlayer);
		}
		else {
			if(aslPlayer == null)
				print("Error: Game Object: ASL Player could not be found in the scene. RCScene Line 95-97");
			else
				print("Error: Game Object: Main Camera could not be found in the scene. RCScene Line 95-97");
		}
	}

	public Vector3 getClickPosition() {
		return clickPosition;
	}
}

/*
	-------------------		OLD CODE	 ----------------------------
	// FROM playerCarTransition() 
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
		player.transform.position = clickPosition;
	}
	else {
		print("Error: Game Object: 'Player Avatar' could not be located in the scene." +
		" RCScene.playerCarTransition() line 101");
	}
*/
