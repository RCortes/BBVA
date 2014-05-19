using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using Data_PushBBVA;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Android;
using PushSharp.Core;
using System.IO;

namespace Connection_PushBBVA
{
    public class SQLConnection
    {

        //public static string conex = "Data Source=RODOLFOCORT6393;Initial Catalog=Notificaciones;Persist Security Info=True;User ID=sa;Password=q1w2e3";
        public static string conex = "Data Source=WINDOWS7-32BITS;Initial Catalog=PushNotification;Persist Security Info=True;User ID=sa;Password=12345";

        public static Status CreateUser(string rut, string firstName, string lastName, string idDevice, string idPlataform)
        {          
            Status status = new Status();

            try 
            {
                string sqlCountUser = @"SELECT COUNT(*) FROM dbo.Users WHERE idUsers = @usuario";

                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(sqlCountUser, conn);
                command.Parameters.AddWithValue("@usuario", rut);
                
                int countU = 0;
                int countD = 0;                
                
                conn.Open();
                countU = Convert.ToInt32(command.ExecuteScalar());                    
                conn.Close();

                if (countU == 0)
                {
                        string sqlCountDevice = @"SELECT COUNT(*) FROM dbo.Device WHERE idDevice = @device";

                        command = new SqlCommand(sqlCountDevice, conn);
                        command.Parameters.AddWithValue("@device", idDevice);
                        conn.Open();
                        countD = Convert.ToInt32(command.ExecuteScalar());
                        conn.Close();
                        if (countD == 0)
                        {
                            try
                            {
                                string sqlInsert = @"INSERT INTO Device (idDevice, token, appVersion, creation, status, idPlataform)
                                                           VALUES (@idDevice,'','',GETDATE(),1,@idPlataform);
                                                           INSERT INTO Users (idUsers, firstName, lastName, creation, status)
                                                           VALUES (@rut,@firstName,@lastName,GETDATE(),1);
                                                           INSERT INTO Users_Device (idUsers, idDevice)
                                                           VALUES (@rut, @idDevice)";

                                command = new SqlCommand(sqlInsert, conn);
                                command.Parameters.AddWithValue("@rut", rut);
                                command.Parameters.AddWithValue("@idDevice", idDevice);
                                command.Parameters.AddWithValue("@idPlataform", idPlataform);
                                command.Parameters.AddWithValue("@firstName", firstName);
                                command.Parameters.AddWithValue("@lastName", lastName);
                                conn.Open();
                                command.ExecuteScalar();
                                conn.Close();



                                string sqlCountNotificationTypes = @"SELECT COUNT(*) FROM dbo.NotificationType_Users WHERE idUsers = @idDevice";

                                command = new SqlCommand(sqlCountNotificationTypes, conn);
                                command.Parameters.AddWithValue("@idDevice", rut);
                                conn.Open();
                                countD = Convert.ToInt32(command.ExecuteScalar());
                                conn.Close();
                                if (countD == 0)
                                {


                                    string sqlSelectNotificationTypes = @"SELECT * FROM dbo.NotificationType";

                                    command = new SqlCommand(sqlSelectNotificationTypes, conn);
                                    conn.Open();
                                    SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                                    DataSet dtDatos = new DataSet();
                                    daAdaptador.Fill(dtDatos);
                                    conn.Close();

                                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                                    {

                                        string sqlInsertNotificationType_Users = @"INSERT INTO dbo.NotificationType_Users (idNotificationType, idUsers, status) VALUES (@idNotificationType, @idUsers, @status)";

                                        command = new SqlCommand(sqlInsertNotificationType_Users, conn);
                                        command.Parameters.AddWithValue("@idUsers", rut);
                                        command.Parameters.AddWithValue("@idNotificationType", _dr[0].ToString());
                                        command.Parameters.AddWithValue("@status", 1);
                                        conn.Open();
                                        command.ExecuteScalar();
                                        conn.Close();

                                    }
                                }
                                status.status = "Success";
                                status.description = "User created";

                            }
                            catch (Exception ex)
                            {
                                status.status = "Error";
                                status.description = ex.Message;
                            }

                        }
                        else
                        {
                            try
                            {
                                string sqlInsert = @"UPDATE dbo.Device SET  token = @token, appVersion = @appVersion, creation = GETDATE(), status = @status, idPlataform = @idPlataform
                                                where idDevice = @idDevice; 
                                                INSERT INTO Users (idUsers, firstName, lastName, creation, status)
                                                VALUES (@rut,@firstName,@lastName,GETDATE(),1)";

                                command = new SqlCommand(sqlInsert, conn);
                                command.Parameters.AddWithValue("@rut", rut);
                                command.Parameters.AddWithValue("@idDevice", idDevice);
                                command.Parameters.AddWithValue("@firstName", firstName);
                                command.Parameters.AddWithValue("@lastName", lastName);
                                command.Parameters.AddWithValue("@token", "");
                                command.Parameters.AddWithValue("@appVersion", "");
                                command.Parameters.AddWithValue("@status", 1);
                                command.Parameters.AddWithValue("@idPlataform", idPlataform);
                                conn.Open();
                                command.ExecuteScalar();
                                conn.Close();



                                string sqlCount = @"SELECT COUNT(*) FROM dbo.Users_Device WHERE idDevice = @idDevice";

                                command = new SqlCommand(sqlCount, conn);
                                command.Parameters.AddWithValue("@idUsers", rut);
                                command.Parameters.AddWithValue("@idDevice", idDevice);
                                conn.Open();
                                countD = Convert.ToInt32(command.ExecuteScalar());
                                conn.Close();
                                if (countD != 0)
                                {


                                    string sqlInsertDevice_Users = @"UPDATE dbo.Users_Device SET idUsers = @idUsers WHERE idDevice = @idDevice";

                                    command = new SqlCommand(sqlInsertDevice_Users, conn);
                                    command.Parameters.AddWithValue("@idUsers", rut);
                                    command.Parameters.AddWithValue("@idDevice", idDevice);
                                    conn.Open();
                                    command.ExecuteScalar();
                                    conn.Close();


                                }
                                else 
                                {
                                    string sqlInsertDevice_Users = @"INSERT INTO dbo.Users_Device (idUsers, idDevice) VALUES (@idUsers, @idDevice)";

                                    command = new SqlCommand(sqlInsertDevice_Users, conn);
                                    command.Parameters.AddWithValue("@idUsers", rut);
                                    command.Parameters.AddWithValue("@idDevice", idDevice);
                                    conn.Open();
                                    command.ExecuteScalar();
                                    conn.Close();
                                
                                
                                
                                }

                             

                                string sqlCountNotificationTypes = @"SELECT COUNT(*) FROM dbo.NotificationType_Users WHERE idUsers = @idDevice";

                                command = new SqlCommand(sqlCountNotificationTypes, conn);
                                command.Parameters.AddWithValue("@idDevice", rut);
                                conn.Open();
                                countD = Convert.ToInt32(command.ExecuteScalar());
                                conn.Close();
                                if (countD == 0)
                                {

                                       string sqlSelectNotificationTypes = @"SELECT * FROM dbo.NotificationType";

                                command = new SqlCommand(sqlSelectNotificationTypes, conn);
                                conn.Open();
                                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                                DataSet dtDatos = new DataSet();
                                daAdaptador.Fill(dtDatos);
                                conn.Close();

                                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                                    {

                                        string sqlInsertNotificationType_Users = @"INSERT INTO dbo.NotificationType_Users (idNotificationType, idUsers, status) VALUES (@idNotificationType, @idUsers, @status)";

                                        command = new SqlCommand(sqlInsertNotificationType_Users, conn);
                                        command.Parameters.AddWithValue("@idUsers", rut);
                                        command.Parameters.AddWithValue("@idNotificationType", _dr[0].ToString());
                                        command.Parameters.AddWithValue("@status", 1);
                                        conn.Open();
                                        command.ExecuteScalar();
                                        conn.Close();

                                    }
                                }

                                status.status = "Success";
                                status.description = "User Created (updated device)";
                            }
                            catch (Exception ex) 
                            {
                                status.status = "Error";
                                status.description = ex.Message;
                            }
                        }
                    
                }
                else 
                {
                     string sqlCountDevice = @"SELECT COUNT(*) FROM dbo.Device WHERE idDevice = @device";

                        command = new SqlCommand(sqlCountDevice, conn);
                        command.Parameters.AddWithValue("@device", idDevice);
                        conn.Open();
                        countD = Convert.ToInt32(command.ExecuteScalar());
                        conn.Close();
                        if (countD != 0)
                        {
                            try 
                            {

                                string sqlInsert = @"UPDATE dbo.Users SET  firstName = @firstName, lastName = @lastName, creation = GETDATE(), status = @status
                                                WHERE idUsers = @idUsers;
                                                UPDATE dbo.Device SET  token = @token, appVersion = @appVersion, creation = GETDATE(), status = @status, idPlataform = @idPlataform
                                                WHERE idDevice = @idDevice";

                                command = new SqlCommand(sqlInsert, conn);
                                command.Parameters.AddWithValue("@idUsers", rut);
                                command.Parameters.AddWithValue("@idDevice", idDevice);
                                command.Parameters.AddWithValue("@firstName", firstName);
                                command.Parameters.AddWithValue("@lastName", lastName);

                                command.Parameters.AddWithValue("@token", "");
                                command.Parameters.AddWithValue("@appVersion", "");
                                command.Parameters.AddWithValue("@status", 1);
                                command.Parameters.AddWithValue("@idPlataform", idPlataform);
                                conn.Open();
                                command.ExecuteScalar();
                                conn.Close();




                                string sqlCount = @"SELECT COUNT(*) FROM dbo.Users_Device WHERE idUsers = @idUsers AND idDevice = @idDevice";

                                command = new SqlCommand(sqlCount, conn);
                                command.Parameters.AddWithValue("@idUsers", rut);
                                command.Parameters.AddWithValue("@idDevice", idDevice);
                                conn.Open();
                                countD = Convert.ToInt32(command.ExecuteScalar());
                                conn.Close();
                                if (countD == 0)
                                {


                                    string sqlInsertDevice_Users = @"INSERT INTO dbo.Users_Device (idUsers, idDevice) VALUES (@idUsers, @idDevice)";

                                    command = new SqlCommand(sqlInsertDevice_Users, conn);
                                    command.Parameters.AddWithValue("@idUsers", rut);
                                    command.Parameters.AddWithValue("@idDevice", idDevice);                                
                                    conn.Open();
                                    command.ExecuteScalar();
                                    conn.Close();
                                
                                
                                }

                                string sqlCountNotificationTypes = @"SELECT COUNT(*) FROM dbo.NotificationType_Users WHERE idUsers = @idUsers";

                                command = new SqlCommand(sqlCountNotificationTypes, conn);
                                command.Parameters.AddWithValue("@idUsers", rut);
                                conn.Open();
                                countD = Convert.ToInt32(command.ExecuteScalar());
                                conn.Close();
                                if (countD == 0)
                                {


                                    string sqlSelectNotificationTypes = @"SELECT * FROM dbo.NotificationType";

                                    command = new SqlCommand(sqlSelectNotificationTypes, conn);
                                    conn.Open();
                                    SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                                    DataSet dtDatos = new DataSet();
                                    daAdaptador.Fill(dtDatos);
                                    conn.Close();

                                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                                    {

                                        string sqlInsertNotificationType_Users = @"INSERT INTO dbo.NotificationType_Users (idNotificationType, idUsers, status) VALUES (@idNotificationType, @idUsers, @status)";

                                        command = new SqlCommand(sqlInsertNotificationType_Users, conn);
                                        command.Parameters.AddWithValue("@idUsers", rut);
                                        command.Parameters.AddWithValue("@idNotificationType", _dr[0].ToString());
                                        command.Parameters.AddWithValue("@status", 1);
                                        conn.Open();
                                        command.ExecuteScalar();
                                        conn.Close();

                                    }
                                }
                                status.status = "Success";
                                status.description = "User existed";
                            }
                            catch (Exception ex) 
                            {
                                status.status = "Error";
                                status.description = ex.Message;
                            }
                        }
                        else 
                        {
                            try
                            {

                                string sqlInsert = @"UPDATE dbo.Users SET  firstName = @firstName, lastName = @lastName, creation = GETDATE(), status = @status
                                                WHERE idUsers = @idUsers;
                                                INSERT INTO Device (idDevice, token, appVersion, creation, status, idPlataform)
                                                VALUES (@idDevice,'','',GETDATE(),1,@idPlataform) ";

                                command = new SqlCommand(sqlInsert, conn);
                                command.Parameters.AddWithValue("@idUsers", rut);
                                command.Parameters.AddWithValue("@idDevice", idDevice);
                                command.Parameters.AddWithValue("@idPlataform", idPlataform);
                                command.Parameters.AddWithValue("@firstName", firstName);
                                command.Parameters.AddWithValue("@status", 1);
                                command.Parameters.AddWithValue("@lastName", lastName);
                                conn.Open();
                                command.ExecuteScalar();
                                conn.Close();



                                string sqlCount = @"SELECT COUNT(*) FROM dbo.Users_Device WHERE idUsers = @idUsers";

                                command = new SqlCommand(sqlCount, conn);
                                command.Parameters.AddWithValue("@idUsers", rut);
                                command.Parameters.AddWithValue("@idDevice", idDevice);
                                conn.Open();
                                countD = Convert.ToInt32(command.ExecuteScalar());
                                conn.Close();
                                if (countD != 0)
                                {


                                    string sqlInsertDevice_Users = @"UPDATE dbo.Users_Device SET idDevice = @idDevice WHERE idUsers = @idUsers";

                                    command = new SqlCommand(sqlInsertDevice_Users, conn);
                                    command.Parameters.AddWithValue("@idUsers", rut);
                                    command.Parameters.AddWithValue("@idDevice", idDevice);
                                    conn.Open();
                                    command.ExecuteScalar();
                                    conn.Close();


                                }
                                else
                                {
                                    string sqlInsertDevice_Users = @"INSERT INTO dbo.Users_Device (idUsers, idDevice) VALUES (@idUsers, @idDevice)";

                                    command = new SqlCommand(sqlInsertDevice_Users, conn);
                                    command.Parameters.AddWithValue("@idUsers", rut);
                                    command.Parameters.AddWithValue("@idDevice", idDevice);
                                    conn.Open();
                                    command.ExecuteScalar();
                                    conn.Close();



                                }



                                string sqlCountNotificationTypes = @"SELECT COUNT(*) FROM dbo.NotificationType_Users WHERE idUsers = @idDevice";

                                command = new SqlCommand(sqlCountNotificationTypes, conn);
                                command.Parameters.AddWithValue("@idDevice", rut);
                                conn.Open();
                                countD = Convert.ToInt32(command.ExecuteScalar());
                                conn.Close();
                                if (countD == 0)
                                {
                                    string sqlSelectNotificationTypes = @"SELECT * FROM dbo.NotificationType";

                                    command = new SqlCommand(sqlSelectNotificationTypes, conn);
                                    conn.Open();
                                    SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                                    DataSet dtDatos = new DataSet();
                                    daAdaptador.Fill(dtDatos);
                                    conn.Close();

                                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                                    {

                                        string sqlInsertNotificationType_Users = @"INSERT INTO dbo.NotificationType_Users (idNotificationType, idUsers, status) VALUES (@idNotificationType, @idUsers, @status)";

                                        command = new SqlCommand(sqlInsertNotificationType_Users, conn);
                                        command.Parameters.AddWithValue("@idUsers", rut);
                                        command.Parameters.AddWithValue("@idNotificationType", _dr[0].ToString());
                                        command.Parameters.AddWithValue("@status", 1);
                                        conn.Open();
                                        command.ExecuteScalar();
                                        conn.Close();

                                    }
                                }
                                status.status = "Success";
                                status.description = "User created (Updated user)";
                            }
                            catch (Exception ex)
                            {
                                status.status = "Error";
                                status.description = ex.Message;
                            }
                        }                   
                }
            }
            catch(Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
            }

            return status;
        }

