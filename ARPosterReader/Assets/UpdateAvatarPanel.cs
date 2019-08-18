using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateAvatarPanel : MonoBehaviour {

    public Text username;

    public Text email;

	// Use this for initialization
	void Start () {
        if(storeLoginSessionId.loginId != -1)
        {
            username.text = storeLoginSessionId.name;
        }
    }
}
