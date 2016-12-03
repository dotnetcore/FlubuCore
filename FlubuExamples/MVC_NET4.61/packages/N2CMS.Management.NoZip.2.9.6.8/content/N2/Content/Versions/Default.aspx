<%@ Page MasterPageFile="..\Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Versions.Default" Title="Previous Versions" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
	<edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel" CssClass="btn">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
	<asp:CustomValidator ID="cvVersionable" runat="server" Text="This item is not versionable." CssClass="alert alert-margin" meta:resourceKey="cvVersionable" Display="Dynamic" />
	<edit:PermissionPanel id="ppPermitted" runat="server" meta:resourceKey="ppPermitted">

	<asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="false" DataKeyNames="VersionIndex" CssClass="table table-striped table-hover table-condensed" UseAccessibleHeader="true" BorderWidth="0" OnRowCommand="gvHistory_RowCommand" OnRowDeleting="gvHistory_RowDeleting">
		<Columns>
			<asp:TemplateField HeaderText="Version" meta:resourceKey="v" ItemStyle-CssClass="Version">
				<ItemTemplate>
					<span title='<%# Eval("State") %>'><%# ((N2.ContentItem)Eval("Content")).VersionIndex + 1%>. </span>
					<b class='<%# GetStateIconClass((N2.ContentState)Eval("State")) %>' title="<%# IsFuturePublished(Eval("Content")) ? ((N2.ContentItem)Eval("Content"))["FuturePublishDate"] : ""%>"></b>
					<asp:Literal runat="server" Text='<%# GetLocalResourceString("ContentState." + Eval("State"), Eval("State").ToString()) %>' />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Title" meta:resourceKey="title" >
				<ItemTemplate>
					<edit:ItemLink DataSource='<%# Eval("Content") %>' InterfaceUrl="../Edit.aspx" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:BoundField HeaderText="Published" DataField="Published" meta:resourceKey="published" />
			<asp:BoundField HeaderText="Expired" DataField="Expires" meta:resourceKey="expires" />
			<asp:BoundField HeaderText="Saved by" DataField="SavedBy" meta:resourceKey="savedBy" />
			<asp:BoundField HeaderText="Parts count" DataField="PartsCount" meta:resourceKey="partsCount" />
			<asp:TemplateField>
				<ItemTemplate>
					<asp:HyperLink runat="server" ID="hlEdit" meta:resourceKey="hlEdit" Text="Edit" NavigateUrl='<%# Engine.ManagementPaths.GetEditExistingItemUrl((N2.ContentItem)Eval("Content")) %>' />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField>
				<ItemTemplate>
					<asp:LinkButton runat="server" ID="btnPublish" meta:resourceKey="btnPublish" Text="Publish" CommandName="Publish" CommandArgument='<%# Eval("VersionIndex") %>' Visible='<%# IsVisible(Eval("Content")) || IsFuturePublished(Eval("Content")) %>' />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField>
				<ItemTemplate>
					<asp:LinkButton runat="server" ID="btnDelete" meta:resourceKey="btnDelete" Text="Delete" CommandName="Delete" CommandArgument='<%# Eval("VersionIndex") %>' Visible='<%# IsVisible(Eval("Content")) %>'
						OnClientClick="return confirm('Are you sure you want to Delete this version?');" />
				</ItemTemplate>
			</asp:TemplateField>

		</Columns>
	</asp:GridView>
	</edit:PermissionPanel>
</asp:Content>
