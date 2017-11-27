'use strict';
var umbracoApp = angular.module('umbraco.directives');

umbracoApp.directive('modalDialog', function () {
    return {
        restrict: 'E',
        scope: {
            show: '=',
            actionName: '&callbackFn'            
        },
        replace: true,
        transclude: true,
        link: function (scope, element, attrs) {            
            scope.dialogStyle = {};
            if (attrs.width) {
                scope.dialogStyle.width = attrs.width;
            };
            if (attrs.height) {
                scope.dialogStyle.height = attrs.height;
            };
            scope.hideModal = function () {
                scope.show = false;
            };
            scope.confirmedClick = function () {
                scope.show = false;
                scope.$eval(scope.actionName());                                
            };
        },
        templateUrl: "/App_Plugins/ThreewoodActiveDirectory/template/modal.dialog.html"        
    };
});