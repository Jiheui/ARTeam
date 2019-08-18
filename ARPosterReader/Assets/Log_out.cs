using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Models;
using System.IO;

public class Log_out : MonoBehaviour {


	public void logOut(string nextScene){
		if (storeLoginSessionId.loginId != -1) {
			storeLoginSessionId.loginId = -1;

			clearLocalFile ();

			SceneManager.LoadScene(nextScene);
		}
	}

	public void clearLocalFile (){
		string path = Application.persistentDataPath + "/User.txt";
		StreamWriter clearFile = File.CreateText (path);
		clearFile.Flush ();
		clearFile.Close ();
	}
}
