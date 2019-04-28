using UnityEngine;
using UnityEngine.UI;
using Models;
public static class storeLoginSessionId{
    public static int loginId = -1;
}
public class SelfLogin : MonoBehaviour {


	public InputField _email;
	public InputField _password;


	public void checkPassword()
	{
		User u = new User ();
		u.username = _email.text;
		u.password = _password.text;
		u.Login();
		print(u.authenticated);
	}


		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
