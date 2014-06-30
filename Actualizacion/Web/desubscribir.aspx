<%@ Page Language="C#" AutoEventWireup="true" CodeFile="desubscribir.aspx.cs" Inherits="desubscribir" EnableEventValidation="False" %>

<!DOCTYPE html>

<head>

    <link rel="stylesheet" type="text/css" href="style.css" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <script type="text/javascript" language="javascript">
        function Error() {
            alert("ha ocurrido un error.");
        }

        function EliminarTexto() {
            document.getElementById("xrut").value = ""
            document.getElementById("xrut").focus();
        }

        function mostrar() {
            document.getElementById("loginDos").style.display = "block";
        }

        function OnlyNumericEntry() {
            if ((event.KeyCode < 44 || event.KeyCode > 44) && (event.KeyCode < 48 || event.KeyCode > 57)) {
                event.returnValue = false;
            }
            else {
                event.returnValue = true;
            }
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
                <asp:Label ID="user" runat="server" Text=""></asp:Label>
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
        <form id="Form1" runat="server" class="ingresar">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <section class="loginUno">
                <h1>Desubscribir mensajes</h1>
                <div class="franja"> </div>
                <div class="ingresar"> 
                <asp:TextBox ID="xrut" runat="server" class="textbox" Value="Ingresar RUT" onclick="EliminarTexto()" onkeypress="OnlyNumericEntry()"></asp:TextBox>
                <asp:Button ID="Cargar" runat="server" Text="Desubscribir" class="ingbt"
                    OnClick="Cargar_Click"></asp:Button>
                    </div>
            </section>
            <section id="loginDos" style="display: none;">
                <p>El RUT <b>
                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label></b> ha dejado de recibir mensajes a su equipo celular.</p>
            </section>
        </form>
    </section>

    <footer class="footer">
        <p>Todos los derechos reservados &#8211; <a href="http://www.movidreams.com">movidreams.com</a></p>
    </footer>
</body>
</html>
