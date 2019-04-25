using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;

public class LoadImage : MonoBehaviour {

	private string url ;
	public RawImage img;

	// Use this for initialization
	void Start () {
		Poster p1 = new Poster ();
		p1.keygroup = "testgroupZ1";
		p1.keyid = "testidZZ1";
		p1.GetPoster ();
		url = p1.poslink;

		StartCoroutine (LoadImageFromUrl ());

		Favourite f_save = new Favourite ();
		f_save.userid = 11111;
		f_save.keygroup = "testgroupZ1";
		f_save.keyid = "testidZZ1";
		print(f_save.Like ());

		Favourite f_get = new Favourite ();
		f_get.userid = 11111;
		f_get.GetFavourites ();
		foreach (Favourite f in f_get.favourites) {
			print (f.keyid);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator LoadImageFromUrl(){
		
		WWW wwwLoader = new WWW (url);
		yield return wwwLoader;
		//		img = wwwLoader.texture;

		Texture2D t = wwwLoader.texture;
		img.texture = t;



	}
}
