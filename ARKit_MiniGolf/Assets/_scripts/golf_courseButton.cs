using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class golf_courseButton : MonoBehaviour {

	private int number;

	public Text text_number;
	public Text text_title;
	public Text text_par;

	//public Transform levelPreviewParent;
	public Image image_levelPreview;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Set(int num, string title){
		number = num;
		text_number.text = "" + num + ".";
		text_title.text = title;
	}

	public void SetPar(int par){
		text_par.text = "" + par;
	}

	public void SetLevelPreview(Sprite levelSprite){
		image_levelPreview.sprite = levelSprite;

		//Instantiate(
	}

	public void Select(){
		fiero_uiManager.instance.LevelSelect_PickLevel (number);
	}
}

