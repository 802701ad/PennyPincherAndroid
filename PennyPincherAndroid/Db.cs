using Android.Util;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
namespace PennyPincher
{
    class Db
    {
        private static string db_file = "PennyPincher.sqlite";
        private static SqliteConnection GetConnection()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db_file);
            Log.Error("info", dbPath);
            bool exists = File.Exists(dbPath);
            if (!exists)
                SqliteConnection.CreateFile(dbPath);//throw new Exception(db_file+"  not found in "+dbPath);//
            var conn = new SqliteConnection("Data Source=" + dbPath);
            if (!exists)
                CreateDatabase(conn);
            return conn;
        }

        private static void CreateDatabase(SqliteConnection connection)
        {
            var l = new List<string>()
            {
                @"
                CREATE TABLE IF NOT EXISTS [T_ACCOUNT] (
	                [ACCOUNT_ID] integer, 
	                [ACCOUNT_NAME] nvarchar(4000)
                )
                ",
                @"
                CREATE INDEX [ACCOUNT_ID_IDX]
	                ON [T_ACCOUNT] (
	                [ACCOUNT_ID]
                )
                               ",
                @"
                 CREATE TABLE IF NOT EXISTS  [T_FUND] (
	                [FUND_ID] integer, 
	                [FUND_NAME] nvarchar(4000)
                )
                ",
                @"
                CREATE INDEX [FUND_ID_IDX]
	                ON [T_FUND] (
	                [FUND_ID]
                )
                ",
                 @"
                CREATE TABLE IF NOT EXISTS  [T_TRANSACTION] (
	                [TRANSACTION_ID] integer, 
	                [TRANSACTION_DATE] nvarchar(50), 
	                [ACCOUNT_ID] integer, 
	                [TRANSACTION_TITLE] nvarchar(2000), 
	                [TRANSACTION_COMMENT] ntext, 
	                [IS_ACTIVE] int
                )
                ",
                @"
                CREATE INDEX [TRANSACTION_ACCOUNT_ID_IDX]
	                ON [T_TRANSACTION] (
	                [ACCOUNT_ID]
                )
                ",
                @"CREATE TABLE IF NOT EXISTS  [T_TRANSACTION_DETAIL] (
	                [TRANSACTION_ID] integer, 
	                [FUND_ID] integer, 
	                [ACCOUNT_ID] integer, 
	                [DESCRIPTION] nvarchar(4000), 
	                [AMOUNT] money
                )
                 ",
                @"
                CREATE INDEX [TRANSACTION_DETAIL_FUND_ACCOUNT_IDX]
	                ON [T_TRANSACTION_DETAIL] (
	                [FUND_ID], 
	                [ACCOUNT_ID]
                )
                "
            };

            connection.Open();
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    foreach (string sql in l)
                    {

                        cmd.CommandText = sql;
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            Log.Error("Table Creation Error", sql);
                        }
                    }
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public static DataTable getDataTable(string qry)
        {
            var dt = new DataTable();

            using (var con = GetConnection())
            {
                var da = new SqliteDataAdapter();

                da.SelectCommand = con.CreateCommand();
                da.SelectCommand.CommandText = qry;

                Log.Info("query", qry);
                try
                {
                    da.Fill(dt);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + ": " + qry);
                }
            }

            return dt;
        }

        public static DataTable getAccounts()
        {
            var q = new QueryFormatter();
            q.src = @"
                    SELECT ACCOUNT_ID, ACCOUNT_NAME
                    FROM T_ACCOUNT
            ";
            return getDataTable(q.Format());
        }

        public static List<Account> GetAllAccounts()
        {
            var result = new List<Account>();
            var sql = "SELECT * FROM T_ACCOUNT;";
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(new Account { account_id = Convert.ToInt16(reader["account_id"]), account_name = Convert.ToString(reader["account_name"]) });
                conn.Close();
            }
            return result;
        }

      
    }
}
