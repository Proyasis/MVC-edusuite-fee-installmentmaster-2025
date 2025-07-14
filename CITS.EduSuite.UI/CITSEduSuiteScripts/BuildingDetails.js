
var BuildingDetails = (function () {
    var getBuildingDetails = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBuildingDetails").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Name, Resources.RoomCount, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BuildingMasterName', index: 'BuildingMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RoomCount', index: 'RoomCount', editable: true, cellEdit: true, sortable: true, resizable: false },
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
                });

            }
        })

        $("#grid").jqGrid("setLabel", "UniversityCourseName", "", "thUniversityCourseName");
    }


    var formSubmit = function (btn) {

        var form = $(btn).closest("form")[0];
        var url = $(form)[0].action;
        if ($(form).valid()) {
            $(".section-content").mLoading();
            var data = $(form).serializeArray();
            delete data[0];
            setTimeout(function () {
                var response = AjaxHelper.ajax("POST", url,
                                  {
                                      model: AppCommon.ObjectifyForm(data)
                                  });

                if (response.IsSuccessful == true) {
                    toastr.success(Resources.Success, response.Message);

                    window.location.href = $("#hdnBuildingDetailsList").val();

                    //$("#tab-application li").each(function () {
                    //    var url = $(this).find("a").data("href");
                    //    $(this).find("a").attr("data-href", url.replace('0', response.RowKey))
                    //    $(this).show();
                    //})
                    //if (!response.HasInstallment) {
                    //    $("#tab-application li a[href='#FeeInstallment']").parent().hide();
                    //}
                    //if (!response.HasElective) {
                    //    $("#tab-application li a[href='#ElectiveBook']").parent().hide();
                    //}
                    //$("#tab-application  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');

                }
                else {
                    $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                }
                //$(".section-content").mLoading("destroy");
            }, 500)
        }

    }

    var loadData = function () {
        var data = AppCommon.ObjectifyForm($("form").serializeArray());
        //var response = AjaxHelper.ajax("POST", $("#hdnGetRoomDetails").val(),{model:data});
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: $("#hdnGetRoomDetails").val(),
            data: JSON.stringify({ model: data }),
            success: function (result) {
                $("#dvStudentsList").html(result);
                AppCommon.FormatInputCase();
                setTimeout(function () {
                    $("[data-repeater-list]").find("input,select,textarea").each(function () {
                        $(this).attr("name", "BuildingDetails" + $(this).attr("name"));
                        $(this).attr("id", $(this).attr("name"));
                    });
                }, 500)
            },
            error: function (request, status, error) {

            }

        });
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnAddEditBuildingDetails").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteBuildingDetailsAll(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    var deleteBuildingDetails = function (_this, rowkey, BuildingMasterKey) {
        var obj = {};
        obj.RowKey = rowkey;
        obj.RowKey = parseInt(obj.RowKey) ? parseInt(obj.RowKey) : 0;
        if (obj.RowKey == 0) {
            var item = $(_this).closest("[data-repeater-item]");
            $(item).slideUp(function () {
                $(this).remove();
                $("#RoomCount").val($("#dvStudentsList [data-repeater-item]").length)
            });
        }
        else {
            obj.BuildingMasterKey = BuildingMasterKey;
            var result = EduSuite.Confirm2({
                title: Resources.Confirmation,
                content: Resources.Delete_Confirm_Enquiry,
                actionUrl: $("#hdnDeleteBuildingDetails").val(),
                parameters: obj,
                //actionValue: rowkey,
                dataRefresh: function () {
                    $("#dvStudentsList").load($("#hdnGetRoomDetails").val() + "/" + $("#RowKey").val(), function () {
                        $("#RoomCount").val($("#dvStudentsList [data-repeater-item]").length)
                    })

                }
            });
        }
    }
    return {
        GetBuildingDetails: getBuildingDetails,
        FormSubmit: formSubmit,
        LoadData: loadData,
        DeleteBuildingDetails: deleteBuildingDetails
    }
}());

function deleteBuildingDetailsAll(RowKey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_UniversityCourse,
        actionUrl: $("#hdnDeleteBuildingDetailsAll").val(),
        actionValue: RowKey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

//function deleteBuildingDetails(rowkey, _this) {
//    var result = EduSuite.Confirm({
//        title: Resources.Confirmation,
//        content: Resources.Delete_Confirm_Book,
//        actionUrl: $("#hdnDeleteBuildingDetails").val(),
//        actionValue: rowkey,
//        dataRefresh: function () {
//            window.location.reload();
//        }
//    });
//}

