var EmployeeLanguageSkills = (function () {
    var getEmployeeLanguageSkills = function (json) {

        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    AppCommon.CustomRepeaterRemoteMethod();
                    AppCommon.FormatInputCase();
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteEmployeeLanguageSkill($(hidden).val(), $(this));
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
                repeatlist: 'EmployeeLanguageSkills',
                submitButton: '#btnSave',
                defaultValues: json
            });
    }


    return {
        GetEmployeeLanguageSkills: getEmployeeLanguageSkills

    }

}());

function deleteEmployeeLanguageSkill(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_LanguageSkills,
        actionUrl: $("#hdnDeleteEmployeeLanguageSkill").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            Employee.ReloadData();
        }
    });
}
