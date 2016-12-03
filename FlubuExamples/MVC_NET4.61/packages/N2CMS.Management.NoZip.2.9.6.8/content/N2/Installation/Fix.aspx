<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Fix.aspx.cs" Inherits="N2.Edit.Install.Fix" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Fix</title>
	<asp:PlaceHolder runat="server">
		<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapCssPath) %>" type="text/css" rel="stylesheet" />
		<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapJsPath)  %>" type="text/javascript"></script>
	</asp:PlaceHolder>
    <link rel="stylesheet" type="text/css" href="../Resources/Css/all.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/framed.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/themes/default.css" />
    <style type="text/css">
    	body {font-size:.7em;font-family:Verdana;}
		td {white-space:nowrap;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="content">
		<asp:Repeater ID="rptCns" runat="server">
			<ItemTemplate>
				<a href="fix.aspx?cn=<%# Eval("Name") %>"><%# Eval("Name") %></a>
			</ItemTemplate>
		</asp:Repeater>
		
		<asp:SqlDataSource ID="sdsItems" runat="server">
			<DeleteParameters>
				<asp:Parameter Name="ID" Type="Int32" />
			</DeleteParameters>
			<UpdateParameters>
				<asp:Parameter Name="Type" Type="String" />
				<asp:Parameter Name="Updated" Type="DateTime" />
				<asp:Parameter Name="Name" Type="String" />
				<asp:Parameter Name="ZoneName" Type="String" />
				<asp:Parameter Name="Title" Type="String" />
				<asp:Parameter Name="Created" Type="DateTime" />
				<asp:Parameter Name="Published" Type="DateTime" />
				<asp:Parameter Name="Expires" Type="DateTime" />
				<asp:Parameter Name="SortOrder" Type="Int32" />
				<asp:Parameter Name="Visible" Type="Boolean" />
				<asp:Parameter Name="SavedBy" Type="String" />
				<asp:Parameter Name="VersionOfID" Type="Int32" />
				<asp:Parameter Name="ParentID" Type="Int32" />
				<asp:Parameter Name="ID" Type="Int32" />
			</UpdateParameters>
			<InsertParameters>
				<asp:Parameter Name="Type" Type="String" />
				<asp:Parameter Name="Updated" Type="DateTime" />
				<asp:Parameter Name="Name" Type="String" />
				<asp:Parameter Name="ZoneName" Type="String" />
				<asp:Parameter Name="Title" Type="String" />
				<asp:Parameter Name="Created" Type="DateTime" />
				<asp:Parameter Name="Published" Type="DateTime" />
				<asp:Parameter Name="Expires" Type="DateTime" />
				<asp:Parameter Name="SortOrder" Type="Int32" />
				<asp:Parameter Name="Visible" Type="Boolean" />
				<asp:Parameter Name="SavedBy" Type="String" />
				<asp:Parameter Name="VersionOfID" Type="Int32" />
				<asp:Parameter Name="ParentID" Type="Int32" />
			</InsertParameters>
		</asp:SqlDataSource>
		
		<ul>
			<li>The "Type" must match one of the definitions in the solution and is typically equal to the class name or name/discriminator on the definition. Changing this my crash the entire application.</li>
			<li>Deleting pages that have children (other.ParentID = this.ID) may cause "detached" items to be lying around in the database which can't be access by normal means (although they'll be here).</li>
		</ul>
		
		<asp:GridView ID="gvItems" runat="server" DataSourceID="sdsItems" PageSize="22" BorderWidth="0"
			AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
			DataKeyNames="ID" CssClass="table table-striped table-hover table-condensed">
			<Columns>
				<asp:CommandField ShowCancelButton="true" ShowDeleteButton="true" ShowEditButton="true" CancelText="Cancel" UpdateText="Update" DeleteText="Delete" EditText="Edit" />
				<asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" />
				<asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
				<asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
				<asp:BoundField DataField="ZoneName" HeaderText="ZoneName" SortExpression="ZoneName" />
				<asp:CheckBoxField DataField="Visible" HeaderText="Visible" SortExpression="Visible" />
				<asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" />
				<asp:BoundField DataField="SavedBy" HeaderText="SavedBy" SortExpression="SavedBy" />
				<asp:BoundField DataField="SortOrder" HeaderText="SortOrder" SortExpression="SortOrder" />
				<asp:BoundField DataField="VersionOfID" HeaderText="VersonOfID" SortExpression="VersionOfID" />
				<asp:BoundField DataField="ParentID" HeaderText="ParentID" SortExpression="ParentID" />
				<asp:BoundField DataField="Created" HeaderText="Created" SortExpression="Created" />
				<asp:BoundField DataField="Updated" HeaderText="Updated" SortExpression="Updated" />
				<asp:BoundField DataField="Published" HeaderText="Published" SortExpression="Published" />
				<asp:BoundField DataField="Expires" HeaderText="Expires" SortExpression="Expires" />
			</Columns>
		</asp:GridView>
    
		<asp:Label runat="server" ID="lblError" />
    </div>
    </form>
</body>
</html>
