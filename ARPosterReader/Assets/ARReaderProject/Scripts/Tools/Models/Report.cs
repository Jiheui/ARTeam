using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Text;

namespace Models
{
    [Serializable]
    public class Report
    {
        public int reportid;
        public string detail;

        public int userid;
        public string targetid;
        public int type;

        [NonSerialized]
        public Report[] reports;


        public string SendReport()
        {
            return Post("/reports");
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
                var fb = JsonUtility.FromJson<ReportResponse>(json);
                return fb.error;
            }
        }

        [Serializable]
        public class ReportResponse
        {
            public string error;
            public bool isexist;
            public bool success;

            public Report[] reports;
        }
    }
}

