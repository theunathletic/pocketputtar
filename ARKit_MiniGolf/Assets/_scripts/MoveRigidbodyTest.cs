using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRigidbodyTest : MonoBehaviour {

	public enum movementState { StartDelay,MoveForward,MidDelay,MoveBackwards,EndDelay};
	public movementState currentMovementState = movementState.StartDelay;

	public Rigidbody rb;

	public float startDelay;
	public float moveForwardTime;
	public float midDelay;
	public float moveBackwardsTime;
	public float endDelay;

	public Vector3 startingPos;
	public Vector3 endingPos;
    public float startingPositionProgress;
	private Vector3 setStartingPos;
    private Vector3 setEndingPos;

	public float currentPositionProgress;
	private float currentWaitingTime = 0;

	private bool shouldUpdate = true;

	void Awake(){
		rb = this.GetComponentInChildren<Rigidbody>();
		print ("RB AWAKE:" + this.transform.name);
		/*currentWaitingTime = startDelay;

		startingPos = transform.TransformPoint (startingPos);
		endingPos = transform.TransformPoint (endingPos);
		rb.MovePosition (startingPos);
		print ("RB AWAKE:" + this.transform.name);
		*/
		SetPositionAndMovmentPath ();
	}

	// Use this for initialization
	void Start () {
		/*
		startingPos = transform.TransformPoint (startingPos);
		endingPos = transform.TransformPoint (endingPos);
		rb.MovePosition (startingPos);

*/
		
	}
	//Testing only
	/*
	void Update(){
		if(Input.GetKeyUp (KeyCode.W)){
			print (transform.TransformPoint (startingPos));
		}

		if(Input.GetKeyUp (KeyCode.Q)){
			SetPositionAndMovmentPath();
		}
	}
	*/

	public void SetPositionAndMovmentPath(){
		
		rb.isKinematic = false;

		currentWaitingTime = startDelay;

		setStartingPos = transform.TransformPoint (startingPos);
		setEndingPos = transform.TransformPoint (endingPos);
		rb.MovePosition (setStartingPos);

        currentPositionProgress = startingPositionProgress;



        rb.isKinematic = true;
		currentMovementState = movementState.StartDelay;
	}

	public void ResetPositionAndMovmentPath(){
		rb.isKinematic = false;
		rb.velocity = Vector3.zero;
		shouldUpdate = false;
		setStartingPos = transform.TransformPoint (startingPos);
		//rb.MovePosition (setStartingPos);
		transform.position = setStartingPos;
        currentPositionProgress = startingPositionProgress;

        currentMovementState = movementState.StartDelay;
	}

	void FixedUpdate(){
		if (shouldUpdate) {

			switch (currentMovementState) {
			case movementState.StartDelay:
				currentWaitingTime -= Time.fixedDeltaTime;
				if (currentWaitingTime <= 0f) {
					//currentPositionProgress = 0f;
					currentMovementState = movementState.MoveForward;
				}

				break;

			case movementState.MoveForward:
				currentPositionProgress += (Time.fixedDeltaTime / moveForwardTime);
				rb.MovePosition (Vector3.Lerp (setStartingPos, setEndingPos, currentPositionProgress));
			/*
			if (Vector2.Distance (transform.position, setEndingPos) < 0.01f) {
				currentWaitingTime = midDelay;
				currentMovementState = movementState.MidDelay;

			}*/

				if (currentPositionProgress >= 1f) {
					rb.MovePosition (Vector3.Lerp (setStartingPos, setEndingPos, 1f));
					currentWaitingTime = midDelay;
					currentMovementState = movementState.MidDelay;
				}

				break;

			case movementState.MidDelay:
				currentWaitingTime -= Time.fixedDeltaTime;
				if (currentWaitingTime <= 0f) {
					currentPositionProgress = 0f;
					currentMovementState = movementState.MoveBackwards;
				}

				break;

			case movementState.MoveBackwards:
				currentPositionProgress += (Time.fixedDeltaTime / moveBackwardsTime);
				rb.MovePosition (Vector3.Lerp (setEndingPos, setStartingPos, currentPositionProgress));
			/*
			if (Vector2.Distance (transform.position, setStartingPos) < 0.01f) {
				currentWaitingTime = endDelay;
				currentMovementState = movementState.EndDelay;

			}*/

				if (currentPositionProgress >= 1f) {
					rb.MovePosition (Vector3.Lerp (setEndingPos, setStartingPos, 1f));
					currentWaitingTime = endDelay;
					currentMovementState = movementState.EndDelay;
				}

				break;

			case movementState.EndDelay:
				currentWaitingTime -= Time.fixedDeltaTime;
				if (currentWaitingTime <= 0f) {
					currentWaitingTime = startDelay;
                        currentPositionProgress = 0f;
                        currentMovementState = movementState.StartDelay;
				}

				break;

			}
		}
	}

}
