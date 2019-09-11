using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using UnityEngine.Networking;

public class Favourite_content : MonoBehaviour {

	public GameObject originObject;
	public Transform parentTransForm;

    public Text text1;
    public Text text2;

	// Use this for initialization
	void Start () {

		GameObject posters;


		//clone list items (poster)


		Favourite f_get = new Favourite ();
		f_get.userid = storeLoginSessionId.loginId;
		f_get.GetFavourites ();
        if (f_get.favourites.Length == 0)
        {
            originObject.SetActive(false);
            text1.GetComponent<Text>().color = new Color(133/255.0f, 36/255.0f, 213/255.0f, 1f);
            text2.GetComponent<Text>().color = new Color(133 / 255.0f, 36 / 255.0f, 213 / 255.0f, 1f);
            return;
        }

		Favourite firstone = f_get.favourites [0];

		Poster pFirst = new Poster ();
		pFirst.targetid = firstone.targetid;
        Like_Button heart = originObject.transform.GetChild(1).GetComponent<Like_Button>();
        heart.targetid = pFirst.targetid;
        pFirst.GetPoster ();
		string urlFirst = pFirst.thumbnail;
		RawImage imgFirst = this.GetComponentsInChildren<RawImage> ()[0];
		StartCoroutine (LoadImageFromUrl (urlFirst, imgFirst));
		Text tFirst = this.GetComponentsInChildren<Text> () [0];
        tFirst.name = pFirst.targetid;
        tFirst.text = pFirst.postitle;


		for (int i = 1; i < f_get.favourites.Length; i++) {

			Poster p1 = new Poster ();
			p1.targetid = f_get.favourites[i].targetid;
			p1.GetPoster ();
			string url = p1.thumbnail;

			posters = GameObject.Instantiate(originObject, parentTransForm);
            RawImage[] imgs = posters.GetComponentsInChildren<RawImage> ();
			foreach (RawImage img in imgs) {
//				img.color = Random.ColorHSV ();
				StartCoroutine (LoadImageFromUrl (url,img));
                Debug.Log(url);
			}

			Text[] texts = posters.GetComponentsInChildren<Text> ();
			foreach (Text t in texts) {
                t.name = p1.targetid;
                t.text = p1.postitle;
			}

//			StartCoroutine (LoadImageFromUrl (url,img));

//			print (url);
		}
			


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator LoadImageFromUrl(string url, RawImage img){

		/*WWW wwwLoader = new WWW (url);
		yield return wwwLoader;
		//		img = wwwLoader.texture;

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
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);
                img.texture = texture;
            }
        }

    }

//	private RawImage[] getRawImages (Favourite[] favourites){
//		foreach (Favourite f in favourites) {
//			Poster p1 = new Poster ();
//			p1.keygroup = f.keygroup;
//			p1.keyid = f.keyid;
//			p1.GetPoster ();
//			string url = p1.resurl;
//
//		}
//	}


}
