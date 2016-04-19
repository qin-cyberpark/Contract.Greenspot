(function () {
    'use strict';

    angular
        .module('greenspot', [])
        .controller('ContractController', ContractController);

    ContractController.$inject = ['$http', '$window'];

    function ContractController($http, $window) {
        /* jshint validthis:true */
        var vm = this;

        //
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
            Discount: 0, Signature: '', Agree: false,

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
            vm.contract.Address = $("#address").val();
            vm.contract.PasspotCountry = $("#passportCountry").val();
            if (!vm.validate()){
                return;
            }

            //submit
            $http.post('/home/preview', vm.contract).success(function (result) {
                if (result === 'ok') {
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

            //general
            $('[required]').each(function (idx, obj) {
                var $obj = $(obj);
                if ($obj.val() === '') {
                    $obj.addClass('form-control-error');
                    hasErr = true;
                } else {
                    $obj.removeClass('form-control-error');
                }
            });

            //
            if (vm.contract.IdType === 'Passport') {
                //passport
                var $obj = $("#passportNo")
                if ($obj.val() === '') {
                    $obj.addClass('form-control-error');
                    hasErr = true;
                } else {
                    $obj.removeClass('form-control-error');
                }

                $obj = $("#passportCountry")
                if ($obj.val() === '') {
                    $obj.addClass('form-control-error');
                    hasErr = true;
                } else {
                    $obj.removeClass('form-control-error');
                }
            } else {
                var $obj = $("#driverLicenceNo5a")
                if ($obj.val() === '') {
                    $obj.addClass('form-control-error');
                    hasErr = true;
                } else {
                    $obj.removeClass('form-control-error');
                }

                $obj = $("#driverLicenceNo5b")
                if ($obj.val() === '') {
                    $obj.addClass('form-control-error');
                    hasErr = true;
                } else {
                    $obj.removeClass('form-control-error');
                }
            }
            if (hasErr) {
                alert('Please check inputted value.');
                return false;
            }

            //agreement
            if (!vm.contract.Agree) {
                alert('Please agree Terms & Conditions.');
                return false;
            }

            //sign
            if (!vm.contract.Signature || vm.contract.Signature === '') {
                alert('Please sign.');
                return false;
            }

            return true;
        }

    }
})();
