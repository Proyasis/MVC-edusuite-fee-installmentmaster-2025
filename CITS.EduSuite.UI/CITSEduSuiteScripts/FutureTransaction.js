var containerOtherAmountTypes = "#dynamicRepeaterOtherAmountTypes";

var FutureTransaction = (function () {
    var getFutureTransaction = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetFutureTransaction").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                branchkey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Branch, Resources.AccountHead, Resources.BillDate, Resources.BillNo, Resources.TotalAmount, Resources.AmountReceived, Resources.AmountPaid, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'AccountHeadName', index: 'AccountHeadName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },                
                { key: false, name: 'BillDate', index: 'OrderDate', editable: true, cellEdit: true, sortable: false, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/M/y' }, width: 50 },
                { key: false, name: 'BillNo', index: 'RowKey', editable: true, cellEdit: true, sortable: true, resizable: false, width: 50 },
                { key: false, name: 'TotalAmount', index: 'TotalAmount', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal', width: 100, formatter: 'currencyFmatter' },
                { key: false, name: 'TotalInAmount', index: 'TotalInAmount', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal', width: 100, formatter: 'currencyFmatter'},
                { key: false, name: 'TotalOutAmount', index: 'TotalOutAmount', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal', width: 100, formatter: 'currencyFmatter' },
             { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 200 },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [Resources.PagingRowNum, 10, 20, 50, 100],
            autowidth: true,
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            sortname: 'RowKey',
            sortorder: 'desc'
            //loadComplete: function (data) {
            //    $("#grid a[ledger-modal='']").each(function () {
            //        AppCommon.EditGridPopup($(this))
            //    }),
            //    $("#grid a[data-payment-modal='']").each(function () {
            //        AppCommon.PaymentPopupWindow(this, $("#hdnAddEditFutureTransactionPayment").val(), Resources.Add + Resources.BlankSpace + Resources.FutureTransactions + Resources.BlankSpace + Resources.Payment,FutureTransactionPayment.FormPaymentRebind);
            //    }),
            //    $("#grid a[data-viewmodal='']").each(function () {
            //        $(this).popupform
            //            ({
            //                modalsize: 'modal-lg',
            //                load: function () {
            //                    setTimeout(function () {
            //                        $(".input-group-addon-end").each(function () {
            //                            AppCommon.SetInputAddOn(this);
            //                        });
            //                    }, 500)
            //                }
            //            });
            //    })
            //}
        }).contextMenu({
            selector: ".jqgrow .context-menu",
            trigger: 'left',
            build: function ($trigger, e) {
                // this callback is executed every time the menu is to be shown
                // its results are destroyed every time the menu is hidden
                // e is the original contextmenu event
                var $tr = $(e.target).closest("tr.jqgrow"),
                    rowid = $tr.attr("id"),
                    item = $("#grid").jqGrid("getRowData", rowid);
                return {
                    callback: function (key, options) {

                        var href = "";
                        switch (key) {
                            case "E":                               
                                FutureTransaction.EditPopup(rowid)                                
                                break;
                            case "P":
                                FutureTransaction.EditPaymentPopup(rowid)
                                break;
                            case "V":
                                FutureTransaction.ViewPopup(rowid)
                                break;
                            case "D":
                                deleteFutureTransaction(rowid);
                                break;

                            default:
                                href = "";

                        }
                    },
                    items: {
                        E: { name: Resources.Edit, icon: "fa-edit" },
                        P: { name: Resources.Payment, icon: "fa-credit-card" },
                        V: { name: Resources.View, icon: "fa-eye" },
                        D: { name: Resources.Delete, icon: "fa-trash" }

                    }
                }

            }
        });

        $("#grid").jqGrid("setLabel", "FutureTransactionName", "", "thFutureTransactionName");
    }

    var getFutureTransactionOtherAmountTypes = function (json) {
        $(containerOtherAmountTypes).repeater(
            {
                show: function () {
                    $(this).slideDown();
                    $("[name*=IsAddition]", $(this)).val("false")
                    AppCommon.FormatSelect2()
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteFutureTransactionItem($(hidden).val(), $(this), remove);

                    }
                    else {
                        $(this).slideUp(remove);
                    }
                },
                data: json,
                repeatlist: 'FutureTransactionOtherAmountTypes',
                defaultValues: json
            });
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
    //            $(".divChequeDetails").show();
    //            $(".divCardDetails").hide();
    //            $(".divBankAccount").show();
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



    var getBalance = function (paymentMode, FutureTransactionPaymentRowKey, BankAccountKey, branchKey, CashFlowTypeKey) {
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&FutureTransactionPaymentRowKey=" + FutureTransactionPaymentRowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey + "&CashFlowTypeKey=" + CashFlowTypeKey)
        $("#BankAccountBalance").val(response);
       
    }


    var editPopup = function (rowid) {
        
        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        //var url = $(_this).attr("data-href");

        var url = $("#hdnAddEditFutureTransaction").val() + "?id=" + rowid;

        $.customPopupform.CustomPopup({
            modalsize: "modal-lg ",
            load: function () {
                setTimeout(function () {
                    $(".input-group-addon-end").each(function () {
                        AppCommon.SetInputAddOn(this);
                    });
                }, 500)
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.reload();
                }
            }
        }, url);

    }
    var editPaymentPopup = function (rowid) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        //var url = $(_this).attr("data-href");
        var url = $("#hdnAddEditFutureTransactionPayment").val() + "?id=" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-md ",
            load: function () {
                setTimeout(function () {
                    $(".input-group-addon-end").each(function () {
                        AppCommon.SetInputAddOn(this);
                    });
                }, 500)
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.reload();
                }
            }
        }, url);

    }
    var viewPopup = function (rowid) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        //var url = $(_this).attr("data-href");
        var url = $("#hdnViewFutureTransaction").val() + "?id=" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg ",
            load: function () {
                setTimeout(function () {
                    $(".input-group-addon-end").each(function () {
                        AppCommon.SetInputAddOn(this);
                    });
                }, 500)
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.reload();
                }
            }
        }, url);

    }

    var hSNCodeDetails = function (_this) {
        var obj = {};
        obj.id = $(_this).val();
        $.ajax({
            url: $("#hdnGetHSNCodeDetails").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            success: function (result) {
                
                $("input[type=text][id*=HSNCode]").val(result.HSNCode);
                $("input[type=text][id*=IGSTPer]").val(result.IGSTPer)
                $("input[type=text][id*=CGSTPer]").val(result.CGSTPer);
                $("input[type=text][id*=SGSTPer]").val(result.SGSTPer);
                $("input[type=text][id*=SGSTPer]").trigger('change')
            }
        });
    }

    var getCompanyState = function () {
        
        var obj = {};
        obj.branchKey = $("#BranchKey").val();
        $.ajax({
            url: $("#hdnGetComapanyStateKey").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            success: function (result) {
                $("#CompanyStateKey").val(result);
                if ($("#StateKey").val() == $("#CompanyStateKey").val()) {
                    $(".IGST").hide();
                    $(".nonIGST").show();
                }
                else {
                    $(".IGST").show();
                    $(".nonIGST").hide();
                }
            }
        });
    }

    var getAccountHeadById = function (Id, ddl) {

        $ddl = ddl;
        $ddl.html(""); // clear before appending new list 
        $ddl.append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.AccountHead));
        if (Id != "") {
            $.ajax({
                url: $("#hdnFillAccountHead").val(),
                type: "GET",
                dataType: "JSON",
                data: { key: Id },
                success: function (result) {
                    $.each(result, function (i, AccountHead) {
                        $ddl.append(
                            $('<option></option>').val(AccountHead.RowKey).html(AccountHead.Text));
                    });
                }
            });
        }
    }

    var getAccountGroup = function (Id) {
        if (Id != "") {
            $.ajax({
                url: $("#hdnGetAccountGroup").val(),
                type: "GET",
                dataType: "JSON",
                data: { accountHeadKey: Id },
                success: function (result) {
                    if (result == Resources.GroupIncome || result == Resources.GroupAsset || result == Resources.GroupDebtors) {
                        $("#MasterCashFlowTypeKey").val(1)
                    }
                    else {
                        $("#MasterCashFlowTypeKey").val(2)
                    }
                    FutureTransaction.PaymentCashFlowTypeChange();
                }
            });
        }
    }

    var calculateGST = function () {
        var TotalAmt = $("#Amount").val();
        var CGSTPer = $("#CGSTPer").val();
        var SGSTPer = $("#SGSTPer").val();
        var IGSTPer = $("#IGSTPer").val();
        var $cGSTAmt = $("#CGSTAmt")
        var $sGSTAmt = $("#SGSTAmt")
        var $iGSTAmt = $("#IGSTAmt")
        var cGSTAmt = 0
        var sGSTAmt = 0
        var iGSTAmt = 0
        TotalAmt = parseFloat(TotalAmt) ? parseFloat(TotalAmt) : 0
        CGSTPer = parseFloat(CGSTPer) ? parseFloat(CGSTPer) : 0
        SGSTPer = parseFloat(SGSTPer) ? parseFloat(SGSTPer) : 0
        IGSTPer = parseFloat(IGSTPer) ? parseFloat(IGSTPer) : 0
        cGSTAmt = (TotalAmt * CGSTPer) / 100;
        sGSTAmt = (TotalAmt * SGSTPer) / 100;
        iGSTAmt = (TotalAmt * IGSTPer) / 100;
        if ($("#StateKey").val() == $("#CompanyStateKey").val()) {
            $cGSTAmt.val(cGSTAmt)
            $sGSTAmt.val(sGSTAmt)
            $iGSTAmt.val(0)
        }
        else {
            $cGSTAmt.val(0)
            $sGSTAmt.val(0)
            $iGSTAmt.val(iGSTAmt)
        }
        if ($("#IsTax").checked == true) {
            $("#NonTaxableAmount").val(0)
            $("#TaxableAmount").val(TotalAmt)
        }
        else {
            $("#NonTaxableAmount").val(TotalAmt)
            $("#TaxableAmount").val(0)
        }
    }

    calculateTotal = function () {
        var Amt = $("#Amount").val();
        var cGSTAmt = $("#CGSTAmt").val()
        var sGSTAmt = $("#SGSTAmt").val()
        var iGSTAmt = $("#IGSTAmt").val()
        var roundOff = $("#RoundOff").val()
        Amt = parseFloat(Amt) ? parseFloat(Amt) : 0
        cGSTAmt = parseFloat(cGSTAmt) ? parseFloat(cGSTAmt) : 0
        sGSTAmt = parseFloat(sGSTAmt) ? parseFloat(sGSTAmt) : 0
        iGSTAmt = parseFloat(iGSTAmt) ? parseFloat(iGSTAmt) : 0
        roundOff = parseFloat(roundOff) ? parseFloat(roundOff) : 0
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
        subTotal = Amt + cGSTAmt + sGSTAmt + iGSTAmt;
        total = subTotal + earnings - deductions - roundOff
        $("#EarningAmount").val(earnings)
        $("#DeductAmount").val(deductions)
        $("#SubTotal").val(subTotal)
        $("#TotalAmount").val(total)

    }

    calculateInstallment = function (evt) {
        var total = $("#TotalAmount").val()
        var downPayment = $("#DownPayment").val()
        var $noOfInstallment = $("#NoOfInstallment")
        var $installmentAmount = $("#InstallmentAmount")
        var noOfInstallment = $noOfInstallment.val()
        var installmentAmount = $installmentAmount.val()
        total = parseFloat(total) ? parseFloat(total) : 0
        downPayment = parseFloat(downPayment) ? parseFloat(downPayment) : 0
        noOfInstallment = parseInt(noOfInstallment) ? parseInt(noOfInstallment) : null
        installmentAmount = parseFloat(installmentAmount) ? parseFloat(installmentAmount) : null
        total = total - downPayment;
        if (evt == 'amt') {
            if (installmentAmount != null && installmentAmount != 0) {
                noOfInstallment = total / installmentAmount
                $noOfInstallment.val(Math.ceil(noOfInstallment))
            }
            else {
                $noOfInstallment.val(installmentAmount)
                $installmentAmount.val(installmentAmount)
            }
        }
        else {
            if (noOfInstallment != null && noOfInstallment != 0) {
                installmentAmount = total / noOfInstallment
                $installmentAmount.val(installmentAmount.toFixed(2))
            }

            else {
                $noOfInstallment.val(noOfInstallment)
                $installmentAmount.val(noOfInstallment)
            }
        }
    }

    calculateAmountType = function ($total, evt, _this) {
        var total = $total.val()
        if (_this) {
            var item = $(_this).closest("[data-repeater-item]")
            var $Amount = $($("[id*=Amount]", "[amount]"), item)
            var $AmountPer = $("[id*=AmountPer]", item)
            var Amount = $Amount.val()
            var AmountPer = $AmountPer.val()
            Amount = parseInt(Amount) ? parseFloat(Amount) : 0
            AmountPer = parseFloat(AmountPer) ? parseFloat(AmountPer) : 0
            if (evt == 'amt') {
                total = parseFloat(total) ? parseFloat(total) : 1
                AmountPer = (Amount * 100) / total
                $AmountPer.val(AmountPer.toFixed(2))
            }
            else {
                total = parseFloat(total) ? parseFloat(total) : 0
                Amount = total * AmountPer / 100
                $Amount.val(Amount.toFixed(2))
            }
        }
        else {
            $("[data-repeater-item]").each(function () {
                var accountHead = $("[id*=AccountHeadKey]", $(this)).val()
                accountHead = parseInt(accountHead) ? parseInt(accountHead) : 0
                if (accountHead != 0) {
                    var $Amount = $($("[id*=Amount]", "[amount]"), $(this))
                    var $AmountPer = $("[id*=AmountPer]", $(this))
                    var Amount = $Amount.val()
                    var AmountPer = $AmountPer.val()
                    Amount = parseInt(Amount) ? parseFloat(Amount) : 0
                    AmountPer = parseFloat(AmountPer) ? parseFloat(AmountPer) : 0
                    total = parseFloat(total) ? parseFloat(total) : 1
                    AmountPer = (Amount * 100) / total
                    $AmountPer.val(AmountPer.toFixed(2))
                }
            })
        }
    }

    var isContraCheck = function (_this) {
        
        if (_this.checked == true) {
           // $("#CashFlowTypeKey").removeAttr('disabled');
            $("#CashFlowTypeKey").removeAttr("disabled");
        } else {
            $("#CashFlowTypeKey").attr("disabled", true);
            //$("#CashFlowTypeKey").attr('disabled', 'disabled');
            var MasterCashFlowTypeKey = $("#MasterCashFlowTypeKey").val();
            $("#CashFlowTypeKey").val(MasterCashFlowTypeKey)
        }
       // $('.selectpicker').selectpicker('refresh');
        $("#CashFlowTypeKey").selectpicker('refresh');

    }

    var paymentCashFlowTypeChange = function () {
        
        if ($("#IsContra")[0].checked) {
            $("#dvOpeningPaidAmount").show()
            $("#dvOpeningReceivedAmount").show()
            if ($("#MasterCashFlowTypeKey").val() == 1) {
                $("#CashFlowTypeKey").val(2)
            }
            else {
                $("#CashFlowTypeKey").val(1)
            }
        }
        else {
            var MasterCashFlowTypeKey = $("#MasterCashFlowTypeKey").val();
            $("#CashFlowTypeKey").val(MasterCashFlowTypeKey)
            $("#CashFlowTypeKey").val($("#MasterCashFlowTypeKey").val())
            if ($("#MasterCashFlowTypeKey").val() == 1) {
                $("#dvOpeningPaidAmount").hide()
                $("#dvOpeningReceivedAmount").show()
            }
            else {
                $("#dvOpeningPaidAmount").show()
                $("#dvOpeningReceivedAmount").hide()
            }
        }
        //$('.selectpicker').selectpicker('refresh');
        $("#CashFlowTypeKey").selectpicker('refresh');
    }

    var calculateBalanceAmount = function () {
        var TotalAmount = $("#TotalAmount").val();
        var Amount = $("#Amount").val();
        var AmountPaid = $("#AmountPaid").val();
        var openingReceived = $("#OpeningReceivedAmount").val();
        var openingPaid = $("#OpeningPaidAmount").val();
        TotalAmount = parseFloat(TotalAmount) ? parseFloat(TotalAmount) : 0
        AmountPaid = parseFloat(AmountPaid) ? parseFloat(AmountPaid) : 0
        Amount = parseFloat(Amount) ? parseFloat(Amount) : 0
        openingReceived = parseFloat(openingReceived) ? parseFloat(openingReceived) : 0
        AmountPaid = parseFloat(AmountPaid) ? parseFloat(AmountPaid) : 0
        var CashFlowTypeKey = $("#CashFlowTypeKey").val()
        var MasterCashFlowTypeKey = $("#MasterCashFlowTypeKey").val()
        if (CashFlowTypeKey != MasterCashFlowTypeKey) {
            var Balace = parseFloat(Amount - AmountPaid);
        }
        else {
            var Balace = parseFloat(TotalAmount - AmountPaid);
        }
        if (CashFlowTypeKey == 1) {
            Balace = Balace - openingReceived;
        }
        else
        {
            Balace = Balace - openingPaid;
        }

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

    var installmentView = function () {
        var totalAmount = 0;
        var installmentFlow = $("#InstallmentFlowKey").val()
        var totalOutAmount = $("#TotalOutAmount").val()
        var totalInAmount = $("#TotalInAmount").val()
        var period = $("#InstallmentPeriod").val()
        var installmentAmount = $("#InstallmentAmount").val()
        var installmentType = $("#InstallmentTypeKey").val()
        var Date = $("#BillDate").val()
        Date = moment(Date, ["DD/MM/YYYY", "DD-MM-YYYY"]);
        totalOutAmount = parseFloat(totalOutAmount) ? parseFloat(totalOutAmount) : 0
        totalInAmount = parseFloat(totalInAmount) ? parseFloat(totalInAmount) : 0
        installmentAmount = parseFloat(installmentAmount) ? parseFloat(installmentAmount) : 0
        period = parseInt(period) ? parseInt(period) : 0
        if (installmentFlow == 1) {
            totalAmount = totalInAmount
        }
        else {
            totalAmount = totalOutAmount
        }
        $("[installmentitem]").each(function (i) {
            var $paidItem = $("[id*=InstallmentPaid]", $(this))
            var $dateItem = $("[id*=InstallmentDate]", $(this))
            if (totalAmount != 0) {
                if (totalAmount > installmentAmount) {
                    $paidItem.val(installmentAmount)
                    totalAmount = totalAmount - installmentAmount
                }
                else {
                    $paidItem.val(totalAmount)
                    totalAmount = 0
                }
            }
            else {
                $paidItem.val(0)
            }
            if (installmentType == Resources.PeriodTypeDay) {
                var futureDate = moment(Date).add((period * (i + 1)), 'd').format('MMM DD YYYY ddd');
                $dateItem.val(futureDate)
            }
            else if (installmentType == Resources.PeriodTypeWeek) {
                var futureDate = moment(Date).add((period * (i + 1)), 'w').format('MMM DD YYYY ddd');
                $dateItem.val(futureDate)
            }
            else if (installmentType == Resources.PeriodTypeMonth) {
                var futureDate = moment(Date).add((period * (i + 1)), 'M').format('MMM DD YYYY ddd');
                $dateItem.val(futureDate)
            }
            else if (installmentType == Resources.PeriodTypeYear) {
                var futureDate = moment(Date).add((period * (i + 1)), 'y').format('MMM DD YYYY ddd');
                $dateItem.val(futureDate)
            }

        })
    }

    var otherAmountChange = function (_this) {
        var item = $(_this).closest("[data-repeater-item]")
        var additionItem = $("[id*=IsAddition]", item)
        var Id = _this.val()
        if (Id != "") {
            $.ajax({
                url: $("#hdnGetAccountGroup").val(),
                type: "GET",
                dataType: "JSON",
                data: { accountHeadKey: Id },
                success: function (result) {
                    if (result == Resources.GroupIncome) {
                        additionItem.prop('checked', false)
                    }
                    else {
                        additionItem.prop('checked', true)
                    }
                }
            });
        }
    }

    var bindAccountHead = function () {
        var Id = $("#AccountHeadKey").val()
        if (Id != "") {
            $.ajax({
                url: $("#hdnGetAccountHeadById").val(),
                type: "GET",
                dataType: "JSON",
                data: { accountHeadKey: Id },
                success: function (response) {
                    $("[data-repeater-item]").each(function () {
                        var dropdDownControl = $("[id*=AccountHeadKey]", $(this))
                        var oldKey = dropdDownControl.val()
                        $(dropdDownControl).empty();
                        $(dropdDownControl).append($("<option></option>").val("").html(Resources.Select + Resources.BlankSpace + Resources.AccountHead));
                        $.each(response, function () {
                            $(dropdDownControl).append($("<option></option>").val(this['RowKey']).html(this['Text']));
                        });
                        dropdDownControl.val(oldKey)
                    })
                }
            });
        }
    }

    //function editLink(cellValue, options, rowdata, action) {
    //    var temp = "'" + rowdata.RowKey + "'";
    //    if (rowdata.IsSystemAccount != true) {
    //        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" onclick="FutureTransaction.EditPopup(this)" data-href="' + $("#hdnAddEditFutureTransaction").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-warning btn-xs" data-modal="" onclick="FutureTransaction.EditPaymentPopup(this)" data-href="' + $("#hdnAddEditFutureTransactionPayment").val() + '/' + rowdata.RowKey + '"  ><i class="fa fa-credit-card" aria-hidden="true"></i></a><a data-viewmodal="" class="btn btn-info btn-xs" data-href="' + $("#hdnViewFutureTransaction").val() + '/' + rowdata.RowKey + '"><i class="fa fa-eye" aria-hidden="true"></i></a><a  class="btn btn-outline-danger btn-xs" href="#"  onclick="javascript:deleteFutureTransaction(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    //    }
    //    else {
    //        return '<div class="divEditDelete"></div>';
    //    }
    //}
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><button type="button" class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</button></div>'
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="AddEditApplication' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm"  onclick="javascript:deleteApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a><a class="btn btn-warning btn-sm" href="' + $("#hdnViewApplication").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';
    }

    return {
        GetFutureTransaction: getFutureTransaction,
        EditPopup: editPopup,
        GetFutureTransactionOtherAmountTypes: getFutureTransactionOtherAmountTypes,
        CheckPaymentMode: checkPaymentMode,
        GetBalance: getBalance,
        HSNCodeDetails: hSNCodeDetails,
        GetCompanyState: getCompanyState,
        GetAccountHeadById: getAccountHeadById,
        CalculateGST: calculateGST,
        CalculateTotal: calculateTotal,
        CalculateInstallment: calculateInstallment,
        CalculateAmountType: calculateAmountType,
        GetAccountGroup: getAccountGroup,
        IsContraCheck: isContraCheck,
        PaymentCashFlowTypeChange: paymentCashFlowTypeChange,
        CalculateBalanceAmount: calculateBalanceAmount,
        InstallmentView: installmentView,
        OtherAmountChange: otherAmountChange,
        BindAccountHead: bindAccountHead,
        EditPaymentPopup: editPaymentPopup,
        ViewPopup: viewPopup,
        CheckPaymentModeSub: checkPaymentModeSub
    }
}());

