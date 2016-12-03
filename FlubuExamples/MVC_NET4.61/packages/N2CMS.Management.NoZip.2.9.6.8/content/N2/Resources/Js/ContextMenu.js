n2nav.currentUrl = "/";
n2nav.memorize = function(selected,action){
	window.n2ctx.memorize(selected, action);
}
n2nav.setupToolbar = function (options) {
	n2ctx.update(options);
	var path = encodeURIComponent(options.path);
	var memory = window.n2ctx.getMemory();
	var action = window.n2ctx.getAction();
	$("a.templatedurl").each(function () {
		this.href = $(this).attr("data-url-template")
			.replace("{selected}", path)
			.replace("{memory}", memory)
			.replace("{action}", action);
	});
}
