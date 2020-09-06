using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace daniel.dal
{
    public static class DbHelper
    {
        private static string ConnStr()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: false)
          .Build();
            return configuration.GetConnectionString("DefaultConnection");
        }

        private static string AdminConnStr()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: false)
          .Build();
            return configuration.GetConnectionString("AdminConnection");
        }

        private static MySqlConnection CreateConnection(bool isAdmin = false)
        {
            string strConn = ConnStr();
            //管理员帐户
            if (isAdmin)
            {
                strConn = AdminConnStr();
            }

            MySqlConnection conn = new MySqlConnection(strConn);
            conn.Open();
            return conn;
        }

        public static object ExecuteScalar(string sql, CommandType cmdType, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = CreateConnection())
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = cmdType;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        public static MySqlDataReader ExecuteReader(string sql, params MySqlParameter[] ps)
        {
            using (MySqlConnection conn = CreateConnection())
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                if (ps != null)
                {
                    cmd.Parameters.AddRange(ps);
                }
                return cmd.ExecuteReader();
            }
        }
        public static DataTable ExecuteDataTable(string sql, params MySqlParameter[] ps)
        {
            DataTable table = new DataTable();
            using (MySqlConnection conn = CreateConnection())
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                if (ps != null)
                {
                    cmd.Parameters.AddRange(ps);
                }
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
                return table;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="isAdmin">是否连接管理员表</param>
        /// <param name="sql"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(bool isAdmin, string sql, params MySqlParameter[] ps)
        {
            DataTable table = new DataTable();
            using (MySqlConnection conn = CreateConnection(isAdmin))
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                if (ps != null)
                {
                    cmd.Parameters.AddRange(ps);
                }
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(table);
                }
                return table;
            }
        }

        #region Transaction
        [ThreadStatic]
        private static MySqlTransaction myTrans = null;

        public static void BeginTransaction()
        {
            if (myTrans == null)
            {
                MySqlConnection conn = new MySqlConnection(ConnStr());
                OpenConn(conn);
                myTrans = conn.BeginTransaction();
            }
        }


        public static void CommitTransaction()
        {
            if (myTrans == null) return;
            myTrans.Commit();
            ReleaseTransaction();
        }

        public static void RollbackTransaction()
        {
            if (myTrans == null) return;
            myTrans.Rollback();
            ReleaseTransaction();
        }

        private static void ReleaseTransaction()
        {
            if (myTrans == null) return;
            MySqlConnection conn = myTrans.Connection;
            myTrans.Dispose();
            CloseConn(conn);
            myTrans = null;
        }
        #endregion

        #region Connection

        private static void OpenConn(MySqlConnection conn)
        {
            if (conn == null) conn = new MySqlConnection(ConnStr());
            if (conn.State == ConnectionState.Closed) conn.Open();
        }

        private static void CloseConn(MySqlConnection conn)
        {
            if (conn == null) return;
            if (conn.State == ConnectionState.Open) conn.Close();
            conn.Dispose();
        }
        #endregion

        #region ExecuteNonQuery

        public static int ExecuteNonQuery(bool isAdmin, string cmdText, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = CreateConnection(isAdmin))
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }

        }

        public static int ExecuteNonQuery(string cmdText, params MySqlParameter[] parameters)
        {
            if (myTrans != null)
            {
                MySqlConnection conn = myTrans.Connection;
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    OpenConn(cmd.Connection);
                    cmd.Transaction = myTrans;
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (MySqlConnection conn = CreateConnection())
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }

        }
        #endregion

        #region  ExecuteScalar
        public static object ExecuteScalar(bool isAdmin, string cmdText, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = CreateConnection(isAdmin))
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        public static object ExecuteScalar(string cmdText, params MySqlParameter[] parameters)
        {
            if (myTrans != null)
            {
                MySqlConnection conn = myTrans.Connection;
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    OpenConn(cmd.Connection);
                    cmd.Transaction = myTrans;
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
            else
            {
                using (MySqlConnection conn = CreateConnection())
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
        #endregion

        #region +ExecuteDataTableAsync 异步查询操作

        public static async Task<DataTable> ExecuteDataTableAsync(string cmdText, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                DataSet ds = await MySqlHelper.ExecuteDatasetAsync(conn, cmdText, parameters).ConfigureAwait(false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return new DataTable();
            }
        }

        #endregion

        #region +ExecuteScalarAsync 异步查询操作

        /// <summary>
        /// 异步查询操作
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<object> ExecuteScalarAsync(string cmdText, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                return await MySqlHelper.ExecuteScalarAsync(conn, cmdText, parameters).ConfigureAwait(false);
                //await Task.CompletedTask;
                //return obj;
            }
        }
        #endregion
    }
}
