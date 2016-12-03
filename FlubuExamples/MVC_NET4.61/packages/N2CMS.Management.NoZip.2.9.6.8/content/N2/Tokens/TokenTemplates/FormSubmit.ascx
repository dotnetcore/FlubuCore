<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="N2.Web.Mvc.Html" %>

<% if (string.IsNullOrEmpty(Model)) { %>
<input type="submit" />
<% } else { %>
<input type="submit" value="<%= Model %>"/>
<% } %>
