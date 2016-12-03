/**
 * n2keyboard 0.1
 */

(function($) {
	var key = { esc: 27, left: 37, up: 38, right: 39, down: 40, del: 46, a: 65, c: 67, n: 78, v: 86, x: 88, z: 90 };
	for (var i = key.a; i < key.z; i++) {
		key[String.fromCharCode(i)] = i;
	}
	$.fn.n2keyboard = function(bindings, focusOn) {
		var s = this.selector;
		bindings = bindings || {};

		var ctx = {
			focus: function(el) {
				$(".focused").removeClass("focused");
				$(el).addClass("focused").focus();
			},
			focused: function() { return $(".focused") },
			all: function() { return $(s); },
			offset: function(i) {
				var a = this.all();
				var fi = a.index(this.focused());
				return a[Math.min(Math.max(0, fi + i), a.length - 1)];
			}
		}

		var boundKeys = {};
		boundKeys[key.up] = function(e) { ctx.focus(ctx.offset(-1)); };
		boundKeys[key.down] = function(e) { ctx.focus(ctx.offset(1)); };
		for (var binding in bindings) {
			if (key[binding]) {
				boundKeys[key[binding]] = bindings[binding];
			}
		}

		$(document).keyup(function(e) {
			if (boundKeys[e.keyCode]) {
				boundKeys[e.keyCode](e, ctx);
				e.preventDefault();
				e.stopPropagation();
			}
		});

		ctx.focus(this.filter(focusOn));
	};
})(jQuery);
