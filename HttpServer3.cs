using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Collections.Specialized;
using INF2course.Attributes;
using System.Diagnostics;
using INF2course.Controllers;

namespace INF2course
{
    //public class ServerSetting
    //{
    //    public int Port { get; set; } = 7700;
    //    public string Path { get; set; } = @"./steam";
    //}
    public class HttpServer3 : IDisposable
    {
        public int Port { get; set; }
        public string Path { get; set; }
        public string MainPage { get; set; }
        public ServerStatus Status = ServerStatus.Stop;
        private readonly HttpListener _httpListener;
        public HttpServer3(int port, string path, string main_page)
        {
            Port = port; Path = path; MainPage=main_page;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:" + Port + "/");
        }
        public HttpServer3()
        {
            _httpListener = new HttpListener();
        }
        public void Init()
        {
            _httpListener.Prefixes.Add($"http://localhost:" + Port + "/");
        }
        public void Start()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже запущен");
                return;
            }
            Console.WriteLine("Запуск сервера...");
            _httpListener.Start();
            Console.WriteLine("Сервер запущен");
            Status = ServerStatus.Start;
            Listening();
        }
        public void Stop()
        {
            if (Status == ServerStatus.Stop) return;
            Console.WriteLine("Остановка сервера...");
            _httpListener.Stop();
            Status = ServerStatus.Stop;
            Console.WriteLine("Сервер остановлен");
        }
        private async void Listening()
        {

            while (_httpListener.IsListening && Status != ServerStatus.Stop)
            {
                var http_context = await _httpListener.GetContextAsync();
                if (MethodHandler(http_context)) return;
                StaticFiles(http_context.Request, http_context.Response);
            }
        }
        private byte[] getFile(string rawUrl,string main_page)
        {
            byte[] buffer = null;
            var filePath = Path + rawUrl;

            if (Directory.Exists(filePath))
            {

                filePath = filePath + main_page;

                if (File.Exists(filePath))
                {
                    
                    buffer = File.ReadAllBytes(filePath);
                }

            }
            else if (File.Exists(filePath))
                buffer = File.ReadAllBytes(filePath);
            return buffer;
        }
        private void StaticFiles(HttpListenerRequest request, HttpListenerResponse response)
        {
            byte[] buffer;
            if (Directory.Exists(Path))
            {
                buffer = getFile(request.RawUrl.Replace("%20", " "),MainPage);
                if (buffer == null)
                {
                    response.Headers.Set("Content-Type", "text/plain");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    string err = "404 - not found";
                    buffer = Encoding.UTF8.GetBytes(err);
                }
            }
            else
            {
                string err = $"Directory " + Path + " not found";
                buffer = Encoding.UTF8.GetBytes(err);
            }
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
        private bool MethodHandler(HttpListenerContext context)
        {
            //объект запроса
            HttpListenerRequest request = context.Request;

            //объект ответа
            HttpListenerResponse response = context.Response;

            if (request.Url.Segments.Length < 2) return false;

            string controllerName = request.Url.Segments[1].Replace("/", "");

            string[] strParams = request.Url
                .Segments
                .Skip(2)
                .Select(s => s.Replace("/", ""))
                .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                .FirstOrDefault(c => string.Equals(c.Name, controllerName, StringComparison.CurrentCultureIgnoreCase));

            if (controller == null) return false;

            var test = typeof(HttpController).Name;

            string methodURI = strParams[0];

            var method = controller
                .GetMethods()
                .Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"))
                .FirstOrDefault(x => request.HttpMethod switch
                {
                    "GET" => x.GetCustomAttribute<HttpGET>()?.MethodURI == methodURI,
                    "POST" => x.GetCustomAttribute<HttpPOST>()?.MethodURI == methodURI
                });
             

            NameValueCollection par = new NameValueCollection();

            if (request.HttpMethod == "GET")
            {
                par = request.QueryString;
            }
            else if (request.HttpMethod == "POST")
            {
                if (request.HasEntityBody)
                {
                    string s;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        s = reader.ReadToEnd();
                    }

                    string[] parameters = s.Split("&");
                    foreach (string parameter in parameters)
                    {
                        string[] parSplit = parameter.Split('=');
                        par.Add(parSplit[0], parSplit[1]);
                    }
                }
                else
                {
                    Console.WriteLine("No client data was sent with the request.");
                }
            }

            strParams = new string[par.Count];
            for (int i = 0; i < strParams.Length; i++)
            {
                strParams[i] = par.Get(i);
            }

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            var controllerInstance = Activator.CreateInstance(controller);
            if (controllerInstance is BaseController)
            {
                (controllerInstance as BaseController).Response = response;
            }
            var ret = method.Invoke(controllerInstance      , queryParams);
            
            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;
        }
        public void Dispose()
        {
            Stop();
        }
    }
    //private void ListenerCallBack(IAsyncResult result)
    //{
    //    if (http_listener.IsListening && Status != ServerStatus.Stop)
    //    {
    //        var _httpContext = http_listener.EndGetContext(result);
    //        HttpListenerRequest request = _httpContext.Request;
    //        HttpListenerResponse response = _httpContext.Response;
    //        byte[] buffer;
    //        if (Directory.Exists(Path))
    //        {
    //            buffer = getFile(_httpContext.Request.RawUrl, MainPage);
    //            if (buffer == null)
    //            {
    //                response.Headers.Set("Content-Type", "text/plain");
    //                response.StatusCode = (int)HttpStatusCode.NotFound;
    //                string err = "404 - not found";
    //                buffer = Encoding.UTF8.GetBytes(err);
    //            }
    //        }
    //        else
    //        {
    //            string err = $"Directory " + Path + " not found";
    //            buffer = Encoding.UTF8.GetBytes(err);
    //        }
    //        Stream output = response.OutputStream;
    //        output.Write(buffer, 0, buffer.Length);
    //        output.Close();
    //        Listening();
    //    }
    //}
    
}
