<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var content = Html.Content(); %>
<% using (content.BeginScope(Model)) {
	var children = content.Traverse.Children(content.Current.Item, content.Is.Page());
	if(children.Any()) { 
		foreach(var item in children) {%>
			<span><%= Html.Link(item) %></span>
		<% } 
	}
} %>