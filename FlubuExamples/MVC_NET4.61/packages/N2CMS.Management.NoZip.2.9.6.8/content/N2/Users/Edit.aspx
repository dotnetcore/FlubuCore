<%@ Page Language="C#" MasterPageFile="../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Membership.Edit" Title="Edit user" meta:resourcekey="PageResource1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
   <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" 
		CssClass="btn btn-primary command primary-action" meta:resourcekey="btnSaveResource1">Save</asp:LinkButton>
   <asp:HyperLink ID="hlPassword" runat="server" NavigateUrl="Password.aspx" 
		CssClass="btn command action" meta:resourcekey="hlPasswordResource1">Password</asp:HyperLink>
   <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="Users.aspx" 
		CssClass="btn command" meta:resourcekey="hlBackResource1">Close</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<div class="tabPanel">
    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" 
		Text="Email" meta:resourcekey="lblEmailResource1" />
    <asp:TextBox ID="txtEmail" runat="server" 
		meta:resourcekey="txtEmailResource1" />
    <asp:Label ID="lblRoles" runat="server" AssociatedControlID="cblRoles" 
		Text="Roles" meta:resourcekey="lblRolesResource1" />
	<div class="checkBoxList">
		<asp:CheckBoxList ID="cblRoles" runat="server" CssClass="cbl" 
			DataSourceID="odsRoles" meta:resourcekey="cblRolesResource1" RepeatLayout="Flow" />
	</div>
    <%--<asp:ObjectDataSource ID="odsRoles" runat="server" TypeName="N2.Edit.Membership.RolesSource" SelectMethod="GetAllRoles" />--%>
	</div>
</asp:Content>
