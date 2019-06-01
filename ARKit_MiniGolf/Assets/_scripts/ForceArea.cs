using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceArea : MonoBehaviour {

	public Vector3 forceDirection;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Ball")
			print ("BALLIN");
		
			other.attachedRigidbody.AddForce(forceDirection);
	}
}
