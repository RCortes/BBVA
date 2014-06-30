using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Configuration;

public partial class configurar : System.Web.UI.Page
{
    int xcantidaId = 0;
    string[] xIds;
    protected void Page_Load(object sender, EventArgs e)
    {
        string xurl = ConfigurationManager.ConnectionStrings["URL"].ConnectionString;
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
        XmlDocument doc = new XmlDocument();
        XmlDocument DocResult = new XmlDocument();
        string metodo = "ListSetting/0";
        string URL = xurl + metodo;
        doc.Load(URL);
        XmlNode DocNode = DocResult.CreateXmlDeclaration("1.0", "utf-8", null);
        DocResult.AppendChild(DocNode);
        XmlNode DatNode = DocResult.CreateElement("Data");
        DocResult.AppendChild(DatNode);
        this.llenar.Controls.Clear();
        this.llenar.DataBind();

        XmlNodeList Notification = doc.GetElementsByTagName("NotificationType");
        xcantidaId = Notification.Count;
        xIds = new string[xcantidaId];
        if (Notification.Count != 0)
        {
            for (int i = 0; i < Notification.Count; i++)
            {
                xIds[i] = Notification[i].ChildNodes[1].InnerText;
                if (Notification[i].ChildNodes[1].InnerText != "3" && Notification[i].ChildNodes[1].InnerText != "9")
                {
                    Button btns = new Button();
                    DateTime xhora = new DateTime();
                    xhora = DateTime.Parse(Notification[i].ChildNodes[2].InnerText);
                    DropDownList droplist = new DropDownList();
                   
                    btns.Text = "Cambiar";
                    btns.ID = Notification[i].ChildNodes[1].InnerText;
                    btns.CssClass = "guardarbt2";
                    btns.Click += new EventHandler(btns_Click);


                    HtmlGenericControl Divcontenedor = new HtmlGenericControl("div");
                    Divcontenedor.Attributes.Add("class", "colConftext");
                    Divcontenedor.Attributes.Add("id", "notifica" + Notification[i].ChildNodes[1].InnerText);
                    this.llenar.Controls.Add(Divcontenedor);

                    HtmlGenericControl xli = new HtmlGenericControl("li");
                    xli.InnerText = Notification[i].ChildNodes[1].InnerText + ".- " + Notification[i].ChildNodes[4].InnerText;
                    DateTime hora = new DateTime(2014, 1, 1, 8, 0, 0);
                    //DateTime xhora = new DateTime();
                    xhora = DateTime.Parse(Notification[i].ChildNodes[2].InnerText);
                    droplist.ID = "droplis_" + Notification[i].ChildNodes[1].InnerText;
                    droplist.Items.Add(new ListItem(xhora.ToShortTimeString()));
                    while (true)
                    {
                        if (DateTime.Compare(hora, DateTime.Parse("2014-01-01 10:00:00")) < 0)
                        {
                            droplist.Items.Add(new ListItem("0" + hora.ToShortTimeString()));
                        }
                        else
                        {
                            droplist.Items.Add(new ListItem(hora.ToShortTimeString()));
                        }
                        hora = hora.AddMinutes(15);
                        if (DateTime.Compare(hora, DateTime.Parse("2014-01-01 23:15:00")) >= 0)
                        {
                            break;
                        }
                    }
                    HtmlGenericControl xli2 = new HtmlGenericControl("li");
                    xli2.Controls.Add(droplist);

                    HtmlGenericControl xli3 = new HtmlGenericControl("li");
                    xli3.Controls.Add(btns);

                    Divcontenedor.Controls.Add(xli);
                    Divcontenedor.Controls.Add(xli2);
                    Divcontenedor.Controls.Add(xli3);
                }
            }
            this.llenar.DataBind();
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Ocurrio un Problema o el servico no responde.');", true);
        }

        

    }

