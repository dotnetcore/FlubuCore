<%@ Page Language="C#" MasterPageFile="~/N2/Content/Framed.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Membership.EditRoles" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <asp:Panel runat="server" ID="panelUsers">
        <asp:GridView
            ID="dgrUsers"
            AutoGenerateColumns="false"
            CssClass="table table-striped table-hover table-condensed"
            UseAccessibleHeader="true"
            DataKeyNames="UserName"
            BorderWidth="0px"
            OnRowDeleting="dgrUsers_OnRowDeleting"
            runat="server">
            
            <Columns>
                <asp:BoundField HeaderText="User name" DataField="UserName" meta:resourcekey="UserNameField" />
			    <asp:ButtonField Text="Delete" CommandName="Delete" meta:resourcekey="DeleteField" />
            </Columns>

        </asp:GridView>
        
        <p id="htmNoUsers" Visible="false" runat="server">
            <asp:Localize meta:resourcekey="NoUsersMessage" runat="server" />
        </p>
        
        <p>
            <asp:HyperLink NavigateUrl="Roles.aspx" Text="Return to list" CssClass="btn" meta:resourcekey="BackButton" runat="server" />
        </p>
    </asp:Panel>
</asp:Content>
