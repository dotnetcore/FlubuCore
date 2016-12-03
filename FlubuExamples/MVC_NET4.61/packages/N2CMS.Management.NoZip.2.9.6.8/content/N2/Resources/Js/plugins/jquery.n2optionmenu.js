/**
 * n2optionmenu 0.1
 */

(function($) {
    $.fn.n2optionmenu = function(options) {
        var settings = {
            wrapper: "<div class='commandOptions closed'></div>",
            opener: "<span class='opener'>open</span>",
            closedClass: "closed"
        };
        $.extend(settings, options || {});

        var closable = false;
        var $menu = this;

        var $wrapper = $menu.wrap(settings.wrapper);
        var closeMenu = function() {
            if (closable)
                $wrapper.parent().addClass(settings.closedClass);
        };
        var openMenu = function(e) {
            e.stopPropagation();
            e.preventDefault();
            closable = false;
            $wrapper.parent().toggleClass(settings.closedClass);
            setTimeout(function() { closable = true; }, 10);
        };

        var $firstEnabled = $menu.children().not("a[disabled='disabled']").not("a[disabled='true']").slice(0, 1);
        $firstEnabled.clone(true).insertBefore($menu)
			.bind('contextmenu', openMenu)
			.after(settings.opener)
			.next().click(openMenu)
			.bind('contextmenu', openMenu);
        $(document.body).click(closeMenu);
        return $menu;
    }
})(jQuery);;

