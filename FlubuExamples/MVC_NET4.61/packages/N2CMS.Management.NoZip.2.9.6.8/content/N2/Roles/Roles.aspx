<%@ Page Title="Roles" Language="C#" MasterPageFile="~/N2/Content/Framed.Master" AutoEventWireup="True" CodeBehind="Roles.aspx.cs" Inherits="N2.Edit.Membership.ListRoles" meta:resourcekey="Page" %>

<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink NavigateUrl="New.aspx" Text="New" CssClass="btn" meta:resourcekey="NewButton" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <asp:GridView
        ID="dgrRoles"
        AutoGenerateColumns="false"
        CssClass="table table-striped table-hover table-condensed"
        UseAccessibleHeader="true"
        BorderWidth="0px"
        OnRowDataBound="dgrRoles_OnRowDataBound"
        OnRowDeleting="dgrRoles_OnRowDeleting"
        DataKeyNames="RoleName"
        runat="server">
        
        <Columns>
			<asp:HyperLinkField HeaderText="Role name" DataNavigateUrlFields="RoleName" DataTextField="RoleName" DataNavigateUrlFormatString="Edit.aspx?role={0}" meta:resourcekey="RoleNameField" />
			<asp:ButtonField Text="Delete" CommandName="Delete" meta:resourcekey="DeleteField" />
        </Columns>

    </asp:GridView>
</asp:Content>
