using UnityEngine;
using UnityEngine.UI;
using Models;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


public class SelfRegister : MonoBehaviour {

	public InputField _username;
	public InputField _birthday;
	public InputField _email;
	public InputField _password;
	public InputField _conform;

	public Text warning;

	public Button registerButton;

	public void Update()
	{
		bool empty = string.IsNullOrEmpty(_username.text) ||
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

	public void AddtoDatabase()
	{

		if (CheckPassword (_password, _conform)) 
		{
			/*
			 * This part check the complex password
			 */
			//			if ((_password.text.Length) <= 7) {
			////				bool yesWasClicked = EditorUtility.DisplayDialog("Title", "Content", "I Got it");
			////				Debug.Log("yesWasClicked="+yesWasClicked);
			//
			//				warning.text = "Password need to have more than 7 letters";
			//				_password.text = "";
			//				_conform.text = "";
			//                return;
			//			}

			if (_password.text.Contains (_username.text)) {
				//				bool yesWasClicked = EditorUtility.DisplayDialog("Title", "Content", "I Got it");
				//				Debug.Log("yesWasClicked="+yesWasClicked);	

				warning.text = "Password should not contain your username";
				_password.text = "";
				_conform.text = "";
				return;
			}
			//complex
			string aPatter = @"(?=.*[0-9])(?=.*[a-zA-Z])(?=.*[^a-zA-Z0-9]).{8,30}";
			//medium
			string bPatter = @"(?=.*[0-9])(?=.*[a-zA-Z]).{8,30}";

			Regex comlex = new Regex (aPatter);
			Regex medium = new Regex (bPatter);

			if (comlex.IsMatch(_password.text)) {
				Debug.Log ("Complex password");
			} else if (medium.IsMatch(_password.text)) {
				Debug.Log ("Medium password");
			}else{
				warning.text = "Password reqires to contain both number and alphabet";
				Debug.Log ("Password reqires to contain both number and alphabet. Special characters only can be choosed from ?,=,.,*. " +
					" Length is 8-30.");
				return;
			}

			User u = new User ();
			u.password = _password.text;
            u.email = _email.text;
			u.dob = _birthday.text;
			u.username = _username.text;
			u.CheckExist();
			if (u.exist)
			{
				Debug.Log("Username already exists");
				warning.text = "Username already exists";
			} 
			else
			{
				u.Create();
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
		}
		else
		{
			warning.text = "Password not the same";
			_password.text = "";
			_conform.text = "";
		}

	}

}
