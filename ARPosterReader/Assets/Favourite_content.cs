using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;

public class Favourite_content : MonoBehaviour {

	public GameObject originObject;
	public Transform parentTransForm;


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
            return;
        }

		Favourite firstone = f_get.favourites [0];

		Poster pFirst = new Poster ();
		pFirst.keygroup = firstone.keygroup;
		pFirst.keyid = firstone.keyid;
        Like_Button heart = originObject.transform.GetChild(1).GetComponent<Like_Button>();
        heart.keyGroup = pFirst.keygroup;
        heart.keyId = pFirst.keyid;
        pFirst.GetPoster ();
		string urlFirst = pFirst.resurl;
		RawImage imgFirst = this.GetComponentsInChildren<RawImage> ()[0];
		StartCoroutine (LoadImageFromUrl (urlFirst, imgFirst));
		Text tFirst = this.GetComponentsInChildren<Text> () [0];
        tFirst.name = pFirst.keygroup + "/" + pFirst.keyid;
        tFirst.text = pFirst.postitle;


		for (int i = 1; i < f_get.favourites.Length; i++) {

			Poster p1 = new Poster ();
			p1.keygroup = f_get.favourites[i].keygroup;
			p1.keyid = f_get.favourites[i].keyid;
			p1.GetPoster ();
			string url = p1.resurl;

			posters = GameObject.Instantiate(originObject, parentTransForm);
            RawImage[] imgs = posters.GetComponentsInChildren<RawImage> ();
			foreach (RawImage img in imgs) {
//				img.color = Random.ColorHSV ();
				StartCoroutine (LoadImageFromUrl (url,img));
                Debug.Log(url);
			}

			Text[] texts = posters.GetComponentsInChildren<Text> ();
			foreach (Text t in texts) {
                t.name = p1.keygroup + '/' + p1.keyid;
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

		WWW wwwLoader = new WWW (url);
		yield return wwwLoader;
		//		img = wwwLoader.texture;

		Texture2D t = wwwLoader.texture;
		img.texture = t;

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
