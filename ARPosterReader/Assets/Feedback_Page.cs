using UnityEngine;
using UnityEngine.UI;
using Models;



public class Feedback_Page : MonoBehaviour {
	public InputField _feedback;




	public void AddtoDatabase()
	{

		string email = storeLoginSessionId.email;

		int id = storeLoginSessionId.loginId;
		string username = storeLoginSessionId.name;
		string f = _feedback.text;


		// Debug.Log (f);    
		Feedback feedback = new Feedback();
		feedback.userid = 666;
		feedback.username = storeLoginSessionId.name;
		feedback.email = storeLoginSessionId.email;
		feedback.content = f;
		feedback.SendFeedback();


	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}

