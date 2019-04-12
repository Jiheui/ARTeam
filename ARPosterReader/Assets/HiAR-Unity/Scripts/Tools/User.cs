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

	public void Login() {
		Get ("/users/login");
	}

	public void FindUser() {
		Get ("/users/" + id.ToString ());
	}

	// Create a new user
	public void Create() {
	}

	// Update user info
	public void Update() {
	}

	// RESTful, HTTP verb: GET
	private string Get(string endpoint) {
		var uri = Tools.Server + endpoint;
		switch (endpoint) {
		case "/users/login":
			RestClient.Get<User> (new RequestHelper {
				Uri = uri,
			}, (err, res, body) => {
				if (err != null) {
					EditorUtility.DisplayDialog ("Error", err.Message, "Ok");
				} else {
					EditorUtility.DisplayDialog ("Success", res.Text, "Ok");
				}	
			});
			break;
		default:
			RestClient.Get (uri, (err, res) => {
				if (err != null) {
					EditorUtility.DisplayDialog ("Error", err.Message, "Ok");
				} else {
					EditorUtility.DisplayDialog ("Success", res.Text, "Ok");
				}	
			});
			return;
		}
	}

	[Serializable]
	public class UsersResponse {
		public string error;
		public bool isexist;
		public bool success;

		public User user;
	}
}
