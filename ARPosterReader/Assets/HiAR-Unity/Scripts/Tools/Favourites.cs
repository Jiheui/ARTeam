using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Proyecto26;
using System.Net;
using System.IO;
using System;
using UnityEngine.UI;
using RSG;
using System.Web;

namespace Models {
	[Serializable]
	public class Favourite {
		public int userid;
		public string keygroup;
		public string keyid;
		public string time;

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
			var err = "";
			var uri = new Tools().Server + endpoint;
			RestClient.Post<FavouritesResponse>(new RequestHelper {
				Uri = uri,
				BodyString = new Tools().MakeJsonStringFromClass<Favourite>(this)
			}).Then(res => {
				err = res.error;
			});
			return err;
		}

		public string Delete(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint + "/" + userid + "/" + keygroup + "/" + keyid;
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
