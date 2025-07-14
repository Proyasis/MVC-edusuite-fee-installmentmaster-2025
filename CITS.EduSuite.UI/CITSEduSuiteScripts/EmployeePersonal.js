

var EmployeePersonal = (function ()
{

    var getDepartmentByBranchId = function (Id){
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetDepartmentByBranchId").val(), $("#DepartmentKey"), Resources.Select + Resources.BlankSpace + Resources.Department, "Departments");
    }


    var getGradeByDesignationId = function (Id) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetGradeByDesignationId").val(), $("#GradeKey"), Resources.Select + Resources.BlankSpace + Resources.Grade, "Grades");
    }

    var getHigherEmployeesById = function () {
        var obj = {};
        obj.BranchId = $("#BranchKey").val();
        obj.DesignationId = $("#DesignationKey").val();
        AppCommon.BindDropDownbyId(obj, $("#hdnGetHigherEmployeesById").val(), $("#HigherEmployeeUserKey"), Resources.Select + Resources.BlankSpace + Resources.Employee, "HigherEmployees");

    }


    var formSubmit = function (btn)
    {

        var form = $(btn).closest("form")[0];
        var url = $(form)[0].action;
        if ($(form).valid())
        {
            $(".section-content").mLoading();
            var data = $(form).serializeArray();
            delete data[0];
            data = AppCommon.ObjectifyForm(data)
            $.ajax({
                type: "POST", url: url,
                data: $(form).serializeArray(),
                success: function (response)
                {
                    if (response.IsSuccessful == true)
                    {

                        if (response.NotificationBySMS)
                        {
                            var smsModel = createConfirmationMobileNumberModel(data);
                            var resultMobile = AjaxHelper.ajax("POST", $("#hdnMobileNumberConfirmation").val(),
                                  {
                                      model: smsModel
                                  });
                            if (resultMobile.IsSuccessful)
                            {
                                ConfirmMobileNumber(resultMobile);
                            }
                        }
                        if (response.NotificationByEmail)
                        {
                            var emailModel = createConfirmationEmailAddressModel(data);
                            var emailTemplate = createConfirmationEmailTemplate(emailModel);

                            var resultEmail = AjaxHelper.ajax("POST", $("#hdnEmailConfirmation").val(),
                               {
                                   template: emailTemplate,
                                   model: emailModel
                               }
                             );
                        }
                        toastr.success(Resources.Success, response.Message);

                        $("#tab-profile li").each(function ()
                        {
                            var url = $(this).find("a").data("href");
                            $(this).find("a").attr("data-href", url.replace('0', response.RowKey))
                            $(this).show();
                        })
                        if (!response.IsTeacher) {
                            $("#tab-profile li a[href='#classAllocation']").parent().hide();
                        }

                        $("#tab-profile  li a.active").parent().next('li').find('a').trigger('click');

                    }
                    else
                    {
                        toastr.error(Resources.Failed, response.Message);
                        $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                    }
                    $(".section-content").mLoading("destroy");
                },
                error: function (xhr)
                {

                }
            });

        }

    }
  var salaryTypeChangeEvent = function (_this) {
        var SalaryTypeKey = $(_this).val();
        SalaryTypeKey = parseInt(SalaryTypeKey) ? parseInt(SalaryTypeKey) : 0;
      if (SalaryTypeKey == Resources.SalaryTypeMonthly) {
            $("#divGrade").show();
        }
        else {
            $("#divGrade").hide();
        }
    }

    var getGradeDetailsById = function (Id) {
        $("#MonthlySalary").val(""); // clear before appending new list 
        $.ajax({
            url: $("#hdnGetGradeDetailsById").val(),
            type: "GET",
            dataType: "JSON",
            data: { id: Id },
            success: function (result) {
                $("#MonthlySalary").val(result.MonthlySalary);
            }
        });
    }

    return {
        GetDepartmentByBranchId: getDepartmentByBranchId,
        GetGradeByDesignationId: getGradeByDesignationId,
        GetHigherEmployeesById: getHigherEmployeesById,
        FormSubmit: formSubmit,
        SalaryTypeChangeEvent: salaryTypeChangeEvent,
        GetGradeDetailsById: getGradeDetailsById
    }

}());


