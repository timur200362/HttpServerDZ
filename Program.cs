using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using INF2course.Controllers;
using INF2course.DAO;

namespace INF2course
{
    public class Program
    {
        private static bool _appIsRunning = true;
        static void Main(string[] args)
        {
            StreamReader SR = new StreamReader("settings.json");
            string open_json = SR.ReadLine();
            SR.Close();
            HttpServer3 server = JsonSerializer.Deserialize<HttpServer3>(open_json);

            server.Init();
            server.Start();
            while (_appIsRunning)
                Handler(Console.ReadLine()?.ToLower(), server);
        }
        static void Handler(string command, HttpServer3 server)
        {
            switch (command)
            {
                case "stop": server.Stop(); break;
                case "restart": server.Start(); server.Stop(); break;
                case "start": server.Start(); break;
                case "status": Console.WriteLine(server.Status.ToString()); break;
                case "exit": _appIsRunning = false; break;
            }
        }
    }
}
//ORM
//string str= @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
//MyORM myORM= new MyORM(str);
//var result = new MyORM(str).ExecuteScalar<Account>("select Id,Login,Password from [dbo].[Table]");
//var result = new MyORM(str)
//    .AddParameter<string>("@Login", "SomeTea")
//   .AddParameter<string>("@Password", "TeaPassword").ExecuteNonQuery("insert into [dbo].[Table] (Login,Password) values(@Login,@Password)");
//var result2 = new MyORM(str).ExecuteQuery<int>("insert into [dbo].[Table] (Login,Password) values('SomeTea','TeaPassword')");
//var result3 = new MyORM(str).Select<Account>();
//foreach(var e in result3)
//{
//    e.Show();
//}
//new MyORM(str).Insert<AccountInfo>(new AccountInfo(5,"testLogin","testPassword"));

//DAO
//DAOAccount Dao = new DAOAccount();
//Dao.Delete(new AccountInfo(1010, "SomeTea", "TeaPassword"));
