using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace HTTPServer
{
    public class RequestMessage
    {
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public HttpMethod HttpMethod;
        public string Path;
        public string Query;
        public string Body = null;

        public RequestMessage()
        {
        }
    }
}