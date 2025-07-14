var AttendanceConfiguration = (function () {

    var getAttendanceConfigurations = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAttendanceConfigurations").val(),
            datatype: 'json',
            mtype: 'Get',
            //colNames: [Resources.RowKey, Resources.AccountNumber, Resources.Bank, Resources.Branch, Resources.IFSC, Resources.MICR, Resources.NameInAccount, Resources.AccountType, Resources.OpeningBalance, Resources.CurrentBalance, Resources.IsActive, Resources.Action],
            colNames: [Resources.RowKey, Resources.Branch, Resources.ConfigurationType, Resources.TotalWorkingHours, Resources.AutoApproval, Resources.Action],

            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AttendanceConfigTypeName', index: 'AttendanceConfigTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TotalWorkingHours', index: 'TotalWorkingHours', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AutoApproval', index: 'AutoApproval', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
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
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                })

            }
        })

        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        var html = '<a data-modal="" class="btn btn-primary btn-sm mx-1" data-href="' + $("#hdnAddEditAttendanceConfiguration").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>';

        if (rowdata.RowKey != 1) {
            html = html + '<a class="btn btn-danger btn-sm mx-1" onclick="javascript:deleteAttendanceConfiguration(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
        }
        return html;
    }
    var hideShowShiftByConfigType = function (value) {
        if (value == Resources.AttendanceConfigTypeMarkPresent) {
            $("#dvShift").hide();
            $("#ShiftKey").val("");
        }
        else {
            $("#dvShift").show();
        }

    }

    return {
        GetAttendanceConfigurations: getAttendanceConfigurations,
        HideShowShiftByConfigType: hideShowShiftByConfigType

    }
}());

function deleteAttendanceConfiguration(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_AttendanceConfiguration,
        actionUrl: $("#hdnDeleteAttendanceConfiguration").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}



