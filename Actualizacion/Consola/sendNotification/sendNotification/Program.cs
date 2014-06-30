using System;
using System.Collections.Generic;
using PushSharp;
using PushSharp.Android;
using PushSharp.Core;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace sendNotification
{

    class Program
    {
        static int enviado = 0;
        static int error = 0;
        public static string conex = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

        static void Main(string[] args)
        {
            Process[] localAll = Process.GetProcesses();
            int p = 1;
            foreach (Process pr in localAll)
            {
                if (pr.ProcessName == "sendNotificationAndroid")
                {
                    if (p > 1)
                    {
                        Console.Write("\n\n\n \"sendNotificationAndroid.exe\" ya esta en ejecución... será cerrada");
                        System.Threading.Thread.Sleep(3000);
                        Environment.Exit(0);
                    }
                    p++;
                }
            }

            Console.Write("Envio de Notificaciones Android \n\n Procesando...");

            var push = new PushBroker();

            push.OnNotificationSent += NotificationSent;
            push.OnChannelException += ChannelException;
            push.OnServiceException += ServiceException;
            push.OnNotificationFailed += NotificationFailed;
            push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            push.OnChannelCreated += ChannelCreated;
            push.OnChannelDestroyed += ChannelDestroyed;

            SqlConnection conn = new SqlConnection(connectionString: conex);

            try
            {
                conn.Open();


                       
                      
                while (true)
                {
                    string type = "";

                    DateTime actual = DateTime.Parse("00:00:00");
                    DateTime start = Convert.ToDateTime("00:00:00");
                    DateTime duration = Convert.ToDateTime("00:00:00");

                    string sqlSelectSchedule = @"UPDATE dbo.NotificationType SET start = DATEADD (year, 2001 - YEAR(start), start)
                                                 UPDATE dbo.NotificationType SET start = DATEADD (month, 01 - MONTH(start), start)
                                                 UPDATE dbo.NotificationType SET start = DATEADD (day, 01 - DAY(start), start)
                                                 SELECT idNotificationType, start, duration FROM dbo.NotificationType ORDER BY start ASC";

                    SqlCommand command = new SqlCommand(sqlSelectSchedule, conn);

                    SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                    DataSet dtDatos = new DataSet();
                    daAdaptador.Fill(dtDatos);

                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                    {
                        actual = DateTime.Parse("01-01-0001 " + DateTime.Now.ToShortTimeString());
                        start = DateTime.Parse("01-01-0001 " + DateTime.Parse(_dr[1].ToString()).ToShortTimeString());
                        duration = DateTime.Parse("01-01-0001 " + DateTime.Parse(_dr[1].ToString()).ToShortTimeString()).AddMinutes(15);

                        if (DateTime.Compare(actual, start) >= 0)
                        {
                            type = _dr[0].ToString();
                        }
                    }


                    string sqlCountUser = @"SELECT COUNT(*) FROM dbo.Notification WHERE idNotificationType = @idtype";

                    command = new SqlCommand(sqlCountUser, conn);
                    command.Parameters.AddWithValue("@idtype", ConfigurationManager.AppSettings["type"]);

                    int countU = 0;
                    countU = Convert.ToInt32(command.ExecuteScalar());

                    if (countU > 0)
                    {
                        type = ConfigurationManager.AppSettings["type"];
                    }


                    if (type != "")
                    {

                   

//                    if (type == "8")
//                    {

//                        string selectHoldOver = @"SELECT * FROM HoldOver WHERE expiration >= @expiration";

//                        command = new SqlCommand(selectHoldOver, conn);
//                        command.Parameters.AddWithValue("@expiration", DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:00"));

//                        daAdaptador = new SqlDataAdapter(command);
//                        dtDatos = new DataSet();
//                        daAdaptador.Fill(dtDatos);

//                        foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                        {
//                            string delivery = DateTime.Parse(_dr[2].ToString()).ToShortDateString() + " 00:00:00";
//                            string ahora = DateTime.Now.ToShortDateString() + " 00:00:00";

//                            if (DateTime.Compare(DateTime.Parse(delivery), DateTime.Parse(ahora)) < 0)
//                            {
//                                try
//                                {
//                                    string insertNotification = @"INSERT INTO [Notification]
//                                                                             ([idDevice]
//                                                                             ,[idNotificationType]
//                                                                             ,[idPlataform]
//                                                                             ,[creation]
//                                                                             ,[idUsers]
//                                                                             ,[shortText]
//                                                                             ,[longText]
//                                                                             ,[deliveryPossibilities]
//                                                                             ,[expiration])
//                                                                       VALUES
//                                                                             (@idDevice
//                                                                             ,@idNotificationType
//                                                                             ,@idPlataform
//                                                                             ,@creation
//                                                                             ,@idUsers
//                                                                             ,@shortText
//                                                                             ,@longText
//                                                                             ,@deliveryPossibilities
//                                                                             ,@expiration)";

//                                    command = new SqlCommand(insertNotification, conn);
//                                    command.Parameters.AddWithValue("@idDevice", _dr[5].ToString());
//                                    command.Parameters.AddWithValue("@idNotificationType", _dr[3].ToString());
//                                    command.Parameters.AddWithValue("@idPlataform", _dr[6].ToString());
//                                    command.Parameters.AddWithValue("@creation", DateTime.Parse(_dr[1].ToString()));
//                                    command.Parameters.AddWithValue("@idUsers", _dr[4].ToString());
//                                    command.Parameters.AddWithValue("@shortText", _dr[7].ToString());
//                                    command.Parameters.AddWithValue("@longText", _dr[8].ToString());
//                                    command.Parameters.AddWithValue("@deliveryPossibilities", _dr[10].ToString());
//                                    command.Parameters.AddWithValue("@expiration", DateTime.Parse(_dr[11].ToString()));
//                                    command.ExecuteScalar();

//                                    string deleteHoldOver = @"DELETE FROM [Notificaciones].[dbo].[HoldOver]
//                                                                             WHERE idHoldOver = @idHoldOver";

//                                    command = new SqlCommand(deleteHoldOver, conn);
//                                    command.Parameters.AddWithValue("@idHoldOver", _dr[0].ToString());
//                                    command.ExecuteScalar();

//                                }
//                                catch (Exception ex)
//                                {
//                                    //conn.Close();
//                                    Console.Write(ex.Message);
//                                    string body = "\n Error: " + ex.Message + " \n hora: " + DateTime.Now.ToString();
//                                    Emails.Email.Enviarcorreo("Error envio de push iOS BBVA", "info.rodolfoc@gmail.com", body, "Error");

//                                }
//                            }
//                        }

//                        try
//                        {
//                            string delete = @"DELETE FROM HoldOver WHERE expiration < @expiration";

//                            command = new SqlCommand(delete, conn);
//                            command.Parameters.AddWithValue("@expiration", DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00"));
//                            command.ExecuteScalar();
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.Write(ex.Message);
//                            string body = "\n Error: " + ex.Message + " \n hora: " + DateTime.Now.ToString();
//                            Emails.Email.Enviarcorreo("Error envio de push iOS BBVA", "info.rodolfoc@gmail.com", body, "Error");

//                        }
//                    }

                    
                  

                        string sql = @"SELECT * FROM dbo.Notification WHERE idPlataform = 1 AND idNotificationType = @type ORDER BY creation";
                        command = new SqlCommand(sql, conn);

                        daAdaptador = new SqlDataAdapter(command);
                        command.Parameters.AddWithValue("@type", type);
                        dtDatos = new DataSet();
                        daAdaptador.Fill(dtDatos);

                        push.StopAllServices(true);
                        
                        push.RegisterGcmService(new GcmPushChannelSettings(ConfigurationManager.AppSettings["configGCM"]));
                            
                        int i = 0;
                        List<string> IDs = new List<string>();
                        foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                        {
                              //string expiration = DateTime.Parse(_dr[14].ToString()).ToShortDateString() + " 00:00:00";
                              //  string ahora = DateTime.Now.ToShortDateString() + " 00:00:00";

                              //  if (DateTime.Compare(DateTime.Parse(expiration), DateTime.Parse(ahora)) <= 0)
                              //  {
                                    push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(_dr[1].ToString())
                                        .WithJson("{\"alert\":\"" + _dr[11].ToString() + "\",\"badge\":\"1\",\"sound\":\"default.caf\"}"));
                                //}
                           
                            try
                            {
                                string deleteNotification = @"DELETE FROM dbo.Notification WHERE idNotificacion = @idNotification";
                                command = new SqlCommand(deleteNotification, conn);
                                command.Parameters.AddWithValue("@idNotification", _dr[0].ToString());
                                command.ExecuteScalar();
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    string updateHistorical = @"UPDATE dbo.Notification SET idUsers = 0, idDevice = 0 WHERE idNotificacion = @idNotification";
                                    command = new SqlCommand(updateHistorical, conn);
                                    command.Parameters.AddWithValue("@idNotification", _dr[0].ToString());
                                    command.ExecuteScalar();

                                    string deleteNotification = @"DELETE FROM dbo.Notification WHERE idNotificacion = @idNotification";
                                    command = new SqlCommand(deleteNotification, conn);
                                    command.Parameters.AddWithValue("@idNotification", _dr[0].ToString());
                                    command.ExecuteScalar();

                                }
                                catch (Exception ex2)
                                {
                                    //Emails.Email.Enviarcorreo("Error envio de push Android BBVA", "info.rodolfoc@gmail.com", ex.Message.ToString(), "Error");
                                    Console.Write(ex2.Message);
                                    System.Threading.Thread.Sleep(3000);
              
                                }

                            }
                            actual = DateTime.Parse("01-01-0001 " + DateTime.Now.ToShortTimeString());
                            if (DateTime.Compare(actual, duration) > 0)
                            {
                                break;
                                
                            }
                        }
                        push.StopAllServices(true);
                    }
                }
                
                    conn.Close();
                

            }
            catch (Exception ex)
            {
                conn.Close(); 
                Console.Write(ex.Message);
                string body = "\n Error: " + ex.Message +" \n hora: " + DateTime.Now.ToString();
                Emails.Email.Enviarcorreo("Error envio de push Android BBVA", "info.rodolfoc@gmail.com", body, "Error");
                System.Threading.Thread.Sleep(7000);
                push.StopAllServices(true);
                Environment.Exit(0);
            }




        }

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);

            SqlConnection conn2 = new SqlConnection(connectionString: conex);
            try
            {
                string select = @"SELECT * FROM Device WHERE idDevice = @idDevice";

                SqlCommand command2 = new SqlCommand(select, conn2);

                command2.Parameters.AddWithValue("@idDevice", oldSubscriptionId);

                conn2.Open();
                SqlDataAdapter daAdaptador2 = new SqlDataAdapter(command2);
                DataSet dtDatos2 = new DataSet();
                daAdaptador2.Fill(dtDatos2);
                conn2.Close();

                foreach (DataRow _dr in dtDatos2.Tables[0].Rows)
                {
                    try
                    {
                        string insert = @"INSERT INTO Device (idDevice, token, appVersion, creation, status, idPlataform)
                                                           VALUES (@idDevice,'','',@creation,@status,@idPlataform)";

                        command2 = new SqlCommand(insert, conn2);

                        command2.Parameters.AddWithValue("@idDevice", newSubscriptionId);
                        command2.Parameters.AddWithValue("@idPlataform", _dr[5].ToString());
                        command2.Parameters.AddWithValue("@status", _dr[4].ToString());
                        command2.Parameters.AddWithValue("@creation", DateTime.Parse(_dr[3].ToString()));

                        conn2.Open();
                        command2.ExecuteScalar();
                        conn2.Close();

                        string update = @"UPDATE Historical SET idDevice = @idNewDevice WHERE idDevice = @idOldDevice;
                                          UPDATE Users_Device SET idDevice = @idNewDevice WHERE idDevice = @idOldDevice;
                                          UPDATE Notification SET idDevice = @idNewDevice WHERE idDevice = @idOldDevice;
                                          UPDATE HoldOver SET idDevice = @idNewDevice WHERE idDevice = @idOldDevice
                                          DELETE FROM Device WHERE idDevice = @idOldDevice";

                        command2 = new SqlCommand(update, conn2);

                        command2.Parameters.AddWithValue("@idNewDevice", newSubscriptionId);
                        command2.Parameters.AddWithValue("@idOldDevice", oldSubscriptionId);
                        conn2.Open();
                        command2.ExecuteScalar();
                        conn2.Close();
                    }
                    catch (Exception ex)
                    {
                        conn2.Close();
                        Console.Write(ex.Message);
                        string body = "\n Error: " + ex.Message + " \n hora: " + DateTime.Now.ToString();
                        Emails.Email.Enviarcorreo("Error envio de push iOS BBVA", "info.rodolfoc@gmail.com", body, "Error");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                conn2.Close();
                Console.Write(ex.Message);
                string body = "\n Error: " + ex.Message + " \n hora: " + DateTime.Now.ToString();
                Emails.Email.Enviarcorreo("Error envio de push iOS BBVA", "info.rodolfoc@gmail.com", body, "Error");

            }

        }

        static void NotificationSent(object sender, INotification notification)
        {
            Console.WriteLine("Sent: " + sender + " -> " + notification);
            enviado++;
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
            error++;
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine("\nChannel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("\nChannel Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("\n\nChannel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine("\n\nChannel Created for: " + sender);
        }
    }
}
