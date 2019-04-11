using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Proyecto26;
using System.Net;
using System.IO;
using System;

namespace Models {
	[Serializable]
	public class Poster {
		public string KeyGroup { get; set; }
		public string KeyId { get; set; }
		public string Detail { get; set; }
		public string Url { get; set; }

		public void GetPoster() {
			Get ("/posters");
		}

		public void SavePoster() {
			Put ("/posters");
		}

		// RESTful, HTTP verb: GET
		public void Get(string endpoint) {
			var uri = Settings.server + endpoint + "/" + KeyGroup + "/" + KeyId + "/" + Detail + "/" + Url;
			RestClient.Get<Poster> (new RequestHelper {
				Uri = uri
			}, (err, res, body) => {
				if (err != null) {
					EditorUtility.DisplayDialog ("Error", err.Message, "Ok");
				} else {
					EditorUtility.DisplayDialog ("Success", res.Text, "Ok");
				}	
			});
		}

		public void Put(string endpoint) {
			var uri = Settings.server + endpoint;
			RestClient.Put<PostersResponse>(new RequestHelper {
				Uri = uri,
				BodyString = ConvertToJsonString(),
				EnableDebug = true
			}).Then(res => {
				if(string.Equals(res.error, "")) {
					EditorUtility.DisplayDialog ("Success", res.success, "Ok");
				} else {
					
					EditorUtility.DisplayDialog ("Error", res.error, "Ok");
				}
			});
		}

		private string ConvertToJsonString(){
			var jsonStr = "{\"keygroup\":\"" + KeyGroup + "\", \"keyid\":\"" + KeyId + "\", \"detail\":\"" + Detail + "\", \"url\":\"" + Url + "\"}";
			return jsonStr;
		}

		[Serializable]
		public class PostersResponse {
			public string error;
			public bool isexist;
			public bool success;
		}
	}
}
