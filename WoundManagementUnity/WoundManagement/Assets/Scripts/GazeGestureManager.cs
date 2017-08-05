using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VR.WSA.Input;

public class GazeGestureManager : MonoBehaviour {

    public static GazeGestureManager Instance { get; private set; }
    public GameObject FocusedObject { get; private set; }
    public GameObject TextViewPrefab;
    public AudioClip captureAudioClip;
    public AudioClip failedAudioClip;
	
	string qrString;
    PhotoInput photoInput;
    QrDecoder qrDecoder;
    AudioSource captureAudioSource;
    AudioSource failedAudioSource;

    void Awake () {
        Instance = this;
        photoInput = GetComponent<PhotoInput>();
        qrDecoder = gameObject.AddComponent<QrDecoder>();
	}

    void Start() {
        captureAudioSource = gameObject.AddComponent<AudioSource>();
        captureAudioSource.clip = captureAudioClip;
        captureAudioSource.playOnAwake = false;
        failedAudioSource = gameObject.AddComponent<AudioSource>();
        failedAudioSource.clip = failedAudioClip;
        failedAudioSource.playOnAwake = false;
    }

    private void Update() {
    }

	public void onScanEvent(){
		photoInput.CapturePhotoAsync(onPhotoCaptured);
	}

    void onPhotoCaptured(List<byte> image, int width, int height) {
        string val = qrDecoder.Decode(image.ToArray(), width, height);
        Debug.Log("photo captured value:" +val);
		qrString = val;
        if (val != null) {
            showText(val);
            captureAudioSource.Play();
        } else {
            failedAudioSource.Play();
        }
    }

    void showText(string text) {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo)) {
            var obj = Instantiate(TextViewPrefab, hitInfo.point, Quaternion.identity);
            var textMesh = obj.GetComponent<TextMesh>();
			textMesh.text = "ID:"+text.Substring(0, 5)+"-";
        }
    }
	public string GetQRString(){
		return qrString;
	}
}
