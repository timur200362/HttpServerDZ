using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace INF2course.Week_4
{
    enum ServerStatus { Start, Stop };
    internal class HttpServer
    {
        public ServerStatus Status = ServerStatus.Stop;
        private readonly HttpListener http_listener;
        public int Port { get; set; }
        public string Path { get; set; }
        public string MainPage { get; set; }
        public HttpServer(int port, string path, string main_page)
        {
            Port = port; Path = path; MainPage = main_page;
            http_listener = new HttpListener();
            http_listener.Prefixes.Add($"http://localhost:" + Port + "/");
        }
        public void Start()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже запущен");
                return;
            }
            http_listener.Start();
            Console.WriteLine("Сервер запущен");
            Status = ServerStatus.Start;
            Listening();
        }
        public void Stop()
        {
            if (Status == ServerStatus.Stop)
            {
                Console.WriteLine("Сервер уже остановлен");
                return;
            }
            Status = ServerStatus.Stop;
            http_listener.Stop();
            Console.WriteLine("Сервер остановлен");
        }
        private void Listening()
        {
            http_listener.BeginGetContext(new AsyncCallback(ListenerCallBack), http_listener);
        }
        private void ListenerCallBack(IAsyncResult result)
        {
            if (http_listener.IsListening && Status != ServerStatus.Stop)
            {
                var _httpContext = http_listener.EndGetContext(result);
                HttpListenerRequest request = _httpContext.Request;
                HttpListenerResponse response = _httpContext.Response;
                byte[] buffer;
                if (Directory.Exists(Path))
                {
                    buffer = getFile(_httpContext.Request.RawUrl, MainPage);
                    //Console.ReadKey();
                }
                else
                {
                    string err = $"Directory " + Path + " not found";
                    buffer = Encoding.UTF8.GetBytes(err);
                }
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                Listening();
            }
        }
        private byte[] getFile(string rawUrl, string main_page)
        {
            byte[] buffer = null;
            var filePath = Path + rawUrl;

            if (Directory.Exists(filePath))
            {
                filePath += main_page;
                if (File.Exists(filePath))
                    buffer = File.ReadAllBytes(filePath);
                //Console.ReadKey();
            }
            else if (File.Exists(filePath))
                buffer = File.ReadAllBytes(filePath);
            return buffer;
        }
    }
}
