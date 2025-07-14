var ApplicationFamilyDetails = (function () {
    var getApplicationFamilyDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
                //$("[id*=OriginalIssuedDate]", $(this)).val(moment(new Date()).format("DD/MM/YYYY"))
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deletegetApplicationFamilyDetails($(hidden).val(), $(this));
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
                    $("#tab-application  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                }

            },
            data: json,
            repeatlist: 'FamilyDetailsModel',
            submitButton: '#btnSave',
            defaultValues: json,
            hasFile: true
        });
    }

    return {
        GetApplicationFamilyDetails: getApplicationFamilyDetails
    }

}());

function deletegetApplicationFamilyDetails(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ApplicationDocument,
        actionUrl: $("#hdnDeleteApplicationFamiliyDetails").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            Application.ReloadData();
        }
    });
}
