using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using INF2course.Attributes;
using INF2course.DAO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace INF2course.Controllers
{
    [HttpController("accounts")]
    public class Accounts: BaseController
    {
        DAOAccount dAO=new DAOAccount();

        [HttpGET("getaccountbyid")]
        public AccountInfo GetAccountById(int id)
        {
            //return dAO.GetById(id);

            AccountInfo account = null;
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
            string sqlExpression = "select * from [dbo].[Table]";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();


                if (reader.HasRows)//если есть данные 
                {
                    while (reader.Read())//Построчно считываем данные
                    {
                        if (reader.GetInt32(0) == id)
                        {
                            account = new AccountInfo
                         (
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2));
                            break;
                        }
                    }
                }
                reader.Close();
            }
            return account;
        }

        [HttpGET("getaccounts")]
        public List<AccountInfo> GetAccounts()
        {
            var cookie = Request.Cookies.FirstOrDefault(x => x.Name == "SessionId");
            if (cookie == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401
                return null;
            }
            AuthInfo auth = System.Text.Json.JsonSerializer.Deserialize< AuthInfo>(cookie.Value.Replace("@", ","));
            if (!auth.IsAuthorized)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401
                return null;
            }
            return dAO.GetAll();
            //List<AccountInfo> accounts = new List<AccountInfo>();
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            //string sqlExpression = "select * from [dbo].[Table]";
            //using (var connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    SqlCommand command = new SqlCommand(sqlExpression, connection);
            //    SqlDataReader reader = command.ExecuteReader();

            //    //если есть данные 
            //    if (reader.HasRows)
            //    {

            //        //Построчно считываем данные
            //        while (reader.Read())
            //        {
            //            accounts.Add(new AccountInfo
            //            (reader.GetInt32(0),
            //             reader.GetString(1),
            //             reader.GetString(2))
            //                    );
            //        }
            //    }
            //    reader.Close();
            //}
            //return accounts;
        }

        [HttpPOST("saveaccount")]
        public bool SaveAccount(string login = "test", string password = "test")
        {
            
            AccountInfo existingAccount = dAO.GetByColumnValue("login", login);
            if (existingAccount != null)
            {
                return true;
            }

            AccountInfo accountInfo = new AccountInfo
            {
                Login = login,
                Password = password
            };
            dAO.Insert(accountInfo);

            return false;
        }

        [HttpPOST("login")]
        public bool Login(string login, string password)
        {
            AccountInfo existingAccount = dAO.GetByColumnValue("login", login);
            if (existingAccount != null && existingAccount.Password == password)
            {
                string cookie = System.Text.Json.JsonSerializer.Serialize(new AuthInfo { IsAuthorized = true, UserId = existingAccount.Id });
                Response.Cookies.Add(new Cookie("SessionId", cookie.Replace(",", "@")));
                return true;
            }

            return false;
        }

        /* public static async void SaveAccount(string login = "test3", string password = "test3")
         {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

             // выражение SQL для добавления данных
             string sqlExpression = "INSERT INTO [dbo].[Table] (Login, Password) VALUES (@login, @password)";

             using (SqlConnection connection = new SqlConnection(connectionString))
             {
                 await connection.OpenAsync();
                 SqlCommand command = new SqlCommand(sqlExpression, connection);
                 // создаем параметр для логина
                 SqlParameter loginParam = new SqlParameter("@login", login);
                 // добавляем параметр к команде
                 command.Parameters.Add(loginParam);
                 // создаем параметр для пароля
                 SqlParameter passwordParam = new SqlParameter("@password", password);
                 // добавляем параметр к команде
                 command.Parameters.Add(passwordParam);
                 // записываем строку в бд
                 await command.ExecuteNonQueryAsync();
             }
         }*/
    }
}
