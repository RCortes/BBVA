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
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Net;


namespace Connection_PushBBVA
{
    public class SQLConnection
    {
        public static string conex = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

        public static Status<Login> Login(string user, string pass)
        { 
            Status<Login> status = new Status<Login>();
            SqlConnection conn = new SqlConnection(connectionString: conex);
            try{

               string sql = @"SELECT *
                               FROM Admin
                               WHERE idAdmin = @usuario AND pass = @password ";         
            
               
                conn = new SqlConnection(connectionString: conex);
                conn.Open();

                user = removeRut(user);

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@usuario", user);
                command.Parameters.AddWithValue("@password", pass);

                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                DataSet dtDatos = new DataSet();
                daAdaptador.Fill(dtDatos);
                conn.Close();
                status.status = "Error";
                status.description = "Access denied";

                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {
                    if (status.Data == null)
                    {
                        status.Data = new Login();
                    }
                    status.Data.name = _dr["firstName"].ToString() + " " + _dr["lastName"].ToString();
                    status.Data.user = _dr["idAdmin"].ToString();
                    status.status = "Success";
                    status.description = "Allowed access";              
                }

            }catch(Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
                conn.Close();
            }

            return status;
           
        }

        public static Status CreateUser(string rut, string firstName, string lastName, string idDevice, string idPlataform)
        {          
            Status status = new Status();
            SqlConnection conn = new SqlConnection(connectionString: conex);
            rut = removeRut(rut);
            rut = agregateCero(rut);


            if (lastName == "0" || firstName == "0")
            {
                lastName = firstName = "";
            }



            try 
            {
                string sqlCountUser = @"SELECT COUNT(*) FROM dbo.Users WHERE idUsers = @usuario";

                conn = new SqlConnection(connectionString: conex);
                SqlCommand command = new SqlCommand(sqlCountUser, conn);
                command.Parameters.AddWithValue("@usuario", removeRut(rut));
                
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
                                command.Parameters.AddWithValue("@firstName",  firstName);
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
                                conn.Close();
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
                                conn.Close();
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




                                string sqlDelete = @"DELETE FROM dbo.Users_Device WHERE idUsers = @idUsers; DELETE FROM dbo.Users_Device WHERE idDevice = @idDevice;";

                                command = new SqlCommand(sqlDelete, conn);
                                command.Parameters.AddWithValue("@idUsers", rut);
                                command.Parameters.AddWithValue("@idDevice", idDevice);
                                conn.Open();
                                countD = Convert.ToInt32(command.ExecuteScalar());
                                conn.Close();

                              


                                //string sqlCount = @"SELECT COUNT(*) FROM dbo.Users_Device WHERE idUsers = @idUsers AND idDevice = @idDevice";

                                //command = new SqlCommand(sqlCount, conn);
                                //command.Parameters.AddWithValue("@idUsers", rut);
                                //command.Parameters.AddWithValue("@idDevice", idDevice);
                                //conn.Open();
                                //countD = Convert.ToInt32(command.ExecuteScalar());
                                //conn.Close();

                                //if (countD == 0)
                                //{
                                    string sqlInsertDevice_Users = @"INSERT INTO dbo.Users_Device (idUsers, idDevice) VALUES (@idUsers, @idDevice)";

                                    command = new SqlCommand(sqlInsertDevice_Users, conn);
                                    command.Parameters.AddWithValue("@idUsers", rut);
                                    command.Parameters.AddWithValue("@idDevice", idDevice);
                                    conn.Open();
                                    command.ExecuteScalar();
                                    conn.Close();
                                //}

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
                                conn.Close();
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

                                    string sqlInsert2 = @"INSERT INTO Device (idDevice, token, appVersion, creation, status, idPlataform)
                                                           VALUES (@idDevice,'','',GETDATE(),1,@idPlataform);
                                                           INSERT INTO Users (idUsers, firstName, lastName, creation, status)
                                                           VALUES (@rut,@firstName,@lastName,GETDATE(),1);
                                                           INSERT INTO Users_Device (idUsers, idDevice)
                                                           VALUES (@rut, @idDevice)";

                                    command = new SqlCommand(sqlInsert2, conn);
                                    command.Parameters.AddWithValue("@rut", rut);
                                    command.Parameters.AddWithValue("@idDevice", idDevice);
                                    command.Parameters.AddWithValue("@idPlataform", idPlataform);
                                    command.Parameters.AddWithValue("@firstName", firstName);
                                    command.Parameters.AddWithValue("@lastName", lastName);
                                    conn.Open();
                                    command.ExecuteScalar();
                                    conn.Close();

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
                                conn.Close();
                            }
                        }                   
                }
            }
            catch(Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
                conn.Close();
            }

            return status;
        }

        public static Status DeleteUser(string rut) 
        {
            Status status = new Status();
            SqlConnection conn = new SqlConnection(connectionString: conex);
               
            rut = removeRut(rut);
            rut = agregateCero(rut);
            try
            {
                string selectDevice = @"SELECT D.idDevice FROM dbo.Device D, dbo.Users_Device DU WHERE D.idDevice = DU.idDevice AND DU.idUsers = @rut";

                conn = new SqlConnection(connectionString: conex);
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


                string sql = @"SELECT COUNT(*)
                       FROM Users
                       WHERE idUsers = @idUsers";

                command = new SqlCommand(sql, conn);

                command.Parameters.AddWithValue("@idUsers", rut);
                conn.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
                if (count == 0)
                {
                    status.status = "Error";
                    status.description = "The user does not exist";
                }
                else
                {
                    status.status = "Success";
                    status.description = "User Update";
                }

                

            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
                conn.Close();
            }

            return status;
        }

        public static Status<List<Data_PushBBVA.Notification>> Notification(string rut, string idNotificationType, string Limit)
        {
            Status<List<Data_PushBBVA.Notification>> status = new Status<List<Data_PushBBVA.Notification>>();
            rut = removeRut(rut);
            rut = agregateCero(rut);
            SqlConnection conn = new SqlConnection(connectionString: conex);
               
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
                
                conn = new SqlConnection(connectionString: conex);
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
                    Notifications.name = (SanitizeXmlString(_dr[1].ToString() + " " + _dr[2].ToString())).Trim();
                    Notifications.rut = (_dr[3].ToString()).Trim();
                    Notifications.plataform = _dr[4].ToString();
                    Notifications.device = _dr[5].ToString();
                    Notifications.create = Convert.ToDateTime(_dr[6].ToString());
                    Notifications.delivery = Convert.ToDateTime(_dr[7].ToString());
                    Notifications.tipo = _dr[8].ToString();
                    Notifications.title = (SanitizeXmlString(_dr[9].ToString())).Trim();
                    Notifications.text = (SanitizeXmlString(_dr[10].ToString())).Trim();
                    Notifications.status = Boolean.Parse(_dr[11].ToString());
                   
                    status.Data.Add(Notifications);

                }

                   string sql = @"SELECT COUNT(*)
                       FROM Users
                       WHERE idUsers = @idUsers";

                    command = new SqlCommand(sql, conn);

                    command.Parameters.AddWithValue("@idUsers", rut);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    conn.Close();
                    if (count == 0) 
                    {
                        status.status = "Error";
                        status.description = "The user does not exist";
                    }
                    else
                    {
                        status.status = "Success";
                        status.description = "List Notification";
                    }

                



               

            }
            catch (Exception ex) 
            {
                status.status = "Error";
                status.description = ex.Message;
                conn.Close();
            }
            return status;
        }

        public static Status<List< Data_PushBBVA.Notification>> NotificationAll(string idNotificationType, string Limit)
        {
            Status<List<Data_PushBBVA.Notification>> status = new Status<List<Data_PushBBVA.Notification>>();
            SqlConnection conn = new SqlConnection(connectionString: conex);
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


                conn = new SqlConnection(connectionString: conex);
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
                    Notifications.name = (SanitizeXmlString(_dr[1].ToString() + " " + _dr[2].ToString())).Trim();
                    Notifications.rut = _dr[3].ToString();
                    Notifications.plataform = _dr[4].ToString();
                    Notifications.device = _dr[5].ToString();
                    Notifications.create = Convert.ToDateTime(_dr[6].ToString());
                    Notifications.delivery = Convert.ToDateTime(_dr[7].ToString());
                    Notifications.tipo = _dr[8].ToString();  
                    Notifications.title = (SanitizeXmlString(_dr[9].ToString())).Trim();
                    Notifications.text = (SanitizeXmlString(_dr[10].ToString())).Trim();                    
                    Notifications.status = Boolean.Parse(_dr[11].ToString());
                   
                    status.Data.Add(Notifications);

                }
                status.status = "Success";
                status.description = "List Notification";
            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
                conn.Close();
            }
            return status;
        }

        public static Status Setting(string rut, string idNotificationType)
        {
            Status status = new Status();
            rut = removeRut(rut);
            rut = agregateCero(rut);
            SqlConnection conn = new SqlConnection(connectionString: conex);
            
            try
            {

                string selectNotificationtype = @"SELECT status FROM dbo.NotificationType_Users WHERE idUsers = @idUsers AND idNotificationType = @idNotificationType";

                conn = new SqlConnection(connectionString: conex);
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
                conn.Close();
            }
            return status;

        
        }

        public static Status changeSchedule(string update, string idNotificationType)
        {
            Status status = new Status();
            string text = base64ToText(update);
            DateTime time = Convert.ToDateTime(text);
            text = time.ToString("yyyy-MM-dd HH:mm:ss");
            SqlConnection conn = new SqlConnection(connectionString: conex);
            try
            {

                string selectNotificationtype = @"UPDATE dbo.NotificationType SET start = @update WHERE idNotificationType = @idNotificationType";

                conn = new SqlConnection(connectionString: conex);
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
                conn.Close();
            }
            return status;


        }

        public static Status readNotification(string idHistorical)
        {
            Status status = new Status();
            SqlConnection conn = new SqlConnection(connectionString: conex);
            try
            {

                string updateHistorical = @"UPDATE dbo.Historical SET status = @status WHERE idHistorical = @idHistorical";

                conn = new SqlConnection(connectionString: conex);
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
                conn.Close();
            }
            return status;


        }

        public static Status holdOver(string idHistorical, string expirateDate) 
        {
            Status status = new Status();

            string text = base64ToText(expirateDate);
            DateTime time = Convert.ToDateTime(text);
            text = time.ToString("yyyy-MM-dd HH:mm:ss");

            SqlConnection conn = new SqlConnection(connectionString: conex);
           
            try
            {

                string selectNotificationtype = @"SELECT * FROM Historical WHERE idHistorical = @idHistorical";

                SqlCommand command = new SqlCommand(selectNotificationtype, conn);
                command.Parameters.AddWithValue("@idHistorical", idHistorical);
                
                conn.Open();
                SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                DataSet dtDatos = new DataSet();
                daAdaptador.Fill(dtDatos);
                conn.Close();

                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {
                    int p = 3;

                    if (_dr[10].ToString() != null && _dr[10].ToString() != "")
                    {
                           p = int.Parse(_dr[10].ToString());
                    }
               

                    if (p > 0)
                    {
                        
                        DateTime expire = DateTime.Parse(text);

                        if (_dr[11].ToString() != null && _dr[11].ToString() != "")
                        {
                            expire = DateTime.Parse(_dr[11].ToString());
                        }

                        if (DateTime.Compare(DateTime.Now, expire) < 0)
                        {
                            string InsertHoldOver = @"INSERT INTO [dbo].[HoldOver]
                                                                                         ([creation]
                                                                                         ,[delivery]
                                                                                         ,[expiration]
                                                                                         ,[idNotificationType]
                                                                                         ,[idUser]
                                                                                         ,[idDevice]
                                                                                         ,[idPlataform]
                                                                                         ,[shortText]
                                                                                         ,[longText]
                                                                                         ,[status]
                                                                                         ,[deliveryPossibilities])
                                                                                   VALUES
                                                                                         (@creation
                                                                                         ,@delivery
                                                                                         ,@expirate
                                                                                         ,@idNotificationType
                                                                                         ,@idUser
                                                                                         ,@idDevice
                                                                                         ,@idPlataform
                                                                                         ,@shortText
                                                                                         ,@longText
                                                                                         ,0
                                                                                         ,@deliveryPossibilities)";
                           
                            try
                            {
                                command = new SqlCommand(InsertHoldOver, conn);
                                command.Parameters.AddWithValue("@creation", DateTime.Parse(_dr[1].ToString()));
                                command.Parameters.AddWithValue("@expirate",DateTime.Parse(text));
                                command.Parameters.AddWithValue("@delivery", DateTime.Parse(_dr[2].ToString()));
                                command.Parameters.AddWithValue("@idNotificationType", _dr[3].ToString());
                                command.Parameters.AddWithValue("@idUser", _dr[4].ToString());
                                command.Parameters.AddWithValue("@idDevice", _dr[5].ToString());
                                command.Parameters.AddWithValue("@idPlataform", _dr[6].ToString());

                                string stext = SanitizeXmlString(RemoveSpecialCharacters( _dr[7].ToString()));
                                string ltext = SanitizeXmlString(RemoveSpecialCharacters(_dr[8].ToString()));

                                command.Parameters.AddWithValue("@shortText",stext);
                                command.Parameters.AddWithValue("@longText", ltext);

                                int possibilities = p - 1;

                                command.Parameters.AddWithValue("@deliveryPossibilities", possibilities);

                                conn.Open();
                                command.ExecuteScalar();
                                conn.Close();

                                try
                                {

                                    string delete = @"DELETE FROM Historical WHERE idHistorical = @idHistorical";

                                    command = new SqlCommand(delete, conn);
                                    command.Parameters.AddWithValue("@idHistorical", _dr[0].ToString());
                                    
                                    conn.Open();
                                    command.ExecuteScalar();
                                    conn.Close();

                                    status.status = "Success";
                                    status.description = "Delayed notification (" + possibilities + ")";
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        string delete = @"UPDATE Historical SET idUsers = '0000000000', idDevice = '0' WHERE idHistorical = @idHistorical";

                                        command = new SqlCommand(delete, conn);
                                        command.Parameters.AddWithValue("@idHistorical", _dr[0].ToString());

                                        conn.Open();
                                        command.ExecuteScalar();
                                        conn.Close();

                                        status.status = "Success";
                                        status.description = "Delayed notification (" + possibilities + ")";
                                    }
                                    catch (Exception ex2)
                                    {
                                        status.status = "Error";
                                        status.description = ex2.Message;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                status.status = "Error";
                                status.description = ex.Message;
                                conn.Close();
                            }
                        }
                        else                         
                        {
                            status.status = "Error";
                            status.description = "Expired";
                            
                        }
                    }
                    else 
                    {
                        status.status = "Error";
                        status.description = "Attempts ended";
                    }
                }

                if (dtDatos.Tables[0].Rows.Count == 0) 
                {
                    status.status = "Error";
                    status.description = "notification does not exist";                
                }                

            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
                conn.Close();
            }


  



            return status;

        
        }

        public static Status<List<NotificationType>> ListSetting(string rut)
        {
            Status<List<NotificationType>> status = new Status<List<NotificationType>>();
            rut = removeRut(rut);
            rut = agregateCero(rut);
            SqlConnection conn = new SqlConnection(connectionString: conex);
            try
            {

                string selectNotificationType = @"SELECT NT.idNotificationType, Nt.title, NTU.status, NT.start, NT.duration FROM dbo.NotificationType_Users NTU, dbo.NotificationType NT WHERE NT.idnotificationType = NTU.idNotificationType AND  NTU.idUsers = @idUsers ORDER BY NTU.idNotificationType ASC";


                conn = new SqlConnection(connectionString: conex);
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
                    Type.duration = int.Parse(_dr[4].ToString());

                    status.Data.Add(Type);
                }
                //status.Data.Reverse();
                status.status = "Success";
                status.description = "Setting list";
            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
                conn.Close();
            }

            return status;
        
        
        }

        static void Historical(string idNotificationType, string idUsers, string idDevice, string idPlataform, string shortText, string longText)
        {
            SqlConnection conn = new SqlConnection(connectionString: conex);
            try
            {
                string Insert = @" Insert into dbo.Historical (creation, delivery, idNotificationType, idUsers, idDevice, idPlataform, shortText, longText, status) 
                               Values ( GETDATE(), GETDATE(), @idNotificationType, @idUsers, @idDevice, @idPlataform, @shortText, @longText, 0)";

                shortText = (SanitizeXmlString(shortText)).Trim();
                longText = (SanitizeXmlString(longText)).Trim();

                 conn = new SqlConnection(connectionString: conex);
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
                conn.Close();
            }
        }

        public static Status sendNotification(string rut, string idNotificationType, string shortText, string longText)
        {
            Status status = new Status();
            SqlConnection conn = new SqlConnection(connectionString: conex);
            rut = removeRut(rut);
            rut = agregateCero(rut);

            var push = new PushSharp.PushBroker();
            try
            {

            shortText = (SanitizeXmlString(shortText)).Trim();
            longText = (SanitizeXmlString(longText)).Trim();


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

                
                conn = new SqlConnection(connectionString: conex);
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

                string file = System.Configuration.ConfigurationManager.AppSettings["FILE"];
                string password = System.Configuration.ConfigurationManager.AppSettings["PASSWORD"];
                string appID = System.Configuration.ConfigurationManager.AppSettings["APPID"];
               
                push.StopAllServices(true);
                
                var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file));
                //push.RegisterAppleService(new ApplePushChannelSettings(false, appleCert, password));
                push.RegisterAppleService(new ApplePushChannelSettings(Convert.ToBoolean(ConfigurationManager.AppSettings["server"]), appleCert, password), new PushServiceSettings() { AutoScaleChannels = false, Channels = 1, MaxAutoScaleChannels = 1, MaxNotificationRequeues = 2, NotificationSendTimeout = 5000 });
                push.RegisterGcmService(new GcmPushChannelSettings(appID));

                          
                foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                {
                    try
                    {
                        string stext = base64ToText(shortText);
                        string ltext = base64ToText(longText);

                        stext = SanitizeXmlString(RemoveSpecialCharacters(stext));
                        ltext = SanitizeXmlString(RemoveSpecialCharacters(ltext));

                        if (_dr[1].ToString() == "2")
                        {
                           
                            push.QueueNotification(new AppleNotification()
                                                       .ForDeviceToken(_dr[0].ToString())
                                                       .WithAlert(stext.ToString())
                                                       .WithExpiry(DateTime.Now.AddHours(2))
                                                       .WithBadge(1)
                                                       .WithSound("sound.caf"));                          

                            Historical(idNotificationType, rut, _dr[0].ToString(), _dr[1].ToString(), stext, ltext);
                        }
                        else if (_dr[1].ToString() == "1")
                        {
                            push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(_dr[0].ToString())
                                                  .WithJson("{\"alert\":\""+ stext.ToString() +"\",\"badge\":1,\"sound\":\"sound.caf\"}"));


                            Historical(idNotificationType, rut, _dr[0].ToString(), _dr[1].ToString(), stext, ltext);
                        }

                        status.status = "Success";
                        status.description = "Send";

                       
                    }
                    catch (Exception ex)
                    {
                        status.status = "Error";
                        status.description = ex.Message;
                        push.StopAllServices(true);
                    }
                }
                push.StopAllServices(true);

            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = ex.Message;
                conn.Close();
                push.StopAllServices(true);
            }
            return status;
        }

