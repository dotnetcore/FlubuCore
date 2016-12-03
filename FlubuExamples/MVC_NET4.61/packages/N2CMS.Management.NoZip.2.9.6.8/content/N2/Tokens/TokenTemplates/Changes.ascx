<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var changes = Html.Content().Search.Items
	   .Where(ci => ci.ZoneName == null)
	   .Where(ci => ci.State != N2.ContentState.Deleted)
	   .OrderByDescending(ci => ci.Updated)
	   .Take(10).ToList(); %>
<% if(changes.Any()) { %>
<ul>
<% foreach(var item in changes) { %>
<li><span><%= Html.Link(item) %></span></li>
<% } %>
</ul>
<% } %>