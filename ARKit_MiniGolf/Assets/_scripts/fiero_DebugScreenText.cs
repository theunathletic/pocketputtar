using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class fiero_DebugScreenText : MonoBehaviour {

	public static fiero_DebugScreenText instance;

	private Text displayText;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
		displayText = GetComponentInChildren<Text> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetDebugText(string t){
		displayText.text = " " + t;
	}
}
