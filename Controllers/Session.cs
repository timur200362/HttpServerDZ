using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INF2course.Controllers
{
    internal class Session
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
