using UnityEngine;
using UnityEngine.UI;

public class ShowLoginStatus : MonoBehaviour {

    public Text userName;
	
	// Update is called once per frame
	void Update () {
		if(storeLoginSessionId.loginId == -1)
        {
            userName.text = "";
        }
        else
        {
            userName.text = "Welcome, Your Id is " + storeLoginSessionId.loginId;
        }
	}
}
