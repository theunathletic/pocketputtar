using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class golf_courseScoreboard : MonoBehaviour {

	public Text text_number;
	public Text text_title;
	public Text text_par;
	public Text text_score;
	public Image background;

	public void Set(int num, string title, int par, int score){
		text_number.text = "" + num + ".";
		text_title.text = title;
		text_par.text = "" + par;
		text_score.text = "" + score;

		SetScoreColor (score);
	}

	public void Set_Finalised(int num, string title, int par, int score){
		background.color = Color.clear;
		if (score < 1) {
			text_score.color = fiero_uiManager.instance.color_green;
		} else {
			text_score.color = fiero_uiManager.instance.color_red;
		}
	}

	public void Set_Empty(int num){
		text_number.text = "" + num + ".";
		text_score.text = "-";
		text_score.color = fiero_uiManager.instance.color_black;
		background.color = Color.clear;
	}

	void SetScoreColor(int score){
		text_score.color = fiero_uiManager.instance.color_white;
		if (score < 1) {
			background.color = fiero_uiManager.instance.color_green;
		} else {
			background.color = fiero_uiManager.instance.color_red;
		}
	}
}
