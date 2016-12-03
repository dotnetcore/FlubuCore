<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Globalization._Default" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="lang" TagName="Languages" Src="Languages.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Import Namespace="N2.Web" %>
<%@ Import Namespace="System.Linq" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
		<link rel="stylesheet" href="../../Resources/Css/Globalization.css" type="text/css" />
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
		<edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
		<asp:CustomValidator Text="Globalization is not enabled." ID="cvGlobalizationDisabled" meta:resourceKey="cvGlobalizationDisabled" Display="Dynamic" CssClass="alert alert-margin" runat="server" />
		<asp:CustomValidator Text="This page cannot be translated." ID="cvOutsideGlobalization" meta:resourceKey="cvOutsideGlobalization" Display="Dynamic" CssClass="alert alert-margin" runat="server" />
	<asp:CustomValidator Text="Select at least two items to associate." ID="cvAssociate" meta:resourceKey="cvAssociate" runat="server" CssClass="alert alert-margin" Display="Dynamic" />
	<asp:CustomValidator Text="Couldn't enable globalization. Please configure it manually in the web configuration." ID="cvEnable" meta:resourceKey="cvEnable" runat="server" CssClass="alert alert-error alert-margin" Display="Dynamic" />
	<asp:CustomValidator Text="No language roots available. Please add one or more start pages and set the language on the site tab." ID="cvLanguageRoots" meta:resourceKey="cvLanguageRoots" runat="server" CssClass="alert alert-margin" Display="Dynamic" />
	<asp:CustomValidator Text="Cannot associate language roots. They are assumed to be translations of each other." ID="cvAssociateLanguageRoots" meta:resourceKey="cvAssociateLanguageRoots" runat="server" CssClass="alert alert-margin" Display="Dynamic" />
	
	<asp:Panel ID="pnlLanguages" runat="server" CssClass="languages">
		<table class="table table-striped table-hover table-condensed">
				<thead>
					<asp:Repeater runat="server" DataSource='<%# GetTranslations(Selection.SelectedItem) %>'>
						<HeaderTemplate><tr class="th"><td></td></HeaderTemplate>
						<ItemTemplate>
							<th title='<%# Eval("Language.LanguageCode") %>'><span class="<%# Eval("Language.LanguageCode").ToString().Split('-').Last().ToLower() %> sprite"></span></th>
						</ItemTemplate>	
						<FooterTemplate></tr></FooterTemplate>
					</asp:Repeater>

					<tr class="selected">
							<td>
						<% if (Selection.SelectedItem.Parent != null){ %>
							<a href="Default.aspx?<%# N2.Edit.SelectionUtility.SelectedQueryKey %>=<%# Selection.SelectedItem.Parent.Path %>"><img src="../../Resources/icons/bullet_toggle_minus.png" class="up" /></a>
						<% } %>
					</td>
							<lang:Languages runat="server" DataSource='<%# GetTranslations(Selection.SelectedItem) %>' />
								</tr>
				</thead>
			<asp:Repeater runat="server" DataSource="<%# GetChildren(true) %>">
				<HeaderTemplate><tbody></HeaderTemplate>
				<ItemTemplate>
					<tr class="i<%# Container.ItemIndex %>">
							<td>
							<asp:HyperLink runat="server" Visible="<%# ((N2.ContentItem)Container.DataItem).GetChildren().Count > 0 %>" href='<%# "Default.aspx?" + N2.Edit.SelectionUtility.SelectedQueryKey + "=" + Eval("Path") %>'><img src="../../Resources/icons/bullet_toggle_plus.png" class="down" /></asp:HyperLink>
						</td>
						<lang:Languages runat="server" DataSource='<%# GetTranslations((N2.ContentItem)Container.DataItem) %>' />
					</tr>
				</ItemTemplate>
				<FooterTemplate></tbody></FooterTemplate>
			</asp:Repeater>
			<asp:Repeater runat="server" DataSource="<%# GetChildren(false) %>">
					<HeaderTemplate><tbody></HeaderTemplate>
				<ItemTemplate>
					<tr class="<%# Container.ItemIndex % 2 == 1 ? "alt" : "" %> i<%# Container.ItemIndex %>">
							<td>
							<asp:HyperLink runat="server" Visible="<%# ((N2.ContentItem)Container.DataItem).GetChildren().Count > 0 %>" href='<%# Eval("Path", "Default.aspx?" + N2.Edit.SelectionUtility.SelectedQueryKey + "={0}") %>'><img src="../../Resources/icons/bullet_toggle_plus.png" class="down" /></asp:HyperLink>
						</td>
						<lang:Languages runat="server" DataSource='<%# GetTranslations((N2.ContentItem)Container.DataItem) %>' />
					</tr>
				</ItemTemplate>
				<FooterTemplate></tbody></FooterTemplate>
			</asp:Repeater>
		</table>
		<div class="panel">
		<asp:Label runat="server" Text="Selected: " id="lblSelected" meta:resourceKey="lblSelected" />
		<asp:Button ID="btnAssociate" runat="server" Text="Associate" OnClick="btnAssociate_Click" meta:resourceKey="btnAssociate" CssClass="btn" />
		<asp:Button ID="btnUnassociate" runat="server" Text="Unassociate" OnClick="btnUnassociate_Click" meta:resourceKey="btnUnassociate" CssClass="btn" />
		</div>
	</asp:Panel>
	<asp:Button ID="btnEnable" runat="server" Text="Enable Globalization" OnClick="btnEnable_Click" meta:resourceKey="btnEnable" CssClass="btn" />
</asp:Content>
