var AttendanceTypeMaster = (function () {
    var getAttendanceTypeMaster = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAttendanceTypeMasterDetails").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.FromDate, Resources.ToDate, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'FromDate', index: 'FromDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'ToDate', index: 'ToDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },

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
            //loadComplete: function (data) {
            //    $("#grid a[data-modal='']").each(function () {
            //        AppCommon.EditGridPopup($(this))
            //    });
            //}
        })

        $("#grid").jqGrid("setLabel", "UniversityCourseName", "", "thUniversityCourseName");
    }
    var getAttendanceTypeDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
                //$("[id*=OriginalIssuedDate]", $(this)).val(moment(new Date()).format("DD/MM/YYYY"))
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                AppCommon.FormatTimeInput();
                $("[id*=RowKey][type=hidden]", $(this)).val(0);
                $("[id*=AttendanceTypeMasterKey ][type=hidden]", $(this)).val(0);
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteAttendanceTypeDetails($(hidden).val(), $(this));
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
                    if ($("#dvInstallmentDetails")[0]) {
                        //WorkSchedule.BindWorkScheduleDetails();
                        //WorkSchedule.AddEditWorkSchedule(null, response.EmployeeKey);
                    }
                    $(".modal").modal("hide");
                }

            },
            data: json,
            repeatlist: 'AttendanceTypeDetailsModel',
            submitButton: '',
            //defaultValues: json,
        });
    }

    var formSubmit = function (btn) {

        var $form = $("#frmAttendanceTypeMaster")
        var JsonData = [];
        var StartTime = ["StartTime"]
        var EndTime = ["EndTime"]
        $("[disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {

            formdData = formData["AttendanceTypeDetailsModel"].map(function (item) {
                item.StartTime = item.StartTime != "" ? moment(item.StartTime.toUpperCase(), ["hh:mm A"]).format("HH:mm") : null;
                item.EndTime = item.EndTime != "" ? moment(item.EndTime.toUpperCase(), ["hh:mm A"]).format("HH:mm") : null;
                return item;
            })
            //formData['StartTime'] = (formData['StartTime'] != "" ? moment(formData['StartTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
            //formData['EndTime'] = (formData['EndTime'] != "" ? moment(formData['EndTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
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
                                window.location.reload();
                            }
                        }
                    }
                })

            }

        }

    }
        
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-primary btn-sm mx-1" data-href="' + $("#hdnAddEditAttendanceTypeMaster").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-danger btn-sm mx-1" href="#"   onclick="javascript:deleteAttendanceTypeMaster(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
        return '<div class="divEditDelete"><a class="btn btn-primary btn-sm mx-1"  onclick="AttendanceTypeMaster.AddEditAttendanceTypeMaster(' + rowdata.RowKey + ')" ><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-danger btn-sm mx-1" href="#"   onclick="javascript:deleteAttendanceTypeMaster(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';

    }

    var addEditAttendanceTypeMaster = function (id) {

        var URL = $("#hdnAddEditAttendanceTypeMaster").val() + "?id=" + id;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {
                AppCommon.FormatInputCase()
                AppCommon.FormatDateInput();
                AppCommon.FormatTimeInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AttendanceTypeMaster.GetAttendanceTypeDetails(jsonData);
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);


        //var obj = {};
        //id = parseInt(id) ? parseInt(id) : 0;
        //var url = $("#hdnAddEditAttendanceTypeMaster").val() + "/" + id + "?" + $.param(obj);
        //$("#dvAddEditAttendanceTypeMaster").mLoading();
        //$.ajax({
        //    type: "Get",
        //    url: url,
        //    success: function (response) {
        //        if (response) {
        //            $("#dvAddEditAttendanceTypeMaster").html(response)
        //            $.validator.unobtrusive.parse("form");
        //            AppCommon.FormatInputCase();
        //            AppCommon.FormatSelect2();
        //             $("#btnCancel", $("#dvAddEditAttendanceTypeMaster")).on("click", function () {
        //                 AttendanceTypeMaster.GetAttendanceTypeMaster();
        //                 AttendanceTypeMaster.AddEditAttendanceTypeMaster(null);
        //                 return false;
        //             })
        //        }
        //        else {
        //            $("#dvAddEditAttendanceTypeMaster").mLoading("destroy");
        //        }
        //    }
        //})

    }


    return {
        GetAttendanceTypeMaster: getAttendanceTypeMaster,
        FormSubmit: formSubmit,
        GetAttendanceTypeDetails: getAttendanceTypeDetails,
        AddEditAttendanceTypeMaster: addEditAttendanceTypeMaster
    }
}());

function deleteAttendanceTypeMaster(RowKey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_UniversityCourse,
        actionUrl: $("#hdnDeleteAttendanceTypeMaster").val(),
        actionValue: RowKey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

var deleteAttendanceTypeDetails = function (rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Enquiry,
        actionUrl: $("#hdnDeleteAttendanceTypeDetails").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");
            if (response.IsSuccessful)
                $(item).remove();
            // Application.ReloadData();
        }
    });
};