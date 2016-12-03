jQuery(document).ready(function () {
	var dragMemory = null;
	var onDrop = function (e, ui) {
		var action = e.ctrlKey ? "copy" : "move";
		var to = $(this).attr("data-path");
		var from = dragMemory;
		parent.preview.location = "../paste.aspx?action=" + action
								+ "&memory=" + encodeURIComponent(from)
								+ "&" + (n2SelectedQueryKey || "selected") + "=" + encodeURIComponent(to);
	};
	var onStart = function (e, ui) {
		dragMemory = $(this).attr("data-path");
	};

	var toDraggable = function (container) {
		jQuery("a", container).draggable({
			delay: 100,
			cursorAt: { top: 8, left: 8 },
			start: onStart,
			helper: 'clone'
		}).droppable({
			accept: '#nav li li a',
			hoverClass: 'droppable-hover',
			tolerance: 'pointer',
			drop: onDrop
		});
	}

	jQuery("#nav").SimpleTree({
		success: function (el) {
			toDraggable(el);
		}
	});

	jQuery("#nav").on("focus", "a", function (e) {
		var $a = $(this);


		var type = $a.attr("data-type");
		var permission = $a.attr("data-permission");

		if ($(document.documentElement).is(".filesselectionLocation")) {
			if (type == "Directory" && (permission == "Write" || permission == "Add" || permission == "Publish" || permission == "Administer")) {
				$(".FileUpload:not(:visible)").slideDown();
			} else {
				$(".FileUpload:visible").slideUp();
			}
		}

		document.body.className = document.body.className.replace(/\w+Selected?/g, type + "Selected");
		document.body.className = document.body.className.replace(/\w+Permission?/g, permission + "Permission");

		jQuery(".focused").removeClass("focused");
		$a.addClass("focused");
	});

	jQuery("#nav").on("click", "a", function (e) {
		var $a = $(this);
		if ($a.is(".toggler"))
			return;

		var handler = n2nav.handlers[$a.attr("data-type")] || n2nav.handlers["fallback"];
		handler.call($a[0], e);

		$a.focus();
	});

	window.onNavigating = function (options) {
		if (options.force)
			return;
		$("#" + options.path.replace(/\//g, "_"), this.document).each(function () {
			$(this).trigger("click");
			options.showNavigation = function () { };
		});
	};

	toDraggable(jQuery("#nav li li"));

	$(".tree a.selected").each(function () { document.body.className += " " + $(this).attr("data-type") + "Selected"; });

	$(".focusGroup a:not(.toggler):visible").n2keyboard({
		left: function (e, ctx) {
			var el = ctx.focused().closest(".folder-open")
							.children(".toggler").click()
							.siblings("a:not(.toggler)");
			ctx.focus(el);
		},
		right: function (e, ctx) {
			ctx.focused().siblings(".folder-close > .toggler").click();
		},
		esc: function (e) {
			if (e.altKey || e.ctrlKey) return;
			$("#contextMenu").n2hide();
		},
		del: function (e) {
			if (e.altKey || e.ctrlKey) return;
			$("#contextMenu a.delete").n2trigger();
		},
		c: function () {
			$("#contextMenu a.copy").n2trigger();
		},
		n: function () {
			$("#contextMenu a.new").n2trigger();
		},
		v: function () {
			$("#contextMenu a.paste").n2trigger();
		},
		x: function () {
			$("#contextMenu a.move").n2trigger();
		},
		enter: function () {
			ctx.focused().click();
		}
	}, ".selected");
});
