var TransactionType = (function () {

    var getTransactionType = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetTransactionTypeList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.TransactionType + Resources.BlankSpace + Resources.Name, Resources.TransactionType + Resources.BlankSpace + Resources.Name + Resources.BlankSpace + Resources.Local, Resources.Status, Resources.Action],
            colModel: [
            { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'TransactionTypeName', index: 'TransactionTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'TransactionTypeNameLocal', index: 'TransactionTypeNameLocal', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StatusName', index: 'StatusName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editTransactionType, resizable: false },
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
            altclass: 'jqgrid-altrow'
        })

        $("#grid").jqGrid("setLabel", "TransactionTypeName", "", "TransactionTypeName");
        $("#grid").jqGrid("setLabel", "TransactionTypeNameLocal", "", "TransactionTypeNameLocal");
    }

    function editTransactionType(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><div class="textCenter inlineBlock divEdit"><a   href="AddEditTransactionType/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a></div>' + '<div class="textCenter inlineBlock divDelete"><a onclick="javascript:deleteTransactionType(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';
    }

    return {
        GetTransactionTypes: getTransactionType
    }

}());

function deleteTransactionType(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_TransactionType,
        actionUrl: $("#hdnDeleteTransactionType").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}
