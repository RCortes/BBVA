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

            try
            {
                SqlConnection conn = new SqlConnection(connectionString: conex);
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

                    //conn = new SqlConnection(connectionString: conex);
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

                    if (type != "")
                    {

                        string sql = @"SELECT * FROM dbo.Notification WHERE idPlataform = 1 AND idNotificationType = @type";
                        command = new SqlCommand(sql, conn);

                        daAdaptador = new SqlDataAdapter(command);
                        command.Parameters.AddWithValue("@type", type);
                        dtDatos = new DataSet();
                        daAdaptador.Fill(dtDatos);

                        push.RegisterGcmService(new GcmPushChannelSettings(ConfigurationManager.AppSettings["configGCM"]));

                        int i = 0;
                        List<string> IDs = new List<string>();
                        foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                        {



                            push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(_dr[1].ToString())
                                .WithJson("{\"alert\":\"" + _dr[9].ToString() + " " + _dr[10].ToString() + " " + _dr[11].ToString() + "\",\"badge\":\"1\",\"sound\":\"default.caf\"}"));

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
                                    Emails.Email.Enviarcorreo("Error envio de push Android BBVA", "info.rodolfoc@gmail.com", ex.Message.ToString(), "Error");
                                    Console.Write(ex2.Message);
                                }

                            }
                           // push.StopAllServices();
                            //i++;
                            //if (i % 500 == 0)
                            //{
                               push.StopAllServices();
                            //}
                            actual = DateTime.Parse("01-01-0001 " + DateTime.Now.ToShortTimeString());
                            if (DateTime.Compare(actual, duration) > 0)
                            {
                                break;
                            }
                        }
                        push.StopAllServices();
                        System.Threading.Thread.Sleep(5000);
                    }
                }
                    conn.Close();
                

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Emails.Email.Enviarcorreo("Error envio de push Android BBVA", "info.rodolfoc@gmail.com", ex.Message.ToString(), "Error");
                push.StopAllServices();
            }




        }

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
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
