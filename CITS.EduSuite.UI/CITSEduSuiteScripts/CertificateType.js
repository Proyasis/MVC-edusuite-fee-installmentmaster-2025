var CertificateType = (function () {
    var getCertificateType = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetCertificateType").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.CertificateType , Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'CertificateTypeName', index: 'CertificateTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
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
                    AppCommon.EditGridPopup($(this))
                })

            }
        })

        $("#grid").jqGrid("setLabel", "CertificateTypeName", "", "thCertificateTypeName");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditCertificateType").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm  mx-1" href="#"   onclick="javascript:deleteCertificateType(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }
    return {
        GetCertificateType: getCertificateType
    }
}());

function deleteCertificateType(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_CertificateType,
        actionUrl: $("#hdnDeleteCertificateType").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}






