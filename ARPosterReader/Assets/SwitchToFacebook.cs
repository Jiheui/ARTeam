using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
 * Move to facebook login page
 * 
 * Author: Daniel
 * Date: 13/04/2019
 */
public class SwitchToFacebook : MonoBehaviour {

	public void MoveToFaceBook(){
		Application.OpenURL("http://www.facebook.com");
	}



//	//move to next scene
//	public void MoveToFacebookScene() {
//		SceneManager.LoadScene(Scene);
//	}
}
