var EmployeeAdvanceReturn = (function () {
    var getEmployeeAdvanceReturn = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetEmployeeAdvanceReturn").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                BranchKey: function () {
                    return $('#ddlBranch').val()
                },
                EmployeeKey: function () {
                    return $('#ddlEmployee').val()
                },
                FromDate: function () {
                    return $('#txtSearchFromDate').val()
                },
                ToDate: function () {
                    return $('#txtSearchToDate').val()
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.BlankSpace, Resources.ReceiptNo, Resources.Employee, Resources.SalaryPaymentDate, Resources.PaymentMode, Resources.Amount, Resources.Action],
            colModel: [
                { key: false, hidden: true, name: 'ChequeStatusKey', index: 'ChequeStatusKey', editable: true },
                { key: true, hidden: true, name: 'PaymentKey', index: 'PaymentKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, name: 'ReceiptNumber', index: 'ReceiptNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PaymentDate', index: 'PaymentDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/M/y' } },
                { key: false, name: 'PaymentModeName', index: 'PaymentModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PaidAmount', index: 'PaidAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [Resources.PagingRowNum, 10, 20, 50, 100].unique(),
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
            sortname: 'PaymentKey',
            sortorder: 'desc',

        })

        $("#grid").jqGrid("setLabel", "EmployeeAdvanceReturnName", "", "thEmployeeAdvanceReturnName");
    }

    var editPopup = function (_this) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        var url = $(_this).attr("data-href");

        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {
                setTimeout(function () {
                    AppCommon.FormatInputCase();
                }, 500)
            },
            submit: function () {
                formSubmit($("#frmPaymentWindow"))
            }
        }, url);



    }

    var getEmployeesByBranchId = function (Id, ddl) {    

        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnFillEmployees").val(), ddl, Resources.Employee);
    }

    var deductAdvance = function () {
        var advanceAmount = 0;
        var totalAdvance = 0;
        var totalBalance = 0;
        $("[advanceRepeater]").each(function () {
            var item = $(this).find("[id*=IsDeduct]")
            var advanceBalanceControl = $("[advanceBalance]", $(this))
            var paidControl = $("[id*=ReturnAmount]", $(this))
            var beforechangePaid = $("[id*=BeforeTakenAdvance]", $(this)).val()
            beforechangePaid = parseFloat(beforechangePaid) ? parseFloat(beforechangePaid) : 0
            if (item[0].checked) {
                var amount = $(this).find("[id*=ReturnAmount]").val()
                amount = amount != "" ? parseFloat(amount) : 0;
                advanceAmount = advanceAmount + amount;
                $(paidControl).removeAttr('readonly')
            }
            else {
                $(advanceBalanceControl).html(beforechangePaid)
                $(paidControl).val(0)
                $(paidControl).attr('readonly', true)
            }
            var paid = $(paidControl).val()
            var advanceBalance = $(advanceBalanceControl).html()
            advanceBalance = parseFloat(advanceBalance) ? parseFloat(advanceBalance) : 0
            paid = parseFloat(paid) ? parseFloat(paid) : 0
            totalAdvance = totalAdvance + paid;
            totalBalance = totalBalance + advanceBalance;
        })
        $("input#SalaryAdvance").val(advanceAmount);
        $("[totaladvance]").html(totalAdvance);
        $("[totalbalance]").html(totalBalance);
    }

    var validatePaidAmount = function () {
        var totalAdvance = $("[totaladvance]").html()
        totalAdvance = parseFloat(totalAdvance) ? parseFloat(totalAdvance) : 0
        var totalbalance = $("[totalbalance]").html()
        totalbalance = parseFloat(totalbalance) ? parseFloat(totalbalance) : 0
        var paid = $("#PaidAmount").val()
        paid = parseFloat(paid) ? parseFloat(paid) : 0
        var advance = totalAdvance + totalbalance
        if (paid > advance) {
            $("#PaidAmount").val(advance)
        }
    }

    var autoDeduct = function () {
        var paid = $("#PaidAmount").val()
        paid = parseFloat(paid) ? parseFloat(paid) : 0
        $("[advanceRepeater]").each(function () {
            var $returnAmount = $("[id*=ReturnAmount]", this)
            var $isDeduct = $("[id*=IsDeduct]", this)
            var returnAmountVal = $("[id*=ReturnAmount]", this).val()
            returnAmountVal = parseFloat(returnAmountVal) ? parseFloat(returnAmountVal) : 0
            var advancebalance = $("[advancebalance]", this).html()
            advancebalance = parseFloat(advancebalance) ? parseFloat(advancebalance) : 0
            var totalAdvance = returnAmountVal + advancebalance
            if (paid == 0) {
                $isDeduct.attr("checked", false)
                $returnAmount.val(0)
            }
            else if (paid >= totalAdvance) {
                $isDeduct.prop("checked", true)
                $returnAmount.val(totalAdvance)
                paid = paid - totalAdvance
            }
            else {
                $isDeduct.prop("checked", true)
                $returnAmount.val(paid)
                paid = 0
            }
            var beforechangePaid = $("[id*=BeforeTakenAdvance]", this).val()
            var $advanceBalance = $("[advanceBalance]", this)
            returnAmountVal = parseFloat($returnAmount.val()) ? parseFloat($returnAmount.val()) : 0
            beforechangePaid = parseFloat(beforechangePaid) ? parseFloat(beforechangePaid) : 0
            $($advanceBalance).html(beforechangePaid - returnAmountVal)
            if (beforechangePaid < returnAmountVal) {
                $(this).val(beforechangePaid)
                $($advanceBalance).html(0)
            }
        })
        EmployeeAdvanceReturn.DeductAdvance();
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.PaymentKey + "'";
           var printtemp = "'" + rowdata.BranchKey + "'" + ',' + "'" + rowdata.ReceiptNumber + "'" + ',' + "'" + Resources.InvoicePrintTypeOrderReceipt + "'";
        return '<div class="divEditDelete"><a  data-modal="" class="btn btn-outline-primary btn-sm mx-1" onclick="EmployeeAdvanceReturn.EditPopup(this)" data-href="' + $("#hdnAddEditEmployeeAdvanceReturn").val() + '/' + rowdata.PaymentKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteEmployeeAdvanceReturn(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-success btn-sm mx-1 btnPrint" onclick = "PrintInvoice.PrintReceipt(null, ' + printtemp + ')" > <i class="fa fa-print" aria-hidden="true"></i></a ></div>';
    }
    return {
        GetEmployeeAdvanceReturn: getEmployeeAdvanceReturn,
        EditPopup: editPopup,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        DeductAdvance: deductAdvance,
        ValidatePaidAmount: validatePaidAmount,
        AutoDeduct: autoDeduct
    }
}());


