using UnityEngine;
using Proyecto26;
using System.Net;
using System.IO;
using System;

namespace Models {
	[Serializable]
	public class Poster {
		public string keygroup;
		public string keyid;
		public string detail;
		public string url;

		public string GetPoster() {
			return Get("/posters");
		}

		public string SavePoster() {
			return Post ("/posters");
		}

		public string Get(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint + "/" + keygroup + "/" + keyid;
            Debug.Log(uri);
			var req = HttpWebRequest.Create(uri);

			req.ContentType = "application/json";
			req.Method = "GET";

			var response = req.GetResponse() as HttpWebResponse;
		
			using (var reader = new StreamReader(response.GetResponseStream())) {
				var json = reader.ReadToEnd();
				var pr = JsonUtility.FromJson<PostersResponse>(json);
				this.detail = pr.poster.detail;
				this.url = pr.poster.url;
				return pr.error;
			}
		}

        public string Post(string endpoint) {
			var err = "";
			var uri = new Tools().Server + endpoint;
			RestClient.Post<PostersResponse>(new RequestHelper {
				Uri = uri,
				BodyString = new Tools().MakeJsonStringFromClass<Poster>(this)
			}).Then(res => {
				err = res.error;
			});
			return err;
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
