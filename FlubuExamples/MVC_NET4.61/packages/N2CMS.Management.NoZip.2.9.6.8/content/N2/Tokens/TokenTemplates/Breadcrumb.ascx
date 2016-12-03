<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% bool subsequent = false; %>
<% var content = Html.Content(); %>
<% foreach(var item in content.Traverse.AncestorsBetween(0, 10)) { 
	if(subsequent) { %><span class="separator"> / </span><% } else { subsequent = true; } %>
<%= Html.Link(item).Class(item == content.Current.Item ? "current" : "crumb") %>
<% } %>