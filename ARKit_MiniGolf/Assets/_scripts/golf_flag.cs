using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class golf_flag : MonoBehaviour {

	public Transform flagObject;

	public float maxHeight;

	public SphereCollider sc;

	void Awake(){
		sc = this.GetComponent<SphereCollider> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other) {

		if (other.tag == "Ball") {

			float newHeight = Mathf.Lerp (maxHeight, 0, (Vector3.Distance (this.transform.position, fiero_appManager.instance.currentBall.transform.position) / sc.radius));

			flagObject.transform.localPosition = new Vector3 (0, newHeight, 0);
		}

		/*if (other.attachedRigidbody)
			other.attachedRigidbody.AddForce(Vector3.up * 10);
			*/

	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Ball") {
			other.GetComponent<golf_golfball> ().SetCloseCall (true);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Ball") {
			other.GetComponent<golf_golfball> ().SetCloseCall (false);

		}
	}
}
