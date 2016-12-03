<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentActivity.ascx.cs" Inherits="N2.Management.Content.Activity.RecentActivity" %>
<%@ Register TagPrefix="n2" Namespace="N2.Web.UI.WebControls" Assembly="N2" %>
<%@ Import Namespace="N2.Web" %>
<%--<n2:Box ID="boxActivity" HeadingText="Recent Activity" CssClass="box activityBox" runat="server" Visible="<%# ShowActivities %>" meta:resourceKey="boxActivity">
</n2:Box>
<asp:PlaceHolder runat="server" ID="activityTemplatePlaceholder">
<script type="text/template" id="activityTemplate">
	<table class="table table-striped table-hover table-condensed">
		<thead>
			<tr><td><%= GetLocalResourceString("bfOperation.HeaderText", "Operation") %></td><td><%= GetLocalResourceString("bfBy.HeaderText", "By") %></td><td></td></tr>
		</thead>
		<tbody>
		{{#Activities}}
			<tr>
				<td>{{Operation}}</td>
				<td>{{PerformedBy}}</td>
				<td>{{AddedDate}}</td>
			</tr>
		{{/Activities}}
		</tbody>
	</table>
</script>
<script type="text/javascript">
	var activityContainer = "#<%= boxActivity.ClientID %> .box-inner";
	var activities = <%= ActivitiesJson %>;
	$(function () {
		var template = Hogan.compile($("#activityTemplate").html());
		$(activityContainer).html(template.render(activities));

		var isDirty = false;
		$(document).keyup(function () {
			isDirty = true;
		});
		setInterval(function () {
			if (!$(activityContainer).is(":visible"))
				return;
			$.ajax({
				method: "POST",
				url: "<%= N2.Web.Url.ResolveTokens("{ManagementUrl}/Collaboration/Ping.ashx") %>",
			dataType: 'json',
			data: { selected: n2ctx.selectedPath, activity: "Edit", dirty: isDirty },
			success: function (data) {
				if (data.LastChange !== activities.LastChange)
					$(activityContainer).html(template.render(data));
			}
		});
		isDirty = false;
	}, 60000);
});
</script>
</asp:PlaceHolder>

<pre class="alert alert-error" id="errorDisplay" runat="server" Visible="False">
	<asp:Literal runat="server" ID="errorDisplayText" />
</pre>--%>
