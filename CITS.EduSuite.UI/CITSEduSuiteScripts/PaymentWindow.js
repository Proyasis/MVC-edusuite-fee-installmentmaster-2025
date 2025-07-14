
var PaymentWindow = (function () {
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
                        Ok: {
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

    //var checkPaymentMode = function (PayMode) {
    //    $("#dvPaymentModes").show();
    //    switch (parseInt(PayMode || 0)) {
    //        case 2:
    //            $(".divCardDetails").show();
    //            $(".divBankAccount,.divChequeDetails").hide();

    //            break;
    //        case 3:
    //            $(".divBankAccount").show();
    //            $(".divCardDetails,.divChequeDetails").hide();
    //            break;
    //        case 4:
    //            $(".divChequeDetails,.divBankAccount").show();
    //            $(".divCardDetails").hide();
    //            break;
    //        default:
    //            $("#dvPaymentModes,.divChequeDetails,.divCardDetails,.divBankAccount").hide();
    //            break;
    //    }



    //}


    var checkPaymentMode = function (PayMode) {
        $("#dvPaymentModes").show();
        $("#dvPaymentModeSub").hide();
        switch (parseInt(PayMode || 0)) {
            case 2:
                $(".divChequeDetails,.divBankAccount").show();
                $(".divCardDetails,.divReferenceNo").hide();

                break;
            case 3:
                $("#dvPaymentModeSub").show();
                $(".divBankAccount").show();
                $(".divCardDetails,.divChequeDetails,.divReferenceNo").hide();
                break;

            default:
                $("#dvPaymentModes,.divChequeDetails,.divCardDetails,.divBankAccount,.divReferenceNo").hide();
                break;
        }
    }

    var checkPaymentModeSub = function (PayMode) {
        $("#dvPaymentModes").show();

        switch (parseInt(PayMode || 0)) {
            case 1:
                $(".divChequeDetails,.divBankAccount").show();
                $(".divCardDetails,.divReferenceNo").hide();

                break;
            case 2:
                $(".divCardDetails,.divBankAccount").show();
                $(".divChequeDetails,.divReferenceNo").hide();
                break;

            default:

                $("#dvPaymentModes,.divBankAccount,.divReferenceNo").show();

                $(".divChequeDetails,.divCardDetails").hide();
                break;
        }

    }



    var calculateBalanceAmount = function () {
        var AmountToPay = $("#AmountToPay").val();
        var AmountPaid = $("#PaidAmount").val();
        var RecievedAmount = $("#TotalReceivedAmount").val();
        AmountToPay = parseFloat(AmountToPay) ? parseFloat(AmountToPay) : 0
        AmountPaid = parseFloat(AmountPaid) ? parseFloat(AmountPaid) : 0
        RecievedAmount = parseFloat(RecievedAmount) ? parseFloat(RecievedAmount) : 0
        var Balace = AmountToPay - RecievedAmount - AmountPaid;
        //if (Balace < 0) {
        //    $("#lblBalanceAmount,#BalanceAmount").removeClass("text-green").addClass("text-red")
        //    $("#lblBalanceAmount").html(Resources.Due)
        //}
        //else {
        //    $("#lblBalanceAmount,#BalanceAmount").removeClass("text-red").addClass("text-green")
        //    $("#lblBalanceAmount").html(Resources.Advance)
        //}
        $("#lblBalanceAmount,#BalanceAmount").removeClass("text-green").addClass("text-red")
        $("#lblBalanceAmount").html(Resources.Due)

        $("#BalanceAmount").val(Balace.toFixed(2));
    }

    var getBalance = function (paymentMode, PurchaseOrderPaymentRowKey, BankAccountKey, branchKey) {
        
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&RowKey=" + PurchaseOrderPaymentRowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey)

        $("#BankAccountBalance").val(response);
    }

    return {

        CheckPaymentMode: checkPaymentMode,
        FormPaymentSubmit: formPaymentSubmit,
        CalculateBalanceAmount: calculateBalanceAmount,
        GetBalance: getBalance,
        CheckPaymentModeSub: checkPaymentModeSub
    }

}());