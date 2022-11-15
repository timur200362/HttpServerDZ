using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INF2course.Week_6.Attributes
{
    public class HttpPOST:Attribute
    {
        public string MethodURI { get; set; }
        public HttpPOST(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}
