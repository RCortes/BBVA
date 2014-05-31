using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Connection_PushBBVA;
using Data_PushBBVA;

namespace WCF_PushBBVA
{
    
    public class Service1 : IService1
    {

        public Status<Login> Login(String user, String pass)
        {
            Status<Login> resultado = SQLConnection.Login(user, pass);
            return resultado;
        }

        public Status Create(string rut, string firstName, string lastName, string idDevice, string idPlataform)
        {
            Status status = SQLConnection.CreateUser(rut, firstName, lastName, idDevice, idPlataform);
            return status;
        }

        public Status Delete(string rut)
        {
            Status status = SQLConnection.DeleteUser(rut);
            return status;
        }

        public Status<List<Notification>> Notification(string rut, string idNotificationType, string Limit)
        {
            Status<List<Notification>> status = SQLConnection.Notification(rut, idNotificationType, Limit);
            return status;
        }

        public Status<List<Notification>> NotificationAll(string idNotificationType, string Limit)
        {
            Status<List<Notification>> status = SQLConnection.NotificationAll(idNotificationType, Limit);
            return status;
        }

        public Status Setting(string rut, string idNotificationType)
        {
            Status status = SQLConnection.Setting(rut,idNotificationType);
            return status;
        }

        public Status<List<NotificationType>> ListSetting(string rut)
        {
            Status<List<NotificationType>> status = SQLConnection.ListSetting(rut);
            return status;
        }

        public Status sendNotification(string rut, string idNotificationType, string shortText, string longText) 
        {
            Status status = SQLConnection.sendNotification(rut, idNotificationType, shortText, longText);
            return status;
        }


        public Status changeSchedule(string update, string idNotificationType)
        {
            Status status = SQLConnection.changeSchedule(update, idNotificationType);
            return status;
        }

        public Status readNotification(string idHistorical)
        {
            Status status = SQLConnection.readNotification(idHistorical);
            return status;
        }

        public Status readExcel()
        {
            Status status = SQLConnection.readExcel();
            return status;
        }
        
        public Status writeExcel()
        {
            Status status = SQLConnection.writeExcel();
            return status;
        }




        ////////// ANTIGUOS //////////////////////////////////////



        ////SQLConnection conx = new SQLConnection();

        //public bool DoWork(String user, String pass)
        //{
        //    bool resultado = SQLConnection.Login(user, pass);
        //    return resultado;
        //}

        //public Login DoWork2(String user, String pass)
        //{
        //    Login login = new Login();
        //    login = SQLConnection.Login2(user, pass);
        //    return login;
        //}

        //public List<NotificationType> DoWork3()
        //{
        //    List<NotificationType> Type = SQLConnection.NotificationType();
        //    return Type;
        //}

        //public string DoWork4(String user, String idDevice, String plataform) 
        //{
        //    String Create = SQLConnection.createUser(user, idDevice, plataform);
        //    return Create;
        //}

        //public List<Notification> DoWork5(String rut, String type) 
        //{ 
        //    List<Notification> Lista = SQLConnection.List(rut, type);
        //    return Lista;

        //}

        //public List<Notification> DoWork6(String type)
        //{
        //    List<Notification> Lista = SQLConnection.ListNotification(type);
        //    return Lista;

        //}

        //public string DoWork7(String idDevice, String token)
        //{
        //    string result = SQLConnection.UpdateDevice(idDevice, token);
        //    return result;
        //}

        //public List<NotificationUser> DoWork8(String rut)
        //{
        //    List<NotificationUser> Lista = SQLConnection.ListSetting(rut);
        //    return Lista;

        //}

        //public String QuitarCarac(String rut)
        //{
        //    rut = rut.Replace("-", "");
        //    rut = rut.Replace(".", "");
        //    return rut;
        //}
     
    }
}
