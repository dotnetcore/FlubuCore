<%@ Page Title="" Language="C#" MasterPageFile="~/N2/Content/Framed.master" AutoEventWireup="true" CodeBehind="Activity.aspx.cs" Inherits="N2.Management.Users.Activity" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<fieldset>
		<legend>Test messages</legend>

		<asp:TextBox ID="txtMessage" runat="server" placeholder="Message" />
		<asp:Button Text="Send" runat="server" OnCommand="OnSendCommand" />

		<asp:GridView ID="gvMessages" runat="server" CssClass="table table-striped table-hover table-condensed" BorderWidth="0">
		</asp:GridView>
	</fieldset>

</asp:Content>
