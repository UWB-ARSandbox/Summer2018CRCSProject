using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinuousDemo : MonoBehaviour {

	private IScanner BarcodeScanner;
	public Text TextHeader;
	//public RawImage Image;
	public AudioSource Audio;
    //public GameObject VideoSource;
	private float RestartTime;

	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		// Changes
		BarcodeScanner = new Scanner();
		BarcodeScanner.Camera.Play();

	 	GameObject [] Cam = GameObject.FindGameObjectsWithTag("CarView");
        Debug.Log(Cam.Length);
        BarcodeScanner.setTexture((Texture2D) Cam[0].GetComponent<WebStream>().getTextureFeed());
		// End Changes
	}
	/* 
	void Start () {
		// Create a basic scanner
		BarcodeScanner = new Scanner();
		BarcodeScanner.Camera.Play();
		
        //CRCS Modification - removed image showing on game object as this is not needed
		// Display the camera texture through a RawImage
        /*
		BarcodeScanner.OnReady += (sender, arg) => {
			// Set Orientation & Texture
			Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
			Image.transform.localScale = BarcodeScanner.Camera.GetScale();
            Image.texture = GameObject.Find(QRFinder).GetComponent<WebStream>().getTextureFeed(); //BarcodeScanner.Camera.Texture;
            //Image.texture = BarcodeScanner.Camera.Texture;
            //BarcodeScanner.Camera.Texture = Image.texture;

            // Keep Image Aspect Ratio
            var rect = Image.GetComponent<RectTransform>();
			var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

			RestartTime = Time.realtimeSinceStartup;
		}; */
	/* 		
        //CRCS Moodification - changed scanner to look for QR code in RC cam feed instead of webcam
	    GameObject [] Cam = GameObject.FindGameObjectsWithTag("CarView");
        Debug.Log(Cam.Length);
        BarcodeScanner.setTexture((Texture2D) Cam[0].GetComponent<WebStream>().getTextureFeed());
	}
	*/

	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner()
	{
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();
			if (TextHeader.text.Length > 250)
			{
				TextHeader.text = "";
			}
            TextHeader.text += "Found: QR Code \n";
			RestartTime += Time.realtimeSinceStartup + 1f;
			
            //CRCS comment - add statements here to do something given what barCodeType and barCodeValue are. 

			// Feedback
			Audio.Play();

			#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
			#endif
		});
	}

	/// <summary>
	/// The Update method from unity need to be propagated
	/// </summary>
	void Update()
	{
		if (BarcodeScanner != null)
		{
			BarcodeScanner.Update();
		}

		// Check if the Scanner need to be started or restarted
		if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
		{
			StartScanner();
			RestartTime = 0;
		}
	}

	#region UI Buttons

	public void ClickBack()
	{
		// Try to stop the camera before loading another scene
		StartCoroutine(StopCamera(() => {
			SceneManager.LoadScene("Boot");
		}));
	}

	/// <summary>
	/// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
	/// Trying to stop the camera in OnDestroy provoke random crash on Android
	/// </summary>
	/// <param name="callback"></param>
	/// <returns></returns>
	public IEnumerator StopCamera(Action callback)
	{
		// Stop Scanning
		//Image = null;
		BarcodeScanner.Destroy();
		BarcodeScanner = null;

		// Wait a bit
		yield return new WaitForSeconds(0.1f);

		callback.Invoke();
	}

	#endregion
}
