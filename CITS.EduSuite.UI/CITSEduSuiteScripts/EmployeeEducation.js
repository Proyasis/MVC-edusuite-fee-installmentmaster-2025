var EmployeeEducation = (function () {
    var getEmployeeEducations = function (json) {

        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    AppCommon.FormatDateInput();
                    AppCommon.CustomRepeaterRemoteMethod();
                    AppCommon.FormatInputCase();
                    $("input[type=text]", $(this)).each(function () { $(this).val("") })
                    $("[href]", $(this)).hide();
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteEmployeeEducation($(hidden).val(), $(this));
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
                    Employee.ReloadData();
                },
                data: json,
                repeatlist: 'EmployeeEducations',
                submitButton: '#btnSave',
                defaultValues: json,
                hasFile:true
            });
    }


    return {
        GetEmployeeEducations: getEmployeeEducations

    }

}());

function deleteEmployeeEducation(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeEducation,
        actionUrl: $("#hdnDeleteEmployeeEducation").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            Employee.ReloadData();

        }
    });
}

