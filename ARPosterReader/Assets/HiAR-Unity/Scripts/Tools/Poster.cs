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

namespace Models {
	[Serializable]
	public class Poster {
		public string keygroup;
		public string keyid;
		public string detail = " ABDKLFJ";
		public string url;

        GameObject target;

        public void SetTarget(GameObject obj)
        {
            target = obj;
        }

		public string GetPoster() {
			return Get ("/posters");
		}

		public string SavePoster() {
			return Put ("/posters");
		}

		// RESTful, HTTP verb: GET
		public string Get(string endpoint) {
			var err = "";
			var uri = Tools.Server + endpoint + "/" + keygroup + "/" + keyid;
            Debug.Log(uri);
			RestClient.Get<PostersResponse> (new RequestHelper {
				Uri = uri
			}).Then(res => {
				this.detail = res.poster.detail;
                target.GetComponent<ImageTargetBehaviour>().showDetail(this.detail);
                //this.updateDetailPanel(this.Detail);
				this.url = res.poster.url;
				err = res.error;
			});
			return err;
		}

        public string Put(string endpoint) {
			var err = "";
			var uri = Tools.Server + endpoint;
			RestClient.Put<PostersResponse>(new RequestHelper {
				Uri = uri,
				BodyString = new Tools().MakeJsonStringFromClass<Poster>(this)
			}).Then(res => {
				//if(!string.Equals(res.error, "")) {
					//EditorUtility.DisplayDialog ("Error", res.error, "Ok");
				//} else {
					//EditorUtility.DisplayDialog ("Success", res.success.ToString(), "Ok");
				//}
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
