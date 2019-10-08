using System;
using UnityEngine;
using UnityEngine.UI;

public class DetailBehaviour : MonoBehaviour {
    
    public Text linkText;
    public Text addressURL;

    public void OpenMapOnClick()
    {
        if (string.IsNullOrEmpty(addressURL.text) || !addressURL.text.StartsWith("http"))
        {
            return;
        }
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
            if(!string.IsNullOrEmpty(addressURL.text))
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
        if (linkText.text.StartsWith("http"))
        {
            Application.OpenURL(linkText.text);
        }
    }
}