        public static Status DeleteUser(string rut) 
        {
            Status status = new Status();

            try
            {
                string selectDevice = @"SELECT D.idDevice FROM dbo.Device D, dbo.Users_Device DU WHERE D.idDevice = DU.idDevice AND DU.idUsers = @rut";

                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(selectDevice, conn);

                command.Parameters.AddWithValue("@rut", rut);
                conn.Open();

                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                DataSet dtDatos = new DataSet();
                daAdaptador.Fill(dtDatos);

                conn.Close();

                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {

                    string sqlUpdateDevice = @"UPDATE dbo.Device SET status = 0 WHERE idDevice = @idDevice";

                    command = new SqlCommand(sqlUpdateDevice, conn);
                    command.Parameters.AddWithValue("@idDevice", _dr[0].ToString());
                    conn.Open();
                    command.ExecuteScalar();
                    conn.Close();


                }

                string sqlUpdateUser = @"UPDATE dbo.Users SET status = 0 WHERE idUsers = @idUsers";

                command = new SqlCommand(sqlUpdateUser, conn);
                command.Parameters.AddWithValue("@idUsers", rut);
                conn.Open();
                command.ExecuteScalar();
                conn.Close();

                status.status = "Success";
                status.description = "disabled user";

            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
            }

            return status;
        }

