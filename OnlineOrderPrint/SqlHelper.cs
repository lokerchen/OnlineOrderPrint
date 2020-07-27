using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineOrderPrint;

namespace GetServerEmail
{
    public static class SqlHelper
    {
        private static string STR_CONN = @"Data Source=eid.db;Pooling=true;FailIfMissing=false";

        public static bool QueryId(string strSql)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(STR_CONN))
                {
                    conn.Open();
                    SQLiteCommand comm = conn.CreateCommand();
                    comm.CommandText = strSql;

                    using (SQLiteDataReader reader = comm.ExecuteReader())
                    {
                        return reader.Read();
                    }
                }
            }
            catch (Exception e) {
                return false;
            }
        }

        public static bool InsertId(string strSql)
        {
            int num = 0;

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(STR_CONN))
                {
                    conn.Open();
                    SQLiteCommand comm = conn.CreateCommand();
                    comm.CommandText = strSql;

                    return comm.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool ClearData(string strSql)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(STR_CONN))
                {
                    conn.Open();
                    SQLiteCommand comm = conn.CreateCommand();
                    comm.CommandText = strSql;

                    return comm.ExecuteNonQuery() >= 0;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static User Query(string strSql)
        {
            User user = new User();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(STR_CONN))
                {
                    conn.Open();
                    SQLiteCommand comm = conn.CreateCommand();
                    comm.CommandText = strSql;

                    using (SQLiteDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.UsrName = reader["UsrName"].ToString();
                            user.UsrPwd = reader["UsrPwd"].ToString();
                            user.MinsInt = reader["MinsInt"].ToString();
                            user.MailServer = reader["MailServer"].ToString();
                            user.MailSender = reader["MailSender"].ToString();
                            user.PrtCount = reader["PrtCount"].ToString();
                            user.Version = reader["Version"].ToString();
                            user.CompanyName = reader["CompanyName"].ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return user;
        }
    }
}
