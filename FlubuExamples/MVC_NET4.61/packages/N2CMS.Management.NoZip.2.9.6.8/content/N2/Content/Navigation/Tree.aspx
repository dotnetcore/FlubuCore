<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tree.aspx.cs" Inherits="N2.Edit.Navigation.Tree" meta:resourceKey="treePage" %>
<%@ Import Namespace="N2.Resources" %>
<%@ Register TagPrefix="edit" TagName="ContextMenu" Src="ContextMenu.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" class="navigation <%= Server.HtmlEncode(Request.QueryString["location"])%>Location">
    <head runat="server">
        <title>Navigation</title>
        <asp:PlaceHolder runat="server">
		<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapCssPath) %>" type="text/css" rel="stylesheet" />
		<link rel="stylesheet" type="text/css" href="<%= MapCssUrl("all.css")%>" />
	    <link rel="stylesheet" type="text/css" href="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.IconsCssPath) %>" />
		<link rel="stylesheet" type="text/css" href="<%= MapCssUrl("framed.css")%>" />
		<link rel="stylesheet" type="text/css" href="<%= MapCssUrl("tree.css")%>" />
		</asp:PlaceHolder>
		<script src="../../Resources/Js/ContextMenu.js?v2" type="text/javascript" ></script>
		<script src="Tree.js" type="text/javascript"></script>
    </head>
<body class="tree framed noneSelected noPermission">
	<form id="form1" runat="server">
		<div class="FileUpload framed-navbar navbar navbar-fixed-bottom">
			<div class="navbar-inner">
					<b class="fa fa-upload"></b>
				<div class="btn-group">
					<input id="inputLocation" type="hidden" runat="server" class="uploadDirectoryLocation" />
					<input id="inputFile" type="file" runat="server" onchange="this.form.submit();" />
				</div>
			</div>
		</div>
        <div id="nav" class="tree nav focusGroup">
            <edit:Tree ID="siteTreeView" runat="server" Target="preview" />
        </div>

		<%
			bool isTinyPopup = Request["destinationType"] != null;
			bool isSelection = Request["location"] == "filesselection" || Request["location"] == "contentselection" || Request["location"] == "selection";
			bool isFilesSelection = Request["location"] == "filesselection";
			bool isContentSelection = Request["location"] == "contentselection";
			bool isFilesNavigation = Request["location"] == "files";
			bool isContentNavigation = Request["location"] == "content" || string.IsNullOrEmpty(Request["location"]);
			bool isNavigation = isFilesNavigation || isContentNavigation;
		%>

<%--		<% if(isTinyPopup) {%>
        <script src="../../Resources/tiny_mce/tiny_mce_popup.js" type="text/javascript"></script>
		<%} %>--%>
        
        <% if (isSelection)
		   { %>
        <script type="text/javascript">
			// selection
        	var updateOpenerWithUrlAndClose = function(relativeUrl) {
        		function selectIn(opener) {
        			var tbid = '<%= HttpUtility.JavaScriptStringEncode(Request["tbid"])%>';
        			if (tbid) {
        				opener.document.getElementById(tbid).value = relativeUrl;
        			} else {
        				window.opener.CKEDITOR.tools.callFunction(<%= int.Parse(Request["CKEditorFuncNum"] ?? "0")%>, relativeUrl);
        			}
        			//if (opener.onFileSelected && opener.srcField)
        			//	opener.onFileSelected(relativeUrl);
        			//else
        			//	opener.document.getElementById('<%= Request["tbid"]%>').value = relativeUrl;
        		}
        		$.cookie('lastSelection', relativeUrl);
        		if (window.opener) {
        			selectIn(window.opener);
        			window.close();
        		} 
        		//else if (typeof tinyMCEPopup != "undefined" && tinyMCEPopup.getWin()) {
        		//	selectIn(tinyMCEPopup.getWin());
        		//	tinyMCEPopup.close();
        		//}
        	}
        	var updateOpenerAndClose = function(e) {
        		var relativeUrl = $(this).attr("data-url");
        		updateOpenerWithUrlAndClose(relativeUrl);
        		e.preventDefault();
        	};
        	n2nav.handlers["fallback"] = updateOpenerAndClose;
        </script>
        <% } %>
		
        <script type="text/javascript">
		<% if (isFilesSelection)
	 { %>
			// file selection
        	n2nav.handlers["fallback"] = function(e) {
        		e.preventDefault();
        		if ($(this).attr("data-selectable") == "true")
        			updateOpenerAndClose.call(this, e);
        		else if ($(this).attr("data-type") == "Directory")
        			$(".uploadDirectoryLocation").attr("value", $(this).attr("data-url"));
        	};
    	<% } %>
		
		<% if (isContentSelection)
	 { %>
			// content selection
			n2nav.handlers["fallback"] = function(e) {
				e.preventDefault();
				if ($(this).attr("data-id") != "0")
					updateOpenerAndClose.call(this, e);
			};
    	<% } %>
		
		<% if (isFilesNavigation)
	 { %>
			// file navigation
        	var fallback = n2nav.handlers["fallback"];
        	n2nav.handlers["fallback"] = function(e) {
        		var type = $(this).attr("data-type");
        		if (type == "File" || type == "Directory" || type == "RootDirectory")
        			fallback.call(this, e);
    			else
    				e.preventDefault();
    		};
    		$("a.selected").focus();
    	<% } %>
        </script>

		<% if (isNavigation)
	 { %>
        <edit:ContextMenu id="cm" runat="server" />
    	<% } %>
    </form>
</body>
</html>