        public static Status<List<Data_PushBBVA.Notification>> Notification(string rut, string idNotificationType, string Limit)
        {
            Status<List<Data_PushBBVA.Notification>> status = new Status<List<Data_PushBBVA.Notification>>();

            try
            {
                string SelectNotification = "";
                if (idNotificationType == "0")
                {

                    SelectNotification = @"SET ROWCOUNT @Limit SELECT DISTINCT H.idHistorical, U.firstName, U.lastName, U.idUsers, P.description, D.idDevice, H.creation, H.delivery, NT.idNotificationType, H.shortText, H.longText, H.status 
                                       FROM dbo.Historical H, dbo.Plataform P, dbo.Users U, dbo.Device D, dbo.NotificationType NT
                                       WHERE NT.idNotificationType = H.idNotificationType AND P.idPlataform = H.idPlataform AND  D.idDevice = H.idDevice AND U.idUsers = H.idUsers AND H.idUsers = @idUsers ORDER BY H.delivery DESC";
                }
                else
                {
                    SelectNotification = @"SET ROWCOUNT @Limit SELECT DISTINCT H.idHistorical, U.firstName, U.lastName, U.idUsers, P.description, D.idDevice, H.creation, H.delivery, NT.idNotificationType, H.shortText, H.longText, H.status 
                                      FROM dbo.Historical H, dbo.Plataform P, dbo.Users U, dbo.Device D, dbo.NotificationType NT
                                      WHERE NT.idNotificationType = H.idNotificationType AND P.idPlataform = H.idPlataform AND  D.idDevice = H.idDevice AND U.idUsers = H.idUsers AND H.idUsers = @idUsers AND H.idNotificationType = @idNotificationType  ORDER BY H.delivery DESC";
                }


                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(SelectNotification, conn);

                command.Parameters.AddWithValue("@idUsers", rut);
                command.Parameters.AddWithValue("@Limit", int.Parse(Limit));
                command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
                conn.Open();

                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                DataSet dtDatos = new DataSet();
                daAdaptador.Fill(dtDatos);

                conn.Close();

                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {
                    if (status.Data == null)
                    {
                        status.Data = new List<Data_PushBBVA.Notification>();
                    }

                    Data_PushBBVA.Notification Notifications = new Data_PushBBVA.Notification();
                    Notifications.idNotification = int.Parse(_dr[0].ToString());
                    Notifications.name = _dr[1].ToString() + " " + _dr[2].ToString();
                    Notifications.rut = _dr[3].ToString();
                    Notifications.plataform = _dr[4].ToString();
                    Notifications.device = _dr[5].ToString();
                    Notifications.create = Convert.ToDateTime(_dr[6].ToString());
                    Notifications.delivery = Convert.ToDateTime(_dr[7].ToString());
                    Notifications.tipo = _dr[8].ToString();
                    Notifications.title = _dr[9].ToString();
                    Notifications.text = _dr[10].ToString();
                    Notifications.status = Boolean.Parse(_dr[11].ToString());
                    


                    status.Data.Add(Notifications);

                }
                status.Data.Reverse();
                status.status = "Success";
                status.description = "List Notification";
            }
            catch (Exception ex) 
            {
                status.status = "Error";
                status.description = ex.Message;
            }
            return status;
        }

