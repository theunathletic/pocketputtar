using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class fiero_uiManager : MonoBehaviour {

	public static fiero_uiManager instance;
	public BlurOptimized blurFilter;

	[Header ("Colours")]
	public Color color_white;
	public Color color_black;
	public Color color_green;
	public Color color_red;

    [Header("Course Select")]
    public GameObject courseSelect_scrollParent;
    public GameObject courseSelect_button_select;
    public GameObject courseSelect_button_selectGhost;

    [Header ("Level Select")]
	public GameObject levelSelect_scrollParent; 
	public GameObject levelSelect_scroll_courseButtonPrefab;

	[Header ("Course Placement")]
	public Button setup_button_confirm;//REMOVE

	public GameObject coursePlacement_marker_wall;//REMOVE
	public GameObject coursePlacement_marker_floor;//REMOVE
	public GameObject coursePlacement_parent_marker_floor;//REMOVE

	public GameObject coursePlacement_parent_instructions;//REMOVE
	public GameObject coursePlacement_parent_found;//REMOVE


	public GameObject[] coursePlacement_instructions;//REMOVE
	public GameObject[] coursePlacement_buttons;//REMOVE


	public tickertape_ui coursePlacement_tickertape;//REMOVE
	//REMOVE
	[System.Serializable]
	public class coursePlacement_tickertape_content
	{
		public string[] content;
	};

	public coursePlacement_tickertape_content[] coursePlacement_tickertape_steps;//REMOVE
	public List<GameObject> coursePlacement_steps;
	public Text coursePlacement_text_stepCounter;//REMOVE

	[Header ("Ball Placement")]
	public List<GameObject> ballPlacement_steps;

	[Header ("Gameplay")]
	public Text play_text_courseNumber;
	public Text play_text_hitsCounter;
	public Text play_text_par;
	public Text play_text_viewAngle;

	public Image play_image_counterParentBG;
	public Image play_image_hitsCounterBG;

	public GameObject play_tutorial;

	public GameObject play_button_swingGhost;

	public GameObject play_button_swingParent;
	public GameObject play_button_swing;
	public GameObject play_button_swing_overPar;

	public Animator play_anim_scoreMessage;
	public Text play_text_scoreMessage;

	[Header ("Course Complete")]
	public GameObject completeCourse_buttonGroup_standard;
	public GameObject completeCourse_buttonGroup_levelselect;

	public Text completeCourse_text_par;
	public Text completeCourse_text_score;
    public Image completeCourse_image_scoreBG;

    public Text completeCourse_text_feedback;

    //public golf_scoreBoardManager completeRound_scoreBoard;
    public golf_scoreBoardManager ingame_scoreBoard;

    [Header ("Round Complete")]
	public golf_roundComplete_scoreboardScroll completeRound_scoreBoard;

	void Awake () {
		instance = this;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ToggleBlurFilter(bool toggle){
//		print("uiManager - ToggleBlurFilter: " + toggle);
		blurFilter.enabled = toggle;
	}

	//Main Menu screen -----------------------------------------------------------------------
	public void Mainmenu_Start(){
		fiero_appManager.instance.currentGameMode = fiero_appManager.GameMode.Standard;
		CompleteCourse_ShowButtonGroup_Standard (true);

		fiero_uiManager.instance.CompleteRound_ClearScoreboard ();
		fiero_uiManager.instance.IngameScoreboard_Setup ();

        //fiero_appManager.instance.SelectCourseNum (1);
        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.courseSelect);

        fiero_audioManager.instance.PlayButtonSound();
	}

	public void Mainmenu_Practice(){
		fiero_appManager.instance.currentGameMode = fiero_appManager.GameMode.Practice;
		CompleteCourse_ShowButtonGroup_Standard (false);
		//fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.levelSelect);
        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.courseSelect);

        fiero_uiManager.instance.CompleteRound_ClearScoreboard ();
		fiero_uiManager.instance.IngameScoreboard_Setup ();

        fiero_audioManager.instance.PlayButtonSound();
    }

    public void Mainmenu_OpenAbout() {
        //fiero_screenManager.instance.CloseCurrent();

        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.about);
        fiero_audioManager.instance.PlayButtonSound();
    }

    public void Mainmenu_OpenSupport()
    {
        //fiero_screenManager.instance.CloseCurrent();

        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.supportTheDevs);
        fiero_audioManager.instance.PlayButtonSound();
    }

    //Course Select screen -----------------------------------------------------------------------
    public void CourseSelect_Close()
    {
        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.mainmenu);
        fiero_audioManager.instance.PlayButtonSound();
    }

    public void CourseSelect_PickCourse(int courseNumber)
    {
        //Should set the paretn course number here.

        fiero_audioManager.instance.PlayButtonSound();

        switch (fiero_appManager.instance.currentGameMode) {
            case fiero_appManager.GameMode.Standard:
                fiero_appManager.instance.SelectCourseNum(1);
                break;

            case fiero_appManager.GameMode.Practice:
                fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.levelSelect);
                break;
        }
    }

    public void CourseSelect_ToggleSelectButtonActive(bool isActive)
    {
        courseSelect_button_selectGhost.SetActive(!isActive);
        courseSelect_button_select.SetActive(isActive);
    }

    //Level Select screen -----------------------------------------------------------------------
    public void LevelSelect_Close(){
		//fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.mainmenu);

        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.courseSelect);
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void LevelSelect_PickLevel(int levelNumber){
		fiero_appManager.instance.SelectCourseNum (levelNumber);
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void LevelSelect_AddLevelButton(int number, string name, int par){
        // Create Buttons
    //    print("uiManager : LevelSelect_AddLevelButton ");

		GameObject tempButtonObject = (GameObject)Instantiate(levelSelect_scroll_courseButtonPrefab);
		Button tempButtonButton = tempButtonObject.GetComponent<Button> ();
		golf_courseButton tempButtonScript = tempButtonObject.GetComponent<golf_courseButton> ();

		// Set button text
		tempButtonObject.transform.SetParent (levelSelect_scrollParent.transform);
		tempButtonScript.Set (number, name);
		tempButtonButton.onClick.AddListener(() => tempButtonScript.Select());

		tempButtonObject.transform.localScale = new Vector3 (1f, 1f, 1f);
		tempButtonObject.transform.localPosition = new Vector3 (tempButtonObject.transform.localPosition.x, tempButtonObject.transform.localPosition.y, 0);
	}

	//Course Placement screen -----------------------------------------------------------------------

	//ARKIT COURSE PLACEMENT New_Option1 
	public void PlayCoursePlacement_PlaceEndPoint(){
		//golf_courseManager.instance. place end point and show the 
		golf_courseManager.instance.ConfirmHolePlacement_ARKit();
		fiero_audioManager.instance.PlayCoursePlacementSound ();
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void PlayCoursePlacement_PlaceStartPoint(){
		golf_courseManager.instance.ConfirmCoursePlacement_ARKit ();
		fiero_audioManager.instance.PlayCoursePlacementSound ();
        fiero_audioManager.instance.PlayButtonSound();
    }
	//ARKIT COURSE PLACEMENT New_Option1


	public void PlayCoursePlacement_Place(){
		setup_button_confirm.interactable = true;
		golf_courseManager.instance.PlaceObject(new Vector2(Screen.width / 2, Screen.height / 2));

		fiero_audioManager.instance.PlayCoursePlacementSound ();
	}
	
	public void PlayCoursePlacement_ConfirmPlace(){
		golf_courseManager.instance.ConfirmPlacement ();

		fiero_audioManager.instance.PlayCoursePlacementSound ();

	}

	public void PlayCoursePlacement_ToggleMarkerWall(bool isOn){
		coursePlacement_marker_wall.SetActive (isOn);
	}

	public void PlayCoursePlacement_ToggleMarkerFloor(bool isOn){
		coursePlacement_marker_floor.SetActive (isOn);
	}

	public void PlayCoursePlacement_MarkerFloorParent_Enabled(bool isOn){
		coursePlacement_parent_marker_floor.SetActive (isOn);
	}



	public void PlayCoursePlacement_ToggleFoundParent(bool isOn){
		coursePlacement_parent_found.SetActive (isOn);
		coursePlacement_parent_instructions.SetActive (!isOn);
	}

	public void PlayCoursePlacement_ToggleInstructions(bool isOn, int number){
		coursePlacement_instructions[number].SetActive (isOn);
	}

	public void PlayCoursePlacement_ToggleButtons(bool isOn, int number){
		coursePlacement_buttons[number].SetActive (isOn);
	}

	public void PlayCoursePlacement_ShowStep(int number){
//		print("uiManager - PlayCoursePlacement_ShowStep: " + number);

		foreach (GameObject step in coursePlacement_steps) {
			step.SetActive (false);
		}
		coursePlacement_steps[number].SetActive (true);

		/*PlayCoursePlacement_SetStepCounter (number);*/
		/*PlayCoursePlacement_SetTickertapeStep (number);*/
	}

	private void PlayCoursePlacement_SetTickertapeStep(int number){
		coursePlacement_tickertape.SetTickertapeStrings (coursePlacement_tickertape_steps [number].content);
	}

	private void PlayCoursePlacement_SetStepCounter(int number){
		coursePlacement_text_stepCounter.text = ((number+2) / 2).ToString ();
	}

	//PlayBallPlaceCourse screen -----------------------------------------------------------------------
	public void PlayBallPlacement_ShowStep(int number){
	//	print("uiManager - PlayBallPlacement_ShowStep: " + number);

		foreach (GameObject step in ballPlacement_steps) {
			step.SetActive (false);
		}
		ballPlacement_steps[number].SetActive (true);
	}

	public void PlayBallPlacement_Confirm(){
		fiero_audioManager.instance.PlayBallPlacementSound ();
		golf_courseManager.instance.ConfirmBallPlacement ();
	}

	//Play screen -----------------------------------------------------------------------
	public void Play_SwingStart(){
	//	print ("Play_SwingStart");
		fiero_appManager.instance.StartHit ();
	}

	public void Play_SwingEnd(){
	//	print ("Play_SwingEnd");
		fiero_appManager.instance.FinishHit ();
	}
		
	public void Play_Swing(){
		fiero_appManager.instance.HitBall ();
	}

	public void Play_SpawnBall(){
		//golf_courseManager.instance.SpawnBall ();
		fiero_appManager.instance.StartCourse ();
	}

	public void Play_Pause(){
		fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.pause);
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void Play_ToggleSwingTutorial(bool isActive){
	//	print ("Play_ToggleSwingTutorial" + isActive);
		play_tutorial.SetActive (isActive);
	}

	public void Play_ToggleSwingButtonActive(bool isActive){
		play_button_swingGhost.SetActive (!isActive);
		play_button_swingParent.SetActive (isActive);
	}

	public void Play_SetUI_ParStyle_UnderPar(){
		play_text_courseNumber.color = color_black;
		play_text_hitsCounter.color = color_black;
		play_text_par.color = color_black;
		play_image_counterParentBG.color = color_white;
		play_image_hitsCounterBG.color = color_green;

		//show green hit button
		play_button_swing.SetActive(true);
		play_button_swing_overPar.SetActive(false);
		
	}

	public void Play_SetUI_ParStyle_OverPar(){
		play_text_courseNumber.color = color_black;
		play_text_hitsCounter.color = color_white;
		play_text_par.color = color_black;
		play_image_counterParentBG.color = color_white;
		play_image_hitsCounterBG.color = color_red;

		//show green hit button
		play_button_swing.SetActive(true);
		play_button_swing_overPar.SetActive(false);
	}

	public void Play_SetUI_ParStyle_MaxHitCount(){
		play_text_courseNumber.color = color_white;
		play_text_hitsCounter.color = color_black;
		play_text_par.color = color_white;
		play_image_counterParentBG.color = color_red;
		play_image_hitsCounterBG.color = color_white;

		//show red hit button
		play_button_swing.SetActive(false);
		play_button_swing_overPar.SetActive(true);
	}

	public void Play_SetUI_Text_Course(int count){
		play_text_courseNumber.text = "HOLE "+count+":";
	}

	public void Play_SetUI_Text_Hits(int count){
		play_text_hitsCounter.text = ""+count;
	}

	public void Play_SetUI_Text_Par(int count){
		play_text_par.text = "/"+count;
	}

	public void Play_SetUI_Text_ViewAngle(float angle, bool isInRange){
		play_text_viewAngle.text = ""+angle.ToString("F0") + "°";
		if (isInRange) {
			play_text_viewAngle.color = Color.white;
		} else {
			play_text_viewAngle.color = Color.red;
		}
	}

    // REMOVE ---------------------------------------
	public void Play_ShowScoreMessage_HoleInOne(int score){
		play_text_scoreMessage.text = "HOLE-IN-ONE!";
		play_anim_scoreMessage.SetTrigger ("Play");
	}

	public void Play_ShowScoreMessage_Other(int score){
		switch (score) {
		case -3:
			play_text_scoreMessage.text = "ALBATROSS";
			break;

		case -2:
			play_text_scoreMessage.text = "EAGLE";
			break;

		case -1:
			play_text_scoreMessage.text = "BIRDIE";
			break;

		case 0:
			play_text_scoreMessage.text = "PAR";
			break;

		case 1:
			play_text_scoreMessage.text = "BOGEY";
			break;

		case 2:
			play_text_scoreMessage.text = "DOUBLE BOGEY";
			break;

		default:
			play_text_scoreMessage.text = "FINALLY...";
			break; 
		}

		play_anim_scoreMessage.SetTrigger ("Play");
	}
    // REMOVE ---------------------------------------


    //Play pause screen -----------------------------------------------------------------------
    public void Play_Pause_Resume(){
        //fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.play);
        Time.timeScale = 1.0f;
		fiero_appManager.instance.SwitchGameState (fiero_appManager.instance.previousAppState);
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void Play_Pause_Exit(){
        Time.timeScale = 1.0f;
        golf_playerRangeManager.instance.SetEnabled (false);
		//golf_courseManager.instance.ResetAll ();

		fiero_screenManager.instance.CloseCurrent ();

		fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.mainmenu);
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void Play_Pause_RepositionCourse(){
        Time.timeScale = 1.0f;
        golf_playerRangeManager.instance.SetEnabled (false);
		golf_courseManager.instance.ResetAll ();
		fiero_screenManager.instance.CloseCurrent ();
		fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.playCoursePlacement);
        fiero_audioManager.instance.PlayButtonSound();


    }

    public void Play_Pause_ChangeTimeScale(float newTimeScale) {
        fiero_appManager.instance.Debug_ChangeTimeScale(newTimeScale);
        fiero_audioManager.instance.PlayButtonSound();
    }

    //Play Out of Bounds screen -----------------------------------------------------------------------
    public void Play_OutOfBounds_ResetBall(){
		//fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.play);
	//	print("Play_OutOfBounds_ResetBall click");
		fiero_appManager.instance.ResetBallInBounds();
        fiero_audioManager.instance.PlayButtonSound();
    }

	//Play MaxHitCount screen -----------------------------------------------------------------------
	public void Play_MaxHitCount_FinishCourse(){
	//	print("Play_MaxHitCount_FinishCourse click");
		fiero_appManager.instance.FinishCourse();
        //CompleteCourse_NextHole ();
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void Play_MaxHitCount_KeepTrying(){
	//	print("Play_MaxHitCount_KeepTrying click");
		fiero_appManager.instance.SwitchGameState (fiero_appManager.instance.previousAppState);
        fiero_audioManager.instance.PlayButtonSound();
    }

	//CompleteCourse Screen -----------------------------------------------------------------------

	public void CompleteCourse_ShowButtonGroup_Standard(bool isStandard){
		completeCourse_buttonGroup_levelselect.SetActive (!isStandard);
		completeCourse_buttonGroup_standard.SetActive (isStandard);

	}

	public void CompleteCourse_PlayAgain(){
		
		//fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.play);
		//golf_courseManager.instance.SpawnBall ();

		fiero_appManager.instance.StartBallPlacement ();
	}

	public void CompleteCourse_NextHole(){

		//fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.play);
		//golf_courseManager.instance.SpawnBall ();
		golf_courseManager.instance.NextCourse();
		fiero_appManager.instance.StartBallPlacement ();
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void CompleteCourse_Exit(){
		fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.mainmenu);
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void CompleteCourse_ExitToLevelSelect(){
		fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.levelSelect);
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void CompleteCourse_SetUI_Text_Hits(int count){
		completeCourse_text_score.text = ""+count;
	}

	public void CompleteCourse_SetUI_Text_Par(int count){
		completeCourse_text_par.text = "/"+count;

	}

    public void CompleteCourse_SetUI_Text_Feedback(string feedback)
    {
        completeCourse_text_feedback.text = "" + feedback;

    }
    public void CompleteCourse_SetUI_ParStyle_UnderPar()
    {
        completeCourse_text_score.color = color_black;
        completeCourse_image_scoreBG.color = color_green;
    }

    public void CompleteCourse_SetUI_ParStyle_OverPar()
    {
        completeCourse_text_score.color = color_white;
        completeCourse_image_scoreBG.color = color_red;

    }

    //CompleteRoundScreen -----------------------------------------------------------------------
    public void IngameScoreboard_Setup(){
//		Debug.Log ("fiero_uiManager: IngameScoreboard_Setup " + golf_courseManager.instance.courses.Count);
		//ingame_scoreBoard.SetupNumberOfScores (18);
		ingame_scoreBoard.SetupNumberOfScores (golf_courseManager.instance.courses.Count);
	}

	public void IngameScoreboard_SetUI_Text_Course(int count){
		ingame_scoreBoard.SetUI_ScoreboardTotal_title (count);
	}
		

	public void IngameScoreboard_Update(int hits, int courseNumber, int totalHits, int totalPar){
		ingame_scoreBoard.UpdateCourseScore(hits,courseNumber,false);
		ingame_scoreBoard.SetUI_TotalCounter (totalHits,totalPar);
	}

	//The final update for this course.
	public void IngameScoreboard_Update_Final(int hits, int courseNumber){
		ingame_scoreBoard.UpdateCourseScore(hits,courseNumber,true);
	}

	public void CompleteRound_AddFinalCourseScore(int hits, int courseNumber, int totalHits, int totalPar){
		completeRound_scoreBoard.AddCourseScore (hits, courseNumber);
		completeRound_scoreBoard.SetUI_TotalCounter (FormatParScore(totalPar,totalHits));
		completeRound_scoreBoard.SetUI_TotalParCounter (totalPar);
		/*
		ingame_scoreBoard.AddCourseScore (hits, courseNumber);
		ingame_scoreBoard.SetUI_TotalCounter (totalHits);
		*/
	}

	//Could be moved into a utils script.
	int FormatParScore(int par, int hits){
		return hits - par;
	}

	//NOT USED ANYMORE
	public void CompleteRound_AddScore(int hits, int courseNumber, int totalHits, int totalPar){
		completeRound_scoreBoard.AddCourseScore (hits, courseNumber);
		completeRound_scoreBoard.SetUI_TotalCounter (totalHits);
		completeRound_scoreBoard.SetUI_TotalParCounter (totalPar);

		ingame_scoreBoard.AddCourseScore (hits, courseNumber);
		ingame_scoreBoard.SetUI_TotalCounter (totalHits,totalPar);
	}

	public void CompleteRound_ClearScoreboard(){
		completeRound_scoreBoard.Clear ();
		ingame_scoreBoard.Clear ();
	}

	public void CompleteRound_PlayAgain(){
		//fiero_appManager.instance.StartBallPlacement ();
		fiero_appManager.instance.ResetRound ();
        fiero_audioManager.instance.PlayButtonSound();
    }

	public void CompleteRound_Exit(){
		fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.mainmenu);
        fiero_audioManager.instance.PlayButtonSound();
    }

    //About -----------------------------------------------------------------------
    public void About_Exit()
    {
        fiero_screenManager.instance.CloseCurrent();

        fiero_appManager.instance.SwitchGameState(fiero_appManager.instance.previousAppState);
        fiero_audioManager.instance.PlayButtonSound();
    }

    public void About_Link_Website()
    {
        Application.OpenURL("https://theunathletic.club/");
    }

    public void About_Link_Twitter()
    {
        Application.OpenURL("https://twitter.com/UnathleticAF");
    }

   

    //Support the devs -----------------------------------------------------------------------
    public void SupportTheDevs_Exit()
    {
        //fiero_screenManager.instance.CloseCurrent();

        if(fiero_appManager.instance.previousAppState == fiero_appManager.AppState.mainmenu) {
            fiero_appManager.instance.SwitchGameState(fiero_appManager.instance.previousAppState);
        }
        else {
            fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.share);
        }


        fiero_audioManager.instance.PlayButtonSound();
    }

    //NOT NEEDED, THIS IS ALL HADLED BY THE CODELESS IAP NOW. (I THINK?)
    public void SupportTheDevs_DonateMoney(int index) {
        fiero_audioManager.instance.PlayButtonSound();
    }
    //SHARE -----------------------------------------------------------------------

    public void Share_TwitterShare()
    {
        //DO SOME TWITTER SHARING HERE>
        TUA_SupportUs.instance.startLogin();
        fiero_audioManager.instance.PlayButtonSound();
    }

    public void Share_TwitterShare_Confirmed()
    {
        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.thankYou);
    }

    public void Share_Exit()
    {
        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.completeRound);

        fiero_audioManager.instance.PlayButtonSound();
    }

    //Thank You -----------------------------------------------------------------------
    public void ThankYou_Exit()
    {
        //fiero_screenManager.instance.CloseCurrent();
        fiero_audioManager.instance.PlayButtonSound();
        //check what appstate we were in before the 'support the devs' sequence started.

        if(TUA_SupportUs.instance.currentSupportFlow == TUA_SupportUs.SupportFlow.mainmenu) {
            fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.mainmenu);
        }
        else if (TUA_SupportUs.instance.currentSupportFlow == TUA_SupportUs.SupportFlow.completeRound)
        {
            fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.completeRound);
        }
        else if (TUA_SupportUs.instance.currentSupportFlow == TUA_SupportUs.SupportFlow.endOf18Holes)
        {
            fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.completeRound);
        }
        //fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.completeRound);
       
    }

}
