using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Check_Login : MonoBehaviour {

	// Use this for initialization
	void Start () {

        string path = Application.persistentDataPath + "/User.txt";

        //Read the text from directly from the test.txt file
        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            string id = reader.ReadLine();

            if (id != null)
            {
                string name = reader.ReadLine();
                reader.Close();
                storeLoginSessionId.loginId = int.Parse(id);
                storeLoginSessionId.name = name;
                SceneManager.LoadScene("MainCameraScene");
            }
        }
            
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
