using INF2course.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INF2course.DAO
{
    internal class DAOAccount: IDAO<AccountInfo>
    {
        const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
        MyORM MyORM = new MyORM(connectionString);
        public void Insert(AccountInfo model)
        {
            MyORM.Insert<AccountInfo>(model);
        }
        public void Delete(AccountInfo model)
        {
            MyORM.Delete<AccountInfo>(model);
        }
        public List<AccountInfo> GetAll()
        {
            return MyORM.Select<AccountInfo>().ToList();
        }
        public AccountInfo GetById(int id)
        {
            return MyORM.AddParameter("@id", id).ExecuteQuery<AccountInfo>($"SELECT * FROM [dbo].[Accounts] WHERE Id = @id").FirstOrDefault();
        }
        
        public AccountInfo GetByColumnValue(string column, object value)
        {
            return MyORM.AddParameter("@columnValue", value).ExecuteQuery<AccountInfo>($"SELECT * FROM [dbo].[Accounts] WHERE {column} = @columnValue").FirstOrDefault();
        }
    }
}
