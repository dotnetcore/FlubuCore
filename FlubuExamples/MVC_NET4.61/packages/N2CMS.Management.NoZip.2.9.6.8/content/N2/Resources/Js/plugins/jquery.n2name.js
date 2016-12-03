/**
* n2name 0.3
*/

var n2Name = (function ($) {
	var nameWaitingInt;

	function sanitizeRegex(value) {
		var n = value.replace(/^\s+|\s+$/g, '')
			.replace(/[.]+/g, '-')
			.replace(/[^a-z0-9 -ãàáäâẽèéëêìíïîõòóöôùúüûñç]/g, '')
			.replace(/\s+/g, whitespace)
			.replace(/[-]+/g, '-')
			.replace(/[-]+$/g, '');
		if (tolower) n = n.toLowerCase();

		var from = "ãàáäâẽèéëêìíïîõòóöôùúüûñç";
		var to = "aaaaaeeeeeiiiiooooouuuunc";
		for (var i = 0, l = from.length ; i < l ; i++) {
			n = n.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
		}
		for (var i in replacements) {
			n = n.replace(replacements[i].pattern, replacements[i].value);
		}
		return n;
	}

	function getName(titleid, nameid, whitespace, tolower, replacements, checkboxid, callback) {
		var titleBox = document.getElementById(titleid);
		if (!titleBox) return;

        $.ajax({
            type: 'POST',
            cache: false,
            url: 'sluggenerator.n2.ashx',
            data: { action: "sluggenerator", title: titleBox.value },
            success: function (result) {
            	callback(result);
            },
            error: function () {
            	callback(sanitizeRegex(titleBox.value));
            }
        });
	};

	function updateName(titleid, nameid, whitespace, tolower, replacements, checkboxid) {
		if (checkboxid && document.getElementById(checkboxid).checked) {
			getName(titleid, nameid, whitespace, tolower, replacements, checkboxid, function callback(name) {
				document.getElementById(nameid).value = name;
			});
		}
	};

	function checkboxHandler() {
		var checked = $(this).find('input').prop('checked');
		if (checked)
			$(this).removeClass('unchecked')
                .siblings('input').removeClass('disabled').removeAttr('readonly');
		else
			$(this).addClass('unchecked')
                .siblings('input').addClass('disabled').attr('readonly', true);
	};

	var ebbTimeout = null;
	function ebb(callback, timeout) {
		ebbTimeout && clearTimeout(ebbTimeout);
		ebbTimeout = setTimeout(callback, timeout || 500);
	}

	$.fn.n2name = function (options) {
		var invokeUpdateName = function () {
			ebb(function () {
				updateName(options.titleId, options.nameId, options.whitespaceReplacement, options.toLower, options.replacements, options.keepUpdatedBoxId);
			});
			
		};
		if (options.keepUpdatedBoxId) {
			var $ku = $(this).siblings(".keepUpdated");

			$ku.click(function (e, stop) {
				var $cb = $(this).find('input');
				$cb.prop("checked", !$cb.prop("checked"));
				checkboxHandler.call(this);
				invokeUpdateName();
			});

			$("#" + options.titleId).keyup(invokeUpdateName);

			getName(options.titleId, options.nameId, options.whitespaceReplacement, options.toLower, options.replacements, options.keepUpdatedBoxId, function callback(expected) {
				var actual = $("#" + options.nameId).attr("value");
				if (!expected || !actual || (expected == actual))
					$ku.each(checkboxHandler)
				else
					$ku.trigger('click');
			});
		}
	};
})(jQuery);;