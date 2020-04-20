using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HTTPServer.Handlers
{
    public class FileHandler : Handler
    {
        private string FilePath;

        public FileHandler(string url, string filePath)
        {
            FilePath = filePath;
            URL = url;
        }

        public override bool CanHandle(RequestMessage request)
        {
            if (request.Query != URL) return false;
            if (!File.Exists(FilePath)) return false;

            return true;
        }

        public override ResponseMessage Process(RequestMessage request)
        {
            ResponseMessage response = new ResponseMessage();
            response.Body = new MemoryStream();

            var fs = new FileStream(FilePath, FileMode.Open);
            fs.CopyTo(response.Body);
            fs.Flush();
            fs.Close();

            response.StatusCode = HttpStatusCode.OK;
            string mimeType = MimeTypeLookup.GetMimeType(FilePath);
            response.Headers.Add("Content-Type", mimeType);

            return response;
        }
    }
}