using UnityEngine;
using System.Collections;
using UnityEngine.XR.iOS;
using UnityEngine.iOS;

public class fiero_appManager : MonoBehaviour {

	public static fiero_appManager instance;

	public enum AppState{mainmenu, courseSelect, levelSelect, playCoursePlacement, playBallPlacement, play, outOfBounds, maxHitCount, pause, pauseReposition, completeCourse, completeRound, about, thankYou, supportTheDevs, share};
	public AppState currentAppState = AppState.mainmenu;
	public AppState previousAppState;

	public enum GameMode{Standard,Practice};
	public GameMode currentGameMode = GameMode.Standard;

	public enum ParState{underPar,overPar,maxHitCount};
	public ParState currentParState = ParState.underPar;

	public golf_golfball currentBall;
	private bool chargingHit = false;
	public float maxChargeTimer = 2f;
	private float currentHitTimer = 0f;

	public float maxHitPower = 2f;
	private float currentNormalisedHitPower = 0f;

	private int hitCounter = 0;
	private int parCounter = 0;
	private int totalHitCounter = 0;
	private int totalParCounter = 0;

	public int maxCourseHitCount = 10; 

	public int maxHitCounterAdd = 5; //added onto the par for the course.


	 
	public GameObject ARKit_pointCloudDisplay;
	public GameObject ARKit_planes;


	public UnityARCameraManager ARCamManager;

	private bool tutorialShown = false;
    private bool playerPrefs_supportedDevs;



	//Editor testing, move to different script.
	public GameObject ARKit_parent;
	public UnityARCameraNearFar ARKit_camNearFar;
	public UnityARVideo ARKit_video; // using UnityEngine.XR.iOS;
	public FlyCamera flycam;
	public bool editorTesting;






	void Awake () {
		instance = this;

	//	AndroidHelper.OnTangoServiceDisconnected += ForceReposition;



	}
	/*
	void ForceReposition(){
		print ("ForceReposition");
		if ( (currentAppState != AppState.mainmenu) || (currentAppState != AppState.levelSelect)) {
			golf_playerRangeManager.instance.SetEnabled (false);
			golf_courseManager.instance.ResetAll ();
			fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.playCoursePlacement);
		}


	}
	*/

	// Use this for initialization
	void Start () {
        //ARCamManager.planeDetectionOFF ();
        playerPrefs_supportedDevs = TUA_SupportUs.instance.Load_SupportedDevsStatus();


        if (!editorTesting) {
			ARKit_pointCloudDisplay.SetActive (false);
			//ARKit_planes.SetActive (false);
			Destroy (flycam);
		}

		if (editorTesting) {
			foreach (Transform child in ARKit_parent.transform) {  
				//child.gameObject.SetActiveRecursively (false);
                child.gameObject.SetActive(false);

            }  

			ARKit_camNearFar.enabled = false;
			ARKit_video.enabled = false;

			Camera.main.clearFlags = CameraClearFlags.SolidColor;
		}


		SwitchGameState (AppState.mainmenu);

		//Testing
		//SwitchGameState (AppState.play);
	}
	
