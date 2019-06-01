using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class golf_courseManager : MonoBehaviour {

	public static golf_courseManager instance;
	private TangoPointCloud m_pointCloud;

	/*AR KIT ----------------*/
	public float ARKitPlacement_maxRayDistance = 30.0f;
	public LayerMask ARKitPlacement_collisionLayer = 1 << 10;  //ARKitPlane layer
	public LayerMask ballPlacement_collisionLayer = 1 << 11; //BallSpawnPlane layer

	public bool ARKit_placementAvailable = false;
	public bool lockedPosition = false; //VERY IMPORTANT
	private bool hasUIChanged = false; //not used


	public GameObject coursePlacementObject; //preview course model


	private Vector3 coursePlacementPosition; //position.

	public GameObject course_parent;
	public GameObject course_offset; //flip this, centre point.

	public GameObject courseEnvironment;
	public GameObject courseScorebord;

	/* NEW PLACEMENT - FIND FLOOR, PLACE HOLE, GO TO TEE OFF / CONFIRM ------------------------*/


	[Header("NEW PLACEMENT - OPTION1")]
	public GameObject coursePlacementPreview_flag;
	public Animator coursePlacementPreview_flag_animator;
	public GameObject coursePlacementPreview_courseBase;

	public enum PlacementState{FindFloor,PlaceHole,PlaceStart,PlacedCourse,FindBallArea,PlaceBall,PlacementFinished};
	public PlacementState currentPlacementState = PlacementState.FindFloor;
	public PlacementState previousPlacementState;
	private int coursePlacementProgress = 0; // Use above instead of this.
	private bool canSeeFloor = false;
	private bool updatePlacementUI = false; //dont really use this. handled by PlacementState

	private Vector3 coursePlacementHolePosition;


	/*------------------------*/




	private bool coursePlacementCheck_floorPlane1 = false;
	private bool coursePlacementCheck_wallPlane1 = true;
	private float coursePlacementCheck_threshold = 10f;

	private Vector3 coursePlacementUp;
	private Vector3 coursePlacementForward;


	private bool ballPlaced = false;



	[Space(30)]
	public Transform coursePositionParent;

    public GameObject courseGroup_parent;
    public List<golf_parentCourseObject> courseGroups;
    public List<golf_courseObject> courses;

	public Transform ball_spawnpoint;
	public GameObject ball_prefab;

	public Vector3 upDir = Vector3.up;

	public int currentCourseNum = 1;

	private float setBallHeight = 0.125f;

	public GameObject[] positionMarkers;
	private int positionMarkerCounter = 0;
	public Animator positionMarker1Anim;

	public float courseLength = 2.7f;
	private float courseScale;





	void Awake () {
		instance = this;


		coursePlacementObject.SetActive (false);
		course_parent.SetActive (false);

		coursePlacementPosition = new Vector3 (0f, 0f, 0f);
	//	ResetMarkerPlacement ();


	}

	// Use this for initialization
	void Start () {
        GetAllCourseGroups();
        GetAllCourses ();

		m_pointCloud = FindObjectOfType<TangoPointCloud>();


	}
	
	// Update is called once per frame
	void Update () {
		if (fiero_appManager.instance.currentAppState == fiero_appManager.AppState.playCoursePlacement) {
			if (!lockedPosition) {
				//CheckCoursePlacementFloor (new Vector2(Screen.width / 2, (Screen.height / 2)) );
				//CheckCoursePlacement_ARKit_New(new Vector2(Screen.width / 2, (Screen.height / 2)) );
				CheckCoursePlacement_ARKit_New_Option1 (new Vector2 (Screen.width / 2, (Screen.height / 2)));

				//CheckCoursePlacementFloor (new Vector2(Screen.width / 2, (Screen.height / 2) - (Screen.height / 4) ));
				//CheckCoursePlacementWall (new Vector2(Screen.width / 2, (Screen.height / 2) + (Screen.height / 4) ));

				//ShowCorrectPlacementUI ();
				ShowCorrectPlacementUI_ARKit();
			//	ShowPlacementModel ();
				/*
				if (Input.touchCount == 1) {
					// Trigger place object function when single touch ended.
					Touch t = Input.GetTouch (0);
					if (t.phase == TouchPhase.Ended) {
						PlaceObject (t.position);
					}
				}
				*/
			}
		}

		if (fiero_appManager.instance.currentAppState == fiero_appManager.AppState.playBallPlacement) {
			//CheckBallPlacement ();
			CheckBallPlacement_New ();
		}
	}

    void GetAllCourseGroups()
    {
        courseGroups = new List<golf_parentCourseObject>();

        for (int i = 0; i < courseGroup_parent.transform.childCount; i++)
        {
            courseGroups.Add(courseGroup_parent.transform.GetChild(i).GetComponent<golf_parentCourseObject>());
        }

        //remove this.
        //LevelSelect_PopulateScroll ();

       // golf_levelSelect_ScrollManager.instance.PopulateScroll(courses);
        golf_courseSelect_ScrollManager.instance.PopulateScroll(courseGroups);
    }

    void GetAllCourses(){
		courses = new List<golf_courseObject>();

		for (int i = 0; i < course_parent.transform.childCount; i++) {
			courses.Add(course_parent.transform.GetChild(i).GetComponent<golf_courseObject>());
		}

		//remove this.
		//LevelSelect_PopulateScroll ();

		golf_levelSelect_ScrollManager.instance.PopulateScroll (courses);
	}
    /*
	public void LevelSelect_PopulateScroll( ){
		int i = 1;
		foreach (golf_courseObject course in courses) {
			fiero_uiManager.instance.LevelSelect_AddLevelButton (i, course.name, course.par);
			i++;
		}
	}*/

	public void SelectCourse(int courseNumber){
		currentCourseNum = courseNumber;

		foreach (golf_courseObject t in courses) {
			t.gameObject.SetActive (false);
		}

		courses [currentCourseNum-1].gameObject.SetActive (true);

		fiero_appManager.instance.SetCurrentCoursePar (courses [currentCourseNum - 1].par);
			

		fiero_uiManager.instance.Play_SetUI_Text_Course (currentCourseNum);
		fiero_uiManager.instance.Play_SetUI_Text_Par (courses [currentCourseNum - 1].par);
		fiero_uiManager.instance.CompleteCourse_SetUI_Text_Par (courses [currentCourseNum - 1].par);

		fiero_uiManager.instance.IngameScoreboard_SetUI_Text_Course (currentCourseNum);

        ToggleGuide_PuttThisWay(true);


    }

    public void ToggleGuide_PuttThisWay(bool isShown) {
        if (courses[currentCourseNum - 1].guide_puttThisWay != null)
        {
            courses[currentCourseNum - 1].guide_puttThisWay.SetActive(isShown);
        }
    }

    public void NextCourse(){
		currentCourseNum++;
		SelectCourse (currentCourseNum);
		CourseOrientation_Flip ();
	}

	public void CourseOrientation_Flip(){
		course_offset.transform.Rotate (0, 180, 0);
		UpdateMovingCourseObjects ();
	}

	private void CourseOrientation_Reset(){
		/*course_offset.transform.Rotate (0, 180, 0);*/
	}

	// Updates the sliders and bumpers when the course is positioned/repositioned/flipped.
	public void UpdateMovingCourseObjects(){
		Component[] movingObjects = courses[currentCourseNum-1].GetComponentsInChildren<MoveRigidbodyTest>();

		foreach (MoveRigidbodyTest obj in movingObjects) {
			obj.SetPositionAndMovmentPath ();
			//obj.ResetPositionAndMovmentPath ();
		//	print ("UpdateMovingCourseObject");
		}
	}

	public bool AllCoursesFinished(){
		//print ("AllCoursesFinished  " + currentCourseNum + " " + courses.Count);
		return currentCourseNum < courses.Count ? false : true;
	}

	//USING
	public void StartCoursePlacement(){
		//fiero_uiManager.instance.PlayCoursePlacement_MarkerFloorParent_Enabled (true);
		SwitchPlacementState(PlacementState.FindFloor);

	}

	//MOST RECENT
	public void CheckCoursePlacement_ARKit_New_Option1(Vector2 screenPosition){
		//print ("golf_courseManager: CheckCoursePlacement_ARKit_New_Option1");

		Ray ray = Camera.main.ScreenPointToRay (screenPosition);
		RaycastHit hit;
		Vector3 planeCenter;

		switch (currentPlacementState) {
			//FIND FLOOR (NEED AR FLOOR RAYCAST). Show the hole and flag here.
		case PlacementState.FindFloor:

			//If editor testing dont raycast because you can always see the floor.
			if (fiero_appManager.instance.editorTesting) {
				SwitchPlacementState (PlacementState.PlaceHole);
			} else {
				//Can now see the plane for placement and is in range
				if (Physics.Raycast (ray, out hit, ARKitPlacement_maxRayDistance, ARKitPlacement_collisionLayer)) {
					planeCenter = hit.point;
					coursePlacementPosition = planeCenter;

					SwitchPlacementState (PlacementState.PlaceHole);
					
				}
			}

				break;

			//PLACE HOLE (NEED AR FLOOR RAYCAST). Show the hole and flag here.
		case PlacementState.PlaceHole:

			//If editor testing dont raycast because you can always see the floor.
			if (fiero_appManager.instance.editorTesting) {
				coursePlacementPosition = new Vector3(0f,0,0);
				coursePositionParent.transform.position = coursePlacementPosition;
			} 
			else {
				//Can now see the plane for placement and is in range
				if (Physics.Raycast (ray, out hit, ARKitPlacement_maxRayDistance, ARKitPlacement_collisionLayer)) {
					planeCenter = hit.point;
					coursePlacementPosition = planeCenter;
					coursePositionParent.transform.position = coursePlacementPosition;
				} 
				else {
					SwitchPlacementState (PlacementState.FindFloor);

				}
			}

			break;

			//GO TO TEE OFF AND PLACE. Show course preview here. Will update orientation to current position.
			case PlacementState.PlaceStart:
				//courePositionParent.rotation = Quaternion.LookRotation (v3, hit.collider.transform.up);

			coursePositionParent.rotation = Quaternion.LookRotation ((new Vector3(Camera.main.transform.position.x,0,Camera.main.transform.position.z) - new Vector3(coursePositionParent.transform.position.x,0,coursePositionParent.transform.position.z)), Vector3.up);

			break;

			case PlacementState.PlacedCourse:

			break;
					

		}
	}

	// Use this for updating the placement ui and which objects to show
	public void SwitchPlacementState(PlacementState nextState){
		previousPlacementState = currentPlacementState;
		currentPlacementState = nextState;

		updatePlacementUI = true;
		switch (currentPlacementState) {

		case PlacementState.FindFloor:
			coursePlacementObject.SetActive (true);
			fiero_uiManager.instance.PlayCoursePlacement_ShowStep (0);
			coursePlacementPreview_flag.SetActive (false);
			coursePlacementPreview_courseBase.SetActive (false);
			break;

		case PlacementState.PlaceHole:
			fiero_uiManager.instance.PlayCoursePlacement_ShowStep (1);
			coursePlacementPreview_flag.SetActive (true);
			coursePlacementPreview_courseBase.SetActive (false);
			break;

		case PlacementState.PlaceStart:
			fiero_uiManager.instance.PlayCoursePlacement_ShowStep (2);
			coursePlacementPreview_flag.SetActive (true);
			coursePlacementPreview_courseBase.SetActive (true);
			break;

		case PlacementState.PlacedCourse:
			break;

		case PlacementState.FindBallArea:
			fiero_uiManager.instance.PlayBallPlacement_ShowStep (0);

			ballPlaced = false;
			ShowBallSpawnpoint (false);

			courses [currentCourseNum - 1].anim_placeBallGuide.Play ("play");
			break;

		case PlacementState.PlaceBall:
			fiero_uiManager.instance.PlayBallPlacement_ShowStep (1);
			ballPlaced = true;
			ShowBallSpawnpoint (true);
			break;

		case PlacementState.PlacementFinished:
			courses [currentCourseNum - 1].anim_placeBallGuide.Play ("stop");

			break;
		}
	}


	public void CheckCoursePlacement_ARKit_New(Vector2 screenPosition){
	//	print ("golf_courseManager: CheckCoursePlacement_ARKit_New");


		Ray ray = Camera.main.ScreenPointToRay (screenPosition);
		RaycastHit hit;

		Vector3 planeCenter;

		if (fiero_appManager.instance.editorTesting) {
		//	print ("golf_courseManager: CheckCoursePlacement_ARKit_New: editor Test");
			ARKit_placementAvailable = true;
			hasUIChanged = true;
		//	Debug.Log ("courseManager - ARKit_placementAvailable: TRUE");
		}
		else {

			//we'll try to hit one of the plane collider gameobjects that were generated by the plugin
			//effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
			if (Physics.Raycast (ray, out hit, ARKitPlacement_maxRayDistance, ARKitPlacement_collisionLayer)) {


				//we're going to get the position from the contact point
				//m_HitTransform.position = hit.point;
				//			Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", hit.point.x, hit.point.y, hit.point.z));
				planeCenter = hit.point;
				//and the rotation from the transform of the plane collider
				//m_HitTransform.rotation = hit.transform.rotation;

				//CreateMoveBall (hit.point);



				if (!ARKit_placementAvailable) {
					ARKit_placementAvailable = true;
					hasUIChanged = true;
				//	Debug.Log ("courseManager - ARKit_placementAvailable: TRUE");
				} 

			//Can now see the plane for placement and is in range.
			else {
					coursePlacementPosition = planeCenter;

				//	Debug.Log ("courseManager - update course position:" + coursePlacementPosition);
					//float dist = Vector3.Distance (positionMarkers [0].transform.position, positionMarkers [1].transform.position);
					//courseScale = dist / courseLength;
					//coursePositionParent.localScale = new Vector3(courseScale,courseScale,courseScale);
					coursePositionParent.transform.position = coursePlacementPosition;
					//coursePositionParent.rotation = Quaternion.LookRotation ((positionMarkers [0].transform.position - positionMarkers [1].transform.position), Vector3.up);
					//coursePositionParent.rotation = Quaternion.LookRotation (-Camera.main.transform.forward, Vector3.up);

					Vector3 v3 = Camera.main.transform.forward;
					v3.y = 0;
					v3.Normalize ();

					coursePositionParent.rotation = Quaternion.LookRotation (v3, hit.collider.transform.up);
					//MATH ME
				
				//	Debug.Log ("courseManager - update course rotation:" + coursePositionParent.rotation);
				}


			} else {
				if (ARKit_placementAvailable) {
					ARKit_placementAvailable = false;
					hasUIChanged = true;
				//	Debug.Log ("courseManager - ARKit_placementAvailable: FALSE");
				}

				return;
			}
		}
	}
	//No longer needed.
	public void ShowCorrectPlacementUI_ARKit(){
		if (hasUIChanged) {
			if (!ARKit_placementAvailable) {
				fiero_uiManager.instance.PlayCoursePlacement_ShowStep (0);

				//hide course outline preview
				coursePlacementObject.SetActive (false);
			//	Debug.Log ("courseManager - ShowCorrectPlacementUI_ARKit - course preview: HIDE");
			} else {
				fiero_uiManager.instance.PlayCoursePlacement_ShowStep (1);

				//show course outline preview
				coursePlacementObject.SetActive (true);
			//	Debug.Log ("courseManager - ShowCorrectPlacementUI_ARKit - course preview: SHOW");
			}

			hasUIChanged = false;
		}
	}

	public void ConfirmHolePlacement_ARKit(){
		SwitchPlacementState (PlacementState.PlaceStart);
	}

	//THIS IS CURRENTLY USED
	public void ConfirmCoursePlacement_ARKit(){
//		Debug.Log ("courseManager - ConfirmPlacement");
		/*fiero_audioManager.instance.PlayCoursePlacementSound ();*/

		courseScale = 1.0f;

		golf_playerRangeManager.instance.SetRangeIndicatorDistance (courseScale);
		ball_spawnpoint.GetComponentInChildren<Projector> ().orthographicSize = 0.02f * courseScale;
		coursePositionParent.localScale = new Vector3(courseScale,courseScale,courseScale);

		SetLockedPosition (true);

		courseEnvironment.SetActive (true);

		//Check to see if the ingame scoreboard needs to be shown. Only happens on first placement.
		if (fiero_appManager.instance.currentGameMode == fiero_appManager.GameMode.Standard) {
			courseScorebord.SetActive (true);
		} else {
			courseScorebord.SetActive (false);
		}

		coursePlacementObject.SetActive (false);
		course_parent.SetActive (true);

		fiero_appManager.instance.ConfirmCoursePlacement ();
	}

	public void CheckCoursePlacement_ARKit(Vector2 screenPosition){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		Vector3 planeCenter;


		//we'll try to hit one of the plane collider gameobjects that were generated by the plugin
		//effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
		if (Physics.Raycast (ray, out hit, ARKitPlacement_maxRayDistance, ARKitPlacement_collisionLayer)) {

//			Debug.Log ("CAN PLACE: YES");
			//we're going to get the position from the contact point
			//m_HitTransform.position = hit.point;
//			Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", hit.point.x, hit.point.y, hit.point.z));
			planeCenter = hit.point;
			//and the rotation from the transform of the plane collider
			//m_HitTransform.rotation = hit.transform.rotation;

			//CreateMoveBall (hit.point);

			if (positionMarkerCounter == 1) {
				if ((Vector3.Distance (positionMarkers [0].transform.position, planeCenter) > 2.6f) &&
					(Vector3.Distance (positionMarkers [0].transform.position, planeCenter) < 2.8f)) {

					if (!coursePlacementCheck_floorPlane1) {
						hasUIChanged = true;
					}
					coursePlacementCheck_floorPlane1 = true;
					coursePlacementPosition = planeCenter;


				} else {
					if (coursePlacementCheck_floorPlane1) {
						hasUIChanged = true;
					}
					coursePlacementCheck_floorPlane1 = false;

				}
			} 
			else {
				if (!coursePlacementCheck_floorPlane1) {
					hasUIChanged = true;
				}
				coursePlacementCheck_floorPlane1 = true;
				coursePlacementPosition = planeCenter;
			}

				
		} 
		else {
			coursePlacementCheck_floorPlane1 = false;
		//	Debug.Log("cannot find floor.");

			return;
		}
	}

	public void CheckCoursePlacementFloor(Vector2 screenPosition){ 
		Camera cam = Camera.main;
		Vector3 planeCenter;
		Plane plane;
		if (!m_pointCloud.FindPlane(cam, screenPosition, out planeCenter, out plane))
		{
			coursePlacementCheck_floorPlane1 = false;
		//	Debug.Log("cannot find floor.");

			return;
		}
		//If mostly upward facing
		if (Vector3.Angle (plane.normal, Vector3.up) < coursePlacementCheck_threshold) {
			coursePlacementUp = plane.normal;
			/*
			coursePlacementCheck_floorPlane1 = true;
		//	coursePlacementPosition.x = planeCenter.x;
		//	coursePlacementPosition.y = planeCenter.y;
		//	coursePlacementPosition.z = planeCenter.z;

			coursePlacementPosition = planeCenter;
*/
			if (positionMarkerCounter == 1) {
				if ((Vector3.Distance (positionMarkers [0].transform.position, planeCenter) > 2.6f) &&
				    (Vector3.Distance (positionMarkers [0].transform.position, planeCenter) < 2.8f)) {

					if (!coursePlacementCheck_floorPlane1) {
						hasUIChanged = true;
					}
					coursePlacementCheck_floorPlane1 = true;
					coursePlacementPosition = planeCenter;
				} else {
					if (coursePlacementCheck_floorPlane1) {
						hasUIChanged = true;
					}
					coursePlacementCheck_floorPlane1 = false;

				}
			} 
			else {
				if (!coursePlacementCheck_floorPlane1) {
					hasUIChanged = true;
				}
				coursePlacementCheck_floorPlane1 = true;
				coursePlacementPosition = planeCenter;
			}

		} else {
			if (coursePlacementCheck_floorPlane1) {
				hasUIChanged = true;
			}
			coursePlacementCheck_floorPlane1 = false;
		}
	}

	public void CheckCoursePlacementWall(Vector2 screenPosition){ 
		Camera cam = Camera.main;
		Vector3 planeCenter;
		Plane plane;

		if (!m_pointCloud.FindPlane(cam, screenPosition, out planeCenter, out plane))
		{
			coursePlacementCheck_wallPlane1 = false;
		//	Debug.Log("cannot find wall.");

			return;
		}
		//If mostly upward facing

		/*
		Vector3 up = plane.normal;
		Vector3 right = Vector3.Cross(plane.normal, cam.transform.forward).normalized;
		Vector3 forward = Vector3.Cross(right, plane.normal).normalized;
		*/

		//fiero_DebugScreenText.instance.SetDebugText ((Vector3.Angle (plane.normal, Vector3.up)).ToString ());

		float angleCalc = Vector3.Angle (plane.normal, Vector3.up) - 90.0f;

		if ( (angleCalc < coursePlacementCheck_threshold) && (angleCalc> -coursePlacementCheck_threshold) ) {
			coursePlacementForward = plane.normal;

			fiero_DebugScreenText.instance.SetDebugText (coursePlacementPosition.ToString () + "  " + planeCenter.ToString() );


			coursePlacementCheck_wallPlane1 = true;
			coursePlacementPosition.z = planeCenter.z;



		} else {
			coursePlacementCheck_wallPlane1 = false;
		}
	}
	//No longer needed
	void ShowCorrectPlacementUI(){
		//this should check for a change in either coursePlacementCheck_floorPlane1 or positionMarkerCounter. before actually doing the rest. if no chnge, dont do anything.

		if (hasUIChanged) {

			if (positionMarkerCounter < 2) {
				/*
			fiero_uiManager.instance.PlayCoursePlacement_ToggleInstructions(!coursePlacementCheck_floorPlane1, positionMarkerCounter);
			fiero_uiManager.instance.PlayCoursePlacement_ToggleButtons (coursePlacementCheck_floorPlane1, positionMarkerCounter);
			*/

				if (positionMarkerCounter == 0) {
					if (!coursePlacementCheck_floorPlane1) {
						fiero_uiManager.instance.PlayCoursePlacement_ShowStep (0);
					} else {
						fiero_uiManager.instance.PlayCoursePlacement_ShowStep (1);
					}
				} else if (positionMarkerCounter == 1) {
					if (!coursePlacementCheck_floorPlane1) {
						fiero_uiManager.instance.PlayCoursePlacement_ShowStep (2);
					} else {
						fiero_uiManager.instance.PlayCoursePlacement_ShowStep (3);
					}
				}

			} else {
				//fiero_uiManager.instance.PlayCoursePlacement_ToggleFoundParent (true);
			}


			fiero_uiManager.instance.PlayCoursePlacement_ToggleMarkerFloor (coursePlacementCheck_floorPlane1);
			hasUIChanged = false;
		}
		/*
	//	fiero_uiManager.instance.PlayCoursePlacement_ToggleMarkerWall (coursePlacementCheck_wallPlane1);

		if (coursePlacementCheck_floorPlane1 && coursePlacementCheck_wallPlane1) {
			fiero_uiManager.instance.PlayCoursePlacement_ToggleFoundParent (true);
		} else {
			fiero_uiManager.instance.PlayCoursePlacement_ToggleFoundParent (false);

		}
		*/

	}

	public void ConfirmMarker(){
		/*
		fiero_uiManager.instance.PlayCoursePlacement_ToggleInstructions(false, positionMarkerCounter);
		fiero_uiManager.instance.PlayCoursePlacement_ToggleButtons (false, positionMarkerCounter);
		*/

		positionMarkers [positionMarkerCounter].transform.position = coursePlacementPosition;

		positionMarkers [positionMarkerCounter].SetActive (true);


		positionMarkerCounter++;
		hasUIChanged = true;

		if (positionMarkerCounter == 1) {
			positionMarker1Anim.SetTrigger ("trigger");

		}

		if(positionMarkerCounter >= 2){
			//fiero_uiManager.instance.PlayCoursePlacement_MarkerFloorParent_Enabled (false);

			SetLockedPosition(true);
			//fiero_uiManager.instance.PlayCoursePlacement_ToggleFoundParent (true);

			positionMarkers [0].SetActive (false);
			positionMarkers [1].SetActive (false);

			//coursePlacementPosition = (positionMarkers [0].transform.position + positionMarkers [1].transform.position) / 2f;

			coursePlacementPosition = positionMarkers [0].transform.position;

			ShowPlacementModel ();

						//finish here.
			ConfirmPlacement();
		}


	}

	public void ResetMarkerPlacement(){

		positionMarker1Anim.SetTrigger ("reset");

		//fiero_uiManager.instance.PlayCoursePlacement_MarkerFloorParent_Enabled (true);
				SetLockedPosition(false);

		positionMarkers [0].SetActive (false);
		positionMarkers [1].SetActive (false);

		coursePlacementObject.SetActive (false);

		fiero_uiManager.instance.PlayCoursePlacement_ShowStep (0);

		/*
		fiero_uiManager.instance.PlayCoursePlacement_ToggleInstructions(false, 0);
			fiero_uiManager.instance.PlayCoursePlacement_ToggleButtons (false, 0);
				fiero_uiManager.instance.PlayCoursePlacement_ToggleInstructions(false, 1);
					fiero_uiManager.instance.PlayCoursePlacement_ToggleButtons (false, 1);
		fiero_uiManager.instance.PlayCoursePlacement_ToggleFoundParent (false);
		*/
		positionMarkerCounter = 0;
	}

	void ShowPlacementModel(){


		//fiero_uiManager.instance.PlayCoursePlacement_MarkerFloorParent_Enabled (false);

		coursePlacementObject.SetActive(true);

		if(coursePlacementCheck_floorPlane1 && coursePlacementCheck_wallPlane1){
			coursePlacementObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;

			Vector3 up = Vector3.up;
			Vector3 right = Vector3.Cross(Vector3.up, -coursePlacementForward).normalized;
			Vector3 forward = Vector3.Cross(right, Vector3.up).normalized;

			coursePositionParent.position = coursePlacementPosition;
			//coursePositionParent.rotation = Quaternion.LookRotation (forward, up);

			//coursePositionParent.rotation = Quaternion.LookRotation (-coursePlacementForward, Vector3.up);
			//coursePositionParent.rotation = Quaternion.LookRotation (forward, Vector3.up);

			float dist = Vector3.Distance (positionMarkers [0].transform.position, positionMarkers [1].transform.position);
			courseScale = dist / courseLength;

			coursePositionParent.localScale = new Vector3(courseScale,courseScale,courseScale);
				
			//coursePositionParent.rotation = Quaternion.LookRotation ((positionMarkers [0].transform.position - positionMarkers [1].transform.position), Vector3.up);
			coursePositionParent.rotation = Quaternion.LookRotation ((new Vector3(positionMarkers [0].transform.position.x,0,positionMarkers [0].transform.position.z) - new Vector3(positionMarkers [1].transform.position.x,0,positionMarkers [1].transform.position.z)), Vector3.up);





		}
		else{
			coursePlacementObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
			//coursePlacementObject.SetActive(false);
		}

	}

	//Dont think this gets used at all
	public void PlaceObject(Vector2 touchPosition)
	{
		

		// Find the plane.
		Camera cam = Camera.main;
		Vector3 planeCenter;
		Plane plane;
		if (!m_pointCloud.FindPlane(cam, touchPosition, out planeCenter, out plane))
		{
		//	Debug.Log("cannot find plane.");
			return;
		}

		// Place kitten on the surface, and make it always face the camera.
		if (Vector3.Angle(plane.normal, Vector3.up) < 30.0f)
		{/*
			SetGravityDirection (-plane.normal);
			upDir = plane.normal;
	
			Vector3 up = plane.normal;
			Vector3 right = Vector3.Cross(plane.normal, cam.transform.forward).normalized;
			Vector3 forward = Vector3.Cross(right, plane.normal).normalized;
			//Instantiate(course_outline, planeCenter, Quaternion.LookRotation(forward, up));

			coursePositionParent.position = planeCenter;
			coursePositionParent.rotation = Quaternion.LookRotation (forward, up);
			*/

			Vector3 up = Vector3.up;
			Vector3 right = Vector3.Cross(Vector3.up, cam.transform.forward).normalized;
			Vector3 forward = Vector3.Cross(right, Vector3.up).normalized;
			//Instantiate(course_outline, planeCenter, Quaternion.LookRotation(forward, up));

			coursePositionParent.position = planeCenter;
			coursePositionParent.rotation = Quaternion.LookRotation (forward, up);
		}
		else
		{
		//	Debug.Log("surface is too steep for kitten to stand on.");
		}
	}

	private void SetGravityDirection(Vector3 dir){
		float gMag = Physics.gravity.magnitude;
	//	print ("GRAVITY" + gMag);
		Physics.gravity = dir * gMag;
	}


	public void ConfirmPlacement(){
	//	print ("golf:coursemanager ConfirmPlacement ");
		//Just for testing
		//courseScale = 0.3f;
		fiero_audioManager.instance.PlayCoursePlacementSound ();

		#if UNITY_EDITOR
		courseScale = 1.0f;
		coursePlacementCheck_floorPlane1 = true;
		coursePlacementCheck_wallPlane1 = true;
		positionMarkers [0].transform.position = new Vector3(0,0,5);
		positionMarkers [1].transform.position = new Vector3(5,0,0);

		ShowPlacementModel();
		#endif

		courseScale = 1.0f;

		golf_playerRangeManager.instance.SetRangeIndicatorDistance (courseScale);
		ball_spawnpoint.GetComponentInChildren<Projector> ().orthographicSize = 0.02f * courseScale;
		coursePositionParent.localScale = new Vector3(courseScale,courseScale,courseScale);

		SetLockedPosition (true);

		courseEnvironment.SetActive (true);
		courseScorebord.SetActive (true);


		coursePlacementObject.SetActive (false);
		course_parent.SetActive (true);

		/*
		fiero_uiManager.instance.PlayCoursePlacement_ToggleInstructions(false, 0);
		fiero_uiManager.instance.PlayCoursePlacement_ToggleButtons (false, 0);
		fiero_uiManager.instance.PlayCoursePlacement_ToggleInstructions(false, 1);
		fiero_uiManager.instance.PlayCoursePlacement_ToggleButtons (false, 1);
		fiero_uiManager.instance.PlayCoursePlacement_ToggleFoundParent (false);
		*/

		//fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.play);
		//SpawnBall();
		//fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.ballPlacement);

		fiero_appManager.instance.ConfirmCoursePlacement ();

	}

	//---------------------------------------------------------------
	//Ball placement
	//---------------------------------------------------------------

	//USED
	public void StartBallPlacement(){
		ballPlaced = false;
		//ShowBallSpawnpoint (true);

		SwitchPlacementState (PlacementState.FindBallArea);

	}

	//USED OLD
	public void CheckBallPlacement(){
		//print ("CheckBallPlacement");
		Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000)) { 
			if (hit.collider.tag == "BallSpawnArea") {
				//print ("CheckBallPlacement2");

				if (!ballPlaced) {
					//print ("CheckBallPlacement3");

					ballPlaced = true;
				}
				SetBallSpawnPoint (new Vector3 (hit.point.x, hit.point.y + 0.025f, hit.point.z));
				//ball_spawnpoint.transform.position = new Vector3 (hit.point.x,hit.point.y+0.025f, hit.point.z);
				//fiero_appManager.instance.currentBall.transform.position = new Vector3 (hit.point.x, setBallHeight, hit.point.z);
			}
			Debug.DrawLine (ray.origin, hit.point);
		}

	}

	public void CheckBallPlacement_New(){
		Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
		RaycastHit hit;

		switch (currentPlacementState) {
			case PlacementState.FindBallArea:
				if (Physics.Raycast (ray, out hit, 1000, ballPlacement_collisionLayer)) {
					if (hit.collider.tag == "BallSpawnArea") {
						SwitchPlacementState (PlacementState.PlaceBall);
					}
				}
				break;

		case PlacementState.PlaceBall:
			if (Physics.Raycast (ray, out hit, 1000, ballPlacement_collisionLayer)) { 
				if (hit.collider.tag == "BallSpawnArea") {
					SetBallSpawnPoint (new Vector3 (hit.point.x, hit.point.y + 0.025f, hit.point.z));
				} else {
					SwitchPlacementState (PlacementState.FindBallArea);
				}
			} else {
				SwitchPlacementState (PlacementState.FindBallArea);
			}
				break;

		}
	}


	//USED
	public void SetBallSpawnPoint(Vector3 newSpawnPoint){
		ball_spawnpoint.transform.position = newSpawnPoint;
	}
	//USED - can move all of this into SwitchPlacementState
	public void ConfirmBallPlacement(){
		if (ballPlaced) {
			SwitchPlacementState (PlacementState.PlacementFinished);
			golf_playerRangeManager.instance.SetEnabled (true);
			ShowBallSpawnpoint (false);
			fiero_appManager.instance.SwitchGameState (fiero_appManager.AppState.play);
		}
	}
	//USED
	public void SpawnBall(){
		//fiero_appManager.instance.ResetCourse ();
		fiero_appManager.instance.DeleteCurrentBall ();

		GameObject ball = Instantiate (ball_prefab, ball_spawnpoint.transform.position, Quaternion.identity);
		float ballScale = ball.transform.localScale.x * courseScale;

		ball.transform.localScale = new Vector3(ballScale,ballScale, ballScale);

		fiero_appManager.instance.SetCurrentBall( ball.GetComponent<golf_golfball>());
		golf_playerRangeManager.instance.SetBallTranform (ball.transform);
		golf_playerRangeManager.instance.SetEnabled (true);

	}
	//USED
	public void ShowBallSpawnpoint(bool show){
		ball_spawnpoint.gameObject.SetActive (show);
	}
	//USED
	public void SetLockedPosition(bool locked){
		lockedPosition = locked;
	}
	//USED
	public bool GetLockedPosition(){
		return lockedPosition;
	}
	//USED
	public void ResetAll(){
//		print ("RESETAll");
		//golf_playerRangeManager.instance.SetEnabled (false);



		fiero_appManager.instance.DeleteCurrentBall ();


		SetLockedPosition (false);

		ballPlaced = false;

		courseEnvironment.SetActive (false);
		courseScorebord.SetActive (false);

		course_parent.SetActive (false);

		coursePlacementPosition = new Vector3 (0f, 0f, 0f);
		ResetMarkerPlacement ();
	}
}
