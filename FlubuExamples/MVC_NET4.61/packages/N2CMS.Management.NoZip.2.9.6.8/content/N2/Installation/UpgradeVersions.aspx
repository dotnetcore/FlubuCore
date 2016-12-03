<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpgradeVersions.aspx.cs" Inherits="N2.Edit.Install.UpgradeVersions" %>
<%@ Import Namespace="N2.Web" %>

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
    	.working { color:moccasin; }
    	textarea { width:95%;height:120px;border:none;background-color:#FFB; }
    	#StopMigration { display:none; }
    </style>
	<script type="text/javascript">
		var stop = false;
		var error = false;
		function StartMigration() {
			if (stop) {
				stop = false;
				$("#StopMigration").hide();
				return;
			}

			$("#StopMigration").show();

			var $next = $(".version:not(.error):first");
			if ($next.length) {
				$next.css("background-color", "moccasin");
				$.post("UpgradeVersion.ashx", { n2Item: $next.children(".version-id").text() }, function (result) {
					if (result.success) {
						$next.css("background-color", "green").remove();
					} else {
						error = true;
						$next.css("background-color", "");
						$next.addClass("error").attr("title", result.message);
					}
					StartMigration();
				});
			}
			else {
				$("#MigrationComplete").slideDown();
				$(".migration-control").hide();
				if (error) {
					$("#MigrationComplete p.error").show();
				} else {
					$("#MigrationComplete h1").addClass("ok");
					$("#MigrationRun").slideUp();
				}				
			}
		}
		function StopMigration() {
			stop = true;
		}
		function CloseMigration() {
			window.close();
		}
	</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <n2:TabPanel ID="TabPanel1" ToolTip="Upgrade Versions" runat="server">
			<div id="MigrationComplete" style="display:none">
				<h1>Finished</h1>
				<p>Versions have been migrated.</p>
				<p class="error" style="display:none">Errors occurred while migrating versions. Hover over the remaining items for some information and configure logging for full stack trace.</p>
				<input type="button" onclick="CloseMigration();" value="Close" />
			</div>

			<div style="overflow:auto; max-height:400px" id="MigrationRun">
				<h1>Upgrade versions to 2.4 model</h1>
				<p>The migrate button will convert versions stored as items in the content item database to an external table. In the process the existing versions items will be deleted, and an added to the version table.</p>
				<p>The version table will also include current parts on the page. Only the latest version of parts is included and previous versions of parts will be lost.</p>
				<table class="table table-striped table-hover table-condensed">
				<thead>
					<tr><th>ID</th><th>Index</th><th>Title</th><th>Version Of</th></tr>
				</thead>
				<tbody>
				<asp:Repeater id="rptVersions" runat="server">
					<ItemTemplate>
						<tr class="version"><td class="version-id"><%# Eval("ID") %></td><td><%# Eval("VersionIndex") %></td><td><a href="<%# Eval("Url") %>"><%# Eval("Title") %></a></td><td><%# Eval("VersionOf.ID") %></td></tr>
					</ItemTemplate>				
				</asp:Repeater>
				</tbody>
				</table>	
			</div>
			<input type="button" onclick="StartMigration();" value="Migrate" class="migration-control" />
			<input type="button" onclick="StopMigration();" value="Stop" id="StopMigration" class="migration-control" />
			
		</n2:TabPanel>
        <asp:Label EnableViewState="false" ID="errorLabel" runat="server" CssClass="errorLabel" />
    </div>
    </form>
</body>
</html>
