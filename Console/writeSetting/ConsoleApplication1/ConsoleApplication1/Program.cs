using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;


namespace ConsoleApplication1
{
    class Program
    {
        public static string conex = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

        static void Main(string[] args)
        {
            SqlConnection conn = new SqlConnection(connectionString: conex);

            string sqlSelect = @"SELECT idUsers FROM dbo.Users";
            SqlCommand command = new SqlCommand(sqlSelect, conn);
            conn.Open();
            SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
            DataSet dtDatos = new DataSet();
            daAdaptador.Fill(dtDatos);
            conn.Close();

            foreach (DataRow _dr in dtDatos.Tables[0].Rows)
            {


                string sqlSelectNotificationTypes = @"SELECT * FROM dbo.NotificationType";

                SqlCommand command2 = new SqlCommand(sqlSelectNotificationTypes, conn);
                conn.Open();
                SqlDataAdapter daAdaptador2 = new SqlDataAdapter(command2);
                DataSet dtDatos2 = new DataSet();
                daAdaptador2.Fill(dtDatos2);
                conn.Close();
               
                foreach (DataRow _dr2 in dtDatos2.Tables[0].Rows)
                {
                    Console.Write(" \n" + _dr[0].ToString() + " " + _dr2[0].ToString() + " ");
                    try
                    {
                        string sqlInsertNotificationType_Users = @"INSERT INTO dbo.NotificationType_Users (idNotificationType, idUsers, status) VALUES (@idNotificationType, @idUsers, @status)";

                        command = new SqlCommand(sqlInsertNotificationType_Users, conn);
                        command.Parameters.AddWithValue("@idUsers", _dr[0].ToString());
                        command.Parameters.AddWithValue("@idNotificationType", _dr2[0].ToString());
                        command.Parameters.AddWithValue("@status", 1);
                        conn.Open();
                        command.ExecuteScalar();
                        conn.Close();
                        Console.Write(" Success ");
                    
                    }catch(Exception ex)
                    {
                        conn.Close();
                        Console.Write(" Error ");
                        continue;
                    }

                }

            }


        }
    }
}
