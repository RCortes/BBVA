<%@ Page Language="C#" AutoEventWireup="true" CodeFile="configurar.aspx.cs" Inherits="configurar" %>

<!DOCTYPE html>

<head>

    <link rel="stylesheet" type="text/css" href="style.css">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">

    <style>
        .guardarbt2
        {
            width: 100px;
            height: 30px;
            background-image: url('images/btnbckg.jpg');
            background-repeat: repeat-x;
            background-size: contain;
            border: none;
            -webkit-border-radius: 5px;
            -moz-border-radius: 5px;
            border-radius: 5px;
            font-family: HNRegular;
            color: #fff;
            font-size: 18px;
            text-align: center;
        }
    </style>
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

    <section class="content">
        <section class="bloque">
            <h1>Configurar mensajes</h1>
            <div class="resultados">
                <div class="colConf">
                    <li>Tipo</li>
                    <li>Hora de inicio</li>
                    <li>Confirmar</li>
                </div>
                <form runat="server">
                    <div id="llenar" runat="server">
                        <input type="button" class="guardarbt" value="Guardar">
                    </div>
                </form>
            </div>
        </section>
    </section>

    <footer class="footer">
        <p>Todos los derechos reservados &#8211; <a href="http://www.movidreams.com">movidreams.com</a></p>
    </footer>
</body>

</html>