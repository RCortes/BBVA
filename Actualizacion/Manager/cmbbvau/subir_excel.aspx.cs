using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Configuration;

public partial class subir_excel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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
    
    protected void uploadbt_Click(object sender, EventArgs e)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            string xurl = ConfigurationManager.ConnectionStrings["URL"].ConnectionString;
            string metodo = "readExcel";
            if (this.upload.PostedFile != null & this.upload.PostedFile.FileName != "")
            {
                if (System.IO.File.Exists(Server.MapPath("~\\" + ConfigurationManager.AppSettings["inputFile"])))
                {
                    System.IO.File.Delete(Server.MapPath("~\\" + ConfigurationManager.AppSettings["inputFile"]));
                }

                upload.PostedFile.SaveAs(Server.MapPath("~\\" + ConfigurationManager.AppSettings["inputFile"]));
                string URL = xurl + metodo;
                doc.Load(xurl + "readExcel");
                XmlNodeList Notification = doc.GetElementsByTagName("status");
                
                if (Notification[0].ChildNodes[0].InnerText == "Success")
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "update", "document.getElementById('up').style.display = 'run-in';document.getElementById('gif').style.display = 'block';document.getElementById('loginDos').style.display = 'block';setInterval(function () { document.getElementById('gif').style.display = 'none';document.getElementById('loginDos').style.display = 'none';document.getElementById('loginDos3').style.display = 'block';document.getElementById('up').style.display = 'block'; }, 5000);", true);
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "update", "document.getElementById('up').style.display = 'run-in';document.getElementById('gif').style.display = 'block';document.getElementById('loginDos').style.display = 'none';setInterval(function () { document.getElementById('gif').style.display = 'none';document.getElementById('loginDos').style.display = 'block'; }, 5000);", true);

                    //System.Threading.Thread.Sleep(2000);
               
                }
                else
                {
                   
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "update", "document.getElementById('up').style.display = 'block';document.getElementById('gif').style.display = 'block';setInterval(function () { document.getElementById('gif').style.display = 'block';document.getElementById('loginDos2').style.display = 'block';document.getElementById('up').style.display = 'block'; }, 2000);setInterval(function () {document.getElementById('loginDos2').style.display = 'block';document.getElementById('gif').style.display = 'none';}, 4000);", true);
                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Debe Seleccionar un Archivo');", true);
            }
        }catch(Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert","alert('"+ ex.Message+"')", true);          
        }
    }

}