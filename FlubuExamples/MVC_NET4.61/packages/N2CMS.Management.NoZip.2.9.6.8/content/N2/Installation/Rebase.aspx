<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rebase.aspx.cs" Inherits="N2.Management.Installation.Rebase" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Rebase N2</title>
	<asp:PlaceHolder runat="server">
		<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapCssPath) %>" type="text/css" rel="stylesheet" />
		<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapJsPath)  %>" type="text/javascript"></script>
	</asp:PlaceHolder>
    <link rel="stylesheet" type="text/css" href="../Resources/Css/all.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/framed.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/themes/default.css" />
    <style type="text/css">
    	form{font-size:1.1em;width:800px;margin:10px auto;}
    	a{color:#00e;}
    	li{margin-bottom:10px}
    	form{padding:20px}
    	.warning{color:#f00;}
    	.ok{color:#0c0;}
    	.buttons { text-align:right; }
    	textarea{width:95%;height:120px; border:none; background-color:#FFB}
    	pre { overflow:auto; font-size:10px; color:Gray; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
	<div>
        <n2:TabPanel ID="tp" ToolTip="Rebase" runat="server">
			<% string newBase = ResolveUrl("~/"); %>
			<h1>Rebase images and links</h1>
			<% if (RebasedLinks == null) {%>
			<% if(newBase == Status.AppPath){%><p class="warning">You don't need to rebase.</p><%}%>
			<p>Rebasing changes the application path of images and links within the site from <strong>'<%=Status.AppPath%>'</strong> to <strong>'<%=newBase%>'</strong>. E.g. the image source of '<%=Status.AppPath%>upload/image.jpg' will be rebased to '<%=newBase%>upload/image.jpg'.</p>
			<p><asp:Button ID="btnRebase" runat="server" OnClick="btnRebase_Click" Text="Rebase links" OnClientClick="return confirm('Rebasing makes changes to the information on the site. I confirm that everything is backed-up and want to continue.');" ToolTip="Click this button to rebase links" CausesValidation="false"/> </p>
			<%} else {%>
			<table>
			<% int count = 0;
			foreach (N2.Edit.Installation.RebaseInfo item in RebasedLinks) {%>
			<tr>
				<td><a href="../Content/Default.aspx?<%= N2.Edit.SelectionUtility.SelectedQueryKey %>=<%= item.ItemPath %>" title="<%= item.ItemID %>"><%= item.ItemTitle%></a></td>
				<td><%= item.PropertyName%></td>
			</tr>
			<% count++; } %>
			</table>
			<p><%= count %> details rebased.</p>
			<p><a href="..">Done &raquo;</a></p>
			<%} %>
		</n2:TabPanel>
	</div>
	</form>
</body>
</html>
