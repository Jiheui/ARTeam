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

//		Poster p = new Poster ();
//		p.keygroup = "testgroupZ11";
//		p.keyid = "testidZZ2";
//		p.resurl = "https://k.zol-img.com.cn/sjbbs/7692/a7691515_s.jpg";
////		p.detail = "13 Apr 2019;The ANU Library;https://www.google.com";
//		p.SavePoster ();

//		Poster p1 = new Poster ();
//		p1.keygroup = "testgroupZ1";
//		p1.keyid = "testidZZ1";
//		p1.GetPoster ();
//		url = p1.resurl;

//		StartCoroutine (LoadImageFromUrl ());

//		Favourite f_save = new Favourite ();
//		f_save.userid = 11111;
//		f_save.keygroup = "testgroupZ11";
//		f_save.keyid = "testidZZ2";
//		f_save.Like ();


//		img.color = Color.red;



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