function createConfirmationEmailTemplate(modelData)
{
    var model = { EmailTo: modelData["EmailAddress"], EmailBody: "", EmailSubject: "Email Confirmation", EmailAttachment: "" };
    var filepath = Resources.ServerPath + "Templates/EmailConfirmation.html";
    $.ajaxSetup({ async: false });
    $.get(filepath, function (data)
    {
        html = data;
        var urlParam = {};
        urlParam.Code = modelData["EmailVerificationCode"];
        urlParam.Id = modelData["EmployeeKey"];
        var url = location.protocol + "//" + location.host + Resources.ServerPath + $("#hdnConfirmEmailAddress").val() + AppCommon.EncodeQueryString($.param(urlParam));
        var replaceData = { "ConfirmationUrl": url };
        html = Mustache.to_html(html, replaceData);
        html = html.replace("script", "html").replace('type="text/html"', "");
        model["EmailBody"] = html;
    });
    return model
}

function createConfirmationEmailAddressModel(data)
{
    var model = {
        RowKey: 0,
        EmployeeKey: data["RowKey"],
        MobileNumber: null,
        EmailAddress: data["EmailAddress"],
        SMSVerificationCode: null,
        EmailVerificationCode: AppCommon.GenerateRandomCode(),
        IsMobileVerified: null,
        IsEmailVerified: false
    };
    return model
}

function createConfirmationMobileNumberModel(data)
{
    var model = {
        RowKey: 0,
        EmployeeKey: data["RowKey"],
        MobileNumber: data["MobileNumber"],
        EmailAddress: null,
        SMSVerificationCode: AppCommon.GenerateRandomCode(),
        EmailVerificationCode: null,
        IsMobileVerified: false,
        IsEmailVerified: null
    };
    return model
}

function ConfirmMobileNumber(data)
{
    var JsonData = data;
    $.confirm({
        title: 'Mobile Number Verification',
        content: 'Verification COde : ' + data["SMSVerificationCode"] + '<input type="hidden" id="hdnEmployeeKey" value="' + data["EmployeeKey"] + '"/><input type="text" placeholder="Enter verification code here..." id="txtVerificationCode" class="form-control" onkeyup="ValidateCodeInput(this)"/>',
        buttons: {
            Add: {
                text: 'Verify',
                btnClass: 'btn-success',
                action: function ()
                {
                    var input = this.$content.find('input#txtVerificationCode');
                    var errorText = this.$content.find('.text-danger');
                    if (!input.val().trim())
                    {
                        $.alert({
                            content: Resources.GradeNameRequired,
                            type: 'red'
                        });
                        return false;
                    } else
                    {
                        var EmployeeKey = $("#hdnEmployeeKey").val() != "" ? parseInt($("#hdnEmployeeKey").val()) : 0;
                        var response = AjaxHelper.ajax("GET", $("#hdnConfirmMobileNumber").val() + "?Code=" + $("#txtVerificationCode").val().trim() + "&&Id=" + EmployeeKey);
                        if (response.IsSuccessful == true)
                        {
                            $.alert({
                                content: response.Message,
                                type: 'green'
                            });
                        }
                        else
                        {
                            $.alert({
                                content: 'Mobile Number Verification Failed',
                                type: 'red'
                            });
                            return false;
                        }
                    }
                }
            },
            Try: {
                text: 'Try Another',
                btnClass: 'btn-outline-primary',
                action: function ()
                {
                    var modelData = createConfirmationMobileNumberModel(JsonData);
                    var response = AjaxHelper.ajax("POST", $("#hdnMobileNumberConfirmation").val(),
                        {
                            model: modelData
                        });
                    if (response)
                    {
                        ConfirmMobileNumber(modelData);
                    }
                }
            },
            Skip: {
                text: 'Skip',
                btnClass: 'btn-warning',
                action: function ()
                {

                }
            }
        }

    });
    setTimeout(function ()
    {
        $("input#txtGradeName").focus();
    }, 1000)
}

function ValidateCodeInput(_this)
{
    var start = _this.selectionStart
    $(_this).val($(_this).val().toUpperCase())
    var test = /[^A-Z0-9]/;
    if (test.test($(_this).val()))
    {
        var newVal = $(_this).val().replace(/[^A-Z0-9]/g, "");
        $(_this).val(newVal);
    }
    _this.selectionStart = _this.selectionEnd = start;
}