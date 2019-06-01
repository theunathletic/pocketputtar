using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRigidBody : MonoBehaviour {

	public Rigidbody rb;
	public Vector3 rotateVelocity;

	void Awake(){
		rb = GetComponent<Rigidbody>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		//Quaternion delta = Quaternion.Euler(

		Quaternion deltaRotation = Quaternion.Euler(rotateVelocity * Time.deltaTime);
		rb.MoveRotation(rb.rotation * deltaRotation);

	}
}
