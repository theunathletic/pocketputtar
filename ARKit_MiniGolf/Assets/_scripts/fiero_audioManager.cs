using UnityEngine;
using System.Collections;

public class fiero_audioManager : MonoBehaviour {

	public static fiero_audioManager instance;


	public AudioSource audioSource_sfx;

	[Header("UI Sounds")]
	public AudioClip buttonSound;
	public AudioClip coursePlacementSound;
	public AudioClip ballPlacementSound;



	[Header("Hit Sounds")]
	public AudioClip[] puttHitSounds;
	public AudioClip[] wallHitSounds;

	[Header("Hole Sounds")]
	public AudioClip[] holeSounds;
	public AudioClip closeCall;

	[Header("Scoring Sounds")]
	public AudioClip score_holeInOne;
	public AudioClip score_albatros; //par-3
	public AudioClip score_eagle; //par-2
	public AudioClip score_birdie; //par-1
	public AudioClip score_par; //par
	public AudioClip score_bogey; //par+1
	public AudioClip score_doubleBogey; //par+2
	public AudioClip score_overpar; //par<

	//Possibly use for charge up
	/*
	private bool loopingSonar = false;
	private float sonarSpeed = 0f;

	private float currentSonarCountdown = 0f;
	private float maxActivateSonarTime = 2f;
	*/


	void Awake () {
		instance = this;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//CheckSonar ();

	}
	/*
	private void CheckSonar(){
		if (loopingSonar) {
			currentSonarCountdown += Time.deltaTime;
			if (currentSonarCountdown >= (maxActivateSonarTime - (1.8f * sonarSpeed))) {
				currentSonarCountdown = 0f;
				PlaySonarSound();
			}
		}

		else {
			currentSonarCountdown = 0f;
		}
	}

	public void StartSonar(){
		loopingSonar = true;
	}

	public void StopSonar(){
		loopingSonar = false;
	}

	public void SetSonarSpeed(float speed){
		sonarSpeed = 1-speed;
	}
	*/

	public void PlayButtonSound(){
		audioSource_sfx.PlayOneShot (buttonSound);
    }

	public void PlayCoursePlacementSound(){
		audioSource_sfx.PlayOneShot (coursePlacementSound);
	}

	public void PlayBallPlacementSound(){
		audioSource_sfx.PlayOneShot (ballPlacementSound);
    }

	public void PlayPuttHitSound(){
		audioSource_sfx.PlayOneShot ( puttHitSounds[Random.Range(0,puttHitSounds.Length) ] );
    }

    public void PlayPuttHitSound(float vol)
    {
//        print("PlayPuttHitSound " + vol);
        audioSource_sfx.PlayOneShot(puttHitSounds[Random.Range(0, puttHitSounds.Length)], vol);
    }

    public void PlayWallHitSound(){
		audioSource_sfx.PlayOneShot ( wallHitSounds[Random.Range(0,wallHitSounds.Length) ] );
	}

    public void PlayWallHitSound(float vol)
    {
  //      print("PlayWallHitSound " + vol);
        //audioSource_sfx.volume = vol;
        audioSource_sfx.PlayOneShot(wallHitSounds[Random.Range(0, wallHitSounds.Length)], vol);
    }

    public void PlayHoleSound(){
		audioSource_sfx.PlayOneShot ( holeSounds[Random.Range(0,holeSounds.Length) ] );
	}

	public void PlayCloseCall(){
		audioSource_sfx.PlayOneShot (closeCall,0.6f);
	}

	public void PlayScoreSound_HoleInOne(){
		audioSource_sfx.PlayOneShot (score_holeInOne);
	}

	public void PlayScoreSound_Other(int score){

		AudioClip tempAudioClip;

		switch (score) {
		case -3:
			tempAudioClip = score_albatros;
			break;

		case -2:
			tempAudioClip = score_eagle;
			break;

		case -1:
			tempAudioClip = score_birdie;

			break;

		case 0:
			tempAudioClip = score_par;

			break;

		case 1:
			tempAudioClip = score_bogey;

			break;

		case 2:
			tempAudioClip = score_doubleBogey;

			break;

		default:
			tempAudioClip = score_overpar;

			break; 
		}
		//print ("HOLE CLIP: " + tempAudioClip.name);
		audioSource_sfx.PlayOneShot (tempAudioClip);

	}
		

}
