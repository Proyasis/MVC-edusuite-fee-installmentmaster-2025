
var AssetPurchasePayment = (function () {
    var formPaymentSubmit = function (btn) {
        var form = $(btn).closest("form")[0];
        var url = $(form).attr("action");
        if ($(form).valid()) {
            $(form).mLoading();
            var data = $(form).serializeArray();
            delete data[0];
            var response = AjaxHelper.ajax("POST", url,
                              {
                                  model: AppCommon.ObjectifyForm(data)
                              });
            if (response.IsSuccessful == true) {
                $.alert({
                    type: 'green',
                    title: Resources.Success,
                    content: response.Message,
                    icon: 'fa fa-check-circle-o-',
                    buttons: {
                        Ok:
                            {
                                text: Resources.Ok,
                                btnClass: 'btn-success',
                                action: function () {
                                    $(form).mLoading("destroy");
                                    location.reload();
                                }
                            }
                    }
                })
            }
            else {
                $("[data-valmsg-for=error_msg_payment]").html(response.Message);
            }

        }
    }

    var checkPaymentMode = function (PayMode) {
        $("#dvPaymentModes").show();
        switch (parseInt(PayMode || 0)) {
            case 2:
                $(".divCardDetails").show();
                $(".divBankAccount,.divChequeDetails").hide();

                break;
            case 3:
                $(".divBankAccount").show();
                $(".divCardDetails,.divChequeDetails").hide();
                break;
            case 4:
                $(".divChequeDetails").show();
                $(".divCardDetails").hide();
                $(".divBankAccount").show();
                break;
            default:
                $("#dvPaymentModes,.divChequeDetails,.divCardDetails,.divBankAccount").hide();
                break;
        }



    }

    var getBalance = function (paymentMode, AssetPurchasePaymentRowKey, BankAccountKey) {
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&AssetPurchasePaymentRowKey=" + AssetPurchasePaymentRowKey + "&BankAccountKey=" + BankAccountKey)
        $("#BankAccountBalance").val(response);
    }

    var calculateBalanceAmount = function () {
        var AmountToPay = $("#AmountToPay").val();
        var AmountPaid = $("#PaidAmount").val();
        var AmountReceived = $("#TotalReceivedAmount").val();
        AmountToPay = AmountToPay != "" ? parseFloat(AmountToPay) : 0
        AmountPaid = AmountPaid != "" ? parseFloat(AmountPaid) : 0
        AmountReceived = AmountReceived != "" ? parseFloat(AmountReceived) : 0

        var Balace = parseFloat(AmountToPay - AmountReceived - AmountPaid);

        if (Balace < 0) {
            $("#lblBalanceAmount,#BalanceAmount").removeClass("text-green").addClass("text-red")
            $("#lblBalanceAmount").html(Resources.Advance)
        }
        else {
            $("#lblBalanceAmount,#BalanceAmount").removeClass("text-red").addClass("text-green")
            $("#lblBalanceAmount").html(Resources.BalanceAmount)
        }
        $("#BalanceAmount").val(Balace.toFixed(2));
    }
    var validBalanceAmount = function (_this) {
        var total = $("#AmountToPay").val();
        var paid = $("#TotalReceivedAmount").val();
        var totalPaid = (total - paid);
        var recive = $(_this).val();
        //if (recive > totalPaid) {
        //    $(_this).val(totalPaid <= 0 ? 0 : totalPaid);
        //}
    }
    return {

        CheckPaymentMode: checkPaymentMode,
        FormPaymentSubmit: formPaymentSubmit,
        CalculateBalanceAmount: calculateBalanceAmount,
        ValidBalanceAmount: validBalanceAmount,
        GetBalance: getBalance
    }

}());

