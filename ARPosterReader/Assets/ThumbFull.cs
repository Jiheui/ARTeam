using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThumbFull : MonoBehaviour {

	public Image img;
	public Button close;
	public Image thumbnail;

    // Use this for initialization
    void Start () {
			gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//This is called when the user clicks on thumbnail of a poster. It enlarges the poster to fullsize.
  void EnlargeImg (){
		img = thumbnail.GetComponent<Image>();
    }
}
