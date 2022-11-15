using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;
using System.Text.Json;
using INF2course.Week_6.Attributes;
//using INF2course.Week_6.Controllers;

namespace INF2course.Week_6
{
    enum ServerStatus { Start, Stop };
    internal class HttpServer3
    {
        public int Port { get; set; }
        public string Path { get; set; }
        public string MainPage { get; set; }
        public ServerStatus Status = ServerStatus.Stop;
        private readonly HttpListener http_listener;
        public HttpServer3() { http_listener = new HttpListener(); }
        public HttpServer3(int port, string path, string main_page)
        {
            Port = port; Path = path; MainPage = main_page;
            http_listener = new HttpListener();
            http_listener.Prefixes.Add($"http://localhost:" + Port + "/");
        }
        public void Init()
        {
            http_listener.Prefixes.Add($"http://localhost:" + Port + "/");
        }
        public void Start()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже запущен");
                return;
            }
            Console.WriteLine("Запуск сервера...");
            http_listener.Start();
            Console.WriteLine("Сервер запущен");
            Status = ServerStatus.Start;
            Listening();
        }
        public void Stop()
        {
            if (Status == ServerStatus.Stop) return;
            Status = ServerStatus.Stop;
            Console.WriteLine("Остановка сервера...");
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
            }
            else if (File.Exists(filePath))
                buffer = File.ReadAllBytes(filePath);
            return buffer;
        }
        private bool MethodHandler(HttpListenerContext _httpContext)
        {
            // объект запроса
            HttpListenerRequest request = _httpContext.Request;

            // объект ответа
            HttpListenerResponse response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

            string[] strParams = _httpContext.Request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController))).FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var test = typeof(HttpController).Name;
            var method = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                                                              .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"))
                                                 .FirstOrDefault();

            if (method == null) return false;

            object[] queryParams = method.GetParameters()
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;
        }
        //private bool MethodHandler(HttpListenerContext _httpContext)
        //{
        //    // объект запроса
        //    HttpListenerRequest request = _httpContext.Request;

        //    // объект ответа
        //    HttpListenerResponse response = _httpContext.Response;

        //    if (_httpContext.Request.Url.Segments.Length < 2) return false;

        //    string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

        //    string[] strParams = _httpContext.Request.Url
        //                            .Segments
        //                            .Skip(2)
        //                            .Select(s => s.Replace("/", ""))
        //                            .ToArray();
        //    if (strParams.Length == 0) return false;
        //    string methodURI = strParams[0];

        //    var assembly = Assembly.GetExecutingAssembly();

        //    var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController))).FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

        //    if (controller == null) return false;

        //    var test = typeof(HttpController).Name;
        //    var method = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
        //                                                      .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"));

        //    var method2 = method.FirstOrDefault(x => _httpContext.Request.HttpMethod switch
        //    {
        //        "GET" => x.GetCustomAttribute<HttpGET>()?.MethodURI == methodURI,
        //        "POST" => x.GetCustomAttribute<HttpPOST>()?.MethodURI == methodURI
        //    });
        //    //if (method == null) return false;

        //    //object[] queryParams = method.GetParameters()
        //    //                    .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
        //    //                    .ToArray();

        //    //var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

        //    //response.ContentType = "Application/json";

        //    //byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
        //    //response.ContentLength64 = buffer.Length;

        //    //Stream output = response.OutputStream;
        //    //output.Write(buffer, 0, buffer.Length);

        //    //output.Close();

        //    //return true;
        //    object[] queryParams = null;

        //    if (request.HttpMethod == "POST")
        //    {
        //        ShowRequestData(request);
        //    }
        //    else
        //    {
        //        switch (methodURI)
        //        {
        //            case "getaccounts":
        //                //параметров нет
        //                break;
        //            case "getaccountbyid":
        //                object[] temp = new object[1] { Convert.ToInt32(strParams[1]) };
        //                queryParams = temp;
        //                break;
        //            case "saveaccount":
        //                //колхоз, как красиво написать?? (чтобы не переименовывать переменную)
        //                object[] temp1 = new object[2] { Convert.ToString(strParams[1]), Convert.ToString(strParams[2]) };
        //                queryParams = temp1;
        //                break;

        //        }
        //    }


        //    var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

        //    response.ContentType = "Application/json";

        //    byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
        //    response.ContentLength64 = buffer.Length;

        //    Stream output = response.OutputStream;
        //    output.Write(buffer, 0, buffer.Length);

        //    output.Close();

        //    Listening();

        //    return true;
        //}
        //public object[] ShowRequestData(HttpListenerRequest request)
        //{
        //    if (!request.HasEntityBody)
        //    {
        //        Console.WriteLine("No client data was sent with the request.");
        //        return null;
        //    }
        //    Stream body = request.InputStream;
        //    Encoding encoding = request.ContentEncoding;
        //    StreamReader reader = new StreamReader(body, encoding);
        //    if (request.ContentType != null)
        //    {
        //        Console.WriteLine("Client data content type {0}", request.ContentType);
        //    }
        //    Console.WriteLine("Client data content length {0}", request.ContentLength64);

        //    Console.WriteLine("Start of client data:");
        //    // Convert the data to a string and display it on the console.
        //    string s = reader.ReadToEnd();
        //    Console.WriteLine(s);
        //    Console.WriteLine("End of client data:");
        //    body.Close();
        //    reader.Close();
        //    object[] paramsA = null;


        //    // If you are finished with the request, it should be closed also.
        //    return paramsA;
        //}
    }
}
