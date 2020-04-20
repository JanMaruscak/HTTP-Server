using System;
using System.IO;
using System.Net.Http;
using HTTPServer.Handlers;

namespace HTTPServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HTTPListener httpListener = new HTTPListener();
            httpListener.AddHandler(HttpMethod.Get, new FolderHandler("/", Path.Combine(Environment.CurrentDirectory, "wwwroot")));
            httpListener.AddHandler(HttpMethod.Get, new FileHandler("/kybernalogo", Path.Combine(Environment.CurrentDirectory, "wwwroot", "ssakhk.png")));
            httpListener.AddHandler(HttpMethod.Get, new RedirectHandler("/", "/index.html"));
            httpListener.AddHandler(HttpMethod.Get, new RedirectHandler("/portfolio", "https://janmaruscak.cz"));
            httpListener.AddHandler(HttpMethod.Get, new RedirectHandler("/kyberna", "https://kyberna.cz/"));
            httpListener.AddHandler(HttpMethod.Post, new FunctionHandler("/test", TestFunc));
            httpListener.AddHandler(HttpMethod.Get, new FunctionHandler("/test2", TestFunc2));
            httpListener.WriteHeadersIntoConsole = true;
            httpListener.Start();
        }

        public static ResponseMessage TestFunc(RequestMessage req)
        {
            var dataString = string.IsNullOrWhiteSpace(req.Body) ? "NULL" : req.Body;
            Console.WriteLine("Data is: " + dataString);
            return new ResponseMessage("Data recieved");
        }

        public static ResponseMessage TestFunc2(RequestMessage req)
        {
            var response = new ResponseMessage("Testovaci funkce na metodu: ");
            response.StreamWriter.Write("GET.");
            response.StreamWriter.Flush();
            return response;
        }
    }
}