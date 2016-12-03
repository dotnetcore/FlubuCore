<%@ Page Title="New role" Language="C#" MasterPageFile="../Content/Framed.Master" AutoEventWireup="true" CodeBehind="New.aspx.cs" Inherits="N2.Edit.Membership.NewRole" meta:resourcekey="Page" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <p>
        <asp:Label AssociatedControlID="txtRoleName" meta:resourcekey="RoleNameLabel" runat="server" />
        <asp:TextBox ID="txtRoleName" runat="server" /><br />

        <asp:RequiredFieldValidator ControlToValidate="txtRoleName" Display="Dynamic" EnableClientScript="false" meta:resourcekey="RequiredFieldValidator" runat="server" />
        <asp:CustomValidator ControlToValidate="txtRoleName" Display="Dynamic" OnServerValidate="ValidateNewGroup" EnableClientScript="false" meta:resourcekey="DuplicateValidator" runat="server" />
    </p>
        
    <p>
        <asp:LinkButton OnClick="btnCreateRoleClick" CausesValidation="True" CssClass="btn" Text="Save" meta:resourcekey="SaveButton" runat="server" />
        <asp:HyperLink NavigateUrl="Roles.aspx" CssClass="btn" Text="Close" meta:resourcekey="CancelButton" runat="server" />
    </p>
</asp:Content>
