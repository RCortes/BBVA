using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Configuration;

public partial class index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session.RemoveAll();
        }
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

    protected void btnIngresar_Click(object sender, EventArgs e)
    {
        XmlDocument doc = new XmlDocument();
        string xurl = ConfigurationManager.ConnectionStrings["URL"].ConnectionString;
        string metodo = "Login/";
        if ((this.Nickname.Text != "" & this.Password.Text != "") && (this.Nickname.Text != "Nombre Usuario" & this.Password.Text != "Contraseña"))
        {
            string rut = removeRut(this.Nickname.Text);
            string pass = this.Password.Text;
            string passEncode = textToBase64(pass);
            string URL = xurl + metodo + rut + "," + passEncode;
            doc.Load(URL);

            XmlNodeList estado = doc.GetElementsByTagName("status");
            XmlNodeList name = doc.GetElementsByTagName("name");
            XmlNodeList user = doc.GetElementsByTagName("user");

            if (estado[0].ChildNodes[0].InnerText == "Success")
            {
                Session["login"] = true;
                Session["User"] = user[0].ChildNodes[0].InnerText;
                Session["name"] = name[0].ChildNodes[0].InnerText;
                Response.Redirect("mensajes_enviados.aspx");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Usuario no valido.');", true);
            }
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Ingrese datos validos.');", true);
        }
    }
}