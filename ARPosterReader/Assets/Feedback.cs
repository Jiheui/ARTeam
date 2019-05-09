using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;

public class Feedback : MonoBehaviour {
	public InputField _feedback;




	public void AddtoDatabase()
	{
//		User u = new User ();
//		string email = u.email;
//		int id = u.id;
//		string username = u.username;

		string f = _feedback.text;
		Debug.Log ("hello world" + f);



		// u.SendFeedback();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
