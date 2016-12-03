// NAVIGATION
var n2nav = new Object();

n2nav.linkContainerId = null;
n2nav.hostName = window.location.hostname;
n2nav.toRelativeUrl = function(absoluteUrl) {
	if(absoluteUrl.indexOf(n2nav.hostName)>0)
		return absoluteUrl.replace(/.*?:\/\/.*?\//, "/");
	return absoluteUrl;
}
n2nav.onUrlSelected = null;
n2nav.findLink = function(el) {
	while(el && el.tagName != "A")
		el = el.parentNode;
	return el;
}

n2nav.displaySelection = function(el){
	$(".selected").removeClass("selected");
	$(el).addClass("selected");
}

n2nav.getPath = function(a) {
	return $(a).attr("data-path");
}
n2nav.onTargetClick = function(el){
	n2nav.displaySelection(el);
	if(n2nav.onUrlSelected)
		n2nav.onUrlSelected(n2nav.getPath(el));
}

n2nav.handlers = {
	fallback: function(e) {
		n2nav.onTargetClick(this)
		n2nav.update({ 
			path: n2nav.getPath(this), 
			previewUrl: this.href,
			permission: $(this).attr("data-permission")
		});
	}
};

n2nav.setupToolbar = function(path, url) {
	n2ctx.update({ path: path, previewUrl: url });
}
n2nav.update = function (options) {
	n2ctx.update(options);
}



// EDIT
var n2toggle = {
	show: function(btn, bar) {
		$(btn).addClass("toggled").blur();
		$(bar).show();
		$.cookie(bar, "show");
	},
	hide: function(btn, bar) {
		$(btn).removeClass("toggled").blur();
		$(bar).hide();
		$.cookie(bar, null)
	}
};

var initn2context = function (w) {
	if (w.n2ctx)
		return w.n2ctx;

	try {
		if (w.name != "top" && w != w.parent) {
			w.n2ctx = initn2context(w.parent);
			return w.n2ctx;
		}
	} catch (e) { }

	w.n2ctx = {
		selectedPath: "/",
		_path: "/",
		selectedUrl: null,
		memorizedPath: null,
		actionType: null,

		// whether there is a top frame
		hasTop: function () {
			return false;
		},

		// selects a toolbar item by name
		toolbarSelect: function (name, context) {
			w.n2.select(name);
		},

		// copy/paste
		memorize: function (selected, action) {
			this.memorizedPath = selected;
			this.actionType = action;
		},
		getSelected: function () {
			return this.selectedPath;
		},
		path: function (value) {
			if (arguments.length == 0)
				return this._path;

			this._path = value;
			return this;
		},
		getSelectedUrl: function () {
			return this.selectedUrl;
		},
		getMemory: function () {
			return encodeURIComponent(this.memorizedPath);
		},
		getAction: function () {
			return encodeURIComponent(this.actionType);
		},

		initToolbar: function () {
			$(".command a").click(function (e) {
				if (this.hash == "#stop")
					e.preventDefault();
			});
		},

		// selection memory
		update: function (options) {
			options.previewUrl = options.previewUrl || this.selectedUrl;
			this.selectedPath = options.path;
			this.selectedUrl = options.previewUrl;

			if (!this.hasTop()) return;

			var memory = this.getMemory();
			var action = this.getAction();

			var formats = {
				url: options.previewUrl,
				selected: options.path,
				memory: memory,
				action: action 
			};
			$("a.templatedurl").each(function () {
				var href = $(this).attr("data-url-template") || a.href;
				for (var key in formats) {
					var format = "{" + key + "}";
					if (href.indexOf(format) >= 0 && formats[key] == "null") {
						href = "#stop";
						$(this).addClass("disabled");
						break;
					}
					else $(this).removeClass("disabled");

					href = href.replace(format, formats[key]);
				}
				href = href.replace("{query}", href.indexOf('?') >= 0 ? "&" : "?");
				this.href = href;
			});

			$(document).ready(function () {
				w.document.getElementById("permission").className = options.permission;
			});
		},

		append: function (url, data) {
			return url + (url.indexOf('?') >= 0 ? "&" : "?") + jQuery.param(data);
		},

		/// update frames
		/// * previewUrl: url to load preview frame
		/// * navigationUrl: url to load navigation frame
		/// * force: force navigation refresh (default true)
		/// * path: update path to
		refresh: function (options) {
			options = jQuery.extend({ previewUrl: null, navigationUrl: null, force: true,
				showPreview: function () {
					var previewFrame = w.document.getElementById("previewFrame");
					previewFrame.src = this.previewUrl;
				},
				showNavigation: function (ctx) {
					var navigationFrame = w.document.getElementById("navigationFrame");
					navigationFrame.src = ctx.append(this.navigationUrl, { location: ctx.location });
				}
			}, options);

			if (this.hasTop()) {

				if (options.previewUrl) {
					if (w.frames.preview && w.frames.preview.onPreviewing)
						w.frames.preview.onPreviewing(options);
					options.showPreview();
				}
				if (options.navigationUrl) {
					if (w.frames.navigation && w.frames.navigation.onNavigating)
						w.frames.navigation.onNavigating(options);
					options.showNavigation(this);
				}
			} else if (options.previewUrl) {
				window.location = options.previewUrl;
			}
			if (options.path)
				this.path(options.path);
			this.update(options);
		},

		// toolbar selection
		select: function (name) {
			if (!name) return;

			$s = jQuery("#" + name);
			n2.unselectFrame($s.find("a").attr("target"));
			$s.addClass("selected");
			jQuery(document.body).addClass(name + "Selected");
		},
		unselectFrame: function (frame) {
			jQuery(".selected a").filter(function () { return this.target === frame || !this.target; })
				.closest(".selected")
				.each(function () {
					n2.unselect(this.id);
				});
		},
		unselect: function (name) {
			if (!name) return;

			jQuery("#" + name).removeClass("selected");
			jQuery(document.body).removeClass(name + "Selected");
		},
		context: function (ctx) {
			this.flags = ctx.Flags;
		},
		isFlagged: function (flag) {
			return this.flags && this.flags[flag];
		},
		layout: {
			init: function () {
				$("#splitter-container,.pane").layout({ useStateCookie: true, cookie: { expires: 365 }, defaults: { spacing_closed: 12 }, north: { resizable: false }, west: { minWidth: 250 }, center: { minWidth: 250} });
				$("#permission").css({ position: "" }); // restore north pane style so drop-downs arn't hidden
			}
		}
	};

	return w.n2ctx;
};
window.n2 = $.extend(window.n2, initn2context(window));
