using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Models;
using System.IO;

public class Log_out : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void logOut(string nextScene){
		if (storeLoginSessionId.loginId != -1) {
			storeLoginSessionId.loginId = -1;

            string path = Application.persistentDataPath + "/User.txt";
            StreamWriter clearFile = File.CreateText (path);
			clearFile.Flush ();
			clearFile.Close ();

			SceneManager.LoadScene(nextScene);
		}
	}
}
