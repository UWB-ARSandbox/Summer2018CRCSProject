using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerSyncManager : MonoBehaviour {

    public GameObject followObject;

    void Update ()
    {

        transform.position = followObject.transform.position;
        transform.rotation = followObject.transform.rotation;
		
	}
}
