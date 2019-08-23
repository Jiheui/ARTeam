using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Text;

namespace Models {
	[Serializable]
	public class Favourite {
		public int userid;
		public string targetid;
		public string time;

		[NonSerialized]
		public Favourite[] favourites;

		public string GetFavourites() {
			return Get("/favourites");
		}

		public string Like() {
			return Post ("/favourites");
		}

		public string Dislike() {
			return Delete ("/favourites");
		}

		public string Get(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint + "/" + userid;
			var req = HttpWebRequest.Create(uri);

			req.ContentType = "application/json";
			req.Method = "GET";

			var response = req.GetResponse() as HttpWebResponse;

			using (var reader = new StreamReader(response.GetResponseStream())) {
				var json = reader.ReadToEnd();
				var fr = JsonUtility.FromJson<FavouritesResponse>(json);
				this.favourites = fr.favourites;
				return fr.error;
			}
		}

		public string Post(string endpoint) {
			var uri = "http://" + new Tools().Server + endpoint;
			var req = HttpWebRequest.Create(uri);

			req.ContentType = "application/json";
			req.Method = "POST";

			ASCIIEncoding encoding = new ASCIIEncoding ();
			byte[] bodyData = encoding.GetBytes (JsonUtility.ToJson (this));
			req.ContentLength = bodyData.Length;
			req.GetRequestStream ().Write (bodyData, 0, bodyData.Length);

			var response = req.GetResponse() as HttpWebResponse;

			using (var reader = new StreamReader(response.GetResponseStream())) {
				var json = reader.ReadToEnd();
				var fr = JsonUtility.FromJson<FavouritesResponse>(json);
				return fr.error;
			}
		}

		public string Delete(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint + "/" + userid + "/" + targetid;
			var req = HttpWebRequest.Create(uri);

			req.ContentType = "application/json";
			req.Method = "DELETE";

			var response = req.GetResponse() as HttpWebResponse;

			using (var reader = new StreamReader(response.GetResponseStream())) {
				var json = reader.ReadToEnd();
				var fr = JsonUtility.FromJson<FavouritesResponse>(json);
				return fr.error;
			}
		}

		[Serializable]
		public class FavouritesResponse {
			public string error;
			public bool isexist;
			public bool success;

			public Favourite[] favourites;
		}
	}
}
