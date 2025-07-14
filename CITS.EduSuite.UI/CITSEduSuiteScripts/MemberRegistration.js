
var MemberRegistration = (function () {

    var getMemberRegistration = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetMemberRegistrationList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Name, Resources.PhoneNo, Resources.EmailAddress, Resources.MemberType, Resources.BorrowerType, Resources.IsBlockMember, Resources.CardId, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'MemberFullName', index: 'MemberFullName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PhoneNo', index: 'PhoneNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmailAddress', index: 'EmailAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MemberTypeName', index: 'MemberTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BorrowerTypeName', index: 'BorrowerTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsBlockMember', index: 'IsBlockMember', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CardId', index: 'CardId', editable: true, cellEdit: true, sortable: true, resizable: false },

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
            altclass: 'jqgrid-altrow'
        })

        $("#grid").jqGrid("setLabel", "MemberFirstName", "", "thMemberFirstName");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm mx-1" href="AddEditMemberRegistration/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteMemberRegistration(' + temp + ');return false;" ><i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';
    }



    return {
        GetMemberRegistration: getMemberRegistration
    }

}());

function deleteMemberRegistration(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_MemberRegistration,
        actionUrl: $("#hdnDeleteMemberRegistration").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}