        public static Status<List< Data_PushBBVA.Notification>> NotificationAll(string idNotificationType, string Limit)
        {
            Status<List<Data_PushBBVA.Notification>> status = new Status<List<Data_PushBBVA.Notification>>();

            try
            {
                string SelectNotification = "";
                if (idNotificationType == "0")
                {

                    SelectNotification = @"SET ROWCOUNT @Limit SELECT DISTINCT H.idHistorical, U.firstName, U.lastName, U.idUsers, P.description, D.idDevice, H.creation, H.delivery, NT.idNotificationType, H.shortText, H.longText, H.status 
                                       FROM dbo.Historical H, dbo.Plataform P, dbo.Users U, dbo.Device D, dbo.NotificationType NT
                                       WHERE NT.idNotificationType = H.idNotificationType AND P.idPlataform = H.idPlataform AND  D.idDevice = H.idDevice AND U.idUsers = H.idUsers ORDER BY H.delivery DESC";
                }
                else
                {
                    SelectNotification = @"SET ROWCOUNT @Limit SELECT DISTINCT H.idHistorical, U.firstName, U.lastName, U.idUsers, P.description, D.idDevice, H.creation, H.delivery, NT.idNotificationType, H.shortText, H.longText, H.status 
                                      FROM dbo.Historical H, dbo.Plataform P, dbo.Users U, dbo.Device D, dbo.NotificationType NT
                                      WHERE NT.idNotificationType = H.idNotificationType AND P.idPlataform = H.idPlataform AND  D.idDevice = H.idDevice AND U.idUsers = H.idUsers AND H.idNotificationType = @idNotificationType  ORDER BY H.delivery DESC";
                }


                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(SelectNotification, conn);

                command.Parameters.AddWithValue("@Limit", int.Parse(Limit));
                command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
                conn.Open();

                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                DataSet dtDatos = new DataSet();
                daAdaptador.Fill(dtDatos);

                conn.Close();

                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {
                    if (status.Data == null)
                    {
                        status.Data = new List<Data_PushBBVA.Notification>();
                    }

                    Data_PushBBVA.Notification Notifications = new Data_PushBBVA.Notification();
                    Notifications.idNotification = int.Parse(_dr[0].ToString());
                    Notifications.name = _dr[1].ToString() + " " + _dr[2].ToString();
                    Notifications.rut = _dr[3].ToString();
                    Notifications.plataform = _dr[4].ToString();
                    Notifications.device = _dr[5].ToString();
                    Notifications.create = Convert.ToDateTime(_dr[6].ToString());
                    Notifications.delivery = Convert.ToDateTime(_dr[7].ToString());
                    Notifications.tipo = _dr[8].ToString();
                    Notifications.title = _dr[9].ToString();
                    Notifications.text = _dr[10].ToString();
                    Notifications.status = Boolean.Parse(_dr[11].ToString());



                    status.Data.Add(Notifications);

                }
                status.Data.Reverse();
                status.status = "Success";
                status.description = "List Notification";
            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
            }
            return status;
        }

