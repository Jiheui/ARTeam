using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Text;

namespace Models
{
	public class File4Poster
	{
		public long id;
		public string keygroup;
		public string keyid;
		public string filename;
		[NonSerialized]
		public byte[] data;

		public string Upload() {
			var uri = "http://" + new Tools ().Server + "/files/upload/" + filename;
			var req = HttpWebRequest.Create (uri);

			req.ContentType = "application/octet-stream";
			req.Method = "POST";

			req.ContentLength = data.Length;
			req.GetRequestStream ().Write (data, 0, data.Length);

			var response = req.GetResponse () as HttpWebResponse;
			if (response.StatusCode == HttpStatusCode.OK) {
				return "";
			} else {
				return response.StatusDescription;
			}
		}

		public string GetFileByFilename() {
			var uri = "http://" + new Tools ().Server + "/files/" + filename;
			var req = HttpWebRequest.Create (uri);

			req.ContentType = "application/octet-stream";
			req.Method = "GET";

			var response = req.GetResponse () as HttpWebResponse;
			

			using (var reader = new StreamReader (response.GetResponseStream ())) {
				data = ReadFully (reader);

				if (response.StatusCode == HttpStatusCode.OK) {
					return "";
				} else {
					return response.StatusDescription;
				}
			}
		}

		private byte[] ReadFully(Stream input)
		{
			try
			{
				int bytesBuffer = 1024;
				byte[] buffer = new byte[bytesBuffer];
				using (MemoryStream ms = new MemoryStream())
				{
					int readBytes;
					while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
					{
						ms.Write(buffer, 0, readBytes);
					}
					return ms.ToArray();
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public class FileResponse {
			public string error;
			public bool isexist;
			public bool success;
		}
	}
}

