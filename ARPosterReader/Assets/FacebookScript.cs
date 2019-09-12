using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using Models;
using UnityEngine.SceneManagement;
using System.IO;

public class FacebookScript : MonoBehaviour
{

    public Text FriendsText;
    public Button favouriteButton;
    public Text keyid;

    public void Start()
    {
        if (favouriteButton != null)
        {
            if (storeLoginSessionId.loginId == -1)
            {
                favouriteButton.interactable = false;
            }
            else
            {
                favouriteButton.interactable = true;
            }
        }
        
    }

    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Couldn't initialize");
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
            FB.ActivateApp();
    }

    #region Login / Logout
    public void FacebookLogin()
    {
        var permissions = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(permissions,AuthCallback);

    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("Log in Success");
            //Get Facebook Details
            FB.API("me?fields=id,name", Facebook.Unity.HttpMethod.GET, GetFacebookData, new Dictionary<string, string>() { });
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    void GetFacebookData(Facebook.Unity.IGraphResult result)
    {
        string fbId = result.ResultDictionary["id"].ToString();
        string name = result.ResultDictionary["name"].ToString();

        Debug.Log("fbIdddddd: " + fbId);
        User u = new User();
        u.facebook = fbId;
        u.name = name;
        u.CheckExist();
        Debug.Log("The CheckExist" + u.exist);
        if (u.exist)
        {
            u.Login();
        }
        else
        {
            u.Create();
            u.Login();
        }

        if (u.authenticated)
        {
            string path = Application.persistentDataPath + "/User.txt";
            StreamWriter writer;
            string uID = u.id.ToString();
            storeLoginSessionId.loginId = u.id;
            if (string.IsNullOrEmpty(u.name))
            {
                storeLoginSessionId.name = u.username;
            }
            else
            {
                storeLoginSessionId.name = u.name;
            }

            storeLoginSessionId.email = u.email;

            if (!File.Exists(path))
            {
                writer = File.CreateText(path);
                writer.Write(uID + "\n");
                writer.Write(storeLoginSessionId.name + "\n");
                writer.Close();
            }
            else
            {
                File.WriteAllText(path, uID + "\n" + storeLoginSessionId.name + "\n");
            }

            SceneManager.LoadScene("HiARRobot");
        }
        else
        {
            Debug.Log("Login Failed");
        }
    }

    public void FacebookLogout()
    {
        FB.LogOut();
    }
    #endregion

    public void FacebookShare()
    {
        Poster poster = new Poster();
        poster.targetid = keyid.text;
        poster.GetPoster();
        FB.ShareLink(new System.Uri(poster.thumbnail), "Check it out!",
            "AR Poster Reader lol!",
            new System.Uri(poster.thumbnail));
        Debug.Log(keyid.text);
    }

    #region Inviting
    public void FacebookGameRequest()
    {
        FB.AppRequest("Hey! Come and play this awesome game!", title: "Reso Coder Tutorial");
    }

    public void FacebookInvite()
    {
        FB.Mobile.AppInvite(new System.Uri("https://play.google.com/store/apps/details?id=com.tappybyte.byteaway"));
    }
    #endregion

    public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            FriendsText.text = string.Empty;
            foreach (var dict in friendsList)
                FriendsText.text += ((Dictionary<string, object>)dict)["name"];
        });
    }
}