// Avoid `console` errors in browsers that lack a console.
(function () {
	var method;
	var noop = function () { };
	var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeStamp', 'trace', 'warn'
	];
	var length = methods.length;
	var console = (window.console = window.console || {});

	while (length--) {
		method = methods[length];

		// Only stub undefined methods.
		if (!console[method]) {
			console[method] = noop;
		}
	}
}());

// Place any jQuery/helper plugins in here.

var N2 = {};

N2.splitter = function (master, slave, options) {
	var settings = $.extend({}, N2.splitter.defaults, options);
	var $master = $(master);
	var $slave = $(slave);
	$(settings.dragbarSelector).mousedown(function (e) {
		$master = $(master);
		$slave = $(slave);
		e.preventDefault();
        $(document).mousemove(function (e) {
        	$master.css("width", e.pageX + "px");
        	$slave.css({
        		left: $master.outerWidth(true) + "px",
        		width: ($(window).width() - $master.outerWidth(true) - ($slave.outerWidth(true) - $slave.width())) + "px"
        	});
        	$("html").css("cursor", "w-resize")
        });
        $("#page-preview-frame-cover").show();
    });
    $(document).mouseup(function (e) {
        $(document).unbind('mousemove');
        $("html").css("cursor", "auto");
        $("#page-preview-frame-cover").hide();
    });
    $(window).resize(resize);
    function resize() {
        $slave.css({
            left: $master.outerWidth(true),
            width: ($(window).width() - $master.outerWidth(true) - ($slave.outerWidth(true) - $slave.width()))
        });
    };
    resize();
};
N2.splitter.defaults = {
    dragbarSelector: ".dragbar"
}

N2.slidingLoader = function (el, options) {
    var $el = $(el),
        settings = $.extend({}, N2.slidingLoader.defaults, options),
        timeout = null,
        isLoading = false;

    this.start = function () {
        isLoading = true;
        slide();
        $el.addClass(settings.loadingClass);
    };

    this.stop = function () {
        isLoading = false;
        $el.stop();
        clearTimeout(timeout);
        reset();
        $el.removeClass(settings.loadingClass);
    };

    function slide() {
        reset();
        $el.animate({
            backgroundPosition: "-" + (settings.loaderWidth) + "px"
        }, settings.speed, function () {
            timeout = setTimeout(slide, settings.delay);
        });
    }

    function reset() {
        $el.css({
            backgroundImage: "url(" + settings.loaderImage + ")",
            backgroundPosition: (settings.loaderWidth) + "px"
        });
        var width = $el.width();

    }
    reset();
}
N2.slidingLoader.defaults = {
    loaderImage: "",
    delay: 0,
    speed: 3000,
    loadingClass: "loading",
    loaderWidth: 1854
}
N2.slidingLoader.prototype = {

};

$(function () {
	var loader = new N2.slidingLoader(".sliding-loader", { loaderImage: "Resources/img/sliding-loader.png" });

	loader.start();

	setTimeout(function () {
		loader.stop();
	}, 5000);

});

$.fn.n2splitter = function (master, slave) {
	new N2.splitter(master, slave);
}