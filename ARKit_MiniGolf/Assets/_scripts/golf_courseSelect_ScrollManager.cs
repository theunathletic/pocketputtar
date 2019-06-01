using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class golf_courseSelect_ScrollManager : MonoBehaviour {
	public static golf_courseSelect_ScrollManager instance;

	public ScrollManager _scrollManager;

	void Awake(){
		instance = this;
		_scrollManager = gameObject.GetComponent<ScrollManager>();
       
    }

	void Start(){
        _scrollManager._scrollRectSnap.finishScrollSnap = CheckIfCourseIsUnlocked;

    }

    public void PopulateScroll( List<golf_parentCourseObject> courses){

		int i = 1;
		foreach (golf_parentCourseObject course in courses) {
			//fiero_uiManager.instance.LevelSelect_AddLevelButton (i, course.name, course.par);
			LevelSelect_AddLevelButton(i,course.name,course.par, course.levelPreview_front,course.levelPreview_back, course.levelPreview_animator);
			i++;
//			print ("PopulateScroll" + i);

			//USE THIS FOR GRABBING THE COURSE IMAGES
			//course.levelPreview
		}

		_scrollManager.SetupBuffers ();
		_scrollManager._scrollRectSnap.SetSnapPoints (courses.Count);
		_scrollManager._scrollRectSnap.ForceScroll (1);
	}

	public void LevelSelect_AddLevelButton(int number, string name, int par, Sprite levelPreview_front, Sprite levelPreview_back, RuntimeAnimatorController animator)
    {
		// Create Buttons
		GameObject tempButtonObject = (GameObject)Instantiate(_scrollManager._scrollObjectPrefab);
		Button tempButtonButton = tempButtonObject.GetComponent<Button> ();
        golf_parentCourseButton tempButtonScript = tempButtonObject.GetComponent<golf_parentCourseButton> ();

		// Set button text
		tempButtonObject.transform.SetParent (_scrollManager._contentParent);
		tempButtonScript.Set (number, name);
		tempButtonScript.SetPar (par);
		tempButtonScript.SetLevelPreview (levelPreview_front, levelPreview_back, animator);

		tempButtonObject.transform.localScale = new Vector3 (1f, 1f, 1f);
		tempButtonObject.transform.localPosition = new Vector3 (tempButtonObject.transform.localPosition.x, tempButtonObject.transform.localPosition.y, 0);

	}
    /*
	public void SelectCurrentLevel(){
		fiero_uiManager.instance.LevelSelect_PickLevel (_scrollManager._scrollRectSnap.currentScreen);
	}
    */
    public void CheckIfCourseIsUnlocked(int courseNumber) {
//        print("CheckIfCourseIsUnlocked" + courseNumber);
        if (courseNumber == 2) {
            fiero_uiManager.instance.CourseSelect_ToggleSelectButtonActive(false);
        }
        else {
            fiero_uiManager.instance.CourseSelect_ToggleSelectButtonActive(true);
        }
    }
}