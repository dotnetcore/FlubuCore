(function ($) {
    $.fn.n2datepicker = function (options) {
    	options = $.extend({ showOn: 'button', changeYear: true }, options);
    	this.click(function (e) {
    		e.stopPropagation();
    	});
        return this.datepicker(options);
    }
})(jQuery);