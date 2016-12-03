<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Management.Content.Templates.Default" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
	<edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
<style>
	fieldset div.container { max-height:120px; overflow:auto; margin-bottom:20px; }
</style>
	<edit:PermissionPanel id="ppPermitted" runat="server" meta:resourceKey="ppPermitted">
    <n2:tabpanel ID="tpTemplates" runat="server" ToolTip="Templates" CssClass="tabPanel" meta:resourcekey="tpTemplates" RegisterTabCss="False">
        <asp:GridView id="gvTemplates" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-hover table-condensed" ShowHeader="false"
			DataKeyNames="Name" OnRowDeleting="gvTemplates_OnRowDeleting" BorderWidth="0"
			DataSource="<%# Templates.GetAllTemplates() %>" 
			meta:resourcekey="gvTemplatesResource1">
			<Columns>
				<asp:TemplateField HeaderText="Template" meta:resourcekey="TemplateFieldResource1"><ItemTemplate>
					<asp:HyperLink runat="server" NavigateUrl='<%# Eval("TemplateUrl") %>'>
						<asp:Image runat="server" ImageUrl='<%# Eval("Template.IconUrl") %>' />
						<%# Eval("Title") %>
					</asp:HyperLink>
				</ItemTemplate></asp:TemplateField>
				<asp:BoundField DataField="Description" />
				
				<asp:TemplateField HeaderText="Template" meta:resourcekey="TemplateFieldResource1"><ItemTemplate>
					<asp:HyperLink runat="server" NavigateUrl='<%# Edits.GetEditExistingItemUrl((N2.ContentItem)Eval("Original")) + "&returnUrl=" + Request.RawUrl %>' Text="Edit" meta:resourceKey="hlEdit" />
				</ItemTemplate></asp:TemplateField>
				<asp:ButtonField CommandName="Delete" Text="Delete" meta:resourcekey="ButtonFieldResource1" />
			</Columns>
			<EmptyDataTemplate><em><asp:Label runat="server" 
					Text="Add the selected page as template on the 'Add template' tab." 
					meta:resourcekey="LabelResource1" /></em></EmptyDataTemplate>
        </asp:GridView>
    </n2:tabpanel>
    <n2:tabpanel ID="tpAdd" runat="server" ToolTip="Add template" CssClass="tabPanel" meta:resourcekey="tpAdd" RegisterTabCss="False">
		<edit:PermissionPanel id="ppAdd" RequiredPermission="Publish" runat="server" meta:resourceKey="ppPermitted">
		<table>
		<tr><td>
			<asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle"
				Text="Title" meta:resourcekey="lblTitleResource1" />
		</td><td colspan="1">
			<asp:TextBox ID="txtTitle" runat="server" 
					Text='<%# Definitions.GetDefinition(Selection.SelectedItem.GetContentType()).Title + " - " + Selection.SelectedItem.Title %>'
					meta:resourcekey="txtTitleResource1" />
		</td></tr>
		<tr><td>
			<asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription" 
				Text="Description" meta:resourcekey="lblDescription" />
		</td><td colspan="1">
			<asp:TextBox ID="txtDescription" runat="server" meta:resourcekey="txtDescription" />
		</td></tr>
		<tr><td>
			Include in template
		</td><td>
			<n2:Repeater ID="rptChildren" runat="server" DataSource="<%# Selection.SelectedItem.GetChildren() %>">
				<EmptyTemplate><em><asp:Literal runat="server" Text="No children" id="ltNoChildren" meta:resourceKey="ltNoChildren" /></em></EmptyTemplate>
				<HeaderTemplate>
					<fieldset>
						<legend><asp:Literal runat="server" Text="Include children (and their descendants)" id="ltChildren" meta:resourceKey="ltChildren" /></legend>
						<div class="container">
				</HeaderTemplate>
				<ItemTemplate>
					<div>
						<asp:CheckBox ID="chkChildren" runat="server" 
							Checked='<%# !((bool)Eval("IsPage")) %>' 
							meta:resourcekey="chkChildrenResource1" 
							Text='<%# string.IsNullOrEmpty((string)Eval("Title")) ? ( "[" + ((N2.ContentItem)Container.DataItem).GetContentType().Name + "]") : Eval("Title") %>' />
					</div>
				</ItemTemplate>
				<FooterTemplate>
					</div>
					</fieldset>
				</FooterTemplate>
			</n2:Repeater>
		</td></tr></table>
		<asp:Button runat="server" Text="Add" ID="btnAdd" OnCommand="btnAdd_Command" 
			meta:resourcekey="btnAddResource1" />
		</edit:PermissionPanel>
    </n2:tabpanel>
	</edit:PermissionPanel>
</asp:Content>
