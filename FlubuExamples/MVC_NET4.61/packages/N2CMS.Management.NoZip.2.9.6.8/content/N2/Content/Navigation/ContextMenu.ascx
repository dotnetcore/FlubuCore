<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContextMenu.ascx.cs" Inherits="N2.Edit.Navigation.ContextMenu" %>
<div id="contextMenu" class="focusGroup ui-menu">
	<div id="permission">
		<asp:PlaceHolder ID="plhMenuItems" runat="server" />
	</div>
</div>

<script type="text/javascript">
//<![CDATA[
	(function ($) {
		var targetSelector = "a[target=preview]";
		var handler = function (e) {
			$(e.target).closest(targetSelector).each(function (e) {
				var options = {
					path: $(this).attr("data-path"),
					previewUrl: this.href,
					permission: $(this).attr("data-permission")
				};
				n2nav.setupToolbar(options);
				$("#permission").attr("class", options.permission);
			});
		};
		jQuery(document).ready(function () {
			$("#nav").click(handler)
				.focus(handler)
				.bind("contextmenu", handler)
				.n2contextmenu("#contextMenu", { target: targetSelector });
		});
	})(jQuery);
//]]>
</script>