        public static Status Setting(string rut, string idNotificationType)
        {
            Status status = new Status();
            try
            {

                string selectNotificationtype = @"SELECT status FROM dbo.NotificationType_Users WHERE idUsers = @idUsers AND idNotificationType = @idNotificationType";

                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(selectNotificationtype, conn);

                command.Parameters.AddWithValue("@idUsers", rut);
                command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
                conn.Open();

                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                DataSet dtDatos = new DataSet();
                daAdaptador.Fill(dtDatos);

                conn.Close();

                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {

                    string updateSetting = @"UPDATE dbo.NotificationType_Users SET status = @status WHERE idUsers = @idUsers AND idNotificationType = @idNotificationType";


                    command = new SqlCommand(updateSetting, conn);
                    command.Parameters.AddWithValue("@idUsers", rut);
                    command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
                    if (bool.Parse(_dr[0].ToString()) == true)
                    {
                        command.Parameters.AddWithValue("@status", 0);
                    } 
                    else 
                    {
                        command.Parameters.AddWithValue("@status", 1);
                    }

                    conn.Open();
                    command.ExecuteScalar();
                    conn.Close();

                }
                              
                status.status = "Success";
                status.description = "Update Setting";

            }catch(Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
            }
            return status;

        
        }

        public static Status changeSchedule(string update, string idNotificationType)
        {
            Status status = new Status();
            string text = base64ToText(update);
            DateTime time = Convert.ToDateTime(text);
            try
            {

                string selectNotificationtype = @"UPDATE dbo.NotificationType SET start = @update WHERE idNotificationType = @idNotificationType";

                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(selectNotificationtype, conn);
                command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
                command.Parameters.AddWithValue("@update", text);
             
                conn.Open();
                command.ExecuteScalar();
                conn.Close();
             
             
             
                status.status = "Success";
                status.description = "Update Setting";

            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
            }
            return status;


        }

        public static Status readNotification(string idHistorical)
        {
            Status status = new Status();
            try
            {

                string updateHistorical = @"UPDATE dbo.Historical SET status = @status WHERE idHistorical = @idHistorical";

                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(updateHistorical, conn);
                command.Parameters.AddWithValue("@status", 1);
                command.Parameters.AddWithValue("@idHistorical", idHistorical);

                conn.Open();
                command.ExecuteScalar();
                conn.Close();



                status.status = "Success";
                status.description = "Update Setting";

            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
            }
            return status;


        }

        public static Status<List<NotificationType>> ListSetting(string rut)
        {
            Status<List<NotificationType>> status = new Status<List<NotificationType>>();

            try
            {

                string selectNotificationType = @"SELECT NT.idNotificationType, Nt.title, NTU.status, NT.start FROM dbo.NotificationType_Users NTU, dbo.NotificationType NT WHERE NT.idnotificationType = NTU.idNotificationType AND  NTU.idUsers = @idUsers";


                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(selectNotificationType, conn);

                command.Parameters.AddWithValue("@idUsers", rut);
                conn.Open();

                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                DataSet dtDatos = new DataSet();
                daAdaptador.Fill(dtDatos);

                conn.Close();

                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {
                    if (status.Data == null)
                    {
                        status.Data = new List<NotificationType>();
                    }

                    NotificationType Type = new NotificationType();
                    Type.idNotificationType = int.Parse(_dr[0].ToString());
                    Type.title = _dr[1].ToString();
                    Type.status = _dr[2].ToString();
                    Type.start = _dr[3].ToString();

                    status.Data.Add(Type);
                }
                status.Data.Reverse();
                status.status = "Success";
                status.description = "Setting list";
            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
            }

            return status;
        
        
        }

        static void Historical(string idNotificationType, string idUsers, string idDevice, string idPlataform, string shortText, string longText)
        {
            try
            {
                string Insert = @" Insert into dbo.Historical (creation, delivery, idNotificationType, idUsers, idDevice, idPlataform, shortText, longText, status) 
                               Values ( GETDATE(), GETDATE(), @idNotificationType, @idUsers, @idDevice, @idPlataform, @shortText, @longText, 0)";


                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(Insert, conn);
                command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
                command.Parameters.AddWithValue("@idUsers", idUsers);
                command.Parameters.AddWithValue("@idDevice", idDevice);
                command.Parameters.AddWithValue("@idPlataform", idPlataform);
                command.Parameters.AddWithValue("@shortText", shortText);
                command.Parameters.AddWithValue("@longText", longText);

                conn.Open();
                command.ExecuteScalar();
                conn.Close();
            }
            catch (Exception ex)
            {
            }
        }

