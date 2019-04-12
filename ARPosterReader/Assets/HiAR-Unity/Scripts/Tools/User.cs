using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using Proyecto26;
using System;
using UnityEditor;

public class User {
	public int id;
	public string name;
	public DateTime dob;
	public string username;
	public string password;
	public string facebook;
	public string google;
	public bool isAuthenticated;

	public void Login() {
		Get ("/users/login");
	}

	public void FindUser() {
		Get ("/users/" + id.ToString ());
	}

	// Create a new user
	public void Create() {
		Put ("/users");
	}

	// Update user info
	public void Update() {
		Patch ("/users");
	}

	// RESTful, HTTP verb: GET
	private string Get(string endpoint) {
		var uri = Tools.Server + endpoint;
		switch (endpoint) {
		case "/users/login":
			RestClient.Get<UsersResponse> (new RequestHelper {
				Uri = uri,
			}).Then(res => {
				this.isAuthenticated = res.success;
				return res.error;
			});
			break;
		default:
			RestClient.Get<UsersResponse>(uri).Then(res => {
				this.name = res.user.name;
				this.dob = res.user.dob;
				return res.error;
			});
			return;
		}
	}

	private string Put(string endpoint) {
		var uri = Tools.Server + endpoint;
		RestClient.Put<UsersResponse>(new RequestHelper {
			Uri = uri,
			BodyString = new Tools().MakeJsonStringFromClass<User>(this)
		}).Then(res => {
			return res.error;
		});
	}

	private string Patch(string endpoint) {
		var uri = Tools.Server + endpoint;
		RestClient.Patch<UsersResponse>(new RequestHelper {
			Uri = uri,
			BodyString = new Tools().MakeJsonStringFromClass<User>(this)
		}).Then(res => {
			return res.error;
		});
	}

	[Serializable]
	public class UsersResponse {
		public string error;
		public bool isexist;
		public bool success;

		public User user;
	}
}
