<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.ascx.cs" Inherits="N2.Management.Files.FileSystem.FileUpload" %>
<%@ Import Namespace="N2.Edit" %>

<div id="fileupload" class="droparea">
	<div id="uploadcontrols">
		<b class="fa fa-upload"></b>
		<p><strong><%= GetLocalResourceString("SelectFiles", "Select files to upload") %></strong></p>
		<p>
			<input id="fuAlternative" runat="server" type="file" name="files[]" multiple="multiple" />
			</p><p>
			<asp:Button runat="server" ID="btnAlternative" OnCommand="OnAlternativeCommand" Text="Upload" CssClass="upload" meta:resuorceKey="btnAlternative" />
			<em><%= GetLocalResourceString("DropFiles", "Or drop files here") %></em>
		</p>
	</div>
    <div class="fileupload-content">
        <div class="fileupload-progressbar"></div>
        <table class="files"></table>
    </div>
</div>
        
<!-- The jQuery UI widget factory, can be omitted if jQuery UI is already included -->
<script src="../../Resources/jQuery-File-Upload-8.5.0/js/vendor/jquery.ui.widget.js"></script>
<!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
<script src="../../Resources/jQuery-File-Upload-8.5.0/js/jquery.iframe-transport.js"></script>
<!-- The basic File Upload plugin -->
<script src="../../Resources/jQuery-File-Upload-8.5.0/js/jquery.fileupload.js"></script>

<%--
<link rel="stylesheet" href="css/jquery.fileupload-ui.css">

<script src="fileupload/tmpl.min.js" type="text/javascript"></script>
<script src="fileupload/load-image.min.js" type="text/javascript"></script>
<script src="fileupload/canvas-to-blob.min.js" type="text/javascript"></script>
<script src="fileupload/js/jquery.iframe-transport.js"></script><!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
<script src="fileupload/js/jquery.fileupload.js"></script><!-- The basic File Upload plugin -->
<script src="fileupload/js/jquery.fileupload-ip.js"></script><!-- The File Upload image processing plugin -->
<script src="fileupload/js/jquery.fileupload-ui.js"></script><!-- The File Upload user interface plugin -->
<script src="fileupload/js/locale.js"></script><!-- The localization script -->
<script src="fileupload/js/main.js"></script><!-- The main application script -->
--%>
<script src="FileUpload.js"></script>

<script type="text/javascript">
    var maxFileSize = <%= maxFileSize %>;
    var ticket = '<%= FormsAuthentication.Encrypt(new FormsAuthenticationTicket("SecureUpload-" + Guid.NewGuid(), false, 60)) %>';
    var selected = '<%= Selection.SelectedItem.Path %>';
	var refreshFrames = function(){
		console.log("refreshFrames");
		<%= Page.GetRefreshFramesScript(Selection.SelectedItem, N2.Edit.ToolbarArea.Navigation, true) %>;
	};
    
	$(function(){
		n2.FileUpload(maxFileSize, ticket, selected, refreshFrames, ".fileupload-content");
	});

</script>

<script id="template-upload" type="text/x-jquery-tmpl">
{% for (var i=0, file; file=o.files[i]; i++) { %}
    <tr class="template-upload fade">
        <td class="preview"><span class="fade"></span></td>
        <td class="name"><span>{%=file.name%}</span></td>
        <td class="size"><span>{%=o.formatFileSize(file.size)%}</span></td>
        {% if (file.error) { %}
            <td class="error" colspan="2"><span class="label label-important">{%=locale.fileupload.error%}</span> {%=locale.fileupload.errors[file.error] || file.error%}</td>
        {% } else if (o.files.valid && !i) { %}
            <td>
                <div class="progress progress-success progress-striped active"><div class="bar" style="width:0%;"></div></div>
            </td>
            <td class="start">{% if (!o.options.autoUpload) { %}
                <button class="btn btn-primary">
                    <i class="fa fa-upload icon-white"></i>
                    <span>{%=locale.fileupload.start%}</span>
                </button>
            {% } %}</td>
        {% } else { %}
            <td colspan="2"></td>
        {% } %}
        <td class="cancel"></td>
    </tr>
{% } %}
</script>
<script id="template-download" type="text/x-jquery-tmpl">
{% for (var i=0, file; file=o.files[i]; i++) { %}
    <tr class="template-download fade">
        {% if (file.error) { %}
            <td></td>
            <td class="name"><span>{%=file.name%}</span></td>
            <td class="size"><span>{%=o.formatFileSize(file.size)%}</span></td>
            <td class="error" colspan="2"><span class="label label-important">{%=locale.fileupload.error%}</span> {%=locale.fileupload.errors[file.error] || file.error%}</td>
        {% } else { %}
            <td class="preview">{% if (file.thumbnail_url) { %}
                <a href="{%=file.url%}" title="{%=file.name%}" rel="gallery" download="{%=file.name%}"><img src="{%=file.thumbnail_url%}"></a>
            {% } %}</td>
            <td class="name">
                <a href="{%=file.url%}" title="{%=file.name%}" rel="{%=file.thumbnail_url&&'gallery'%}" download="{%=file.name%}">{%=file.name%}</a>
            </td>
            <td class="size"><span>{%=o.formatFileSize(file.size)%}</span></td>
            <td colspan="2" class="complete"></td>
        {% } %}
    </tr>
{% } %}
</script>