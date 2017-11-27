'use strict';
var umbracoApp = angular.module('umbraco.directives');

umbracoApp.directive('parseStyle', function ($interpolate) {
    return function (scope, elem) {
        var exp = $interpolate(elem.html()),
            watchFunc = function () { return exp(scope); };

        scope.$watch(watchFunc, function (html) {
            elem.html(html);
        });
    };
});