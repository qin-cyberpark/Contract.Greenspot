(function () {
    'use strict';

    angular
        .module('greenspot', [])
        .controller('ContractController', ContractController);

    ContractController.$inject = ['$http', '$window'];

    function ContractController($http,$window) {
        /* jshint validthis:true */
        var vm = this;

        vm.contract = {
            BusinessName: '',
            Address: '', PostCode: '',
            Title: 'Mr', FistName: '', LastName: '',
            Phone: '', Mobile: '', Email: '',
            IdType: 'Driver Licence', DoB: '', IdFirstName: '', IdLastName: '',
            DriverLicenceNo5a: '', DriverLicenceNo5b: '',
            PassportNo: '', PasspotCountry: '',
            //product & service panel
            Router: { Model: 'Wifi Router MX30', Price: 99, Quantity: '1' },
            AP: { Model: 'AP MXP-2001', Price: 149, Quantity: '0' },
            InstallDate: 'As Soon As Possible', InstallTime: '10AM-12PM',
            Discount: 0, Signature: '', Agree:false,

            OriginalPrice: function () {
                var ttl = 0;
                switch (vm.contract.Router.Model) {
                    case 'Wifi Router MX30': vm.contract.Router.Price = 99; break;
                    case 'Wifi Router MX60': vm.contract.Router.Price = 129; break;
                    case 'Wifi Router MX150': vm.contract.Router.Price = 299; break;
                }
                ttl += vm.contract.Router.Price * vm.contract.Router.Quantity;
                ttl += vm.contract.AP.Price * vm.contract.AP.Quantity;
                return ttl;
            },

            ActualPrice: function () {
                return this.OriginalPrice() - this.Discount;
            }
        }

        vm.submit = function () {
            //load image data
            vm.contract.Signature = $("#signData").val();
            if (!vm.validate() || !vm.contract.Agree) {
                alert('Please check inputed value, signature and agree terms and conditions.');
                return;
            };

            //submit
            $http.post('/home/preview', vm.contract).success(function (result) {
                if (result === 'ok') {
                    console.log(result);
                    $window.open("/home/preview");
                }
            }).error(function () {
                alert("Oops....Something went wrong.");
            }).finally(function () {
            });;

            return false;
        }

        vm.validate = function () {
            var hasErr = false;
            $('[required]').each(function (idx, obj) {
                var $obj = $(obj);
                if ($obj.val() === '') {
                    $obj.addClass('form-control-error');
                    hasErr = true;
                } else {
                    $obj.removeClass('form-control-error');
                }
            });

            if (!vm.contract.Signature || vm.contract.Signature === '') {
                hasErr = true;
            }

            return !hasErr;
        }
    }
})();
