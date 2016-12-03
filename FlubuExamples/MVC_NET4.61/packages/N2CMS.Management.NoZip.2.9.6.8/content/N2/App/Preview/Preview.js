n2 = window.n2 || {};

n2.preview = angular.module('n2preview', ['n2.directives', 'n2.services'], function () {
});

n2.preview.factory("Organizable", ["$window", "Context", "Uri", function ($window, Context, Uri) {
	function Organizable() {
		this.$getDropPoint = function(){
			var $dropPoint = $(this.$element).is(".n2-editable")
				? $(this.$element)
				: $(this.$element).children(".n2-drop-area.n2-append,.titleBar");
			return $dropPoint;
		}
		this.reveal = function () {
			var $dropPoint = this.$getDropPoint();
			$("html,body").animate({ scrollTop: $dropPoint.offset().top - window.innerHeight / 3 }, function () {
				$dropPoint[0].scrollIntoViewIfNeeded();
			})
		}
		this.highlight = function () {
			var $dropPoint = this.$getDropPoint();
			$(".n2-highlighted").removeClass("n2-highlighted");
			$dropPoint.addClass("n2-highlighted");
		}
		this.createUrl = function (template, beforePart) {
			var qs = {
				zoneName: this.zone || this.name,
				n2versionIndex: Context.CurrentItem.VersionIndex,
				n2scroll: document.body.scrollTop,
				belowVersionKey: this.versionKey,
				n2reveal: this.leash,
				returnUrl: encodeURIComponent(window.location.pathname + window.location.search)
			};
			qs[Context.Paths.SelectedQueryKey] = this.path;
			if (beforePart) {
				angular.extend(qs, { before: !beforePart.versionKey && beforePart.path, beforeSortOrder: beforePart.sortOrder, beforeVersionKey: beforePart.versionKey });
			} else {
				angular.extend(qs, { below: this.path, n2versionKey: this.versionKey, belowVersionKey: this.versionKey });
			}
			var uri = new Uri(template.EditUrl).setQuery(qs);
			return uri.toString();
		}
		this.moveUrl = function (part, beforePart) {
			return "#";
		}
	}

	return new Organizable();
}]);

n2.preview.factory("ZoneFactory", ["$window", "Context", "Uri", "PartFactory", "Organizable", function ($window, Context, Uri, PartFactory, Organizable) {

	function Zone(element, $scope) {
		var zone = this;
		this.isZone = true;
		this.name = $(element).attr("data-zone");
		this.title = element.title || this.name;
		this.allowed = $(element).attr("data-allowed").split(",");
		this.path = $(element).attr("data-item");
		this.versionKey = $(element).attr("data-versionKey");
		this.$element = element;
		this.parts = [];
		if ($(element).closest(".dropZone").attr("data-versionIndex")) {
			this.versionIndex = $(element).closest(".dropZone").attr("data-versionIndex");
			this.versionKey = $(element).closest(".dropZone").attr("data-versionKey");
		}
		this.addPlaceholders = function (template, callback) {
			var url = template.Discriminator
				? this.createUrl(template)
				: this.moveUrl(template);
			$("<div class='n2-drop-area-marker n2-append'><a href='" + url + "'><span>" + "Append to <b>" + this.title + "</b></span></a></div>")
				.click(function (e) {
					callback && callback(e, zone);
				})
				.appendTo(this.$element);

			angular.forEach(zone.parts, function (part, index) {
				part.addPlaceholders(template, callback, index == 0);
			})

			setTimeout(function () {
				$(".n2-drop-area-marker").addClass("n2-drop-area");
			});
		};
		this.removePlaceholders = function () {
			$(".n2-drop-area-marker", this.$element).removeClass("n2-drop-area");
			setTimeout(function () {
				$(".n2-drop-area-marker").remove();
			}, 500);
		}

		$(".zoneItem", element).each(function () {
			if ($(this).closest(".dropZone")[0] != element)
				return; // sub-zone's items
			var part = new PartFactory(this, zone, $scope);
			zone.parts.push(part);
		});

		return this;
	}
	Zone.prototype = Organizable;

	return Zone;
}]);

