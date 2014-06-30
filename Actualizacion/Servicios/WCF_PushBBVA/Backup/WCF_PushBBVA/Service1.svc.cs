using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Data_PushBBVA;
using Connection_PushBBVA;

namespace WCF_PushBBVA
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        SQLConnection conx = new SQLConnection();

        public Login DoWork(String user, String pass)
        {
            Login login = new Login();
            login = conx.Login(this.QuitarCarac(user), pass);
            return login;
        }

        public String DoWork1()
        {           
            return "CUEK";
        }


        public String QuitarCarac(String rut)
        {
            rut = rut.Replace("-", "");
            rut = rut.Replace(".", "");
            return rut;
        }
     
    }
}
