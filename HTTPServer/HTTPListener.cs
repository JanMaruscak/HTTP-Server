using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using HTTPServer.Handlers;

namespace HTTPServer
{
    public class HTTPListener
    {
        private Dictionary<HttpMethod, List<Handler>> Handlers = new Dictionary<HttpMethod, List<Handler>>();
        private List<Thread> Threads = new List<Thread>();
        private int Port = 80;
        private IPAddress IP = IPAddress.Any;
        private TcpListener Server;
        public bool WriteHeadersIntoConsole = true;

        public HTTPListener()
        {
            Server = new TcpListener(IP, Port);
        }

        public HTTPListener(IPAddress ip, int port)
        {
            Port = port;
            IP = ip;
            Server = new TcpListener(IP, Port);
        }

        public void Start()
        {
            Server.Start();
            Console.WriteLine("HTTP server running...");
            Console.WriteLine($"IP: {IP}");
            Console.WriteLine($"Port: {Port}");
            Console.WriteLine();
            while (true)
            {
                var remoteClient = Server.AcceptTcpClient();
                var thread = new Thread(new ParameterizedThreadStart(ProcessClient));
                lock (Threads)
                {
                    Threads.Add(thread);
                }
                thread.IsBackground = true;
                thread.Start(remoteClient);
            }
        }

        public void ProcessClient(object client)
        {
            var remoteClient = client as TcpClient;
            var clientStream = remoteClient.GetStream();
            bool handled = false;

            using (var sr = new StreamReader(clientStream))
            using (var sw = new StreamWriter(clientStream))
            {
                RequestMessage request = GetRequest(remoteClient, sr);

                if (request.HttpMethod != null && Handlers.ContainsKey(request.HttpMethod))
                {
                    foreach (var handler in Handlers[request.HttpMethod])
                    {
                        if (handler.CanHandle(request))
                        {
                            var response = handler.Process(request);
                            sw.WriteLine($"HTTP/1.0 {(int)response.StatusCode} {response.StatusCode}");
                            foreach (var header in response.Headers)
                            {
                                sw.WriteLine($"{header.Key}: {header.Value}");
                            }
                            sw.WriteLine();
                            sw.Flush();
                            if (response.Body != null)
                            {
                                response.Body.Position = 0;
                                response.Body.CopyTo(clientStream);
                                response.Body.Dispose();
                            }
                            handled = true;
                            break;
                        }
                    }
                }
                if (!handled)
                {
                    sw.WriteLine($"HTTP/1.0 404 Not Found");
                    sw.WriteLine("Content-Type: text/html; charset=utf-8");
                    sw.WriteLine();
                    sw.WriteLine("<html><body><h1>Error: 404 Not Found</h1></body></html>");
                }
            }
            clientStream.Close();
            lock (Threads)
            {
                Threads.Remove(Thread.CurrentThread);
            }
        }

        public RequestMessage GetRequest(TcpClient remoteClient, StreamReader sr)
        {
            RequestMessage requestMessage = new RequestMessage();

            // Headers
            var firstLine = "";
            var line = sr.ReadLine();
            firstLine = line;
            line = sr.ReadLine();
            while (line != null && line != "")
            {
                var headerSplit = line.Split(": ");
                requestMessage.Headers.Add(headerSplit[0], headerSplit[1]);
                if (WriteHeadersIntoConsole)
                    Console.WriteLine(line);

                line = sr.ReadLine();
            }
            if (WriteHeadersIntoConsole)
                Console.WriteLine();

            // Method and query
            if (firstLine != null)
            {
                var split = firstLine.Split(' ');
                requestMessage.HttpMethod = new HttpMethod(split[0]);
                requestMessage.Query = split[1];
            }

            // Body
            if (requestMessage.Headers.ContainsKey("Content-Length"))
            {
                var len = int.Parse(requestMessage.Headers["Content-Length"]);
                char[] buffer = new char[len];
                sr.Read(buffer, 0, len);

                requestMessage.Body = new string(buffer);
            }

            return requestMessage;
        }

        public void AddHandler(HttpMethod httpMethod, Handler handler)
        {
            if (!Handlers.ContainsKey(httpMethod))
            {
                Handlers.Add(httpMethod, new List<Handler>());
            }

            Handlers[httpMethod].Add(handler);
        }
    }
}