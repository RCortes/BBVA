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

        ////public Status holdOver(string idHistorical, string expirateDate)
        ////{
        ////    Status status = SQLConnection.holdOver(idHistorical, expirateDate);
        ////    return status;
        ////}


      
    }
}
