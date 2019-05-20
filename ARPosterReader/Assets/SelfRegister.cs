﻿using UnityEngine;
using UnityEngine.UI;
using Models;
using UnityEngine.SceneManagement;
using UnityEditor;


public class SelfRegister : MonoBehaviour {

	public InputField _name;
	public InputField _birthday;
	public InputField _email;
	public InputField _password;
	public InputField _conform;

    public Text warning;

    public Button registerButton;

    public void Update()
    {
        bool empty = string.IsNullOrEmpty(_name.text) ||
            string.IsNullOrEmpty(_birthday.text) ||
            string.IsNullOrEmpty(_password.text) ||
            string.IsNullOrEmpty(_password.text) ||
            string.IsNullOrEmpty(_conform.text);

        registerButton.interactable = !empty;

        if (!empty)
        {
            warning.text = "";
        }
    }


    public static bool CheckPassword(InputField _password,InputField _conform)
	{
		string pw = _password.text;
		string cf = _conform.text;
		bool result = pw.Equals(cf);
		return result;
	}


	[MenuItem("Test/DisplayDialog")]
	public void AddtoDatabase()
	{
		
		if (CheckPassword (_password, _conform)) 
		{
			/*
			 * This part check the complex password
			 */
			if ((_password.text.Length) <= 7) {
//				bool yesWasClicked = EditorUtility.DisplayDialog("Title", "Content", "I Got it");
//				Debug.Log("yesWasClicked="+yesWasClicked);

				warning.text = "Password need to have more than 7 letters";
				_password.text = "";
				_conform.text = "";
			}

			if (_password.text.Contains (_name.text)) {
//				bool yesWasClicked = EditorUtility.DisplayDialog("Title", "Content", "I Got it");
//				Debug.Log("yesWasClicked="+yesWasClicked);	

				warning.text = "Password should not contain your username";
				_password.text = "";
				_conform.text = "";
			}
				
			
			User u = new User ();
			u.password = _password.text;
			u.username = _email.text;
			u.dob = _birthday.text;
			u.name = _name.text;
            u.Create();
            u.CheckExist();
            if (u.authenticated)
            {
                storeLoginSessionId.loginId = u.id;
                SceneManager.LoadScene("MainScene");
                if (string.IsNullOrEmpty(u.name))
                {
                    storeLoginSessionId.name = u.username;
                }
                else
                {
                    storeLoginSessionId.name = u.name;
                }

                storeLoginSessionId.email = u.email;
            }
            else
            {
                Debug.Log("Login Failed");
                warning.text = "Register Failed";
                _password.text = "";
                _conform.text = "";
            }
        }
        else
        {
            warning.text = "Password not the same";
            _password.text = "";
            _conform.text = "";
        }
			
	}

		
	// Use this for initialization
	void Start () {
		
	}


}