        public static Status sendNotification(string rut, string idNotificationType, string shortText, string longText)
        {
            Status status = new Status();

            
            try
            {
                var push = new PushSharp.PushBroker();

                push.OnNotificationSent += NotificationSent;
                push.OnChannelException += ChannelException;
                push.OnServiceException += ServiceException;
                push.OnNotificationFailed += NotificationFailed;
                push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                push.OnChannelCreated += ChannelCreated;
                push.OnChannelDestroyed += ChannelDestroyed;


             

            

                string selectDevice = @"SELECT D.idDevice, D.idPlataform 
                                        FROM dbo.Device D, dbo.Users_Device UD, dbo.NotificationType_Users NTU
                                        WHERE D.status = 1  AND D.idDevice = UD.idDevice AND NTU.status = 1 AND NTU.idNotificationType = @idNotificationType AND NTU.idUsers = UD.idUsers AND UD.idUsers = @idUsers ";

                
                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(selectDevice, conn);

                command.Parameters.AddWithValue("@idUsers", rut);
                command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
                conn.Open();

                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                DataSet dtDatos = new DataSet();
                daAdaptador.Fill(dtDatos);

                conn.Close();

                status.status = "Error";
                status.description = "unregistered user";

                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {
                   string text = base64ToText(shortText);
                   //var text = Convert.FromBase64String(shortText);
                    if (_dr[1].ToString() == "2")
                    {
                        var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificados.p12"));
                        push.RegisterAppleService(new ApplePushChannelSettings(false, appleCert, "q1w2e3r4"));
                        push.QueueNotification(new AppleNotification()
                                                   .ForDeviceToken(_dr[0].ToString())
                                                   .WithAlert(text.ToString())
                                                   .WithBadge(-1)
                                                   .WithSound("sound.caf"));
                        push.StopAllServices();
                        status.status = "Success";
                        status.description = "Send iOS";

                       Historical(idNotificationType, rut, _dr[0].ToString(), _dr[1].ToString(), shortText, longText);
                    }
                    else if (_dr[1].ToString() == "1")
                    {
                        push.RegisterGcmService(new GcmPushChannelSettings("AIzaSyBbsQnPByBI484hHMLOC_FRLowkIKqlWO0"));

                        push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(_dr[0].ToString())
                                              .WithJson("{\"alert\":\"+ text +\",\"badge\":7,\"sound\":\"sound.caf\"}"));
                        push.StopAllServices();
                        status.status = "Success";
                        status.description = "Send Android";

                        Historical(idNotificationType, rut, _dr[0].ToString(), _dr[1].ToString(), shortText, longText);
                    }
                }
               

            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
            }
            return status;
        }

