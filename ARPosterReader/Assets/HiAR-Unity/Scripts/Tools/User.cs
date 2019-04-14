using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using Proyecto26;
using System;
using UnityEditor;
using UnityEngine;
using System.Text;
using System.Globalization;


namespace Models {
	[Serializable]
	public class User {
		public int id;
		public string name;
		public string dob;
		public string username;
		public string password;
		public string facebook;
		public string google;


		[NonSerialized]
		public bool authenticated;

		public string Login() {
			return Get ("/users/login");
		}

		public string FindUser() {
			return Get ("/users/" + id.ToString ());
		}

		// Create a new user
		public string Create() {
			return Post ("/users");
		}

		// Update user info
		public string Update() {
			return Patch ("/users");
		}

		public string Get(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint;
			var req = HttpWebRequest.Create(uri);
			req.ContentType = "application/json";
			req.Method = "GET";

			switch (endpoint) {
			case "/users/login":
				req.Method = "POST"; // Actually, check credentials is kind of GET thing from the server, however, GET will make username&password as a plaintext, so use POST instead.
				ASCIIEncoding encoding = new ASCIIEncoding ();
				byte[] bodyData = encoding.GetBytes (JsonUtility.ToJson (this));
				req.ContentLength = bodyData.Length;
				req.GetRequestStream ().Write (bodyData, 0, bodyData.Length);

				var response = req.GetResponse () as HttpWebResponse;

				using (var reader = new StreamReader (response.GetResponseStream ())) {
					var json = reader.ReadToEnd ();
					var ur = JsonUtility.FromJson<UsersResponse> (json);
					this.authenticated = ur.success;
					return ur.error;
				}
				break;
			default:
				response = req.GetResponse() as HttpWebResponse;

				using (var reader = new StreamReader(response.GetResponseStream())) {
					var json = reader.ReadToEnd();
					var ur = JsonUtility.FromJson<UsersResponse>(json);
					this.name = ur.user.name;
					this.dob = ur.user.dob;
					this.facebook = ur.user.facebook;
					this.google = ur.user.google;
					return ur.error;
				}
				break;
			}
		}

		private string Post(string endpoint) {
			var err = "";
			var uri = new Tools().Server + endpoint;
			RestClient.Post<UsersResponse>(new RequestHelper {
				Uri = uri,
				BodyString = new Tools().MakeJsonStringFromClass<User>(this)
			}).Then(res => {
				err = res.error;
			});
			return err;
		}

		private string Patch(string endpoint) {
			var err = "";
			var uri = new Tools().Server + endpoint;
			RestClient.Patch<UsersResponse>(new RequestHelper {
				Uri = uri,
				BodyString = new Tools().MakeJsonStringFromClass<User>(this)
			}).Then(res => {
				err = res.error;
			});
			return err;
		}

		[Serializable]
		public class UsersResponse {
			public string error;
			public bool isexist;
			public bool success;

			public User user;
		}
	}
}