using INF2course.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INF2course.Repository
{
    internal class AccountRepository : IRepository<AccountInfo>
    {
        const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
        MyORM orm = new MyORM(connectionString);
        public void Add(AccountInfo account)
        {
            orm.Insert<AccountInfo>(account);
        }
        public void Update(AccountInfo account)
        {
            orm.AddParameter("@login", account.Login)
                .AddParameter("@password", account.Password)
                .ExecuteNonQuery("UPDATE dbo.Accounts SET Password = @password WHERE Login = @login");
        }
        public void Delete(AccountInfo account)
        {
            orm.Delete<AccountInfo>(account);
        }
        public IEnumerable<AccountInfo> GetAll()
        {
            return orm.Select<AccountInfo>().ToList();
        }
        public AccountInfo GetById(AccountInfo account)
        {
            return orm.AddParameter("@id", account.Id).ExecuteQuery<AccountInfo>("SELECT * FROM dbo.Accounts WHERE Id = @id").FirstOrDefault();
        }
    }
}
