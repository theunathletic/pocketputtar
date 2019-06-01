using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class tickertape_ui : MonoBehaviour {

	private float maskWidth;
	private float contentWidth = 0;

	private bool contentLoopCalculated = false;
	private float contentLoopWidth;

	public Transform contentParent;
	public GameObject tickertapeTextObject;

	public float scrollSpeed = 1f;

	public string[] tickerContent;
	public Color contentColor;

	void Awake(){
		contentParent = this.transform.Find ("Content");

	}

	// Use this for initialization
	void Start () {
		maskWidth = this.GetComponent<RectTransform> ().sizeDelta.x;
		PopulateTickertape ();
	}
	
	// Update is called once per frame
	void Update () {
			
			Scroll ();

	}

	public void SetTickertapeStrings( string[] newTickerContent){
		tickerContent = newTickerContent;
			
			ClearTickertape ();
			ResetScroll ();
			PopulateTickertape ();

	}

	void PopulateTickertape(){
		contentLoopCalculated = false;
		int currentObjectToAdd = 0;
		while (contentWidth < (maskWidth+contentLoopWidth) ) {
			//print ("WHILE");

			if (currentObjectToAdd >= tickerContent.Length) {
				contentLoopCalculated = true;
				currentObjectToAdd = 0;
			}

			AddTickerTapeObject (currentObjectToAdd);
			currentObjectToAdd++;
		}
	}

	void AddTickerTapeObject(int objectType){
		GameObject tempTickerTapeObject = Instantiate (tickertapeTextObject);
		tempTickerTapeObject.GetComponent<Text> ().text = tickerContent [objectType];
		tempTickerTapeObject.GetComponent<Text> ().color = contentColor;

		tempTickerTapeObject.transform.SetParent (contentParent);
		tempTickerTapeObject.transform.localScale = Vector3.one;

		Vector3 tempPos = tempTickerTapeObject.transform.localPosition;
		tempPos.z = 0f;

		tempTickerTapeObject.transform.localPosition = tempPos;


		Canvas.ForceUpdateCanvases();
//		print("width " +tempTickerTapeObject.GetComponent<RectTransform> ().sizeDelta.x);


		contentWidth += tempTickerTapeObject.GetComponent<RectTransform> ().sizeDelta.x;

		if (!contentLoopCalculated) {
		//	print ("contentLoopWidth " + contentWidth);
			contentLoopWidth = contentWidth;
		}
	}

	void ClearTickertape(){
			if (contentParent.childCount > 0) {
				foreach (Transform child in contentParent) {
					Destroy (child.gameObject);
				}
			}

		contentWidth = 0f;
			
	}

	void Scroll(){
		Vector3 newPos = contentParent.GetComponent<RectTransform> ().anchoredPosition3D;

		newPos.x -= scrollSpeed;

		if (newPos.x < -contentLoopWidth) {
			newPos.x = 0f;
		}
			
		contentParent.GetComponent<RectTransform> ().anchoredPosition3D = newPos;
	}

	void ResetScroll(){
			
			Vector3 newPos = contentParent.GetComponent<RectTransform> ().anchoredPosition3D;
			newPos.x = 0f;
			contentParent.GetComponent<RectTransform> ().anchoredPosition3D = newPos;

	}
}
