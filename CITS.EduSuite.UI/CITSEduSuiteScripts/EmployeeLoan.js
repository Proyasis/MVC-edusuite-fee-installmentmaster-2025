
var EmployeeLoan = (function () {

    var getEmployeeLoan = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetEmployeeLoanList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                employeeId: function () {
                    return $('#ddlEmployee').val()
                },
                branchId: function () {
                    return $('#ddlBranch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Employee, Resources.Amount, Resources.LoanDate, Resources.MonthlyRepaymentAmount, Resources.RepaymentStartDate, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, hidden: true, name: 'EmployeeKey', index: 'EmployeeKey', editable: true },
                { key: true, hidden: true, name: 'LoanStatusKey', index: 'LoanStatusKey', editable: true },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Amount', index: 'Amount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LoanDate', index: 'LoanDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'MonthlyRepaymentAmount', index: 'MonthlyRepaymentAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RepaymentStartDate', index: 'RepaymentStartDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'LoanStatusName', index: 'LoanStatusName', formatter: formatStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 200 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
              hidegrid: false,
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
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $(data.rows).each(function (i) {
                   
                    var _this = $("#grid tr[id='" + i + "'] a[data-modal='']");
                    if (data.rows.length == 1 && $("#grid tr").length > data.rows.length)
                    {
                        _this = $("#grid tr:last a[data-modal='']");
                    }
                    $("#grid a[data-payment-modal='']").each(function () {
                        AppCommon.PaymentPopupWindow($("a[data-payment-modal='']"), $("#hdnAddEditEmployeeLoanPayment").val(), Resources.Add + Resources.BlankSpace + Resources.Transactions + Resources.BlankSpace + Resources.Payment);
                    })
                    AppCommon.EditGridPopup($(_this));
                    var obj = {};
                    obj.BranchKey = this.BranchKey;
                    obj.EmployeeKey = this.EmployeeKey
                    EmployeeLoan.SetUrlParam($(_this), obj);
                })
            }
        })

        $("#grid").jqGrid("setLabel", "FullName", "", "thFullName");
    }
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        var html = '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" data-modal="" data-href="AddEditEmployeeLoan/' + rowdata.RowKey + '" ><i class="fa fa-pencil" aria-hidden="true"></i></a>'
            + '<a class="btn btn-outline-danger btn-sm" onclick="javascript:deleteEmployeeLoan(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>'
        if (rowdata.LoanStatusKey == Resources.ProcessStatusApproved) {
            html = html + '<a class="btn btn-warning btn-sm"   data-payment-modal="" data-href="' + $("#hdnAddEditEmployeeLoanPayment").val() + '/' + rowdata.RowKey + '" ><i class="fa fa-credit-card" aria-hidden="true"></i>' + Resources.Payment + '</a>'
        }
        html = html + "</div>"

        return html;
    }
    function formatStatus(cellValue, options, rowdata, action) {
        var html = "";
        switch (rowdata.LoanStatusKey) {
            case Resources.ProcessStatusApproved:
                html = '<span class="label label-success">' + cellValue + '</span>';
                break;
            case Resources.ProcessStatusRejected:
                html = '<span class="label label-danger">' + cellValue + '</span>';
                break;
            default:
                html = '<span class="label label-warning">' + cellValue + '</span>';

        }
        return html;
    }

    var paymentPopupWindow = function (Id) {

        var validator = null
        var url = $("#hdnAddEditEmployeeLoanPayment").val() + "/" + Id;

        $('#paymentModalContent').load(url, function () {
            var form = $(this).closest("form");
            $(form).removeData("validator");
            $(form).removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse($(form));
            $("#paymentModal").one('show.bs.modal', function () {

            }).modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');
        });
    }

    var getEmployeesByBranchId = function (Id, ddl) {
        $(ddl).html(""); // clear before appending new list 
        $(ddl).append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Employee));
        $(ddl).select2("val", "");
        if (Id != "") {
            var response = AjaxHelper.ajax("GET", $("#hdnGetEmployeesByBranchId").val() + "/" + Id);
            var $optgroup;
            $.each(response, function (i, Employee) {

                //if (response[i - 1] == undefined || Employee.GroupName != response[i - 1]["GroupName"]) {
                //    $optgroup = "";
                //    $optgroup = $("<optgroup>", {
                //        label: Employee.GroupName
                //    });

                //    $optgroup.appendTo(ddl);
                //}
                var $option = $("<option>", {
                    text: Employee.Text,
                    value: Employee.RowKey
                });
                $option.appendTo(ddl);

            });
        }
    }
    var setUrlParam = function (lnk, data) {
        var obj = {};
        obj.employeeKey = data.EmployeeKey ? data.EmployeeKey : 0;
        obj.branchKey = data.BranchKey ? data.BranchKey : 0;
        var url = $(lnk).data("href");
        url = url + "?" + $.param(obj);
        $(lnk).attr("data-href", url);
    }


    return {
        GetEmployeeLoan: getEmployeeLoan,
        SetUrlParam: setUrlParam,
        PaymentPopupWindow: paymentPopupWindow,
        GetEmployeesByBranchId: getEmployeesByBranchId
    }

}());

function deleteEmployeeLoan(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeLoan,
        actionUrl: $("#hdnDeleteEmployeeLoan").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function bindForm(dialog) {
    $('#frmEmployeeLoan', dialog).submit(function () {
        var validate = $('#frmEmployeeLoan').valid();
        if (validate) {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        $('#myModal').modal('hide');
                        // location.reload();
                        //alert("success")
                        var url = window.location.pathname.split("/");
                        var controller = url[2];
                        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');

                        //  Load data from the server and place the returned HTML into the matched element
                    } else {
                        $('#myModalContent').html(result);
                        bindForm(dialog);

                    }


                }
            });

        }
        return false;

    });
    AppCommon.FormatDateInput();
}
