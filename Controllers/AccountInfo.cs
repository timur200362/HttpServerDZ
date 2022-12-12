using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INF2course.Controllers
{
    public class AccountInfo
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public AccountInfo(int id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
        }
        public AccountInfo() { }

        public void Show()
        {
            Console.WriteLine("Id: "+Id+";Login: "+Login+";Password: "+Password);
        }
    }
}
