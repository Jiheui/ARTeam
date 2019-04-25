using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailBehaviour : MonoBehaviour {

    Text timeText;
    Text addressText;
    Text linkText;
    Text addressURL;

    // Use this for initialization
    void Start () {
        timeText = GameObject.Find("Time").GetComponent<Text>();
        addressText = GameObject.Find("Address").GetComponent<Text>();
        linkText = GameObject.Find("Web Link").GetComponent<Text>();
        addressURL = GameObject.Find("Address Url").GetComponent<Text>();
    }

    public void OpenMapOnClick()
    {
        if (addressURL.Equals(""))
            return;
        bool fail = false;
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", "com.google.android.apps.maps");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            fail = true;
        }

        if (fail)
        {   //open app in store
            Application.OpenURL("https://google.com");

        }
        else
        {
            //open the app
            //Application.OpenURL("https://goo.gl/maps/sL2Tug3N5oytfJvR7");
            Application.OpenURL(addressURL.text);
        }

        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();
    }

    public void OpenWebOnClick()
    {
        //open the URL
        Application.OpenURL(linkText.text);
    }
}
