<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>



<!DOCTYPE html>

	<head>

		<link rel="stylesheet" type="text/css" href="style.css">
		<meta http-equiv="X-UA-Compatible" content="IE=edge"> 

	</head>

	<body>

		<header>	<!-- Header -->

			<article class="head">
				
				<section class="titulo">
					<h2>Administrador</h2>
					<h1>Notificaciones PUSH</h1>
				</section>

				<!-- [Aparecerá después de hacer login]
				
				<section class="login">
					Bienvenido, nombreUsuario <a href="#">Salir</a>
				</section>
				
				-->

			</article>

		</header>

		<section class="content" style="padding-bottom:50px;"><!-- Content -->

			<section class="loginUno">

				<h1>Bienvenido</h1>
				
				<div class="franja"></div>

				<form class="ingresar">
					<input type"text" class="textbox" value="Nombre Usuario">
					<input type"text" class="textbox" value="Contrase&ntilde;a">
					<form><input type="button" class="ingbt" onClick="window.location.href = 'mensajes_enviados.html'" value="Ingresar"></form> <!-- El HTML que sale en windows.location.href es fijo y se debe cambiar cuando se haga la versión final. -->
				</form>

			</section>

		</section>

		<footer class="footer"><!-- Footer -->

			<p>Todos los derechos reservados &#8211; <a href="http://www.movidreams.com">movidreams.com</a></p>

		</footer>

	</body>	


</html>




<%--<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>--%>
