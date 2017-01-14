using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public static class DatabaseManager
    {
        private static bool IsInit = false;
        public static void init(string name = "default.sql")
        {
            if (!IsInit)
            {
                DatabaseName = name;
                connection = new SQLiteAsyncConnection(DatabaseName);
                IsInit = true;
            }
        }
        private static string DatabaseName;
        private static SQLiteAsyncConnection connection;

        public static async Task<CreateTablesResult> CreateTables<T>() where T : new()
        {
            return await connection.CreateTableAsync<T>();
        }

        public static async Task<int> DropTables<T>(string dbname) where T : new()
        {
            return await connection.DropTableAsync<T>();
        }

        public static async Task<int> Insert<T>(T item) where T : new()
        {
            return await connection.InsertAsync(item);
        }

        public static async Task<int> InsertAll<T>(List<T> items) where T : new()
        {
            return await connection.InsertAllAsync(items);
        }

        public static async Task<List<T>> Query<T>(string sql, params object[] args) where T : new()
        {
            return await connection.QueryAsync<T>(sql, args);
        }

        public static async Task<T> ExecuteAsync<T>(string sql, params object[] args) where T : new()
        {
            return await connection.ExecuteScalarAsync<T>(sql, args);
        }
    }
}
