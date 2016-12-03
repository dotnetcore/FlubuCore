<%@ Page Language="C#" MasterPageFile="../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="N2.Edit.Membership.Users" Title="Users" meta:resourcekey="PageResource1" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<asp:HyperLink runat="server" NavigateUrl="New.aspx" CssClass="btn" meta:resourcekey="HyperLinkResource1">New user</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	
	<asp:GridView ID="dgrUsers" runat="server" DataSourceID="odsUsers" AllowPaging="True"
		AutoGenerateColumns="False" UseAccessibleHeader="True" OnRowDeleting="dgrUsers_OnRowDeleting"
		DataKeyNames="UserName" BorderWidth="0px"
		CssClass="table table-striped table-hover table-condensed table-bordered" meta:resourcekey="dgrUsersResource1">
		<PagerSettings Mode="NumericFirstLast" PageButtonCount="20" Position="TopAndBottom" />
		<Columns>
			<asp:HyperLinkField DataNavigateUrlFields="UserName" DataTextField="UserName" DataNavigateUrlFormatString="Edit.aspx?user={0}" meta:resourcekey="bcUserName" runat="server"/>
			<asp:BoundField DataField="Email" HeaderText="email" meta:resourcekey="bcEmail" runat="server"/>
			<asp:BoundField DataField="CreationDate" HeaderText="created" meta:resourcekey="bcCreated" runat="server"/>
			<asp:HyperLinkField DataNavigateUrlFields="UserName" 
				DataNavigateUrlFormatString="Password.aspx?user={0}" Text="password" 
				meta:resourcekey="HyperLinkColumnResource1" />
			<asp:ButtonField Text="delete" CommandName="Delete"  
				meta:resourcekey="ButtonColumnResource1" runat="server" />
			<asp:TemplateField runat="server">
				<ItemTemplate>
					<div><%# Eval("Comment") %></div>
					<%# (bool)Eval("IsOnline") ? "<span title='Online'>O</span>" : string.Format("<span title='Offline, last login: {0}'>F</span>", Eval("LastLoginDate"))%>
					<%# (bool)Eval("IsLockedOut") ? string.Format("<span title='Locked out: {0}'>L</span>", Eval("LastLockoutDate")) : ""%>
					<%# (bool)Eval("IsApproved") ? "" : "<span title='Not Approved'>A</span>"%>
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
	<asp:ObjectDataSource ID="odsUsers" runat="server" TypeName="N2.Edit.Membership.UsersSource"
		EnablePaging="True" SelectMethod="GetUsers" SelectCountMethod="GetUsersCount"
		MaximumRowsParameterName="max" StartRowIndexParameterName="start" DeleteMethod="DeleteUser" />
</asp:Content>
