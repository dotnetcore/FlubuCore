//n2nav.parentInputId = null;
//n2nav.setOpenerSelected = function(relativeUrl) {
//	if(window.opener){
//		if(window.opener.onFileSelected && window.opener.srcField)
//			window.opener.onFileSelected(relativeUrl);
//		else
//			window.opener.document.getElementById(this.parentInputId).value = relativeUrl;
//		window.close();
//    }
//}
//n2nav.onUrlSelected = function(rewrittenUrl) {
//	this.setOpenerSelected(rewrittenUrl);
//}
//n2nav.getUrl = function(a){
//	return decodeURIComponent(n2nav.toRelativeUrl(a.href));
//}

function openUrlSelectorPopup(popupUrl, tbId, popupOptions, defaultMode, availableModes, availableTypes, availableExtensions) {
	var tb = document.getElementById(tbId);
	window.open(popupUrl
			+ '&tbid=' + tbId
			+ '&defaultMode=' + defaultMode
			+ '&availableModes=' + availableModes
			+ '&selectedUrl=' + encodeURIComponent(tb.value)
			+ '&selectableTypes=' + availableTypes
			+ '&selectableExtensions=' + availableExtensions,
	null,
	popupOptions);
}