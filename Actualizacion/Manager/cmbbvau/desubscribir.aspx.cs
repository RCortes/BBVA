using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Configuration;

public partial class desubscribir : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Form.Attributes.Add("enctype","multipart/form-data");
        if (!IsPostBack)
        {
            if (Session["login"] != null && Session["User"].ToString() != "")
            {
                string resultados = Session["login"].ToString();
                if (resultados != "True")
                {
                    Response.Redirect("index.aspx");
                }

            }
            else
            {
                Response.Redirect("index.aspx");
            }
            this.user.Text = Session["name"].ToString();
        }
    }

    static string removeRut(string rut)
    {

        rut = rut.Replace(".", "");

        rut = rut.Replace("-", "");

        return rut;

    }
    protected void Cargar_Click(object sender, EventArgs e)
    {
        XmlDocument doc = new XmlDocument();
        string xurl = ConfigurationManager.ConnectionStrings["URL"].ConnectionString;
        if (this.xrut.Text != "" && this.xrut.Text != "Ingresar RUT")
        {
            string metodo = "DeactivateUser/";
            string URL = xurl + metodo + removeRut(this.xrut.Text);
            doc.Load(URL);
            XmlNodeList estado = doc.GetElementsByTagName("status");
            if (estado[0].ChildNodes[0].InnerText == "Success")
            {
                this.Label1.Text = this.xrut.Text;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "acepta", "document.getElementById('loginDos').style.display = 'block';setInterval(function () { document.getElementById('loginDos').style.display = 'none'; }, 5000);", true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('No se pudo desactivar el usuario');", true);
            }

        }
        else
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Ingrese Usuario Valido');", true);
        }
    }
}