function deleteFutureTransaction(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_FutureTransaction,
        actionUrl: $("#hdnDeleteFutureTransaction").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteFutureTransactionItem(rowkey, _this, remove) {
    var result = EduSuite.Confirm
        ({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_OrderProcess,
            actionUrl: $("#hdnDeleteFutureTransactionItem").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                $(_this).slideUp(remove)
            }
        });
}

function deleteFutureTransactionPayment(_this, rowkey) {
    var item = $(_this).closest(".salesOrderView")
    var result = EduSuite.Confirm
        ({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Payment,
            actionUrl: $("#hdnDeleteFutureTransactionPayment").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                item.hide()
                $('.modal').modal('hide');
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        });
}

function bindFutureTransactionForm(dialog) {
    $('#frmFutureTransaction', dialog).submit(function () {
        var validate = $('#frmFutureTransaction').valid();
        var form = this;
        if (validate) {
            $(form).mLoading();
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        $('#myModal').modal('hide');
                        window.location.reload();
                    } else {
                        $('#myModalContent').html(result);
                        bindFutureTransactionForm(dialog)
                    }
                    $(form).mLoading("destroy");
                }
            });

        }

        return false;

    });
}



function checkbalance() {
    
    var amount = $("#AmountPaid").val();
    amount = amount != "" ? parseFloat(amount) : 0;
    var paidAmount = parseFloat(amount)
    var AccountHeadBalance = parseFloat($("#BankAccountBalance").val())

    var CashFlowTypeKey = $("#MasterCashFlowTypeKey").val();
    CashFlowTypeKey = parseInt(CashFlowTypeKey) ? parseInt(CashFlowTypeKey) : 0;
    if (CashFlowTypeKey == Resources.CashFlowTypeOut) {
        if (amount > AccountHeadBalance) {
            $("#AmountPaid").val("")
            $.alert({
                type: 'yellow',
                title: Resources.Warning,
                content: "Due to InSufficent Balance in Our account Please Check",
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
}