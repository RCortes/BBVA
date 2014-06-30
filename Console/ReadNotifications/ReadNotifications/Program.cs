using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using System.Diagnostics;

namespace ReadNotifications
{
    class Program
    {
        public static string conex = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

        static void Main(string[] args)
        {
            Process[] localAll = Process.GetProcesses();
            int p = 1;
            foreach (Process pr in localAll)
            {
                if (pr.ProcessName == "ReadNotifications")
                {
                    if (p > 1) 
                    {
                        Console.Write("\n\n\n \"ReadNotifications.exe\" ya esta en ejecución... será cerrada");
                        System.Threading.Thread.Sleep(3000);
                        Environment.Exit(0);
                    }
                    p++;              
                }
            }


            List<string> reads = new List<string>();

            Console.Write("\n------------------------------------------------");
            Console.Write("\n  Load Notifications (please do not close)");
            Console.Write("\n------------------------------------------------");
            Console.Write("\n\n  - Downloading from FTP...");

            try
            {
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ftpUser"], ConfigurationManager.AppSettings["ftpPass"]);
                byte[] fileData = request.DownloadData(ConfigurationManager.AppSettings["ftpRoute"] + ConfigurationManager.AppSettings["ftpFile"]);

                if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"])))
                {
                    FileStream file = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"]));
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
                else
                {
                    File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"]));
                    FileStream file = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"]));
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }

                Console.Write(" OK");
                Console.Write("\n  - Notifications Loading...");

                StreamReader read;
                read = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"]));

                while (!read.EndOfStream)
                {
                    reads.Add(Convert.ToString(read.ReadLine()));
                }

                read.Close();

                Console.Write(" OK");
                Console.Write("\n  - Submitting Notifications...");
                for (int i = 0; i <= reads.Count - 1; i++)
                {

                    string[] notification = reads[i].Split(';');

                    string user = removeRut(notification[0]);
                    string type = notification[1];
                    string shortText = notification[2];
                    string longText = notification[3];

                   
                    string selectDevice = @"SELECT D.idDevice, D.idPlataform 
                           FROM dbo.Device D, dbo.Users_Device UD, dbo.NotificationType_Users NTU
                           WHERE D.status = 1  AND D.idDevice = UD.idDevice AND NTU.status = 1 AND NTU.idNotificationType = @idNotificationType AND NTU.idUsers = UD.idUsers AND UD.idUsers = @idUsers ";

                
                    SqlConnection conn = new SqlConnection(connectionString: conex);
                    SqlCommand command = new SqlCommand(selectDevice, conn);
                  
                    command.Parameters.AddWithValue("@idUsers", user);
                    command.Parameters.AddWithValue("@idNotificationType", type);
                    conn.Open();
                  
                    SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                    DataSet dtDatos = new DataSet();
                    daAdaptador.Fill(dtDatos);
                  
                    conn.Close();                  
                  
                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                    {
                        
                             string insertNotification = @"INSERT INTO dbo.Notification (idDevice, token, idNotificationType, title, text, idPlataform, creation, idUsers, firstName, lastName, shortText, longText)
                                                               VALUES (@idDevice, '', @idNotificationType, '', '', @idPlataform, GETDATE(), @idUsers, '', '', @shortText, @longText)";                             
                                                     
                             SqlConnection conn2 = new SqlConnection(connectionString: conex);
                             SqlCommand command2 = new SqlCommand(insertNotification, conn2);
                             command2.Parameters.AddWithValue("@idNotificationType", type);
                             command2.Parameters.AddWithValue("@idUsers", user);
                             command2.Parameters.AddWithValue("@idDevice", _dr[0].ToString());
                             command2.Parameters.AddWithValue("@idPlataform", _dr[1].ToString());
                             command2.Parameters.AddWithValue("@shortText", shortText);
                             command2.Parameters.AddWithValue("@longText", longText);
                  
                             conn2.Open();
                             command2.ExecuteScalar();
                             conn2.Close();                  
                  
                     }             
                
                }                
                Console.Write(" OK");
                Console.Write("\n\n-----------------------------------------------");
                Console.Write("\n\n\n Status: Success");
                System.Threading.Thread.Sleep(7000);
            }
            catch (Exception ex)
            {
                Console.Write("\n\n Error " + ex.Message + "\n\n Press any key to close...");
                Console.ReadLine();
            }




            
                
        }

        static string removeRut(string rut)
        {
            rut = rut.Replace(".", "");
            rut = rut.Replace("-", "");
            return rut;
        }
    }
}
