var FieldValidation = (function () {
    var getFieldValidation = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetFieldValidation").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Name, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'FieldType', index: 'FieldType', editable: true },
                { key: false, name: 'FieldTypeName', index: 'FieldTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },               
                { name: 'edit', search: false, index: 'FieldType', sortable: false, formatter: editLink, resizable: false },
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


        $("#grid").jqGrid("setLabel", "FieldValidationName", "", "thFieldValidationName");
    }
   

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.FieldType + "'";
        return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnAddEditFieldValidation").val() + '/' + rowdata.FieldType + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>';
    }
    return {
        GetFieldValidation: getFieldValidation
    }
}());





