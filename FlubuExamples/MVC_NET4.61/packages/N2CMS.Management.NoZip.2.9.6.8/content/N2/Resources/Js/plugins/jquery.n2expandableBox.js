(function ($) {
	$.fn.n2expandableBox = function (args) {
		args = $.extend({ selector: this.selector, openerClosedClass: "opener-closed", boxClosedClass: "box-closed" }, args);
		this.each(function () {
			var box = this;
			var boxId = box.id;
			function openerClicked() {
				var opener = this;
				$(box).find(args.opened).toggle(100, function (e) {
					var c = $(this).is(":visible") ? null : 1;
					$.cookie(boxId, c);
					if (c) {
						$(opener).addClass(args.openerClosedClass);
						$(box).addClass(args.boxClosedClass);
					} else {
						$(opener).removeClass(args.openerClosedClass);
						$(box).removeClass(args.boxClosedClass);
					}
				});
			};
			$(box).find(args.opener).click(openerClicked).each(function () {
				if ($.cookie(boxId))
					$(this).click();
			});
		});
	};

	$.fn.n2expandableBootstrapColumn = function (args) { /* TODO: Not done */
		args = $.extend({ selector: this.selector, openerClosedClass: "opener-closed", boxClosedClass: "box-closed" }, args);
		this.each(function () {
			var box = this;
			var boxId = box.id;
			function openerClicked() {
				var opener = this;
				$(box).find(args.opened).toggle(100, function (e) {
					var c = $(this).is(":visible") ? null : 1;
					$.cookie(boxId, c);
					if (c) {
						$(opener).addClass(args.openerClosedClass);
						$(box).addClass(args.boxClosedClass);
					} else {
						$(opener).removeClass(args.openerClosedClass);
						$(box).removeClass(args.boxClosedClass);
					}
				});
			};
			$(box).find(args.opener).click(openerClicked).each(function () {
				if ($.cookie(boxId))
					$(this).click();
			});
		});
	};
})(jQuery);
