using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

using UnityEngine.Networking;
using System.Collections;
using UnityEditor;
using Proyecto26;
using System.Net;
using System.IO;
using System;
using UnityEngine.UI;
using RSG;
using System.Web;

public class Poster_Thumbnail : MonoBehaviour {

	public string url;
//	Texture img;
	public Renderer thisRenderer;

	// Use this for initialization
	void Start () {
//		Poster p = new Poster ();
//		p.keygroup = "testgroupZ1";
//		p.keyid = "testidZZ1";
//		p.url = "https://k.zol-img.com.cn/sjbbs/7692/a7691501_s.jpg";
//		p.detail = "";
//		p.SavePoster ();


		Poster p1 = new Poster ();
		p1.keygroup = "testgroupZ1";
		p1.keyid = "testidZZ1";
		p1.GetPoster ();
		url = p1.url;
//		print (p1.url);
//		for (int i = 0; i < 100; i++) {
//			p1.GetPoster ();
//			print (p1.url);
//		}

		StartCoroutine (LoadImage ());
		thisRenderer.material.color = Color.red;


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator LoadImage(){
		yield return 0;
		WWW wwwLoader = new WWW (url);
		yield return wwwLoader;
//		img = wwwLoader.texture;

		thisRenderer.material.color = Color.white;
		thisRenderer.material.mainTexture = wwwLoader.texture;

	
	}

//	void OnGUI(){
//		GUILayout.Label (img);
//	}



}
