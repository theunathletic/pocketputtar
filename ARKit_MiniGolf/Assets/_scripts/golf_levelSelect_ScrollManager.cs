using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class golf_levelSelect_ScrollManager : MonoBehaviour {
	public static golf_levelSelect_ScrollManager instance;

	public ScrollManager _scrollManager;

	void Awake(){
		instance = this;
		_scrollManager = gameObject.GetComponent<ScrollManager>();

	}

	void Start(){
	}

	public void PopulateScroll( List<golf_courseObject> courses){

		int i = 1;
		foreach (golf_courseObject course in courses) {
			//fiero_uiManager.instance.LevelSelect_AddLevelButton (i, course.name, course.par);
			LevelSelect_AddLevelButton(i,course.name,course.par, course.levelPreview);
			i++;
//			print ("PopulateScroll" + i);

			//USE THIS FOR GRABBING THE COURSE IMAGES
			//course.levelPreview
		}

		_scrollManager.SetupBuffers ();
		_scrollManager._scrollRectSnap.SetSnapPoints (courses.Count);
		_scrollManager._scrollRectSnap.ForceScroll (1);
	}

	public void LevelSelect_AddLevelButton(int number, string name, int par, Sprite levelPreview){
		// Create Buttons
		GameObject tempButtonObject = (GameObject)Instantiate(_scrollManager._scrollObjectPrefab);
		Button tempButtonButton = tempButtonObject.GetComponent<Button> ();
		golf_courseButton tempButtonScript = tempButtonObject.GetComponent<golf_courseButton> ();

		// Set button text
		tempButtonObject.transform.SetParent (_scrollManager._contentParent);
		tempButtonScript.Set (number, name);
		tempButtonScript.SetPar (par);
		tempButtonScript.SetLevelPreview (levelPreview);
		//tempButtonButton.onClick.AddListener(() => tempButtonScript.Select());

		tempButtonObject.transform.localScale = new Vector3 (1f, 1f, 1f);
		tempButtonObject.transform.localPosition = new Vector3 (tempButtonObject.transform.localPosition.x, tempButtonObject.transform.localPosition.y, 0);

	}

	public void SelectCurrentLevel(){
		fiero_uiManager.instance.LevelSelect_PickLevel (_scrollManager._scrollRectSnap.currentScreen);
	}

}
