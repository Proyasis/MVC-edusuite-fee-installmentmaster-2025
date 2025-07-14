var ApplicationFeeInstallment = (function () {
    var getApplicationFeeInstallments = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).fadeIn();
                //$("[id*=OriginalIssuedDate]", $(this)).val(moment(new Date()).format("DD/MM/YYYY"))
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                $("input[type=text]", $(this)).each(function () { $(this).val("") })
                var item = $(this);

                setTimeout(function () {
                    $("input[id*=InstallmentAmount]", $(item)).on("input", function () {
                        ApplicationFeeInstallment.CalculateInstallmentFeeTotal();
                    })
                    $("select[id*=InstallmentMonth]", $(item)).on('change', function () {
                        monthOrYearChanged($(this))
                    });
                }, 500)


            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];

                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteApplicationFeeinstallment($(hidden).val(), $(this));
                }
                else {
                    $(this).fadeIn(remove);
                    setTimeout(function () {
                        ApplicationFeeInstallment.CalculateInstallmentFeeTotal();
                    }, 500)
                }

            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    var feeyears = response.FeeYears.length;
                    if (response.FeeYear == feeyears) {
                        $("#tab-application  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                    }
                    else {
                        $("#tab-feeyear ul  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                    }


                }

            },
            data: json,
            repeatlist: 'ApplicationFeeInstallments',
            submitButton: '#btnSave',
            defaultValues: json,
            hasFile: true
        });
    }

    var calculateInstallmentFeeTotal = function () {

        var TotalInstallmentFee = 0;
        var TotalFeeAmount = $("#TotalFeeAmount").val();
        TotalFeeAmount = TotalFeeAmount != "" ? parseFloat(TotalFeeAmount) : 0;
        $("#dvRepeaterList [data-repeater-item]").each(function (i) {
            var InstallmentAmount = $("input[id*=InstallmentAmount]", $(this)).val();
            InstallmentAmount = InstallmentAmount != "" ? parseFloat(InstallmentAmount) : 0;
            TotalInstallmentFee = TotalInstallmentFee + InstallmentAmount;

            if (TotalInstallmentFee > TotalFeeAmount) {

                var balance = TotalInstallmentFee - TotalFeeAmount;
                balance = balance != "" ? parseFloat(balance) : 0;
                InstallmentAmount = InstallmentAmount - balance;
                $("input[id*=InstallmentAmount]", $(this)).val(InstallmentAmount)
                TotalInstallmentFee = TotalFeeAmount;
            }
        })
        $("span#TotalInstallmentFee").html(TotalInstallmentFee);
        $("input#TotalInstallmentFee").val(TotalInstallmentFee);
        $("span#TotalInstallmentFee").css("font-size", "12px").css("text-align", "center");
    }

    return {
        GetApplicationFeeInstallments: getApplicationFeeInstallments,
        CalculateInstallmentFeeTotal: calculateInstallmentFeeTotal
    }

}());

function deleteApplicationFeeinstallment(rowkey, _this) {

    //var result = EduSuite.Confirm({
    //    title: Resources.Confirmation,
    //    content: Resources.Delete_Confirm_ApplicationDocument,
    //    actionUrl: $("#hdnDeleteApplicationFeeInstallment").val(),
    //    actionValue: rowkey,
    //    dataRefresh: function () {
    //        //$(_this).fadeIn(remove);
    //        setTimeout(function () {
    //            ApplicationFeeInstallment.CalculateInstallmentFeeTotal();
    //        }, 500)
    //    }
    //});

    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ApplicationDocument,
        actionUrl: $("#hdnDeleteApplicationFeeInstallment").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            //$(_this).fadeIn(remove);
            setTimeout(function () {
                Application.ReloadData();
                //ApplicationFeeInstallment.CalculateInstallmentFeeTotal();
            }, 500)
        }
    });
}
