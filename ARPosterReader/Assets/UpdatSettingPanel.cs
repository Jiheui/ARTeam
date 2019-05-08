using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatSettingPanel : MonoBehaviour {

    public Button loginButton;

    public Button logoutButton;

	// Use this for initialization
	void Start () {
        if (storeLoginSessionId.loginId == -1)
        {
            logoutButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(true);
        }
        else
        {
            logoutButton.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(false);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
