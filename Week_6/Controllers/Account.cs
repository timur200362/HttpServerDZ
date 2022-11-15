using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INF2course.Week_6.Attributes;

namespace INF2course.Week_6.Controllers
{
    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Account(int id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
        }
    }
}
