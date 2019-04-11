using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using Proyecto26;
using System;
using UnityEditor;

public class User {
	public int id { get; set; }
	public string name { get; set; }
	public DateTime dob { get; set; }
	public string username { get; set; }
	public string password { get; set; }

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
	private void Get(string endpoint) {
		var uri = Settings.server + endpoint;
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

	private string ConvertToJsonString(){
		return "";
	}
}
