var GSTMaster = (function () {
    var getGSTMaster = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetGSTMaster").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Name, Resources.HSNCode, Resources.CGSTPer, Resources.SGSTPer, Resources.IGSTPer, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'Name', index: 'Name', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'HSNCode', index: 'HSNCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CGSTRate', index: 'CGSTRate', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SGSTRate', index: 'SGSTRate', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IGSTRate', index: 'IGSTRate', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActive', index: 'IsActive', editable: true, cellEdit: true, sortable: true, resizable: false },
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
                    AppCommon.EditGridPopup($(this))
                })

            }
        })


        $("#grid").jqGrid("setLabel", "GSTMasterName", "", "thGSTMasterName");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditGSTMaster").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteGSTMaster(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    var calculateIGST = (function () {
        var CGSTRate = $("#CGSTRate").val();
        var SGSTRate = $("#SGSTRate").val();
        CGSTRate = parseInt(CGSTRate) ? parseInt(CGSTRate) : 0;
        SGSTRate = parseInt(SGSTRate) ? parseInt(SGSTRate) : 0;
        var IGSTRate = CGSTRate + SGSTRate;
        $("#IGSTRate").val(IGSTRate)
    })
    return {
        GetGSTMaster: getGSTMaster,
        CalculateIGST: calculateIGST
    }
}());

function deleteGSTMaster(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_GSTMaster,
        actionUrl: $("#hdnDeleteGSTMaster").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}




