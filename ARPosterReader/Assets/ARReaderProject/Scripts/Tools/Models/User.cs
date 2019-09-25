using System.Net;
using System.IO;
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
        public string nickname;
        public string email;
		public string username;
		public string password;
		public string facebook;
		public string google;
        public int activated;


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
			return Post ("/users");
		}

		// Update user info
		public string Update() {
			if (!String.IsNullOrEmpty(this.password)) {
				this.password = MD5Encrypt64 (this.password);
			}
			return Patch ("/users");
		}

        public string ResetPassword()
        {
            if(!string.IsNullOrEmpty(password))
            {
                return Post("/users/reset/password");
            }
            else
            {
                return Get("/users/temp/password/" + email);
            }
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
                    this.id = ur.user.id;
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
                    this.authenticated = ur.isexist;
					return ur.error;
				}
			}
		}

		private string Post(string endpoint) {
			var uri = "http://" + new Tools().Server + endpoint;
			var req = HttpWebRequest.Create(uri);
			req.ContentType = "application/json";
			req.Method = "POST";

			ASCIIEncoding encoding = new ASCIIEncoding ();
			byte[] bodyData = encoding.GetBytes (JsonUtility.ToJson (this));
			req.ContentLength = bodyData.Length;
			req.GetRequestStream ().Write (bodyData, 0, bodyData.Length);

			var response = req.GetResponse () as HttpWebResponse;

			using (var reader = new StreamReader (response.GetResponseStream ())) {
				var json = reader.ReadToEnd ();
				var ur = JsonUtility.FromJson<UsersResponse> (json);
				this.exist = ur.success;
                this.authenticated = ur.success;
				return ur.error;
			}
		}

		private string Patch(string endpoint) {
			var uri = "http://" + new Tools().Server + endpoint;
			var req = HttpWebRequest.Create(uri);
			req.ContentType = "application/json";
			req.Method = "PATCH"; 

			ASCIIEncoding encoding = new ASCIIEncoding ();
			byte[] bodyData = encoding.GetBytes (JsonUtility.ToJson (this));
			req.ContentLength = bodyData.Length;
			req.GetRequestStream ().Write (bodyData, 0, bodyData.Length);

			var response = req.GetResponse () as HttpWebResponse;

			using (var reader = new StreamReader (response.GetResponseStream ())) {
				var json = reader.ReadToEnd ();
				var ur = JsonUtility.FromJson<UsersResponse> (json);
				return ur.error;
			}
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