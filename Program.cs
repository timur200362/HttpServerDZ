using System;
using INF2course.Week_4;
using System.Collections.Generic;

namespace INF2course
{
    class ServerClass
    {
        private bool running = true;
        public bool Running { get => running; set { running = value; } }
        public void Execute()
        {
            HttpServer server = new HttpServer(7700, @"./google", "google2.html");
            server.Start();
            while (running)
                Handler(Console.ReadLine()?.ToLower(), server);
        }
        delegate void cmd(HttpServer server, ServerClass server_class);
        static void Stop(HttpServer server, ServerClass server_class) { server.Stop(); }
        static void Restart(HttpServer server, ServerClass server_class) { server.Start(); server.Stop(); }
        static void Start(HttpServer server, ServerClass server_class) { server.Start(); }
        static void Exit(HttpServer server, ServerClass server_class) { server_class.Running = false; }

        Dictionary<string, cmd> dict = new Dictionary<string, cmd> { { "stop", Stop }, { "start", Start }, { "restart", Restart }, { "exit", Exit } };
        void Handler(string command, HttpServer server)
        {
            if (dict.ContainsKey(command))
                dict[command](server, this);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerClass server_class = new ServerClass();
            server_class.Execute();
        }

    }
}
