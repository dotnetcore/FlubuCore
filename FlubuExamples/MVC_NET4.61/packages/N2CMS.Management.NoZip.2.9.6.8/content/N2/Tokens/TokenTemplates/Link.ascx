<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var content = Html.Content(); %>

<% using (content.BeginScope(Model)) { %>
<%= Html.Link(content.Current.Item) %>
<% } %>
