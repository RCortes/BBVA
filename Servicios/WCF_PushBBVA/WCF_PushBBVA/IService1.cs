using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Data_PushBBVA;

namespace WCF_PushBBVA
{
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Login/{user},{pass}")]
        Status<Login> Login(string user, string pass);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "CreateUser/{rut},{firstName},{lastName},{idDevice},{idPlataform}")]
        Status Create(string rut, string firstName, string lastName, string idDevice, string idPlataform);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "DeactivateUser/{rut}")]
        Status Delete(string rut);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Notification/{rut},{idNotificationType},{Limit}")]
        Status<List<Notification>> Notification(string rut, string idNotificationType, string Limit);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "NotificationAll/{idNotificationType},{Limit}")]
        Status<List<Notification>> NotificationAll(string idNotificationType, string Limit);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Setting/{rut},{idNotificationType}")]
        Status Setting(string rut, string idNotificationType);

         [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "ListSetting/{rut}")]
        Status<List<NotificationType>> ListSetting(string rut);
        

         [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "sendNotification/{rut},{idNotificationType},{shortText},{longText}")]
        Status sendNotification(string rut, string idNotificationType, string shortText, string longText);


          [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "changeSchedule/{update},{idNotificationType}")]
         Status changeSchedule(string update, string idNotificationType);

        
          [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "readNotification/{idHistorical}")]
         Status readNotification(string idHistorical);

          [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "readExcel")]
          Status readExcel();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "writeExcel")]
          Status writeExcel();
        
        

       
    }
}
