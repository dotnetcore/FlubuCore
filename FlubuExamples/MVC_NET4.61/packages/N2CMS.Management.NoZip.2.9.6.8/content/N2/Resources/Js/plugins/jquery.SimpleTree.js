$.fn.SimpleTree = function(opt) {
	this.each(function() {
		var TREE = this;
		var ROOT = $('li:first', this);
		TREE.option = {
			animate: false, 	// this parameter has a value "true/false" (enable/disable animation for expanding/collapsing menu items) 
			autoclose: false, // this parameter has a value "true/false" (enable/disable collapse of neighbor branches)
			speed: 'fast', 	// speed open/close folder
			success: false, 	// this parameter defines function, which executes after ajax is loaded (set to "false" by default)
			click: false		// this parameter defines function, which is executed after item clicked (set to "false" by default) 

		};
		TREE.option = $.extend(TREE.option, opt);
		TREE.setAjaxNodes = function(obj) {
			var url = $.trim($(obj).children("li").text());
			if (url && url.indexOf('url:')) {
				url = $.trim(url.replace(/.*\{url:(.*)\}/i, '$1'));
				$.ajax({
					type: "GET",
					url: url,
					contentType: 'html',
					cache: false,
					success: function(response) {
						if (response) {
							obj.removeAttr('class');
							obj.html(response);
							TREE.setTreeNodes(obj, true);
							if (typeof TREE.option.success == 'function') {
								TREE.option.success(obj);
							}
						} else {
							var parent = obj.parent();
							var pClassName = parent.attr('class');
							pClassName = pClassName.replace('folder-open', 'leaf');
							parent.attr('class', pClassName);
							obj.remove();
							$('.toggler', parent).remove();
						}
					}
				});
			}
		};
		TREE.closeNearby = function(obj) {
			$(obj).siblings().filter('.folder-open, .folder-open-last').each(function() {
				var childUl = $(this).children("ul");
				var className = this.className;
				className = className.replace('open', 'close');
				$(this).attr('class', className);
				if (TREE.option.animate) {
					childUl.animate({ height: "toggle" }, TREE.option.speed);
				} else {
					childUl.hide();
				}
			});
		};
		TREE.setEventToggler = function(obj) {
			$(obj).prepend('<a class="toggler" href="#">&nbsp;</a>');
			$('>.toggler', obj).bind('click', function(e) {
				e.preventDefault();

				var childUl = $(obj).children("ul");
				var className = obj.className;
				if (childUl.is(':visible')) {
					className = className.replace('open', 'close');
					$(obj).attr('class', className);
					if (TREE.option.animate) {
						childUl.animate({ height: "toggle" }, TREE.option.speed);
					} else {
						childUl.hide();
					}
				} else {
					className = className.replace('close', 'open');
					$(obj).attr('class', className);
					if (TREE.option.animate) {
						childUl.animate({ height: "toggle" }, TREE.option.speed, function() {
							if (TREE.option.autoclose) TREE.closeNearby(obj);
							if (childUl.is('.ajax')) TREE.setAjaxNodes(childUl);
						});
					} else {
						childUl.show();
						if (TREE.option.autoclose) TREE.closeNearby(obj);
						if (childUl.is('.ajax')) TREE.setAjaxNodes(childUl);
					}
				}
			});
		};
		TREE.setTreeNodes = function(obj, useParent) {
			obj = useParent ? obj.parent() : obj;
			$('li', obj).each(function(i) {
				var className = this.className;
				var open = false;
				var childNode = $(this).children("ul");

				if (childNode.size() > 0) {
					var setClassName = 'folder-';
					if (className && className.indexOf('open') >= 0) {
						setClassName = setClassName + 'open';
						open = true;
					} else {
						setClassName = setClassName + 'close';
					}
					this.className = setClassName + ($(this).is(':last-child') ? '-last' : '');
					TREE.setEventToggler(this);
					if (!open || className.indexOf('ajax') >= 0) childNode.hide();

				} else {
					var setClassName = 'leaf';
					this.className = setClassName + ($(this).is(':last-child') ? '-last' : '');
				}
				//				$('>.text, >.active',this).bind('click', function(){
				//					$('.active',TREE).attr('class','text');
				//					$(this).attr('class','active');
				//					if(typeof TREE.option.click == 'function')
				//					{
				//						TREE.option.click(this);
				//					}
				//				});
			});
		};


		TREE.init = function(obj) {
			TREE.setTreeNodes(obj);
		};
		TREE.init(ROOT);
	});
};;
