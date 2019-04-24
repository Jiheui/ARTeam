using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfLogin : MonoBehaviour {


	public InputField email;
	public InputField password;

	public bool ConnecttoDatabase()
	{
		return true;
		
	}

	public void checkPassword()
	{
		print("Emial"+email.text);
		print("Password"+password.text);
	}


		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
