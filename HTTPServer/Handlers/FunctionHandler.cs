using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HTTPServer.Handlers
{
    public class FunctionHandler : Handler
    {
        private Func<RequestMessage, ResponseMessage> Function;

        public FunctionHandler(string url, Func<RequestMessage, ResponseMessage> function)
        {
            URL = url;
            Function = function;
        }

        public override bool CanHandle(RequestMessage request)
        {
            if (URL != request.Query) return false;
            return true;
        }

        public override ResponseMessage Process(RequestMessage request)
        {
            return Function.Invoke(request);
        }
    }
}