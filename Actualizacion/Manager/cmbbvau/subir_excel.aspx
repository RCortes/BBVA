<%@ Page Language="C#" AutoEventWireup="true" CodeFile="subir_excel.aspx.cs" Inherits="subir_excel" %>

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
            <h1>Subir Excel</h1>
            <div class="franja"></div>
            <form runat="server">
            <div class="ingresar">
                <div id="up">
                    <div class="upload">
                        <asp:FileUpload ID="upload" runat="server"></asp:FileUpload>
                    </div>
                    <asp:Button ID="uploadbt" runat="server" Text="Subir Archivo"
                        OnClick="uploadbt_Click"></asp:Button>
                    <%--<a href="#">Descargar Excel de ejemplo</a>--%>
                </div>
                <div id="gif" style="display: none;"></div>
            </div>
            
            <section id="loginDos" style="display: none;">
                <ul>
                    <li style="color: #44A718;">Subiendo Excel</li>
                </ul>
            </section>
            <section id="loginDos2" style="display: none;">
                <ul>
                    <li style="color: #ff0000;">No Cargo el Archivo..</li>
                </ul>
            </section>
            <section id="loginDos3" style="display: none;">
                <ul>
                    <li style="color: #114b90;">Las notificaciones serán enviadas en los próximos minutos ...</li>
                    <%--<li>Las notificaciones serán enviadas en los próximos minutos</li>--%>
                </ul>
            </section>
            </form>
        </section>
        <script>
            function uploadShow() {
                document.getElementById('up').style.display = "initial";
                document.getElementById('loginDos').style.display = "initial";
                document.getElementById('gif').style.display = "initial";
            }
        </script>
    </section>

    <footer class="footer">
        <p>Todos los derechos reservados &#8211; <a href="http://www.movidreams.com">movidreams.com</a></p>
    </footer>
</body>

</html>