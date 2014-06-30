using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Configuration;
using System.Xml;

public partial class informes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
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
            
            doc.Load(xurl + "writeExcel");
            XmlNodeList Notification = doc.GetElementsByTagName("status");

            if (Notification[0].ChildNodes[0].InnerText == "Success")
            {
                if (System.IO.File.Exists(Server.MapPath("~\\" + ConfigurationManager.AppSettings["writeFile"])))
                {
                    Response.Clear();
                    Response.ContentType = "application/excel";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + ConfigurationManager.AppSettings["writeFile"]);
                    Response.WriteFile("~\\" + ConfigurationManager.AppSettings["writeFile"]);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('El Archivo no Existe.');", true);
                }
            }
            else 
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('"+ Notification[0].ChildNodes[1].InnerText +"');", true);
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message.ToString() +"')", true);
            
        }
    }
}