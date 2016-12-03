<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="N2.Web.Rendering" %>
<%@ Import Namespace="N2.Web.Mvc.Html" %>
<%
	DisplayableToken token = Html.DisplayableToken();
	string[] components = token.GetComponents();
	string[] options = (components.Length > 1 ? components[1] : components[0]).Split(',');
	string name = token.GetOptionalInputName(0, 1);
%>
<span class="formfield formselect">
<select name="<%= name %>">
	<% foreach(var opt in options) { %>
	<option><%= opt %></option>
	<% } %>
</select>
</span>
