using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	The RCScene class is the scene manager for the RC_PC_Demo_3 scene 
*/
public class RCScene : MonoBehaviour {
	private ASL.Manipulation.Objects.ObjectInteractionManager objManager;
	private bool playerIsCarView;
	private bool playerOwnsCar;
	private bool objsInstantiated;
	private RCBehavior_TCP car;
	private GameObject player;
	//private PlayerController playerControl;

	// NOTE: Change to the Awake() method instead of using a bool and polling in update()
	// Use this for initialization
	void Start () {
		print("In RCScene.Start() initializing class fields");
		objsInstantiated = false;
		playerIsCarView = playerOwnsCar = false;
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
					if(player == null)
					{
						print("Player Avatar null at time of initialization" +
						" Attempting to assign now.");
						player = GameObject.Find("Player Avatar");
					}
					if(player == null) {
						print("Player still equals null");
					}
					else
					player.GetComponent<PlayerController>().setTransEnabled(false);
				}
            }
        }
        else { 
            if(!playerIsCarView)
            {
				if(Input.GetMouseButtonDown(0)) {
					// For Debug
					print("In RCScene.update() MouseButtonDown = true and playerIsCarView = false");
					// End Debug
                if(car.isCarFirstPerson())
                    {
                        playerIsCarView = true;
                        disablePlayerAvatar();
                    }
                }
            }
        }
	}

	void instantiateSceneObjects() {
		//print("Instantiating a ROBOT!");
		objsInstantiated = true;
		objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ASL.Manipulation.Objects.ObjectInteractionManager>();
		objManager.InstantiateOwnedObject("ASL Player");
		objManager.InstantiateOwnedObject("Robot");
	
		car = GameObject.Find("Robot").GetComponent<RCBehavior_TCP>();
		player = GameObject.Find("Player Avatar");
		//playerControl = GameObject.Find("Player Avatar").GetComponent<PlayerController>();
	}

	/*
		The disablePlayerAvatar method makes the Player Avatar 
		for the ASL Player invisible by disabling the MeshRenderer
		for each of the avatar's children.
	*/
	void disablePlayerAvatar() {
		MeshRenderer tempRend;
		Transform xform = player.GetComponent<Transform>();
		for (int i = 0; i < xform.childCount - 1; i++)
		{
			tempRend = xform.GetChild(i).GetComponent<MeshRenderer>();
			tempRend.enabled = false;
		}
	}
}
