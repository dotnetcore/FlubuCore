n2nav.fileClickHandler = function(event) {
    $("#toolbar").addClass("file");
    $("#toolbar").removeClass("folder");
    var a = n2nav.findLink(event.target);
	$(n2nav.selectionInputId).val(n2nav.toRelativeUrl(a.href));
	n2nav.onTargetClick(a)
    event.preventDefault();
}
n2nav.targetHandlers["file"] = function(a,i) {
    var relativeUrl = n2nav.toRelativeUrl(a.href);
    $(a).addClass("enabled").bind("click", null, n2nav.fileClickHandler);
    a.target = "";
}
n2nav.folderClickHandler = function(event) {
    $("#toolbar").addClass("folder");
    $("#toolbar").removeClass("file");
    var a = n2nav.findLink(event.target);
	$(n2nav.selectionInputId).val(n2nav.toRelativeUrl(a.href));
	if(n2nav.displaySelection)n2nav.displaySelection(a);
    event.preventDefault();
}
n2nav.targetHandlers["folder"] = function(a,i) {
    var relativeUrl = n2nav.toRelativeUrl(a.href);
    $(a).addClass("enabled").bind("click", null, n2nav.folderClickHandler);
    a.target = "";
}
n2nav.onCancel = function(){
    $(document.body).removeClass("newFolder").removeClass("upload");
}
n2nav.onUploadClick = function(){
    $(document.body).removeClass("newFolder").addClass("upload");
}
n2nav.onNewFolderClick = function(){
    $(document.body).removeClass("upload").addClass("newFolder");
}
n2nav.selectionInputId = null;