using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.PortalSystem;

/*
	The RCScene class is the scene manager for the RC_PC_Demo_3 scene 
*/
public class RCScene : MonoBehaviour {
	private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
	private bool playerOwnsCar;
	private bool objsInstantiated;
	private Vector3 firstPersonCam;
	private RCBehavior_TCP car;
	private GameObject player;
	
	/*
		The Awake method is called for the RCScene after all Start
		methods have completed to instantiate game objects for the
		scene across the Photon Unity Network.
	*/
	void Awake() {
		objsInstantiated = false;
		playerOwnsCar = false;
		firstPersonCam = new Vector3(0, 0, 40);
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
		player = objManager.InstantiateOwnedObject("Player Avatar");
		car = objManager.InstantiateOwnedObject("BlueCar").GetComponent<RCBehavior_TCP>();
		Camera mainCam = Camera.main;
		mainCam.transform.position = firstPersonCam;
		
		if(player != null) {
			player.tag = "Local Primary Camera";
			GameObject.Find("PortalManager").GetComponent<PortalManager>().player = player;
			ASLLocalEventManager.Instance.Trigger(player.GetComponent<Camera>(), ASLLocalEventManager.LocalEvents.PlayerInstanceActive);
		}
		else 
			print("Error: RCScene.instantiateSceneObjects() Line 59. Unable to instantiate 'Player Avatar'");
		if(car == null)
			print("Error: RCScene.instantiateSceneObjects() Line 60. Unable to instantiate 'BlueCar'");
		//else
			//car.GetComponent<RCBehavior_TCP>().enabled = true;
	}

	/*
		The playerCarTransition method assigns the current position
		of the ASL Player to the main camera, destroys the player 
		across PUN using the ASL object interaction manager, and 
		activates the main camera.
	*/
	void playerCarTransition() {
		GameObject cam = GameObject.Find("Main Camera");
		if(cam != null) {
			cam.transform.position = firstPersonCam;
			cam.SetActive(true);
			objManager.Destroy(player);
		}
		else 
			print("Error: Game Object: Main Camera could not be found in the scene. RCScene Line 92");
		
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
