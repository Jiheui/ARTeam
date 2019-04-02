using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // This is called when the "back" button is clicked, 
    // then the main scene comes back
    // Wrote by Norton
    public void BackToMainScene(string scenename) {
        SceneManager.LoadScene(scenename);
    }
}
