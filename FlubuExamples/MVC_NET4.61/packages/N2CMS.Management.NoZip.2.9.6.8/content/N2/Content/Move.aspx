<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Move.aspx.cs" Inherits="N2.Edit.Move" Title="Move" %>

<%@ Register Src="AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Toolbar" runat="server">
	<asp:LinkButton ID="btnMove" runat="server" OnClick="OnMoveClick" CssClass="btn btn-primary command primary-action" meta:resourceKey="btnMove">try again</asp:LinkButton>
	<asp:HyperLink ID="btnCancel" runat="server" Text="cancel" CssClass="btn command cancel" meta:resourceKey="hlCancel" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
	<asp:CustomValidator ID="cvMove" runat="server" CssClass="alert alert-margin" meta:resourceKey="cvMove" />
	<asp:CustomValidator ID="cvException" runat="server" CssClass="alert alert-error alert-margin" Display="Dynamic" />

	<div class="cf">
		<asp:Panel ID="pnlNewName" runat="server" CssClass="formField">
			<asp:Label ID="lblNewName" runat="server" Text="New name" meta:resourceKey="lblNewName" AssociatedControlID="txtNewName" />
			<asp:TextBox ID="txtNewName" runat="server" />
		</asp:Panel>
		<div class="formField">
			<asp:Label ID="lblFrom" runat="server" Text="From" meta:resourceKey="lblFrom" AssociatedControlID="from" />
			<asp:Label ID="from" runat="server" />
		</div>
		<div class="formField">
			<asp:Label ID="lblTo" runat="server" Text="To" meta:resourceKey="lblTo" AssociatedControlID="to" />
			<asp:Label ID="to" runat="server" />
		</div>
	</div>
	<hr />
	<h3 id="h3" runat="server" meta:resourcekey="h3">Moved items:</h3>
	<uc1:AffectedItems ID="itemsToMove" runat="server" />
</asp:Content>
