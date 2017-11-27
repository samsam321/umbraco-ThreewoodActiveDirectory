'use strict';
var umbracoApp = angular.module('umbraco.directives');
// add ng-blur directive because this is missing in Angular 1.1.5
function ngBlur() {
    return function (scope, elem, attrs) {
        elem.bind('blur', function () {
            scope.$apply(attrs.ngBlur);
        });
    };
};

//Smart Table directive 
function pageSelect() {
    return {
        restrict: 'E',
        template: '<input type="text" class="select-page" ng-model="inputPage" ng-change="selectPage(inputPage)">',
        link: function (scope, element, attrs) {
            scope.$watch('currentPage', function (c) {
                scope.inputPage = c;
            });
        }
    }
};

function stPersist() {
    return {
        require: '^stTable',
        link: function (scope, element, attr, ctrl) {
            var nameSpace = attr.stPersist;

            //save the table state every time it changes
            scope.$watch(function () {
                return ctrl.tableState();
            }, function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    localStorage.setItem(nameSpace, JSON.stringify(newValue));
                }
            }, true);

            //fetch the table state when the directive is loaded
            if (localStorage.getItem(nameSpace)) {
                var savedState = JSON.parse(localStorage.getItem(nameSpace));
                var tableState = ctrl.tableState();

                angular.extend(tableState, savedState);
                ctrl.pipe();

            }

        }
    };
};

// Register directive
umbracoApp.directive('stPersist', stPersist);
umbracoApp.directive('pageSelect', pageSelect);
umbracoApp.directive('ngBlur', ngBlur);
//End of Smart Table directive