function deleteEmployeeAdvanceReturn(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeAdvanceReturn,
        actionUrl: $("#hdnDeleteEmployeeAdvanceReturn").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

var formSubmit = function (form) {
    var validator = $(form).validate();
    var validate = $(form).valid();
    if (validate) {
        var totalAdvance = $("[totaladvance]").html()
        totalAdvance = parseFloat(totalAdvance) ? parseFloat(totalAdvance) : 0
        var paid = $("#PaidAmount").val()
        paid = parseFloat(paid) ? parseFloat(paid) : 0
        if (totalAdvance != paid) {
            $("[data-valmsg-for=error_msg_payment]").html("Amount Paid and Deduct Amount Must have equal")
            $(form).mLoading("destroy");
            return false;
        }
        $(form).mLoading();
        $("[disabled]", $(form)).removeAttr("disabled");
        $.ajax({
            url: $(form)[0].action,
            type: $(form)[0].method,
            data: $(form).serialize(),
            success: function (response) {
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
                                    $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                                }
                            }
                        }
                    })
                    $('.modal').modal('hide');
                }
                else {
                    $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                }
                $(form).mLoading("destroy");
            },
            error: function (xhr) {
                $(form).mLoading("destroy");
            }
        });
    }
    else {
        validator.focusInvalid();
    }

}



