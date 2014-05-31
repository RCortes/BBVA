using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;

namespace writeSetting
{
    class Program
    {
        public static string conex = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

        static void Main(string[] args)
        {
            Console.Write("\n------------------------------------------------");
            Console.Write("\n  User configuration Updater (please do not close)");
            Console.Write("\n------------------------------------------------");
            Console.Write("\n\n  - Reading user data...");
            
            string Users = "";
            int i = 0;

            List<string> User = new List<string>();

            string users = @"Select idUsers FROM dbo.Users WHERE status = 1";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString: conex))
                {

                    SqlCommand command = new SqlCommand(users, conn);
                    conn.Open();
                    SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                    DataSet dtDatos = new DataSet();
                    daAdaptador.Fill(dtDatos);


                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                    {
                        
                        string user = _dr[0].ToString();

                        Users = user + ";";
                        try
                        {
                            string Types = @"SELECT * FROM dbo.NotificationType_Users NTU, dbo.Users U WHERE NTU.idUsers = U.idUsers AND U.idUsers = @rut";

                            using (SqlConnection conn2 = new SqlConnection(connectionString: conex))
                            {
                                SqlCommand command2 = new SqlCommand(Types, conn2);
                                command2.Parameters.AddWithValue("@rut", user);
                                conn2.Open();
                                SqlDataAdapter daAdaptador2 = new SqlDataAdapter(command2);
                                DataSet dtDatos2 = new DataSet();
                                daAdaptador2.Fill(dtDatos2);
                                foreach (DataRow _dr2 in dtDatos2.Tables[0].Rows)
                                {
                                    if (_dr2[2].ToString() == "True")
                                    {
                                        Users += 1 + ";";
                                    }
                                    else
                                    {
                                        Users += 0 + ";";
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            
                        }

                        User.Add(Users);

                    }
                    Console.Write(" OK");
                    try
                    {
                        Console.Write("\n  - Writing file...");

                        List<string> reads = new List<string>();

                        StreamWriter write;
                        
                        write = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"]));



                        for (i = 0; i <= User.Count - 1; i++)
                        {
                            write.WriteLine(User[i]);
                        }
                        write.Close();
                        Console.Write(" OK");

                        Console.Write("\n  - Loading in FTP...");
                        FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ConfigurationManager.AppSettings["ftpRoute"] + ConfigurationManager.AppSettings["ftpFile"]);
                        request.Method = WebRequestMethods.Ftp.UploadFile;
                        request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ftpUser"], ConfigurationManager.AppSettings["ftpPass"]);
                        request.UsePassive = true;
                        // request.UseBinary = true;
                        request.KeepAlive = true;

                        FileStream stream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"]));

                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        stream.Close();

                        Stream reqStream = request.GetRequestStream();
                        reqStream.Write(buffer, 0, buffer.Length);
                        reqStream.Flush();
                        reqStream.Close();
                        Console.Write(" OK");
                        Console.Write("\n\n------------------------------------------------\n\n\n");
                        Console.Write("  Output File: " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"]));
                        Console.Write("\n\n\n Status: Success");
                        System.Threading.Thread.Sleep(7000);

                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n\n Error " + ex.Message + "\n\n Press any key to close...");
                        Console.ReadLine();
                    }

                }


            }
            catch (Exception ex)
            {
                Console.Write("\n\n Error " + ex.Message + "\n\n Press any key to close...");
                Console.ReadLine();
            }

                
        
        }
    }
}
