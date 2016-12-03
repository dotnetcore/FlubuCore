<%@ Page Language="C#" MasterPageFile="../../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Directory.aspx.cs" Inherits="N2.Edit.FileSystem.Directory1" %>
<%@ Register TagPrefix="edit" TagName="FileUpload" Src="FileUpload.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Import Namespace="N2.Web" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<edit:ButtonGroup runat="server" CssClass="btn btn-danger">
		<asp:LinkButton ID="btnDelete" runat="server" Text="Delete selected" CssClass="command primary-action" OnCommand="OnDeleteCommand" OnClientClick="return confirm('Delete selected files and folders?');" meta:resourceKey="btnDelete" />
		<asp:HyperLink ID="hlEdit" runat="server" Text="Edit" CssClass="command edit" meta:resourceKey="hlEdit" />
	</edit:ButtonGroup>
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">	
	<h1><% foreach (N2.ContentItem node in ancestors) { %>/<a href="<%= Url.Parse("Directory.aspx").AppendSelection(node) %>"><%= node.Title %></a><% } %></h1>
	<div class="tabPanel" data-flag="Unclosable">
        <div class="directory cf">
		    <asp:Repeater ID="rptDirectories" runat="server">
			    <ItemTemplate>
				    <div class="file">
					    <label>
						    <input name="directory" value="<%# Eval("Path") %>" type="checkbox" />
						    <asp:Image ImageUrl='<%# Eval("IconUrl") %>' runat="server" />
					    </label>
						<edit:ItemLink DataSource="<%# Container.DataItem %>" InterfaceUrl="Directory.aspx" runat="server" />
				    </div>
			    </ItemTemplate>
		    </asp:Repeater>
		
		    <asp:Repeater ID="rptFiles" runat="server">
			    <ItemTemplate>
				    <div class="file">
					    <label style='<%# ImageBackgroundStyle((string)Eval("LocalUrl")) %>'>
						    <input name="file" value="<%# Eval("LocalUrl") %>" type="checkbox" />
					    </label>
						<edit:ItemLink DataSource="<%# Container.DataItem %>" InterfaceUrl="File.aspx" runat="server" />
				    </div>
			    </ItemTemplate>
		    </asp:Repeater>
	    </div>

		<edit:PermissionPanel id="ppPermitted" RequiredPermission="Write" runat="server" meta:resourceKey="ppPermitted">
			<edit:FileUpload runat="server" />
		</edit:PermissionPanel>
    </div>
</asp:Content>
