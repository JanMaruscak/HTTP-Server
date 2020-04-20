using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HTTPServer.Handlers
{
    public class FolderHandler : Handler
    {
        private string FolderLocation;

        public FolderHandler(string url, string folderLocation)
        {
            FolderLocation = folderLocation;
            URL = url;
        }

        public override ResponseMessage Process(RequestMessage request)
        {
            ResponseMessage response = new ResponseMessage();
            var path = FolderLocation + GetFileRequestName(request);
            response.Body = new MemoryStream();

            var fs = new FileStream(path, FileMode.Open);
            fs.CopyTo(response.Body);
            fs.Flush();
            fs.Close();
            response.Body.Position = 0;

            string mimeType = MimeTypeLookup.GetMimeType(request.Query);
            response.Headers.Add("Content-Type", mimeType);
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public string GetFileRequestName(RequestMessage request)
        {
            if (!request.Query.Contains('.')) return "";
            var split = request.Query.Split('/');
            if (split.Length <= 1) return "";

            string name = "";
            for (int i = 0; i < split.Length; i++)
            {
                name += split[i];
                if (i < split.Length - 1)
                {
                    name += "/";
                }
            }
            return name;
        }

        public override bool CanHandle(RequestMessage request)
        {
            if (!Directory.Exists(FolderLocation)) return false;
            if (!request.Query.StartsWith(URL + "/") && URL != "/" && request.Query != URL) return false;
            var path = FolderLocation + GetFileRequestName(request);
            if (!File.Exists(path)) return false;

            return true;
        }
    }
}