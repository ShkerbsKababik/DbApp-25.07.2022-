using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data.SQLite;
using System.Data;

namespace DBServer
{
    public static class SQLHandler
    {
        public static Packet Operate(Client client, Packet packet)
        {
            try
            {
                Console.WriteLine("-> SQL      : start");
                switch (packet.type)
                {
                    case "autorization":
                        packet = Autorization(packet);
                        return packet;
                    case "open":
                        packet = Autorization(packet);
                        if (packet.pass == "true")
                        {
                            packet.columns_name = Columns(packet);
                            packet = OpenTable(packet);
                        }
                        return packet;
                    case "save":
                        packet = Autorization(packet);
                        if (packet.pass == "true")
                        {
                            //Console.WriteLine(AddColumn("users", "rights"));
                            packet = SaveTable(packet);
                        }
                        return packet;
                }
            }
            catch
            {
                Console.WriteLine("-> SQL      : error");
            }
            finally
            {
                Console.WriteLine("-> SQL      : end");
            }
            return packet;
        }

        // SQL macro operations
        public static Packet Autorization(Packet packet)
        {
            if (File.Exists(@"dbf\DataBase.db"))
            {
                try
                {
                    SQLiteConnection connection = new SQLiteConnection("Data Source=" + @"dbf\DataBase.db");
                    SQLiteCommand command = new SQLiteCommand($"SELECT * FROM users WHERE name='{packet.name}' AND passsword='{packet.password}'", connection);
                    List<string[]> answer = new List<string[]>();

                    connection.Open();
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        answer.Add(new string[reader.FieldCount]);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            answer[answer.Count - 1][i] = reader[i].ToString();
                        }

                    }
                    reader.Close();
                    connection.Close();

                    if (answer.Count > 0)
                    {
                        packet.pass = "true";
                    }
                    else
                    {
                        packet.pass = "false";
                    }
                    Console.WriteLine("-> pass     : " + packet.pass);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e); return null;
                }
            }
            return packet;
        }
        public static Packet OpenTable(Packet packet)
        {
            Console.WriteLine($"-> table    : {packet.table_name}");
            try
            {
                SQLiteConnection connection = new SQLiteConnection("Data Source=" + @"dbf\DataBase.db");
                SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {packet.table_name}", connection);
                List<string[]> answer = new List<string[]>();

                connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    answer.Add(new string[reader.FieldCount]);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        answer[answer.Count - 1][i] = reader[i].ToString();
                    }
                }
                reader.Close();
                connection.Close();

                packet.table  = answer.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e); return packet;
            }
            return packet;
        }
        public static Packet SaveTable(Packet packet)
        {
            Console.WriteLine($"-> table    : {packet.table_name}");
            try
            {
                string[] oldColumns = Columns(packet);
                // If packet has new columns
                if (oldColumns != packet.columns_name)
                {

                }
                // If packet doesnt have new columns
                else
                { 
                    
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e); return packet;
            }
            return packet;
        }

        // SQl micro operations
        public static string[] Columns(Packet packet)
        {
            List<string> answer = new List<string>();
            using (var con = new SQLiteConnection("Data Source=" + @"dbf\DataBase.db"))
            {
                using (var cmd = new SQLiteCommand("PRAGMA table_info(" + packet.table_name + ");"))
                {
                    var table = new DataTable();

                    cmd.Connection = con;
                    cmd.Connection.Open();

                    SQLiteDataAdapter adp = null;
                    try
                    {
                        adp = new SQLiteDataAdapter(cmd);
                        adp.Fill(table);
                        con.Close();
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.ToString()); }
                    foreach (DataRow row in table.Rows)
                    {
                        Console.WriteLine(row[1].ToString());
                        answer.Add(row[1].ToString());
                    };
                }
            }
            return answer.ToArray();
        }
        public static string AddColumn(string tableName, string columnName)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection("Data Source=" + @"dbf\DataBase.db");
                SQLiteCommand command = new SQLiteCommand($"ALTER TABLE {tableName} ADD rights VARCHAR; ", connection);
                List<string[]> answer = new List<string[]>();

                connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    answer.Add(new string[reader.FieldCount]);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        answer[answer.Count - 1][i] = reader[i].ToString();
                    }
                }
                reader.Close();
                connection.Close();

                return "dobavleno";
            }
            catch
            {
                return "error | ne poluchiloc";
            }
        }
    }
}
