<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="N2.Edit.Login" Title="Login" meta:resourceKey="LoginPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Log in</title>
	<asp:PlaceHolder runat="server">
		<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.JQueryJsPath)  %>" type="text/javascript"></script>
		<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapCssPath) %>" type="text/css" rel="stylesheet" />
		<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapJsPath)  %>" type="text/javascript"></script>
	</asp:PlaceHolder>
    
	<link rel="stylesheet" href="Resources/Css/All.css" type="text/css" runat="server" />
	<style>
		body { background:#fff url(Resources/Img/logo.png) no-repeat 98% 10px; }
		.container { width:400px; position:absolute; top:50%; left:50%; margin-top:-100px; margin-left:-200px; }
		label { margin-right:10px; }
	</style>
</head>
<body class="edit login">
	<form id="form1" runat="server">
		<div>
		</div>
		<div class="container">
			<asp:Login ID="Login1" TitleText="<h1>Log in</h1>" runat="server" meta:resourceKey="Login1" CssClass="login"
				MembershipProvider="AspNetSqlMembershipProvider" OnAuthenticate="Login1_Authenticate" >
				<LoginButtonStyle CssClass="btn btn-primary" />
				<LabelStyle CssClass="login-label" />
				<TextBoxStyle CssClass="login-input" />
			</asp:Login>
		</div>
		<script type="text/javascript">
			try {
				if (window.top.location.pathname !== window.location.pathname) {
					window.top.location = window.location;
				}
			} catch (e) {
				window.top.location = window.location;
			}
		</script>
	</form>
</body>
</html>
