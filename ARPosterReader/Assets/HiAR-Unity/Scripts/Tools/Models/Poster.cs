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
                this.postitle = pr.poster.posdate;
                this.posdate = pr.poster.posdate;
                this.poslocation = pr.poster.poslocation;
                this.posmap = pr.poster.posmap;
                this.poslink = pr.poster.poslink;
                this.resurl = pr.poster.resurl;
				return pr.error;
			}
		}

        public string Post(string endpoint) {
			var err = "";
			var uri = new Tools().Server + endpoint;
			RestClient.Post<PostersResponse>(new RequestHelper {
				Uri = uri,
				//BodyString = new Tools().MakeJsonStringFromClass<Poster>(this)
				BodyString = JsonUtility.ToJson(this)
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
