using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPS_counter : MonoBehaviour {

	public float updateInterval = 0.5f;

	private float accum = 0.0f;
	private int frames = 0;
	private float timeleft = 0.0f;

	private Text displayText;

	// Use this for initialization
	void Start () {
		displayText = GetComponentInChildren<Text> ();

		timeleft = updateInterval;
	}
	
	// Update is called once per frame
	void Update () {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;

		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			displayText.text = "FPS - " + (accum/frames).ToString("f2");
			timeleft = updateInterval;
			accum = 0.0f;
			frames = 0;
		}
	}
}
