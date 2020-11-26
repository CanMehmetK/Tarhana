// npm install --save @types/angular
// npm install --save @types/angular-material
/// <reference path="all.d.ts" /> 
angular.module('UserManageApp', ['ngMaterial', 'toaster', 'dx'], function () { })
    .service('httpRequestInterceptor', ['$q', '$rootScope', '$injector', 'toaster', function ($q, $rootScope, $injector, $mdToast, toaster) {
        var interceptorInstance = {
            // Add token to request side...
            response: function (response) {
                var toaster = $injector.get("toaster");
                if (response.status == 200 && response.data && response.data.type == "gresult") { // 
                    if (response.hasError == true)
                        toaster.warning({ title: "", body: response.data.message });
                    else
                        toaster.success({ title: "", body: response.data.message });
                }
                return response;
            }
        };
        return interceptorInstance;
    }
])
    .config(function ($httpProvider) {
    $httpProvider.interceptors.push('httpRequestInterceptor');
})
    .run(function ($window, $rootScope, $mdSidenav, $http, toaster) {
    $rootScope.mainleftopen = false;
    $rootScope.toggleLeft = function () {
        $mdSidenav('mainleft').toggle();
        $rootScope.mainleftopen = $mdSidenav('mainleft').isOpen();
    };
});
//# sourceMappingURL=boot.js.map