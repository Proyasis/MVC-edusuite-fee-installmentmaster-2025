var EmployeeShift = (function () {

    var getEmployeeShifts = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetEmployeeShifts").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.ShiftName, Resources.FromDate, Resources.ToDate, Resources.Employee + Resources.BlankSpace + Resources.Count, Resources.Action],

            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'ShiftName', index: 'ShiftName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FromDate', index: 'FromDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'ToDate', index: 'ToDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'EmployeeCount', index: 'EmployeeCount', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        return '<button type="button" onclick="EmployeeShift.EditPopup(this)" class="btn btn-primary btn-sm mx-1" data-href="' + $("#hdnAddEditEmployeeShift").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></button><button type="button" class="btn btn-danger btn-sm mx-1" onclick="javascript:deleteEmployeeShift(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></button>';

    }
    var editPopup = function (_this) {
        var url = $(_this).attr("data-href");
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                setTimeout(function () {
                    EmployeeShiftPopupLoad();
                }, 500);
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, url);
    }

    var checkboxAllEmployee = function (chk) {
        if (chk) {
            $("[data-repeater-item]").each(function (index) {

                var Object = $('input[name*=IsActive]:checkbox', $(this))[0];
                if (Object) {
                    if (!Object.disabled) {
                        Object.checked = chk.checked;

                    }
                }
            });
        }
        var CheckedCount = $("input[type=checkbox][name*=IsActive]:checked").length;
        var CheckboxCount = $("input[type=checkbox][name*=IsActive]").length;
        var CheckedAll = false;
        if (parseInt(CheckboxCount) == parseInt(CheckedCount)) {
            CheckedAll = true;
        }
        $("#RecordCount").val(0);
        $("#spnRecordCount").html("");
        if (CheckedCount) {
            //$("#CheckedAllCount").html("(" + CheckedCount + ")");
            $("#RecordCount").val(CheckedCount);
            $("#spnRecordCount").html(CheckedCount);
            if (CheckedAll) {
                $("#chekAllIsActive").prop("checked", true);
            }
            else {
                $("#chekAllIsActive").prop("checked", false);
            }
        }
    }
    return {
        GetEmployeeShifts: getEmployeeShifts,
        EditPopup: editPopup,
        CheckboxAllEmployee: checkboxAllEmployee

    }
}());

function deleteEmployeeShift(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeShift,
        actionUrl: $("#hdnDeleteEmployeeShift").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}

function deleteEmployeeShiftDetails(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeShift,
        actionUrl: $("#hdnDeleteEmployeeShiftDetails").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");

            if (response.IsSuccessful) {
                $("[id*=IsActive]", item).removeProp("disabled")
                $("[id*=IsActive]", item).prop("checked", false);
                $("[id*=RowKey]", item).val(0)
                $(_this).remove()

                var RecordCount = $("#RecordCount").val();
                RecordCount = parseInt(RecordCount) - 1;
                $("#RecordCount").val(RecordCount)
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }
    });
}


function EmployeeShiftPopupLoad() {
    AppCommon.FormatDateInput();
    AppCommon.FormatInputCase();
    AppCommon.AutoHideDropdown();

    AppCommon.CustomRepeaterRemoteMethod();
    EmployeeShift.CheckboxAllEmployee();

    $("[id*=IsActive]").change(function () {
        EmployeeShift.CheckboxAllEmployee();
    });



}

