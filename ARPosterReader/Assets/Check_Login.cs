using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Check_Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		string path = "Assets/HiAR-Unity/Resources/User_Info/User.txt";

		//Read the text from directly from the test.txt file
		StreamReader reader = new StreamReader(path); 
		string id = reader.ReadLine ();
//		print (id);
		reader.Close();

		if (id != null) {
			storeLoginSessionId.loginId = int.Parse (id);
			SceneManager.LoadScene("HiARRobot");
//			print (storeLoginSessionId.loginId + " good");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
