using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Like_Button : MonoBehaviour {

	public Sprite red_heart;
	public Sprite white_heart;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void unlikePoster(){
		if (gameObject.GetComponent<Image> ().sprite == red_heart) {
			gameObject.GetComponent<Image> ().sprite = white_heart;

		} else if (gameObject.GetComponent<Image> ().sprite == white_heart) {
			gameObject.GetComponent<Image> ().sprite = red_heart;
		} else {
			print ("error!");
		}



	}

}
