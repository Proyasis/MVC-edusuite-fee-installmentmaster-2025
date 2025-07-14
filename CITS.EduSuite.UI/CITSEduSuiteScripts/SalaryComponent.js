var SalaryComponent = (function () {

    var getSalaryComponentByType = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetSalaryComponentsByType").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                ComponentTypeId: function () {
                    return $("#tab-componets li.active a").attr('data-val');
                },
                employeeId: function () {
                    return $("#ddlEmployee").val();
                },
                branchId: function () {
                    return $('#ddlBranch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Employee, Resources.Date, Resources.Amount, Resources.Remarks, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, hidden: true, name: 'EmployeeKey', index: 'EmployeeKey', editable: true },
                { key: true, hidden: true, name: 'ApprovalStatusKey', index: 'ApprovalStatusKey', editable: true },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SalaryComponentDate', index: 'ComponentDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'AmountUnit', index: 'AmountUnit', editable: true, cellEdit: true, sortable: true, resizable: false },
                 { key: false, name: 'Remarks', index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApprovalStatusName', index: 'ApprovalStatusName',formatter:formatStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
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
                    if (data.rows.length == 1 && $("#grid tr").length > data.rows.length) {
                        _this = $("#grid tr:last a[data-modal='']");
                    }
                    AppCommon.EditGridPopup($(_this));
                    var obj = {};
                    obj.ComponentTypeKey = this.SalaryComponentTypeKey;
                    obj.BranchKey = this.BranchKey;
                    obj.EmployeeKey = this.EmployeeKey
                    SalaryComponent.SetUrlParam($(_this), obj);
                })
            }
        })
       

        $("#grid").jqGrid("setLabel", "ComponentDate", "", "thComponentDate");
    }

   
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" data-modal=""  data-href="AddEditSalaryComponent/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" onclick="javascript:deleteSalaryComponent(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';
    }
    function formatStatus(cellValue, options, rowdata, action) {
        var html = "";
        switch (rowdata.ApprovalStatusKey) {
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
        obj.componentTypeKey = data.ComponentTypeKey ? data.ComponentTypeKey : 0;
        obj.employeeKey = data.EmployeeKey ? data.EmployeeKey : 0;
        obj.branchKey = data.BranchKey ? data.BranchKey : 0;
        var url = $(lnk).data("href");
        url = url + "?" + $.param(obj);
        $(lnk).attr("data-href", url);
    }
    return {
        GetSalaryComponentByType: getSalaryComponentByType,
        SetUrlParam: setUrlParam,
        GetEmployeesByBranchId: getEmployeesByBranchId
    }

}());

function deleteSalaryComponent(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Employee,
        actionUrl: $("#hdnDeleteSalaryComponent").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}
