<%@ Page MasterPageFile="../Content/Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Rebuild.aspx.cs" Inherits="N2.Management.Files.Rebuild" %>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<fieldset>
		<legend>Images below:</legend>
		<%= Selection.SelectedItem.Title %>
	</fieldset>
	<fieldset>
		<legend>Added sizes:</legend>
		<ul>
		<% foreach(var size in Request["add"].Split(',').Where(s => s != "")) { %>
			<li><%= size %></li>
		<% } %>
		</ul>
	</fieldset>
	<fieldset>
		<legend>Updated sizes:</legend>
		<ul>
		<% foreach(var size in Request["modify"].Split(',').Where(s => s != "")) { %>
			<li><%= size %></li>
		<% } %>
		</ul>
	</fieldset>
	<fieldset>
		<legend>Removed sizes:</legend>
		<ul>
		<% foreach(var size in Request["remove"].Split(',').Where(s => s != "")) { %>
			<li><%= size %></li>
		<% } %>
		</ul>
	</fieldset>

	<asp:LinkButton ID="btnUpdate" Text="Rebuild" CssClass="btn" runat="server" OnCommand="btnUpdate_Command" />
	<style>
		.processed-image { display:none; }
	</style>
</asp:Content>
