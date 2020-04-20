using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HTTPServer.Handlers
{
    public abstract class Handler
    {
        private protected string URL;

        public abstract bool CanHandle(RequestMessage request);

        public abstract ResponseMessage Process(RequestMessage request);
    }
}