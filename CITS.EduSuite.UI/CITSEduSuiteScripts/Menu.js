var Menu = (function () {
    var getMenu = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetMenuDetailView").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#searchText').val()
                },
                MenuTypeKey: function () {
                    return $('#MenuTypeKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.MenuType, Resources.MenuCode, Resources.MenuName, Resources.ActionName, Resources.ControllerName, Resources.DisplayOrder, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'MenuTypeName', index: 'MenuTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MenuCode', index: 'MenuCode', width: 80, editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MenuName', index: 'MenuName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ActionName', index: 'ActionName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ControllerName', index: 'ControllerName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DisplayOrder', index: 'DisplayOrder', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        })


        $("#grid").jqGrid("setLabel", "AgentName", "", "thAgentName");
    }

    function IsActive(cellValue, options, rowdata, action) {
        if (cellValue == true) {
            return 'Yes'
        }
        else {
            return 'No'
        }
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";

        //return '<a data-href="' + $("#hdnAddEditMenuDetailView").attr("data-href") + '" onclick="Menu.EditMenuDetailViewPopup(this,' + rowdata.RowKey + ');" class="badge badge-pill badge-secondary ml-2 text-white CustomBtnEdit"> <i class="fas fa-pencil-alt" aria-hidden="true"></i> ' + Resources.Edit + '</a>'
        //    + ' <a  onclick="deleteMenuDetailView(' + rowdata.RowKey + ')" class="badge badge-pill badge-danger   ml-2 text-white  CustomBtnDelete"> <i class="fas fa-trash-alt"></i> ' + Resources.Delete + '</a> '

        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" onclick="Menu.EditMenuDetailViewPopup(' + rowdata.RowKey + ')" ><i class="fa fa-pencil" aria-hidden="true"></i></a><a  class="btn btn-outline-danger btn-sm mx-1" href="#"  onclick="javascript:deleteMenu(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';


    }

    var editMenuDetailViewPopup = function (rowkey) {

        var obj = {};
        obj.id = rowkey;

        //var url = $(_this).attr("data-href");

        var url = $("#hdnAddEditMenuDetailView").val() + '?' + $.param(obj);

        $.customPopupform.CustomPopup({
            modalsize: "modal-lg ",
            load: function () {
                Menu.GetMenuAction(jsonData);

            },
            rebind: function (result) {

            }
        }, url);
    }

    var getMenuAction = function (json) {

        $('#rptRepeater').repeater(
            {
                show: function () {
                    $(this).slideDown();

                    AppCommon.FormatDateInput();
                    AppCommon.CustomRepeaterRemoteMethod();
                    AppCommon.FormatInputCase();
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteMenuAction($(hidden).val(), this, remove);
                    }
                    else {
                        $(this).slideUp(remove);
                    }
                },
                rebind: function (response) {
                    if (typeof response == "string") {

                    }
                    else if (response.IsSuccessful) {

                    }

                },
                data: json,
                repeatlist: 'MenuActionsList',
                submitButton: "",
                hasFile: false
            });
    }

    var formSubmit = function () {
        var $form = $("#frmMenuDetailViewAddEdit")

        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
          
        if ($form.valid()) {
           
            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
            if (typeof response == "string") {
                $("[data-valmsg-for=error_msg]").html(response);
            }
            else if (response.IsSuccessful)
            {
                $.alert({
                    type: 'green',
                    title: Resources.Success,
                    content: response.Message,
                    icon: 'fa fa-check-circle-o-',
                    buttons: {
                        Ok: {
                            text: Resources.Ok,
                            btnClass: 'btn-success',
                            action: function () {
                                
                                $('[data-dismiss="modal"]').trigger('click');                                
                                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                            }
                        }
                    }
                })
            }
        }
    }

    return {
        GetMenu: getMenu,
        EditMenuDetailViewPopup: editMenuDetailViewPopup,
        GetMenuAction: getMenuAction,
        FormSubmit: formSubmit
    }
}());

function deleteMenu(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Agent,
        actionUrl: $("#hdnDeleteMenuDetailView").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteMenuAction(rowkey, _this, _remove) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Agent,
        actionUrl: $("#hdnDeleteMenuAction").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $(_this).slideUp(_remove);
           
        }
    });
}




