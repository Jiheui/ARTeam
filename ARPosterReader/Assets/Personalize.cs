using Models;
using UnityEngine;
using UnityEngine.UI;

public class Personalize : MonoBehaviour
{
    public Text Name;

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
            Name.text = "Hello: " + storeLoginSessionId.name;
        }
        else
        {
            Debug.Log("Login Failed");
            Name.text = "";
        }
        
    }


}
