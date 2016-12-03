<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Install.Begin.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Install N2</title>
	<asp:PlaceHolder runat="server">
		<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapCssPath) %>" type="text/css" rel="stylesheet" />
		<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapJsPath)  %>" type="text/javascript"></script>
	</asp:PlaceHolder>
	<link rel="stylesheet" type="text/css" href="../../Resources/Css/all.css" />
	<link rel="stylesheet" type="text/css" href="../../Resources/Css/framed.css" />
	<link rel="stylesheet" type="text/css" href="../../Resources/Css/themes/default.css" />
	<style type="text/css">
		form{font-size:1.1em;width:800px;margin:10px auto;}
		a{color:#00e;}
		li{margin-bottom:10px}
		form{padding:20px}
		.warning{color:#f00;}
		.ok{color:#0d0;}
		textarea{width:80%;height:120px}
		label { min-width:120px; display:inline-block; }
	</style>
</head>
<body>
<form id="form1" runat="server">
<div>
	<ul class="nav nav-tabs">
		<li class="active"><a href="#">0. Prepare yourself</a></li>
		<li><a href="<%= continueUrl %>">1-3. Continue installation</a></li>
	</ul>
	<div class="tab-content">
		<asp:CustomValidator ID="cvSave" runat="server" CssClass="alert alert-error" Display="Dynamic" />
		<pre runat="server" visible="false" id="preManualConfig"></pre>

		<% if (!installationAllowed) { %>
		<%= N2.Management.Installation.InstallationUtility.InstallationUnallowedHtml %>
	<% } else if (needsPasswordChange && !autoLogin) { %>
		<h1>Welcome to <a href="http://n2cms.com/">N2 CMS</a></h1>
		<p>
			<asp:CheckBox OnCheckedChanged="chkLoginUrl_CheckedChanged" AutoPostBack="true" ID="chkLoginUrl" Checked="true" Text="Use N2 to sign in on this site" ToolTip="Checking this box will update web.config forms element and membership providers. You can reconfigure this later in web.config." runat="server" />
			<em>(Recommended! You most likely need this to log in)</em>
		</p>
		<asp:Panel runat="server" ID="pnlPassword">
			<p>Please select a secure password for the <strong>N2 CMS Superadmin</strong>. Other administrators and editors can be created later.</p>
			<p><asp:Label Text="User Name" AssociatedControlID="lblUserName" runat="server" /><asp:Label Text="admin" ID="lblUserName" runat="server" Font-Bold="true" /></p>
			<p><asp:Label Text="Password" AssociatedControlID="txtPassword" runat="server" /><asp:TextBox TextMode="Password" ID="txtPassword" runat="server" /><asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtPassword" Text="Password is required" runat="server" /></p>
			<p><asp:Label Text="Repeat Password" AssociatedControlID="txtPassword" runat="server" /><asp:TextBox TextMode="Password" ID="txtRepeatPassword" runat="server" /><asp:RequiredFieldValidator ControlToValidate="txtRepeatPassword" runat="server" /><asp:CompareValidator ControlToValidate="txtRepeatPassword" ControlToCompare="txtPassword" Text="Passwords doesn't match" runat="server" /></p>
			<p>
				<asp:Button runat="server" Text="OK" OnCommand="OkCommand" CssClass="btn btn-primary" />
			</p>
		</asp:Panel>
		<asp:Panel runat="server" ID="pnlNoPassword" Visible="false">
			<p><a href="<%= continueUrl %>">Continue anyway</a></p>
		</asp:Panel>
	<%} else if (action == "install"){%>
		<h1>Welcome to <a href="http://n2cms.com/">N2 CMS</a> Installation Wizard</h1>
		<% if (autoLogin) { %>
		<p>
			You have been automatically logged in with the user <strong>admin</strong> and the password <strong>changeme</strong>. 
			N2 CMS is not allowed to change this password, you must <strong>change it manually</strong> in web.config.
		</p>
		<% } else { %>
		<p>To continue you need to log in with the username <strong>admin</strong> and the password you specified during installation.</p>
		<% } %>
		<p>Okay, <a href="<%= continueUrl %>">please help me <strong>install</strong> the database on a new site &raquo;</a></p>
	<%} else if(action == "upgrade") {%>
		<h1>Welcome to <a href="http://n2cms.com/">N2 CMS</a> Upgrade Wizard</h1>
		<% if (autoLogin) { %>
		<p>
			You have been automatically logged in with the user <strong>admin</strong> and the password <strong>changeme</strong>. 
			N2 CMS is not allowed to change this password, you must <strong>change it manually</strong> in web.config.
		</p>
		<% } else { %>
		<p>To continue you need to log in with the username <strong>admin</strong> and the password you specified during installation.</p>
		<% } %>
		<p><a href="<%= continueUrl %>">Please help me <strong>upgrade</strong> from a previous version &raquo;</a></p>
	<%} else if (action == "rebase") {%>
		<h1>Welcome to <a href="http://n2cms.com/">N2 CMS</a> Rebase Wizard</h1>
		<% if (autoLogin) { %>
		<p>
			You have been automatically logged in with the user <strong>admin</strong> and the password <strong>changeme</strong>. 
			N2 CMS is not allowed to change this password, you must <strong>change it manually</strong> in web.config.
		</p>
		<% } else { %>
		<p>To continue you need to log in with the username <strong>admin</strong> and the password you specified during installation.</p>
		<% } %>
		<p><a href="<%= continueUrl %>"><strong>Rebase</strong> links from a previous virtual directory &raquo;</a></p>
	<%} else {%>
		<h1>Welcome to N2 CMS</h1>
		<p>What do you want to do with <a href="http://n2cms.com/">N2 CMS</a>?</p>
		<p><a href="<%= N2.Web.Url.ResolveTokens(config.InstallUrl) %>"><strong>Install</strong> a the database for a new site &raquo;</a></p>
		<p><a href="<%= N2.Web.Url.ResolveTokens(config.UpgradeUrl) %>"><strong>Upgrade</strong> from a previous version &raquo;</a></p>
		<p><a href="<%= N2.Web.Url.ResolveTokens(config.RebaseUrl) %>"><strong>Rebase</strong> links from another virtual directory &raquo;</a></p>
	<%}%>
	<hr />
	<p><strong>Already installed?</strong> There might be problems connecting to the database. To prevent this screen from appearing modify web.config:</p>
	<pre>&lt;n2&gt;&lt;edit&gt;&lt;installer checkInstallationStatus="<strong>false</strong>"/&gt;</pre>
</div>
</div>
</form>
</body>
</html>
