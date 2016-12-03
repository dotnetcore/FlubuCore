<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upgrade.aspx.cs" Inherits="N2.Edit.Install.Upgrade" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Upgrade N2</title>
	<asp:PlaceHolder runat="server">
		<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapCssPath) %>" type="text/css" rel="stylesheet" />
		<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapJsPath)  %>" type="text/javascript"></script>
	</asp:PlaceHolder>
    <link rel="stylesheet" type="text/css" href="../Resources/Css/all.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/framed.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/themes/default.css" />
    <style>
    	form { font-size:1.1em;width:800px;margin:10px auto; }
    	a { color:#00e; }
    	li { margin-bottom:10px; }
    	form { padding:20px; }
    	.warning { color:#f00; }
    	.error { color:#f00; padding:5px; background-color:Yellow; }
    	.ok { color:#0c0; }
    	textarea { width:95%;height:80px;border:none;background-color:#FFB; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <n2:TabPanel ID="tpProgress" ToolTip="Progress" runat="server" Visible="false">
	        <h1>Progress</h1>
			<p>N2CMS is processing the requested action.</p>
			<hr/>
	        <p style="font-weight: bold;"><asp:Literal runat="server" ID="lblProgress" Text="Percent complete" /></p>
			<hr/>
			<p>Progress will be updated in 10 seconds. <asp:Button runat="server" OnClick="RefreshProgress" Text="Update now" CssClass="btn btn-primary" /></p>
			<script type="text/javascript"> setTimeout("document.forms[0].submit();", 10000);</script>
		</n2:TabPanel>
		<n2:TabPanel ID="TabPanel1" ToolTip="Upgrade" runat="server">
			<h1>Upgrade database from <%= Checker.Status.DatabaseVersion %> to <%= N2.Edit.Installation.DatabaseStatus.RequiredDatabaseVersion%></h1>
			
			<% if (Checker.Status.NeedsUpgrade){ %>
			<p class="alert alert-info">The database needs to be upgraded.</p>
				<% if(Checker.Status.ConnectionType == "MySqlConnection") {%>
			<p class="alert">MySQL database might not be upgradeable from this interface. Execute <a href="mysql.upgrade.2.sql">mysql.upgrade.2.sql</a> with an SQL admin tool and manually execute migrations from advanced optins.</p>
				<% } %>
			<% } else if (!Checker.Status.IsInstalled){ %>
			<p class="alert alert-error">No database to be upgraded. Please <a href="Default.aspx">install using the installation wizard</a>.</p>
			<hr />
			<% } else {%>
			<p class="alert alert-success">All core tables are up to date. Happy <a href="..">editing</a>.</p>
			<%} %>
			
			<fieldset>
				<legend>SQL Schema</legend>
				<p>Please review this schema update script.</p>
				<textarea readonly="readonly"><%= Installer.ExportUpgradeSchema() %></textarea>
			</fieldset>
			
			<fieldset>
				<legend>Migrations</legend>
				<p>In addition to any schema changes, the following migrations will be executed on the <%= Checker.Status.Items %> items in your database: </p>
				<table class="table table-striped">
					<% foreach (N2.Edit.Installation.AbstractMigration migration in Migrator.GetMigrations(Checker.Status)) { %>
					<tr>
						<th style="text-align:left"><%= migration.Title %></th><td><%= migration.Description %></td>
					</tr>
					<%} %>
				</table>
				<% if (Checker.Status.Items > 1000) { %>
				<p class="alert">The database contains a large number of items, the migration may take a while. It's recommended to increase the request execution timeout and the database connection timeout.</p>
				<% } %>
			</fieldset>
            <div class="alert alert-info">
				Make sure you have <strong>backed up</strong> your data (just in case).
			</div>
			<p>
				<asp:Button ID="btnUpgrade" runat="server" OnClick="btnInstallAndMigrate_Click" Text="Update tables and run migrations" OnClientClick="return confirm('Updating the database makes changes to the information on the site. I confirm that everything is backed-up and want to continue.');" ToolTip="Click this button to update the database and execute the migrations" CausesValidation="false" CssClass="btn btn-primary btn-large"/>
			</p>
			<asp:Label ID="lblResult" runat="server" />
		    <script type="text/javascript">
		    	function showadvancedcontentoptions() {
		    		document.getElementById("advancedcontentoptions").style.display = "block";
		    		this.style.display = "none";
		    		return false;
		    	}
		    </script>
		    <hr />
			<p><a href="#advancedcontentoptions" onclick="return showadvancedcontentoptions.call(this);">Advanced options (includes update schema only and script download).</a></p>
			<div id="advancedcontentoptions" style="display:none;">
				<p>
					Only 
					<asp:Button ID="btnInstall" runat="server" OnClick="btnInstall_Click" Text="update tables" OnClientClick="return confirm('Updating the database makes changes to the information on the site. I confirm that everything is backed-up and want to continue.');" ToolTip="Click this button to update the database schema" CausesValidation="false" CssClass="btn"/> 
					without running migrations.
				</p>
				<p>
					<asp:Button ID="btnExport" runat="server" OnClick="btnExportSchema_Click" Text="download the SQL script" ToolTip="Click this button to generate update database script" CausesValidation="false" CssClass="btn"/>
					for the connection type <strong><%= Checker.Status.ConnectionType %></strong> and manually update tables.
				</p>
				<p>
					Manually select 
					<blockquote>
						<asp:CheckBoxList ID="cblMigrations" runat="server" />
					</blockquote>
					and 
					<asp:Button ID="btnMigrate" runat="server" OnClick="btnMigrate_Click" Text="run selected migrations" OnClientClick="return confirm('Updating the database makes changes to the information on the site. I confirm that everything is backed-up and want to continue.');" ToolTip="Execute migrations against current database" CausesValidation="false" CssClass="btn"/> 
					against the current version of the database.
				</p>
            </div>
		</n2:TabPanel>
        <asp:Label EnableViewState="false" ID="errorLabel" runat="server" CssClass="alert alert-error" Visible="false" style="font-size:10px; display:block;" />
    </div>
    </form>
</body>
</html>
