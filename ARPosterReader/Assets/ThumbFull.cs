using Models;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
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
    public Text keyId;
    public Text posterTitle;
    public Text relevantInfo;

    public static string planeDetectModelString;

    // Use this for initialization
    void Start () {
    }

    //This is called when the user clicks on thumbnail of a poster. It enlarges the poster to fullsize.
    public void EnlargeImg (GameObject go){
        RawImage img = this.GetComponentInChildren<RawImage>();
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
                string url = p.thumbnail;
                StartCoroutine(LoadImageFromUrl(url, img));
                planeDetectModelString = p.targetid;

                timeText.text = p.posdate;
                addressText.text = p.poslocation;
                addressURL.text = p.posmap;
                linkText.text = p.poslink; 
                posterTitle.text = p.postitle;
                keyId.text = p.targetid;
                relevantInfo.text = p.relevantinfo;
                return;
            }
        }
        return;
    }

    public void clearDetail()
    {
        posterTitle.text = "Title";
        timeText.text = "Time";
        addressText.text = "Address";
        linkText.text = "Web Link";
        addressURL.text = "";
        keyId.text = "";
        relevantInfo.text = "Relevant Information";
        planeDetectModelString = "";
    }

    private IEnumerator LoadImageFromUrl(string url, RawImage img)
    {
        /*UnityWebRequest wwwLoader = new UnityWebRequestTexture.GetTexture(url));
        yield return wwwLoader;
        Texture2D t = wwwLoader.texture;
        img.texture = t;*/

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded Picture
                var texture = DownloadHandlerTexture.GetContent(uwr);
                img.texture = texture;
            }
        }
    }
}
