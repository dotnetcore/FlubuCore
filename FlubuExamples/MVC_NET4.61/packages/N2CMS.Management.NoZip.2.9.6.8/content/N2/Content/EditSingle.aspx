<%@ Page Title="" Language="C#" MasterPageFile="~/N2/Content/Framed.master" AutoEventWireup="true" CodeBehind="EditSingle.aspx.cs" Inherits="N2.Management.Content.EditSingle" meta:resourcekey="PageResource1" %>
<%@ Import namespace="N2"%>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
		<link rel="stylesheet" href="<%= MapCssUrl("edit.css") %>" type="text/css" />
</asp:Content>

<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
	<asp:LinkButton ID="btnSave" OnCommand="OnPublishCommand" runat="server" CssClass="btn btn-primary command iconed publish primary-action" meta:resourceKey="btnSave">Save</asp:LinkButton>
	<edit:CancelLink ID="hlCancel" runat="server" Text="Close" CssClass="btn" meta:resourceKey="hlCancel" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<edit:PermissionPanel id="ppPermitted" RequiredPermission="Write" runat="server" meta:resourceKey="ppPermitted">
		<asp:ValidationSummary ID="vsEdit" runat="server" CssClass="alert alert-margin" HeaderText="The item couldn't be saved. Please look at the following:" meta:resourceKey="vsEdit"/>
		<asp:CustomValidator ID="cvException" CssClass="alert alert-error alert-margin" runat="server" Display="None" />

		<n2:ItemEditor ID="ie" runat="server" />
		
	</edit:PermissionPanel>

</asp:Content>
