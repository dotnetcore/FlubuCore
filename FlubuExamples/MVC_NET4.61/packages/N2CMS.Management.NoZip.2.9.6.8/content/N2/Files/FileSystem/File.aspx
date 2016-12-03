<%@ Page Language="C#" MasterPageFile="../../Content/Framed.Master" AutoEventWireup="true" CodeBehind="File.aspx.cs" Inherits="N2.Edit.FileSystem.File1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Import Namespace="N2.Web" %>

<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<asp:LinkButton ID="btnDownload" runat="server" CssClass="btn command iconed download" OnCommand="OnDownloadCommand">
		<b class="fa fa-download"></b>
		<%= GetLocalResourceString("btnDownload.Text", "Download") %>
	</asp:LinkButton>
	
	<edit:ButtonGroup ID="bgSizes" runat="server"></edit:ButtonGroup>
	
	<asp:HyperLink ID="hlCrop" NavigateUrl="Crop.aspx" CssClass="btn command crop iconed" runat="server">
		<b class="fa fa-crop"></b>
		<%= GetLocalResourceString("hlCrop.Text", "Crop") %>
	</asp:HyperLink>
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary command" OnCommand="OnSaveCommand" Visible="false" meta:resourceKey="btnSave" />
	<asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CssClass="btn command" OnCommand="OnEditCommand" Visible="false" meta:resourceKey="btnEdit" />
	<asp:LinkButton ID="btnCancel" runat="server" Text="Close" CssClass="btn command cancel" OnCommand="OnCancelCommand" Visible="false" meta:resourceKey="btnCancel" />
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">
	<h1><% foreach (N2.ContentItem node in Ancestors) { %>/<a href="<%= Url.Parse(node is N2.Definitions.IFileSystemDirectory ? "Directory.aspx" : "File.aspx").AppendSelection(node) %>"><%= node.Title %></a><% } %></h1>

	<div class="tabPanel" data-flag="Unclosable">
	<a href="<%= SelectedItem.Url %>">
		<img src="<%= N2.Web.Url.ToAbsolute(Selection.SelectedItem.IconUrl) %>" alt="icon" />
		<%= SelectedItem.Title %>
		(<%= GetFileSize(SelectedFile.Size) %>)
	</a>

	<div class="fileContents">
		<asp:TextBox ID="txtContent" runat="server" CssClass="fileContents" Visible="false" TextMode="MultiLine" />
		<edit:ResizedImage MaxHeight="200" MaxWidth="300" ImageUrl="<%# SelectedFile.LocalUrl %>" runat="server" Hash="<%# SelectedFile.Updated.ToString() %>" />
	</div>
	</div>

	<script>
		$(function () {
			$('body').removeClass('toolbar-hidden');
		});
	</script>
</asp:Content>
