var EmployeeSalaryAdvance = (function () {
    var getEmployeeSalaryAdvance = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetEmployeeSalaryAdvances").val(),
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
            colNames: [Resources.RowKey, Resources.RowKey, Resources.BlankSpace, Resources.VoucherNumber, Resources.Employee, Resources.SalaryPaymentDate, Resources.PaymentMode, Resources.Amount, Resources.Status, Resources.ClearedAmount, Resources.BalanceAmount, Resources.Action],
            colModel: [
                { key: false, hidden: true, name: 'ChequeStatusKey', index: 'ChequeStatusKey', editable: true },
                { key: true, hidden: true, name: 'PaymentKey', index: 'PaymentKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, name: 'ReceiptNumber', index: 'ReceiptNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PaymentDate', index: 'PaymentDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/M/y' } },
                { key: false, name: 'PaymentModeName', index: 'PaymentModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PaidAmount', index: 'PaidAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Status', index: 'Status', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ClearedAmount', index: 'ClearedAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BalanceAmount', index: 'BalanceAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        $("#grid").jqGrid("setLabel", "EmployeeSalaryAdvanceName", "", "thEmployeeSalaryAdvanceName");
    }

    var editPopup = function (_this) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        var url = $(_this).attr("data-href");

        $.customPopupform.CustomPopup({
            load: function () {
                setTimeout(function () {
                    AppCommon.FormatInputCase();
                }, 500)
            },
            rebind: function (responce) {
                window.location.reload();
            }
        }, url);



    }

    var getEmployeesByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnFillEmployees").val(), ddl, Resources.Employee);
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.PaymentKey + "'";
        var printtemp = "'" + rowdata.BranchKey + "'" + ',' + "'" + rowdata.ReceiptNumber + "'" + ',' + "'" + Resources.InvoicePrintTypeOrderReceipt + "'";
        if ((rowdata.ClearedAmount != null && rowdata.ClearedAmount != 0) || (rowdata.ChequeStatusKey != Resources.ProcessStatusPending && rowdata.ChequeStatusKey != null)) {
            if (rowdata.ReceiptNumber != null && rowdata.ReceiptNumber != "") {
                return '<div class="divEditDelete"><a class="btn btn-outline-success btn-sm mx-1 btnPrint" onclick="PrintInvoice.PrintReceipt(null, ' + printtemp + ')"><i class="fa fa-print" aria-hidden="true"></i></a></div>';
            }
            else {
                return "";
            }
        }
        else {
            if (rowdata.ReceiptNumber != null && rowdata.ReceiptNumber != "") {
                return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" onclick="EmployeeSalaryAdvance.EditPopup(this)"  data-href="' + $("#hdnAddEditEmployeeSalaryAdvance").val() + '/' + rowdata.PaymentKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteEmployeeSalaryAdvance(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-success btn-sm mx-1 btnPrint" onclick = "PrintInvoice.PrintReceipt(null, ' + printtemp + ')" > <i class="fa fa-print" aria-hidden="true"></i></a></div>';
            }
            else {
                return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" onclick="EmployeeSalaryAdvance.EditPopup(this)"  data-href="' + $("#hdnAddEditEmployeeSalaryAdvance").val() + '/' + rowdata.PaymentKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteEmployeeSalaryAdvance(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
            }

        }
    }
    return {
        GetEmployeeSalaryAdvance: getEmployeeSalaryAdvance,
        EditPopup: editPopup,
        GetEmployeesByBranchId: getEmployeesByBranchId
    }
}());



function deleteEmployeeSalaryAdvance(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeSalaryAdvance,
        actionUrl: $("#hdnDeleteEmployeeSalaryAdvance").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function bindLocationForm(dialog) {
    $('#frmPaymentWindow', dialog).submit(function () {
        var validate = $('#frmPaymentWindow').valid();
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
                        bindLocationForm(dialog)
                    }
                    $(form).mLoading("destroy");
                }
            });

        }

        return false;

    });
}
