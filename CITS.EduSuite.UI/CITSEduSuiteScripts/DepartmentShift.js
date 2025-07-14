var DepartmentShift = (function ()
{

    var getDepartmentShifts = function ()
    {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetDepartmentShifts").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function ()
                {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Department, Resources.ShiftName, Resources.FromDate, Resources.ToDate, Resources.Action],

            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'DepartmentName', index: 'DepartmentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ShiftName', index: 'ShiftName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FromDate', index: 'FromDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'ToDate', index: 'ToDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
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
            loadComplete: function (data)
            {
                $("#grid a[data-modal='']").each(function ()
                {
                    AppCommon.EditGridPopup($(this))
                })

            }
        })

        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
    }

    function editLink(cellValue, options, rowdata, action)
    {
        var temp = "'" + rowdata.RowKey + "'";

        return '<button type="button" onclick="DepartmentShift.EditPopup(this)" class="btn btn-primary btn-sm mx-1" data-href="' + $("#hdnAddEditDepartmentShift").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></button><button type="button" class="btn btn-danger btn-sm mx-1" onclick="javascript:deleteDepartmentShift(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></button></div>';

    }
    var editPopup = function (_this) {
        var url = $(_this).attr("data-href");
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                setTimeout(function () {
                    DepartmentShiftPopupLoad();
                }, 500);
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, url);
    }

    return {
        GetDepartmentShifts: getDepartmentShifts,
        EditPopup: editPopup

    }
}());

function deleteDepartmentShift(rowkey)
{
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_DepartmentShift,
        actionUrl: $("#hdnDeleteDepartmentShift").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}



function DepartmentShiftPopupLoad() {
    AppCommon.FormatDateInput();
    AppCommon.FormatInputCase();
    AppCommon.FormatTimeInput();
    AppCommon.AutoHideDropdown();

}