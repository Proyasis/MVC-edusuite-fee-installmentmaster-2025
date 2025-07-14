var Province = (function () {

    var getProvince = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetProvinceList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()

                }
            },
            colNames: [Resources.RowKey, Resources.Province + Resources.BlankSpace + Resources.Name, Resources.Province + Resources.BlankSpace + Resources.Local + Resources.BlankSpace + Resources.Name, Resources.Country + Resources.BlankSpace + Resources.Name, Resources.Language + Resources.BlankSpace + Resources.Name, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'Provincename', index: 'Provincename', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ProvincenameLocal', index: 'ProvincenameLocal', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CountryName', index: 'CountryName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LanguageName', index: 'LanguageName', editable: true, cellEdit: true, sortable: true, resizable: false },
                // { key: false, name: 'DisplayOrder', index: 'DisplayOrder', editable: true, cellEdit: true, sortable: true, resizable: false },
                 { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: true, resizable: false },
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
                    AppCommon.EditGridPopup($(this));
                })
            }
        })

        $("#grid").jqGrid("setLabel", "ProvinceName", "", "thProvinceName");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-primary btn-sm" data-modal="" data-href="' + $("#hdnAddEditProvince").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-sm" onclick="javascript:deleteProvince(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a>';
    }
  
    return {
        GetProvince: getProvince
    }
}());

function deleteProvince(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Province,
        actionUrl: $("#hdnDeleteProvince").val(),
        actionValue: rowkey,
        dataRefresh: function () {$("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');}
    });
}
