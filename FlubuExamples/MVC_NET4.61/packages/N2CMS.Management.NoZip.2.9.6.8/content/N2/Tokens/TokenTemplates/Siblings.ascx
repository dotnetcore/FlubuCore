<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var content = Html.Content(); %>
<% var current = content.Current.Page; %>
<ul>
<% foreach (var item in content.Traverse.Siblings(content.Is.Navigatable()))
   { %>
	<li><%= (item != current) ? Html.Link(item).ToString() : string.Format("<span>{0}</span>", item.Title)%></li>
<% } %>
</ul>