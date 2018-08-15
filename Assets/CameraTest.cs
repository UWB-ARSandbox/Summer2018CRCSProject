using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour {
	public GameObject leftCam;
	public GameObject rightCam;
	private const float CAM_Z = 5.22f;
	private const float CAM_H = 6f;
	private const float CAM_ASPECT = 4/3f;
	// Use this for initialization
	void Start () {
		Instantiate(leftCam);
		Instantiate(rightCam);
		GameObject left = GameObject.Find("LeftCam(Clone)");
		GameObject right = GameObject.Find("RightCam(Clone)");
		Camera cam = Camera.main;
		if(cam == null) {
			print("MainCamera could not be located");
		}
		else {
			print("MainCamera.position: " + cam.transform.position);
			if(left != null) {
				float width = CAM_ASPECT * CAM_H;
				print("Setting LeftCam Scale w/ Width: " + width + " Height: " + CAM_H + " Length: 1");
				left.transform.localScale = new Vector3(width, CAM_H, 1f);
				left.transform.position = new Vector3(cam.transform.position.x - (0.5f * width), cam.transform.position.y, CAM_Z);
				print("LeftCam New Position: " + left.transform.position);
			}
			else
				print("Clone of LeftCam could not be found in the scene");
			if(right != null) {
				float width = CAM_ASPECT * CAM_H;
				print("Setting RightCam Scale w/ Width: " + width + " Height: " + CAM_H + " Length: 1");
				right.transform.localScale = new Vector3(width, CAM_H, 1f);
				right.transform.position = new Vector3(cam.transform.position.x + (0.5f * width), cam.transform.position.y, CAM_Z);
				print("RightCam New Position: " + right.transform.position);
			}
			else
				print("Clone of RightCam could not be found in the scene");
			}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
