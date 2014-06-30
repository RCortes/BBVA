<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mensajes_enviados.aspx.cs" Inherits="mensajes_enviados" %>

<!DOCTYPE html>

<head>
    <link href="style.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <script type="text/javascript" language="javascript">
        function OnlyNumericEntry() {
            if ((event.KeyCode < 44 || event.KeyCode > 44) && (event.KeyCode < 48 || event.KeyCode > 57)) {
                event.returnValue = false;
            }
            else {
                event.returnValue = true;
            }
        }
        function EliminarTexto() {
            document.getElementById("buscar").value = ""
            document.getElementById("buscar").focus();
        }
    </script>
</head>

<body>

    <header>
        <article class="head">
            <section class="titulo">
                <h2>Administrador</h2>
                <h1>Notificaciones PUSH</h1>
            </section>
            <section class="login">
                Bienvenido,
                <asp:Label ID="user" runat="server" Text="Label"></asp:Label>
                <a href="index.aspx">Salir</a>
            </section>
        </article>

        <article class="menu">
            <section class="btmenu">
                <a href="mensajes_enviados.aspx">
                    <div data-icon="b"></div>
                    Mensajes enviados</a>
            </section>
            <section class="btmenu">
                <a href="desubscribir.aspx">
                    <div data-icon="c"></div>
                    Desuscribir mensajes</a>
            </section>
            <section class="btmenu">
                <a href="subir_excel.aspx">
                    <div data-icon="a"></div>
                    Subir Excel</a>
            </section>
            <section class="btmenu">
                <a href="informes.aspx">
                    <div data-icon="d"></div>
                    Descargar informes</a>
            </section>
            <section class="btmenu">
                <a href="configurar.aspx">
                    <div data-icon="e"></div>
                    Configurar mensajes</a>
            </section>
        </article>
    </header>

    <section class="content">
        <section class="bloque">
            <h1>Mensajes enviados</h1>
            <form runat="server">
                <div class="buscar">
                    <asp:TextBox ID="buscar" runat="server" class="searchtxt" value="Ingresar RUT" onkeypress="OnlyNumericEntry()" onclick="javascript:EliminarTexto();"></asp:TextBox>
                    <asp:Button ID="btnaceptar" runat="server" Text="Buscar" class="searchbt" OnClick="btnaceptar_Click"></asp:Button>
                </div>
                <div class="resultados">
                    <div class="tabla">
                        <asp:GridView ID="GridView1" runat="server" OnPageIndexChanging="GridView1_PageIndexChanging" AllowPaging="True" CellPadding="3" CssClass="tabla_datos" PageSize="5">
                            <HeaderStyle CssClass="col" />
                            <PagerStyle CssClass="pagination" />
                            <RowStyle CssClass="coltext" />
                        </asp:GridView>
                    </div>
                </div>
            </form>
        </section>
    </section>

    <footer class="footer">
        <p>Todos los derechos reservados &#8211; <a href="http://www.movidreams.com">movidreams.com</a></p>
    </footer>
</body>

</html>
