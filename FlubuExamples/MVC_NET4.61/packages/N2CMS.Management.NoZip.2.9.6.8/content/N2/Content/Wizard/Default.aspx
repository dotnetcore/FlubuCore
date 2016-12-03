<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Wizard.Default" Title="Wizard" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Import namespace="N2.Edit.Wizard.Items"%>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
	<edit:CancelLink ID="hlCancel" runat="server" CssClass="btn cancel command" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<edit:PersistentOnlyPanel runat="server" meta:resourceKey="popNotSupported">
		<n2:tabpanel ID="tpType" runat="server" ToolTip="Select type" CssClass="tabPanel" meta:resourcekey="tpTypeResource1" RegisterTabCss="False">
			<asp:GridView ID="gvLocations" runat="server" BorderWidth="0"
			OnRowDeleting="gvLocations_OnRowDeleting" DataKeyNames="ID"
			CssClass="table table-striped table-hover table-condensed" AutoGenerateColumns="false" ShowHeader="false">
			<Columns>
				<asp:TemplateField>
					<ItemTemplate>
						<asp:HyperLink ID="hlNew" NavigateUrl='<%# GetEditUrl((MagicLocation)Container.DataItem) %>' ToolTip='<%# Eval("ToolTip") %>' runat="server">
							<asp:Image ID="imgIco" ImageUrl='<%# Eval("IconUrl") %>' CssClass="icon" runat="server" meta:resourcekey="imgIcoResource1" />
							<%# Eval("Title") %>
						</asp:HyperLink>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:BoundField DataField="Description" />
				<asp:TemplateField>
					<ItemTemplate>
						<asp:HyperLink ID="hlNew" NavigateUrl='<%# Eval("Location.Url") %>' Visible='<%# Eval("HasLocation") %>' runat="server">
							<asp:Image ID="imgIco" ImageUrl='<%# Engine.ManagementPaths.ResolveResourceUrl((string)Eval("Location.IconUrl")) %>' Visible='<%# !string.IsNullOrEmpty((string)Eval("Location.IconUrl")) %>' CssClass="icon" runat="server" meta:resourcekey="imgIcoResource1" />
							<%# Eval("Location.Title")%>
						</asp:HyperLink>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:ButtonField Text="Delete" CommandName="Delete" />
			</Columns>
			<EmptyDataTemplate>
				<asp:Literal ID="ltWizards" Text="No wizards configured" runat="server" meta:resourceKey="ltWizards" />
			</EmptyDataTemplate>
        </asp:GridView>
		</n2:tabpanel>
		<n2:tabpanel ID="tpAdd" runat="server" ToolTip="Add location" CssClass="tabPanel" meta:resourcekey="tpAddResource1" RegisterTabCss="False">
			<edit:PermissionPanel id="ppAdd" RequiredPermission="Publish" runat="server" meta:resourceKey="ppPermitted">
			<asp:MultiView ID="mvAdd" runat="server" ActiveViewIndex="0">
            <asp:View runat="server">
                <div class="cf">
                    <asp:Label ID="lblLocation" runat="server" Text="Location" AssociatedControlID="lblLocationTitle" meta:resourcekey="lblLocation" />
                    <strong><asp:Label ID="lblLocationTitle" runat="server" CssClass="title"
						  Text=<%# Selection.SelectedItem.Title %>/></strong>
                </div>
                <div class="cf">
                    <asp:Label runat="server" Text="Title" AssociatedControlID="txtTitle" 
						meta:resourcekey="LabelResource1" />
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="title"
						meta:resourcekey="txtTitleResource1" />
                </div>
                <div class="cf">
                    <asp:Label runat="server" Text="Type" AssociatedControlID="ddlTypes" 
						meta:resourcekey="LabelResource2" />
                    <asp:DropDownList ID="ddlTypes" runat="server" DataTextField="Title" CssClass="types"
						DataValueField="Value" meta:resourcekey="ddlTypesResource1" />
                </div>
                <div class="cf">
                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnCommand="btnAdd_Command" 
						meta:resourcekey="btnAddResource1" />
                </div>
                <script type="text/javascript">
                	var append = function() {
                		var $t = $("input.title");
                		var selected = " - " + this.options[this.selectedIndex].text.replace(/^\W+/, "");
                		var last = $t.data("type") || "";
                		var value = $t.attr("value");
                		var index = value.lastIndexOf(selected);
                		if (index < 0)
                			index = value.lastIndexOf(last);
                		if (index > 0) {
                			$t.attr("value", value.substr(0, index) + selected);
                			$t.data("type", selected);
                		}
                	};
                	$(".types").change(append).each(append);
                </script>
            </asp:View>
            <asp:View runat="server">
                <asp:Label runat="server" Text="Added" meta:resourcekey="LabelResource3" />
            </asp:View>
        </asp:MultiView>
			</edit:PermissionPanel>
		</n2:tabpanel>
	</edit:PersistentOnlyPanel>
</asp:Content>
