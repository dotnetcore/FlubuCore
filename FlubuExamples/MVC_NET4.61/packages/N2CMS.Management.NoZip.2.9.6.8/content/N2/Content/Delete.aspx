<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="N2.Edit.Delete" Title="Delete" %>
<%@ Register Src="AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
		<asp:LinkButton ID="btnDelete" runat="server" OnClick="OnDeleteClick" CssClass="btn btn-danger command iconed delete primary-action" data-icon-class="fa fa-trash-o" meta:resourceKey="btnDelete">Delete</asp:LinkButton>
		<edit:CancelLink ID="hlCancel" runat="server" CssClass="btn cancel" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
	<edit:PermissionPanel id="ppPermitted" RequiredPermission="Publish" runat="server" meta:resourceKey="ppPermitted">
		<asp:CustomValidator ID="cvRemoved" Text="Can't delete item that is not available. It may have been deleted or renamed in another window." runat="server" CssClass="alert alert-error alert-margin" meta:resourceKey="cvRemoved" Display="Dynamic" />
		<asp:CustomValidator ID="cvDelete" runat="server" CssClass="alert alert-margin" meta:resourceKey="cvDelete" Display="Dynamic" />
		<asp:CustomValidator ID="cvException" runat="server" CssClass="alert alert-error alert-margin" Display="Dynamic" />
	<fieldset id="referencingItems" runat="server" style="padding:8px; margin-bottom:10px">
		<legend><asp:CheckBox ID="chkAllow" Checked="true" AutoPostBack="true" OnCheckedChanged="chkAllow_OnCheckedChanged" runat="server" Text="Delete and break references" meta:resourceKey="chkAllow" /></legend>
		<div style="padding: 5px;">
		<asp:Repeater ID="rptReferencing" runat="server">
			<ItemTemplate><div><edit:ContentLink runat="server" DataSource='<%# Container.DataItem %>' /></div></ItemTemplate>
		</asp:Repeater>
		</div>
		<asp:HyperLink runat="server" ID="hlReferencingItems" CssClass="hrtop" Text="List Referencing Items" meta:resourceKey="hlReferencingItems" />
	</fieldset>
	<edit:FieldSet ID="affectedItems" class="affectedItems" runat="server" Legend="Affected items" meta:resourceKey="affectedItems">
		<uc1:AffectedItems id="itemsToDelete" runat="server" />
	</edit:FieldSet>
	</edit:PermissionPanel>
</asp:Content>
