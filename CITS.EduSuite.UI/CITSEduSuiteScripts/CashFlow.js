var jsonModel = [];
var CashFlow = (function () {

    var getCashFlows = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json', page: 1 }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetCashFlows").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchAccountHead: function () {
                    return $('#txtsearch').val()
                },
                branchKey: function () {
                    return $('#BranchKey').val()
                },
                CashFlowTypeKey: function () {
                    return $('#CashFlowTypeKey').val()
                },
                AccountHeadKey: function () {
                    return $('#AccountHeadKey').val()
                },
                SearchDate: function () {
                    return $('#SearchDate').val()
                }
            },
            //colNames: [Resources.RowKey, Resources.Payment + Resources.BlankSpace + Resources.Date, Resources.Amount, Resources.PaymentModeKey, Resources.CardNumber, Resources.Bank + Resources.BlankSpace + Resources.Name, Resources.ChequeOrDDNumber, Resources.ChequeOrDDDate, Resources.PartyTypeKey, Resources.PartyKey, Resources.VoucherNumber, Resources.TransactionTypeKey, Resources.TransactionKey, Resources.Purpose, Resources.PaidBy, Resources.AuthorizedBy, Resources.ReceivedBy, Resources.OnBehalfOf, Resources.Remarks, Resources.Action],
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.VoucherNumber, Resources.Branch, Resources.AccountHead, Resources.CashFlowType, Resources.Date, Resources.Amount, Resources.PaymentMode, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'CashFlowKey', index: 'CashFlowKey', editable: true },
                { key: false, hidden: true, name: 'CashFlowTypeKey', index: 'CashFlowTypeKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, hidden: true, name: 'ReceiptNumber', index: 'ReceiptNumber', editable: true },
                { key: false, hidden: true, name: 'ChequeStatusKey', index: 'ChequeStatusKey', editable: true },

                { key: false, name: 'ReceiptNumber', index: 'ReceiptNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AccountHeadName', index: 'AccountHeadName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CashFlowTypeName', index: 'CashFlowTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CashFlowDate', index: 'CashFlowDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'Amount', index: 'Amount', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: RupeeIcon, classes: 'text-right-decimal' },
                { key: false, name: 'PaymentModeName', index: 'PaymentModeName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: Chequestatus },

                //{ key: false, name: 'CardNumber', index: 'CardNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'BankName', index: 'BankName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ChequeOrDDNumber', index: 'ChequeOrDDNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ChequeOrDDDate', index: 'ChequeOrDDDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },

                //{ key: false, name: 'TransactionTypeName', index: 'TransactionTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'TransactionKey', index: 'TransactionKey', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'Remarks', index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'CashFlowKey', sortable: false, formatter: editCashFlowLink, resizable: false, width: 150 },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [5, 10, 20, 50, 100],
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
            sortname: 'CashFlowKey',
            sortorder: 'desc',
            altclass: 'jqgrid-altrow',
            //loadComplete: function (data) {
            //    $("#grid a[data-modal='']").each(function () {
            //        AppCommon.EditGridPopup($(this))
            //    })
            //}
        });
        $("#grid").jqGrid("setLabel", "PaymentName", "", "thPaymentName");
    }



    function Chequestatus(cellValue, options, rowdata, action) {
        var string = rowdata.PaymentModeName;

        if (rowdata.ChequeStatusKey == Resources.ProcessStatusApproved) {
            string = '<span style="color:green;font-weight:600"> Cheque ' + rowdata.ChequeAction + '  </span> '
        }
        else if (rowdata.ChequeStatusKey == Resources.ProcessStatusRejected) {
            string = '<span style="color:red;font-weight:600""> Cheque ' + rowdata.ChequeAction + '  </span>'
        }

        return string;
    }
    function RupeeIcon(cellValue, option, rowdata, action) {

        var amount = parseFloat(cellValue) ? parseFloat(cellValue) : 0;
        amount = amount.toLocaleString();
        return '<i  class="fa fa-inr" aria-hidden="true"></i>' + amount;
    }

    var getAccountHeadById = function () {
        var isContra = $("#IsContra").prop("checked")
        isContra = false
        var key = $("#AccountGroupKey").val()
        $ddl = $("#AccountHeadKey", $("#frmCashFlow"));
        $ddl.html(""); // clear before appending new list 
        $ddl.append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.AccountHead));
        $.ajax({
            url: $("#hdnFillAccountHead").val(),
            type: "GET",
            dataType: "JSON",
            data: { key: key, isContra: isContra },
            success: function (result) {
                $.each(result, function (i, AccountHead) {
                    $ddl.append(
                        $('<option></option>').val(AccountHead.RowKey).html(AccountHead.Text));
                });
                $("#AccountHeadKey", $("#frmCashFlow")).val("").selectpicker("refresh");
            }
        });
    }
      

    var checkPaymentMode = function (PayMode, json) {
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

    var getBalance = function (paymentMode, RowKey, BankAccountKey, branchKey, CashFlowTypeKey) {
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&RowKey=" + RowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey + "&CashFlowTypeKey=" + CashFlowTypeKey)

        $("#AccountHeadBalance").val(response);


    }

    var getAccountHeadBalance = function (Amount, RowKey, AccountKey) {
        var response = AjaxHelper.ajax("GET", $("#hdnGetAccountHeadBalance").val() + "?Amount=" + Amount + "&RowKey=" + RowKey + "&AccountHeadKey=" + AccountKey)

        $("#AccountHeadBalance").val(response);
        $("#AccountHeadBalance").focus();
        var validator = $('#frmCashFlow').validate({
            ignore: 'input[type="button"],input[type="submit"]'
        });
        validator.element($("#AccountHeadBalance"));
    }

    function editCashFlowLink(cellValue, options, rowdata, action) {

        var temp = "'" + rowdata.CashFlowKey + "','" + rowdata.CashFlowTypeKey + "'";
        var printtemp = "'" + rowdata.BranchKey + "'" + ',' + "'" + rowdata.ReceiptNumber + "'" + ',' + "'" + Resources.InvoicePrintTypeOrderReceipt + "'";
        var obj = {};
        obj.cashFlowTypeKey = rowdata.CashFlowTypeKey ? rowdata.CashFlowTypeKey : 0;
        obj.branchKey = rowdata.BranchKey ? rowdata.BranchKey : 0;
        obj.partyKey = 0;
        url = $("#hdnAddEditCashFlow").val() + "/" + rowdata.CashFlowKey + "?" + $.param(obj);
       
        var IsPayment = rowdata.CashFlowTypeKey == 2 ? true : false;

        if (rowdata.ChequeStatusKey == Resources.ProcessStatusApproved) {
            return '<div class="divEditDelete"><a  class="btn btn-outline-danger btn-sm mx-1" href="#"  onclick="javascript:deleteCashFlow(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-success btn-sm mx-1 btnPrint" onclick="changePrintHtml(' + rowdata.CashFlowTypeKey + ');PrintInvoice.PrintReceipt(null, ' + printtemp + ')"><i class="fa fa-print" aria-hidden="true"></i></a></div>';

        }
        else {
            return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" onclick="CashFlow.EditPopup(' + IsPayment + ',' + rowdata.CashFlowKey + ')" ><i class="fa fa-pencil" aria-hidden="true"></i></a><a  class="btn btn-outline-danger btn-sm mx-1" href="#"  onclick="javascript:deleteCashFlow(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-success btn-sm mx-1 btnPrint" onclick="changePrintHtml(' + rowdata.CashFlowTypeKey + ');PrintInvoice.PrintReceipt(null, ' + printtemp + ')"><i class="fa fa-print" aria-hidden="true"></i></a></div>';
        }

        
    }

    var setUrlParam = function (lnk, data) {
        var obj = {};
        obj.branchKey = data.BranchKey ? data.BranchKey : 0;
        var url = $(lnk).data("href");
        url = url + "?" + $.param(obj);
        $(lnk).attr("data-href", url);
    }

    var editPopup = function (IsPayment, Id) {


        var obj = {};
        obj.IsPayment = IsPayment;
        if ($("#BranchKey").val())
            obj.branchKey = $("#BranchKey").val();
        //obj.Id = Id;
        url = $("#hdnAddEditCashFlow").val() + "/" + Id + "?" + $.param(obj);

        // var url = $("#hdnAddEditCashFlow").val() + '?' + $.param(obj);
        var validator = null

        $.customPopupform.CustomPopup({
            modalsize: "modal-md ",
            load: function () {

            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.reload();
                }
            }
        }, url);

    }


    var editPopupRebind = function (_this, data) {
        var controls = $("#frmCashFlow").find("select,input,textarea");
        $.each(controls, function () {
            if ($(this).attr("name") == "PaymentModeKey") {
                var radio = $(this);
                if (radio[0].checked == true) {
                    var name = $(this).attr("name");
                    data[name] = $(this).val();
                }
            }
            else {
                var name = $(this).attr("name");
                data[name] = $(this).val();
            }

        })
        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        var url = $(_this).attr("data-href");
        data = { model: data };
        $.ajaxSetup({ cache: false, async: false });
        $.post(url, data, function (response) {
            $('#myModalContent').html(response);
            $.validator.unobtrusive.parse($('#frmCashFlow'));
            $("#myModal").one('show.bs.modal', function () {

                bindCashFlowForm(this);

            }).modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');

        });

    }

    var bindParty = function () {
        if ($("#IsOrderPayment").val() == true) {
            var key = $("#CashFlowTypeKey", $("#frmCashFlow")).val()
            var branchKey = $("#BranchKey", $("#frmCashFlow")).val()
            if ($("#BranchKey", $("#frmCashFlow")).val() != "" && $("#CashFlowTypeKey").val() != "") {
                key = $("#CashFlowTypeKey", $("#frmCashFlow")).val()
                branchKey = $("#BranchKey", $("#frmCashFlow")).val()
                var response = AjaxHelper.ajax("GET", $("#hdnFillPartyAccountHead").val() + "?key=" + key + "&branchKey=" + branchKey)
                $("#AccountHeadKey", $("#frmCashFlow")).empty();
                $("#AccountHeadKey", $("#frmCashFlow")).append($("<option></option>").val("").html(Resources.Select + Resources.BlankSpace + Resources.AccountHead));
                $.each(response, function () {
                    $("#AccountHeadKey", $("#frmCashFlow")).append($("<option></option>").val(this['RowKey']).html(this['Text']));
                });
            }
        }
    }

    var bindBalanceAmount = function () {
        var remainBalace = $("#Amount").val()
        var TotalPaid = 0;
        var orderNumbers = '';
        CashFlow.SetPurpose();
        var purpose = $("#Purpose").val()
        remainBalace = parseFloat(remainBalace) ? parseFloat(remainBalace) : 0;
        if (remainBalace == NaN || remainBalace == 0) {
            $("[pendingAccountRepeater]").each(function () {
                var disableChk = $(".chkClearAccount", $($(this)))
                if (disableChk[0].checked) {
                    disableChk[0].checked = false;
                }
                disableChk.attr('disabled', 'disabled');
            });
        }
        else {
            $("[pendingAccountRepeater]").each(function () {
                var enableChk = $(".chkClearAccount", $($(this)))
                enableChk.removeAttr('disabled')

            });
        }
        var grandTotal = 0;
        var totalPaidAmount = 0;
        var totalBillBalance = 0;
        var totalBalance = 0;
        var grandDiscount = 0;
        var amountToBePaid = $("#TotalAmountToBePaidBeforeDiscount").val()
        amountToBePaid = parseFloat(amountToBePaid) ? parseFloat(amountToBePaid) : 0;
        $("[pendingAccountRepeater]").each(function () {
            var $balanceAmountControl = $(this).find("[id*=AmountBalace]")
            var $paidAmount = $(this).find("input[type='hidden'] [id*=AmountPaid]")
            var $totalAmount = $(this).find("[id*=AmountTotal]")
            var $billBalanceAmount = $(this).find("[id*=BillBalanceAmount]")
            var paidAmount = $paidAmount.val();
            var discount = $(this).find("[id*=DiscountAmount]").val()
            discount = parseFloat(discount) ? parseFloat(discount) : 0
            amountToBePaid = amountToBePaid - discount;
            $("#TotalAmountToBePaid").val(amountToBePaid)
            paidAmount = parseFloat(paidAmount) ? parseFloat(paidAmount) : 0;
            var oldBalanceAmount = $(this).find("[id*=BillBalanceAmount]").val();
            oldBalanceAmount = parseFloat(oldBalanceAmount) ? parseFloat(oldBalanceAmount) : 0;
            var chk = $(".chkClearAccount", $(this))
            if (remainBalace <= 0) {
                chk[0].checked = false;
                $balanceAmountControl.val(oldBalanceAmount);
                return false;
            }
            $balanceAmountControl.val(oldBalanceAmount);
            if (chk[0].checked) {
                remainBalace = remainBalace - oldBalanceAmount;
                if (remainBalace < 0) {
                    $balanceAmountControl.val(-(remainBalace));
                    $("[pendingAccountRepeater]").each(function () {
                        var disableChk = $(".chkClearAccount", $($(this)))
                        if (!disableChk[0].checked) {
                            disableChk.attr('disabled', 'disabled');
                        }
                    });
                }
                else {
                    $balanceAmountControl.val(0);
                }
                var referenceNumber = $(this).closest("[pendingAccountRepeater]").find("[id*=ReferenceNumber]").val()
                orderNumbers = orderNumbers + referenceNumber + ','
            }
            TotalPaid = TotalPaid + oldBalanceAmount - parseFloat($balanceAmountControl.val());


        });
        $("[pendingAccountRepeater]").each(function () {
            var $paidAmount = $(this).find('input[type="hidden"][id*=AmountPaid]')
            var $balanceAmountControl = $(this).find("[id*=AmountBalace]")
            var $totalPaidAmount = $(this).find("[id*=TotalAmountPaid]")
            var $totalAmount = $(this).find("[id*=AmountTotal]")
            var $billBalanceAmount = $(this).find("[id*=BillBalanceAmount]")
            var discountTotal = $(this).find("[id*=DiscountTotal]").val()
            discountTotal = discountTotal ? parseFloat(discountTotal) : 0;
            $paidAmount.val(parseFloat($billBalanceAmount.val()) - parseFloat($balanceAmountControl.val()))
            grandTotal = parseFloat(grandTotal) + parseFloat($totalAmount.val());
            totalPaidAmount = parseFloat(totalPaidAmount) + parseFloat($totalPaidAmount.val())
            totalBillBalance = parseFloat(totalBillBalance) + parseFloat($billBalanceAmount.val());
            totalBalance = parseFloat(totalBalance) + parseFloat($balanceAmountControl.val());
            grandDiscount = grandDiscount + discountTotal;

        });
        $("#GrandTotal").val(AppCommon.RoundTo(grandTotal, 2));
        $("#GrandTotalAmountPaid").val(totalPaidAmount)
        $("#TotalBillBalance").val(totalBillBalance)
        $("#TotalBalance").val(totalBalance)
        $("#GrandDisount").val(grandDiscount)
        var RemainAmount = parseFloat(($("#Amount").val()) - TotalPaid)
        RemainAmount = parseFloat(RemainAmount) ? parseFloat(RemainAmount) : 0;
        $("#lblRemainBalance").html(RemainAmount);
        orderNumbers = orderNumbers.slice(0, -1)
        $("#OrderNumber").val(orderNumbers);
        var isOrder = $("#IsOrderPayment").val()
        if ($("#IsOrderPayment").val() == 'True') {
            $("#Purpose").val(purpose + orderNumbers + ')')
        }
        else {
            CashFlow.SetPurpose()
        }
    }

    var bindGrandTotal = function () {
        var grandTotal = 0;
        var totalPaidAmount = 0;
        var totalBillBalance = 0;
        var totalBalance = 0;
        var grandDiscount = 0;
        $("[pendingAccountRepeater]").each(function () {
            var $paidAmount = $(this).find('input[type="hidden"][id*=AmountPaid]')
            var $balanceAmountControl = $(this).find("[id*=AmountBalace]")
            var $totalPaidAmount = $(this).find("[id*=TotalAmountPaid]")
            var $totalAmount = $(this).find("[id*=AmountTotal]")
            var $billBalanceAmount = $(this).find("[id*=BillBalanceAmount]")
            var discountTotal = $(this).find("[id*=DiscountTotal]").val()
            discountTotal = discountTotal ? parseFloat(discountTotal) : 0;
            $paidAmount.val(parseFloat($billBalanceAmount.val()) - parseFloat($balanceAmountControl.val()))
            grandTotal = parseFloat(grandTotal) + parseFloat($totalAmount.val());
            totalPaidAmount = parseFloat(totalPaidAmount) + parseFloat($totalPaidAmount.val())
            totalBillBalance = parseFloat(totalBillBalance) + parseFloat($billBalanceAmount.val());
            totalBalance = parseFloat(totalBalance) + parseFloat($balanceAmountControl.val());
        });
        $("#GrandTotal").val(grandTotal)
        $("#GrandTotalAmountPaid").val(totalPaidAmount)
        $("#TotalBillBalance").val(totalBillBalance)
        $("#TotalBalance").val(totalBalance)
        $("#GrandDisount").val(grandDiscount)
    }

    var setPurpose = function () {
        var AccountHeadText = ($("#AccountHeadKey", $("#frmCashFlow")).val() != "0" && $("#AccountHeadKey", $("#frmCashFlow")).val() != "" ? $("#AccountHeadKey :selected", $("#frmCashFlow")).text() : "");
        if ($("#IsOrderPayment").val() == 'True') {
            if ($("#CashFlowTypeKey", $("#frmCashFlow")).val() == "1") {
                $("#Purpose").val(Resources.OrderReceiptof)
            }
            else {
                $("#Purpose").val(Resources.OrderPaymentof);
            }
        }
        else {
            if ($("#CashFlowTypeKey", $("#frmCashFlow")).val() == "1") {
                $("#Purpose").val('Reciept Of (' + ($("#AccountHeadKey", $("#frmCashFlow")).val() != "0" && $("#AccountHeadKey", $("#frmCashFlow")).val() != "" ? $("#AccountHeadKey :selected", $("#frmCashFlow")).text() : ""))
            }
            else {
                $("#Purpose").val('Payment Of (' + ($("#AccountHeadKey", $("#frmCashFlow")).val() != "0" && $("#AccountHeadKey", $("#frmCashFlow")).val() != "" ? $("#AccountHeadKey :selected", $("#frmCashFlow")).text() : ""));
            }
            $("#Purpose").val($("#Purpose").val() + ')')
        }
    }


    var bindTotlalAmountToBePaid = function () {
        var grandTotal = 0;
        var totalPaidAmount = 0;
        var totalBillBalance = 0;
        var totalBalance = 0;
        var totalAmountPaid = 0;
        var amountPaid = $("#Amount").val()
        amountPaid = parseFloat(amountPaid) ? parseFloat(amountPaid) : 0
        $("[pendingAccountRepeater]").each(function () {
            var $balanceAmountControl = $(this).find("[id*=AmountBalace]")
            var $paidAmount = $(this).find("[id*=TotalAmountPaid]")
            var $totalAmount = $(this).find("[id*=AmountTotal]")
            var $billBalanceAmount = $(this).find("[id*=BillBalanceAmount]")
            var balanceAmount = parseFloat($(this).find("[id*=AmountBalace]").val())
            totalAmountPaid = parseFloat(totalAmountPaid) + balanceAmount;
            grandTotal = parseFloat(grandTotal) + parseFloat($totalAmount.val());
            totalPaidAmount = parseFloat(totalPaidAmount) + parseFloat($paidAmount.val())
            totalBillBalance = parseFloat(totalBillBalance) + parseFloat($billBalanceAmount.val());
            totalBalance = parseFloat(totalBalance) + parseFloat($balanceAmountControl.val());
        });
        $("#GrandTotal").val(AppCommon.RoundTo(grandTotal, 2));
        $("#GrandTotalAmountPaid").val(AppCommon.RoundTo(totalPaidAmount, 2))
        $("#TotalBillBalance").val(AppCommon.RoundTo(totalBillBalance, 2))
        $("#TotalBalance").val(AppCommon.RoundTo(totalBalance, 2))
        totalAmountPaid = totalAmountPaid != "" ? parseFloat(totalAmountPaid) : 0;
        $("#TotalAmountToBePaid").val(AppCommon.RoundTo((totalAmountPaid + amountPaid), 2));
        $("#TotalAmountToBePaidBeforeDiscount").val(AppCommon.RoundTo((totalAmountPaid + amountPaid), 2));
    }

    var onLoad = function () {
        var rowKey = $("#CashFlowKey").val();
        var IsOrderPayment = $("#IsOrderPayment").val();
        if (rowKey != 0) {
            if ($("#IsOrderPayment").val() == true) {

                $("#addEditAccountHead").hide()
                $("#CashFlowMaster").find('select, input, textarea').attr('disabled', 'disabled');
            }
            else {
                //  $("#IsOrderPayment").hide().parent().parent().parent().hide()
            }
        }
    }

    var contraChange = function () {

        var isContra = $("#IsContra").prop("checked")
        isContra = JSON.parse(isContra) ? JSON.parse(isContra) : false
        var $CashFlowType = $("#CashFlowTypeKey", $("#frmCashFlow"))
        var $AccountGroup = $("#AccountGroupKey")
        var modeCheque = $("[id*=rbPaymentMode]").filter(function (n, p) {
            return p.value == Resources.PaymentModeCheque
        })
        var modeCash = $("[id*=rbPaymentMode]").filter(function (n, p) {
            return p.value == Resources.PaymentModeCash
        })
        if (isContra) {
            $("#CashFlowTypeKey", $("#frmCashFlow")).val(Resources.CashFlowTypeOut)
            $CashFlowType.attr("disabled", "disabled")
            $AccountGroup.attr("disabled", "disabled")
            var accountHead = $("#AccountHeadKey", $("#frmCashFlow")).val()
            modeCheque.attr("checked", false)
            modeCheque.closest('label').hide()
            if (accountHead == Resources.HeadPettycashaccount) {
                modeCash.attr("checked", false)
                modeCash.closest('label').hide()
            }
            else {
                modeCash.closest('label').show()
            }
            CashFlow.ContraPurpose()
        }
        else {
            var rowKey = $("#CashFlowKey").val();
            if (rowKey == 0) {
                $CashFlowType.removeAttr("disabled")
                $AccountGroup.removeAttr("disabled")
            }

            modeCheque.closest('label').show()
            modeCash.closest('label').show()
            CashFlow.SetPurpose()
        }
        //var obj = {}
        //obj.key = $("#BranchKey").val()
        //obj.headKey = $("#AccountHeadKey").val()        
        //AppCommon.BindDropDownbyId(obj, $("#hdnFillBankAccount").val(), $("#BankAccountKey"), Resources.BankAccount)
    }

    var contraPurpose = function () {
        var accountHead = $("#AccountHeadKey", $("#frmCashFlow")).val()
        var accountHeadText = $("#AccountHeadKey option:selected", $("#frmCashFlow")).text()
        var BankAccount = $("#BankAccountKey option:selected").text()
        var PaymentMode = $("[id*=rbPaymentMode]:checked").val()
        var purpose = ""
        if (accountHead == Resources.HeadPettycashaccount) {
            purpose = "Cash Withdraw from " + BankAccount
        }
        else {
            if (PaymentMode == Resources.PaymentModeCash) {
                purpose = "Cash Deposited to " + accountHeadText
            }
            else {
                purpose = "Cash Transfer from " + BankAccount + " To " + accountHeadText
            }
        }
        $("#Purpose").val(purpose)
    }

    return {
        GetCashFlows: getCashFlows,
        GetAccountHeadById: getAccountHeadById,
        CheckPaymentMode: checkPaymentMode,
        SetUrlParam: setUrlParam,
        EditPopup: editPopup,
        EditPopupRebind: editPopupRebind,
        BindBalanceAmount: bindBalanceAmount,
        BindTotlalAmountToBePaid: bindTotlalAmountToBePaid,
        OnLoad: onLoad,
        BindParty: bindParty,
        GetBalance: getBalance,
        SetPurpose: setPurpose,
        BindGrandTotal: bindGrandTotal,
        GetAccountHeadBalance: getAccountHeadBalance,
        ContraChange: contraChange,
        ContraPurpose: contraPurpose,
        CheckPaymentModeSub: checkPaymentModeSub
    }

}());

