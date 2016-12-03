<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CsvImport.ascx.cs" Inherits="N2.Management.Content.Export.CsvImport" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<div>
	<edit:InfoLabel id="lblLocation" Label="Import to" runat="server" />
</div>
<div>
	<asp:Label ID="lblTypes" runat="server" Text="Type" />
	<asp:DropDownList ID="ddlTypes" DataTextField="Title" DataValueField="Discriminator" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlTypes_OnSelectedIndexChanged" />
</div>
<div>
	<asp:CheckBox ID="chkFirstRow" runat="server" Text="First row is header row" AutoPostBack="true" OnCheckedChanged="chkFirstRow_OnCheckedChanged" />

	<table class="table table-striped table-hover">
		<thead><tr>
			<asp:Repeater id="rptPreview" runat="server" DataSource="<%# FirstRow.Columns %>">
				<ItemTemplate>
					<th>
						<span><%# Container.DataItem %></span>
						<asp:DropDownList id="ddlColumnMap" runat="server" DataSource="<%# Editables %>" DataValueField="Name" DataTextField="Title" />
					</th>
				</ItemTemplate>
			</asp:Repeater>
		</tr></thead>

	<% if(Rows != null && Rows.Any()) { %>
		<tbody>
		<% foreach(var row in Rows.Take(10)){ %>
			<tr>
			<% foreach(var col in row.Columns){ %>
				<td><%= col %></td>
			<%} %>
			</tr>
		<%} %>
		</tbody>
	<%} %>
	</table>

	<asp:Button ID="btnImport" Text="Import" runat="server" OnCommand="ImportCommand"  />

</div>