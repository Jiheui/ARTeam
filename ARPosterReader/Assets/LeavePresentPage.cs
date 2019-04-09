using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeavePresentPage : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // This is called when the "scan poster" button is clicked, 
    // then the camera view shows up waiting for recognisation
    // Wrote by Norton
    public void MoveToNextScene(string nextScene) {
        SceneManager.LoadScene(nextScene);
    }
}
