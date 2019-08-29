using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Text;


namespace Models {
	[Serializable]
	public class Poster {
        public string targetid;
        public string postitle;
		public string posdate;
        public string poslocation;
        public string posmap;
        public string poslink;
		public string resurl;
        public string model;
        public string thumbnail;
        public string relevantinfo;

		public string GetPoster() {
			return Get("/posters");
		}

		public string SavePoster() {
			return Post ("/posters");
		}

		public string Get(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint + "/" + targetid;
			var req = HttpWebRequest.Create(uri);

			req.ContentType = "application/json";
			req.Method = "GET";

			var response = req.GetResponse() as HttpWebResponse;
		
			using (var reader = new StreamReader(response.GetResponseStream())) {
				var json = reader.ReadToEnd();
				var pr = JsonUtility.FromJson<PostersResponse>(json);
                //Debug.Log(json);
                this.postitle = pr.poster.postitle;
                this.posdate = pr.poster.posdate;
                this.poslocation = pr.poster.poslocation;
                this.posmap = pr.poster.posmap;
                this.poslink = pr.poster.poslink;
                this.resurl = pr.poster.resurl;
                this.model = pr.poster.model;
                this.thumbnail = pr.poster.thumbnail;
                this.relevantinfo = pr.poster.relevantinfo;
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