n2.preview.factory("PartFactory", ["$window", "Context", "Uri", "Organizable", function ($window, Context, Uri, Organizable) {

	function Part(element, zone, $scope) {
		var part = this;
		this.isPart = true;
		this.zone = zone.name;
		this.id = $(element).attr("data-id");
		this.path = $(element).attr("data-item");
		this.versionKey = $(element).attr("data-versionkey")
		this.sortOrder = $(element).attr("data-sortorder");
		this.type = $(element).attr("data-type");
		this.leash = "part" + (this.versionKey || this.id || "").replace(/\//g, "-");
		this.$element = element;

		this.addPlaceholders = function (template, callback, first) {
			var url = template.Discriminator
				? this.createUrl(template, part)
				: this.moveUrl(template, part);
			$("<div class='n2-drop-area-marker n2-prepend'><a href='" + url + "'><span>" + (first ? "Prepend to " : "Insert into ") + "<b>" + zone.title + "</b></span></a></div>")
				.click(function (e) {
					callback && callback(e, zone, part);
				})
				.prependTo(part.$element);
		}

		$(this.$element).on("click", ".titleBar .move", function (e) {
			e.preventDefault();
			e.stopPropagation();
			$scope.$apply(function () {
				$scope.$emit("move-requested", part);
			})
		})

		$(this.$element).hover(function (e) {
			e.stopPropagation();
			$(this).addClass("n2-part-hover").parents(".n2-part-hover").removeClass("n2-part-hover");
		}, function (e) {
			e.stopPropagation();
			$(this).removeClass("n2-part-hover");
		})

		$(this.$element).attr("data-leash", this.leash);

		return this;
	}
	Part.prototype = Organizable;

	return Part;
}]);

n2.preview.factory("EditableFactory", ["Context", "Uri", "Organizable", function (Context, Uri, Organizable) {

	function Editable(element, $scope) {
		var editable = this;

		this.isEditable = true;
		this.$element = element;
		this.displayable = $(element).attr("data-displayable");
		this.id = $(element).attr("data-id");
		this.path = $(element).attr("data-path");
		this.property = $(element).attr("data-property");
		this.title = $(element).attr("title") || this.property;
		this.versionIndex = $(element).attr("data-versionindex");
		this.versionKey = $(element).attr("data-versionkey");
		
		this.leash = "editable" + (this.id || this.path || this.versionKey || "").replace(/\//g, "-") + "-" + this.property;
		$(this.$element).attr("data-leash", this.leash);

		this.enableEditing = function () {
			var url = new Uri(Context.Paths.Management + "Content/EditSingle.aspx")
				.setQuery(Context.Paths.SelectedQueryKey, this.path)
				.setQuery(Context.Paths.ItemQueryKey, this.id)
				.setQuery({
					below: this.path,
					property: this.property,
					n2versionKey: this.versionKey,
					n2versionIndex: this.versionIndex,
					n2reveal: this.leash,
					returnUrl: encodeURIComponent(window.location.pathname + window.location.search)
				});
			$("<a class='n2-editable-link' href='" + url + "'><b class='fa fa-pencil'></b> <span>" + "Edit " + this.property + "</span></a>")
				.prependTo(this.$element);
			$(this.$element).addClass("n2-editable").on("dblclick", function (e) {
				e.stopPropagation();
				window.location = url.toString();
			});
		}
		return this;
	}
	Editable.prototype = Organizable;
	return Editable;
}]);

n2.preview.factory("ZoneOperator", ["$window", "Context", "Uri", "ZoneFactory", "EditableFactory", function ($window, Context, Uri, ZoneFactory, EditableFactory) {
	
	function ZoneOperator($scope) {
		var operator = this;
		this.zones = [];
		this.names = [];
		this.editables = [];

		this.removePlaceholders = function () {
			angular.forEach(this.zones, function (zone) {
				zone.removePlaceholders();
			});
		}

		this.enableEditables = function () {
			angular.forEach(this.editables, function (editable) {
				editable.enableEditing();
			});
		}

		this.reveal = function (leash) {
			angular.forEach(this.zones, function (z) {
				angular.forEach(z.parts, function (p) {
					if (p.leash == leash) {
						p.reveal();
					}
				});
			});
			angular.forEach(this.editables, function (e) {
				if (e.leash == leash) {
					e.reveal();
				}
			});
		}

		$(".dropZone", $window.document).each(function () {
			var zone = new ZoneFactory(this, $scope);
			operator.zones.push(zone);
			operator.names.push(zone.name);
		});
		$(".editable", $window.document).each(function () {
			var editable = new EditableFactory(this, $scope);
			operator.editables.push(editable);
		});
	}
	return ZoneOperator;
}]);

n2.preview.factory("Mode", ["$window", function ($window) {
	return (/edit=([^&]*)/.exec($window.location.search) || [])[1] || "view";
}]);

n2.preview.factory("Reveal", ["$window", function ($window) {
	return (/n2reveal=([^&]*)/.exec($window.location.search) || [])[1] || "";
}]);

n2.preview.factory("Context", ["$window", function ($window) {
	return $window.n2.settings;
}]);

n2.preview.factory("Fullscreen", ["$window", function ($window) {
	try {
		return !window.top.n2ctx || !window.top.n2ctx.hasTop();
	} catch (e) {
		console.warn(e);
		return true;
	}
}]);

n2.preview.directive("n2Preview", ["$http", "$templateCache", "$compile", "Paths", "Context", function ($http, $templateCache, $compile, Paths, Context) {
	Paths.initialize(Context.Paths);

	return {
		link: function(scope, element){
			$http.get("/N2/App/Preview/PreviewBar.html", { cache: $templateCache }).success(function (response) {
				element.html(response);
				$compile(element.contents())(scope);
				element.removeClass("n2-loading").addClass("n2-loaded");
			});
		},
		controller: function ($scope, Mode, Fullscreen, ContentFactory, Security, Uri) {
			var Content = ContentFactory(Context.Paths);

			function appendSelection(url, ci, appendVersionIndex) {
				var uri = new Uri(url).appendQuery(Context.Paths.SelectedQueryKey, ci.Path).appendQuery(Context.Paths.ItemQueryKey, ci.ID);
				if (appendVersionIndex)
					uri = uri.appendQuery("n2versionIndex", ci.VersionIndex);
				return uri.toString();
			}
			
			$scope.mode = Mode;
			$scope.dragging = $scope.mode == "drag";
			$scope.fullscreen = Fullscreen;
			$scope.Context = Context;
			$scope.$watch("Context.CurrentItem", function (ci) {
				$scope.Paths = {
					management: appendSelection(Context.Paths.Management, ci),
					create: appendSelection(Context.Paths.Create, ci),
					edit: appendSelection(Context.Paths.Edit, ci, /*appendVersionIndex*/true),
					remove: appendSelection(Context.Paths.Delete, ci),
					discard: appendSelection(Context.Paths.Management + "Content/DiscardPreview.aspx", ci, /*appendVersionIndex*/true),
					preview: Context.Paths.PreviewUrl,
					organize: new Uri(Context.Paths.PreviewUrl).appendQuery("edit", "drag", /*appendVersionIndex*/true).toString(),
					publish: appendSelection(Context.Paths.Management + "Content/PublishPreview.aspx", ci, /*appendVersionIndex*/true)
				}
				var permissions = $scope.Permissions = {
					write: ci.MaximumPermission >= Security.permissions.Write,
					publish: ci.MaximumPermission >= Security.permissions.Publish,
					administer: ci.MaximumPermission >= Security.permissions.Administer
				}
				var states = $scope.States = {
					draft: Content.states.is(ci.State, Content.states.Draft),
					waiting: Content.states.is(ci.State, Content.states.Waiting),
					published: Content.states.is(ci.State, Content.states.Published),
					unpublished: Content.states.is(ci.State, Content.states.Unpublished),
					deleted: Content.states.is(ci.State, Content.states.Deleted)
				}
				$scope.publishable = permissions.publish && Content.states.is(ci.State, Content.states.Draft);
				//$scope.publishableFuture = permissions.publish && Content.states.is(ci.State, Content.states.waiting);
				$scope.deletable = permissions.publish;
				$scope.discardable = permissions.write && Content.states.is(ci.State, Content.states.Draft);
			});

			if (Context.ActivityTracking.Path) {
				var handle = setInterval(function () {
					$.get(appendSelection(Context.ActivityTracking.Path + '?activity=View', Context.CurrentItem), function (result) {
						try { n2 && n2.context && n2.context(result) } catch (ex) { console.log(ex); }
					}).fail(function (result) {
						try { n2 && n2.failure && n2.failure(result) } catch (ex) { console.log(ex); }
					});
				}, Context.ActivityTracking.Interval * 1000);
				$scope.$on("$destroy", function () {
					clearInterval(handle);
				});
			}

		}
	}
}])

n2.preview.directive("n2PreviewBar", [function () {
	return {
		controller: function ($scope) {
			//console.log("PreviewBarCtrl", $scope);
		}
	}
}])

n2.preview.directive("n2PreviewParts", [function () {
	return {
		controller: function ($scope, $location, Context, Content, ZoneOperator, Reveal) {
			var operator = new ZoneOperator($scope);

			$scope.toggleParts = function () {
				$scope.adding = null;
				if ($scope.templates)
					$scope.templates = null;
				else
					$scope.templates = Context.Templates
			}

			$scope.scrollTo = function (zone) {
				zone.reveal();
				zone.highlight();
			}

			// add

			$scope.beginAdding = function (template) {
				$scope.adding = {
					zones: [],
					template: template
				};
				$scope.templates = null;
				angular.forEach(operator.zones, function (zone) {
					if (zone.allowed.indexOf(template.Discriminator) >= 0) {
						$scope.adding.zones.push(zone);
						zone.addPlaceholders(template);
					}
				})
			}

			$scope.cancelAdding = function () {
				$scope.adding = null;
				operator.removePlaceholders();
			}

			// move

			$scope.$on("move-requested", function (e, part) {
				$scope.moving = {
					part: part,
					zones: []
				};
				angular.forEach(operator.zones, function (zone) {
					if (zone.allowed.indexOf(part.type) >= 0) {
						$scope.moving.zones.push(zone);
						zone.addPlaceholders(part, function (e, destinationZone, beforePart) {
							e.preventDefault();
							
							var request = {
								to: destinationZone.path,
								zone: destinationZone.name,
								below: destinationZone.path,
								belowVersionKey: destinationZone.versionKey,
								before: beforePart && beforePart.path,
								beforeVersionKey: beforePart && beforePart.versionKey,
								n2versionIndex: destinationZone.versionIndex
							}
							request[Context.Paths.SelectedQueryKey] = part.path;
							request[Context.Paths.ItemQueryKey] = part.id;
							request["n2versionKey"] = part.versionKey;

							Content.organize(request, function () {
								window.location.reload();
							})
						});
					}
				})
			});

			$scope.cancelMoving = function () {
				$scope.moving = null;
				operator.removePlaceholders();
			}

			// init

			operator.enableEditables();
			if (Reveal)
				operator.reveal(Reveal);
		}
	}
}])

n2.preview.directive("n2PreviewDraft", [function () {
	return {
		controller: function ($scope, $filter, Context, Uri) {
			if (Context.Draft) {
				$scope.tooltip = $filter('date')("medium", Context.Draft.Saved)
					+ " - " + Context.Draft.SavedBy;
				$scope.href = new Uri(Context.CurrentItem.Url).setQuery("n2versionIndex", Context.Draft.VersionIndex).toString();
			}
		}
	}
}])

n2.preview.directive("n2PreviewMore", [function () {
	return {
		controller: function ($scope) {

			$scope.toggleMore = function () {
				$scope.showMore = !$scope.showMore;
			}
		}
	}
}])
