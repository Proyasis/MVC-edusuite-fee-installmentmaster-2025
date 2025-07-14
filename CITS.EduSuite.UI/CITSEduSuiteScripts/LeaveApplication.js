
var LeaveApplication = (function () {

    var getLeaveApplication = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetLeaveApplicationsList").val(),
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
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Employee, Resources.LeaveDurationType, Resources.LeaveTypeName, Resources.LeaveReason, Resources.LeaveFrom, Resources.LeaveTo, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, hidden: true, name: 'EmployeeKey', index: 'EmployeeKey', editable: true },
                { key: true, hidden: true, name: 'LeaveStatusKey', index: 'LeaveStatusKey', editable: true },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LeaveDurationTypeName', index: 'LeaveDurationTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LeaveTypeName', index: 'LeaveTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LeaveReason', index: 'LeaveReason', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LeaveFrom', index: 'LeaveFrom', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'LeaveTo', index: 'LeaveTo', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'LeaveStatusName', index: 'LeaveStatusName', formatter: formatStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
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
                var Menus = {
                    A: { name: Resources.Approve, icon: "fa-check-circle" },
                    R: { name: Resources.Reject, icon: "fa-ban" },
                    E: { name: Resources.Edit, icon: "fa-pencil" },
                    D: { name: Resources.Delete, icon: "fa-trash" }

                }
                if (item.LeaveStatusKey == Resources.ProcessStatusApproved) {
                    delete Menus.A;
                    delete Menus.R;
                }
                else if (item.LeaveStatusKey == Resources.ProcessStatusRejected) {
                    delete Menus.A;
                    delete Menus.R;
                    delete Menus.E;
                }
                return {
                    callback: function (key, options) {

                        switch (key) {
                            case "A":
                                ApproveLeaveApplication(item.RowKey, Resources.ProcessStatusApproved);
                                break;
                            case "R":
                                ApproveLeaveApplication(item.RowKey, Resources.ProcessStatusRejected);
                                break;
                            case "E":
                                LeaveApplication.EditPopup("AddEditLeaveApplication/" + item.RowKey);
                                break;
                            case "D":
                                deleteLeaveApplication(item.RowKey);
                                ; break;
                            default:
                                break;

                        }
                    },
                    items: Menus
                }

            }
        });


        $("#grid").jqGrid("setLabel", "FullName", "", "thFullName");
    }



    function editLink(cellValue, options, rowdata, action) {
        //var temp = "'" + rowdata.RowKey + "'";
        //var html = '';
        //if (rowdata.LeaveStatusKey == Resources.ProcessStatusApproved) {
        //    html = html + '<button type="button"  class="btn  btn-primary btn-sm mx-1" onclick="LeaveApplication.EditPopup(this)" data-href="AddEditLeaveApplication/' + rowdata.RowKey + '" ><i class="fa fa-pencil" aria-hidden="true"></i></button>'
        //        + '<a class="btn btn-danger btn-sm" onclick="javascript:deleteLeaveApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a>';
        //}
        //else if (rowdata.LeaveStatusKey != Resources.ProcessStatusRejected) {
        //    html = html + '<button type="button"  class="btn  btn-primary btn-sm mx-1" onclick="LeaveApplication.EditPopup(this)" data-href="AddEditLeaveApplication/' + rowdata.RowKey + '" ><i class="fa fa-pencil" aria-hidden="true"></i></button>'
        //        + '<button type="button" class="btn btn-success btn-sm mx-1" onclick="javascript:ApproveLeaveApplication(' + rowdata.RowKey + ',' + Resources.ProcessStatusApproved + ');return false;" ><i class="fa fa-thumbs-o-up pointer" aria-hidden="true"></i></a>'
        //        + '<button type="button" class="btn btn-danger btn-sm mx-1" onclick="javascript:ApproveLeaveApplication(' + rowdata.RowKey + ',' + Resources.ProcessStatusRejected + ');return false;" ><i class="fa fa-thumbs-o-down pointer" aria-hidden="true"></i></a>';
        //}
        //html = html + '<a class="btn btn-danger btn-sm" onclick="javascript:deleteLeaveApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a>';




        //html = html + '</div">';
        //return html
        return '<button type="button" class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</button>'
    }
    function formatStatus(cellValue, options, rowdata, action) {
        var html = "";
        switch (rowdata.LeaveStatusKey) {
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

    var leaveDurationTypeChange = function (value) {

        if (parseInt(value) != Resources.LeaveDurationTypeMultipleDays) {
            $('#divLeaveTo').hide();
            $('#lblLeaveFrom').text('Leave Date');
            $("#LeaveTo").val("");

        } else {
            $('#divLeaveTo').show();
            $('#lblLeaveFrom').text('Leave From');
        }
    }

    var getEmployeesByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetEmployeesByBranchId").val(), ddl, Resources.Employee);

    }
    var editPopup = function (_this) {

        var obj = {};
        obj.branchKey = $("#BranchKey").val()
        obj.employeeKey = $("#EmployeeKey").val()
        if (!obj.branchKey) {
            delete obj.branchKey;
        }
        if (!obj.employeeKey) {
            delete obj.employeeKey;
        }
        var url = _this;
        if (typeof _this == "object") {
            url = $(_this).attr("data-href");
        }
        url = url + "?" + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                setTimeout(function () {
                    LeaveApplicationPopupLoad()
                }, 500);
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, url);
    }

    return {
        GetLeaveApplication: getLeaveApplication,
        LeaveDurationTypeChange: leaveDurationTypeChange,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        EditPopup: editPopup
    }

}());

function deleteLeaveApplication(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_LeaveApplication,
        actionUrl: $("#hdnDeleteLeaveApplication").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function ApproveLeaveApplication(RowKey, LeaveStatusKey) {
    if (LeaveStatusKey == Resources.ProcessStatusApproved) {
        var result = EduSuite.ConfirmWithMultipleParam({
            title: Resources.Confirmation,

            content: Resources.Approve_Confirm_LeaveApplication,
            actionUrl: $("#hdnApproveLeaveApplication").val(),
            param: { RowKey: RowKey, LeaveStatusKey: LeaveStatusKey },
            dataRefresh: function () {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        });
    }
    else {
        EduSuite.ConfirmWithMultipleParam({
            title: Resources.Confirmation,
            content: Resources.Reject_Confirm_LeaveApplication,
            actionUrl: $("#hdnApproveLeaveApplication").val(),
            param: { RowKey: RowKey, LeaveStatusKey: LeaveStatusKey },
            dataRefresh: function () {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        });
    }

}
function LeaveApplicationPopupLoad() {
    AppCommon.AutoHideDropdown();
    AppCommon.FormatDateInput();

    $("#BranchKey").on("change", function () {
        LeaveApplication.GetEmployeesByBranchId($(this).val(), $("#EmployeeKey"));
    });
    LeaveApplication.LeaveDurationTypeChange($("#LeaveDurationTypeKey").val());
    $('#LeaveDurationTypeKey').change(function () {
        LeaveApplication.LeaveDurationTypeChange($(this).val());
    });
}
