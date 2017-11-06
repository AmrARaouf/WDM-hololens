using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;
using UnityEngine.UI;
using UnityEngine.Networking;
using HoloToolkit.Examples.GazeRuler;

public class PhotoCaptureAndSave : MonoBehaviour {
	PhotoCapture photoCaptureObject = null;
	string filename;

	public string serverUrl = "http://192.168.36.1:3000";
	public AudioClip captureAudioClip;
	public AudioClip failedAudioClip;

	bool isMixedRealityPhoto;
	
	AudioSource captureAudioSource;
	AudioSource failedAudioSource;

	void Start() {
		captureAudioSource = gameObject.AddComponent<AudioSource>();
		captureAudioSource.clip = captureAudioClip;
		captureAudioSource.playOnAwake = false;
		failedAudioSource = gameObject.AddComponent<AudioSource>();
		failedAudioSource.clip = failedAudioClip;
		failedAudioSource.playOnAwake = false;
		isMixedRealityPhoto = false;
	}

	public void StartCapture()
	{
		Debug.Log("ON Start capture. isMixedRealityPhoto = "+isMixedRealityPhoto);
		PhotoCapture.CreateAsync(isMixedRealityPhoto, OnPhotoCaptureCreated);
	}

	void OnPhotoCaptureCreated(PhotoCapture captureObject)
	{
		Debug.Log ("On OnPhotoCaptureCreated");
		photoCaptureObject = captureObject;

		Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

		CameraParameters c = new CameraParameters();
		c.cameraResolutionWidth = cameraResolution.width;
		c.cameraResolutionHeight = cameraResolution.height;
		c.hologramOpacity = isMixedRealityPhoto? 1.0f:0.0f;
		c.pixelFormat = CapturePixelFormat.BGRA32;
		
		GetComponent<ShowHideSight>().HideAll();
		
		captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
	}

	private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
	{
		if (result.success)
		{
			float oneDistance = 0.0f, twoDistance = 0.0f;
			if (LineManager.Lines.Count > 1) {
				Line one = LineManager.Lines.Pop ();
				oneDistance = one.Distance;
				Line two = LineManager.Lines.Peek ();
				twoDistance = two.Distance;
				LineManager.Lines.Push (one);
			}

			filename = string.Format(@"{0}_{1}_{2}_{3}_{4}.jpg", 
				GetComponent<GazeGestureManager>().GetQRString(),
				DateTime.Now.ToString("yy-MM-dd-HH-mm-ss"), 
				oneDistance, //length
				twoDistance, //width
				isMixedRealityPhoto?'M':'N');
			string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

			Debug.Log ("To be saved to: " + filePath + ':' + filename);

			photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
		}
		else
		{
			Debug.LogError("Unable to start photo mode!");
		}
	}

	void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
	{
		if (result.success)
		{
			Debug.Log("Saved Photo to disk!");
			captureAudioSource.Play();
			StartCoroutine (UploadSample());
			photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
		}
		else
		{
			Debug.Log("Failed to save Photo to disk");
			failedAudioSource.Play();
			photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
		}
		GetComponent<ShowHideSight>().SetVisibility();
	}

	void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
		// Shutdown the photo capture resource
		Debug.Log("photostopped mode result.success: " +result.success);
		photoCaptureObject.Dispose();
		photoCaptureObject = null;

		//disabling mixed reality for deploy
		isMixedRealityPhoto = false;

		/*if (isMixedRealityPhoto) {
			isMixedRealityPhoto = false;
			Debug.Log("ON STOPPED PHOTO MODE. isMixedRealityPhoto = true");
			StartCapture();
		} else {
			Debug.Log("ON STOPPED PHOTO MODE. isMixedRealityPhoto = false");
			isMixedRealityPhoto = true;
		}*/
	}

	IEnumerator UploadSample() {
		// Create a Web Form
		WWWForm form = new WWWForm();
		form.AddBinaryData("img", File.ReadAllBytes(System.IO.Path.Combine(Application.persistentDataPath, filename)), filename, "image/jpeg");

		// Upload to a cgi script
		WWW w = new WWW(serverUrl+"/wounds", form);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print("new upload: "+w.error);
		}
		else {
			print("Finished Uploading Screenshot");
		}
	}
}
