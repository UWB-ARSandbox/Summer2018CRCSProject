using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInitializer : MonoBehaviour {

	void Start ()
    {
        transform.GetComponent<MeshRenderer>().sharedMaterial = GameObject.Find("ASL Player").GetComponent<ControllerInstantiation>().avatarMaterial;
    }
	
}
