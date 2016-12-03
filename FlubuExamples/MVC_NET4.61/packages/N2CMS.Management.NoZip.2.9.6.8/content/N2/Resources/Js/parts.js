(function ($) {
	var isDragging = false;
	var dialog = null;

	window.n2DragDrop = function(urls, messages, context) {
		this.urls = $.extend({
			copy: 'copy.n2.ashx',
			move: 'move.n2.ashx',
			remove: 'delete.n2.ashx',
			create: 'create.n2.ashx',
			editsingle: '/N2/Content/EditSingle.aspx'
		}, urls);
		this.messages = $.extend({
			deleting: 'Do you really want to delete?',
			helper: "Drop on a highlighted area"
		}, messages);
		this.context = context;
		this.init();
	};

	window.n2DragDrop.prototype = {

		init: function () {
			var self = this;
			this.makeDraggable();
			$(document.body).addClass("dragDrop");
			var host = window.location.protocol + "//" + window.location.host + "/";
			$("a").filter(function () { return this.href.indexOf(host) == 0; })
				.filter(function () { return this.parentNode.className.indexOf('control') < 0; })
				.filter(function () { return !this.target || this.target == "_self"; })
				.each(function () {
					var hashIndex = this.href.indexOf("#");
					if (hashIndex >= 0)
						this.href = this.href.substr(0, hashIndex) + ((this.href.indexOf('?') >= 0 ? '&' : '?') + "edit=drag") + this.href.substr(hashIndex);
					else
						this.href += (this.href.indexOf('?') >= 0 ? '&' : '?') + "edit=drag";
				});

			self.makeEditable();
			self.scroll();
		},

		makeDraggable: function () {
			$('.definition').draggable({
				dragPrevention: 'a,input,textarea,select,img',
				helper: this.makeDragHelper,
				cursorAt: { top: 8, left: 8 },
				scroll: true,
				stop: this.stopDragging,
				start: this.startDragging
			}).data("handler", this);
			$('.zoneItem').draggable({
				handle: "> .titleBar",
				dragPrevention: 'a,input,textarea,select,img',
				helper: this.makeDragHelper,
				cursorAt: { top: 8, left: 8 },
				scroll: true,
				stop: this.stopDragging,
				start: this.startDragging
			}).data("handler", this);
		},

		appendSelection: function (url, command) {
			return url
				+ (url.indexOf("?") >= 0 ? "&" : "?") + (n2SelectedQueryKey || "selected") + "=" + command.below
				+ (this.context.isMasterVersion ? "" : "&n2versionIndex=" + this.context.versionIndex)
				+ (!command.n2versionKey ? "" : "&n2versionKey=" + command.n2versionKey);
		},

		makeEditable: function () {
			var self = this;
			$(".editable").each(function () {
				var $t = $(this);
				var url = self.appendSelection(self.urls.editsingle, { below: $t.attr("data-path") })
					+ "&property=" + $t.attr("data-property")
					+ "&n2versionKey=" + $t.attr("data-versionKey")
					+ "&returnUrl=" + encodeURIComponent(window.location.pathname + window.location.search)
					+ "&edit=drag";
				
				$(this).dblclick(function (e) {
					window.location = url;
				}).each(function () {
					if ($(this).closest("a").length > 0)
						$(this).click(function (e) { e.preventDefault(); e.stopPropagation(); });
				});
				$("<a class='editor fa fa-pencil' href='" + url + "'></a>").appendTo(this);
			});
		},
		scroll: function () {
			if (window.location.search.indexOf("n2scroll") >= 0) {
				var top = parseInt(window.location.search.match(/n2scroll=([^&]*)/)[1]);
				if (top) {
					$(function () {
						setTimeout(function () {
							$(document).scrollTop(top, 0);
						}, 100);
					});
				}
			}
		},
		makeDragHelper: function (e) {
			isDragging = true;
			var $t = $(this);
			var handler = $t.data("handler");
			$(document.body).addClass("dragging");
			var shadow = document.createElement('div');
			$(shadow).addClass("dragShadow")
				.css({ height: Math.min($t.height(), 200), width: $t.width() })
				.text(handler.messages.helper).appendTo("body");
			return shadow;
		},

		makeDropPoints: function (dragged) {
			var type = $(dragged).addClass("dragged").attr("data-type");

			$(".dropZone").each(function () {
				var zone = this;
				var allowed = $(zone).attr("data-allowed") + ",";
				var title = $(zone).attr("title");
				if (allowed.indexOf(type + ",") >= 0) {
					$(zone).append("<div class='dropPoint below'/>");
					$(".zoneItem", zone)
						.not(function () { return $(this).closest(".dropZone")[0] !== zone; })
						.each(function (i) { $(this).before("<div class='dropPoint before' title='" + i + "'/>"); });
				}
				$(".dropPoint", zone).html("<div class='description'>" + title + "</div>");
			});
			$(dragged).next(".dropPoint").remove();
			$(dragged).prev(".dropPoint").remove();
		},

		makeDroppable: function () {
			$(".dropPoint").droppable({
				activeClass: 'droppable-active',
				hoverClass: 'droppable-hover',
				tolerance: 'pointer',
				drop: this.onDrop,
				over: function (e, ui) {
					currentlyOver = this;
					var $t = $(this);
					$t.data("html", $t.html()).data("height", $t.height());
					//$t.html(ui.draggable.html()).css("height", "auto");
					ui.helper.height($t.height()).width($t.width());
				},
				out: function (e, ui) {
					if (currentlyOver === this) {
						currentlyOver = null;
					}
					var $t = $(this);
					$t.html($t.data("html")).height($t.data("height"));
				}
			});
		},

		onDrop: function (e, ui) {
			if (isDragging) {
				isDragging = false;

				var $droppable = $(this);
				var $draggable = $(ui.draggable);

				var handler = $draggable.data("handler");
				$draggable.html("");
				$droppable.append("<div class='dropping'/>");

				var $next = $droppable.filter(".before").next();
				var data = {
					ctrlKey: e.ctrlKey,
					n2item: $draggable.attr("data-item"),
					n2versionKey: $draggable.attr("data-versionKey"),
					n2versionIndex: $draggable.attr("data-versionIndex") || n2ddcp.context.versionIndex,
					discriminator: $draggable.attr("data-type"),
					template: $draggable.attr("data-template"),
					before: ($next.attr("data-versionKey") ? "" : $next.attr("data-item")) || "", // data-item may be page+index+key when new part
					beforeSortOrder: $next.attr("data-sortOrder") || "",
					below: $droppable.closest(".dropZone").attr("data-item"),
					zone: $droppable.closest(".dropZone").attr("data-zone"),
					returnUrl: window.location.href,
					dropped: true
				};
				if ($droppable.closest(".dropZone").attr("data-versionIndex")) {
					data.belowVersionIndex = $droppable.closest(".dropZone").attr("data-versionIndex");
					data.belowVersionKey = $droppable.closest(".dropZone").attr("data-versionKey");
				}
				if ($next.attr("data-versionKey")) {
					data.beforeVersionKey = $next.attr("data-versionKey");
					data.beforeVersionIndex = $next.attr("data-versionIndex");
				}

				handler.process(data);
			}
		},

		stopDragging: function (e, ui) {
			n2SlidingCurtain.fadeIn();
			$(this).html($(this).data("html")); // restore html removed by jquery ui
			$(this).removeClass("dragged");
			$(".dropPoint").remove();
			$(document.body).removeClass("dragging");
			setTimeout(function () { isDragging = false; }, 100);
		},

		startDragging: function (e, ui) {
			n2SlidingCurtain.fadeOut();
			$(this).data("html", $(this).html());
			var dragged = this;
			var handler = $(dragged).data("handler");
			handler.makeDropPoints(dragged);
			handler.makeDroppable();

			dragged.dropHandler = function (ctrl) {
				var id = $(this).attr("data-item");
				if (!id)
					t.createIn(s.id, d);
				else if (ctrl)
					return t.copyTo(s.id, dragged);
				else
					return t.moveTo(s.id, dragged);
			}
		},

		format: function (f, values) {
			for (var key in values) {
				var keyIndex = url.indexOf("{" + key + "}", 0);
				if (keyIndex >= 0)
					f = f.substring(0, keyIndex) + values[key] + f.substring(2 + keyIndex + formatKey.length);
			}
			return f;
		},

		process: function (command) {
			var self = this;
			if (command.n2item)
				command.action = command.ctrlKey ? "copy" : "move";
			else
				command.action = "create";
			command.random = Math.random();

			var url = self.urls[command.action];
			url = self.appendSelection(url, command);

			var reloaded = false;
			$.post(url, command, function (data) {
				reloaded = true;
				var newLocation = (data.redirect
					? data.redirect
					: window.location);
				newLocation += (newLocation.indexOf("?") >= 0 ? "&" : "?") + "n2scroll=" + ($(document).scrollTop() | 0);
				window.location = newLocation;
			}, "json");

			// hack: why no success??
			setTimeout(function () {
				if (!reloaded)
					window.location.reload();
			}, 15000);
		}
	};

	var n2 = {
		setupToolbar: function () {
		},
		refreshPreview: function () {
			window.top.location.reload();
		},
		refresh: function () {
			window.top.location.reload();
		}
	};

	n2SlidingCurtain = {
		selector: ".sc",
		closedPos: { top: "0px", left: "0px" },
		openPos: { top: "0px", left: "0px" },

		recalculate: function () {
			var $sc = $(this.selector)
			this.closedPos = { top: (33 - $sc.height()) + "px", left: (15 - $sc.width()) + "px" };
			if (!this.isOpen())
				$sc.css(this.closedPos);
		},

		isOpen: function () {
			return $.cookie("sc_open") == "true";
		},

		init: function (selector, startsOpen) {
			this.selector = selector;
			var $sc = $(selector);
			var self = this;

			$(function () {
				self.recalculate();
				setTimeout(function () { self.recalculate(); }, 500);
			});

			self.open = function (e) {
				if (e) {
					$sc.animate(self.openPos);
				} else {
					$sc.css(self.openPos);
				}
				$sc.addClass("opened");
				$.cookie("sc_open", "true", { expires: 1 });
			};
			self.close = function (e) {
				if (e) {
					$sc.animate(self.closedPos);
				} else {
					$sc.css(self.closedPos);
				}
				$sc.removeClass("opened");
				$.cookie("sc_open", null);
			};
			self.fadeIn = function (e) {
				$sc.fadeIn();
			};
			self.fadeOut = function (e) {
				$sc.fadeOut();
			};

			if (startsOpen) {
				$sc.animate(self.openPos).addClass("opened");
			} else if (this.isOpen()) {
				self.open();
			} else {
				self.close();
			}

			$sc.find(".close").click(self.close);
			$sc.find(".open").click(self.open);
		}
	};

	window.frameInteraction = {
		location: "Organize",
		ready: true,
		getActions: function() {

			function create(commandElement) {
				return {
					Title: $(commandElement).attr('title'),
					Id: commandElement.id,
					Selector: '#' + commandElement.id,
					Href: commandElement.href,
					CssClass: commandElement.className,
					IconClass: $(commandElement).attr('data-icon-class')
				};
			};
			var actions = [];
			var idCounter = 0;
			$('.controlPanel .plugins .control > a').not('.cpView, .cpAdminister, .cpOrganize, .complementary, .authorizedFalse').each(function() {
				if (!this.id)
					this.id = "action" + ++idCounter;
				actions.push({ Current: create(this) });
			});

			if (actions.length == 0)
				return actions;
			return [{
				Current: actions[0].Current,
				Children: actions.slice(1)
			}];
		},
		hideToolbar: function(force) {
			$('.controlPanel .plugins .control > a').not('.cpView, .cpAdminister, .cpOrganize, .complementary, .authorizedFalse')
				.parent().hide();
		},
		execute: function(selector) {
			window.location = $(selector).attr('href');
		}
	};

})(jQuery);
