var modelData = [];
var ApplicationFeePayment = (function () {


    var getFeePaymentDetails = function (json) {

        modelData = $.extend(true, {}, json);
        $('.repeater').repeater({
            show: function () {

                $(this).slideDown();
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                var item = $(this);
                $("[id*=RowKey][type=hidden]", $(this)).val(0);
                var Rowkey = $("[id*=RowKey][type=hidden]", $(this)).val();
                if (Rowkey == 0) {
                    //$("select").selectpicker();
                    //$("select[id*=FeeTypeKey]").selectpicker('refresh');
                    ApplicationFeePayment.GetFeeTypeByReceiptTypeitem($("#ReceiptNumberConfigurationKey:checked").val(), item);
                }
                $("[id*=FeeTypeKey]", $(this)).on("change", function () {
                    ApplicationFeePayment.GetCashFlowTypeKey(item, $(this).val())
                    ApplicationFeePayment.CheckFeeTypeMode($(this).val())
                    ApplicationFeePayment.CalculateTotalFee($("input[id*=FeeAmount]", $(item)));

                    var Feetypekey = $(this).val();
                    if (Feetypekey == "" && Feetypekey == null) {
                        $(".intra-state", $(item)).hide();
                        $(".inter-state", $(item)).hide();
                        $(".cess", $(item)).hide();
                        $(".itemTotalAmount", $(item)).hide();
                    }

                });
                $("[id*=FeeTypeKey]").attr("disabled", false);
                $("[id*=FeeYear]").attr("disabled", false);




            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteApplicationFeeOneByOne($(hidden).val(), $(this));
                }
                else {
                    $(this).slideUp(remove);
                }
                setTimeout(function () {
                    ApplicationFeePayment.CalculateItemFee($("input[id*=FeeAmount]", $(self)));
                }, 500);
            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    if ($("#RowKey").val() == "0") {
                        window.open($("#hdnViewFeePrint").val() + "/" + response.RowKey, '_blank');

                        //ApplicationFeePayment.FeeReceiptPopup(response.RowKey);

                    }
                    toastr.success(Resources.Success, response.Message);
                    if ($("#tab-application li a.active").parent().next('li').find('a')[0])
                        $("#tab-application  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                    else
                        window.location.reload();
                }

            },
            data: json,
            repeatlist: 'ApplicationFeePaymentDetails',
            submitButton: '',
            //defaultValues: json,
        });
    }

    var editApplicationFeePayment = function (id, ApplicationKey) {

        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.ApplicationKey = ApplicationKey;
        var url = $("#hdnAddEditApplicationFeePayment").val() + "/" + id + "?" + $.param(obj);
        $("#dvFeeDetails").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvFeeDetails").html(response)
                    $.validator.unobtrusive.parse("form");
                    ApplicationFeePayment.LessEditAmount(this);


                    $("[data-repeater-item]").each(function () {

                        ApplicationFeePayment.ShowHideFeeColumns(this);
                        ApplicationFeePayment.ShowHideGSTColumns(this);

                        if (id != 0) {
                            $(".btnfeeEdit").hide();
                        }
                        else {
                            $(".btnfeeEdit").show();
                        }

                        var eachrowKey = $("[id*=RowKey]", this).val();
                        eachrowKey = parseInt(eachrowKey) ? parseInt(eachrowKey) : 0;
                        if (eachrowKey != 0) {

                            $("[id*=FeeTypeKey]").attr("disabled", true);
                            $("[id*=FeeYear]").attr("disabled", true);

                        }
                    })
                    //ApplicationFeePayment.BindTotalFeeDetails();
                    //ApplicationFeePayment.BindInstallmentFeeDetails();
                } else {
                    $("#dvFeeDetails").mLoading("destroy");
                }
            }
        })

    }

    var deleteApplicationFeePayment = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationFeePayment,
            actionUrl: $("#hdnDeleteApplicationFeePayment").val(),
            actionValue: rowkey,
            dataRefresh: function () {

                if (typeof Application != "undefined")
                    Application.ReloadData();

                else
                    window.location.reload();
            }
        });
    }

    var formPaymentSubmit = function (btn) {
        var form = $(btn).closest("form")[0];
        var url = $(form).attr("action");
        if ($(form).valid()) {
            $(form).closest(".panel-body").mLoading()
            var data = $(form).serializeArray();
            delete data[0];

            setTimeout(function () {
                var response = AjaxHelper.ajax("POST", url,
                    {
                        model: AppCommon.ObjectifyForm(data)
                    });
                if (response.IsSuccessful == true) {
                    toastr.success(Resources.Success, response.Message);
                    $("#tab-profile  li a.active").parent().next('li').find('a').trigger('click');
                }
                else {
                    $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                }
                $(form).closest(".panel-body").mLoading("destroy")
            }, 500);
        }
    }

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

    var getCashFlowTypeKey = function (item, id) {
        var response = AjaxHelper.ajax("GET", $("#hdnGetCashFlowTypeKey").val() + "?id=" + id)
        $("[id*=CashFlowTypeKey]", $(item)).val(response);
    }

    var checkFeeTypeMode = function (_this) {

        var obj = {};
        var item = $(_this).closest('[data-repeater-item]');


        obj.id = $("select[id*=FeeTypeKey]", item).val();
        obj.id = parseInt(obj.id) ? parseInt(obj.id) : 0;

        obj.applicationkey = $("#ApplicationKey").val();

        obj.Year = $("select[id*=FeeYear]", item).val();
        obj.Year = parseInt(obj.Year) ? parseInt(obj.Year) : 0;

        $.ajax({
            url: $("#hdncheckFeeTypeMode").val(),
            type: "GET",
            dataType: "JSON",
            //data: { id: Id,year:obj.Year,applicationkey:applicationKey },
            data: obj,
            success: function (result) {

                $(".inter-state", $(item)).hide();

                if ((result.CGSTRate != 0 && result.CGSTRate != null) && (result.SGSTRate != 0 && result.SGSTRate != null)) {
                    var IsTax = $("#IsTax").val();
                    if (IsTax.toLowerCase() == "true") {
                        $(".intra-state", $(item)).show();
                        $(".divTax").show();
                        $(".itemTotalAmount", $(item)).show();
                        if (result.CessRate == 0) {
                            $(".cess", $(item)).hide();
                        }
                        else {
                            $(".cess", $(item)).show();
                        }
                    }
                    else {
                        $(".intra-state", $(item)).hide();
                        $(".cess", $(item)).hide();
                        $(".divTax").hide();
                        $(".itemTotalAmount", $(item)).hide();
                    }
                    $("[id*=CGSTAmount]", $(item)).val(result.CGSTAmount);
                    $("[id*=CGSTRate]", $(item)).val(result.CGSTRate);
                    $("[id*=SGSTAmount]", $(item)).val(result.SGSTAmount);
                    $("[id*=SGSTRate]", $(item)).val(result.SGSTRate);
                    $("[id*=IGSTAmount]", $(item)).val(result.IGSTAmount);
                    $("[id*=IGSTRate]", $(item)).val(result.IGSTRate);
                    $("[id*=CessAmount]", $(item)).val(result.CessAmount);
                    $("[id*=CessRate]", $(item)).val(result.CessRate);

                    $("[id*=FeeTypeCategoryKey]", $(item)).val(result.FeeTypeCategoryKey);
                    $("[id*=IsInitial]", $(item)).val(result.IsInitial);
                    if (result.IsInitial == true) {
                        $("[id*=TaxableAmount]", $(item)).val(result.TaxableAmount);
                        $("[id*=FeeAmount]", $(item)).val(result.TaxableAmount + (result.CGSTAmount + result.SGSTAmount + result.CessAmount));
                        $("[id*=FeeAmount]", $(item)).attr("readonly", "readonly");
                    }
                    else {
                        result.TaxableAmount = 0;
                        //result.FeeAmount = 0;

                        $("[id*=TaxableAmount]", $(item)).val();
                        $("[id*=FeeAmount]", $(item)).val();
                        $("[id*=FeeAmount]", $(item)).removeAttr("readonly");

                    }
                    $("[id*=PreviousAmount]", $(item)).val(0);

                }
                else {
                    $(".intra-state", $(item)).hide();
                    $(".inter-state", $(item)).hide();
                    $(".cess", $(item)).hide();
                    $(".itemTotalAmount", $(item)).hide();

                    $("[id*=CGSTAmount]", $(item)).val(0);
                    $("[id*=CGSTRate]", $(item)).val(0);
                    $("[id*=SGSTAmount]", $(item)).val(0);
                    $("[id*=SGSTRate]", $(item)).val(0);
                    $("[id*=IGSTAmount]", $(item)).val(0);
                    $("[id*=IGSTRate]", $(item)).val(0);
                    $("[id*=CessAmount]", $(item)).val(0);
                    $("[id*=CessRate]", $(item)).val(0);
                    $("[id*=FeeAmount]", $(item)).val(result.FeeAmount);
                    $("[id*=FeeTypeCategoryKey]", $(item)).val(result.FeeTypeCategoryKey);
                    $("[id*=IsInitial]", $(item)).val(result.IsInitial);
                    $("[id*=PreviousAmount]", $(item)).val(0);
                    if (result.IsInitial == true) {
                        $("[id*=TaxableAmount]", $(item)).val(result.TaxableAmount);
                        $("[id*=FeeAmount]", $(item)).val(result.FeeAmount);
                        $("[id*=FeeAmount]", $(item)).attr("readonly", "readonly");
                    }
                    else {
                        result.TaxableAmount = 0;
                        //result.FeeAmount = 0;

                        $("[id*=TaxableAmount]", $(item)).val();
                        $("[id*=FeeAmount]", $(item)).val();
                        $("[id*=FeeAmount]", $(item)).removeAttr("readonly");

                    }
                }
                $("[id*=TaxableAmount]", $(item)).val(result.TaxableAmount ? result.TaxableAmount : "").trigger("change");
                $("[id*=ActualFeeAmount]", $(item).val(result.FeeAmount));

                // $("[id*=TaxableAmount]", $(item)).val(result.FeeAmount);

                $("[id*=IsFeeTypeYear]", $(item)).val(result.IsFeeTypeYear);
                $("[id*=IsDeduct]", $(item)).val(result.IsDeduct);

                ApplicationFeePayment.ShowHideFeeColumns(item);
                // $("[id*=FeeAmount]", $(item)).val(result.FeeAmount ? result.FeeAmount : "").trigger("change");
                // $("[id*=ActualFeeAmount]", $(item).val(result.FeeAmount));
            }
        });
    }

    var calculateItemFee = function (_this) {
        var item = $(_this).closest("[data-repeater-item]");
        var IsTax = $('#IsTax').val()
        var TaxRateTypeKey = $("#TaxRateTypeKey:checked").val();
        var FeeAmount = $("input[id*=FeeAmount]", $(item)).val();
        var TaxableAmount = $("input[id*=TaxableAmount]", $(item)).val();
        var TotalAmount = $("input[id*=TotalAmount]", $(item)).val();

        TaxRateTypeKey = parseInt(TaxRateTypeKey) ? parseInt(TaxRateTypeKey) : 0;
        FeeAmount = AppCommon.RoundTo(FeeAmount, Resources.RoundToDecimalPostion);
        FeeAmount = parseFloat(FeeAmount) ? parseFloat(FeeAmount) : 0;
        TaxableAmount = AppCommon.RoundTo(TaxableAmount, Resources.RoundToDecimalPostion);
        TaxableAmount = parseFloat(TaxableAmount) ? parseFloat(TaxableAmount) : 0;
        TotalAmount = AppCommon.RoundTo(TotalAmount, Resources.RoundToDecimalPostion);
        TotalAmount = parseFloat(TotalAmount) ? parseFloat(TotalAmount) : 0;


        var CGSTRate = $("input[id*=CGSTRate]", $(item)).val();
        var SGSTRate = $("input[id*=SGSTRate]", $(item)).val();
        var IGSTRate = $("input[id*=IGSTRate]", $(item)).val();
        var CessRate = $("input[id*=CessRate]", $(item)).val();
        CGSTRate = parseFloat(CGSTRate) ? parseFloat(CGSTRate) : 0;
        SGSTRate = parseFloat(SGSTRate) ? parseFloat(SGSTRate) : 0;
        IGSTRate = parseFloat(IGSTRate) ? parseFloat(IGSTRate) : 0;
        CessRate = parseFloat(CessRate) ? parseFloat(CessRate) : 0;
        IsTax = IsTax ? JSON.parse(IsTax.toLowerCase()) : false;
        if (IsTax && TaxRateTypeKey != 3) {

            CGSTRate = CGSTRate;
            SGSTRate = SGSTRate;
            IGSTRate = IGSTRate;
            CessRate = CessRate;
            //if (CGSTRate != 0) {
            //    $(".intra-state", $(item)).show();
            //    $(".cess", $(item)).show();

            //    $(".itemTotalAmount", $(item)).show();
            //}
            //else {
            //    $(".intra-state", $(item)).hide();
            //    $(".inter-state", $(item)).hide();
            //    $(".cess", $(item)).hide();

            //    $(".itemTotalAmount", $(item)).hide();
            //}
            //$(".divTax").show();
            ApplicationFeePayment.ShowHideGSTColumns(item);

        }
        else {
            CGSTRate = 0;
            SGSTRate = 0;
            IGSTRate = 0;
            CessRate = 0;

            //$(".intra-state", $(item)).hide();
            //$(".inter-state", $(item)).hide();
            //$(".cess", $(item)).hide();
            //$(".divTax").hide();
            //$(".itemTotalAmount", $(item)).hide();
            ApplicationFeePayment.ShowHideGSTColumns(item);
        }
        var GStRate = GstAmount = CessAmount = GstCessRate = 0;
        var CGSTAmount = SGSTAmount = TotalTaxableAmount = 0;
        if (TaxRateTypeKey == 1) {
            GStRate = CGSTRate + SGSTRate;
            GstAmount = FeeAmount * GStRate / 100;
            GstAmount = AppCommon.RoundTo(GstAmount, Resources.RoundToDecimalPostion);
            GstAmount = parseFloat(GstAmount) ? parseFloat(GstAmount) : 0;
            CessAmount = FeeAmount * CessRate / 100;
            CessAmount = AppCommon.RoundTo(CessAmount, Resources.RoundToDecimalPostion);
            CessAmount = parseFloat(CessAmount) ? parseFloat(CessAmount) : 0;
            //CessAmount = CessAmount.toFixed(2);
            //CGSTAmount = (GstAmount / 2).toFixed(2);
            //SGSTAmount = (GstAmount / 2).toFixed(2);
            CGSTAmount = (GstAmount / 2);
            SGSTAmount = (GstAmount / 2);
            TaxableAmount = FeeAmount;
            TaxableAmount = AppCommon.RoundTo(TaxableAmount, Resources.RoundToDecimalPostion);
            TotalAmount = FeeAmount + (parseFloat(CGSTAmount) + parseFloat(SGSTAmount) + parseFloat(CessAmount));
            TotalAmount = AppCommon.RoundTo(TotalAmount, Resources.RoundToDecimalPostion);
        }
        else if (TaxRateTypeKey == 2) {
            TotalAmount = FeeAmount;
            GStRate = CGSTRate + SGSTRate;

            GstCessRate = CGSTRate + SGSTRate + CessRate;

            TaxableAmount = (FeeAmount * 100 / (100 + GstCessRate));
            TaxableAmount = AppCommon.RoundTo(TaxableAmount, Resources.RoundToDecimalPostion);
            TaxableAmount = parseFloat(TaxableAmount) ? parseFloat(TaxableAmount) : 0;
            GstAmount = TaxableAmount * GStRate / 100;
            GstAmount = AppCommon.RoundTo(GstAmount, Resources.RoundToDecimalPostion);
            CessAmount = TaxableAmount * CessRate / 100;
            CessAmount = AppCommon.RoundTo(CessAmount, Resources.RoundToDecimalPostion);
            //CessAmount = CessAmount.toFixed(2);
            CGSTAmount = (GstAmount / 2);
            SGSTAmount = (GstAmount / 2);
        }
        else {
            CGSTAmount = 0;
            SGSTAmount = 0;
            CessAmount = 0;
            TotalAmount = FeeAmount;
            TaxableAmount = FeeAmount;
        }

        //var CGSTAmount = (GstAmount / 2).toFixed(2);
        //var SGSTAmount = (GstAmount / 2).toFixed(2);
        //var FeeAmount = Amount + (parseFloat(CGSTAmount) + parseFloat(SGSTAmount) + parseFloat(CessAmount));

        //$("input[id*=CGSTRate]", $(item)).val(CGSTRate);
        //$("input[id*=SGSTRate]", $(item)).val(SGSTRate);
        //$("input[id*=IGSTRate]", $(item)).val(IGSTRate);
        //$("input[id*=CessRate]", $(item)).val(CessRate);

        $("input[id*=CGSTAmount]", $(item)).val(CGSTAmount);
        $("input[id*=SGSTAmount]", $(item)).val(SGSTAmount);
        $("input[id*=CessAmount]", $(item)).val(CessAmount);
        $("input[id*=TaxableAmount]", $(item)).val(TaxableAmount);


        $("input[id*=TotalAmount]", $(item)).val(TotalAmount);

        ApplicationFeePayment.calculateItemTotalAmount(item);

    }

    var calculateItemTotalAmount = function (currItem) {
        var objNetAmount = {};

        $("#divCourseFeeDetails [data-repeater-item]").each(function () {
            var totalItem = this;
            var currFeeYear = $("[name*=FeeYear]", currItem).val();
            currFeeYear = parseInt(currFeeYear) ? parseInt(currFeeYear) : 0;
            var currFeeTypeKey = $("[name*=FeeTypeKey]", currItem).val();
            currFeeTypeKey = parseInt(currFeeTypeKey) ? parseInt(currFeeTypeKey) : 0;
            var currFeeAmount = $("[name*=FeeAmount]", currItem);
            var currAmount = $(currFeeAmount).val();
            currAmount = parseInt(currAmount) ? parseInt(currAmount) : 0;
            var totalFeeYear = $("[name*=FeeYear]", totalItem).val();
            totalFeeYear = parseInt(totalFeeYear) ? parseInt(totalFeeYear) : 0;
            var totalFeeTypeKey = $("[name*=FeeTypeKey]", totalItem).val();
            totalFeeTypeKey = parseInt(totalFeeTypeKey) ? parseInt(totalFeeTypeKey) : 0;
            var EditedList = $("#dynamicRepeater [data-repeater-item]").toArray().filter(function (item, n) {
                var FeeYear = $("[id*=FeeYear]", item).val();
                FeeYear = parseInt(FeeYear) ? parseInt(FeeYear) : 0;
                var FeeTypeKey = $("[id*=FeeTypeKey]", item).val();
                FeeTypeKey = parseInt(FeeTypeKey) ? parseInt(FeeTypeKey) : 0;
                return FeeYear === totalFeeYear && FeeTypeKey === totalFeeTypeKey;
            }).map(function (item) {
                var obj = {};
                obj.TotalAmount = 0;
                obj.FeeAmount = $("input[name*=TaxableAmount]", item).val();
                obj.FeeAmount = parseFloat(obj.FeeAmount) ? parseFloat(obj.FeeAmount) : 0;
                var CGSTAmount = $("input[name*=CGSTAmount]", item).val();
                CGSTAmount = parseFloat(CGSTAmount) ? parseFloat(CGSTAmount) : 0;
                var SGSTAmount = $("input[name*=SGSTAmount]", item).val();
                SGSTAmount = parseFloat(SGSTAmount) ? parseFloat(SGSTAmount) : 0;
                var IGSTAmount = $("input[name*=IGSTAmount]", item).val();
                IGSTAmount = parseFloat(IGSTAmount) ? parseFloat(IGSTAmount) : 0;
                obj.GSTAmount = CGSTAmount + SGSTAmount + IGSTAmount;
                obj.CessAmount = $("input[name*=CessAmount]", item).val();
                obj.CessAmount = parseFloat(obj.CessAmount) ? parseFloat(obj.CessAmount) : 0;
                return obj;
            });
            //var totalItem = $("#divCourseFeeDetails [data-repeater-item]").toArray().filter(function (item, n) {
            //    var FeeYear = $("[name*=FeeYear]", item).val();
            //    FeeYear = parseInt(FeeYear) ? parseInt(FeeYear) : 0;
            //    var FeeTypeKey = $("[name*=FeeTypeKey]", item).val();
            //    FeeTypeKey = parseInt(FeeTypeKey) ? parseInt(FeeTypeKey) : 0;
            //    return FeeYear === currFeeYear && FeeTypeKey === currFeeTypeKey;
            //});
            var existingList = $(totalItem).toArray().map(function (item) {
                var obj = {};
                obj.TotalAmount = $("input[name*=TotalAmount]", item).val();
                obj.TotalAmount = parseFloat(obj.TotalAmount) ? parseFloat(obj.TotalAmount) : 0;
                obj.FeeAmount = $("input[name*=FeeAmount]", item).val();
                obj.FeeAmount = parseFloat(obj.FeeAmount) ? parseFloat(obj.FeeAmount) : 0;
                obj.GSTAmount = $("input[name*=GSTAmount]", item).val();
                obj.GSTAmount = parseFloat(obj.GSTAmount) ? parseFloat(obj.GSTAmount) : 0;
                obj.CessAmount = $("input[name*=TotalCessAmount]", item).val();
                obj.CessAmount = parseFloat(obj.CessAmount) ? parseFloat(obj.CessAmount) : 0;
                return obj;
            });
            var length = EditedList.length;
            EditedList = EditedList.concat(existingList);

            var TotalList = {};
            $(Object.keys(EditedList[0])).each(function (n, key) {
                TotalList[key] = AppCommon.RoundTo(EditedList.reduce(function (sum, item) {
                    return sum + item[key];
                }, 0), Resources.RoundToDecimalPostion);

            });
            TotalList.TotalAmount = parseFloat(TotalList.TotalAmount) ? parseFloat(TotalList.TotalAmount) : 0;
            TotalList.FeeAmount = parseFloat(TotalList.FeeAmount) ? parseFloat(TotalList.FeeAmount) : 0;


            var oldBalance = TotalList.TotalAmount - TotalList.FeeAmount + currAmount;
            oldBalance = parseFloat(oldBalance) ? parseFloat(oldBalance) : 0;

            var balanceAmount = TotalList.TotalAmount - TotalList.FeeAmount;
            balanceAmount = parseFloat(balanceAmount) ? parseFloat(balanceAmount) : 0;

            balanceAmount = AppCommon.RoundTo(balanceAmount, Resources.RoundToDecimalPostion)
            balanceAmount = parseFloat(balanceAmount) ? parseFloat(balanceAmount) : 0;

            if (balanceAmount < 0) {
                if (TotalList.TotalAmount == (oldBalance + balanceAmount)) {
                    oldBalance = oldBalance + balanceAmount;
                }
                else {
                    oldBalance = "";
                }
                $(currFeeAmount).val((oldBalance ? oldBalance : ""));
                //ApplicationFeePayment.CalculateItemFee(currItem);
                return false;

            } else {
                var TotalAmount = parseFloat(TotalList.FeeAmount) + parseFloat(TotalList.GSTAmount) + parseFloat(TotalList.CessAmount);
                $("[paid-amount]", totalItem).html(TotalList.FeeAmount);
                $("[balance-amount]", totalItem).html(balanceAmount);
                $("[gst-amount]", totalItem).html(TotalList.GSTAmount);
                $("[cess-amount]", totalItem).html(TotalList.CessAmount);
                $("[totalpaid-amount]", totalItem).html(AppCommon.RoundTo(TotalAmount, Resources.RoundToDecimalPostion));

                objNetAmount.FeeAmount = (objNetAmount.FeeAmount ? parseFloat(objNetAmount.FeeAmount) : 0) + parseFloat(TotalList.FeeAmount);
                objNetAmount.BalanceAmount = (objNetAmount.BalanceAmount ? parseFloat(objNetAmount.BalanceAmount) : 0) + parseFloat(balanceAmount);
                objNetAmount.GSTAmount = (objNetAmount.GSTAmount ? parseFloat(objNetAmount.GSTAmount) : 0) + parseFloat(TotalList.GSTAmount);
                objNetAmount.CessAmount = (objNetAmount.CessAmount ? parseFloat(objNetAmount.CessAmount) : 0) + parseFloat(TotalList.CessAmount);
                objNetAmount.TotalAmount = (objNetAmount.TotalAmount ? parseFloat(objNetAmount.TotalAmount) : 0) + parseFloat(TotalAmount);
            }


            $("[net-feeamount]").html(objNetAmount.FeeAmount);
            $("[net-balanceamount]").html(objNetAmount.BalanceAmount);
            $("[net-gstamount]").html(objNetAmount.GSTAmount);
            $("[net-cessamount]").html(objNetAmount.CessAmount);
            $("[net-totalamount]").html(objNetAmount.TotalAmount);
            $("#TotalPaid").val(objNetAmount.FeeAmount);
        })

    }

    var lessEditAmount = function () {
        var objNetAmount = {};

        $("#divCourseFeeDetails [data-repeater-item]").each(function () {
            var totalItem = this;

            var totalFeeYear = $("[name*=FeeYear]", totalItem).val();
            totalFeeYear = parseInt(totalFeeYear) ? parseInt(totalFeeYear) : 0;
            var totalFeeTypeKey = $("[name*=FeeTypeKey]", totalItem).val();
            totalFeeTypeKey = parseInt(totalFeeTypeKey) ? parseInt(totalFeeTypeKey) : 0;
            var EditedList = $("#dynamicRepeater [data-repeater-item]").toArray().filter(function (item, n) {
                var FeeYear = $("[id*=FeeYear]", item).val();
                FeeYear = parseInt(FeeYear) ? parseInt(FeeYear) : 0;
                var FeeTypeKey = $("[id*=FeeTypeKey]", item).val();
                FeeTypeKey = parseInt(FeeTypeKey) ? parseInt(FeeTypeKey) : 0;
                return FeeYear === totalFeeYear && FeeTypeKey === totalFeeTypeKey;
            }).map(function (item) {
                var obj = {};
                obj.TotalAmount = 0;
                obj.FeeAmount = $("input[name*=TaxableAmount]", item).val();
                obj.FeeAmount = parseFloat(obj.FeeAmount) ? -parseFloat(obj.FeeAmount) : 0;
                var CGSTAmount = $("input[name*=CGSTAmount]", item).val();
                CGSTAmount = parseFloat(CGSTAmount) ? parseFloat(CGSTAmount) : 0;
                var SGSTAmount = $("input[name*=SGSTAmount]", item).val();
                SGSTAmount = parseFloat(SGSTAmount) ? parseFloat(SGSTAmount) : 0;
                var IGSTAmount = $("input[name*=IGSTAmount]", item).val();
                IGSTAmount = parseFloat(IGSTAmount) ? parseFloat(IGSTAmount) : 0;
                obj.GSTAmount = -(CGSTAmount + SGSTAmount + IGSTAmount);
                obj.CessAmount = $("input[name*=CessAmount]", item).val();
                obj.CessAmount = parseFloat(obj.CessAmount) ? -parseFloat(obj.CessAmount) : 0;
                return obj;
            });
            //var totalItem = $("#divCourseFeeDetails [data-repeater-item]").toArray().filter(function (item, n) {
            //    var FeeYear = $("[name*=FeeYear]", item).val();
            //    FeeYear = parseInt(FeeYear) ? parseInt(FeeYear) : 0;
            //    var FeeTypeKey = $("[name*=FeeTypeKey]", item).val();
            //    FeeTypeKey = parseInt(FeeTypeKey) ? parseInt(FeeTypeKey) : 0;
            //    return FeeYear === currFeeYear && FeeTypeKey === currFeeTypeKey;
            //});
            var existingList = $(totalItem).toArray().map(function (item) {
                var obj = {};
                obj.TotalAmount = $("input[name*=TotalAmount]", item).val();
                obj.TotalAmount = parseFloat(obj.TotalAmount) ? parseFloat(obj.TotalAmount) : 0;
                obj.FeeAmount = $("input[name*=FeeAmount]", item).val();
                obj.FeeAmount = parseFloat(obj.FeeAmount) ? parseFloat(obj.FeeAmount) : 0;
                obj.GSTAmount = $("input[name*=GSTAmount]", item).val();
                obj.GSTAmount = parseFloat(obj.GSTAmount) ? parseFloat(obj.GSTAmount) : 0;
                obj.CessAmount = $("input[name*=TotalCessAmount]", item).val();
                obj.CessAmount = parseFloat(obj.CessAmount) ? parseFloat(obj.CessAmount) : 0;
                return obj;
            });
            var length = EditedList.length;
            EditedList = EditedList.concat(existingList);
            if (length) {
                var TotalList = {};
                $(Object.keys(EditedList[0])).each(function (n, key) {
                    TotalList[key] = AppCommon.RoundTo(EditedList.reduce(function (sum, item) {
                        return sum + item[key];
                    }, 0), Resources.RoundToDecimalPostion);
                });
                var balanceAmount = TotalList.TotalAmount - TotalList.FeeAmount;
                $("input[name*=FeeAmount]", totalItem).val(TotalList.FeeAmount);
                $("[name*=BalanceAmount]", totalItem).val(balanceAmount);
                $("input[name*=GSTAmount]", totalItem).val(TotalList.GSTAmount);
                $("input[name*=CessAmount]", totalItem).val(TotalList.CessAmount);
                //$("[totalpaid-amount]", totalItem).html(TotalList.FeeAmount);
            }
        })

    }

    calculateItemAmount = function (obj) {
        obj.TaxRateKey = $("#TaxRateTypeKey:checked").val();
        //obj.IsExclusive = Resources.ITRKey == obj.TaxRateKey ? false : true;

        var TotalAmount = obj.FeeAmount;
        TotalAmount = AppCommon.RoundTo(TotalAmount, Resources.RoundToDecimalPostion);
        TotalAmount = parseFloat(TotalAmount) ? parseFloat(TotalAmount) : 0;
        var IsIgst = $("#IsIgst").val();
        obj.IsIgst = IsIgst ? JSON.parse(IsIgst.toLowerCase()) : false;

        var CessPer = $("#CessPer").val();
        CessPer = parseFloat(CessPer) ? parseFloat(CessPer) : 0;


        var GSTPer = IsIgst ? obj.IGSTPer : (obj.CGSTPer + obj.SGSTPer);
        if (Resources.NTRKey == obj.TaxRateKey) {
            GSTPer = 0;
        }
        var GstRate = 0;
        obj.IGSTAmt = 0;
        obj.CGSTAmt = 0;
        obj.SGSTAmt = 0;
        obj.CessAmt = 0;
        if (obj.IsExclusive) {

            GstRate = (obj.Rate * GSTPer / 100);
            if (IsIgst) {
                obj.IGSTAmt = GstRate;
                obj.IGSTAmt = AppCommon.RoundTo(obj.IGSTAmt, Resources.RoundToDecimalPostion);
                obj.IGSTAmt = parseFloat(obj.IGSTAmt) ? parseFloat(obj.IGSTAmt) : 0;
            }
            else {
                obj.CGSTAmt = GstRate / 2;
                obj.CGSTAmt = AppCommon.RoundTo(obj.CGSTAmt, Resources.RoundToDecimalPostion);
                obj.CGSTAmt = parseFloat(obj.CGSTAmt) ? parseFloat(obj.CGSTAmt) : 0;
                obj.SGSTAmt = obj.CGSTAmt;

                if (CessPer) {
                    obj.CessAmt = (obj.Rate * CessPer / 100);
                }
            }
            obj.InclusiveRate = obj.Rate + obj.IGSTAmt + obj.CGSTAmt + obj.SGSTAmt + obj.CessAmt;

            GstRate = obj.IGSTAmt + obj.CGSTAmt + obj.SGSTAmt;
            obj.RowTotalGST = GstRate * Quantity;
            obj.CGSTAmt = obj.CGSTAmt * Quantity;
            obj.SGSTAmt = obj.SGSTAmt * Quantity;
            obj.IGSTAmt = obj.IGSTAmt * Quantity;
            obj.CessAmt = obj.CessAmt * Quantity;

            obj.TaxableAmount = obj.Rate * Quantity;
            obj.TotalAmount = obj.InclusiveRate * Quantity;

        }
        else {
            obj.TotalAmount = obj.InclusiveRate * Quantity;




            if (IsIgst) {
                obj.TaxableAmount = (obj.TotalAmount * 100 / (100 + GSTPer));
                obj.TaxableAmount = AppCommon.RoundTo(obj.TaxableAmount, Resources.RoundToDecimalPostion);
                obj.TaxableAmount = parseFloat(obj.TaxableAmount) ? parseFloat(obj.TaxableAmount) : 0;

                obj.RowTotalGST = obj.TotalAmount - obj.TaxableAmount;
                obj.IGSTAmt = obj.RowTotalGST;
                obj.IGSTAmt = AppCommon.RoundTo(obj.IGSTAmt, Resources.RoundToDecimalPostion);
                obj.IGSTAmt = parseFloat(obj.IGSTAmt) ? parseFloat(obj.IGSTAmt) : 0;
            }
            else {
                var GstCessPer = GSTPer;
                if (CessPer) {
                    GstCessPer = (GSTPer + CessPer);
                }
                obj.TaxableAmount = (obj.TotalAmount * 100 / (100 + GstCessPer));
                obj.TaxableAmount = AppCommon.RoundTo(obj.TaxableAmount, Resources.RoundToDecimalPostion);
                obj.TaxableAmount = parseFloat(obj.TaxableAmount) ? parseFloat(obj.TaxableAmount) : 0;

                obj.RowTotalGST = obj.TaxableAmount * GSTPer / 100;

                obj.CGSTAmt = obj.RowTotalGST / 2;
                obj.CGSTAmt = AppCommon.RoundTo(obj.CGSTAmt, Resources.RoundToDecimalPostion);
                obj.CGSTAmt = parseFloat(obj.CGSTAmt) ? parseFloat(obj.CGSTAmt) : 0;
                obj.SGSTAmt = obj.CGSTAmt;
                if (CessPer) {
                    obj.CessAmt = obj.TaxableAmount * CessPer / 100;
                }


            }


            obj.Rate = obj.TaxableAmount / Quantity;
            obj.Rate = AppCommon.RoundTo(obj.Rate, Resources.RoundToDecimalPostion);
            obj.Rate = parseFloat(obj.Rate) ? parseFloat(obj.Rate) : 0;

            obj.InclusiveRate = AppCommon.RoundTo(obj.InclusiveRate, Resources.RoundToDecimalPostion);
            obj.InclusiveRate = parseFloat(obj.InclusiveRate) ? parseFloat(obj.InclusiveRate) : 0;
        }

        obj.RowTotalGST = AppCommon.RoundTo(obj.RowTotalGST, Resources.RoundToDecimalPostion);
        obj.RowTotalGST = parseFloat(obj.RowTotalGST) ? parseFloat(obj.RowTotalGST) : 0;

        obj.CGSTAmt = AppCommon.RoundTo(obj.CGSTAmt, Resources.RoundToDecimalPostion);
        obj.CGSTAmt = parseFloat(obj.CGSTAmt) ? parseFloat(obj.CGSTAmt) : 0;
        obj.SGSTAmt = AppCommon.RoundTo(obj.SGSTAmt, Resources.RoundToDecimalPostion);
        obj.SGSTAmt = parseFloat(obj.SGSTAmt) ? parseFloat(obj.SGSTAmt) : 0;
        obj.IGSTAmt = AppCommon.RoundTo(obj.IGSTAmt, Resources.RoundToDecimalPostion);
        obj.IGSTAmt = parseFloat(obj.IGSTAmt) ? parseFloat(obj.IGSTAmt) : 0;

        obj.CessAmt = AppCommon.RoundTo(obj.CessAmt, Resources.RoundToDecimalPostion);
        obj.CessAmt = parseFloat(obj.CessAmt) ? parseFloat(obj.CessAmt) : 0;


        obj.TaxableAmount = AppCommon.RoundTo(obj.TaxableAmount, Resources.RoundToDecimalPostion);
        obj.TaxableAmount = parseFloat(obj.TaxableAmount) ? parseFloat(obj.TaxableAmount) : 0;

        obj.TotalAmount = AppCommon.RoundTo(obj.TotalAmount, Resources.RoundToDecimalPostion);
        obj.TotalAmount = parseFloat(obj.TotalAmount) ? parseFloat(obj.TotalAmount) : 0;

    }

    var calculateTotalFee = function () {

        var TotalAmount = $("#spnTotalFee").html() ? parseFloat($("#spnTotalFee").html()) : 0;
        var InputAmount = 0, ActualFeeAmount = 0;
        var TotalPaidAmount = $("#divCourseFeeDetails [paid-amount]").toArray().reduce(function (sum, element) {
            return sum + (parseFloat($(element).html()) ? parseFloat($(element).html()) : 0);
        }, 0);
        var BalanceFee = TotalAmount - TotalPaidAmount;
        TotalPaidAmount = parseFloat(TotalPaidAmount.toFixed(2)).toString();
        $("#TotalPaid").val(TotalPaidAmount);
        $("#spnTotalPaid").html(TotalPaidAmount);
        $("#spnBalanceFee").html(BalanceFee);

    }

    var bindTotalFeeDetails = function () {
        $("#dvTotalFeeDetails").mLoading()

        $("#dvTotalFeeDetails").load($("#hdnTotalFeeDetails").val(), function () {
            ApplicationFeePayment.CalculateTotalFee();
        });
    }

    var bindInstallmentFeeDetails = function () {
        $("#dvTotalFeeDetails").mLoading()
        $("#dvInstallmentFeeDetails").load($("#hdnInstallmentFeeDetails").val());
    }
    var bindFeeScheduleDetails = function () {
        $("#dvFeeSchedule").mLoading()
        $("#dvFeeSchedule").load($("#hdnFeeScheduleDetails").val());
    }

    var bindFeePaymentDetails = function () {
        $("#dvFeePaymentDetails").mLoading()
        $("#dvFeePaymentDetails").load($("#hdnApplicationFeePaymentList").val());
    }

    var totalFeeDatails = function () {

        var TotalPaidFeeAmount = 0;
        $("#divCourseFeeDetails [data-repeater-item]").each(function (i) {

            var TotalPaidAmount = 0;
            FeeAmount = $("input[name*=FeeAmount]", $(this)).val();
            GSTAmount = $("input[name*=GSTAmount]", $(this)).val();
            TotalCessAmount = $("input[name*=TotalCessAmount]", $(this)).val();


            FeeAmount = parseFloat(FeeAmount) ? parseFloat(FeeAmount) : 0;
            GSTAmount = parseFloat(GSTAmount) ? parseFloat(GSTAmount) : 0;
            TotalCessAmount = parseFloat(TotalCessAmount) ? parseFloat(TotalCessAmount) : 0;
            FeeAmount = parseFloat(FeeAmount) ? parseFloat(FeeAmount) : 0;

            var IsTax = $("#IsTax").val();
            IsTax = IsTax ? JSON.parse(IsTax.toLowerCase()) : false;

            if (!IsTax) {
                TotalPaidAmount = TotalPaidAmount + FeeAmount;
            }
            else {
                TotalPaidAmount = TotalPaidAmount + FeeAmount + GSTAmount + TotalCessAmount;
            }
            TotalPaidAmount = parseFloat(TotalPaidAmount.toFixed(2));
            $("input[name*=TotalPaid]", $(this)).val(TotalPaidAmount);

            TotalPaidFeeAmount = TotalPaidFeeAmount + TotalPaidAmount;
            TotalPaidFeeAmount = parseFloat(TotalPaidFeeAmount.toFixed(2));
        });
        $("#TotalPaidFeeAmount").val(TotalPaidFeeAmount);

    }

    var showHideFeeColumns = function (item) {
        var IsFeeTypeYear = $("[id*=IsFeeTypeYear]", item).val();
        IsFeeTypeYear = IsFeeTypeYear ? JSON.parse(IsFeeTypeYear.toLowerCase()) : false;
        if (IsFeeTypeYear) {
            $(".divFeeYear", $(item)).hide(function () {
                $("select", this).val("");
            });

        }
        else {
            $(".divFeeYear", $(item)).show();
        }

    }

    var showHideGSTColumns = function (item) {

        var TaxRateTypeKey = $("#TaxRateTypeKey:checked").val();
        var CGSTRate = $("input[id*=CGSTRate]", $(item)).val();
        var SGSTRate = $("input[id*=SGSTRate]", $(item)).val();
        var IGSTRate = $("input[id*=IGSTRate]", $(item)).val();
        var CessRate = $("input[id*=CessRate]", $(item)).val();
        CGSTRate = parseFloat(CGSTRate) ? parseFloat(CGSTRate) : 0;
        SGSTRate = parseFloat(SGSTRate) ? parseFloat(SGSTRate) : 0;
        IGSTRate = parseFloat(IGSTRate) ? parseFloat(IGSTRate) : 0;
        CessRate = parseFloat(CessRate) ? parseFloat(CessRate) : 0;
        TaxRateTypeKey = parseInt(TaxRateTypeKey) ? parseInt(TaxRateTypeKey) : 0;
        var IsTax = $('#IsTax').val()
        $(".inter-state").hide();
        IsTax = IsTax ? JSON.parse(IsTax.toLowerCase()) : false;

        if (IsTax && TaxRateTypeKey != 3) {
            CGSTRate = CGSTRate;
            SGSTRate = SGSTRate;
            IGSTRate = IGSTRate;
            CessRate = CessRate;
            if (CGSTRate != 0) {
                $(".intra-state", $(item)).show();

                $(".itemTotalAmount", $(item)).show();
                if (CessRate == 0) {
                    $(".cess", $(item)).hide();
                }
                else {
                    $(".cess", $(item)).show();
                }
            }
            else {
                $(".intra-state", $(item)).hide();
                $(".inter-state", $(item)).hide();
                $(".cess", $(item)).hide();

                $(".itemTotalAmount", $(item)).hide();
            }
            $(".divTax").show();

        }
        else {
            CGSTRate = 0;
            SGSTRate = 0;
            IGSTRate = 0;
            CessRate = 0;

            $(".intra-state", $(item)).hide();
            $(".inter-state", $(item)).hide();
            $(".cess", $(item)).hide();
            $(".divTax").hide();
            $(".itemTotalAmount", $(item)).hide();
        }

    }

    var feeReceiptPopup = function (Id) {
        window.open($("#hdnViewFeePrint").val() + "/" + Id, '_blank');
        //var URL = $("#hdnViewFeePrint").val() + "/" + Id;
        //$.customPopupform.CustomPopup({
        //    modalsize: "modal-lg mw-100 w-75",
        //    headerText: 'Fee Recept',
        //    footerText: '<button type="button"  id="btnPrintReceipt" class="btnForm btnSubmit btn btn-sm btn-default pull-left ">' + Resources.Print + '</button>'
        //                + '<button type="button" id="btnCancel" data-dismiss="modal" class="btnForm btnCancel btn btn-sm btn-default pull-left">' + Resources.Cancel + '</button>',
        //    load: function () {
        //        $("#amountWords").html(AppCommon.AmounToWords($("#totalAmount").html()))

        //        printReceiptDiv($("#dvPrintReciept"))
        //        $("#btnPrintReceipt").on("click", function () {
        //            printReceiptDiv($("#dvPrintReciept"))

        //        })
        //    },
        //    rebind: function (result) {

        //    }
        //}, URL);

        $("#imglogop").show();
        $("#imglogod").hide();
    }

    function formSubmit() {

        var $form = $("#frmApplicationFeePayment")
        var JsonData = [];
        $("[disabled=disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        var ApplicationKey = formData["ApplicationKey"];

        if ($form.valid()) {
            $form.mLoading();
            var dataurl = $form.attr("action");

            AjaxHelper.ajaxAsync("POST", dataurl,
                {
                    model: formData
                }, function () {
                    var response = this;
                    if (typeof response == "string") {

                    }
                    if (response.IsSuccessful) {
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
                                        $form.mLoading("destroy");
                                        ApplicationFeePayment.EditApplicationFeePayment(null, ApplicationKey);
                                        ApplicationFeePayment.BindTotalFeeDetails();
                                        ApplicationFeePayment.BindInstallmentFeeDetails();
                                        ApplicationFeePayment.BindFeePaymentDetails();
                                    }
                                }
                            }
                        })

                    }
                    else {
                        $("[data-valmsg-for=error_msg]").html(response);
                    }
                    $form.mLoading("destroy");
                });
        }
    }

    var getReceiptNo = function (id) {
        var response = AjaxHelper.ajax("GET", $("#hdnGetReceiptNo").val() + "?ReceiptNumberConfigurationKey=" + id)
        $("[id*=RecieptNo]").val(response.RecieptNo);
    }
    var getFeeTypeByReceiptType = function (id) {
        var obj = {};
        obj.ReceiptNumberConfigurationKey = id;
        //AppCommon.BindDropDownbyId(obj, $("#hdnGetFeeTypeByReceiptType").val(), $("[id*=FeeTypeKey]"), Resources.Select + Resources.BlankSpace + Resources.FeeType, "FeeTypes");

        AppCommon.BindDropDownbyId(obj, $("#hdnGetFeeTypeByReceiptType").val(), $("[id*=FeeTypeKey]"), Resources.FeeType);

        //$("select").selectpicker();
        //$("select[id*=FeeTypeKey]").selectpicker('refresh');
    }
    var getFeeTypeByReceiptTypeitem = function (id, item) {
        var obj = {};
        obj.ReceiptNumberConfigurationKey = id;
        //AppCommon.BindDropDownbyId(obj, $("#hdnGetFeeTypeByReceiptType").val(), $("[id*=FeeTypeKey]"), Resources.Select + Resources.BlankSpace + Resources.FeeType, "FeeTypes");

        //AppCommon.BindDropDownbyId(obj, $("#hdnGetFeeTypeByReceiptType").val(), $("[id*=FeeTypeKey]"), Resources.FeeType);
        AppCommon.BindDropDownbyId(obj, $("#hdnGetFeeTypeByReceiptType").val(), $("select[id*=FeeTypeKey]", $(item)), Resources.FeeType);


        //$("select").selectpicker();
        //$("select[id*=FeeTypeKey]").selectpicker('refresh');
    }
    var downloadFeeReceipt = function (Id) {
        window.open($("#hdnDownloadFeePrint").val() + "/" + Id, '_blank');

    }

    return {
        GetFeePaymentDetails: getFeePaymentDetails,
        EditApplicationFeePayment: editApplicationFeePayment,
        DeleteApplicationFeePayment: deleteApplicationFeePayment,
        CheckPaymentMode: checkPaymentMode,
        FormPaymentSubmit: formPaymentSubmit,
        GetCashFlowTypeKey: getCashFlowTypeKey,
        CalculateItemFee: calculateItemFee,
        CalculateTotalFee: calculateTotalFee,
        CheckFeeTypeMode: checkFeeTypeMode,
        BindTotalFeeDetails: bindTotalFeeDetails,
        BindInstallmentFeeDetails: bindInstallmentFeeDetails,
        BindFeePaymentDetails: bindFeePaymentDetails,
        TotalFeeDetails: totalFeeDatails,
        ShowHideFeeColumns: showHideFeeColumns,
        ShowHideGSTColumns: showHideGSTColumns,
        FeeReceiptPopup: feeReceiptPopup,
        calculateItemTotalAmount: calculateItemTotalAmount,
        LessEditAmount: lessEditAmount,
        CheckPaymentModeSub: checkPaymentModeSub,
        FormSubmit: formSubmit,
        GetReceiptNo: getReceiptNo,
        GetFeeTypeByReceiptType: getFeeTypeByReceiptType,
        GetFeeTypeByReceiptTypeitem: getFeeTypeByReceiptTypeitem,
        DownloadFeeReceipt: downloadFeeReceipt,
        BindFeeScheduleDetails: bindFeeScheduleDetails
    }

}());


function deleteApplicationFeeOneByOne(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ApplicationDocument,
        actionUrl: $("#hdnDeleteApplicationFeeOneByOne").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            if (typeof Application != "undefined")
                Application.ReloadData();

            else
                window.location.reload();
        }
    });
}
function printReceiptDiv(div) {
    var mode;
    var close = mode == "popup";
    var options = { mode: mode, popClose: close };
    $(div).printArea(options);
    window.open(div)
    setTimeout(function () {
        $("#dvPrintReciept").closest("div.modal").modal("hide");
    }, 1000)
}
