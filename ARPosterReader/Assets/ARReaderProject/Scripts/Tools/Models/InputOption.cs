using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Text;

namespace Models
{
    [Serializable]
    public class InputOption
    {
        public int uid;
        public string targetid;
        public string content; // Format: [["Answer for Q0"], ["Answer for Q1"], ["Answer for Q2"]] --- if the type is checkbox, use semicolon to combine the selected option into a string.

        public class Question
        {
            public long id;
            public int tid; // 1 - text; 2 - radio button; 3 - check box
            public string name;
            public string option_string; // options are separated by semicolon
        }
               
        [NonSerialized]
        public Question[] questions;


        public string StoreAnswer()
        {
            return Post("/inputs/answer");
        }

        public string GetInputOptions()
        {
            return Get("/inputs/");
        }

        public string Post(string endpoint)
        {
            var uri = "http://" + new Tools().Server + endpoint;
            var req = HttpWebRequest.Create(uri);

            req.ContentType = "application/json";
            req.Method = "POST";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bodyData = encoding.GetBytes(JsonUtility.ToJson(this));
            req.ContentLength = bodyData.Length;
            req.GetRequestStream().Write(bodyData, 0, bodyData.Length);

            var response = req.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var json = reader.ReadToEnd();
                var ior = JsonUtility.FromJson<InputOptionResponse>(json);
                return ior.error;
            }
        }

        public string Get(string endpoint)
        {
            var uri = "http://" + new Tools().Server + endpoint + targetid;
            var req = HttpWebRequest.Create(uri);

            req.ContentType = "application/json";
            req.Method = "GET";

            var response = req.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var json = reader.ReadToEnd();
                var ior = JsonUtility.FromJson<InputOptionResponse>(json);
                this.questions = ior.questions;
                return ior.error;
            }
        }

        [Serializable]
        public class InputOptionResponse
        {
            public string error;
            public bool isexist;
            public bool success;

            public Question[] questions;
        }
    }
}

