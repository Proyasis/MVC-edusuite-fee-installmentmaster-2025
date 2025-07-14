var ajaxRequest = null;

var StudentLeave = (function () {

    var getApplicationDetails = function () {



        var model = {};
        model.AdmissionNo = $("#AdmissionNo").val();
        var ApplicationKey = $("#ApplicationKey").val();
        model.ApplicationKey = parseInt(ApplicationKey) ? parseInt(ApplicationKey) : 0;

        if (model.AdmissionNo != "" && model.AdmissionNo != null || (model.ApplicationKey != 0)) {



            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnStudentLeaveApplication").val(),
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

    var bindStudentLeaveDetails = function () {

        $("#dvInstallmentDetails").mLoading()
        $("#dvInstallmentDetails").load($("#hdnBindStudentLeaveDetails").val());
    }

    var deleteStudentLeave = function (rowkey, ApplicationKey) {
        var id = ApplicationKey;
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationFeePayment,
            actionUrl: $("#hdnDeleteStudentLeave").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                //window.location.href = $("#hdnStudentLeaveList").val() + "/" + id;
                StudentLeave.BindStudentLeaveDetails();
                StudentLeave.AddEditStudentLeave(0, id);
            }
        });
    }

    var addEditStudentLeave = function (id, ApplicationKey) {

        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.ApplicationKey = ApplicationKey;
        var url = $("#hdnAddEditStudentLeave").val() + "/" + id + "?" + $.param(obj);
        $("#dvFeeDetails").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvFeeDetails").html(response)
                    $.validator.unobtrusive.parse("form");
                    StudentLeave.FillAttendanceTypeByDate();
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
        $("#btnSave").removeAttr("disabled");
        $("[data-valmsg-for=error_msg]").html("");
        var dropdDownControl = $("#AttendanceTypeKey");;
        var obj = {};
        obj.Date = $("#LeaveDateFrom").val();
        var DateTo = $("#LeaveDateTo").val();
        if (!DateTo) {
            DateTo = obj.Date;
            $("#LeaveDateTo").val(obj.Date);
        }
        if (obj.Date != DateTo) {
            $(dropdDownControl).empty();
            $(dropdDownControl).append($("<option></option>").val("").html(Resources.Select + Resources.BlankSpace + Resources.AttendanceType));
            $(dropdDownControl).closest("[data-hidden]").hide();

        } else {
            $(dropdDownControl).closest("[data-hidden]").show();
            obj.ApplicationKey = $("#ApplicationsKey").val();
            obj.AttendanceTypeKey = $("#OldAttendanceTypeKey").val();
            obj.AttendanceTypeKey = parseInt(obj.AttendanceTypeKey) ? parseInt(obj.AttendanceTypeKey) : 0;
            obj.AttendanceStatusKey = Resources.AttendanceStatusAbsent;
            Shared.GetAttendanceTypeByDate(obj);
        }
    }

    function formSubmit(data) {

        var $form = $("#frmAddEditStudentLeave")
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
                                StudentLeave.BindStudentLeaveDetails();
                                StudentLeave.AddEditStudentLeave(0, id);
                            }
                        }
                    }
                })

            }

        }
    }

    return {
        GetApplicationDetails: getApplicationDetails,
        BindStudentLeaveDetails: bindStudentLeaveDetails,
        AddEditStudentLeave: addEditStudentLeave,
        DeleteStudentLeave: deleteStudentLeave,
        FillAttendanceTypeByDate: fillAttendanceTypeByDate,
        FormSubmit: formSubmit,
    }
}());