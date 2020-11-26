/// <reference path="ts.d.ts" /> 
var UserManageApp;
(function (UserManageApp) {
    var UserAdminController = /** @class */ (function () {
        function UserAdminController($scope, $q, $mdSidenav, $mdMedia, $http, $timeout, toaster) {
            this.$scope = $scope;
            this.$q = $q;
            this.$mdSidenav = $mdSidenav;
            this.$mdMedia = $mdMedia;
            this.$http = $http;
            this.$timeout = $timeout;
            this.toaster = toaster;
            this.url = "/Management/User";
            var self = this;
            // self.getUserList();
            self.userGridOptions = {
                dataSource: {
                    load: function (loadOptions) {
                        var deferred = self.$q.defer();
                        self.$http.post('/Management/User/Users', {
                            command: 'getuserlist',
                            skip: loadOptions.skip,
                            take: loadOptions.take,
                            filteroptions: loadOptions.filter ? JSON.stringify(loadOptions.filter) : "",
                            sortOptions: loadOptions.sort ? JSON.stringify(loadOptions.sort) : "",
                            requireTotalCount: loadOptions.requireTotalCount
                        })
                            .then(function (response) {
                            return deferred.resolve(response.data);
                        });
                        return deferred.promise;
                    }
                },
                remoteOperations: true,
                columns: [
                    { dataField: "master", caption: "MasterUser" },
                    { dataField: "ownerid", caption: "Owner" },
                    { dataField: "tenantid", caption: "Tenant Id" },
                    { dataField: "tenant", caption: "Tenant" },
                    //   { dataField: "id", caption: "id", },                    
                    { dataField: "userName", caption: "userName" },
                    //{ dataField:"normalizedUserName"       , caption:"normalizedUserName"     },    
                    { dataField: "email", caption: "email" },
                    // { dataField:"normalizedEmail"          , caption:"normalizedEmail"        },    
                    { dataField: "emailConfirmed", caption: "emailConfirmed" },
                    // { dataField:"passwordHash"             , caption:"passwordHash"           },    
                    //{ dataField:"securityStamp"            , caption:"securityStamp"          },    
                    //{ dataField:"concurrencyStamp"         , caption:"concurrencyStamp"       },    
                    { dataField: "phoneNumber", caption: "phoneNumber" },
                    { dataField: "phoneNumberConfirmed", caption: "phoneNumberConfirmed" },
                    //{ dataField:"twoFactorEnabled"         , caption:"twoFactorEnabled"       },    
                    { dataField: "lockoutEnd", caption: "lockoutEnd" },
                ],
                filterRow: { visible: true },
                headerFilter: { visible: false },
                groupPanel: { visible: false },
                scrolling: { mode: "normal" },
                //height: 600,
                showBorders: true,
                paging: { pageSize: 15 },
                pager: {
                    showPageSizeSelector: true,
                    allowedPageSizes: [5, 10, 15, 20, 30],
                },
                showInfo: true,
                selection: {
                    mode: 'single' // 'multiple'
                },
                onSelectionChanged: function (selectedItems) {
                    var data = selectedItems.selectedRowsData[0];
                    if (data) {
                        self.selectedUser = data;
                    }
                }
            };
        }
        UserAdminController.$inject = ['$scope', '$q', '$mdSidenav', '$mdMedia', '$http', '$timeout', 'toaster'];
        return UserAdminController;
    }());
    UserManageApp.UserAdminController = UserAdminController;
})(UserManageApp || (UserManageApp = {}));
angular.module('UserManageApp').controller('UserAdminController', UserManageApp.UserAdminController);
//# sourceMappingURL=admincontrollers.js.map