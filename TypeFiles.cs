using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace INF2course
{
    internal class TypeFiles
    {
        public static void GetExtension(ref HttpListenerResponse response, string path)
        {
            response.ContentType = Path.GetExtension(path) switch
            {
                ".html" => "text/html",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".svg" => "image/svg+xml",
                ".gif" => "image/gif",
                ".js" => "text/javascript",
                ".css" => "text/css",
                ".ico" => "image/x-icon",
                _ => "text/plain",
            };
        }
    }
}
