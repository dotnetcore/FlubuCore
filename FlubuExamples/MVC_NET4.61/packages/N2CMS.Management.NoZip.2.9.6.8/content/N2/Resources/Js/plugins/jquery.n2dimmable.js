/**
 * n2dimmable 0.1
 */

(function($) {
    $.fn.n2dimmable = function(options) {
        var selector = this.selector;
        options = $.extend(options, { dimmerClass: "dimmed", valuesSelector: "input[type='text'], input[type='file']" });
        var toggleDimmer = function() {
            var $t = $(this).closest(selector);
            $t.siblings().addClass(options.dimmerClass);
            $t.removeClass(options.dimmerClass);
        };
        var checkDimming = function() {
            var $t = $(this).closest(selector);
            var $inputs = $t.siblings().andSelf().contents().filter(options.valuesSelector);
            $inputs.each(function() {
                if (this.value) {
                    toggleDimmer.call(this);
                }
            });
        };
        this.click(toggleDimmer)
    					.contents().filter("input")
    						.focus(toggleDimmer)
    						.blur(checkDimming);
    };
})(jQuery);;
