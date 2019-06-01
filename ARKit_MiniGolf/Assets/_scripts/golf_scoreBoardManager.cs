using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class golf_scoreBoardManager : MonoBehaviour {

	public GameObject scoreboardPrefab;
	public Transform scoreBoardParent;

	public Text scoreboardTotal_title;
	public Text scoreboardTotal;
	public Image scoreboardTotal_bg;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	public void SetupNumberOfScores(int number){
//		Debug.Log ("golf_scoreBoardManager: SetupNumberOfScores");
		for (int i = 0; i < number; i++) {
			
			/*
			GameObject temp = (GameObject)Instantiate (scoreboardPrefab, scoreBoardParent);
			temp.transform.localScale = Vector3.one;
			temp.transform.localRotation = Quaternion.identity; 
			temp.transform.localPosition = new Vector3 (temp.transform.localPosition.x, temp.transform.localPosition.y, 0);

			golf_courseScoreboard CS = temp.GetComponent<golf_courseScoreboard> ();
			golf_courseObject CO = golf_courseManager.instance.courses[i];

			CS.Set (i, CO.name, CO.par, 0);
			*/
			AddCourseScore (0, i);
		} 
	}

	public void AddCourseScore(int hitCounter, int courseNumber){
//		Debug.Log ("golf_scoreBoardManager: AddCourseScore:" + courseNumber);
		
		GameObject temp = (GameObject)Instantiate (scoreboardPrefab, scoreBoardParent);
		temp.transform.localScale = Vector3.one;
		temp.transform.localRotation = Quaternion.identity; 
		temp.transform.localPosition = new Vector3 (temp.transform.localPosition.x, temp.transform.localPosition.y, 0);

		golf_courseScoreboard CS = temp.GetComponent<golf_courseScoreboard> ();
		golf_courseObject CO = golf_courseManager.instance.courses[courseNumber];

		CS.Set (courseNumber+1, CO.name, CO.par, FormatParScore(CO.par,hitCounter));
		CS.Set_Empty (courseNumber + 1);




	}
	//New version that works with a scoreboard that already has all the scores added.
	//Something like scoreBoardParent.getChild(courseNumber) etc.
	//Format the scores correctly
	public void UpdateCourseScore(int hitCounter, int courseNumber, bool isFinal){
		golf_courseScoreboard CS = scoreBoardParent.GetChild (courseNumber-1).GetComponent<golf_courseScoreboard> ();
		golf_courseObject CO = golf_courseManager.instance.courses[courseNumber-1];

		if (!isFinal) {
			CS.Set (courseNumber, CO.name, CO.par, FormatParScore (CO.par, hitCounter));
		} else {
			CS.Set_Finalised(courseNumber, CO.name, CO.par, FormatParScore (CO.par, hitCounter));
		}
	}

	public void SetUI_ScoreboardTotal_title(int currentHole){
		scoreboardTotal_title.text = "THROUGH " + currentHole + ":";
	}

	public void SetUI_TotalCounter(int count, int par){
		scoreboardTotal.text = "" + FormatParScore(par,count);
		//Format the total score correctly
		SetTotalScoreColor(FormatParScore(par,count));
	}

	void SetTotalScoreColor(int score){
		if (score < 1) {
			scoreboardTotal_bg.color = fiero_uiManager.instance.color_green;
		} else {
			scoreboardTotal_bg.color = fiero_uiManager.instance.color_red;
		}
	}

	public void Clear(){
		SetUI_TotalCounter (0,0);

		foreach (Transform child in scoreBoardParent) {
			GameObject.Destroy(child.gameObject);
		}
	}

	int FormatParScore(int par, int hits){
		return hits - par;
	}
}
