<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Trash.Default" %>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-danger command restore primary-action" meta:resourceKey="btnClear" OnClientClick="return confirm('really empty trash?');" OnClick="btnClear_Click">Empty trash</asp:LinkButton>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="btn cancel command" meta:resourceKey="hlCancel">Close</asp:HyperLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
	<n2:ItemDataSource id="idsTrash" runat="server" />
	<h1>Trash</h1>
	
	<asp:HyperLink ID="hlRunning" runat="server" Text="A delete task is in progress" CssClass="info" Visible="False" meta:resourcekey="hlRunning"/>

	<asp:CustomValidator ID="cvRestore" CssClass="alert alert-margin" ErrorMessage="An item with the same name already exists at the previous location." runat="server" Display="Dynamic" />
	<asp:GridView ID="gvTrash" DataKeyNames="ID" runat="server" BorderWidth="0" DataSourceID="idsTrash" AutoGenerateColumns="false" OnRowCommand="gvTrash_RowCommand" EmptyDataText="No items in trash" CssClass="table table-striped table-hover table-condensed">
		<Columns>
			<asp:TemplateField HeaderText="Title" meta:resourceKey="colTitle">
				<ItemTemplate>
					<asp:HyperLink ID="hlDeletedItem" runat="server" NavigateUrl='<%# Eval("Url") %>' data-id='<%# Eval("ID") %>'>
						<asp:Image runat="server" ImageUrl='<%# Eval("IconUrl") %>' />
						<%# HtmlEncode((string) Eval("Title")) %>
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Deleted" meta:resourceKey="colDeleted">
				<ItemTemplate>
					<%# ((N2.ContentItem)Container.DataItem)["DeletedDate"] %>				
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Previous location" meta:resourceKey="colPrevious">
				<ItemTemplate>
					<asp:HyperLink ID="hlPreviousLocation" runat="server" NavigateUrl='<%# DataBinder.Eval(((N2.ContentItem)Container.DataItem)["FormerParent"], "Url") %>'>
						<asp:Image runat="server" ImageUrl='<%# DataBinder.Eval(((N2.ContentItem)Container.DataItem)["FormerParent"], "IconUrl") %>' />
						<%# HtmlEncode((string)DataBinder.Eval(((N2.ContentItem)Container.DataItem)["FormerParent"], "Title")) %>
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:ButtonField Text="Restore" CommandName="Restore" meta:resourceKey="colRestore" />
			<asp:ButtonField Text="Delete" CommandName="Purge" meta:resourceKey="colDelete" />
		</Columns>
	</asp:GridView>
</asp:Content>
