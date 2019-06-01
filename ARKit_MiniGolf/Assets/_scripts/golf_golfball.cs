using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class golf_golfball : MonoBehaviour {

	public Transform directionGuide;
	private Rigidbody rb;

	private Image powerBar;

	private bool moving = false;
	public float ballMovingTheshold = 0.02f;
	public int ballMovingFramesTheshold = 5;
	private int ballMovingFramesCounter = 0;

	private bool closeCall = false;


	void Awake(){
		rb = GetComponent<Rigidbody>();
		powerBar = GetComponentInChildren<Image> ();
		UpdatePowerBar (0);

	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		SetDirectionGuide ();
		CheckMoving ();
	}

	public void SetCloseCall(bool wasClose){
		closeCall = wasClose;
	}

	public void SetCanMove(bool canMove){
		rb.isKinematic = canMove;
	}

	void SetDirectionGuide(){
		//forward = Vector3.Cross(up, cam.transform.right);
		//directionGuide.localRotation = Quaternion.LookRotation(Camera.main.transform.forward,Vector3.up);

		Vector3 forward;
		//Vector3 up = Vector3.up;
		Vector3 up = golf_courseManager.instance.upDir;
		/*
		 * if (Vector3.Angle(up, Camera.main.transform.forward) < 175)
		{
			Vector3 right = Vector3.Cross(up, Camera.main.transform.right).normalized;
			forward = Vector3.Cross(right, up).normalized;
		}
		else
		{
			// Normal is nearly parallel to camera look direction, the cross product would have too much
			// floating point error in it.
			forward = Vector3.Cross(up, -Camera.main.transform.forward);
		}
		*/
		Vector3 right = Vector3.Cross(up, Camera.main.transform.right).normalized;
		forward = Vector3.Cross(right, up).normalized;

		directionGuide.rotation = Quaternion.LookRotation (forward, up);
	}

	public void UpdatePowerBar(float amount){
		//powerBar.fillAmount = amount;
	}

	void CheckMoving(){
		if (moving) {
//			print ("BALL RB" + rb.velocity.magnitude);
			if (rb.velocity.magnitude < ballMovingTheshold) {
				ballMovingFramesCounter++;
				if (ballMovingFramesCounter > ballMovingFramesTheshold) {
					StoppedMoving ();
				}
			} else {
				ballMovingFramesCounter = 0;
			}
		}
	}

	private IEnumerator StartedMoving(float waitTime) {
		//while (true) {
			yield return new WaitForSeconds(waitTime);
			moving = true;
			//print("WaitAndPrint " + Time.time);
		//}
	}

	void StoppedMoving(){
		moving = false;
		golf_courseManager.instance.SetBallSpawnPoint (this.transform.position);
		if (fiero_appManager.instance.currentAppState == fiero_appManager.AppState.play) {
			golf_playerRangeManager.instance.SetEnabled (true);
			fiero_appManager.instance.BallStoppedCheck (); //check if max hit

			if (closeCall) {
				fiero_audioManager.instance.PlayCloseCall ();
			}
		}
		closeCall = false;

	}

	public void Hitball(float power){
		if (!rb) {
			rb = GetComponent<Rigidbody>();

		}
		/*
		rb.AddForce (Camera.main.transform.right * -1f * power);
		StartCoroutine(StartedMoving(0.5f));
*/
		//No chipping

		Vector3 forward;
		Vector3 up = golf_courseManager.instance.upDir;
		Vector3 right = Vector3.Cross(up, -Camera.main.transform.right).normalized;
		forward = Vector3.Cross(right, up).normalized;

		rb.AddForce (forward * power);
		StartCoroutine(StartedMoving(0.5f));



	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Hole") {
			SetCanMove (false);
			fiero_appManager.instance.FinishCourse ();
		}

		else if (other.tag == "DeathTrigger") {
			//golf_courseManager.instance.SpawnBall ();
			fiero_appManager.instance.BallOutOfBounds ();
		}

		else if(other.tag == "Wall"){
            //print("Wall hit force:" + rb.velocity.magnitude);
            if (rb.velocity.magnitude > 0.1) {
                float aValue = rb.velocity.magnitude;
                float normal = Mathf.InverseLerp(0.5f, 3.5f, aValue);
                float bValue = Mathf.Lerp(0.2f, 1f, normal);
                fiero_audioManager.instance.PlayWallHitSound(bValue);
            }


		}
	}


    //Not using this.
	void OnCollisionEnter(Collision collision) {
		if (collision.collider.tag == "Wall") {
			if (collision.relativeVelocity.magnitude > 0.1) {
                print("Wall hit force:" + collision.relativeVelocity.magnitude);
				fiero_audioManager.instance.PlayWallHitSound (0.5f);
			}
		}

	}


}
