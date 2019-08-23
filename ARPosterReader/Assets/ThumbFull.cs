using Models;
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

    //This is called when the user clicks on thumbnail of a poster. It enlarges the poster to fullsize.
    public void EnlargeImg (GameObject go){

        RawImage img = this.GetComponentInChildren<RawImage>();
        //img.texture = new Texture();
        Favourite f_get = new Favourite();
        f_get.userid = storeLoginSessionId.loginId;
        f_get.GetFavourites();
        string n = go.name;

        for (int i = 0; i < f_get.favourites.Length; i++)
        {
            Poster p = new Poster();
            Favourite f = f_get.favourites[i];
            p.targetid = f.targetid;
            p.GetPoster();

            string s2 = p.targetid;

            if (s2.Equals(n))
            {
                string url = p.resurl;
                StartCoroutine(LoadImageFromUrl(url, img));

                timeText.text = p.posdate;
                addressText.text = p.poslocation;
                addressURL.text = p.posmap;
                linkText.text = p.poslink;
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
