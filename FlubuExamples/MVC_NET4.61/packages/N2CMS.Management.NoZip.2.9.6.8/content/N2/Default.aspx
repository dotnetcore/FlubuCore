<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<!DOCTYPE html>
<!--[if lt IE 7]>      <html class="no-js lt-ie9 lt-ie8 lt-ie7"> <![endif]-->
<!--[if IE 7]>         <html class="no-js lt-ie9 lt-ie8"> <![endif]-->
<!--[if IE 8]>         <html class="no-js lt-ie9"> <![endif]-->
<!--[if gt IE 8]><!-->
<html class="no-js">
<!--<![endif]-->
<head>
	<meta charset="utf-8">
	<meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
	<title>N2 Management</title>
	<meta name="viewport" content="width=device-width">

	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.JQueryJsPath)%>"></script>
	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.JQueryUiPath)%>"></script>
	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularJsPath)%>"></script>
	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularJsResourcePath) %>"></script>
	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularJsSanitizePath)%>"></script>
	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularJsRoot + "angular-route.js")%>"></script>
	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularUiJsPath)%>"></script>
	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularStrapJsPath)%>"></script>

	<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapCssPath)%>" type="text/css" rel="stylesheet" />
	<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapJsPath)%>" type="text/javascript"></script>

	<link rel="stylesheet" href="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.IconsCssPath)%>" />

	<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapDatePickerCssPath)%>" type="text/css" rel="stylesheet" />
	<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapDatePickerJsPath)%>" type="text/javascript"></script>
	<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapTimePickerCssPath)%>" type="text/css" rel="stylesheet" />
	<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapTimePickerJsPath)%>" type="text/javascript"></script>
	

	<link href="Resources/icons/flags.css" rel="stylesheet" />

	<!--<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularStrapJsPath)%>"></script>-->

	<script src="Resources/js/n2.js"></script>
	<link rel="stylesheet" href="Resources/css/n2.css">

	<script src="<%= GetLocalizationPath()%>"></script>
	<script src="App/Js/Services.js"></script>
	<script src="App/Js/Controllers.js"></script>
	<script src="App/Js/Directives.js"></script>

	<asp:PlaceHolder runat="server">
	<% foreach (var module in N2.Context.Current.Container.ResolveAll<N2.Management.Api.ManagementModuleBase>())
	{ %>
	<!-- <%= module.GetType().Name%> -->
	<% foreach (var script in module.ScriptIncludes)
	{ %>
	<script src="<%= N2.Web.Url.ResolveTokens(script)%>"></script>
	<% } %>
	<% foreach (var style in module.StyleIncludes)
	{ %>
	<link href="<%= N2.Web.Url.ResolveTokens(style)%>" rel="stylesheet" />
	<% } %>
	<% } %>
	</asp:PlaceHolder>
	<base href="/<%= Request.Url.Segments[1] %>" />
</head>
<body ng-app="n2" ng-view>
</body>
</html>

<script runat="server">

	protected string GetLocalizationPath()
	{
		var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
		var languagePreferenceList = new[] { culture.ToString(), culture.TwoLetterISOLanguageName };
		foreach (var languageCode in languagePreferenceList)
		{
			var path = N2.Web.Url.ResolveTokens("{ManagementUrl}/App/i18n/" + languageCode + ".js.ashx");
			if (System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(path))
				return path;
		}
		return "App/i18n/en.js.ashx";
	}
</script>
