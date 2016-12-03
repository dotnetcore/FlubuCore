<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens("{ManagementUrl}/Resources/SyntaxHighlighter/Scripts/shCore.js") %>"></script>
<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens("{ManagementUrl}/Resources/SyntaxHighlighter/Scripts/shBrushCSharp.js") %>"></script>
<link type="text/css" rel="stylesheet" href="<%= N2.Web.Url.ResolveTokens("{ManagementUrl}/Resources/SyntaxHighlighter/Styles/shCoreDefault.css") %>"/>
<script type="text/javascript">SyntaxHighlighter.all();</script>
