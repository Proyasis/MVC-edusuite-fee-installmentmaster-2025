var Rack = (function () {

    var getRack = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetRackList").val(),
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
            //colNames: [Resources.RowKey, Resources.Rack + Resources.BlankSpace + Resources.Name, Resources.Rack + Resources.BlankSpace + Resources.Name + Resources.BlankSpace + Resources.Local, Resources.Remarks, Resources.BlankSpace, Resources.Status, Resources.Action],
            colNames: [Resources.RowKey, Resources.Wardrobe + Resources.BlankSpace + Resources.Name, Resources.Wardrobe + Resources.BlankSpace + Resources.Code, Resources.Remarks, Resources.Rack, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'MasterRowKey', index: 'MasterRowKey', editable: true },
                { key: false, name: 'RackName', index: 'RackName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RackCode', index: 'RackCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Remarks', index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SubRackCount', index: 'SubRackCount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'MasterRowKey', sortable: false, formatter: editLink, resizable: false },
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
                    AppCommon.EditGridPopup($(this), RackPopupLoad)
                });

            }
        })

        $("#grid").jqGrid("setLabel", "RackName", "", "thRackName");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.MasterRowKey + "'";
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" href="AddEditRack/' + rowdata.MasterRowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteRack(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';

        return '<div class="divEditDelete"><a data-modal="" class="btn btn-primary btn-sm mx-1" data-href="' + $("#hdnAddEditRack").val() + '/' + rowdata.MasterRowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-danger btn-sm mx-1" href="#"   onclick="javascript:deleteRack(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    var getSubRackDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();

                $("[id*=RowKey]", $(this)).val("0")
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteSubRack($(hidden).val(), $(this));
                }
                else {
                    $(this).slideUp(remove);
                }

            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    $("#frmRackDetails").closest(".modal").modal("hide")
                    $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                }

            },
            data: json,
            repeatlist: 'SubRackDetailsModel',
            submitButton: '',
        });
    }

    function formSubmit() {


        var $form = $("#frmRackDetails")
        var JsonData = [];

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
            else if (response.IsSuccessful) {
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
                                //$("#frmRackDetails").closest(".modal").modal("hide")
                                //$("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                                window.location.href = $("#hdnRackList").val();
                            }
                        }
                    }
                })

            }

        }
    }

    return {
        GetRack: getRack,
        GetSubRackDetails: getSubRackDetails,
        FormSubmit: formSubmit
    }

}());

function deleteRack(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Rack,
        actionUrl: $("#hdnDeleteRack").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}
function deleteSubRack(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Rack,
        actionUrl: $("#hdnDeleteSubRack").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");
            if (response.IsSuccessful)
                $(item).remove();
        }
    });
}


function RackPopupLoad() {
    $("form").removeData("validator");
    $("form").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("form");
    AppCommon.FormatInputCase()
    AppCommon.CustomRepeaterRemoteMethod();
    Rack.GetSubRackDetails();

}