	// Update is called once per frame
	void Update () {

		switch (currentAppState) {
		case AppState.playCoursePlacement:
			 //Not doing anything
			/*
			if (Input.GetKeyDown (KeyCode.Space)) {
				print (" PlayCoursePlacement_ConfirmPlace ");
				fiero_uiManager.instance.PlayCoursePlacement_ConfirmPlace ();
			}
			*/

			if (Input.GetKeyUp (KeyCode.U)) {
		//		print ("TestEnablePlanes");
				//ARKit_planes.GetComponent<UnityARGeneratePlane> ().TestEnablePlanes ();
				ARCamManager.ARgenPlane.MakeTestPlane ();
			} else if (Input.GetKeyUp (KeyCode.I)) {
				ARCamManager.ARgenPlane.RestartPlaneGenerator ();
			}
			break;

		case AppState.playBallPlacement:
			if (Input.GetKeyDown (KeyCode.Space)) {
				fiero_uiManager.instance.PlayBallPlacement_Confirm ();
			}
			else if(Input.GetKeyUp (KeyCode.I)){
		//		print ("TestDestroyPlanes");
				ARKit_planes.GetComponent<UnityARGeneratePlane> ().TestDestroyPlanes ();
			}

			break;
			

		case AppState.play:
			if (Input.GetKeyDown (KeyCode.Space)) {
			//	StartHit ();
				fiero_uiManager.instance.Play_SwingStart ();
			} else if (Input.GetKeyUp (KeyCode.Space)) {
			//	FinishHit ();
				fiero_uiManager.instance.Play_SwingEnd ();
			}

			else if(Input.GetKeyUp (KeyCode.F)){
				FinishCourse();
			}

			else if(Input.GetKeyUp (KeyCode.G)){
				Debug_SkipToCompleteRound ();
			}

			else if(Input.GetKeyUp (KeyCode.O)){
				golf_courseManager.instance.CourseOrientation_Flip();
			}



			CalcHit ();
			break;

		}
	
	}

