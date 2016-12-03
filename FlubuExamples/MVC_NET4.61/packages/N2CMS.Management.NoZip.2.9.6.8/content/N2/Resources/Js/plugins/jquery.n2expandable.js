(function ($) {
	$.fn.n2expandable = function (args) {
		this.each(function () {
			$panel = $(this);
			var $children = $panel.children();
			if (args.visible) {
				$children = $children.not(args.visible);
			}

			if ($children.length == 0)
				return;

			var text = "Details";
			if (text = $panel.attr("title"))
				$panel.attr("title", "");

			var $expander = (args.expander)
				? $(args.expander)
				: $("<a href='#' class='expander'>" + text + "</a>");

			$expander.prependTo($panel);

			function show() {
				$children.fadeIn();
				$panel.removeClass('expandable-contracted').addClass('expandable-expanded');
			}

			$panel.find('.expander').click(function (e) {
				if ($panel.is('.expandable-expanded')) {
					$children.hide();
					$panel.removeClass('expandable-expanded').addClass('expandable-contracted');
				} else {
					show();
				}
				e.preventDefault();
				e.stopPropagation();
			});

			$panel.click(function (e) {
				if (!$panel.is(".expandable-expanded")) {
					show();
				}
			});

			$children.hide();
			$panel.addClass("expandable-contracted");
		});
	};
})(jQuery);
