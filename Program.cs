using System;
using INF2course.Week_4;
using INF2course.Week_5;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace INF2course
{
    //WEEK4
    //class ServerClass
    //{
    //    private bool running = true;
    //    public bool Running { get => running; set { running = value; } }
    //    public void Execute()
    //    {
    //        HttpServer server = new HttpServer(7700, @"./google", "google2.html");
    //        server.Start();
    //        while (running)
    //            Handler(Console.ReadLine()?.ToLower(), server);
    //    }
    //    delegate void cmd(HttpServer server, ServerClass server_class);
    //    static void Stop(HttpServer server, ServerClass server_class) { server.Stop(); }
    //    static void Restart(HttpServer server, ServerClass server_class) { server.Start(); server.Stop(); }
    //    static void Start(HttpServer server, ServerClass server_class) { server.Start(); }
    //    static void Exit(HttpServer server, ServerClass server_class) { server_class.Running = false; }

    //    Dictionary<string, cmd> dict = new Dictionary<string, cmd> { { "stop", Stop }, { "start", Start }, { "restart", Restart }, { "exit", Exit } };
    //    void Handler(string command, HttpServer server)
    //    {
    //        if (dict.ContainsKey(command))
    //            dict[command](server, this);
    //    }
    //}
    //public class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        ServerClass server_class = new ServerClass();
    //        server_class.Execute();
    //    }
    //}
    
    //Week5
    class ServerClass
    {
        private bool running = true;
        public bool Running { get => running; set { running = value; } }
        public void Execute()
        {
            StreamReader SR = new StreamReader("settings.json");
            string open_json = SR.ReadLine();
            SR.Close();
            HttpServer2 server = JsonSerializer.Deserialize<HttpServer2>(open_json);


            //HttpServer server = new HttpServer(7700, @"./google", "index.html");
            server.Init();
            server.Start();
            while (running)
                Handler(Console.ReadLine()?.ToLower(), server);
        }
        delegate void cmd(HttpServer2 server, ServerClass server_class);
        static void Stop(HttpServer2 server, ServerClass server_class) { server.Stop(); }
        static void Restart(HttpServer2 server, ServerClass server_class) { server.Stop(); server.Start(); }
        static void Start(HttpServer2 server, ServerClass server_class) { server.Start(); }
        static void Exit(HttpServer2 server, ServerClass server_class) { server_class.Running = false; }

        Dictionary<string, cmd> dict = new Dictionary<string, cmd> { { "stop", Stop }, { "start", Start }, { "restart", Restart }, { "exit", Exit } };
        void Handler(string command, HttpServer2 server)
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
