using System.Net;
using System.IO;
using Proyecto26;
using System;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;


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

		[NonSerialized]
		public bool exist;

		public string Login() {
			if (!String.IsNullOrEmpty (this.password)) {
				this.password = MD5Encrypt64 (this.password);
			}
			return Get ("/users/login");
		}

		public string CheckExist() {
			return Post("/users/checkExist");
		}

		public string FindUser() {
			return Get ("/users/" + id.ToString ());
		}

		// Create a new user
		public string Create() {
			if (!String.IsNullOrEmpty (this.password)) {
				this.password = MD5Encrypt64 (this.password);
			}
			if (Regex.IsMatch (this.username, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")) {
				// Do something, like send confirm email
			}
			return Post ("/users");
		}

		// Update user info
		public string Update() {
			if (!String.IsNullOrEmpty(this.password)) {
				this.password = MD5Encrypt64 (this.password);
			}
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
			}
		}

		private string Post(string endpoint) {
			var err = "";
			var uri = new Tools().Server + endpoint;
			RestClient.Post<UsersResponse>(new RequestHelper {
				Uri = uri,
				//BodyString = new Tools().MakeJsonStringFromClass<User>(this)
				BodyString = JsonUtility.ToJson(this)
			}).Then(res => {
				err = res.error;
				if(endpoint.Equals("/users/checkExist")){
					this.exist = res.success;
				}
			});
			return err;
		}

		private string Patch(string endpoint) {
			var err = "";
			var uri = new Tools().Server + endpoint;
			RestClient.Patch<UsersResponse>(new RequestHelper {
				Uri = uri,
				//BodyString = new Tools().MakeJsonStringFromClass<User>(this)
				BodyString = JsonUtility.ToJson(this)
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

		public static string MD5Encrypt64(string password)
		{
			string cl = password;
			MD5 md5 = MD5.Create();
			byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
			return Convert.ToBase64String(s);
		}
	}
}