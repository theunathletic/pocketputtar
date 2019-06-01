using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class golf_playerRangeManager : MonoBehaviour {

	public static golf_playerRangeManager instance;

	public Transform playerTransform;
	public Transform ballTransform;

	private bool active = false;

	public Projector rangeProjector;
	public GameObject rangeMeshObject;
	public bool playerInRange = false;
	public float maxPlayerRange = 0.5f;



	public Color inRangeColor;
	public Color outOfRangeColor;

	public bool playerInAngle = true;
	public float viewAngle = 0f;
	public float maxViewAngle = 30f;

	public Transform club_direction_pivot;
	public Transform club_swing_pivot;
	private float maxSwingRot = 20f;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
		this.gameObject.SetActive (false);
		SetRangeIndicatorDistance (1f);
	}
	
	// Update is called once per frame
	void Update () {

		if (active) {
			MoveToBall ();
			CheckRange ();
			//CheckAngle ();

			PositionClub ();
		}
	}

	public void SetEnabled(bool isOn){
		active = isOn;

		if (!isOn) {
			playerInRange = false;

		}

		this.gameObject.SetActive (active);
		fiero_uiManager.instance.Play_ToggleSwingButtonActive (false);
		SetClubVisible ();
		SetIndicatorColor ();
        SetRangeMeshVisible();
    }


	public void SetBallTranform(Transform ball){
		ballTransform = ball;
	}

	//FollowBall
	public void MoveToBall(){
		this.transform.position = ballTransform.position;
	}

	//Check range
	public void SetRangeIndicatorDistance(float scale){
		rangeProjector.orthographicSize = maxPlayerRange * scale;
	}

	public void CheckRange(){
		float dist = Vector2.Distance (new Vector2 (playerTransform.position.x, playerTransform.position.z), new Vector2 (this.transform.position.x, this.transform.position.z));
		//playerInRange = dist < maxPlayerRange;
		bool tempPlayerInRange = dist < maxPlayerRange;
		if (playerInRange != tempPlayerInRange) {
			playerInRange = tempPlayerInRange;
			SetIndicatorColor ();

			SetRangeMeshVisible ();

			SetClubVisible ();

			fiero_uiManager.instance.Play_ToggleSwingButtonActive (playerInRange);
		}


//		SetIndicatorColor ();
//
//		SetRangeMeshVisible ();
//
//		SetClubVisible ();
	}

	private void SetIndicatorColor(){
		rangeProjector.material.color = playerInRange ? inRangeColor : outOfRangeColor;
	}

	private void SetRangeMeshVisible(){
		rangeMeshObject.SetActive (!playerInRange);
	}

	//CheckAngle
	public void CheckAngle(){
		//viewAngle = 180f - Vector3.Angle (playerTransform.transform.forward, (playerTransform.position-ballTransform.position) );
		viewAngle = 180f - Vector3.Angle (playerTransform.transform.forward, golf_courseManager.instance.upDir );

		playerInAngle = viewAngle < maxViewAngle;
		fiero_uiManager.instance.Play_SetUI_Text_ViewAngle(viewAngle,playerInAngle);
	}

	//Club position
	public void PositionClub (){
		Vector3 forward;
		Vector3 up = golf_courseManager.instance.upDir;
		Vector3 right = Vector3.Cross(up, Camera.main.transform.right).normalized;
		forward = Vector3.Cross(right, up).normalized;

		club_direction_pivot.rotation = Quaternion.LookRotation (-forward, up);

	}

	public void UpdateSwing (float value){
		//club_direction_pivot.localRotation = Quaternion.LookRotation(club_direction_pivot.transform.forward,
	//		club_swing_pivot.RotateAround(club_direction_pivot.transform.position,Vector3.right, Mathf.Lerp(0,maxSwingRot,value));

		club_swing_pivot.localEulerAngles = new Vector3(Mathf.Lerp(0,maxSwingRot,value), 0, 0);
	}

	public void SetClubVisible(){
		club_direction_pivot.gameObject.SetActive (playerInRange);
	}
		


}
