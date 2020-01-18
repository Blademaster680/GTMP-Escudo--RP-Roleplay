using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Web
{
    public class MySQL
    {
        public static string ConnectionString =
            "SERVER=197.242.159.41; PORT=3307; DATABASE=egrp; UID=egrp; PASSWORD=D3c3mb3r;";

        public static long Query(string Query)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    var cmd = conn.CreateCommand();
                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();

                    conn.Close();

                    return cmd.LastInsertedId;
                }
                catch (MySqlException e)
                {
                    Console.Write("[MySQL][Error]" + e);
                    return -1;
                }
            }
        }

        public static DataTable QueryResult(string Query)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = Query;
                    var reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        conn.Close();
                        return null;
                    }
                    var reslut = new DataTable();
                    reslut.Load(reader);
                    reader.Close();

                    conn.Close();

                    return reslut;
                }
                catch (MySqlException e)
                {
                    Console.Write("[MySQL][Error] " + e);
                    return null;
                }
            }
        }
    }
}