/// <reference path="ts.d.ts" /> 
module UserManageApp {

    export class UserAdminController {
        static $inject = ['$scope', '$q', '$mdSidenav', '$mdMedia', '$http', '$timeout', 'toaster'];
        userGridOptions;
        selectedUser;
        constructor(
            public $scope,
            public $q,
            public $mdSidenav,
            public $mdMedia,
            public $http: ng.IHttpService,
            public $timeout,
            public toaster

        ) {
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
                            .then((response) =>
                                deferred.resolve(response.data)
                            );
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
                    //{ dataField:"lockoutEnabled"           , caption:"lockoutEnabled"         },    
                    //{ dataField:"accessFailedCount"        , caption:"accessFailedCount"      },                                                                                                                   
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
        url = "/Management/User";


        //getUserList() {
        //    var self = this;
        //    self.$http.post('/Management/User/Users', { command: 'getuserlist', page: 1, pagesize: 5 })
        //        .then(
        //        function successCallback(response) {
        //            console.log(response.data);
        //        },
        //        function errorCallback(response) {
        //            console.log(response);

        //        });
        //}


    }

  
}
angular.module('UserManageApp').controller('UserAdminController', UserManageApp.UserAdminController);
