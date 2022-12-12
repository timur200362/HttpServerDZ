using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INF2course.DAO
{
    public interface IDAO<T>
    {
        void Insert(T model);

        void Delete(T model);

        List<T> GetAll();

        T GetById(int id);

        T GetByColumnValue(string columnValue, object value);
    }
}
