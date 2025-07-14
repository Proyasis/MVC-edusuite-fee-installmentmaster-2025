
var MemberTypes = (function () {

    var getMemberTypes = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetMemberTypesList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.MemberType + Resources.BlankSpace + Resources.Code,Resources.MemberType + Resources.BlankSpace + Resources.Name, Resources.NumberOfBooksAllowed, Resources.RegistrationFee, Resources.MembershipFee, Resources.LateFeePerDay, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'MemberTypeCode', index: 'MemberTypeCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MemberTypeName', index: 'MemberTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NumberOfBooksAllowed', index: 'NumberOfBooksAllowed', align: "center", editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RegistrationFee', index: 'RegistrationFee', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MemberShipFee', index: 'MemberShipFee', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LateFeePerDay', index: 'LateFeePerDay', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        $("#grid").jqGrid("setLabel", "MemberTypeName", "", "thMemberTypeName");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" data-modal=""  data-href="AddEditMemberType/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteMemberType(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';
    }

    return {
        GetMemberTypes: getMemberTypes
    }

}());

function deleteMemberType(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_MemberType,
        actionUrl: $("#hdnDeleteMemberType").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}