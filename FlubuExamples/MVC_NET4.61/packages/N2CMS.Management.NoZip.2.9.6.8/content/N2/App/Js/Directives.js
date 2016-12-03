(function (module) {

	module.filter("translate", function (Translate) {
		return function (fallback, key) {
			if (!key) console.error("No key provided for translation text ", fallback);

			return Translate(key, fallback);
		}
	});

	module.directive("translate", function (Translate) {
		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				if (!attrs.translate) console.error("No key provided for translation element", element[0]);

				var translation = Translate(attrs.translate);
				if (typeof translation == "string") {
					element.html(translation);
					return;
				}
				for (var key in translation) {
					if (key == "html")
						element.html(translation[key]);
					else if (key == "text") {
						for (var i in element[0].childNodes) {
							var node = element[0].childNodes[i]
							if (node.nodeType == Node.TEXT_NODE && node.nodeValue && node.nodeValue.replace(/\s/g, "")) {
								node.nodeValue = translation[key];
								return;
							}
						}
					}
					else
						element.attr(key, translation[key]);
				}
			}
		}
	});

	module.directive("contextMenuTrigger", function () {
		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				element.bind("contextmenu", function (e) {
					var clickedElements = $(e.target).closest(".item").find(".dropdown-toggle").trigger("click").length;
					if (clickedElements) {
						e.preventDefault();
						setTimeout(function () {
							$(e.target).closest(".item").find(".dropdown-menu li:first-child a").focus();
						});
					}
				});
			}
		}
	});

	var key = { enter: 13, esc: 27, left: 37, up: 38, right: 39, down: 40, del: 46, a: 65, c: 67, n: 78, v: 86, x: 88, z: 90 };

	angular.forEach(["Enter", "Esc", "Down", "Up"], function (k) {
		var name = "n2Key" + k;
		module.directive(name, function () {
			return {
				restrict: "A",
				link: function compile(scope, element, attrs) {
					var code = key[k.toLowerCase()];
					element.bind("keyup", function(e) {
						if (e.keyCode == code) {
							e.preventDefault();
							e.stopPropagation();
							scope.$apply(attrs[name]);
						}
					});
				}
			};
		});
	});

	module.directive("n2Focus", function () {
		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				scope.$watch(function() {
					return scope.$eval(attrs.n2Focus);
				}, function(focus) {
					if (focus) element.focus();
				});
			}
		};
	});

	module.directive("evaluateHref", function ($interpolate) {
		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				scope.$watch(attrs.evaluateHref, function(expr) {
					element.attr("href", expr && $interpolate(expr)(scope));
				});
			}
		};
	});

	module.directive("evaluateTitle", function ($interpolate) {
		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				scope.$watch(attrs.evaluateTitle, function(expr) {
					element.attr("title", expr && $interpolate(expr)(scope));
				});
			}
		};
	});

	module.directive("evaluateInnerHtml", function ($interpolate) {
		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				scope.$watch(attrs.evaluateInnerHtml, function(expr) {
					element.html(expr && $interpolate(expr)(scope));
				});
			}
		};
	});

	module.directive("pageActionIconClass", function () {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				if (attrs.pageActionIconClass)
					scope.$watch(function () {
						var i = scope.$eval(attrs.pageActionIconClass);
						return i && i.Current && (i.Current.IconClass || i.Current.IconUrl)
					}, function (icon) {
						element.addClass(icon ? "page-action-iconified" : "page-action-plain");
					});
				else
					element.addClass("page-action-iconified");
			}
		}
	});

	module.directive("pageActionLink", function ($interpolate) {
		return {
			restrict: "A",
			scope: true,
			link: function compile(scope, element, attrs) {

				function watch(expr, scope, applicator) {
					var factory = expr && $interpolate(expr);
					if (factory) {
						return scope.$watch(function() {
							return factory(scope);
						}, applicator);
					} else
						return applicator(null);
				}

				scope.evaluateExpression = function(expr) {
					return expr && $interpolate(expr)(scope);
				};
				scope.evalExpression = function(expr) {
					expr && scope.$eval(expr);
				};

				var unwatchHref, unwatchTitle, unwatchInnerHtml, unwatchCurrent;
				scope.$watch(attrs.pageActionLink, function(node) {
					scope.node = node;

					unwatchCurrent && unwatchCurrent();
					unwatchCurrent = scope.$watch(function() { return node.Current }, function(current) {
						if (!current || current.Divider) {
							element.hide();
							return;
						} else
							element.show();

						if (!current.Target)
							current.Target = "preview";
						if (!current.Url && current.PreviewUrl)
							current.Url = current.PreviewUrl;
						if (current.Url == "#") {
							element.click(function (e) {
								e.stopPropagation();
								e.preventDefault();
							});
						}

						unwatchHref && unwatchHref();
						unwatchHref = watch(current.Url, scope, function(value) { element.attr("href", value || "#"); });

						unwatchTitle && unwatchTitle();
						unwatchTitle = watch(current.ToolTip, scope, function(value) { element.attr("title", value); });

						unwatchInnerHtml && unwatchInnerHtml();

						unwatchInnerHtml = watch(
							(current.IconClass ? ("<b class='ico " + current.IconClass + "'></b> ") : current.IconUrl ? ("<b class='ico ico-custom' style='background-image:url(" + current.IconUrl + ")'></b> ") : "")
								+ "{{evaluateExpression(node.Current.Title)}}"
								+ (current.Description ? "<br /><span>{{evaluateExpression(node.Current.Description)}}</span>" : ""), scope, function(value) { element.html(value); });

						element.attr("target", current.Target);

						element.attr("class", (current.Description ? "page-action page-action-description" : "page-action") + (current.IconClass || current.IconUrl ? " page-action-iconified" : " page-action-plain"));

						element.click(function(e) {
							if (current.ClientAction) {
								e.preventDefault();
								scope.$apply(node.Current.ClientAction);
							}
							scope.$emit("nodeclicked", node);
						});
					});
				});
			}
		};
	});

	module.directive("backgroundImage", function () {
		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				scope.$watch(attrs.backgroundImage, function (backgroundImage) {
					if (backgroundImage) {
						var style = element.attr("style");
						if (style)
							style += ";";
						else
							style = "";
						style += "background-image:url(" + backgroundImage + ")";
						element.attr("style", style);
					}
				});
			}
		}
	});

	module.directive("load", function($parse){
		return function (scope, element, attr) {
			var fn = $parse(attr.load);
			element.bind("load", function (e) {
				scope.$apply(function () {
					fn(scope, { $event: e });
				});
			});
		};
	});

	module.directive("n2PreviewFrame", function (EbbCallbacks) {
		return function (scope, element, attr) {
			var src = scope.appendPreviewOptions(scope.Context.Paths.PreviewUrl);
			element.prop("src", src);
			element.prop("id", "page-preview-frame");
			element.prop("name", "preview");

			element.bind("load", EbbCallbacks(function (e) {
				try {
					scope.$apply(function () {
						var loco = e.target.contentWindow.location;
						scope.$emit("preiewloaded", { path: loco.pathname, query: loco.search, url: loco.toString() });
					});
				} catch (ex) {
					window.console && console.log("frame access exception", ex);
				}
			}, 50));
		};
	})

	module.directive("keyup", function ($parse) {
		return function (scope, element, attr) {
			var fn = $parse(attr.keyup);
			element.bind("keyup", function (e) {
				scope.$apply(function () {
					fn(scope, { $event: e });
				});
			});
		};
	});

	module.directive("esc", function ($parse) {
		return function (scope, element, attr) {
			var fn = $parse(attr.keyup);
			element.bind("keyup", function (e) {
				if (e.keyCode == 27) {
					scope.$apply(function () {
						fn(scope, { $event: e });
					});
				}
			});
		};
	});

	module.directive("sortable", function () {

		var ctx = {
		};

		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				var sort = {
					start: function (e, args) {
						var $from = $(args.item[0]).parent().closest("li");
						ctx = {
							operation: "sort",
							indexes: {
								from: args.item.index()
							},
							scopes: {
								from: $from.length && angular.element($from).scope()
							},
							paths: {
								from: $from.attr("sortable-path") || null
							}
						};
					},
					remove: function (e, args) {
					},
					update: function (e, args) {
					},
					receive: function (e, args) {
						ctx.operation = "move";
					},
					stop: function (e, args) {
						var $selected = $(args.item[0]);
						var $to = $selected.parent().closest("li");

						ctx.paths.selected = $selected.attr("sortable-path") || null;
						ctx.paths.to = $to.attr("sortable-path") || null;
						ctx.paths.before = $selected.next().attr("sortable-path") || null;

						ctx.scopes.selected = $selected.length && angular.element($selected).scope();
						ctx.scopes.to = $to.length && angular.element($to).scope();
						
						ctx.indexes.to = args.item.index();

						ctx.scopes.from.node.Children.splice(ctx.indexes.from, 1);

						ctx.callback = function () {
							$selected.remove();
						}

						var options = scope.$eval(attrs.sortable);

						if (ctx.operation == "move") {
							options.move && options.move(ctx);
						} else {
							options.sort && options.sort(ctx);
						}
						ctx.scopes.from.$digest();
						ctx.scopes.to.$digest();
						ctx = {};
					}
				};

				setTimeout(function () {
					element.sortable({
						connectWith: '.targettable',
						placeholder: 'sortable-placeholder',
						handle: '.handle',
						receive: sort.receive,
						remove: sort.remove,
						update: sort.update,
						start: sort.start,
						stop: sort.stop
					});
				}, 100);
			}
		}
	});

	module.directive('compile', function ($compile) {
		return function (scope, element, attrs) {
			scope.$watch(
				function (scope) {
					// watch the 'compile' expression for changes
					return scope.$eval(attrs.compile);
				},
				function (value) {
					if (value === null)
						return;

					// when the 'compile' expression changes assign it into the current DOM
					element.html(value);

					// compile the new DOM and link it to the current scope.
					// NOTE: we only compile .childNodes so that we don't get into infinite loop compiling ourselves
					$compile(element.contents())(scope);
				}
			);
		};
	});

	angular.forEach(['X', 'Y'], function (dir) {
		module.directive('n2Resize' + dir, function ($parse) {
			return function (scope, element, attrs) {
				var modelGet = $parse(attrs["n2Resize" + dir]);
				var modelSet = modelGet.assign;

				var initialClientValue, initialModelValue;

				element.bind("mousedown", function (e) {
					initialClientValue = e["client" + dir];
					initialModelValue = modelGet(scope);

					$(document).bind("mousemove.n2Resize", function (evt) {
						modelSet(scope, initialModelValue + evt["client" + dir] - initialClientValue);
						scope.$digest();
					});
					$(document.body).addClass("resizing");
				});
				element.bind("mouseup", function (e) {
					scope.$emit("resized", { direction: dir, from: initialModelValue, to: modelGet(scope)});
					$(document).unbind("mousemove.n2Resize");
					$(document.body).removeClass("resizing");
					initialClientValue = undefined;
					initialModelValue = undefined;
				});
			};
		});
	});

	module.filter('pretty', function () {
		function syntaxHighlight(json) {
			if (typeof json != 'string') {
				json = angular.toJson(json, true);
			}
			json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
			return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
				var cls = 'number';
				if (/^"/.test(match)) {
					if (/:$/.test(match)) {
						cls = 'key';
					} else {
						cls = 'string';
					}
				} else if (/true|false/.test(match)) {
					cls = 'boolean';
				} else if (/null/.test(match)) {
					cls = 'null';
				}
				return '<span class="' + cls + '">' + match + '</span>';
			});
		}
		return function (obj) {
			return "<style>\
span.key {color:blue}\
span.number {color:green}\
span.string {color:red}\
span.boolean {color:green}\
span.null {color:silver}\
</style><pre>" + syntaxHighlight(obj) + "</pre>";
		};
	});

	module.directive('n2DelayClass', function ($timeout, $compile) {
	    return {
	        link: function (scope, element, attrs) {
	            scope.$watch(attrs.n2DelayClass, function (options) {
	                if (!options)
	                    return;

	                $timeout(function () {
	                    for (var k in options) {
	                        if (options[k]) {
	                            element.addClass(k);
	                        } else {
	                            element.removeClass(k);
	                        }
	                    }
	                }, 10);
	            }, true);
	        }
	    };
	});

	module.directive("debug", function () {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				element.hide();
				function attach(e) {
					if (e.keyCode != 68 || !e.shiftKey || !e.altKey)
						return;

					angular.element(document).unbind("keydown", attach);
					element.show().addClass("n2-debug").html(angular.toJson(scope.$eval(attrs.debug), true));

					scope.$watch(attrs.debug, function (debug) {
						element.html(angular.toJson(debug, true));
					}, true);

					element.click(function () {
						setTimeout(function () {
							if (element.is(".n2-debug-pinned")) {
								element.toggleClass("n2-debug-pinned-left");
							} else if (document.body.createTextRange) {
								range = document.body.createTextRange();
								range.moveToElementText(element[0]);
								range.select();
							} else if (window.getSelection) {
								selection = window.getSelection();
								range = document.createRange();
								range.selectNodeContents(element[0]);
								selection.removeAllRanges();
								selection.addRange(range);
							}
						}, 200);
					});

					element.dblclick(function () {
						element.toggleClass("n2-debug-pinned");
						element.focus();
					});
				}
				angular.element(document).keydown(attach);
			}
		}
	});

	module.directive("n2StopPropagation", function () {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				element.click(function (e) {
					e.stopPropagation();
				});
			}
		}
	});

	module.factory("Keys", function () {
		return {
			esc: 27,
			left: 37,
			up: 38,
			right: 39,
			down: 40,
			pageup: 33,
			pagedown: 34
		};
	});

	module.directive("n2KeyboardHome", function (Keys, $parse, $timeout) {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				$timeout(function () {
					element.focus();
				}, 100);

				var modelSet = $parse(attrs.ngModel).assign;

				jQuery(document).keydown(function (e) {
					if (e.keyCode == Keys.esc) {
						element.focus();

						$timeout(function () {
							modelSet(scope, "");
							scope.$eval(attrs.ngChange);
							scope.$digest();
						});
					}
				})
			}
		}
	});

	module.directive("n2KeyboardStop", function (Keys) {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				var myStopClass = "n2-keyboard-stop-" + attrs.hKeyboardStop;
				element.addClass("n2-keyboard-stop")
					.addClass(myStopClass)
				element.bind("keydown", function (e) {
					if (e.keyCode == Keys.down || e.keyCode == Keys.up) {
						var $siblings = jQuery("." + myStopClass + ":visible");
						var myIndex = $siblings.index(this);
						if (myIndex > 0 && e.keyCode == Keys.up) {
							e.preventDefault();
							$siblings[myIndex - 1].focus();
						}
						if (myIndex < $siblings.length - 1 && e.keyCode == Keys.down) {
							e.preventDefault();
							$siblings[myIndex + 1].focus();
						}
					}
					if (e.keyCode == Keys.left || e.keyCode == Keys.right) {
						var columnIndex = parseInt(attrs.hKeyboardStop);
						var columnQuery = ".n2-keyboard-stop-"
							+ (e.keyCode == Keys.left ? columnIndex - 1 : columnIndex + 1)
							+ ":visible";

						var $rows = jQuery(columnQuery);
						if ($rows.length) {
							var $actives = $rows.filter(function () { return jQuery(this).parent().is(".active"); });

							if ($actives.length)
								$actives[0].focus();
							else
								$rows[0].focus();
						}
					}
				});
			}
		}
	});
	
	module.directive("n2KeyboardMap", function (Keys) {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				element.addClass("n2-keyboard-map");
				var map = undefined;
				element.bind("keydown", function (e) {
					if (!map) map = scope.$eval(attrs.n2KeyboardMap);
					angular.forEach(Keys, function (code, key) {
						if (e.keyCode == code) {
							var $target = element.find(map[key]);
							if ($target.length) {
								$target.click();
								e.stopPropagation();
							}
						}
					});
				});
			}
		}
	});

	module.directive("n2EventFocus", function ($rootScope) {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				var map = scope.$eval(attrs.n2EventFocus);
				angular.forEach(map, function (selector, eventName) {
					$rootScope.$on(eventName, function () {
						setTimeout(function () {
							if (!scope.Context.Flags.ContentEdit)
								if (element.find(selector).siblings(".dropdown.open").length == 0)
									element.find(selector).focus();
						});
					});
				})
			}
		}
	});

	module.directive("n2BindEscaped", function () {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				var message = scope.$eval(attrs.n2BindEscaped) || "";
				//message = message
				//	.replace(/&/g, '&amp;')
				//	.replace(/"/g, '&quot;')
				//	.replace(/'/g, '&#39;')
				//	.replace(/</g, '&lt;')
				//	.replace(/>/g, '&gt;');
				//console.log("msg", message, scope.message);
				element.html(message);
			}
		}
	});

	angular.forEach(["Active", "Loading"], function (key) {
		var attributeNam = "n2" + key;
		var cssClass = key.toLowerCase();
		module.directive(attributeNam, function () {
			return {
				restrict: "A",
				link: function (scope, element, attrs) {
					scope.$watch(attrs[attributeNam], function (value) { element.toggleClass(cssClass, !!value); })
				}
			}
		});
	})

	module.directive("n2DelayedActivation", function () {
		return {
			restrict: "A",
			link: function (scope, element, attrs) {
				scope.$watch(attrs.n2DelayedActivation, function (active) {
					if (active)
						setTimeout(function () { element.addClass("active"); })
					else
						setTimeout(function () { element.removeClass("active"); }, 200)
				});
			}
		}
	});
})(angular.module('n2.directives', ['n2.localization']));
