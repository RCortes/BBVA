<%@ Page Language="C#" AutoEventWireup="true" CodeFile="informes.aspx.cs" Inherits="informes" %>

<!DOCTYPE html>

<head>
    <link rel="stylesheet" type="text/css" href="style.css">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
</head>

<body>
    <header>

        <article class="head">
            <section class="titulo">
                <h2>Administrador</h2>
                <h1>Notificaciones PUSH</h1>
            </section>
            <section class="login">
                Bienvenido,<asp:Label ID="user" runat="server" Text="Label"></asp:Label>
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
                    Descargar Informes</a>
            </section>
            <section class="btmenu">
                <a href="configurar.aspx">
                    <div data-icon="e"></div>
                    Configurar mensajes</a>
            </section>
        </article>
    </header>

    <section class="content" style="padding-bottom: 50px;">
        <section class="loginUno">
            <h1>Descargar Informes</h1>
            <div class="franja"></div>
            <form class="ingresar" runat="server">
                <div id="up">
                    <asp:Button ID="uploadbt" runat="server" Text="Descargar clientes enrolados"
                        OnClick="uploadbt_Click"></asp:Button>
                </div>
            </form>
        </section>
    </section>

    <footer class="footer">
        <p>Todos los derechos reservados &#8211; <a href="http://www.movidreams.com">movidreams.com</a></p>
    </footer>
</body>

</html>
