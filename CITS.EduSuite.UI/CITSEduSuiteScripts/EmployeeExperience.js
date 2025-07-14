
var EmployeeExperiences = (function () {

    var getEmployeeExperiences = function (json) {
        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    var item = $(this);
                    AppCommon.CustomRepeaterRemoteMethod();
                    AppCommon.FormatInputCase();
                    AppCommon.FormatDateInput();
                    $("input[type=text]", $(this)).each(function () { $(this).val("") })
                    $("[href]", $(this)).hide();
                    $("input[id*=ExperienceStartDate]", $(item)).datepicker('setStartDate', '');
                    $("input[id*=ExperienceStartDate]", $(item)).datepicker('setEndDate', new Date());
                    $("input[id*=ExperianceEndDate]", $(item)).datepicker('setStartDate', '');
                    $("input[id*=ExperianceEndDate]", $(item)).datepicker('setEndDate', new Date());
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteEmployeeExperience($(hidden).val(), $(this));
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
                        $("#tab-profile  li a.active").parent().next('li').find('a').trigger('click');
                    }

                },
                data: json,
                repeatlist: 'EmployeeExperiences',
                submitButton: '#btnSave',
                defaultValues: json,
                hasFile: true
            });
    }


    return {
        GetEmployeeExperiences: getEmployeeExperiences

    }

}());

function deleteEmployeeExperience(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeExperience,
        actionUrl: $("#hdnDeleteEmployeeExperience").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            Employee.ReloadData();
        }
    });
}


