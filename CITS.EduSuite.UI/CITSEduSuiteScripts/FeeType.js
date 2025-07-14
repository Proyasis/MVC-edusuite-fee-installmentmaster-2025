var FeeType = (function () {
    var getFeeType = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetFeeType").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.FeeType,
                //Resources.CashFlowType, 
                Resources.FeeTypeModeName,
                //Resources.IsTax, 
                Resources.IsUniversity,
                Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'FeeTypeName', index: 'FeeTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'CashFlowTypeName', index: 'CashFlowTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FeeTypeModeName', index: 'FeeTypeModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'AccountGroupName', index: 'AccountGroupName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'IsTaxName', index: 'IsTaxName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsUniverisityName', index: 'IsUniverisityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: true, resizable: false },
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
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this), FeeTypePopupLoad)
                });

            }
        })

        $("#grid").jqGrid("setLabel", "FeeTypeName", "", "thFeeTypeName");
    }

    var checkIsTax = function (_this) {

        if (_this.checked) {
            $("#DivHSNCode").show();
        }
        else {
            $("#DivHSNCode").hide();
            $("input,select", $("#DivHSNCode")).val("");
        }
    }
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditFeeType").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteFeeType(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }
    return {
        GetFeeType: getFeeType,
        CheckIsTax: checkIsTax
    }
}());

function deleteFeeType(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_FeeType,
        actionUrl: $("#hdnDeleteFeeType").val(),
        actionValue: rowkey,
        //{ id: rowkey },
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function FeeTypePopupLoad() {
    AppCommon.FormatInputCase()

    FeeType.CheckIsTax($("#IsTax")[0]);

    $("#IsTax").change(function () {
        FeeType.CheckIsTax($("#IsTax")[0]);
    });

    $("#AccountGroupKey").on("change", function () {
        var obj = {};
        obj.key = $(this).val();
        AppCommon.BindDropDownbyId(obj, $("#hdnFillAccountHeadType").val(), $("#AccountHeadTypeKey"), Resources.AccountHeadType)

      
    });

}




