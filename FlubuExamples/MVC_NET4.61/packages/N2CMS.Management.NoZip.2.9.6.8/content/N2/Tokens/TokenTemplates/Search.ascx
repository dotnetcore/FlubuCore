<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="N2.Persistence.Search" %>
<% var content = Html.Content(); %>
<% var searcher = content.Services.Resolve<ITextSearcher>(); %>
<% var results = searcher.Search(Query.For(Model).Below(content.Traverse.StartPage).Pages(true).Except(Query.For(typeof(N2.Definitions.ISystemNode)))); %>
<% if(results.Hits.Any()) { %>
<ul>
	<% foreach (var item in results.Hits) { %>
	<li><span><%= Html.Link(item.Content) %></span></li>
	<% } %>
</ul>
<% } %>