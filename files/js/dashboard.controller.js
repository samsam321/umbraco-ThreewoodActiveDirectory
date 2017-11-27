'use strict';
angular.module('umbraco').requires.push('smart-table');

(function () {
    // Create controller variable
    function ThreewoodActiveDirectoryDashboardViewController($scope, $http, $injector, userService, notificationsService, dialogService) {

        var rowCollection = (function () {
            var rowCollection = {
                totalPages: 0,
                totalItems: 0,
                items: []
            };
            return rowCollection;
        })();
        
        $scope.user = userService.getCurrentUser();
        $scope.rowCollection = rowCollection;
        $scope.itemsByPage = 15;
        $scope.selectedItem = {
            value : null
        };
        
        var API_ROOT = 'ThreewoodActiveDirectory/Dashboard/';

        $scope.modalShown = false;
        $scope.actionName = "";
        $scope.toggleModal = function (action) {
            $scope.actionName = action;
            $scope.modalShown = !$scope.modalShown;
        };

        $scope.isTriggered = $scope.isTriggered || {};
        $scope.isTriggered.value = false;    

        $scope.getAllADUsers = function () {
            var fn = function () {                
                $scope.showLoader = true;                
                return $http({
                    method: 'GET',
                    url: API_ROOT + 'GetAllUsers'
                }).then(function successCallback(response) {
                    $scope.rowCollection.items = response.data;
                    $scope.rowCollection.totalItems = response.data.length;                    
                    $scope.isTriggered.value = !$scope.isTriggered.value; // trigger refresh the smart table
                    $scope.showLoader = false;
                    notificationsService.success("Success", "AD user loaded successfully");                    
                }, function errorCallback(response) {
                    $scope.rowCollection.totalItems = 0;
                    $scope.showLoader = false;
                    notificationsService.error("Success", "AD user load failure");
                });
            };
            return fn;
        };

        $scope.getAllMemberRole = function (users) {                        
            var dialog = dialogService.open(
            {                    
                template: "/App_Plugins/ThreewoodActiveDirectory/template/member.role.html",                                     
                callback: done
            });
            function done(data)
            {
                $scope.showLoader = true;

                var makeMembers = function (data) {

                    var r = data.filter(function (item) {
                        return item.selected === true;
                    });
                    var f = [];
                    angular.forEach(r, function (item) {
                        f.push(item.value)
                    });

                    return { users: users, roles: f }
                };

                var members = makeMembers(data);                

                return $http({
                    method: 'POST',
                    url: API_ROOT + 'ImportUsers',
                    data: members
                }).then(function successCallback(response) {
                    $scope.showLoader = false;
                    notificationsService.success("Success", response.data.Message);
                }, function errorCallback(response) {
                    $scope.showLoader = false;
                    notificationsService.error("Success", "AD user import failure");
                });
                
            }            
        };

        $scope.ImportSelectedUsers = function () {
            var fn = function () {
                $scope.showLoader = true;
                var selectedRow = getSelectedRows();
                if (selectedRow.length >= 1)
                {
                    $scope.getAllMemberRole(selectedRow);
                }
                else
                {
                    notificationsService.warning("Action Aborted", "No user selected!")
                }
                $scope.showLoader = false;
            };
            return fn;
        }; 

        var getSelectedRows = function () {
            var selected;
            selected = $scope.selectedItem.value.filter(function (item) {
                return item.isSelected === true;
            });
            return selected;
        };

        $scope.DeleteAllMembers = function () {
            var fn = function () {
                $scope.showLoader = true;
                return $http({
                    method: 'DELETE',
                    url: API_ROOT + 'DeleteAllMember'
                }).then(function successCallback(response) {
                    $scope.rowCollection.items = response.data;
                    $scope.rowCollection.totalItems = response.data.length;
                    $scope.showLoader = false;
                    notificationsService.success("Success", "Delete All Members successfully");
                }, function errorCallback(response) {
                    $scope.rowCollection.totalItems = 0;
                    console.log(response);
                    $scope.showLoader = false;
                    notificationsService.error("Success", "Delete All Members failure");
                });
            };
            return fn;
        };
    };


    // Register the controller    
    angular.module("umbraco").controller("Threewood.ActiveDirectory.Dashboard.ViewController", ThreewoodActiveDirectoryDashboardViewController);
})();