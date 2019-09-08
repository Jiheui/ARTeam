using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Text;

namespace Models
{
    [Serializable]
    public class Option
    {
        public long optionid;
        public string targetid;
        public int value;
        public string key;

        [NonSerialized]
        public Option[] options;


        public string Incr()
        {
            return Post("/options/incr");
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
                var fb = JsonUtility.FromJson<OptionResponse>(json);
                return fb.error;
            }
        }

        public string Delete(string endpoint)
        {
            var uri = "http://" + new Tools().Server + endpoint + "/" + this.id;
            var req = HttpWebRequest.Create(uri);

            req.ContentType = "application/json";
            req.Method = "DELETE";

            var response = req.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var json = reader.ReadToEnd();
                var fb = JsonUtility.FromJson<OptionResponse>(json);
                return fb.error;
            }
        }

        [Serializable]
        public class OptionResponse
        {
            public string error;
            public bool isexist;
            public bool success;

            public Option[] options;
        }
    }
}

