using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HTTPServer.Handlers
{
    public class RedirectHandler : Handler
    {
        private string RedirectTo;

        public RedirectHandler(string url, string redirectTo)
        {
            URL = url;
            RedirectTo = redirectTo;
        }

        public override bool CanHandle(RequestMessage request)
        {
            if (request.Path != URL) return false;
            return true;
        }

        public override ResponseMessage Process(RequestMessage request)
        {
            ResponseMessage response = new ResponseMessage();
            response.Headers.Add("Location", RedirectTo);
            response.Headers.Add("Content-Type", "text/plain");
            response.StatusCode = HttpStatusCode.MovedPermanently;

            return response;
        }
    }
}