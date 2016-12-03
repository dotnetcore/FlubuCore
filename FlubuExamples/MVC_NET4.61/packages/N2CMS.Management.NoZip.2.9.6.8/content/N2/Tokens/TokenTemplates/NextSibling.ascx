<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var content = Html.Content(); %>
<% using (content.BeginScope(Model)) {
	N2.ContentItem next = content.Traverse.Children(content.Traverse.Parent(), content.Is.AccessiblePage())
		.SkipWhile(i => i != content.Current.Item)
		.Skip(1)
		.FirstOrDefault(); %>
	<%= Html.Link(next) %>
<% } %>	