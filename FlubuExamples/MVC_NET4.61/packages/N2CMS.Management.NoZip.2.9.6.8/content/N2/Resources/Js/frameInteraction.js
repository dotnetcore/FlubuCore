window.frameInteraction = {
	location: "EditPage",
	ready: false,
	getActions: function () {
		var actions = [];

		function create(commandElement) {
			return {
				Title: $(commandElement).text(),
				Id: commandElement.id,
				Selector: '#' + commandElement.id,
				Href: commandElement.href,
				CssClass: commandElement.className,
				IconClass: $(commandElement).attr('data-icon-class')
			};
		};

		$('.primary-action,.command.action').each(function () {
			if ($(this).closest('.optionGroup').length)
				// skip itself cloned in the option group
				return;

			var node = {
				Current: create(this),
				Children: []
			};

			$(this).siblings('.optionGroup').find('.command').not('.hidden-action').each(function () {
				node.Children.push({ Current: create(this) });
			});

			actions.push(node);
		});

		return actions;
	},
	getFlags: function () {
		var flags = [];
		$("[data-flag]").each(function () {
			flags.push($(this).attr("data-flag"));
		});
		return flags;
	},
	getReturnUrl: function () {
		return $("[data-return-url]").attr("data-return-url");
	},
	hideToolbar: function (force) {
		if (force || $('#toolbar .inner > .command, #toolbar .rightAligned > .command, #toolbar .inner > .commandOptions > .command, #toolbar .rightAligned > .commandOptions >.command').not('.primary-action, .cancel, .globalize').length == 0) {
			$('body').addClass('toolbar-hidden');
		} else {
			$('body').removeClass('toolbar-hidden');
		}
	},
	execute: function (selector) {
		window.location = $(selector).attr('href');
		//$(selector).click();
	}
}
jQuery(function ($) {
	window.frameInteraction.ready = true;
});
