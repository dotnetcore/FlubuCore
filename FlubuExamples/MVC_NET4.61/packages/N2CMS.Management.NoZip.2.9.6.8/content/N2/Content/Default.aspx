<%@ Page MasterPageFile="../Top.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Default" Title="Edit" meta:resourcekey="DefaultResource" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
		<edit:ToolbarPluginDisplay ID="NavigationPlugins" Area="Navigation" runat="server" />
	<edit:ToolbarPluginDisplay ID="OptionsPlugins" Area="Options" runat="server" />
</asp:Content>

<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<div id="leftPane" class="ui-layout-pane ui-layout-west">
		<div id="navTools" class="contentTools"><edit:ToolbarPluginDisplay ID="OperationsPlugins" Area="Operations" runat="server" /></div>
		<iframe id="navigationFrame" src="<%= GetNavigationUrl(Selection.SelectedItem) %>" frameborder="0" name="navigation" class="frame"></iframe>
	</div>

	<div id="rightPane" class="ui-layout-pane ui-layout-center">
		<div id="pageTools" class="contentTools"><edit:ToolbarPluginDisplay ID="PagePlugins" Area="Preview" runat="server" /></div>
		<iframe id="previewFrame" src="<%= GetPreviewUrl(Selection.SelectedItem) %>" frameborder="0" name="preview" class="frame"></iframe>
	</div>

	<script type="text/javascript">
		window.name = "top";
		n2ctx.hasTop = function() { return "legacy"; }
		n2ctx.location = "content";

		jQuery(document).ready(function () {
			n2ctx.initToolbar();
			n2ctx.update({ path: '<%= SelectedPath %>', previewUrl: '<%= ResolveClientUrl(SelectedUrl) %>' });
			n2.layout.init();

			function resizeFrames() {
				$("iframe").css("height", ($(window).height() - $("#permission").height() - $("#pageTools").height() - 1) + "px");
			}
			resizeFrames();
			$(window).resize(resizeFrames);
		});
		</script>
</asp:Content>
