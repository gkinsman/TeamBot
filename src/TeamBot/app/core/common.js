(function () {
    'use strict';

    angular
        .module('app.core')
        .factory('common', common);

    common.$inject = ['$q', '$rootScope', '$timeout', 'logger'];

    function common($q, $rootScope, $timeout, logger) {
        
        var service = {
            // common angular dependencies
            $broadcast: $broadcast,
            $q: $q,
            $timeout: $timeout,
            // generic
            logger: logger, // for accessibility
        };

        return service;

        function $broadcast() {
            return $rootScope.$broadcast.apply($rootScope, arguments);
        }
    }
})();