<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<!DOCTYPE html>

<head>
    <link rel="stylesheet" type="text/css" href="style.css">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <script type="text/javascript" language="javascript">
        function EliminarTexto() {
            document.getElementById("Nickname").value = ""
            document.getElementById("Nickname").focus();
        }
        function EliminarTexto2() {
            document.getElementById("Password").value = ""
            document.getElementById("Password").focus();
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
        </article>
    </header>

    <section class="content" style="padding-bottom: 50px;">
        <section class="loginUno">
            <h1>Bienvenido</h1>
            <div class="franja"></div>
            <form runat="server">
                <div class="ingresar">
                    <asp:TextBox ID="Nickname" runat="server" class="textbox" Value="Nombre Usuario" onclick="javascript:EliminarTexto();" Style="margin: 25px 0 20px 0px !important;"></asp:TextBox>
                    <asp:TextBox ID="Password" runat="server" class="textbox" value="Contraseña"
                        onclick="javascript:EliminarTexto2();" TextMode="Password" Style="margin: 25px 0 20px 0px !important;"></asp:TextBox>
                    <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" class="ingbt"
                        OnClick="btnIngresar_Click" Style="margin: 0px 0 20px 0px !important;"></asp:Button>
                </div>
            </form>
        </section>
    </section>

    <footer class="footer">
        <p>Todos los derechos reservados &#8211; <a href="http://www.movidreams.com">movidreams.com</a></p>
    </footer>

</body>

</html>