<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Settings.Default" meta:resourceKey="settingsPage" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
	<asp:LinkButton ID="btnSave" OnClick="btnSave_Click" runat="server" CssClass="btn btn-primary command primary-action" meta:resourceKey="btnSave">Save</asp:LinkButton>
	<edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <asp:PlaceHolder ID="phSettings" runat="server" />
</asp:Content>
