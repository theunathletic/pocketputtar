using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class golf_roundComplete_scoreboardScroll : MonoBehaviour {

	public ScrollRect scroll;

	public GameObject scoreboardPrefab;
	public Transform scoreBoardParent;

	public Text scoreboardTotal;
	public Text scoreboardParTotal; //wont be used anymore
	public Image scoreboardTotal_bg;

	public Mask buttonsMask;
	public GameObject fixedButtons;

	private float toggleFixedButtonsY = 300f;
	public bool showingFixedButtons = false;




	// Use this for initialization
	void Start () {
		ResetScroller ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void ScrollMovement(Vector2 v2){
		CheckFixedButtonToggle (scroll.content.anchoredPosition.y);

	}

	void CheckFixedButtonToggle(float yPos){
		if (!showingFixedButtons && (yPos > toggleFixedButtonsY)) {
			showingFixedButtons = true;
			buttonsMask.enabled = true;
			fixedButtons.SetActive(true);
		} 
		else if(showingFixedButtons && (yPos < toggleFixedButtonsY) ){
			showingFixedButtons = false;
			buttonsMask.enabled = false;
			fixedButtons.SetActive(false);

		}
	}

	void ResetScroller(){
		//maybe reset scroll position here.
		//scroll.verticalNormalizedPosition = 1f;
		showingFixedButtons = false;
		buttonsMask.enabled = false;
		fixedButtons.SetActive(false);
	}

	public void AddCourseScore(int hitCounter, int courseNumber){

		GameObject temp = (GameObject)Instantiate (scoreboardPrefab, scoreBoardParent);
		temp.transform.localScale = Vector3.one;
		temp.transform.localRotation = Quaternion.identity; 
		temp.transform.localPosition = new Vector3 (temp.transform.localPosition.x, temp.transform.localPosition.y, 0);

		golf_roundComplete_scoreboardObject CS = temp.GetComponent<golf_roundComplete_scoreboardObject> ();
		golf_courseObject CO = golf_courseManager.instance.courses[courseNumber-1];

		CS.Set (courseNumber, CO.name, CO.par, FormatParScore(CO.par,hitCounter), CO.levelPreview);
	}

	public void SetUI_TotalCounter(int count){
		scoreboardTotal.text = "" + count;
		SetTotalScoreColor (count);
	}

	void SetTotalScoreColor(int score){
		if (score < 1) {
			scoreboardTotal_bg.color = fiero_uiManager.instance.color_green;
		} else {
			scoreboardTotal_bg.color = fiero_uiManager.instance.color_red;
		}
	}

	public void SetUI_TotalParCounter(int count){
		scoreboardParTotal.text = "/" + count;
	}

	public void Clear(){
		SetUI_TotalCounter (0);
		SetUI_TotalParCounter (0);

		foreach (Transform child in scoreBoardParent) {
			GameObject.Destroy(child.gameObject);
		}
	}

	int FormatParScore(int par, int hits){
		return hits - par;
	}
}
