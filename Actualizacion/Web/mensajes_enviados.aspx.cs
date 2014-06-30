using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using System.Net;
using System.Configuration;


public partial class mensajes_enviados : System.Web.UI.Page
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
            this.LoadGrid();
        }
    }

    private void LoadGrid()
    {
        XmlDocument doc = new XmlDocument();
        XmlDocument DocResult = new XmlDocument();
        string xurl = ConfigurationManager.ConnectionStrings["URL"].ConnectionString;
        string metodo = "NotificationAll/0,1000";
        string URL = xurl + metodo;
        doc.Load(URL);
        string xmlmano = "";
        XmlNode DocNode = DocResult.CreateXmlDeclaration("1.0", "utf-8", null);
        DocResult.AppendChild(DocNode);
        XmlNode DatNode = DocResult.CreateElement("Data");
        DocResult.AppendChild(DatNode);
        xmlmano += "<UsersDetalles>";

        XmlNodeList Notification = doc.GetElementsByTagName("Notification");

        if (Notification.Count == 0)
        {
            xmlmano += "<Users>";
            xmlmano += "<Mensaje>" + "No existen notificaciones disponibles." + "</Mensaje>";
            xmlmano += "</Users>";
            for (int i = 0; i < 4; i++)
            {
                    xmlmano += "<Users>";
                    xmlmano += "<Mensaje>" +" "+ "</Mensaje>";
                    xmlmano += "</Users>";            
            }
        }
        else
        {

            for (int i = 0; i < Notification.Count; i++)
            {
                xmlmano += "<Users>";
                xmlmano += "<Mensaje>" + Notification[i].ChildNodes[8].InnerText + "</Mensaje>";
                xmlmano += "<Fecha>" + Convert.ToDateTime(Notification[i].ChildNodes[1].InnerText).ToString("dd-MM-yyyy  HH:mm:ss") + "</Fecha>";
                xmlmano += "<Tipo>" + Notification[i].ChildNodes[9].InnerText + "</Tipo>";
                xmlmano += "<Nombre>" + Notification[i].ChildNodes[6].InnerText + "</Nombre>";
                xmlmano += "</Users>";
            }
        }

        xmlmano += "</UsersDetalles>";
        System.IO.StringReader xmlSR = new System.IO.StringReader(xmlmano);
        DataSet dt = new DataSet();
        DataTable datatable = new DataTable("Users");
        datatable.Columns.Add("Mensaje", typeof(string));
        datatable.Columns.Add("Fecha", typeof(string));
        datatable.Columns.Add("Tipo", typeof(string));
        datatable.Columns.Add("Nombre", typeof(string));
        dt.Tables.Add(datatable);
        dt.ReadXml(xmlSR, XmlReadMode.IgnoreSchema);
        GridView1.DataSource = dt;
        GridView1.DataBind();
    }


    private void CargarGrid(string rut)
    {
        XmlDocument doc2 = new XmlDocument();
        XmlDocument DocResult = new XmlDocument();

        string xurl = ConfigurationManager.ConnectionStrings["URL"].ConnectionString;
        string metodo = "Notification/" + rut + ",0,30";
        string URL = xurl + metodo;
        doc2.Load(URL);
        string xmlmano = "";
        XmlNode DocNode = DocResult.CreateXmlDeclaration("1.0", "utf-8", null);
        DocResult.AppendChild(DocNode);
        XmlNode DatNode = DocResult.CreateElement("Data");
        DocResult.AppendChild(DatNode);
        xmlmano += "<UsersDetalles>";

        XmlNodeList Notification = doc2.GetElementsByTagName("Notification");
        if (Notification.Count == 0)
        {
            xmlmano += "<Users>";
            xmlmano += "<Mensaje>" + "No existen notificaciones disponibles para este usuario." + "</Mensaje>";
            xmlmano += "</Users>";
            for (int i = 0; i < 4; i++)
            {
                xmlmano += "<Users>";
                xmlmano += "<Mensaje>" + " " + "</Mensaje>";
                xmlmano += "</Users>";
            }
        }
        else
        {
            for (int i = 0; i < Notification.Count; i++)
            {
                xmlmano += "<Users>";
                xmlmano += "<Mensaje>" + Notification[i].ChildNodes[8].InnerText + "</Mensaje>";
                xmlmano += "<Fecha>" + Convert.ToDateTime(Notification[i].ChildNodes[1].InnerText).ToString("dd-MM-yyyy  HH:mm:ss") + "</Fecha>";
                xmlmano += "<Tipo>" + Notification[i].ChildNodes[9].InnerText + "</Tipo>";
                xmlmano += "<Nombre>" + Notification[i].ChildNodes[6].InnerText + "</Nombre>";
                xmlmano += "</Users>";
                XmlNode ListNode = DocResult.CreateElement("Listitems");
                DatNode.AppendChild(ListNode);

                XmlNode xMensaje = DocResult.CreateElement("xI");
                xMensaje.InnerText = Notification[i].ChildNodes[8].InnerText;
                ListNode.AppendChild(xMensaje);

                XmlNode xFecha = DocResult.CreateElement("Fecha");
                xFecha.InnerText = Notification[i].ChildNodes[1].InnerText;
                ListNode.AppendChild(xFecha);

                XmlNode xCantidad = DocResult.CreateElement("Cantidad");
                xCantidad.InnerText = Notification[i].ChildNodes[10].InnerText;
                ListNode.AppendChild(xCantidad);

                XmlNode xName = DocResult.CreateElement("Name");
                xName.InnerText = Notification[i].ChildNodes[4].InnerText;
                ListNode.AppendChild(xName);
            }
        }
        xmlmano += "</UsersDetalles>";
        System.IO.StringReader xmlSR = new System.IO.StringReader(xmlmano);
        DataSet dt = new DataSet();
        DataTable datatable = new DataTable("Users");
        datatable.Columns.Add("Mensaje", typeof(string));
        datatable.Columns.Add("Fecha", typeof(string));
        datatable.Columns.Add("Tipo", typeof(string));
        datatable.Columns.Add("Nombre", typeof(string));
        dt.Tables.Add(datatable);
        dt.ReadXml(xmlSR, XmlReadMode.IgnoreSchema);
        GridView1.DataSource = dt;
        GridView1.DataBind();
    }
    protected void btnaceptar_Click(object sender, EventArgs e)
    {
        string rut = "";
        if (this.buscar.Text != "")
        {
            rut = this.buscar.Text;
            this.CargarGrid(removeRut(rut));
        }
    }

    static string removeRut(string rut)
    {
        rut = rut.Replace(".", "");
        rut = rut.Replace("-", "");
        return rut;
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (this.buscar.Text != "" && this.buscar.Text != "Ingresar RUT")
        {
            string rut = "";
            GridView1.PageIndex = e.NewPageIndex;
            rut = this.buscar.Text;
            this.CargarGrid(removeRut(rut));
        }
        else
        {
            GridView1.PageIndex = e.NewPageIndex;
            this.LoadGrid();
        }

    }

}