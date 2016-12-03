<%@ Page Language="C#" MasterPageFile="../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Crop.aspx.cs" Inherits="N2.Management.Files.Crop" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary command save primary-action" OnCommand="OnSaveCommand" meta:resourceKey="btnSave" />
    <edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlCancel">Close</edit:CancelLink>
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">
	<div class="resize">
		<img id="original" src="<%= originalImagePath %>" />
		<asp:Label runat="server" meta:resourceKey="lblResize" Text="Drag a rectangle to select new area for this image size" />
	</div>
	<input id="x" name="x" type="hidden" />
	<input id="y" name="y" type="hidden" />
	<input id="w" name="w" type="hidden" />
	<input id="h" name="h" type="hidden" />
	<script>
		jQuery(function ($) {
			var max = {w:<%= size.Width %>, h: <%= size.Height %>};
			$("#original").Jcrop($.extend(<%= settings %>, {
				onChange: function(e){
					for(var k in e)
						$("#" + k).val(e[k]);
					if(e.w && e.h)
						$(".save").removeAttr("disabled");
					else
						$(".save").attr("disabled", "disabled");

					if(e.w < max.w) {
						$(document.body).addClass("resize-warning");
					} else {
						$(document.body).removeClass("resize-warning");
					}
				}
			}));
			$(document).click(function(){
				$(".resize span").fadeOut();
				$(document).unbind("click");
			});
			$(".save").attr("disabled", "disabled");
		});
	</script>
</asp:Content>