	public void SwitchGameState(AppState nextState){
		previousAppState = currentAppState;
		currentAppState = nextState;
//		print("appManager - SwitchGameState: " + currentAppState);
		switch (currentAppState) {

		case AppState.mainmenu:
			
			//fiero_uiManager.instance.ToggleBlurFilter (true);

			totalHitCounter = 0;
			totalParCounter = 0;

			//fiero_uiManager.instance.CompleteRound_ClearScoreboard ();

			//golf_playerRangeManager.instance.SetEnabled (false);
			if (previousAppState != AppState.levelSelect) {
				if (!golf_courseManager.instance.GetLockedPosition ()) {
					golf_courseManager.instance.ResetAll ();
				}
			}

			//Show ARKit visuals.

			if (!editorTesting) {
				ARCamManager.planeDetectionOFF ();
				ARKit_pointCloudDisplay.SetActive (false);
				//ARKit_planes.SetActive (false);
			}

			fiero_screenManager.instance.OpenPanel (fiero_screenManager.instance.screen_mainmenu, 4);
			break;

           case AppState.courseSelect:
                fiero_uiManager.instance.ToggleBlurFilter(true);
                fiero_screenManager.instance.OpenPanel(fiero_screenManager.instance.screen_courseSelect, 4);
                break;

            case AppState.levelSelect:
			fiero_uiManager.instance.ToggleBlurFilter (true);

			fiero_screenManager.instance.OpenPanel (fiero_screenManager.instance.screen_levelSelect, 4);
			break;

		case AppState.playCoursePlacement:
			
			fiero_uiManager.instance.ToggleBlurFilter (false);
			/*
			if (!golf_courseManager.instance.GetLockedPosition ()) {
				print("appManager - SwitchGameState: playCoursePlacement: not locked");
				fiero_screenManager.instance.OpenPanel (fiero_screenManager.instance.screen_play_placeCourse, 4);
				fiero_uiManager.instance.setup_button_confirm.interactable = false;
			} else {
				print("appManager - SwitchGameState: playCoursePlacement: locked");
				StartBallPlacement ();
			}
			*/
			//print ("appManager - SwitchGameState: playCoursePlacement: not locked");
			//Show ARKit visuals.
			//ARKit_pointCloudDisplay.SetActive (true);
			//ARKit_planes.SetActive (true);

			//Check if course is already placed (locked)
			if (golf_courseManager.instance.GetLockedPosition ()) {

				//Check to see if the ingame scoreboard needs to be reshown. Only happens if course is already placed (locked), otherwise happens in golf_courseManager 
				if (fiero_appManager.instance.currentGameMode == fiero_appManager.GameMode.Standard) {
					golf_courseManager.instance.courseScorebord.SetActive (true);
				} else {
					golf_courseManager.instance.courseScorebord.SetActive (false);
				}
				StartBallPlacement ();
			} 
			else {
				if (!editorTesting) {
					ARKit_pointCloudDisplay.SetActive (true);
					ARCamManager.planeDetectionON ();
					//ARKit_planes.SetActive (true);
				}
				golf_courseManager.instance.StartCoursePlacement ();
				fiero_screenManager.instance.OpenPanel (fiero_screenManager.instance.screen_play_placeCourse, 4);
				//fiero_uiManager.instance.setup_button_confirm.interactable = false; NOT NEEDED
			}
				


			break;

		case AppState.playBallPlacement:
			//ARKit_pointCloudDisplay.SetActive (false);
			//ARKit_planes.SetActive (false);


			fiero_uiManager.instance.ToggleBlurFilter (false);

			fiero_screenManager.instance.OpenPanel (fiero_screenManager.instance.screen_play_placeBall, 4);
			break;

		case AppState.play:
			fiero_uiManager.instance.ToggleBlurFilter (false);

			fiero_screenManager.instance.OpenPanel (fiero_screenManager.instance.screen_play, 4);
			if ((previousAppState != AppState.pause) && 
				(previousAppState != AppState.outOfBounds) && 
				(previousAppState != AppState.maxHitCount)) {
				StartCourse ();
			}


			/*if ( previousAppState.Equals (AppState.pause) || previousAppState.Equals (AppState.outOfBounds)) {
				StartCourse ();
			}
		*/
			/*
			if(new[] {AppState.pause, AppState.outOfBounds}.Contains(previousAppState)){
				StartCourse ();
			}
			*/
			break;

		case AppState.outOfBounds:
			fiero_uiManager.instance.ToggleBlurFilter (true);
			fiero_screenManager.instance.OpenPanelOverTop (fiero_screenManager.instance.screen_play_outOfBounds, 4);
			break;

		case AppState.maxHitCount:
			fiero_uiManager.instance.ToggleBlurFilter (true);
			fiero_screenManager.instance.OpenPanelOverTop (fiero_screenManager.instance.screen_play_maxHitCount, 4);
			break;

		case AppState.pause:
			fiero_uiManager.instance.ToggleBlurFilter (true);

			Time.timeScale = 0f;
			fiero_screenManager.instance.OpenPanelOverTop (fiero_screenManager.instance.screen_play_pause, 4);
			break;

		case AppState.completeCourse:
			fiero_uiManager.instance.ToggleBlurFilter (true);

			fiero_screenManager.instance.OpenPanel (fiero_screenManager.instance.screen_completeCourse, 4);
			break;

		case AppState.completeRound:
			fiero_uiManager.instance.ToggleBlurFilter (true);

			fiero_screenManager.instance.OpenPanel (fiero_screenManager.instance.screen_completeRound, 4);

			break;

        case AppState.about:
            fiero_uiManager.instance.ToggleBlurFilter(true);

            fiero_screenManager.instance.OpenPanelOverTop(fiero_screenManager.instance.screen_about, 4);

            break;

        case AppState.thankYou:
            fiero_uiManager.instance.ToggleBlurFilter(true);

            fiero_screenManager.instance.OpenPanel(fiero_screenManager.instance.screen_thankYou, 4);

            break;

            case AppState.supportTheDevs:
                fiero_uiManager.instance.ToggleBlurFilter(true);

                fiero_screenManager.instance.OpenPanel(fiero_screenManager.instance.screen_supportTheDevs, 4);

                break;

            case AppState.share:
                fiero_uiManager.instance.ToggleBlurFilter(true);

                fiero_screenManager.instance.OpenPanel(fiero_screenManager.instance.screen_share, 4);
                Device.RequestStoreReview();

                break;


        }
	}


    //Setup
    public void SelectParentCourseNum(int courseNum)
    {
        //WOuld set the parent course number here.

        fiero_appManager.instance.SwitchGameState(fiero_appManager.AppState.playCoursePlacement);

    }

