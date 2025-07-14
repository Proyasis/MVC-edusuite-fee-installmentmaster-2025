var EmployeeIdentities = (function () {
    var getEmployeeIdentities = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                EmployeeIdentities.SetAdharNumberMask($(this))
                $("input[type=text]", $(this)).each(function () { $(this).val("") })
                $("[href]", $(this)).hide();
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteEmployeeIdentity($(hidden).val(), $(this));
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
            repeatlist: 'EmployeeIdentities',
            submitButton: '#btnSave',
            defaultValues: json,
            hasFile: true
        });
    }
    var setAdharNumberMask = function (item) {
        var ddlType = $("select[id*=IdentityTypeKey]", $(item));
        var txtAdhar = $("input[id*=IdentyUniqueID]", $(item));
        if ($(ddlType).val() == "1") {
            $(txtAdhar).inputmask("9999 9999 9999")
        }
        else {
            $(txtAdhar).inputmask('remove');
        }
        $(ddlType).change(function () {
            if ($(this).val() == "1") {
                $(txtAdhar).inputmask("9999 9999 9999")
            }
            else {
                $(txtAdhar).inputmask("remove");
            }
        })

    }
    return {
        GetEmployeeIdentities: getEmployeeIdentities,
        SetAdharNumberMask: setAdharNumberMask
    }

}());

function deleteEmployeeIdentity(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeIdentity,
        actionUrl: $("#hdnDeleteEmployeeIdentity").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            Employee.ReloadData();
        }
    });
}
