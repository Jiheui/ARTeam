﻿using Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/**this script is used for "Making thumbnail fullsize", 
   When a user touch on thumbnail of a poster in myfavourite scene, it will bring up a larger version of
    thumbnail.

    Author: Ji Heui Yu
    Date: 26/04/2019
**/
public class ThumbFull : MonoBehaviour {
    public GameObject poster;
    public Text timeText;
    public Text addressText;
    public Text linkText;
    public Text addressURL;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //This is called when the user clicks on thumbnail of a poster. It enlarges the poster to fullsize.
    public void EnlargeImg (GameObject go){

        Favourite f_get = new Favourite();
        f_get.userid = 11111;
        f_get.GetFavourites();
        string n = go.name;

        for (int i = 0; i < f_get.favourites.Length; i++)
        {
            Poster p = new Poster();
            Favourite f = f_get.favourites[i];
            p.keygroup = f.keygroup;
            p.keyid = f.keyid;
            p.GetPoster();

            string s2 = p.keygroup + "/" + p.keyid;

            if (s2 == n)
            {
                string url = p.resurl;
                RawImage img = this.GetComponentInChildren<RawImage>();
                StartCoroutine(LoadImageFromUrl(url, img));

                timeText.text = p.posdate;
                addressText.text = p.poslocation;
                addressURL.text = p.poslink;
                linkText.text = "To be added!!!";
                return;
            }
        }
        return;
    }

    private IEnumerator LoadImageFromUrl(string url, RawImage img)
    {
        WWW wwwLoader = new WWW(url);
        yield return wwwLoader;
        Texture2D t = wwwLoader.texture;
        img.texture = t;
    }
}
