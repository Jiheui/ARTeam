using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using Models;

public class FacebookScript : MonoBehaviour
{

    public Text FriendsText;

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
        if (u.exist)
        {
            u.Login();
        }
        else
        {
            u.Create();
            u.Login();
        }

        Debug.Log("The User has Loged in :  "+u.authenticated);
    }

    public void FacebookLogout()
    {
        FB.LogOut();
    }
    #endregion

    public void FacebookShare()
    {
        FB.ShareLink(new System.Uri("https://resocoder.com"), "Check it out!",
            "AR Poster Reader lol!",
            new System.Uri("https://resocoder.com/wp-content/uploads/2017/01/logoRound512.png"));
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