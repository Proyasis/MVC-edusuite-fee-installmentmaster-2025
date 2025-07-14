
var Designation = (function () {

    var getDesignation = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetDesignationList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()

                }
            },
            colNames: [Resources.RowKey, Resources.Designation, Resources.HigherDesignation, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'DesignationName', index: 'DesignationName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'HigherDesignationName', index: 'HigherDesignationName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
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

        $("#grid").jqGrid("setLabel", "DesignationName", "", "thDesignationName");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" data-modal="" data-href="' + $("#hdnAddEditDesignation").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>'
            + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteDesignation(' + temp + ');return false;" ><i class="fa fa-trash pointer" aria-hidden="true"></i></a>'
            //+ '<a class="btn btn-warning btn-sm mx-1" href="' + $("#hdnAddEditDesignationPermission").val() + '/' + rowdata.RowKey + '"><i class="fa fa-cogs" aria-hidden="true"></i></a>'
    }

    var getDesignationPermissionById = function () {
        var response = AjaxHelper.ajax("Get", $("#hdnGetDesingationPermissionById").val(),
                             {

                             });
        $("#dvDesignationPermission").bind("loaded.jstree", function (event, data) {
            data.instance.open_all();
        }).jstree({
            core: {
                data: response,
                "themes": {
                    "icons": false,
                    "dots": false
                },
                "dblclick_toggle": false,
            },

            "plugins": ["checkbox"]

        });


    }



    var formPermissionSubmit = function (btn, jsonData) {
        var permissionList = $('#dvDesignationPermission').jstree(true).get_json('#', { flat: true })

        var data = $.extend(true, {}, jsonData || {});

        $(permissionList).each(function () {
            var permission = this.data;
            var permissionState = this.state;

            if (permission && permission.mid) {
                dataItem = {};
                dataItem.RowKey = permission.key;
                dataItem.MenuKey = permission.mid;
                dataItem.ActionKey = permission.aid
                dataItem.IsActive = permissionState.selected;
                data["DesignationPermissions"].push(dataItem);
            }
        })

        var form = $(btn).closest("form");
        var validate = $(form).valid();
        if (validate) {

            response = AjaxHelper.ajax($(form)[0].method, $(form).attr("action"),
                     {
                         model: data
                     });
            if (response.IsSuccessful == true) {
                toastr.success(Resources.Success, response.Message);
            }
        }
    }
    return {
        GetDesignation:getDesignation,
        GetDesignationPermissionById: getDesignationPermissionById,
        FormPermissionSubmit: formPermissionSubmit
    }
}());

function deleteDesignation(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Division,
        actionUrl: $("#hdnDeleteDesignation").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}


