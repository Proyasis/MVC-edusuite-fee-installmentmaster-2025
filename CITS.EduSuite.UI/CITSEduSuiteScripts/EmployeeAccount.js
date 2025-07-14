var EmployeeAccount = (function () {
    var formSubmit = function (btn) {
        var form = $(btn).closest("form")[0];
        var url = $(form).attr("action");
        if ($(form).valid()) {
            var data = $(form).serializeArray();
            delete data[0];
            var response = AjaxHelper.ajax("POST", url,
                              {
                                  model: AppCommon.ObjectifyForm(data)
                              });
            if (response.IsSuccessful == true) {
                toastr.success(Resources.Success, response.Message);
                 $("#tab-profile  li a.active").parent().next('li').find('a').trigger('click');

            }
            else {
                $("[data-valmsg-for=error_msg_payment]").html(response.Message);
            }

        }
    }

    return {
        FormSubmit: formSubmit
    }

}());