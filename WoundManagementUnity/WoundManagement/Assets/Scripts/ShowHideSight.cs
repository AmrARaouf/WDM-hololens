using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideSight : MonoBehaviour {
	public GameObject ToolTip;
	public GameObject ShowToolTipText;

	bool StateToolTipShown = true;
	// Use this for initialization
	void Start () {
		SetVisibility();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowToolTip(){
		StateToolTipShown = true;
		SetVisibility();
	}

	public void HideToolTip(){
		StateToolTipShown = false;
		SetVisibility();
	}
	
	public void HideAll(){
		
		Debug.Log("attempting to hide elements");
		ToolTip.SetActive (false);
		ShowToolTipText.SetActive (false);
	}
	
	public void SetVisibility(){
		ToolTip.SetActive (StateToolTipShown);
		ShowToolTipText.SetActive (!StateToolTipShown);
	}
}
