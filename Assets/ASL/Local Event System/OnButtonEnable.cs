using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonEnable : MonoBehaviour {

    public KeyCode key;
    public GameObject[] toEnable;

	
	// Update is called once per frame
	void Update ()
    {
		if (PhotonNetwork.inRoom && Input.GetKeyDown(key))
        {
            activate();
        }
	}

    private void activate()
    {
        foreach(GameObject go in toEnable)
        {
            go.SetActive(true);
        }
    }
}
