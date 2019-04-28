using UnityEngine;
using Proyecto26;
using System.Net;
using System.IO;
using System;
using System.Text;

namespace Models {
	[Serializable]
	public class Feedback {
		public int id;
		public string content;
		public string email;
		public string time;

		public int userid;
		public string username;
		public int isdeleted;

		[NonSerialized]
		public Feedback[] feedbacks;

		public string GetFeedbacks() {
			return Get("/feedbacks");
		}

		public string GetOneFeedback() {
			return GetOne("/feedbacks");
		}

		public string SendFeedback() {
			return Post ("/feedbacks");
		}

		public string DeleteFeedback() {
			return Delete ("/feedbacks");
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
				var fb = JsonUtility.FromJson<FeedbackResponse>(json);
				this.feedbacks = fb.feedbacks;
				return fb.error;
			}
		}

		public string GetOne(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint + "/feedback/" + id;
			var req = HttpWebRequest.Create(uri);

			req.ContentType = "application/json";
			req.Method = "GET";

			var response = req.GetResponse() as HttpWebResponse;

			using (var reader = new StreamReader(response.GetResponseStream())) {
				var json = reader.ReadToEnd();
				var fb = JsonUtility.FromJson<FeedbackResponse>(json);
				this.feedbacks = fb.feedbacks;
				return fb.error;
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
				var fb = JsonUtility.FromJson<FeedbackResponse>(json);
				return fb.error;
			}
			//var err = "";
			//var uri = new Tools().Server + endpoint;
			//RestClient.Post<FeedbackResponse>(new RequestHelper {
			//	Uri = uri,
			//	BodyString = JsonUtility.ToJson(this)
			//}).Then(res => {
		//		err = res.error;
		//	});
		//	return err;
		}

		public string Delete(string endpoint)
		{
			var uri = "http://" + new Tools().Server + endpoint + "/" + this.id;
			var req = HttpWebRequest.Create(uri);

			req.ContentType = "application/json";
			req.Method = "DELETE";

			var response = req.GetResponse() as HttpWebResponse;

			using (var reader = new StreamReader(response.GetResponseStream())) {
				var json = reader.ReadToEnd();
				var fb = JsonUtility.FromJson<FeedbackResponse>(json);
				return fb.error;
			}
		}

		[Serializable]
		public class FeedbackResponse {
			public string error;
			public bool isexist;
			public bool success;

			public Feedback[] feedbacks;
		}
	}
}

