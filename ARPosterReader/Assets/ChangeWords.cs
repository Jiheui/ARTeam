using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**this script is used for "favorite", 
    when first click the button, it changes to red, then add to favourite
    second click the botton, it turns into write, remove from favorite
    Take turns to happen this

    Author: Daniel
    Date: 11/04/2019
**/
public class ChangeWords : MonoBehaviour {

	public Text favouriteText = null;

	public int counter = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void changeText()
	{
		counter++;
		if (counter % 2 == 0) {
			favouriteText.text = "favourite";
		} else {
			favouriteText.text = "removed";
		}
	}
//	// click change to red
//	public void ChangeColorRed()
//	{
//		cube.GetComponent<MeshRenderer> ().material.color = Color.red;
//	}
//
//	// click change to white
//	public void ChangeColorWhite()
//	{
//		cube.GetComponent<MeshRenderer> ().material.color = Color.white;
//	}
}