    public void cargar()
    {
        string xurl = ConfigurationManager.ConnectionStrings["URL"].ConnectionString;
        XmlDocument doc = new XmlDocument();
        XmlDocument DocResult = new XmlDocument();
        string metodo = "ListSetting/0";
        string URL = xurl + metodo;
        doc.Load(URL);
        XmlNode DocNode = DocResult.CreateXmlDeclaration("1.0", "utf-8", null);
        DocResult.AppendChild(DocNode);
        XmlNode DatNode = DocResult.CreateElement("Data");
        DocResult.AppendChild(DatNode);
        this.llenar.Controls.Clear();
        this.llenar.DataBind();

        XmlNodeList Notification = doc.GetElementsByTagName("NotificationType");
        xcantidaId = Notification.Count;
        xIds = new string[xcantidaId];
        if (Notification.Count != 0)
        {
            for (int i = 0; i < Notification.Count; i++)
            {
                xIds[i] = Notification[i].ChildNodes[1].InnerText;
                if (Notification[i].ChildNodes[1].InnerText != "3" && Notification[i].ChildNodes[1].InnerText != "9")
                {
                    Button btns = new Button();
                    DateTime xhora = new DateTime();
                    xhora = DateTime.Parse(Notification[i].ChildNodes[2].InnerText);
                    DropDownList droplist = new DropDownList();
                   
                    btns.Text = "Cambiar";
                    btns.ID = Notification[i].ChildNodes[1].InnerText;
                    btns.CssClass = "guardarbt2";
                    btns.Click += new EventHandler(btns_Click);


                    HtmlGenericControl Divcontenedor = new HtmlGenericControl("div");
                    Divcontenedor.Attributes.Add("class", "colConftext");
                    Divcontenedor.Attributes.Add("id", "notifica" + Notification[i].ChildNodes[1].InnerText);
                    this.llenar.Controls.Add(Divcontenedor);

                    HtmlGenericControl xli = new HtmlGenericControl("li");
                    xli.InnerText = Notification[i].ChildNodes[1].InnerText + ".- " + Notification[i].ChildNodes[4].InnerText;
                    DateTime hora = new DateTime(2014, 1, 1, 8, 0, 0);
                    //DateTime xhora = new DateTime();
                    xhora = DateTime.Parse(Notification[i].ChildNodes[2].InnerText);
                    droplist.ID = "droplis_" + Notification[i].ChildNodes[1].InnerText;
                    droplist.Items.Add(new ListItem(xhora.ToShortTimeString()));
                    while (true)
                    {
                        if (DateTime.Compare(hora, DateTime.Parse("2014-01-01 10:00:00")) < 0)
                        {
                            droplist.Items.Add(new ListItem("0" + hora.ToShortTimeString()));
                        }
                        else
                        {
                            droplist.Items.Add(new ListItem(hora.ToShortTimeString()));
                        }
                        hora = hora.AddMinutes(15);
                        if (DateTime.Compare(hora, DateTime.Parse("2014-01-01 23:15:00")) >= 0)
                        {
                            break;
                        }
                    }
                    HtmlGenericControl xli2 = new HtmlGenericControl("li");
                    xli2.Controls.Add(droplist);

                    HtmlGenericControl xli3 = new HtmlGenericControl("li");
                    xli3.Controls.Add(btns);

                    Divcontenedor.Controls.Add(xli);
                    Divcontenedor.Controls.Add(xli2);
                    Divcontenedor.Controls.Add(xli3);
                }
            }
            this.llenar.DataBind();
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Ocurrio un Problema o el servico no responde.');", true);
        }
    }

    private void btns_Click(object sender, EventArgs e)
    {
        string xurl = ConfigurationManager.ConnectionStrings["URL"].ConnectionString;
        string metodo = "changeSchedule/";
        int auxiliar = 1;
        Button btn = (Button)sender;
         String id = btn.ID;
        //String id = btn.ID.Split('*')[0].ToString();
        //String xhora_devuelta = btn.ID.Split('*')[1].ToString();
        DropDownList drop = (DropDownList)this.Page.FindControl("droplis_" + id.ToString());
        DateTime xdate = new DateTime();
        xdate = DateTime.Now;
        string xhora = drop.SelectedValue;
        xdate = Convert.ToDateTime(xhora);

        XmlDocument doc = new XmlDocument();
        //if (xhora_devuelta != xhora)
        //{
        for (int i = 0; i < xcantidaId - 1; i++)
        {
            if (id != xIds[i].ToString())
            {
                if (xIds[i].ToString() != "3")
                {
                    DropDownList xdrop_general = (DropDownList)this.Page.FindControl("droplis_" + xIds[i].ToString());
                    if (xhora == xdrop_general.SelectedValue)
                    {
                        auxiliar = 0;
                    }
                }
            }
        }
        if (auxiliar != 0)
        {
            string URL = xurl + metodo + textToBase64(xdate.ToString()) + "," + id;
            doc.Load(URL);
            XmlNodeList Notification = doc.GetElementsByTagName("status");
            if (Notification[0].ChildNodes[0].InnerText == "Success")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Cambios Realizados.');", true);

            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Ocurrio un Problema al Actualizar.');", true);
            }
        }
        //}
        else
        {
            this.cargar();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Debe Modificar el horario para actualizar.');", true);
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
}
