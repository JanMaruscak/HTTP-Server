using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace HTTPServer
{
    public class ResponseMessage
    {
        public HttpStatusCode StatusCode;
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public Stream Body = null;
        public StreamWriter StreamWriter;

        public ResponseMessage()
        {
        }

        public ResponseMessage(string content)
        {
            Body = new MemoryStream();
            StreamWriter = new StreamWriter(Body);
            StreamWriter.Write(content);
            StreamWriter.Flush();
            StatusCode = HttpStatusCode.OK;
            Headers.Add("Content-Type", "text/html");
        }
    }
}