using UnityEngine;
using UnityEngine.UI;
using Models;



public class Feedback : MonoBehaviour {
	public InputField _feedback;




	public void AddtoDatabase()
	{
		
		string email = storeLoginSessionId.email;
		int id = storeLoginSessionId.loginId;
		string username = storeLoginSessionId.name;
		string f = _feedback.text;

		Feedback feedback = new Feedback();
		feedback.SendFeedback();

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
