using UnityEngine;
using System;
using System.Collections;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad_Script : MonoBehaviour {

	public static SaveLoad_Script instance;


	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}

	public void SaveInt(string name, int value){
		PlayerPrefs.SetInt(name, value);
	}

	public int LoadInt(string name){
		return PlayerPrefs.GetInt(name);
	}

	public int LoadInt(string name, int defaultValue){
		return PlayerPrefs.GetInt(name, defaultValue);
	}

	public void SaveFloat(string name, float value){
		PlayerPrefs.SetFloat(name, value);
	}

	public float LoadFloat(string name){
		return PlayerPrefs.GetFloat(name);
	}

	public float LoadFloat(string name, float defaultValue){
		return PlayerPrefs.GetFloat(name,defaultValue);
	}


	public void SaveString(string name, string value){
		PlayerPrefs.SetString(name, value);
	}

	public string LoadString(string name){
		return PlayerPrefs.GetString(name);
	}

    //Use this for clearing PlayerPrefs
    public void DeleteKey(string name) {
        PlayerPrefs.DeleteKey(name);
    }

}

