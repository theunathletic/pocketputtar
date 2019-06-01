using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class golf_roundComplete_scoreboardObject : MonoBehaviour {

	public Text text_number;
	//public Text text_title;
	public Text text_par;
	public Text text_score;
	public Image image_levelPreview;
	public Image image_scoreBackground;

	//public Color color_scoreGood;
	//public Color color_scoreBad;

	public void Set(int num, string title, int par, int score, Sprite levelSprite){
		text_number.text = "" + num + ".";
		//text_title.text = title;
		text_par.text = "/" + par;
		text_score.text = "" + score;

		image_levelPreview.sprite = levelSprite;

		SetScoreColor (score);
	}

	private void SetScoreColor(int score){
		if (score < 1) {
			image_scoreBackground.color = fiero_uiManager.instance.color_green;
		} else {
			image_scoreBackground.color = fiero_uiManager.instance.color_red;
		}
	}
}
