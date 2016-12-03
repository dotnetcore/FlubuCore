<%@ Page MasterPageFile="../Content/Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Root.aspx.cs" Inherits="N2.Management.Myself.Root" Title="Root" %>
<asp:Content ContentPlaceHolderID="Head" runat="server">
	<link href="<%= N2.Web.Url.ResolveTokens("{ManagementUrl}/Resources/Css/Root.css") %>"" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">
	<div id="home">
		<n2:ControlPanel ID="cp" runat="server" />
		<n2:Zone ID="ZoneAbove" ZoneName="Above" runat="server" />
		<table class="columns">
			<tr>
				<td class="column" runat="server" id="c1">
					<n2:DroppableZone ID="Zone2" ZoneName="Left" runat="server" />
				</td>
				<td class="column" runat="server" id="c2">
					<n2:DroppableZone ID="Zone3" ZoneName="Center" runat="server" />
				</td>
				<td class="column" runat="server" id="c3">
					<n2:DroppableZone ID="Zone4" ZoneName="Right" runat="server" />
				</td>
			</tr>
		</table>
		<n2:Zone ID="Zone5" ZoneName="Below" runat="server" />
	</div>
	<script type="text/javascript">
		function handleExternalForm(formId, placeholderId) {
			function place() {
				var $f = $("#" + formId);
				var $ph = $("#" + placeholderId);
				setTimeout(function () {
					var pos = $ph.offset();
					$f.css({ position: "absolute", top: pos.top + "px", left: pos.left + "px", width: $ph.width() + "px" });
					$ph.height($f.height());
				}, 10);
			};
			$(document).ready(place).click(place);
			$(window).resize(place);
		}
	</script>
</asp:Content>
