using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AutomobileApp.DAL
{
    public class SqlConnectionClass
    {
        public static void Using(Action<SqlConnection> action)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                try
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["Auto"].ConnectionString;
                    conn.Open();
                    action(conn);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }

        public static int ExecuteNonQuery(string sql, Dictionary<string, object> parameters, CommandType commandType)
        {
            var result = 0;
            Using(conn =>
            {
                using (var cmd = CreateCommand(conn, sql, parameters, commandType))
                    result = cmd.ExecuteNonQuery();
            });
            return result;
        }

        private static SqlCommand CreateCommand(SqlConnection conn, string sql, Dictionary<string, object> parameters, CommandType commandType)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = commandType;
            cmd.Parameters.Clear();
            if (parameters != null)
            {
                foreach (var key in parameters)
                {
                    if (key.Value.GetType() == typeof(System.Enum))
                    {
                        cmd.Parameters.Add(new SqlParameter
                        {
                            SqlDbType = SqlDbType.Int,
                            ParameterName = key.Key,
                            Value = key.Value
                        });
                    }
                    else if (key.Value.GetType() == typeof(Byte[]))
                    {
                        cmd.Parameters.Add(new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarBinary,
                            ParameterName = key.Key,
                            Value = key.Value
                        });
                    }
                    else if (key.Value.GetType() == typeof(Boolean))
                    {
                        cmd.Parameters.Add(new SqlParameter
                        {
                            SqlDbType = SqlDbType.SmallInt,
                            ParameterName = key.Key,
                            Value = key.Value.Equals(true) ? 1 : 0
                        });
                    }
                    else if (key.Value.GetType() == typeof(DateTime))
                    {
                        cmd.Parameters.Add(new SqlParameter
                        {
                            SqlDbType = SqlDbType.DateTime,
                            ParameterName = key.Key,
                            Value = key.Value
                        });
                    }
                    else
                        cmd.Parameters.AddWithValue(key.Key, key.Value);
                }
            }
            return cmd;
        }

        public static object ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            object result = null;
            Using(conn =>
            {
                using (var cmd = CreateCommand(conn, sql, parameters, CommandType.Text))
                    result = cmd.ExecuteScalar();
            });
            return result;
        }

        public static List<T> GetList<T>(string sql, Dictionary<string, object> parameters, Func<SqlDataReader, T> builder)
        {
            var result = new List<T>();
            Using(conn =>
            {
                using (var cmd = CreateCommand(conn, sql, parameters, CommandType.Text))
                using (var dr = cmd.ExecuteReader())
                    try
                    {
                        while (dr.Read())
                        {
                            result.Add(builder(dr));
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
            });
            return result;
        }

        public static T GetFirstOrDefault<T>(string sql, Dictionary<string, object> parameters, Func<SqlDataReader, T> builder)
        {
            return GetList(sql, parameters, builder).FirstOrDefault();
        }
    }
}