    public void SelectCourseNum(int courseNum){
		golf_courseManager.instance.SelectCourse (courseNum);

		fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.playCoursePlacement);

	}

	public void SetCurrentCoursePar(int par){
		parCounter = par;
		totalParCounter += parCounter;
		maxCourseHitCount = parCounter + maxHitCounterAdd;
		fiero_uiManager.instance.IngameScoreboard_Update (hitCounter, golf_courseManager.instance.currentCourseNum, totalHitCounter, totalParCounter);

	}



	public void CheckParState(){
	//	print ("PARSTATE");
		switch (currentParState) {
			case ParState.underPar:
				//hitCounter++;
				//totalHitCounter++;
				if(hitCounter> (parCounter)){
					SwitchParState (ParState.overPar);
				}
			break;

			case ParState.overPar:
				//hitCounter++;
				//totalHitCounter++;
				if (hitCounter >= maxCourseHitCount) {
					SwitchParState (ParState.maxHitCount);
				}
				break;

			case ParState.maxHitCount:
				break;
		}
	}

	//Trigger UI changes here.
	public void SwitchParState(ParState nextState){
		currentParState = nextState;

		switch (currentParState) {
		case ParState.underPar:
			fiero_uiManager.instance.Play_SetUI_ParStyle_UnderPar ();
            fiero_uiManager.instance.CompleteCourse_SetUI_ParStyle_UnderPar();

            break;

		case ParState.overPar:
			fiero_uiManager.instance.Play_SetUI_ParStyle_OverPar ();
            fiero_uiManager.instance.CompleteCourse_SetUI_ParStyle_OverPar();
			break;

		case ParState.maxHitCount:
			fiero_uiManager.instance.Play_SetUI_ParStyle_MaxHitCount ();
			SwitchGameState (AppState.maxHitCount);
			break;
		}
	}
		
	public void ConfirmCoursePlacement(){
		if (!editorTesting) {
			ARCamManager.planeDetectionOFF ();
			ARKit_pointCloudDisplay.SetActive (false);
		}


		golf_courseManager.instance.UpdateMovingCourseObjects ();
		if (currentAppState == AppState.playCoursePlacement) {
			StartBallPlacement ();
		} else if(currentAppState == AppState.pauseReposition) {
			
		}
	}

	public void StartBallPlacement(){
	//	golf_playerRangeManager.instance.SetEnabled (false);
//		Debug.Log("StartBallPlacement");

		DeleteCurrentBall ();
		ResetCourse ();
		golf_courseManager.instance.StartBallPlacement ();

		SwitchGameState (AppState.playBallPlacement);
	}

	public void StartCourse(){
		//ResetCourse ();
		golf_courseManager.instance.SpawnBall ();
		//golf_playerRangeManager.instance.SetBallTranform (currentBall.transform);
		golf_playerRangeManager.instance.SetEnabled (true);

		if (tutorialShown == false) {
			fiero_uiManager.instance.Play_ToggleSwingTutorial (true);
		}

	//	print ("StartCourse");
	}

	public void BallOutOfBounds(){
		if (currentAppState != AppState.outOfBounds) {
			SwitchGameState (AppState.outOfBounds);
		}
	}

	public void ResetBallInBounds(){
	//	print("ResetBallInBounds click");
		golf_courseManager.instance.SpawnBall ();
		fiero_appManager.instance.SwitchGameState (fiero_appManager.instance.previousAppState);
	}

	public void ResetCourse(){
	//	print ("ResetCourse");
		hitCounter = 0;
		SwitchParState (ParState.underPar);
		fiero_uiManager.instance.Play_SetUI_Text_Hits (hitCounter);
		fiero_uiManager.instance.IngameScoreboard_SetUI_Text_Course (golf_courseManager.instance.currentCourseNum);
		fiero_uiManager.instance.IngameScoreboard_Update (hitCounter, golf_courseManager.instance.currentCourseNum, totalHitCounter, totalParCounter);

	//	print ("IngameScoreboard_Update" + hitCounter + golf_courseManager.instance.currentCourseNum + totalHitCounter + totalParCounter);
		//CHECKPARSTATE
	}

	public void SetCurrentBall(golf_golfball b){
		currentBall = b;
		b.SetCanMove (false);
	}

	public void DeleteCurrentBall(){
		if (currentBall != null) {
			Destroy (currentBall.gameObject);
		}
	}



	//Hit sequence
	private void CalcHit(){
		if (chargingHit) {
			currentHitTimer += Time.deltaTime;

			currentNormalisedHitPower = Mathf.InverseLerp(0,maxChargeTimer, Mathf.PingPong (currentHitTimer, maxChargeTimer));

			currentNormalisedHitPower = currentNormalisedHitPower * currentNormalisedHitPower;

			currentBall.UpdatePowerBar (currentNormalisedHitPower);

			golf_playerRangeManager.instance.UpdateSwing (currentNormalisedHitPower);
		}
	}

	public void StartHit(){
		if (golf_playerRangeManager.instance.playerInRange && golf_playerRangeManager.instance.playerInAngle) {
			currentHitTimer = 0f;
			chargingHit = true;

			if (tutorialShown == false) {
				tutorialShown = true;
				fiero_uiManager.instance.Play_ToggleSwingTutorial (false);
			}
		}
	}

	public void FinishHit(){
		if (golf_playerRangeManager.instance.playerInRange && golf_playerRangeManager.instance.playerInAngle && chargingHit) {
			//CHECKPAR
			//CheckParState();


			if ((currentParState == ParState.underPar) ||
				(currentParState == ParState.overPar)) {
				hitCounter++;
				totalHitCounter++;

				if (currentParState == ParState.underPar) {
					if (hitCounter > (parCounter)) {
						SwitchParState (ParState.overPar);
					}
				}
			}

			fiero_uiManager.instance.Play_SetUI_Text_Hits (hitCounter);
			//fiero_uiManager.instance.IngameScoreboard_Update (hitCounter, golf_courseManager.instance.currentCourseNum, totalHitCounter, (totalParCounter + parCounter));
			fiero_uiManager.instance.IngameScoreboard_Update (hitCounter, golf_courseManager.instance.currentCourseNum, totalHitCounter, totalParCounter);

			chargingHit = false;
			currentBall.UpdatePowerBar (0);
			currentBall.Hitball ( (currentNormalisedHitPower * (maxHitPower*0.95f)) + maxHitPower*0.05f);

            float volValue = Mathf.Lerp(0.2f, 1f, currentNormalisedHitPower);

            fiero_audioManager.instance.PlayPuttHitSound (volValue);

		//	print ("hit power : " + (currentNormalisedHitPower * maxHitPower));

			golf_playerRangeManager.instance.UpdateSwing (0f);

			golf_playerRangeManager.instance.SetEnabled (false);

            if (hitCounter == 1) {
               golf_courseManager.instance.ToggleGuide_PuttThisWay(false);
            }

        } else {
			chargingHit = false;
			currentBall.UpdatePowerBar (0);
		}

	}

	public void HitBall(){
		if (currentBall != null) {
			currentBall.Hitball (1f);
		}
	}

	public void BallStoppedCheck(){
		CheckParState ();
	}

	//Gameover
	public void FinishCourse(){
		//fiero_audioManager.instance.PlayHoleSound ();
		CalcScore ();

	//	print ("IN THE HOLE");
		fiero_uiManager.instance.IngameScoreboard_Update_Final (hitCounter, golf_courseManager.instance.currentCourseNum);
		fiero_uiManager.instance.CompleteCourse_SetUI_Text_Hits (hitCounter);

		//totalParCounter += golf_courseManager.instance.courses [golf_courseManager.instance.currentCourseNum-1].par;
		//totalParCounter += parCounter;

		golf_playerRangeManager.instance.SetEnabled (false);

		fiero_uiManager.instance.CompleteRound_AddFinalCourseScore (hitCounter, golf_courseManager.instance.currentCourseNum, totalHitCounter, totalParCounter);
		if (currentGameMode == fiero_appManager.GameMode.Standard) {
			if (golf_courseManager.instance.AllCoursesFinished ()) {
		//		print ("completeRound");
				//SwitchGameState (AppState.completeRound);
                SwitchGameState(AppState.thankYou);
                if (playerPrefs_supportedDevs) {
                    Device.RequestStoreReview();
                    SwitchGameState(AppState.completeRound);
                }
                else {
                    SwitchGameState(AppState.supportTheDevs);
                    TUA_SupportUs.instance.SetSupportFlow(2);


                }

            } else {
		//		print ("completeCourse");
				if (currentAppState == AppState.maxHitCount) {
					//fiero_screenManager.instance.CloseCurrent ();
					SwitchGameState (fiero_appManager.instance.previousAppState);
					fiero_uiManager.instance.CompleteCourse_NextHole ();
				} else {
					SwitchGameState (AppState.completeCourse);
				}
                //golf_advertManager.instance.CheckIfShowAd(false, golf_courseManager.instance.currentCourseNum, (hitCounter - parCounter));

            }
        } else if (currentGameMode == fiero_appManager.GameMode.Practice) {
			SwitchGameState (AppState.completeCourse);
            //golf_advertManager.instance.CheckIfShowAd(true, golf_courseManager.instance.currentCourseNum, (hitCounter - parCounter));


        }
    }
	/*
	public void SkipToNextHole(){
		fiero_uiManager.instance.CompleteCourse_SetUI_Text_Hits (hitCounter);
		totalParCounter += parCounter;

		golf_playerRangeManager.instance.SetEnabled (false);
		fiero_uiManager.instance.CompleteRound_AddScore (hitCounter, golf_courseManager.instance.currentCourseNum, totalHitCounter, totalParCounter);
	}
	*/

	private void CalcScore(){
		
		//int calcScore = hitCounter - golf_courseManager.instance.courses [golf_courseManager.instance.currentCourseNum-1].par;
		int calcScore = hitCounter - parCounter;

		//Hole in One
		if (hitCounter == 1) {
			fiero_audioManager.instance.PlayScoreSound_HoleInOne ();
            //DISPLAY UI POPUP EFFECT HERE TOO;
            fiero_uiManager.instance.CompleteCourse_SetUI_Text_Feedback(
                golf_scoreFeedback.instance.GetRandomFeedbackForHoleInOne()
                );

        }
		//Other
		else {
			fiero_audioManager.instance.PlayScoreSound_Other (calcScore);
            //DISPLAY UI POPUP EFFECT HERE TOO;
            fiero_uiManager.instance.CompleteCourse_SetUI_Text_Feedback(
                golf_scoreFeedback.instance.GetRandomFeedbackForPar(calcScore)
                );
        }

    }

	void OnApplicationPause(bool pauseStatus)
	{
		golf_courseManager.instance.ResetAll ();
	}

	public void ResetRound(){
		totalHitCounter = 0;
		totalParCounter = 0;



		//Maybe this isn't the best reset?
		fiero_uiManager.instance.Mainmenu_Start ();
	}

    public void ConfirmSupportDevsDonation() {
        playerPrefs_supportedDevs = true;
        SwitchGameState(AppState.thankYou);
    }

    public void ConfirmTwitterShare() {
        SwitchGameState(AppState.thankYou);
    }

    //DEBUG TESTING
    public void Debug_SkipToCompleteRound(){
		FinishCourse ();
		for (int i = golf_courseManager.instance.currentCourseNum+1; i < 19; i++) {
			golf_courseManager.instance.currentCourseNum = i;
			SetCurrentCoursePar (golf_courseManager.instance.courses [i - 1].par);
			FinishCourse ();
		}
	//	golf_courseManager.instance.currentCourseNum = 18;
	//	FinishCourse ();
	}

    public void Debug_ChangeTimeScale(float newTimeScale) {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void Debug_TwitterShare()
    {

    }






    }
