using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

public class personalizePoster3D : MonoBehaviour
{
    public TextMesh textMesh; 

    public void Start()
    {
        User u = new User();
        u.id = storeLoginSessionId.loginId;

        u.FindUser();
        if (u.authenticated)
        {
            if (string.IsNullOrEmpty(u.name))
            {
                storeLoginSessionId.name = u.username;
            }
            else
            {
                storeLoginSessionId.name = u.name;
            }
            textMesh.text = "Hello " + storeLoginSessionId.name;
        }
        else
        {
            Debug.Log("Login Failed");
            textMesh.text = "";
        }

    }
}
