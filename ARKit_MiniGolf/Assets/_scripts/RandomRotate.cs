using UnityEngine;
using System.Collections;

public class RandomRotate : MonoBehaviour {



	public float speedX = 5f;
	public float speedY = 5f;
	public float speedZ = 5f;

	public bool useVariationValue = false;
	public float speedXVariation = 5f;
	public float speedYVariation = 5f;
	public float speedZVariation = 5f;


	public bool rotateX;
	public bool rotateY;
	public bool rotateZ;

	// Use this for initialization
	void Start () {
		if (useVariationValue) {
			speedX = Random.Range (speedX - speedXVariation, speedX + speedXVariation);
			speedY = Random.Range (speedY - speedYVariation, speedY + speedYVariation);
			speedZ = Random.Range (speedZ - speedZVariation, speedZ + speedZVariation);

		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (rotateX) {
			transform.Rotate(Vector3.up, speedX * Time.deltaTime);
		}

		if (rotateY) {
			transform.Rotate(Vector3.right, speedY * Time.deltaTime);
		}

		if (rotateZ) {
			transform.Rotate(Vector3.forward, speedZ * Time.deltaTime);
		}
	}
}
