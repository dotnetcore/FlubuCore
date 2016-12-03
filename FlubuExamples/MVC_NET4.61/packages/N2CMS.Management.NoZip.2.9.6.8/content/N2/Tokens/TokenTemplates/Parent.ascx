<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% using (Html.Content().BeginScope(Model)) { %>
<%= Html.Link(Html.Content().Traverse.Parent())%>
<% } %>