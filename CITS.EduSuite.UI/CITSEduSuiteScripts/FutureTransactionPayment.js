
var FutureTransactionPayment = (function () {
    var formPaymentRebind = function (response) {
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

    var getFutureTransactionPayment = function () {
        var obj = {};
        obj.masterKey = $("#MasterKey").val();
        obj.cashFlowTypeKey = $("#CashFlowTypeKey").val();
        $.ajax({
            url: $("#hdnFutureTransactionCalculation").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            success: function (result) {
                $("#TotalReceivedAmount").val(result.TotalReceivedAmount)
                $("#AmountToPay").val(result.AmountToPay)
                $("#EarningAmount").val(result.EarningAmount)
                $("#DeductAmount").val(result.DeductAmount)
                FutureTransactionPayment.CalculateBalanceAmount()
            }
        });
    }

    calculateTotal = function () {
        var Amt = $("#AmountToPay").val();
        Amt = parseFloat(Amt) ? parseFloat(Amt) : 0
        var oldEarning = $("#OldEarningAmount").val()
        oldEarning = parseFloat(oldEarning) ? parseFloat(oldEarning) : 0
        var oldDeduct = $("#OldDeductAmount").val()
        oldDeduct = parseFloat(oldDeduct) ? parseFloat(oldDeduct) : 0
        var earnings = 0;
        var deductions = 0;
        var subTotal = 0
        var total = 0
        $("[data-repeater-item]").each(function () {
            var amount = $($("[id*=Amount]", "[amount]"), $(this)).val()
            amount = parseFloat(amount) ? parseFloat(amount) : 0
            if ($("[id*=IsAddition]", $(this))[0].checked) {
                earnings = earnings + amount;
            }
            else {
                deductions = deductions + amount;
            }
        })
        earnings = earnings + oldEarning
        deductions = deductions + oldDeduct
        $("#DeductAmount").val(deductions)
        $("#EarningAmount").val(earnings)

    }

    var calculateBalanceAmount = function () {
        var AmountToPay = $("#AmountToPay").val();
        var AmountPaid = $("#PaidAmount").val();
        var AmountReceived = $("#TotalReceivedAmount").val();
        var deductions = $("#DeductAmount").val()
        var earnings = $("#EarningAmount").val()
        AmountToPay = AmountToPay != "" ? parseFloat(AmountToPay) : 0
        AmountPaid = AmountPaid != "" ? parseFloat(AmountPaid) : 0
        AmountReceived = AmountReceived != "" ? parseFloat(AmountReceived) : 0
        deductions = parseFloat(deductions) ? parseFloat(deductions) : 0
        earnings = parseFloat(earnings) ? parseFloat(earnings) : 0

        var Balace = parseFloat(AmountToPay + (earnings - deductions) - AmountReceived - AmountPaid);

        if (Balace < 0) {
            $("#lblBalanceAmount,#BalanceAmount").removeClass("text-green").addClass("text-red")
            $("#lblBalanceAmount").html(Resources.Advance)
        }
        else {
            $("#lblBalanceAmount,#BalanceAmount").removeClass("text-red").addClass("text-green")
            $("#lblBalanceAmount").html(Resources.BalanceAmount)
        }
        $("#BalanceAmount").val(Balace.toFixed(2));
        checkbalance();
    }
    var validBalanceAmount = function (_this) {
        var total = $("#AmountToPay").val();
        var paid = $("#TotalReceivedAmount").val();
        var totalPaid = (total - paid);
        var recive = $(_this).val();
        //if (recive > totalPaid) {
        //    $(_this).val(totalPaid <= 0 ? 0 : totalPaid);
        //}
        checkbalance();
    }

    return {

        FormPaymentRebind: formPaymentRebind,
        CalculateBalanceAmount: calculateBalanceAmount,
        ValidBalanceAmount: validBalanceAmount,
        GetFutureTransactionPayment: getFutureTransactionPayment,
        CalculateTotal:calculateTotal
    }

}());

function checkbalance() {

    //var amount = $("#PaidAmount").val();
    //amount = amount != "" ? parseFloat(amount) : 0;
    //var paidAmount = parseFloat(amount)
    //var AccountHeadBalance = parseFloat($("#BankAccountBalance").val())

    //var CashFlowTypeKey = $("#CashFlowTypeKey").val();
    //CashFlowTypeKey = parseInt(CashFlowTypeKey) ? parseInt(CashFlowTypeKey) : 0;
    //if (CashFlowTypeKey == Resources.CashFlowTypeOut) {
    //    if (amount > AccountHeadBalance) {
    //        $("#PaidAmount").val("")
    //        $.alert({
    //            type: 'yellow',
    //            title: Resources.Warning,
    //            content: "Due to InSufficent Balance in Our account Please Check",
    //            icon: 'fa fa-check-circle-o-',
    //            buttons: {
    //                Ok: {
    //                    text: Resources.Ok,
    //                    btnClass: 'btn-success',
    //                    action: function () {
    //                        // window.location.href = $("#hdnEmployeeList").val();
    //                    }
    //                }
    //            }
    //        })
    //    }
    //}




    
    var amount = $("#PaidAmount").val();
    amount = amount != "" ? parseFloat(amount) : 0;
  
    var AccountHeadBalance = parseFloat($("#BankAccountBalance").val())

    var RowKey = $("#MasterKey").val();
    RowKey = parseInt(RowKey) ? parseInt(RowKey) : 0;

    var oldAmount = $("#OldAmount").val();
    oldAmount = parseFloat(oldAmount) ? parseFloat(oldAmount) : 0

    var CashFlowTypeKey = $("#CashFlowTypeKey").val();
    CashFlowTypeKey = parseInt(CashFlowTypeKey) ? parseInt(CashFlowTypeKey) : 0;
    var Message = "";
    if (CashFlowTypeKey == Resources.CashFlowTypeIn) {
        AccountHeadBalance = AccountHeadBalance + amount;
        Message = "its cant editted Due to InSufficent Balance in Our account Please Check"
    } else {
        Message = "Due to InSufficent Balance in Our account Please Check";
        AccountHeadBalance = AccountHeadBalance - amount;
    }
    if (AccountHeadBalance < 0) {
        $("#PaidAmount").val(oldAmount ? oldAmount : "")
        $.alert({
            type: 'yellow',
            title: Resources.Warning,
            content: Message,
            icon: 'fa fa-check-circle-o-',
            buttons: {
                Ok: {
                    text: Resources.Ok,
                    btnClass: 'btn-success',
                    action: function () {
                        // window.location.href = $("#hdnEmployeeList").val();
                    }
                }
            }
        })

    }
}