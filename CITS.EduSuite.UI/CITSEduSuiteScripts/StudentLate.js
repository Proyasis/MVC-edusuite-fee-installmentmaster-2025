var ajaxRequest = null;

var StudentLate = (function () {

    var getApplicationDetails = function () {
        var model = {};
        model.AdmissionNo = $("#AdmissionNo").val();
        var ApplicationKey = $("#ApplicationKey").val();
        model.ApplicationKey = parseInt(ApplicationKey) ? parseInt(ApplicationKey) : 0;

        if (model.AdmissionNo != "" && model.AdmissionNo != null || (model.ApplicationKey != 0)) {



            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnStudentLateApplication").val(),
                contentType: "application/json; charset=utf-8",
                data: model,
                beforeSend: function () {
                    if (ajaxRequest != null) {
                        ajaxRequest.abort();
                    }
                },
                success: function (result) {
                    if (result.IsSuccessful == false) {
                        $("[data-valmsg-for=error_msg]").html(result.Message);

                    }
                    $("#DivCourseFee").html("")
                    $("#DivCourseFee").html(result);
                },
                error: function (request, status, error) {

                }
            });
        }
    }

    var bindStudentLateDetails = function () {

        $("#dvInstallmentDetails").mLoading()
        $("#dvInstallmentDetails").load($("#hdnBindStudentLateDetails").val());
    }
    var deleteStudentLate = function (rowkey, ApplicationKey) {
        var id = ApplicationKey;
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationFeePayment,
            actionUrl: $("#hdnDeleteStudentLate").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                StudentLate.BindStudentLateDetails();
                StudentLate.AddEditStudentLate(0, id);
            }
        });
    }

    var addEditStudentLate = function (id, ApplicationKey) {

        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.ApplicationKey = ApplicationKey;
        var url = $("#hdnAddEditStudentLate").val() + "/" + id + "?" + $.param(obj);
        $("#dvFeeDetails").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvFeeDetails").html(response)
                    $.validator.unobtrusive.parse("form");
                    StudentLate.FillAttendanceTypeByDate();
                    //ApplicationFeePayment.BindTotalFeeDetails();
                    //ApplicationFeePayment.BindInstallmentFeeDetails();
                }
                else {
                    $("#dvFeeDetails").mLoading("destroy");
                }
            }
        })

    }

    var fillAttendanceTypeByDate = function () {
        var obj = {};
        obj.Date = $("#LateDate").val();
        obj.ApplicationKey = $("#ApplicationsKey").val();
        obj.AttendanceTypeKey = $("#OldAttendanceTypeKey").val();
        obj.AttendanceTypeKey = parseInt(obj.AttendanceTypeKey) ? parseInt(obj.AttendanceTypeKey) : 0;
        obj.AttendanceStatusKey = Resources.AttendanceStatusAbsent;
        Shared.GetAttendanceTypeByDate(obj);
    }

    var getLateMinuteById = function () {
        var obj = {};
        obj.id = $("#AttendanceTypeKey").val();
        obj.Date = $("#LateDate").val();
        obj.ApplicationKey = $("#ApplicationsKey").val();
        var btn = $("#btnSave");
        if (!parseInt($("#RowKey").val())) {
            $.ajax({
                url: $("#hdnGetLateMinuteById").val(),
                type: "POST",
                dataType: "JSON",
                data: obj,
                success: function (result) {
                    if (result.LateMinutes > 0)
                        $("#LateMinutes").val(result.LateMinutes)
                    if (!result.IsSuccessful) {
                        result.Message = result.Message + AppCommon.JsonDateToNormalDate2(result.LateDate);
                        $("#spnerr").html(result.Message);

                        $(btn).attr("disabled", true);
                        $(btn).hide();
                    }
                    else {
                        $("#LateMinutes").val("")
                        $("#spnerr").html(result.Message);
                        $(btn).show();
                    }
                }

            });
        }
    }
    function formSubmit(data) {

        var $form = $("#frmAddEditStudentLate")
        var JsonData = [];
        $("[disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {
            var id = formData['ApplicationsKey'];

            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
            $("#btnSave").hide();
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
                                $("#btnSave").show();
                                StudentLate.BindStudentLateDetails();
                                StudentLate.AddEditStudentLate(0, id);
                            }
                        }
                    }
                })

            }

        }
    }
    return {
        GetApplicationDetails: getApplicationDetails,
        BindStudentLateDetails: bindStudentLateDetails,
        AddEditStudentLate: addEditStudentLate,
        DeleteStudentLate: deleteStudentLate,
        FillAttendanceTypeByDate: fillAttendanceTypeByDate,
        GetLateMinuteById: getLateMinuteById,
        FormSubmit: formSubmit,
    }
}());