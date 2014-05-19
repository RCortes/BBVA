using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace ReadNotifications
{
    class Program
    {
        //public static string conex = "Data Source=RODOLFOCORT6393;Initial Catalog=Notificaciones;Persist Security Info=True;User ID=sa;Password=q1w2e3";
        public static string conex = "Data Source=WINDOWS7-32BITS;Initial Catalog=PushNotification;Persist Security Info=True;User ID=sa;Password=12345";

        static void Main(string[] args)
        {
          

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
                    try
                    {
                        //i = 0;
                        List<string> reads = new List<string>();
                        //string[] reads = new string[i];
                        //StreamReader read;
                        StreamWriter write;

                        //  read = new StreamReader("Z://setting.txt");
                        //
                        //   while (!read.EndOfStream)
                        //   {
                        //       reads.Add(Convert.ToString(read.ReadLine()));
                        //       // i++;
                        //   }
                        //   read.Close();
                        //

                        //if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Archivos"))
                        //    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Archivos");

                        //if (!File.Exists(Directory.GetCurrentDirectory() + "\\Archivos\\" + "Setting.txt"))
                        //{
                        //    FileStream file = File.Create(Directory.GetCurrentDirectory() + "\\Archivos\\" + "Setting.txt");
                        //}


                      //  write = new StreamWriter("Z://setting.txt");

                        write = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.txt"));



                        for (i = 0; i <= User.Count - 1; i++)
                        {
                            write.WriteLine(User[i]);
                        }
                        write.Close();


                        //FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create("ftp://190.215.44.18:2373/afaundez/Publicaciones/img/setting.txt");
                        //request.Method = WebRequestMethods.Ftp.UploadFile;
                        //request.Credentials = new NetworkCredential("Administrator", "$bTv2o1o");
                        //request.UsePassive = true;
                        //// request.UseBinary = true;
                        //request.KeepAlive = true;

                        //FileStream stream = File.OpenRead("Z://setting.txt");

                        //byte[] buffer = new byte[stream.Length];
                        //stream.Read(buffer, 0, buffer.Length);
                        //stream.Close();

                        //Stream reqStream = request.GetRequestStream();
                        //reqStream.Write(buffer, 0, buffer.Length);
                        //reqStream.Flush();
                        //reqStream.Close();

                        //status.status = "Success";
                        Console.Write("Success");
                        //Console.ReadLine();

                    }
                    catch (Exception ex)
                    {
                        Console.Write("Error");
                        Console.ReadLine();
                    }

                }


            }
            catch (Exception ex)
            {
                Console.Write("Error");
                Console.ReadLine();
            }

                
        }
    }
}
