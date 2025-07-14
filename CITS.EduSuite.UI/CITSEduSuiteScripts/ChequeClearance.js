var ChequeClearance = (function () {
    var getChequeClearance = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetChequeClearanceList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                branchKey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.TransactionType, Resources.Branch, Resources.TransactionType,Resources.CashFlowType, Resources.ChequeOrDDNumber, Resources.ChequeOrDDDate, Resources.Amount, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'TransactionTypeKey', index: 'TransactionTypeKey', editable: true },
                //{ key: false, name: 'AccountHeadName', index: 'AccountHeadName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TransactionTypeName', index: 'TransactionTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CashFlowTypeName', index: 'CashFlowTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ChequeOrDDNumber', index: 'ChequeOrDDNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ChequeOrDDDate', index: 'ChequeOrDDDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'Amount', index: 'Amount', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal', formatter: 'currencyFmatter'},
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [5, 10, 15, 20],
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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow'
        })

        $("#grid").jqGrid("setLabel", "ChequeClearanceName", "", "thChequeClearanceName");
    }

    //var editPopup = function (_this) {        
    //    var validator = null
    //    var url = $(_this).attr("data-href");

    //    $('#myModalContent').load(url, function () {

    //        $("#myModal").one('show.bs.modal', function () {
    //            $.validator.unobtrusive.parse($('#frmChequeClearance'));
    //            bindChequeClearanceForm(this);

    //        }).modal({
    //            backdrop: 'static',
    //            keyboard: false
    //        }, 'show');
    //    });
    //}

    var editPopup = function (Id, type) {
        var obj = {};
        obj.type = type;
        obj.Id = Id;
        //url = $("#hdnAddEditCashFlow").val() + "/" + Id + "?" + $.param(obj);

        var url = $("#hdnClearCheques").val() + '?' + $.param(obj);
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

    function formSubmit() {

        var $form = $("#frmChequeClearance")
        var JsonData = [];
        $("[disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {
           
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
            if (typeof response == "string") {
                $("[data-valmsg-for=error_msg]").html(response);
            }
            else if (response.IsSuccessful) {
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
                                window.location.href = $("#hdnGetChequeClearanceList").val();
                            }
                        }
                    }
                })

            }

        }
    }

    var checkPaymentMode = function (PayMode, json) {
        jsonModel = json;
        $("#dvPaymentModes").show();
        switch (parseInt(PayMode || 0)) {
            case 2:
                $(".divCardDetails").show();
                $(".divBankAccount,.divChequeDetails").hide();
                break;
            case 3:
                $(".divBankAccount").show();
                break;
            case 4:
                $(".divChequeDetails").show();
                $(".divCardDetails,.divBankAccount").hide();
                break;
            default:
                $(".divChequeDetails,.divCardDetails,.divBankAccount,#dvPaymentModes").hide();
                break;
        }
    }

    var getBalance = function (paymentMode, PurchaseOrderPaymentRowKey, BankAccountKey, branchKey) {
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&PurchaseOrderPaymentRowKey=" + PurchaseOrderPaymentRowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey)
        $("#BankAccountBalance").val(response);
    }

    var hideCashMode = function () {
        if ($("#CashFlowTypeKey").val() == 2) {
            $("#PaymentModeRadios input[type='radio']").each(function () {
                if ($(this).val() == 1) {
                    $(this).hide().parent().hide();
                }
            });
        }
    }

    function editLink(cellValue, options, rowdata, action) {

        var temp = "'" + rowdata.RowKey + "'";
        var chequeDate = AppCommon.JsonDateToNormalDate(rowdata.ChequeOrDDDate);
        var date = []
        date = chequeDate.split('/')
        chequeDate = new Date(date[2], date[1] - 1, date[0])
        var todayDate = new Date();
        if (chequeDate <= todayDate) {
            //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-xs" onclick="ChequeClearance.EditPopup(this)" data-href="' + $("#hdnClearCheques").val() + '?id=' + rowdata.RowKey + '&type=' + rowdata.TransactionTypeKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Clear + '</div>';
            return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-xs" onclick="ChequeClearance.EditPopup(' + rowdata.RowKey + ',' + rowdata.TransactionTypeKey + ')"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Clear + '</div>';

        }
        else {
            return '<div class="divEditDelete"></div>';
        }
    }
    return {
        GetChequeClearance: getChequeClearance,
        EditPopup: editPopup,
        CheckPaymentMode: checkPaymentMode,
        GetBalance: getBalance,
        HideCashMode: hideCashMode,
        FormSubmit: formSubmit
    }
}());

function deleteChequeClearance(rowkey) {
    var result = confirm(Resources.Delete_Confirm_ChequeClearance);
    if (result == true) {
        var response = AjaxHelper.ajax("POST", $("#hdnDeleteChequeClearance").val(),
            {
                id: rowkey
            });

        if (response.Message === Resources.Success) {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
        else
            alert(response.Message);
        event.preventDefault();
    }
}


function bindChequeClearanceForm(dialog) {
    $('#frmChequeClearance', dialog).submit(function () {
        var validate = $('#frmChequeClearance').valid();
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
                        bindChequeClearanceForm(dialog)
                    }
                    $(form).mLoading("destroy");
                }
            });

        }

        return false;

    });
}

