using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace INF2course
{
    internal class MyORM : IDisposable 
    {
        public IDbConnection _connection = null;
        public IDbCommand _cmd = null;
        private bool disposedValue;

        public MyORM(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _cmd = _connection.CreateCommand();
        }
        public MyORM AddParameter<T>(string name, T value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.Value = value;
            _cmd.Parameters.Add(param);
            return this;
        }
        public int ExecuteNonQuery(string query)
        {
            int noOfAffectedRows = 0;
           
                _cmd.CommandText = query;
                noOfAffectedRows = _cmd.ExecuteNonQuery();
            
            return noOfAffectedRows;
        }
        public IEnumerable<T> ExecuteQuery<T>(string query)
        {
            IList<T> list = new List<T>();
            Type t = typeof(T);
           
                _cmd.CommandText = query;
                using var reader = _cmd.ExecuteReader();
                while (reader.Read())
                {
                    T obj = (T)Activator.CreateInstance(t);
                    t.GetProperties().ToList().ForEach(p =>
                    {
                        p.SetValue(obj, reader[p.Name]);
                    });
                    list.Add(obj);
                }
            

            return list;
        }
        public T ExecuteScalar<T>(string query)
        {
            T result = default;
           
                _cmd.CommandText = query;
                result = (T)_cmd.ExecuteScalar();
           
            return result;
        }
        public IEnumerable<T> Select<T>()
        {
            IList<T> list = new List<T>();
            Type t = typeof(T);

           
                string sqlExpression = $"SELECT * FROM [dbo].[{GetTableName(typeof(T))}]";
                _cmd.CommandText = sqlExpression;
                using var reader = _cmd.ExecuteReader();

                while (reader.Read())
                {
                    T obj = (T)Activator.CreateInstance(t);
                    t.GetProperties().ToList().ForEach(p =>
                    {
                        p.SetValue(obj, reader[p.Name]);
                    });

                    list.Add(obj);
                }
            
            return list;
        }

        public void Insert<T>(T model)
        {
            
               PropertyInfo[] modelFields = model.GetType().GetProperties().Where(p => !p.Name.Equals("Id")).ToArray();
                List<string> parameters = modelFields.Select(x => $"@{x.Name}").ToList();
                Console.WriteLine(string.Join(",", parameters));
                string sqlExpression = $"INSERT INTO [dbo].[{GetTableName(typeof(T))}] ({string.Join(",", modelFields.Select(f => f.Name))}) VALUES ({string.Join(", ", parameters)})";
                _cmd.CommandText = sqlExpression;
                foreach (var field in modelFields)
                {
                    _cmd.Parameters.Add(new SqlParameter($"@{field.Name}", field.GetValue(model)));
                }
                _cmd.ExecuteNonQuery();
            
        }

        public void Delete<T>(T model)
        {
            
                PropertyInfo[] modelFields = model.GetType().GetProperties().Where(p => p.Name.Equals("Id")).ToArray();
                List<string> parameters = modelFields.Select(x => $"@{x.Name}").ToList();
                string sqlExpression = $"DELETE FROM [dbo].[{GetTableName(typeof(T))}] WHERE {string.Join(",", modelFields.Select(f => $"{f.Name}=@{f.Name}").ToList())}";
                _cmd.CommandText = sqlExpression;
                Console.WriteLine(sqlExpression);
                foreach (var field in modelFields)
                {
                    _cmd.Parameters.Add(new SqlParameter($"@{field.Name}", field.GetValue(model)));
                }                
                _cmd.ExecuteNonQuery();
            
        }
        //public void Update<T>(T model)
        //{
        //    using (_connection)
        //    {
        //        PropertyInfo[] modelFields = model.GetType().GetProperties().Where(p => p.Name.Equals("Id")).ToArray();
        //        List<string> parameters = modelFields.Select(x => $"@{x.Name}").ToList();
        //        var collection = from p in modelFields where p.Name!="Id" select p;
        //        string sqlExpression = $"UPDATE [dbo].[Table] SET {string.Join(",", collection.Select(f => $"{f.Name}=@{f.Name}").ToList())} WHERE Id=" + modelFields[0].GetValue(model);
        //        _cmd.CommandText = sqlExpression;
        //        Console.WriteLine(sqlExpression);
        //        foreach (var field in modelFields)
        //        {
        //            _cmd.Parameters.Add(new SqlParameter($"@{field.Name}", field.GetValue(model)));
        //        }

        //        _connection.Open();
        //        _cmd.ExecuteNonQuery();
        //    }
        //}

        protected string GetTableName(Type t)
        {
            string tableName = t.Name;
            if (tableName.EndsWith("Info"))
            {
                tableName = tableName.Substring(0, tableName.Length - 4);
            }
            return tableName + "s";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MyORM()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
