<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%if (Model != null) {
	var segments = Model.Split('|');
	using (Html.Content().BeginScope(segments.Length > 1 ? segments[1] : null))
	{ %>
		<%= Html.Content().Current.Item[segments[0]]%>
	<% }
} %>