function changePrintHtml(key) {
    if (key != Resources.CashFlowTypeIn) {
        $("#hdnReceiptPath").val($("#hdnVoucherPath").val())
    }
    else {
        $("#hdnReceiptPath").val($("#hdnDummyReceiptPath").val())
    }
}

function bindCashFlowForm(dialog) {
    $('#frmCashFlow', dialog).submit(function () {
        var validate = $('#frmCashFlow').valid();
        var form = this;
        if (validate) {
            $(form).mLoading();
            var checkedCount = $("[pendingAccountRepeater] input.chkClearAccount:checked").length;
            var chkCount = $("[pendingAccountRepeater] input.chkClearAccount").length;
            var balance = $("#TotalBalance").val()
            balance = parseFloat(balance) ? parseFloat(balance) : 0
            if (checkedCount != 0 && chkCount != 0 && balance != 0) {
                $("[data-valmsg-for=paymenterror_msg]").html("Balance Must have zero")
                $(form).mLoading("destroy");
                return false;
            }
            $("#CashFlowMaster").find('select, input, textarea').removeAttr('disabled');
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
                        bindCashFlowForm(dialog)
                    }
                    $(form).mLoading("destroy");
                }
            });

        }

        return false;

    });
}

function objectifyForm(formArray, returnArray) {//serialize data function

    for (var i = 1; i < formArray.length; i++) {

        var subModelName = '', index = 0, keyName = ''
        var name = formArray[i]['name'];
        var tempName = name.match(/^[^\]}),]*/) ? name.match(/^[^\]}),]*/)[0] : name;
        keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;
        var arr = tempName.split('[');
        if (arr.length == 2) {
            subModelName = arr[0];
            index = arr[1];
        }
        else {
            keyName = arr[0];
        }
        if (subModelName == "") {
            returnArray[name] = formArray[i]['value'];
        }
        else {
            if (!returnArray[subModelName]) {
                returnArray[subModelName] = [];
            }
            if (!returnArray[subModelName][index]) {
                returnArray[subModelName][index] = $.extend(true, {}, returnArray[subModelName][0]) || {};
            }
            returnArray[subModelName][index][keyName] = formArray[i]['value'];
        }
    }
    return returnArray
}

function deleteCashFlow(rowkey) {
    //var result = confirm(Resources.Delete_Confirm_Journal);
    //if (result == true) {
    //    var response = AjaxHelper.ajax("POST", $("#hdnDeleteCashFlow").val(),
    //                {
    //                    id: rowkey
    //                });

    //    if (response.Message === Resources.Success) {
    //        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
    //    }
    //    else
    //        alert(response.Message);
    //    event.preventDefault();
    //}

    var result = EduSuite.Confirm
        ({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Journal,
            actionUrl: $("#hdnDeleteCashFlow").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        });
}