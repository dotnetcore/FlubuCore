(function (module) {
	module.factory('LocationKeeper', function ($rootScope, $routeParams, $location, Content) {
		$rootScope.$on("contextchanged", function (s, ctx) {
			$location.search(Content.applySelection({}, ctx.CurrentItem))
				.replace();
		});
		return {};
	});

	module.factory("EbbCallbacks", function (Eventually) {
		return function (callback, ms, onWorkCancelled, parallelWorkGroup) {
			return function () {
				Eventually(callback, ms, onWorkCancelled, parallelWorkGroup, arguments);
			}
		}
	});

	module.factory('Eventually', function ($timeout) {
		return (function () {
			// clears the previous action if a new event is triggered before the timeout
			var timer = 0;
			var timers = {};
			return function(callback, ms, onWorkCancelled, parallelWorkGroup, callbackArguments) {
				if (!!parallelWorkGroup) {
					if (timers[parallelWorkGroup]) {
						$timeout.cancel(timers[parallelWorkGroup]);
						onWorkCancelled();
					}
					timers[parallelWorkGroup] = $timeout(function() {
						timers[parallelWorkGroup] = null;
						callback.apply(null, callbackArguments);
					}, ms);
				} else {
					timer && onWorkCancelled && onWorkCancelled();
					timer && $timeout.cancel(timer);
					timer = $timeout(function() {
						timer = 0;
						callback.apply(null, callbackArguments);
					}, ms);
				}
			};
		})();
	});

	window.frameHost = {
		notify: function() {
		}
	};
	module.factory('FrameManipulator', function () {
		var frameManipulator = {
			host: window.frameHost,
			click: function (selector) {
				var pf = window.frames.preview;
				pf && pf.frameInteraction && pf.frameInteraction.execute(selector);
			},
			isReady: function () {
				var pf = window.frames.preview;
				return pf && pf.frameInteraction && pf.frameInteraction.ready;
			},
			hideToolbar: function (force) {
				var pf = window.frames.preview;
				pf && pf.frameInteraction && pf.frameInteraction.hideToolbar(force);
			},
			getFrameActions: function () {
				var pf = window.frames.preview;
				return pf && pf.frameInteraction && pf.frameInteraction.getActions();
			},
			getFlags: function () {
				var pf = window.frames.preview;
				var flags = pf && pf.frameInteraction && pf.frameInteraction.getFlags && pf.frameInteraction.getFlags();
				return flags || [];
			}
		};

		return frameManipulator;
	});

	module.factory('FrameContext', function ($rootScope) {
		var context = { Flags: {} };
		$rootScope.$on("contextchanged", function (scope, ctx) {
			context = ctx;
		});
		var lastFlags = {};
		function objSize(o) {
			var i = 0;
			angular.forEach(o, function () { i++ });
			return i;
		}
		window.top.n2ctx = {
			refresh: function (ctx) {
			},
			select: function () {
			},
			unselect: function(){
			},
			update: function () {
			},
			hasTop: function () {
				return "metro";
			},
			toolbarSelect: function () {
			},
			context: function (ctx) {
				if (ctx.Messages && ctx.Messages.length) {
					$rootScope.$broadcast("changecontext", { Messages: ctx.Messages });
				}
				if (ctx.Flags) {
					if (objSize(lastFlags) || objSize(ctx.Flags)) {
						var flagsChagned = false;
						angular.forEach(lastFlags, function (value, flag) {
							if (context.Flags[flag]) {
								context.Flags[flag] = false;
								flagsChagned = true;
							}
								
						});
						angular.forEach(ctx.Flags, function (value, flag) {
							if (!context.Flags[flag]) {
								context.Flags[flag] = true;
								flagsChagned = true;
							}
						});
						lastFlags = ctx.Flags;
						if (flagsChagned && !$rootScope.$$phase)
							$rootScope.$apply();
					}
				}
			},
			failure: function (response) {
				$rootScope.$broadcast("communicationfailure", { status: response.status, statusText: response.statusText });
			},
			isFlagged: function (flag) {
				return context.Flags[flag];
			},
			notifyDraft: function (draft) {
				console.log(draft, window);
			}
		};
		return window.top.n2ctx;
	});

	module.factory('Content', function (ContentFactory, Paths) {
		return ContentFactory(Paths);
	});

	module.factory('ContentFactory', function ($resource) {
		return function (paths) {
			var res = $resource(paths.Management + 'Api/Content.ashx/:target', { target: '' }, {
				'children': { method: 'GET', params: { target: 'children' } },
				'branch': { method: 'GET', params: { target: 'branch' } },
				'tree': { method: 'GET', params: { target: 'tree' } },
				'ancestors': { method: 'GET', params: { target: 'ancestors' } },
				'node': { method: 'GET', params: { target: 'node' } },
				'parent': { method: 'GET', params: { target: 'parent' } },
				'search': { method: 'GET', params: { target: 'search' } },
				'translations': { method: 'GET', params: { target: 'translations' } },
				'versions': { method: 'GET', params: { target: 'versions' } },
				'definitions': { method: 'GET', params: { target: 'definitions' } },
				'templates': { method: 'GET', params: { target: 'templates' } },
				'move': { method: 'POST', params: { target: 'move' } },
				'organize': { method: 'POST', params: { target: 'organize' } },
				'sort': { method: 'POST', params: { target: 'sort' } },
				'remove': { method: 'POST', params: { target: 'delete' } },
				'removeMessage': { method: 'DELETE', params: { target: 'message' } },
				'publish': { method: 'POST', params: { target: 'publish' } },
				'unpublish': { method: 'POST', params: { target: 'unpublish' } },
				'schedule': { method: 'POST', params: { target: 'schedule' } },
				'discard': { method: 'POST', params: { target: 'discard' } }
			});

			res.paths = paths;

			res.applySelection = function(settings, currentItem) {
				var path = currentItem && currentItem.Path;
				var id = currentItem && currentItem.ID;

				if (typeof currentItem == "string") {
					path = currentItem;
				} else if (typeof currentItem == "number") {
					id = currentItem;
				}

				if (path || id) {
					var selection = {};
					selection[paths.SelectedQueryKey] = path;
					selection[paths.ItemQueryKey] = id;
					return angular.extend(selection, settings);
				}
				return settings;
			};

			res.loadChildren = function (node, callback) {
				if (!node)
					return;

				node.Loading = true;
				return res.children(res.applySelection({}, node.Current), function (data) {
					node.Children = data.Children;
					delete node.Loading;
					node.IsPaged = data.IsPaged;
					node.HasChildren = data.Children.length > 0;
					callback && callback(node);
				});
			};

			res.unloadChildren = function (node, callback) {
				if (node) node.Children = [];
				callback && callback(node);
			};

			res.reload = function (node, callback) {
				if (!node)
					return;

				node.Loading = true;
				res.node(res.applySelection({ }, node.Current), function (data) {
					node.Current = data.Node.Current;
					delete node.Loading;
					callback && callback(node);
				});
			};

			res.states = {
				None: 0,
				New: 1,
				Draft: 2,
				Waiting: 4,
				Published: 16,
				Unpublished: 32,
				Deleted: 64,
				All: 2 + 4 + 8 + 16 + 32 + 64,
				is: function (actual, expected) {
					return (actual & expected) == expected;
				},
				toString: function (state) {
					for (var key in res.states)
						if (res.states[key] == state)
							return key;
					return null;
				}
			};
		
			return res;
		}
	});

	module.factory('Context', function ($resource) {
		var res = $resource('Api/Context.ashx/:target', { target: '' }, {
			'interface': { method: 'GET', params: { target: 'interface' } },
			'full': { method: 'GET', params: { target: 'full' } },
			'messages': { method: 'GET', params: { target: 'messages' } },
			'status': { method: 'GET', params: { target: 'status' } }
		});

		return res;
	});

	module.factory('Profile', function ($resource) {
		var res = $resource('Api/Profile.ashx', {}, {
		});

		return res;
	});

	module.factory('Security', function ($resource) {
		var res = $resource('Api/Security.ashx', {}, {});
		res.permissions = {
			None: 0,
			Read: 1,
			Write: 2,
			Publish: 4,
			Administer: 8,
			ReadWrite: 3,
			ReadWritePublish: 7,
			Full: 13,
			is: function (actual, expected) {
				if (expected === null)
					return true;
				return actual <= expected;
			}
		};
		return res;
	});

	module.factory('Notify', function () {
		var callbacks = [];
		var notify = {
			subscribe: function (callback) {
				callbacks.push(callback);
			},
			unsubscribe: function (callback) {
				callbacks.slice(callbacks.indexOf(callback), 1);
			},
			show: function (options) {
				angular.forEach(callbacks, function (cb) { cb(options); });
			}
		};
		return notify;
	});

	module.factory("Paths", function () {
		return {
			Management: "",
			SelectedQueryKey: "selected",
			ItemQueryKey: "n2item",
			initialize: function (paths) {
				angular.extend(this, paths);
			}
		};
	});

	module.factory('ContextMenuFactory', function () {
		return function (scope) {
			var contextMenu = this;

			contextMenu.appendSelection = function (url, appendPreviewQueries) {
				url = scope.appendQuery(url, scope.Context.Paths.SelectedQueryKey + "=" + contextMenu.CurrentItem.Path + "&" + scope.Context.Paths.ItemQueryKey + "=" + contextMenu.CurrentItem.ID);
				if (appendPreviewQueries) {
					for (var key in scope.Context.PreviewQueries) {
						url += "&" + key + "=" + scope.Context.PreviewQueries[key];
					}
				}
				return url;
			}

			contextMenu.show = function (node) {

				scope.ContextMenu.node = node;
				scope.ContextMenu.options = [];
				scope.ContextMenu.CurrentItem = node.Current;

				for (var i in scope.Context.ContextMenu.Children) {
					var cm = scope.Context.ContextMenu.Children[i];
					scope.ContextMenu.options.push(cm.Current);
				}
			};
			contextMenu.hide = function() {
				delete scope.ContextMenu.node;
				delete scope.ContextMenu.CurrentItem;
				delete scope.ContextMenu.options;
				delete scope.ContextMenu.memory;
				delete scope.ContextMenu.action;
			};
			contextMenu.cut = function(node) {
				contextMenu.memory = node.Current;
				contextMenu.action = "move";

			};
			contextMenu.copy = function(node) {
				contextMenu.memory = node.Current;
				contextMenu.action = "copy";
			};
		};
	});

	module.factory('Confirm', function ($rootScope) {
	    return function (settings) {
	        $rootScope.$emit("confirm", settings);
	    };
	});

	module.factory('SortHelperFactory', function ($timeout, Content, Notify, Translate, Confirm) {
		var context = {}
		return function (scope) {
			function reload(ctx) {
				var node = ctx.scopes.to && ctx.scopes.to.node;
				if (!node) return;

				node.HasChildren = true;
				node.Loading = true;
				Content.children(Content.applySelection({}, node.Current), function (data) {
					node.Children = data.Children;
					node.Expanded = true;
					node.Loading = false;
					if (data.IsPaged)
						node.IsPaged = true;
				});

				scope.reloadChildren(ctx.scopes.from.node);
			}
			this.move = function (ctx) {
			    Confirm({
			        title: Translate("confirm.move.title"),
			        moved: ctx.scopes.selected.node.Current,
			        destination: ctx.scopes.to.node.Current,
			        template: "<div class='alert alert-info' translate='confirm.move.info'>This may break inbound links</div>"
                            + "<p><label translate='confirm.move.moved'>Moved</label><b class='ico' ng-show='settings.moved.IconClass || settings.moved.IconUrl' ng-class='settings.moved.IconClass' x-background-image='settings.moved.IconUrl'></b> {{settings.moved.Title}}<p>"
                            + "<p><label translate='confirm.move.destination'>Destination</label><b class='ico' ng-show='settings.destination.IconClass || settings.destination.IconUrl' ng-class='settings.destination.IconClass' x-background-image='settings.destination.IconUrl'></b> {{settings.destination.Title}}<p>",
			        confirmed: function () {
			            Content.move(ctx.paths, function () {
			                reload(ctx);
			                Notify.show({ message: "Moved " + (ctx.scopes.selected && ctx.scopes.selected.node && ctx.scopes.selected.node.Current.Title), type: "success", timeout: 3000 });
			            }, function () {
			            	reload(ctx);
			            	Notify.show({ message: "Failed moving " + (ctx.scopes.selected && ctx.scopes.selected.node && ctx.scopes.selected.node.Current.Title), type: "error" });
			            });
			            ctx.callback && ctx.callback();
			        },
			        cancelled: function () {
			            scope.reloadChildren(ctx.scopes.from.node);
			            scope.reloadChildren(ctx.scopes.to.node);
			        }
			    });
			};
			this.sort = function (ctx) {
				Content.sort(ctx.paths, function () {
					reload(ctx);
					Notify.show({ message: "Sorted " + (ctx.scopes.selected && ctx.scopes.selected.node && ctx.scopes.selected.node.Current.Title), type: "success", timeout: 3000 });
					ctx.callback && ctx.callback();
				}, function () {
					Notify.show({ message: "Failed sorting " + (ctx.scopes.selected && ctx.scopes.selected.node && ctx.scopes.selected.node.Current.Title), type: "error" });
				});
			};

			return this;
		};
	});

	module.factory('Uri', function () {
		function Uri(uri) {
			this.$uri = uri || "";
			this.getSeparator = function() {
				return this.$uri.indexOf("?") >= 0 ? "&" : "?";
			};
			this.appendQuery = function (key, value) {
				return new Uri(this.$uri + this.getSeparator() + key + "=" + value);
			};
			this.setQuery = function (key, value) {
				if (typeof key == "object") {
					var uri = new Uri(this.$uri);
					angular.forEach(key, function (value, key) {
						uri = uri.setQuery(key, value);
					});
					return uri;
				} else if (!value)
					return this;

				var queryIndex = this.$uri.indexOf("?");
				if (queryIndex < 0)
					return this.appendQuery(key, value);
				var qs = this.$uri.substr(queryIndex + 1).split("&");
				var modified = false;
				for (var i = 0; i < qs.length; i++) {
					if (qs[i] == key || qs[i].indexOf(key + "=") == 0) {
						qs[i] = key + "=" + value;
						modified = true;
						break;
					}
				}
				if (!modified)
					return this.appendQuery(key, value);
				
				return new Uri(this.$uri.substr(0, queryIndex + 1) + qs.join("&"));
			};
			this.toString = function () {
				return this.$uri;
			};
		};
		return Uri;
	});
})(angular.module('n2.services', ['ngResource']));