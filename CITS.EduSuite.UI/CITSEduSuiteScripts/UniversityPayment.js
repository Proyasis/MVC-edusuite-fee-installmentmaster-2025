
var UniversityPayment = (function () {

    var getApplication = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetApplications").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                ApplicantName: function () {
                    return $('#txtSearchApplicantName').val()
                },
                MobileNumber: function () {
                    return $('#txtSearchMobileNumber').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                BatchKey: function () {
                    return $('#BatchKey').val()
                },
                CourseKey: function () {
                    return $('#SearchCourseKey').val()
                },
                UniversityKey: function () {
                    return $('#SearchUniversityKey').val()
                }

            },
            colNames: [Resources.RowKey, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Branch, Resources.AdmissionNo,
            Resources.Name, Resources.MobileNo, Resources.Course,// Resources.University,
            Resources.CurrentYear, Resources.Batch, Resources.TotalPaid, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: false, hidden: true, name: 'CurrentYear', index: 'CurrentYear', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicantName', index: 'ApplicantName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear },
                //{ key: false, name: 'UniversityName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TotalPaid', index: 'TotalPaid', editable: true, cellEdit: true, formatter: 'currencyFmatter', sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 50, 100, 250, 500, 1000],
            hidegrid: false,
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            },
            //onPaging: function () {
            //    var CurrPage = $(".ui-pg-input", $("#pager")).val();
            //    $("#grid").setGridParam({ datatype: 'json', page: CurrPage });
            //    $("#grid").trigger("reloadGrid");
            //}
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnAddUniversityFeeDetails").val() + '/' + rowdata.RowKey + '"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + Resources.Payment + '</a></div>';
    }

    function formatColor(cellValue, option, rowdata, action) {

        var amount = parseFloat(cellValue) ? parseFloat(cellValue) : 0;
        amount = amount.toLocaleString();
        //return '<span  class="label label-success">' + amount + '</span>';
        return '<i  class="fa fa-inr" aria-hidden="true"></i>' + amount;

    }


    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }
    function formatCourseUniversityYear(cellValue, options, rowdata, action) {

        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }

    var getFeePaymentDetails = function (json) {
        modelData = $.extend(true, {}, json);
        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                var item = $(this);
                $("[id*=RowKey][type=hidden]", $(this)).val(0);
                $("[id*=FeeTypeKey]", $(this)).on("change", function () {
                    UniversityPayment.GetCashFlowTypeKey(item, $(this).val())
                    UniversityPayment.CalculateTotalFee();
                    UniversityPayment.CheckFeeTypeMode($(this).val())
                });
                $("input[id*=FeeAmount]", $(this)).on("input", function () {
                    UniversityPayment.CalculateTotalFee($(this));
                });
                $("[id*=FeeTypeKey]").attr("disabled", false);
                $("[id*=UniversityPaymentYear]").attr("disabled", false);

            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteUniversityFeeOneByOne($(hidden).val(), $(this));
                }
                else {
                    $(this).slideUp(remove);
                }
                setTimeout(function () {
                    UniversityPayment.CalculateTotalFee();
                }, 500);
            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    if ($("#RowKey").val() == "0") {
                        window.open($("#hdnViewFeePrint").val() + "/" + response.RowKey, '_blank');
                    }
                    toastr.success(Resources.Success, response.Message);
                    if ($("#tab-application li.active").parent().next('li').find('a')[0])
                        $("#tab-application  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                    else
                        window.location.reload();
                }

            },
            data: json,
            repeatlist: 'UniversityFeePaymentDetails',
            submitButton: '',
            //defaultValues: json,
        });
    }

    var editUniversityFeePayment = function (id, ApplicationKey) {


        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.ApplicationKey = ApplicationKey;
        var url = $("#hdnAddEditUniversityFeePayment").val() + "/" + id + "?" + $.param(obj);
        $("#dvFeeDetails").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvFeeDetails").html(response)
                    $.validator.unobtrusive.parse("form");

                    $("[data-repeater-item]").each(function () {

                        var isFeeTypeYear = $("[id*=IsFeeTypeYear]", this).val();

                        if (isFeeTypeYear != null) {
                            isFeeTypeYear = JSON.parse(isFeeTypeYear.toLowerCase()) ? JSON.parse(isFeeTypeYear.toLowerCase()) : false;
                            if (isFeeTypeYear) {
                                $(".divFeeYear", this).hide();
                            }
                            else {
                                $(".divFeeYear", this).show();
                            }
                        }
                        var rowKey = $("[id*=RowKey]", this).val();
                        rowKey = parseInt(rowKey) ? parseInt(rowKey) : 0;
                        if (rowKey != 0) {
                            $("[id*=FeeTypeKey]").attr("disabled", true);
                            $("[id*=FeeYear]").attr("disabled", true);

                        }

                        if (id != 0) {
                            $(".btnfeeEdit").hide();

                        }
                        else {
                            $(".btnfeeEdit").show();
                        }
                    })


                } else {
                    $("#dvFeeDetails").mLoading("destroy");
                }

            }
        })

    }

    var deleteUniversityFeePayment = function (rowkey, ApplicationKey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationFeePayment,
            actionUrl: $("#hdnDeleteUniveristyFeePayment").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                UniversityPayment.EditUniversityFeePayment(null, ApplicationKey);
                UniversityPayment.BindTotalFeeDetails();
                UniversityPayment.BindFeePaymentDetails();
                //window.location.reload();
                //ContentLoad();
            }
        });
    }

    var formPaymentSubmit = function (btn) {
        var form = $(btn).closest("form")[0];
        var url = $(form).attr("action");
        if ($(form).valid()) {
            $(form).closest(".panel-body").mLoading()
            var data = $(form).serializeArray();
            delete data[0];

            setTimeout(function () {
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
                $(form).closest(".panel-body").mLoading("destroy")
            }, 500);
        }
    }

    var checkPaymentMode = function (PayMode) {
        $("#dvPaymentModes").show();
        $("#dvPaymentModeSub").hide();
        switch (parseInt(PayMode || 0)) {
            case 2:
                $(".divChequeDetails,.divBankAccount").show();
                $(".divCardDetails,.divReferenceNo").hide();

                break;
            case 3:
                $("#dvPaymentModeSub").show();
                $(".divBankAccount").show();
                $(".divCardDetails,.divChequeDetails,.divReferenceNo").hide();
                break;

            default:
                $("#dvPaymentModes,.divChequeDetails,.divCardDetails,.divBankAccount,.divReferenceNo").hide();
                break;
        }




    }
    var checkPaymentModeSub = function (PayMode) {
        $("#dvPaymentModes").show();

        switch (parseInt(PayMode || 0)) {
            case 1:
                $(".divChequeDetails,.divBankAccount").show();
                $(".divCardDetails,.divReferenceNo").hide();

                break;
            case 2:
                $(".divCardDetails,.divBankAccount").show();
                $(".divChequeDetails,.divReferenceNo").hide();
                break;

            default:

                $("#dvPaymentModes,.divBankAccount,.divReferenceNo").show();

                $(".divChequeDetails,.divCardDetails").hide();
                break;
        }

    }

    var getCashFlowTypeKey = function (item, id) {
        var response = AjaxHelper.ajax("GET", $("#hdnGetCashFlowTypeKey").val() + "?id=" + id)
        $("[id*=CashFlowTypeKey]", $(item)).val(response);
    }


    var checkFeeTypeMode = function (_this) {

        var obj = {};
        var item = $(_this).closest("[data-repeater-item]");


        obj.id = $("select[id*=FeeTypeKey]", item).val();
        obj.id = parseInt(obj.id) ? parseInt(obj.id) : 0;

        obj.applicationkey = $("#ApplicationKey").val();

        obj.Year = $("select[id*=UniversityPaymentYear]", item).val();
        obj.Year = parseInt(obj.Year) ? parseInt(obj.Year) : 0;

        $.ajax({
            url: $("#hdncheckFeeTypeMode").val(),
            type: "GET",
            dataType: "JSON",
            //data: { id: Id,year:obj.Year,applicationkey:applicationKey },
            data: obj,
            success: function (result) {
                $("[id*=IsFeeTypeYear]", $(item)).val(result.IsFeeTypeYear);
                $("[id*=UniversityPaymentAmount]", $(item)).val(result.UniversityPaymentAmount);
                if (result.IsFeeTypeYear == true) {
                    $(".divFeeYear", $(item)).hide();
                }
                else {
                    $(".divFeeYear", $(item)).show();
                }

            }
        });
    }
    var feeReceiptPopup = function (Id) {
        window.open($("#hdnViewFeePrint").val() + "/" + Id, '_blank');
    }

    var bindTotalFeeDetails = function () {
        $("#dvTotalFeeDetails").mLoading()
        $("#dvTotalFeeDetails").load($("#hdnUniversityTotalFeeDetails").val(), function () {
            UniversityPayment.CalculateTotalFee();
        });
    }

    var bindFeePaymentDetails = function () {
        $("#dvFeePaymentDetails").mLoading()
        $("#dvFeePaymentDetails").load($("#hdnUniversityFeePaymentList").val());
    }
    var calculateTotalFee = function () {
        var TotalAmount = $("#spnTotalFee").html() ? parseFloat($("#spnTotalFee").html()) : 0;
        var InputAmount = 0, ActualFeeAmount = 0;
        var TotalPaidAmount = $("#divCourseFeeDetails [paid-amount]").toArray().reduce(function (sum, element) {
            return sum + (parseFloat($(element).html()) ? parseFloat($(element).html()) : 0);
        }, 0);

        var BalanceFee = TotalAmount != 0 ? TotalAmount - TotalPaidAmount : 0;

        TotalPaidAmount = parseFloat(TotalPaidAmount.toFixed(2)).toString();
        $("#TotalPaid").val(TotalPaidAmount);
        $("#spnTotalPaid").html(TotalPaidAmount);
        $("#spnBalanceFee").html(BalanceFee);

    }

    var calculateFee = function (_this) {
        var TotalFee = 0;
        $("[data-repeater-item]").each(function () {
            var item = $(this)


            var FeeAmount = $("[id*=UniversityPaymentAmount]", $(item)).val();
            FeeAmount = parseFloat(FeeAmount) ? parseFloat(FeeAmount) : 0;
            TotalFee = TotalFee + FeeAmount;

        })
        $("#TotalUniversityFee").val(TotalFee);
        UniversityPayment.Checkbalance(_this)
    }

    function formSubmit() {


        var $form = $("#frmUniversityFeePayment")
        var JsonData = [];

        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        var ApplicationKey = formData["ApplicationKey"];

        if ($form.valid()) {
            $form.mLoading();
            var dataurl = $form.attr("action");

            AjaxHelper.ajaxAsync("POST", dataurl,
                {
                    model: formData
                }, function () {
                    var response = this;
                    if (typeof response == "string") {

                    }
                    if (response.IsSuccessful) {
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
                                        $form.mLoading("destroy");
                                        UniversityPayment.EditUniversityFeePayment(null, ApplicationKey);
                                        UniversityPayment.BindTotalFeeDetails();
                                        UniversityPayment.BindFeePaymentDetails();
                                    }
                                }
                            }
                        })

                    }
                    else {
                        $("[data-valmsg-for=error_msg]").html(response);
                    }
                    $form.mLoading("destroy");
                });


        }
    }

    var getBalance = function () {

        var PaidBranchKey = $("#PaidBranchKey").val();
        PaidBranchKey = parseInt(PaidBranchKey) ? parseInt(PaidBranchKey) : 0;

        if (PaidBranchKey != 0) {
            branchKey = PaidBranchKey;
        }
        else {
            branchKey = $("#BranchKey").val();
        }

        var paymentMode = $("[id*=PaymentModeKey]:checked", $("#dvPaymentMode")).val()
        var BankAccountKey = $("#BankAccountKey").val();
        var RowKey = $("#RowKey").val();
        var branchKey = branchKey;
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&RowKey=" + RowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey)

        $("#AccountHeadBalance").val(response);


    }

    var checkbalance = function (_this) {

        var item = $(_this).closest("[data-repeater-item]");
        var paymentModeKey = $("[id*=PaymentModeKey]:checked", $("#dvPaymentMode")).val()
        var IfChecking = false;

        if (Resources.PaymentModeCheque != paymentModeKey) {

            if (Resources.PaymentModeBank == paymentModeKey) {
                var BankAccountKey = $("#BankAccountKey").val();
                BankAccountKey = parseInt(BankAccountKey) ? parseInt(BankAccountKey) : 0;
                if (BankAccountKey != 0)
                    IfChecking = true;
                else
                    IfChecking = false;
            }
            else {
                IfChecking = true;
            }
            if (IfChecking) {


                var amount = $("#TotalUniversityFee").val();
                amount = amount != "" ? parseFloat(amount) : 0;
                if (amount != 0) {
                    var paidAmount = parseFloat(amount)
                    var AccountHeadBalance = $("#AccountHeadBalance").val()
                    AccountHeadBalance = parseFloat(AccountHeadBalance) ? parseFloat(AccountHeadBalance) : 0;

                    var RowKey = $("#RowKey").val();
                    RowKey = parseInt(RowKey) ? parseInt(RowKey) : 0;

                    var oldAmount = $("#OldAmount").val();
                    oldAmount = parseFloat(oldAmount) ? parseFloat(oldAmount) : 0


                    var Message = "";
                    Message = "Due to InSufficent Balance in Our account Please Check";
                    AccountHeadBalance = AccountHeadBalance - amount;
                    if (AccountHeadBalance < 0) {

                        if (typeof _this === "undefined") {
                            var list = $("[data-repeater-item]").last();
                            $("[id*=UniversityPaymentAmount]", $(list)).val("");
                        } else {
                            $("[id*=UniversityPaymentAmount]", $(item)).val("");
                        }

                        $.alert({
                            type: 'yellow',
                            title: Resources.Warning,
                            content: Message,
                            icon: 'fa fa-check-circle-o-',
                            buttons: {
                                Ok: {
                                    text: Resources.Ok,
                                    btnClass: 'btn-success',
                                    action: function () {
                                        // window.location.href = $("#hdnEmployeeList").val();
                                    }
                                }
                            }
                        })

                    }
                }
            }
        }
    }

    return {
        GetApplication: getApplication,
        GetFeePaymentDetails: getFeePaymentDetails,
        EditUniversityFeePayment: editUniversityFeePayment,
        DeleteUniversityFeePayment: deleteUniversityFeePayment,
        CheckPaymentMode: checkPaymentMode,
        FormPaymentSubmit: formPaymentSubmit,
        GetCashFlowTypeKey: getCashFlowTypeKey,
        //CalculateTotalFee: calculateTotalFee,
        CheckFeeTypeMode: checkFeeTypeMode,
        FeeReceiptPopup: feeReceiptPopup,
        BindTotalFeeDetails: bindTotalFeeDetails,
        CalculateTotalFee: calculateTotalFee,
        CheckPaymentModeSub: checkPaymentModeSub,
        FormSubmit: formSubmit,
        BindFeePaymentDetails: bindFeePaymentDetails,
        GetBalance: getBalance,
        Checkbalance: checkbalance,
        CalculateFee: calculateFee
    }

}());


function ContentLoad() {
    AppCommon.FormatInputCase();
    AppCommon.FormatDateInput();
    $("form").removeData("validator");
    $("form").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("form");
    $("form").on("submit", function () {
        $(".section-content").mLoading();
    }).bind('ajax:complete', function () {
        $(".section-content").mLoading("destroy");

    });
}
function EnableTabs() {
    $("#tab-application li").each(function () {
        $(this).removeClass("disabled");
    });
}
function DisableTabs() {
    $("#tab-application li").each(function () {
        $(this).addClass("disabled");
    });
}


function deleteUniversityFeeOneByOne(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ApplicationDocument,
        actionUrl: $("#hdnDeleteUniversityFeeOneByOne").val(),
        actionValue: rowkey,
        dataRefresh: function () {

            window.location.reload();

        }
    });

}


