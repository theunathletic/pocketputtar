using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwitterKit.Unity;
using TwitterKit.Unity.Settings;
using System;

public class TUA_SupportUs : MonoBehaviour {

    public static TUA_SupportUs instance;

    public enum SupportFlow { mainmenu, completeRound, endOf18Holes};
    public SupportFlow currentSupportFlow = SupportFlow.mainmenu;
    
    void Awake()
    {
        instance = this;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSupportFlow(int flowInt) {
        // currentSupportFlow = flow;
        currentSupportFlow = (SupportFlow)flowInt;
    }

    public void SupportedDevsConfirmed() {
        print("SupportedDevsConfirmed");
        Save_SupportedDevsStatus(true);
        fiero_appManager.instance.ConfirmSupportDevsDonation();
    }

    public void Save_SupportedDevsStatus(bool hasSupported)
    {
        int testInt = hasSupported ? 1 : 0;
        SaveLoad_Script.instance.SaveInt("supportedDevs", testInt);
    }

    public bool Load_SupportedDevsStatus()
    {
        bool returnValue = false;
        if (SaveLoad_Script.instance.LoadInt("supportedDevs",0) == 1) {
            returnValue = true;
        }

        return returnValue;
    }

    public void Delete_Load_SupportedDevsStatus() {
        SaveLoad_Script.instance.DeleteKey("supportedDevs");
    }

    public void startLogin()
    {
        UnityEngine.Debug.Log("startLogin()");
        // To set API key navigate to tools->Twitter Kit
        Twitter.Init();

        Twitter.LogIn(LoginCompleteWithCompose, (ApiError error) => {
            UnityEngine.Debug.Log(error.message);
        });
    }

    public void LoginCompleteWithCompose(TwitterSession session)
    {
        UnityEngine.Debug.Log(Application.streamingAssetsPath + "/TwitterLogo.png");
        string twitterMessage = "I've just played 18 holes of #PocketPuttAR, you should too! How many hole-in-ones do you think you can get?";
        string imageUri = Application.streamingAssetsPath + "/TwitterLogo.png";
        Twitter.Compose(session, imageUri, twitterMessage, new string[] { "#minigolf", "#AR"},
            (string tweetId) => { TweetSuccess(); },
            (ApiError error) => { UnityEngine.Debug.Log("Tweet Failed " + error.message); },
            () => { Debug.Log("Compose cancelled"); }
         );
    }
    /*
    public void LoginCompleteWithCompose(TwitterSession session)
    {
        ScreenCapture.CaptureScreenshot("Screenshot.png");
        UnityEngine.Debug.Log("Screenshot location=" + Application.persistentDataPath + "/Screenshot.png");
        string imageUri = "file://" + Application.persistentDataPath + "/Screenshot.png";
        Twitter.Compose(session, imageUri, "Welcome to", new string[] { "#TwitterKitUnity" },
            (string tweetId) => { UnityEngine.Debug.Log("Tweet Success, tweetId=" + tweetId); },
            (ApiError error) => { UnityEngine.Debug.Log("Tweet Failed " + error.message); },
            () => { Debug.Log("Compose cancelled"); }
         );
    }*/

    public void TweetSuccess() {
     //   UnityEngine.Debug.Log("Tweet Success, tweetId=" + tweetId);
        fiero_appManager.instance.ConfirmTwitterShare();
    }
}
