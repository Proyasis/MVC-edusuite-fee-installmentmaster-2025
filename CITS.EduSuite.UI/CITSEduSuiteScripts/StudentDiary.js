var ajaxRequest = null;

var StudentDiary = (function () {

    var getApplicationDetails = function () {

        var model = {};
        model.AdmissionNo = $("#AdmissionNo").val();
        var ApplicationKey = $("#ApplicationKey").val();
        model.ApplicationKey = parseInt(ApplicationKey) ? parseInt(ApplicationKey) : 0;
        if (model.AdmissionNo != "" && model.AdmissionNo != null || (model.ApplicationKey != 0)) {

            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnStudentDiaryApplication").val(),
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

    var bindStudentDiaryDetails = function () {
        $("#dvInstallmentDetails").mLoading()
        $("#dvInstallmentDetails").load($("#hdnBindStudentDiaryDetails").val());
    }
    var deleteStudentDiary = function (rowkey, ApplicationKey) {
        var id = ApplicationKey;
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationFeePayment,
            actionUrl: $("#hdnDeleteStudentDiary").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                StudentDiary.BindStudentDiaryDetails();
            }
        });
    }

    var addEditStudentDiary = function (id, ApplicationKey) {

        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.ApplicationKey = ApplicationKey;
        var url = $("#hdnAddEditStudentDiary").val() + "/" + id + "?" + $.param(obj);
        $("#dvFeeDetails").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvFeeDetails").html(response)
                    $.validator.unobtrusive.parse("form");
                    $("#frmAddEditStudentDiary", $("#dvFeeDetails")).on("submit", function () {
                        formSubmit();
                        return false;
                    })
                    //ApplicationFeePayment.BindTotalFeeDetails();
                    //ApplicationFeePayment.BindInstallmentFeeDetails();
                }
                else {
                    $("#dvFeeDetails").mLoading("destroy");
                }
            }
        })

    }
    function formSubmit() {

        var $form = $("#frmAddEditStudentDiary")
        var JsonData = [];
        var formData = $form.serializeToJSON({ associativeArrays: false });
      
        if ($form.valid()) {
            var dataurl = $form.attr("action");
            var response = [];

            AjaxHelper.ajaxAsync("POST", dataurl,
                       {
                           model: formData
                       }, function () {
                           response = this;
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
                                               var teacherPortalRebind = localStorage.getItem('teacherPortalRebind');
                                               if (teacherPortalRebind) {
                                                   var teacherPortalRebind = eval('(' + teacherPortalRebind + ')');

                                               } else {
                                                   StudentDiary.BindStudentDiaryDetails();
                                                   StudentDiary.AddEditStudentDiary(null, response.ApplicationsKey);
                                               }

                                           }
                                       }
                                   }
                               })

                           }
                           $form.mLoading("destroy");
                       });


        }
    }
    return {
        GetApplicationDetails: getApplicationDetails,
        BindStudentDiaryDetails: bindStudentDiaryDetails,
        AddEditStudentDiary: addEditStudentDiary,
        DeleteStudentDiary: deleteStudentDiary
    }
}());