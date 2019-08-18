﻿using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Text;


namespace Models {
	[Serializable]
	public class Poster {
		public string keygroup;
		public string keyid;
        public string postitle;
		public string posdate;
        public string poslocation;
        public string posmap;
        public string poslink;
		public string resurl;

		public string GetPoster() {
			return Get("/posters");
		}

		public string SavePoster() {
			return Post ("/posters");
		}

		public string Get(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint + "/" + keygroup + "/" + keyid;
			var req = HttpWebRequest.Create(uri);

			req.ContentType = "application/json";
			req.Method = "GET";

			var response = req.GetResponse() as HttpWebResponse;
		
			using (var reader = new StreamReader(response.GetResponseStream())) {
				var json = reader.ReadToEnd();
				var pr = JsonUtility.FromJson<PostersResponse>(json);
                this.postitle = pr.poster.postitle;
                this.posdate = pr.poster.posdate;
                this.poslocation = pr.poster.poslocation;
                this.posmap = pr.poster.posmap;
                this.poslink = pr.poster.poslink;
                this.resurl = pr.poster.resurl;
				return pr.error;
			}
		}

		public string Post(string endpoint) {
			var uri = "http://" + new Tools ().Server + endpoint;
			var req = HttpWebRequest.Create (uri);

			req.ContentType = "application/json";
			req.Method = "POST";

			ASCIIEncoding encoding = new ASCIIEncoding ();
			byte[] bodyData = encoding.GetBytes (JsonUtility.ToJson (this));
			req.ContentLength = bodyData.Length;
			req.GetRequestStream ().Write (bodyData, 0, bodyData.Length);

			var response = req.GetResponse () as HttpWebResponse;

			using (var reader = new StreamReader (response.GetResponseStream ())) {
				var json = reader.ReadToEnd ();
				var pr = JsonUtility.FromJson<PostersResponse> (json);
				return pr.error;
			}
		}

		[Serializable]
		public class PostersResponse {
			public string error;
			public bool isexist;
			public bool success;

			public Poster poster;
		}
	}
}