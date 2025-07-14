
var Roles = (function () {

    var getRoles = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetRolesList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Role + Resources.BlankSpace + Resources.Name, Resources.Role + Resources.BlankSpace + Resources.Name + Resources.BlankSpace + Resources.Local, Resources.Status, Resources.NumberOfPeople, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'RoleName', index: 'RoleName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RoleNameLocal', index: 'RoleNameLocal', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StatusName', index: 'StatusName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfPeople', index: 'NoOfPeople', sortable: true, formatter: addHypLink, width: 50, resizable: false },
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

        $("#grid").jqGrid("setLabel", "RoleName", "", "thRoleName");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="AddEditRole/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-outline-danger btn-sm" onclick="javascript:deleteRole(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';
    }

    function addHypLink(cellValue, options, rowdata, action) {
        if (rowdata.NoOfPeople > 0)
            return '<div class="textCenter hyperLink"><a href="' + $("#hdnPeople").val() + '/' + rowdata.RowKey + '" >' + rowdata.NoOfPeople + '</a></div>';
        else
            return '<div class="textCenter">' + rowdata.NoOfPeople + '</div>';
    }

    return {
        GetRoles: getRoles
    }

}());

function deleteRole(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Role,
        actionUrl: $("#hdnDeleteRole").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}