//        void insertHistorical(string idNotificationType, string idUsers,string idDevice, string idPlataform,string shortText, string longText)
//        {
//            SqlConnection conn = new SqlConnection(connectionString: conex);
           
//            shortText = SanitizeXmlString(shortText);
//            longText = SanitizeXmlString(longText);

//            try
//            {
//                string Insert = @" Insert into dbo.Historical (creation, delivery, idNotificationType, idUsers, idDevice, idPlataform, shortText, longText, status) 
//                               Values ( GETDATE(), GETDATE(), @idNotificationType, @idUsers, @idDevice, @idPlataform, @shortText, @longText, 0)";


//                conn = new SqlConnection(connectionString: conex);
//                SqlCommand command = new SqlCommand(Insert, conn);
//                command.Parameters.AddWithValue("@idNotificationType", idNotificationType);
//                command.Parameters.AddWithValue("@idUsers", idUsers);
//                command.Parameters.AddWithValue("@idDevice", idDevice);
//                command.Parameters.AddWithValue("@idPlataform", idPlataform);
//                command.Parameters.AddWithValue("@shortText", shortText);
//                command.Parameters.AddWithValue("@longText", longText);
            
//                conn.Open();
//                command.ExecuteScalar();
//                conn.Close();
//            }
//            catch (Exception ex)
//            {
//                conn.Close();
//            }
//        }

        public static Status readExcel()
        {
            Status status = new Status();
            int x = 0;

            SqlConnection conn = new SqlConnection(connectionString: conex);
            SqlCommand command;
            int i = 0;
            try
            {
                string file = Path.Combine(ConfigurationManager.AppSettings["writeDirectory"], ConfigurationManager.AppSettings["inputFile"]).ToString();

                OleDbConnection con = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file +";Mode=ReadWrite;Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"");
                con.Open();
                DataSet dset = new DataSet();
                OleDbDataAdapter dadp = new OleDbDataAdapter("SELECT * FROM [Hoja1$]", con);
                dadp.TableMappings.Add("tbl", "Table");
                dadp.Fill(dset);
                DataTable table = dset.Tables[0];
                conn.Open();
                con.Close();
                for (i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][0].ToString() != "" && table.Rows[i][1].ToString() != "" && table.Rows[i][2].ToString() != "" && table.Rows[i][3].ToString() != "")
                    {
                        try
                        {
                            string user = removeRut(table.Rows[i][0].ToString());
                            user = agregateCero(user);
                            string type = table.Rows[i][1].ToString();
                            string shortTecxt = (SanitizeXmlString(table.Rows[i][2].ToString())).Trim();
                            string longText = (SanitizeXmlString(table.Rows[i][3].ToString())).Trim();

                            string selectDevice = @"SELECT D.idDevice, D.idPlataform 
                                                  FROM dbo.Device D, dbo.Users_Device UD, dbo.NotificationType_Users NTU
                                                  WHERE D.status = 1  AND D.idDevice = UD.idDevice AND NTU.status = 1 AND NTU.idNotificationType = @idNotificationType AND NTU.idUsers = UD.idUsers AND UD.idUsers = @idUsers ";

                            command = new SqlCommand(selectDevice, conn);
                            command.Parameters.AddWithValue("@idUsers", user);
                            command.Parameters.AddWithValue("@idNotificationType", type);

                            SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
                            DataSet dtDatos = new DataSet();
                            daAdaptador.Fill(dtDatos);

                            foreach (DataRow _dr in dtDatos.Tables[0].Rows)
                            {
                                try
                                {
                                    string selectNotificationtype = @"INSERT INTO [dbo].[Notification]
                                         ([idDevice]
                                         ,[token]
                                         ,[idNotificationType]
                                         ,[title]
                                         ,[text]
                                         ,[idPlataform]
                                         ,[creation]
                                         ,[idUsers]
                                         ,[firstName]
                                         ,[lastName]
                                         ,[shortText]
                                         ,[longText])
                                   VALUES
                                         (@idDevice               
                                         ,@token
                                         ,@idNotificationType  
                                         ,@title
                                         ,@text
                                         ,@idPlataform
                                         ,GETDATE()
                                         ,@idUsers
                                         ,@firstName
                                         ,@lastName
                                         ,@shortText
                                         ,@longText)";


                                    command = new SqlCommand(selectNotificationtype, conn);
                                    command.Parameters.AddWithValue("@idDevice", _dr[0].ToString());
                                    command.Parameters.AddWithValue("@token", "");
                                    command.Parameters.AddWithValue("@idNotificationType", type);
                                    command.Parameters.AddWithValue("@title", "");
                                    command.Parameters.AddWithValue("@text", "");
                                    command.Parameters.AddWithValue("@idPlataform", _dr[1].ToString());
                                    command.Parameters.AddWithValue("@idUsers", user);
                                    command.Parameters.AddWithValue("@firstName", "");
                                    command.Parameters.AddWithValue("@lastName", "");
                                    command.Parameters.AddWithValue("@shortText", shortTecxt);
                                    command.Parameters.AddWithValue("@longText", longText);
                                    command.ExecuteScalar();
                                    //x++;
                                    status.status = "Success";
                                    status.description = "Create file";


                                }
                                catch (Exception ex2)
                                {

                                    status.status = "Error";
                                    status.description = ex2.Message;
                                    con.Close();
                                    conn.Close();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            status.status = "Error";
                            status.description = ex.Message;
                            conn.Close();
                            con.Close();
                        }
                    }
                    else 
                    {

                        //status.status = "Error";
                        //status.description = "Raw document or empty";

                        //if (x > 2)
                        //{
                        //    conn.Close();
                        //    break;
                        //}
                        //x++;
                    }
                
               


                }

                if (table.Rows.Count == 0) 
                {
                    status.status = "Error";
                    status.description = "Raw document or empty";

                }
                conn.Close();

                //status.status = "Success";
                //status.description = "Enviados "+ x.ToString() +" Notificaciones";

            }
            catch (Exception ex)
            {
                status.status = "Error";
                status.description = x.ToString() +" "+ ex.Message;
                conn.Close();
            }
            return status;

        }

        public static Status writeExcel() 
        {
            Status status = new Status();

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

                        string user = removeRut(_dr[0].ToString());
                        user = agregateCero(user);

                        Users =  "<tr><td>"+ user + "</td>";
                        try
                        {
                            string Types = @"SELECT * FROM dbo.NotificationType_Users NTU, dbo.Users U WHERE NTU.idUsers = U.idUsers AND  U.idUsers = @rut";

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
                                        Users += "<td>"+1 + "</td>";
                                    }
                                    else
                                    {
                                        Users += "<td>"+ 0 + "</td>";
                                    }
                                }
                                Users += "</tr>";
                            }

                        }
                        catch (Exception ex)
                        {

                        }

                        User.Add(Users);

                    }
                    try
                    {
                        List<string> reads = new List<string>();

                        StreamWriter write;

                        write = new StreamWriter(Path.Combine(ConfigurationManager.AppSettings["writeDirectory"], ConfigurationManager.AppSettings["writeFile"]));


                        write.WriteLine("<html><table>");
                        for (i = 0; i <= User.Count - 1; i++)
                        {
                            write.WriteLine(User[i]);
                        }
                        write.WriteLine("</table></html>");

                        write.Close();


                        status.status = "Success";
                        status.description = "Create file";

                        //FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ConfigurationManager.AppSettings["ftpRoute"] + ConfigurationManager.AppSettings["ftpFile"]);
                        //request.Method = WebRequestMethods.Ftp.UploadFile;
                        //request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ftpUser"], ConfigurationManager.AppSettings["ftpPass"]);
                        //request.UsePassive = true;
                        //request.KeepAlive = true;

                        //FileStream stream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftpFile"]));

                        //byte[] buffer = new byte[stream.Length];
                        //stream.Read(buffer, 0, buffer.Length);
                        //stream.Close();

                        //Stream reqStream = request.GetRequestStream();
                        //reqStream.Write(buffer, 0, buffer.Length);
                        //reqStream.Flush();
                        //reqStream.Close();
                       


                    }
                    catch (Exception ex)
                    {
                        status.status = "Error";
                        status.description = ex.Message;
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

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, PushSharp.Core.INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
           // Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, PushSharp.Core.INotification notification)
        {
            //Console.WriteLine("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, PushSharp.Core.INotification notification, Exception notificationFailureException)
        {
           // Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        static void ChannelException(object sender, PushSharp.Core.IPushChannel channel, Exception exception)
        {
          //  Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
         //   Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, PushSharp.Core.INotification notification)
        {
          //  Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            //Console.WriteLine("Channel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {
           // Console.WriteLine("Channel Created for: " + sender);
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

        static string removeRut(string rut)
        {
            rut = rut.Replace(".", "");
            rut = rut.Replace("-", "");
            return rut;
        }

        static string agregateCero(string rut)
        {
            string ceros = "";
            if (rut.Length < 10)
            {
                for (int i = 0; i < 10 - rut.Length; i++)
                {
                    ceros += "0";
                }
                rut = ceros + rut;
               
            }
            return rut;
        }

        static string RemoveSpecialCharacters(string str)
        {
           return Regex.Replace(str, @"[^\w\.@-]", " ", RegexOptions.Compiled);
           //return str;
        }

        static string SanitizeXmlString(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            StringBuilder buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }
    }
}
