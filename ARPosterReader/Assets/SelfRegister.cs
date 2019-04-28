using UnityEngine;
using UnityEngine.UI;
using Models;


public class SelfRegister : MonoBehaviour {

	public InputField _name;
	public InputField _birthday;
	public InputField _email;
	public InputField _password;
	public InputField _conform;


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
            }
            else
            {
                Debug.Log("Login Failed");
            }
        }
			
	}

		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
