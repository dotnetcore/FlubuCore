<%@ Page MasterPageFile="../Top.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Management.Files.Default" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
	<edit:ToolbarPluginDisplay ID="NavigationPlugins" Area="Navigation" runat="server" />
</asp:Content>

<asp:Content ID="cs" ContentPlaceHolderID="Subbar" runat="server">
</asp:Content>

<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<div id="leftPane" class="ui-layout-pane ui-layout-west">
	    <edit:ToolbarPluginDisplay ID="OperationsPlugins" Area="Operations" runat="server" />
		<iframe id="navigationFrame" src="../Content/Navigation/Tree.aspx?location=files&root=/&<%= N2.Edit.SelectionUtility.SelectedQueryKey %>=<%= Server.UrlEncode(Request[N2.Edit.SelectionUtility.SelectedQueryKey]) %>" frameborder="0" name="navigation" class="frame"></iframe>
	</div>

	<div id="rightPane" class="ui-layout-pane ui-layout-center">
		<edit:ToolbarPluginDisplay ID="FilesPlugins" Area="Files" runat="server" />
		<iframe id="previewFrame" src="../Empty.aspx" frameborder="0" name="preview" class="frame"></iframe>
	</div>
	
	<script type="text/javascript">
		window.name = "top";
		n2ctx.hasTop = function() { return "legacy"; }
		n2ctx.initToolbar();
		n2ctx.update({ path: '<%= Selection.SelectedItem.Path %>', previewUrl: '<%= ResolveClientUrl(Selection.SelectedItem.Url) %>' });
		n2ctx.location = "files";
		
		jQuery(document).ready(function() {
			n2.layout.init();
			jQuery(".command").n2glow();
			jQuery(".operations a").click(function(e) {
				if (jQuery(document.body).is(".editSelected, .wizardSelected, .versionsSelected, .securitySelected, .exportimportSelected, .globalizationSelected, .linktrackerSelected, .usersSelected")) {
					e.preventDefault();
				};
			});
		});
    </script>
</asp:Content>
