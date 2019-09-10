using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using UnityEngine.Networking;

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

        // UnityWebRequest is more efficient than WWW
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("http://www.my-server.com/myimage.png"))
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
}
