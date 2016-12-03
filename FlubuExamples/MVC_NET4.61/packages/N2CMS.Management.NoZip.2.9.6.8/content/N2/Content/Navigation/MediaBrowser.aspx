<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MediaBrowser.aspx.cs" Inherits="N2.Edit.Navigation.MediaBrowser" Trace="false" %>
<%@ Import Namespace="N2.Resources" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Web.Configuration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Media Browser - N2</title>
        <asp:PlaceHolder runat="server">
		<link href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css" type="text/css" rel="stylesheet" />
		<link rel="stylesheet" type="text/css" href="<%= MapCssUrl("framed.css")%>" />
		<link rel="stylesheet" type="text/css" href="<%= MapCssUrl("mediaBrowser.css")%>" />
		</asp:PlaceHolder>
    </head>

<body>
    <div id="fileBrowser-main-div" class="thumbs-div">

        <ul class="nav nav-tabs no-active-outline" id="tabsCtrl">
            <li role="presentation" class="active"><a id="galleryTab" href="#0"><%= GetLocalResourceString("TabsGallery", "Gallery") %></a></li>
            <li role="presentation"><a id="uploadTab" href="#1"><%= GetLocalResourceString("TabsUpload", "Upload file") %></a></li>
        </ul>

        <div id="browser-files-list" class="browser-files-section first browser-files-list">

            <div class="row files-search-cont">
                <div class="col-sm-8 col-md-6">
                    <div class="input-group input-group-sm">
                        <input type="text" id="input-group-q" class="form-control" placeholder="<%= GetLocalResourceString("Search", "Search...") %>" />
                        <span class="input-group-btn">
                            <button id="btn-search" class="btn btn-default" type="button"><span class="glyphicon glyphicon-search"></span></button>
                            <button id="btn-search-clean" class="btn btn-default" type="button"><span class="glyphicon glyphicon-remove"></span></button>
                        </span>
                    </div><!-- /input-group -->
                </div>
            </div>
            <div class="row files-breadcrumb" id="dirs-breadcrumb" data-rootisselectable="<%= mediaBrowserModel.RootIsSelectable %>">
                <ul>
                <%  var bc = "/";
                    foreach (var s in mediaBrowserModel.Breadcrumb) {
                        bc += (s == "[root]" ? "" : s + "/");%>
                    <li data-url="<%= bc %>"><%= s %></li>
                <% } %>
                </ul>
            </div>

            <div id="lblMessage" class="bg-warning" style="display:none"></div>

            <% var regIsImage = new Regex(@"^.*\.(jpg|jpeg|gif|png)$", RegexOptions.IgnoreCase);
               var counter = 0;%>
            <ul id="browser-files-list-ul" class="files-list" data-path="<%= mediaBrowserModel.Path %>"
                data-selurl="<%= Request["selectedUrl"]%>"
                data-baseajax="<%= mediaBrowserModel.HandlerUrl %>" data-mediacontrol="<%= mediaBrowserModel.MediaControl %>"
                data-ckeditor="<%= mediaBrowserModel.CkEditor %>" data-ckeditorfuncnum="<%= mediaBrowserModel.CkEditorFuncNum %>" data-preferredsize="<%= mediaBrowserModel.PreferredSize %>"
                data-i18size="<%= GetLocalResourceString("Size", "Size") %>" data-i18date="<%= GetLocalResourceString("DateModified", "Date") %>" data-i18url="<%= GetLocalResourceString("Url", "Url") %>">
                <% if(mediaBrowserModel.Dirs!=null) foreach (var d in mediaBrowserModel.Dirs) { %>
                <li data-i="<%= counter++ %>" class="dir" data-url="<%= d.Path %>">
                    <span class="file-ic glyphicon glyphicon-folder-open"></span>
                    <label><%= d.Name %></label>
                </li>
                <%} %>
                <% if(mediaBrowserModel.Files!=null) foreach (var f in mediaBrowserModel.Files) {
                            var img = f.IsImage ? f.Thumb : string.Empty;
                            %>
                    <% if(f.IsImage) { %>
                    <li data-i="<%= counter++ %>" class="file image" data-size="<%= f.Size %>" data-date="<%= f.Date %>" 
                        data-name="<%= f.Title %>" data-isimage="true" data-url="<%= f.Url %>"
                        style="background-image:url('<%= img %>');" >
                        <label><%= f.Title %></label>
                        <% if (f.Children!=null && f.Children.Count > 0) { %>
                        <div class="image-sizes">
                            <em class="<%= string.IsNullOrEmpty(mediaBrowserModel.PreferredSize) ? "selected" : "" %>" data-size="<%= f.Size %>" data-url="<%= f.Url %>">default</em>
                            <% foreach(var ch in f.Children) { %>
                            <em class="<%= mediaBrowserModel.PreferredSize==ch.SizeName ? "selected" : "" %>" data-size="<%= ch.Size %>" data-url="<%= ch.Url %>"><%= ch.SizeName %></em>
                            <%} %>
                        </div>
                        <%} %>
                       </li>
                    <%} else { %>
                    <li data-i="<%= counter++ %>" class="file" data-size="<%= f.Size %>" data-date="<%= f.Date %>" data-isimage="false" data-url="<%= f.Url %>"
                        data-name="<%= f.Title %>">
                        <span class="file-ic glyphicon glyphicon-file"></span>
                        <label><%= f.Title %></label>
                       </li>
                    <%} %>
                <%} %>
            </ul>

        </div>

        <div id="browser-upload-file" class="browser-files-section">
            <div class='file-selector-container'>
            
                <button type="button" id="FileUploadItemId_Btn" class="btn btn-info" data-fire="FileUploadItem"><span class="glyphicon glyphicon-hdd"></span> <%= GetLocalResourceString("SelectFiles", "Select files...") %></button>

                <div class='file-selector-control'>
                    <input class="file-upload-ajax valid" data-valueid="FileUploadItemId" id="FileUploadItem" multiple="multiple" name="FileUploadItem" type="file" />
                </div>

                <div id="holder-FileUploadItemId" data-src="" style="display: none;"></div>
                <input id="FileUploadItemId" name="FileUploadItemId" value="" type="hidden" />

                <div class="progress file-upload-ajax-progress">
                    <div id="file-upload-ajax-progres" class=" progress-bar progress-bar-success active" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="min-width: 2.5em;">
                        0%
                    </div>
                </div>
                <div id="lblMessageUpload" class="bg-warning" style="display:none"></div>

            </div>
            <div class="file-selector-disallowed">
                <%= GetLocalResourceString("UploadDisallowedBrowser", "This browser is outdated and cannot upload files using this dialog. Please use a modern browser to get the most out of N2cms") %>
            </div>
        </div>

    </div>
    <div id="info-div" class="info-div">
        <div id="info-div-details" class="info-div-details"></div>
    </div>

    <div id="browser-files-layover" class="browser-files-layover"></div>
    <div id="browser-files-layover-cont" class="browser-files-layover-cont">
        <h1><%= GetLocalResourceString("ExistingFiles", "Some files already exist in the server<br />What do you want to do with them?") %></h1>
        <ul id="browser-files-layover-ul"
            data-i18keep="<%= GetLocalResourceString("UploadKeepBoth", "Keep both")%>"
            data-i18repl="<%= GetLocalResourceString("UploadReplace", "Replace")%>"
            data-i18ignr="<%= GetLocalResourceString("UploadIgnore", "Ignore")%>">
        </ul>
        <button type="button" id="btn-continue-upload" name="btn-continue-upload" class="btn btn-primary"><%= GetLocalResourceString("Continue", "Continue") %></button>
    </div>

    <div class="framed-navbar navbar navbar-fixed-bottom">
        <div class="navbar-inner">
            <button type="button" id="btn-select" name="btn-select" class="btn btn-primary" disabled="disabled"><%= GetLocalResourceString("Select", "Select") %></button>
            <button type="button" id="btn-cancel" name="btn-cancel command" class="btn btn-cancel"><%= GetLocalResourceString("Cancel", "Cancel") %></button>
        </div>
    </div>

    <script type="text/javascript">
        var tbid = '<%= HttpUtility.JavaScriptStringEncode(Request["tbid"])%>';
        var selectableExtensions = '<%= HttpUtility.JavaScriptStringEncode(Request["selectableExtensions"])%>';
        var ticket = '<%= GetEncryptedTicket() %>';
        var maxSize = <%= GetMaxSize() %>;
        var initialTab = '<%= GetInitialTab() %>';
    </script>

	<script type="text/javascript" src="<%= N2.Web.Url.ResolveTokens(Register.JQueryJsPath)%>"></script>
    <script src="MediaBrowser.js" type="text/javascript"></script>
    <script>
        $(function(){
            if (window.initialTab){
                $(window.initialTab).click();
            }
        })
    </script>
</body>
</html>
