var n2framed = angular.module('n2framed', ['n2.directives', 'n2.services'], function () {
});

n2framed.controller("EditLayoutCtrl", ["$scope", function ($scope) {
	$scope.toggleSidebar = function () {
		$scope.sidebarOpen = !$scope.sidebarOpen;
	}
}])

n2framed.controller("CustomTranslationsCtrl", ["$scope", function ($scope) {
	$scope.update = function (key, value) {
		$scope.translations[key] = value;
	}
	$scope.remove = function (key) {
		delete $scope.translations[key];
	}
}])

n2framed.directive("customTranslations", ["$compile", function ($compile) {
	return {
		link: function (scope, element, attrs) {
			scope.translations = angular.fromJson(element.val());
			scope.$watch("translations", function (translations) {
				element.val(angular.toJson(translations));
			}, true);
			var editor = angular.element("<div class='custom-transltaions' ng-controller='CustomTranslationsCtrl' ng-include=\"'../App/Framed/CustomTranslations.html'\"></div>").insertAfter(element);
			element.hide();
			$compile(editor)(scope);
		}
	}
}])