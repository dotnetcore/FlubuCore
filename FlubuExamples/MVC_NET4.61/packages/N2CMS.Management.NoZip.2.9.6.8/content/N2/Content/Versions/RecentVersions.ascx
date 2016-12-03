<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentVersions.ascx.cs" Inherits="N2.Management.Content.Versions.RecentVersions" %>
<%@ Register TagPrefix="n2" Namespace="N2.Web.UI.WebControls" Assembly="N2" %>
<n2:Box ID="boxVersions" HeadingText="Recent Versions" CssClass="box versionBox" runat="server" meta:resourceKey="boxVersions">
	<table class="table table-striped table-hover table-condensed">
		<thead>
			<tr><td><%= GetLocalResourceString("bfVersion.HeaderText", "Version")%></td><td><%= GetLocalResourceString("bfSavedBy.HeaderText", "Saved by")%></td><td></td></tr>
		</thead>
		<tbody>
		<% foreach(var version in Versions){ %>
			<tr class="<%= version.ID == CurrentItem.ID ? "current" : "" %>">
				<td title="<%= version.State %>" class="<%= version.State %> State"><%= version.VersionIndex %></td>
				<td><%= version.SavedBy %></td>
				<td><%= version.Info %></td>
			</tr>
		<% } %>
		</tbody>
	</table>

	<asp:HyperLink ID="hlMoreVersions" CssClass="moreVersions" NavigateUrl="<%# VersionsUrl %>" Text="More versions &raquo;" Visible="<%# ShowMoreVersions %>" runat="server" meta:resourceKey="hlMoreVersions"/>
</n2:Box>
