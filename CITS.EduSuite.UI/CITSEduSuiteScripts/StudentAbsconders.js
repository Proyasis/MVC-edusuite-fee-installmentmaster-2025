var ajaxRequest = null;

var StudentAbsconders = (function () {

    var getApplicationDetails = function () {
        
        var model = {};
        model.AdmissionNo = $("#AdmissionNo").val();
        var ApplicationKey = $("#ApplicationKey").val();
        model.ApplicationKey = parseInt(ApplicationKey) ? parseInt(ApplicationKey) : 0;

        if (model.AdmissionNo != "" && model.AdmissionNo != null || (model.ApplicationKey != 0)) {

            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnStudentAbscondersApplication").val(),
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

    var bindStudentAbscondersDetails = function () {
        $("#dvStudentAbscondersDetails").mLoading()
        $("#dvStudentAbscondersDetails").load($("#hdnBindStudentAbscondersDetails").val());
    }
    var deleteStudentAbsconders = function (rowkey, ApplicationKey) {
        var id = ApplicationKey;
       var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationFeePayment,
            actionUrl: $("#hdnDeleteStudentAbsconders").val(),
            actionValue: rowkey,
            dataRefresh: function () {              
                StudentAbsconders.BindStudentAbscondersDetails()
            }
        });
    }
    var updateStudentAbsconders = function (rowkey, ApplicationKey) {
        
        var id = ApplicationKey;
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationFeePayment,
            actionUrl: $("#hdnUpdateStatusStudentAbsconders").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                StudentAbsconders.BindStudentAbscondersDetails();
                StudentAbsconders.AddEditStudentAbsconders(0, id);
            }
        });
    }

    var addEditStudentAbsconders = function (id, ApplicationKey) {

        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.ApplicationKey = ApplicationKey;
        var url = $("#hdnAddEditStudentAbsconders").val() + "/" + id + "?" + $.param(obj);
        $("#dvFeeDetails").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvFeeDetails").html(response)
                    $.validator.unobtrusive.parse("form");
                    StudentAbsconders.FillAttendanceTypeByDate();
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
        obj.Date = $("#AbscondersDate").val();
        obj.ApplicationKey = $("#ApplicationsKey").val();
        obj.AttendanceTypeKey = $("#OldAttendanceTypeKey").val();
        obj.AttendanceTypeKey = parseInt(obj.AttendanceTypeKey) ? parseInt(obj.AttendanceTypeKey) : 0;
        obj.AttendanceStatusKey = Resources.AttendanceStatusPresent;
        Shared.GetAttendanceTypeByDate(obj);
    }

    function formSubmit(data) {

        var $form = $("#frmAddEditStudentAbsconders")
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
                                StudentAbsconders.BindStudentAbscondersDetails();
                                StudentAbsconders.AddEditStudentAbsconders(0, id);
                            }
                        }
                    }
                })

            }

        }
    }

    return {
        GetApplicationDetails: getApplicationDetails,
        BindStudentAbscondersDetails: bindStudentAbscondersDetails,
        AddEditStudentAbsconders: addEditStudentAbsconders,
        DeleteStudentAbsconders: deleteStudentAbsconders,
        UpdateStudentAbsconders: updateStudentAbsconders,
        FillAttendanceTypeByDate: fillAttendanceTypeByDate,
        FormSubmit: formSubmit,
    }
}());