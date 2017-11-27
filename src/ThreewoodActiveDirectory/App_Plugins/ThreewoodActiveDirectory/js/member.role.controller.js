'use strict';

(function () {
    // Create controller variable
    function ThreewoodActiveDirectoryMemberRoleViewController($scope, $http, $attrs) {

        var API_ROOT = 'ThreewoodActiveDirectory/Dashboard/';
        var WIDTH_OFFSET = 50;

        $scope.roles = [];
        $scope.isAllSelected = {} || $scope.isAllSelected;
        $scope.isAllSelected.value = false;

        var getRoles = function () {
            $scope.showLoader = true;
            return $http({
                method: 'GET',
                url: API_ROOT + 'GetAllMemberRoles'
            }).then(function successCallback(response) {
                angular.forEach(response.data, function(item){
                    $scope.roles.push({ value: item, selected: false })
                });

                $scope.showLoader = false;
            }, function errorCallback(response) {
                $scope.showLoader = false;
            });
        };

        getRoles();

        $scope.checkboxwidth = $attrs.$$element.context.clientWidth - WIDTH_OFFSET + "px";              

        $scope.toggleAll = function () {
            var toggleStatus = $scope.isAllSelected.value;
            angular.forEach($scope.roles, function (itm) { itm.selected = toggleStatus; });

        }

        $scope.optionToggled = function () {
            $scope.isAllSelected.value = $scope.roles.every(function (itm) { return itm.selected; })
        }
        
    };

    // Register the controller    
    angular.module("umbraco").controller("Threewood.ActiveDirectory.MemberRole.ViewController", ThreewoodActiveDirectoryMemberRoleViewController);
})();