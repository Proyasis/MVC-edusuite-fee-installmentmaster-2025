var ajaxRequest = null;

var StudentEarlyDeparture = (function () {

    var getApplicationDetails = function () {



        var model = {};
        model.AdmissionNo = $("#AdmissionNo").val();
        var ApplicationKey = $("#ApplicationKey").val();
        model.ApplicationKey = parseInt(ApplicationKey) ? parseInt(ApplicationKey) : 0;

        if (model.AdmissionNo != "" && model.AdmissionNo != null || (model.ApplicationKey != 0)) {
            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnStudentEarlyDepartureApplication").val(),
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

    var bindStudentEarlyDeparture = function () {
        $("#dvStudentEarlyDeparture").mLoading()
        $("#dvStudentEarlyDeparture").load($("#hdnBindStudentEarlyDepartureDetails").val());
    }
    var deleteStudentEarlyDeparture = function (rowkey, ApplicationKey) {
        var id = ApplicationKey;
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationFeePayment,
            actionUrl: $("#hdnDeleteStudentEarlyDeparture").val(),
            actionValue: rowkey,
            dataRefresh: function () {               
                StudentEarlyDeparture.BindStudentEarlyDeparture();
                StudentEarlyDeparture.AddEditStudentEarlyDeparture(0, id);
            }
        });
    }

    var addEditStudentEarlyDeparture = function (id, ApplicationKey) {

        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.ApplicationKey = ApplicationKey;
        var url = $("#hdnAddEditStudentEarlyDeparture").val() + "/" + id + "?" + $.param(obj);
        $("#dvFeeDetails").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvFeeDetails").html(response)
                    $.validator.unobtrusive.parse("form");
                    StudentEarlyDeparture.FillAttendanceTypeByDate();
                    //ApplicationFeePayment.BindTotalFeeDetails();
                    //ApplicationFeePayment.BindInstallmentFeeDetails();
                }
                else {
                    $("#dvFeeDetails").mLoading("destroy");
                }
            }
        })

    }

    function formSubmit(data) {

        var $form = $("#frmAddEditStudentEarlyDeparture")
        var JsonData = [];
        var TimeKeys = ["EarlyDepartureTime"]
        $("[disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {
            var id = formData['ApplicationsKey'];
            formData['EarlyDepartureTime'] = (formData['EarlyDepartureTime'] != "" ? moment(formData['EarlyDepartureTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
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
                                StudentEarlyDeparture.BindStudentEarlyDeparture();
                                StudentEarlyDeparture.AddEditStudentEarlyDeparture(0, id);
                            }
                        }
                    }
                })

            }

        }
    }
    var fillAttendanceTypeByDate = function () {
        var obj = {};
        obj.Date = $("#EarlyDepartureDate").val();
        obj.ApplicationKey = $("#ApplicationsKey").val();
        obj.AttendanceTypeKey = $("#OldAttendanceTypeKey").val();
        obj.AttendanceTypeKey = parseInt(obj.AttendanceTypeKey) ? parseInt(obj.AttendanceTypeKey) : 0;
        obj.AttendanceStatusKey = Resources.AttendanceStatusPresent;
        Shared.GetAttendanceTypeByDate(obj);
    }
    return {
        GetApplicationDetails: getApplicationDetails,
        BindStudentEarlyDeparture: bindStudentEarlyDeparture,
        AddEditStudentEarlyDeparture: addEditStudentEarlyDeparture,
        DeleteStudentEarlyDeparture: deleteStudentEarlyDeparture,
        FormSubmit: formSubmit,
        FillAttendanceTypeByDate: fillAttendanceTypeByDate,
    }
}());