        void insertHistorical(string idNotificationType, string idUsers,string idDevice, string idPlataform,string shortText, string longText)
        {
            try
            {
                string Insert = @" Insert into dbo.Historical (creation, delivery, idNotificationType, idUsers, idDevice, idPlataform, shortText, longText, status) 
                               Values ( GETDATE(), GETDATE(), @idNotificationType, @idUsers, @idDevice, @idPlataform, @shortText, @longText, 0)";


                SqlConnection conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(Insert, conn);
                command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
                command.Parameters.AddWithValue("@idUsers", idUsers);
                command.Parameters.AddWithValue("@idDevice", idDevice);
                command.Parameters.AddWithValue("@idPlataform", idPlataform);
                command.Parameters.AddWithValue("@shortText", shortText);
                command.Parameters.AddWithValue("@longText", longText);
            
                conn.Open();
                command.ExecuteScalar();
                conn.Close();
            }
            catch (Exception ex)
            {
            }
        }

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, PushSharp.Core.INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, PushSharp.Core.INotification notification)
        {
            Console.WriteLine("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, PushSharp.Core.INotification notification, Exception notificationFailureException)
        {
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        static void ChannelException(object sender, PushSharp.Core.IPushChannel channel, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, PushSharp.Core.INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {
            Console.WriteLine("Channel Created for: " + sender);
        }

        static string textToBase64(string sAscii)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(sAscii);
            return System.Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        static string base64ToText(string sbase64)
        {
            byte[] bytes = System.Convert.FromBase64String(sbase64);
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetString(bytes, 0, bytes.Length);
        } 
        
        ///////// ANTIGUOS ///////////////


//        public static bool Login(string user, string pass)
//        {
           
//            string sql = @"SELECT COUNT(*)
//                       FROM Admin
//                       WHERE idAdmin = @usuario AND password = @password ";

//            using (SqlConnection conn = new SqlConnection(connectionString: conex))
//            {
              
//                    conn.Open();

//                    SqlCommand command = new SqlCommand(sql, conn);
//                    command.Parameters.AddWithValue("@usuario", user);
//                    command.Parameters.AddWithValue("@password", pass);

//                    int count = Convert.ToInt32(command.ExecuteScalar());

//                    if (count == 0)
//                        return false;
//                    else
//                        return true;

//            }
//        }

//        public static Data_PushBBVA.Login Login2(string user, string pass)
//        {

//            string sql = @"SELECT *
//                       FROM Admin
//                       WHERE idAdmin = @usuario AND password = @password ";


//            using (SqlConnection conn = new SqlConnection(connectionString: conex))
//            {
//                Data_PushBBVA.Login access = new Data_PushBBVA.Login();

//                try
//                {
//                    SqlCommand command = new SqlCommand(sql, conn);

//                    command.Parameters.AddWithValue("@usuario", user);
//                    command.Parameters.AddWithValue("@password", pass);

//                    conn.Open();

//                    SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
//                    DataSet dtDatos = new DataSet();
//                    daAdaptador.Fill(dtDatos);

//                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                    {
//                        access.name = _dr["firtsName"].ToString() + " " + _dr["lastName"].ToString();
//                        access.user = _dr["idAdmin"].ToString();
//                        access.status = "Success";
//                    }

//                    return access;

//                }
//                catch (Exception ex)
//                {
//                    access.status = "Error";
//                    return access;
//                }

//            }
//        }

//        public static List<Data_PushBBVA.NotificationType> NotificationType() 
//        {

//            List<Data_PushBBVA.NotificationType> Type = new List<Data_PushBBVA.NotificationType>();

//            string sql = @"SELECT * FROM NotificationType";

//            using (SqlConnection conn = new SqlConnection(connectionString: conex))
//            {             
//                try
//                {
//                    SqlCommand command = new SqlCommand(sql, conn);                   

//                    conn.Open();

//                    SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
//                    DataSet dtDatos = new DataSet();
//                    daAdaptador.Fill(dtDatos);

//                    foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                    {
//                        Data_PushBBVA.NotificationType types = new Data_PushBBVA.NotificationType();
//                        types.idNotificationType = int.Parse(_dr["idNotificationType"].ToString());
//                        types.description = _dr["description"].ToString();
//                        types.text = _dr["text"].ToString();
//                        Type.Add(types);
//                    }

//                    return Type;

//                }
//                catch (Exception ex)
//                {
                   
//                    return Type;
//                }

//            }

            
//        }

//        public static string createUser(string user, string idDevice, string plataform)
//        {
//            string status = "";
//            string IdUser = "";
//            string IdDevice = idDevice;
//            string token = "";
//            string plataforma = plataform;
//            string firstName = "";
//            string lastName = "";
//            string appVersion = "1.0";
//            string appId = "1.0";

//            string sql = @"SELECT COUNT(*) FROM Users WHERE rut = @usuario";

//            using (SqlConnection conn = new SqlConnection(connectionString: conex))
//            {
//                    SqlCommand command = new SqlCommand(sql, conn);
//                    command.Parameters.AddWithValue("@usuario", user);
//                    int count = 0;
//                    conn.Open();
//                    count = Convert.ToInt32(command.ExecuteScalar());

//                    if (count == 0)
//                    {                        
//                            string createUser = @"INSERT INTO Users (firstName, lastName, rut, creation, status)
//                                                 VALUES (@firstName, @lastName, @rut, @creation, @status)";

//                            using (SqlConnection conn2 = new SqlConnection(connectionString: conex))
//                            {
//                                SqlCommand command2 = new SqlCommand(createUser, conn2);
//                                command2.Parameters.AddWithValue("@firstName", firstName);
//                                command2.Parameters.AddWithValue("@lastName", lastName);
//                                command2.Parameters.AddWithValue("@rut", user);
//                                command2.Parameters.AddWithValue("@creation", DateTime.Now);
//                                command2.Parameters.AddWithValue("@status", 1);
//                                conn2.Open();
//                                command2.ExecuteScalar();
//                            }

//                            string selectUser = @"SELECT idUsers FROM Users WHERE rut = @rut";                          
                          
//                            using (SqlConnection conn3 = new SqlConnection(connectionString: conex))
//                            {
//                                SqlCommand command3 = new SqlCommand(selectUser, conn3);
//                                command3.Parameters.AddWithValue("@rut", user);
//                                conn3.Open();
                            
//                                SqlDataAdapter daAdaptador = new SqlDataAdapter(command3);
//                                DataSet dtDatos = new DataSet();
//                                daAdaptador.Fill(dtDatos);

//                                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                                {
//                                    IdUser = _dr["idUsers"].ToString();
//                                }                              
//                            }

//                            string createDevice = @"INSERT INTO Device (idDevice, token, appVersion, creation, status, idPlataform)
//                                                 VALUES (@idDevice, @token, @appVersion, @creation, @status, @idPlataform)";

//                            using (SqlConnection conn4 = new SqlConnection(connectionString: conex))
//                            {
//                                SqlCommand command4 = new SqlCommand(createDevice, conn4);
//                                command4.Parameters.AddWithValue("@idDevice", idDevice);
//                                command4.Parameters.AddWithValue("@token", token);
//                                command4.Parameters.AddWithValue("@appVersion", appVersion);
//                                command4.Parameters.AddWithValue("@creation", DateTime.Now);
//                                command4.Parameters.AddWithValue("@status", 1);
//                                command4.Parameters.AddWithValue("@idPlataform", plataforma);
//                                conn4.Open();
//                                command4.ExecuteScalar();
//                            }

//                            string selectDevice = @"SELECT idDevice FROM Device WHERE token = @token";

//                            using (SqlConnection conn5 = new SqlConnection(connectionString: conex))
//                            {
//                                SqlCommand command5 = new SqlCommand(selectDevice, conn5);
//                                command5.Parameters.AddWithValue("@token", token);
//                                conn5.Open();

//                                SqlDataAdapter daAdaptador = new SqlDataAdapter(command5);
//                                DataSet dtDatos = new DataSet();
//                                daAdaptador.Fill(dtDatos);

//                                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                                {
//                                    IdDevice = _dr["idDevice"].ToString();
//                                }
//                            }

//                            string create = @"INSERT INTO Users_Device (idUsers, idDevice)
//                                                 VALUES (@idUsers, @idDevice)";

//                            using (SqlConnection conn6 = new SqlConnection(connectionString: conex))
//                            {
//                                SqlCommand command6 = new SqlCommand(create, conn6);
//                                command6.Parameters.AddWithValue("@idUsers", IdUser);
//                                command6.Parameters.AddWithValue("@idDevice", IdDevice);                
//                                conn6.Open();
//                                command6.ExecuteScalar();
//                            }
                            
//                            string selectNotification = @"SELECT idNotificationType FROM NotificationType";

//                            using (SqlConnection conn7 = new SqlConnection(connectionString: conex))
//                            {
//                                SqlCommand command7 = new SqlCommand(selectNotification, conn7);
//                                conn7.Open();

//                                SqlDataAdapter daAdaptador = new SqlDataAdapter(command7);
//                                DataSet dtDatos = new DataSet();
//                                daAdaptador.Fill(dtDatos);

//                                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                                {
                                    
//                                    string create2 = @"INSERT INTO NotificationType_Users (idUsers, idNotificationType, status)
//                                                 VALUES (@idUser, @idNotificatioType, @status)";

//                                    using (SqlConnection conn8 = new SqlConnection(connectionString: conex))
//                                    {
//                                        SqlCommand command8 = new SqlCommand(create2, conn8);
//                                        command8.Parameters.AddWithValue("@idUser", IdUser);
//                                        command8.Parameters.AddWithValue("@idNotificatioType", _dr["idNotificationType"].ToString());
//                                        command8.Parameters.AddWithValue("@status", 1);
//                                        conn8.Open();
//                                        command8.ExecuteScalar();
//                                    }
//                                }
//                            }
//                            status = "Success";
//                    }
//                    else 
//                    {
//                        status = "Error";
//                    }
             

//            }

//            return status;
        
//        }

//        public static List<Data_PushBBVA.Notification> List(string rut, string type){


//            string sql = @"SELECT N.idNotification, N.status, NT.idNotificationType ,  N.creation, N.delivery, U.firstName, U.lastName, U.rut, NT.title, NT.text, D.token, P.description 
//                            FROM dbo.Notification N, dbo.Users U, dbo.NotificationType NT, dbo.Device D, dbo.Plataform P
//                            WHERE P.idPlataform = D.idPlataform  AND D.idDevice = N.idDevice AND NT.idNotificationType = N.idNotificationType AND N.status = @type AND N.idUser = U.idUser AND U.rut = @rut"; 
            

//             using (SqlConnection conn = new SqlConnection(connectionString: conex))
//             {
                 
               

//                    List<Data_PushBBVA.Notification> notification = new List<Data_PushBBVA.Notification>();

//                     SqlCommand command = new SqlCommand(sql, conn);

//                     command.Parameters.AddWithValue("@rut", rut);
//                     command.Parameters.AddWithValue("@type", type);

//                     conn.Open();

//                     SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
//                     DataSet dtDatos = new DataSet();
//                     daAdaptador.Fill(dtDatos);

//                     foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                     {
//                         Data_PushBBVA.Notification Notifi = new Data_PushBBVA.Notification();
//                         Notifi.idNotification = int.Parse(_dr[0].ToString());
//                         Notifi.name = _dr[5].ToString() + " " + _dr[6].ToString();
//                         Notifi.create = DateTime.Parse(_dr[3].ToString());
//                         Notifi.delivery = DateTime.Parse(_dr[4].ToString());
//                         Notifi.tipo = _dr[2].ToString();
//                         Notifi.rut = _dr[7].ToString();
//                         Notifi.text = _dr[9].ToString();
//                         Notifi.title = _dr[8].ToString();
//                        // Notifi.token = _dr[10].ToString();
//                         Notifi.plataform = _dr[11].ToString();


//                         notification.Add(Notifi);
//                     }


//                     return notification;
                 
//             }

        
        
//        }

//        public static List<Data_PushBBVA.Notification> ListNotification(string type){


//            string sql = @"SELECT N.idNotification, N.status, NT.idNotificationType ,  N.creation, N.delivery, U.firstName, U.lastName, U.rut, NT.title, NT.text, D.token, P.description 
//                            FROM dbo.Notification N, dbo.Users U, dbo.NotificationType NT, dbo.Device D, dbo.Plataform P
//                            WHERE P.idPlataform = D.idPlataform  AND D.idDevice = N.idDevice AND NT.idNotificationType = N.idNotificationType AND N.idUser = U.idUser AND N.status = @type"; 
           

//             using (SqlConnection conn = new SqlConnection(connectionString: conex))
//             {
                 
               

//                    List<Data_PushBBVA.Notification> notification = new List<Data_PushBBVA.Notification>();

//                     SqlCommand command = new SqlCommand(sql, conn);

                   
//                     command.Parameters.AddWithValue("@type", type);

//                     conn.Open();

//                     SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
//                     DataSet dtDatos = new DataSet();
//                     daAdaptador.Fill(dtDatos);

//                     foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                     {
//                         Data_PushBBVA.Notification Notifi = new Data_PushBBVA.Notification();
//                         Notifi.idNotification = int.Parse(_dr[0].ToString());
//                         Notifi.name = _dr[5].ToString() + " " + _dr[6].ToString();
//                         Notifi.create = DateTime.Parse(_dr[3].ToString());
//                         Notifi.delivery = DateTime.Parse(_dr[4].ToString());
//                         Notifi.tipo = _dr[2].ToString();
//                         Notifi.rut = _dr[7].ToString();
//                         Notifi.text = _dr[9].ToString();
//                         Notifi.title = _dr[8].ToString();
//                         //Notifi.token = _dr[10].ToString();
//                         Notifi.plataform = _dr[11].ToString();


//                         notification.Add(Notifi);
//                     }


//                     return notification;
                 
//             }

        
        
//        }

//        public static string UpdateDevice(string idDevice, string token) {

//            string createUser = @"UPDATE dbo.Device SET token = @token WHERE idDevice = @idDevice";
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString: conex))
//                {
//                    SqlCommand command = new SqlCommand(createUser, conn);
//                    command.Parameters.AddWithValue("@idDevice", idDevice);
//                    command.Parameters.AddWithValue("@token", token);
//                    conn.Open();
//                    command.ExecuteScalar();

//                    return "Success";
//                }
//            }
//            catch (Exception ex)
//            {
//                return "Error";
//            }
            
//        }

//        public static List<Data_PushBBVA.NotificationUser> ListSetting(string rut) {


//            string sql = @"SELECT NTU.status, NT.title, NT.idNotificationType
//FROM dbo.Users U, dbo.NotificationType NT, dbo.User_NotificationType NTU
//WHERE NT.idNotificationType = NTU.idNotificationType AND NTU.idUser = U.idUser AND U.rut = @rut";


//            using (SqlConnection conn = new SqlConnection(connectionString: conex))
//            {



//                List<Data_PushBBVA.NotificationUser> Setting = new List<Data_PushBBVA.NotificationUser>();

//                SqlCommand command = new SqlCommand(sql, conn);


//                command.Parameters.AddWithValue("@rut", rut);

//                conn.Open();

//                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
//                DataSet dtDatos = new DataSet();
//                daAdaptador.Fill(dtDatos);

//                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
//                {
//                    Data_PushBBVA.NotificationUser setting = new Data_PushBBVA.NotificationUser();
//                    setting.status = _dr[0].ToString();
//                    setting.title = _dr[1].ToString();
//                    setting.idNotificationType = int.Parse(_dr[2].ToString());
//                    Setting.Add(setting);
//                }


//                return Setting;

//            }

        
        
//        }
      
    }
}
