var jsonData = [], request = null;
$(".TableHeadId").hide();
var UniversityPaymentBulk = (function () {



    //University Payment Bulk

    var getUniversityPaymentStudentsList = function (json, pageIndex, resetPagination) {
        $(".TableHeadId").hide();
        if ($("#FeeTypeKey").val() != "") {
            JsonData = json;
            JsonData["SearchBranchKey"] = $("#SearchBranchKey").val();
            JsonData["SearchAcademicTermKey"] = $("#SearchAcademicTermKey").val();
            JsonData["SearchCourseTypeKey"] = $("#SearchCourseTypeKey").val();
            JsonData["SearchCourseKey"] = $("#SearchCourseKey").val();
            JsonData["SearchUniversityKey"] = $("#SearchUniversityKey").val();
            JsonData["SearchYearKey"] = $("#SearchYearKey").val();
            JsonData["FeeTypeKey"] = $("#FeeTypeKey").val();
            JsonData["SearchBatchKey"] = $("#SearchBatchKey").val();
            JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
            JsonData["PageSize"] = 10;
            $("#dvRepeaterList").empty();
            $("#dvScheduleContainer").mLoading();

            request = $.ajax({
                url: $("#hdnGetUniversityPaymentStudentsList").val(),
                data: JsonData,
                datatype: "json",
                type: "post",
                contenttype: 'application/json; charset=utf-8',
                async: true,
                beforeSend: function () {
                    if (request != null) {
                        request.abort();
                    }
                },
                success: function (data) {
                    $("#dvRepeaterList").html(data);

                    $(".TableHeadId").show();

                    $("form").removeData("validator");
                    $("form").removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse("form");

                    UniversityPaymentBulk.CheckIsApplied();

                    if (resetPagination) {
                        UniversityPaymentPagination();
                        $('.pagination  li').removeClass("active");
                        $('[data-lp="' + JsonData["PageIndex"] + '"]').addClass("active");
                        $('.pagination  li.next').removeClass("active");
                    }

                    $("#dvScheduleContainer").mLoading("destroy");
                },
                error: function (error) {
                    console.log(JSON.stringify(error));
                    // $("#dvScheduleContainer").mLoading("destroy");
                }
            });
        }

    }

    var getCourseTypeBySyllabus = function (obj, ddl) {
        //$(ddl).html("");
        //$(ddl).append($('<option></option>').val("").html(Resources.CourseType));
        //$.ajax({
        //    url: $("#hdnGetCourseTypeBySyllabus").val(),
        //    type: "GET",
        //    data: obj,
        //    dataType: "JSON",
        //    contentType: "application/json; charset=utf-8",
        //    success: function (result)
        //    {
        //        $.each(result.CourseTypes, function (i, CourseType)
        //        {
        //            $(ddl).append(
        //                $('<option></option>').val(CourseType.RowKey).html(CourseType.Text));
        //        });
        //    }
        //});
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseTypeBySyllabus").val(), ddl, Resources.CourseType, "CourseTypes");
    }

    var getCourseByCourseType = function (obj, ddl) {
        //$(ddl).html("");
        //$(ddl).append($('<option></option>').val("").html(Resources.Course));
        //$.ajax(
        //    {
        //        url: $("#hdnGetCourseByCourseType").val(),
        //        type: "GET",
        //        dataType: "JSON",
        //        data: obj,
        //        contentType: "application/json; charset=utf-8",
        //        success: function (result)
        //        {
        //            $.each(result.Courses, function (i, Course)
        //            {
        //                $(ddl).append(
        //                    $('<option></option>').val(Course.RowKey).html(Course.Text));
        //            });
        //        }
        //    });
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseByCourseType").val(), ddl, Resources.Course, "Courses");
    }

    var getUniversityByCourse = function (obj, ddl) {
        //$(ddl).html("");
        //$(ddl).append($('<option></option>').val("").html(Resources.University));
        //$.ajax({
        //    url: $("#hdnUniversityByCourse").val(),
        //    type: "GET",
        //    dataType: "JSON",
        //    data: obj,
        //    contentType: "application/json; charset=utf-8",
        //    success: function (result)
        //    {
        //        $.each(result.Universities, function (i, University)
        //        {
        //            $(ddl).append(
        //                $('<option></option>').val(University.RowKey).html(University.Text));
        //        });
        //    }
        //});
        AppCommon.BindDropDownbyId(obj, $("#hdnUniversityByCourse").val(), ddl, Resources.University, "Universities");

    }

    var getYearsBySyllabus = function (obj, ddl) {
        //$(ddl).html("");
        //$(ddl).append($('<option></option>').val("").html(Resources.Year));
        //$.ajax({
        //    url: $("#hdnGetYearsBySyllabus").val(),
        //    type: "GET",
        //    dataType: "JSON",
        //    data: obj,
        //    contentType: "application/json; charset=utf-8",
        //    success: function (result)
        //    {
        //        $.each(result.Years, function (i, Years)
        //        {
        //            $(ddl).append($('<option></option>').val(Years.RowKey).html(Years.Text));
        //        });
        //    }
        //});
        AppCommon.BindDropDownbyId(obj, $("#hdnGetYearsBySyllabus").val(), ddl, Resources.Year, "Years");
    }



    var checkIsApplied = function () {
        var Status = false;
        var Count = 0;
        $("#IsAppliedCount").html("");
        $(".ChkIsApplied").each(function () {
            if (this.checked == false) {
                Status = true;

            }
            else {
                if ($(this).hasClass("ChkIsAppliedClass")) {
                    Count = Count + 1;
                }
            }

        });

        if (Status == false) {
            $("#IsActive").prop('checked', true);
        }
        else {
            $("#IsActive").prop('checked', false);
        }

        if (Count > 0) {
            $("#IsAppliedCount").html("(" + Count + ")");
        }

        UniversityPaymentBulk.CalculateFee();
    }

    function deleteUniversityFeeOneByOneBulk(rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationDocument,
            actionUrl: $("#hdnDeleteUniversityFeeOneByOne").val(),
            actionValue: rowkey,
            dataRefresh: function (Result) {
                $('[key="' + rowkey + '"] .paymentBankClassDisabled').addClass("paymentBankClass");
                $('[key="' + rowkey + '"] .examTermClassDisabled').addClass("examTermClass");

                $('[key="' + rowkey + '"] .paymentBankClass').removeClass("paymentBankClassDisabled");
                $('[key="' + rowkey + '"] .examTermClass').addClass("examTermClassDisabled");

                $('[key="' + rowkey + '"] .paymentAppliedDateClassDisabled').addClass("paymentAppliedDateClass");
                $('[key="' + rowkey + '"] .paymentAppliedDateClass').removeClass("paymentBankClassDisabled");

                $('[key="' + rowkey + '"] input').removeAttr("disabled");
                $('[key="' + rowkey + '"] select').removeAttr("disabled");

                $('[key="' + rowkey + '"] input[type="text"]').val("");
                $('[key="' + rowkey + '"] select').val("");
                $('[key="' + rowkey + '"] .rowKeyClass').val(0);
                $('[key="' + rowkey + '"] .ScheduleStatusClass').val("False");

                $('[key="' + rowkey + '"] .ChkIsAppliedDisabledClass').prop('checked', false);
                $('[key="' + rowkey + '"] .ChkIsAppliedDisabledClass').addClass("ChkIsAppliedClass");
                $('[key="' + rowkey + '"] .ChkIsAppliedClass').removeClass("ChkIsAppliedDisabledClass");

                $('[deleteKey="' + rowkey + '"]').remove();

                UniversityPaymentBulk.CheckIsApplied();
            }

        });

    }
    var checkPaymentMode = function (PayMode) {

        $(".dvPaymentModeSub").hide();
        switch (parseInt(PayMode || 0)) {

            //case 2:
            //    $(".divCardDetails").show();
            //    $(".divBankAccount,.divChequeDetails").hide();

            //    break;
            //case 3:
            //    $(".divBankAccount").show();
            //    $(".divCardDetails,.divChequeDetails").hide();
            //    break;
            //case 4:
            //    $(".divChequeDetails,.divBankAccount").show();
            //    $(".divCardDetails").hide();
            //    break;
            //default:
            //    $(".dvPaymentModes,.divChequeDetails,.divCardDetails,.divBankAccount").hide();
            //    break;

            case 2:
                $(".divBankAccount").show();

                break;
            case 3:
                $(".dvPaymentModeSub").show();
                $(".divBankAccount").show();

                break;

            default:
                $(".dvPaymentModeSub").hide();
                $(".divBankAccount").hide();
                break;
        }
    }
    var checkPaymentModeItem = function (_this) {
        $("#BankAccountKey,#ChequeOrDDNumber,#ChequeOrDDDate,#BankAccountKey").each(function () {
            $(this).val("")
        });
        var PayMode = $(_this).val();
        var item = $(_this).closest("[data-repeater-item]");

        $("[id*=PaymentModeSubKey],[id*=BankAccountKey]", $(item)).val("");

        $(".dvPaymentModes", $(item)).show();
        $(".dvPaymentModeSub", $(item)).hide();
        switch (parseInt(PayMode || 0)) {

            //case 2:
            //    $(".divCardDetails", $(item)).show();
            //    $(".divBankAccount,.divChequeDetails", $(item)).hide();

            //    break;
            //case 3:
            //    $(".divBankAccount", $(item)).show();
            //    $(".divCardDetails,.divChequeDetails", $(item)).hide();
            //    break;
            //case 4:
            //    $(".divChequeDetails,.divBankAccount", $(item)).show();
            //    $(".divCardDetails", $(item)).hide();
            //    break;
            //default:
            //    $(".dvPaymentModes,.divChequeDetails,.divCardDetails,.divBankAccount", $(item)).hide();
            //    break;



            case 2:
                $(".divChequeDetails,.divBankAccount", $(item)).show();
                $(".divCardDetails,.divReferenceNo", $(item)).hide();

                break;
            case 3:
                $(".dvPaymentModeSub", $(item)).show();
                $(".divBankAccount", $(item)).show();
                $(".divCardDetails,.divChequeDetails,.divReferenceNo", $(item)).hide();
                break;

            default:
                $(".dvPaymentModes,.divChequeDetails,.divCardDetails,.divBankAccount,.divReferenceNo", $(item)).hide();
                break;
        }
    }

    var checkPaymentModeSubItem = function (_this) {


        var PayMode = $(_this).val();
        var item = $(_this).closest("[data-repeater-item]");
        $(".dvPaymentModes", $(item)).show();

        switch (parseInt(PayMode || 0)) {
            case 1:
                $(".divChequeDetails,.divBankAccount", $(item)).show();
                $(".divCardDetails,.divReferenceNo", $(item)).hide();

                break;
            case 2:
                $(".divCardDetails,.divBankAccount", $(item)).show();
                $(".divChequeDetails,.divReferenceNo", $(item)).hide();
                break;

            default:

                $(".dvPaymentModes,.divBankAccount,.divReferenceNo", $(item)).show();

                $(".divChequeDetails,.divCardDetails", $(item)).hide();
                break;
        }

    }

    var checkFeeTypeMode = function (_this) {

        var obj = {};
        //var item = $(_this).closest("[data-repeater-item]");


        //obj.id = $("select[id*=FeeTypeKey]", item).val();
        //obj.id = parseInt(obj.id) ? parseInt(obj.id) : 0;
        obj.id = parseInt($(_this).val()) ? parseInt($(_this).val()) : 0;


        //obj.applicationkey = $("#ApplicationKey").val();
        //obj.Year = $("select[id*=UniversityPaymentYear]", item).val();
        //obj.Year = parseInt(obj.Year) ? parseInt(obj.Year) : 0;

        $.ajax({
            url: $("#hdnCheckFeeTypeModeBulk").val(),
            type: "GET",
            dataType: "JSON",
            //data: { id: Id,year:obj.Year,applicationkey:applicationKey },
            data: obj,
            success: function (result) {

                $("#IsFeeTypeYear").val(result.IsFeeTypeYear);
                //$("[id*=UniversityPaymentAmount]", $(item)).val(result.FeeAmount);
                if (result.IsFeeTypeYear == true) {
                    $(".divFeeYear").hide();
                    $("#SearchYearKey").val(null);
                }
                else {
                    $(".divFeeYear").show();
                }

            }
        });
    }


    var getBalance = function () {

        var paymentMode = $("[id*=SearchPaymentModeKey]").val();
        var BankAccountKey = $("[id*=SearchBankKey]").val();
        var RowKey = 0;
        var branchKey = $("#SearchBranchKey").val();
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&RowKey=" + RowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey)

        $("#AccountHeadBalance").val(response);


    }

    var calculateFee = function (_this) {
        var TotalFee = 0;
        $("[data-repeater-item]").each(function () {
            var item = $(this)
            //var IsActive = $("[id*=IsActive]", $(item)).val();
            //if (IsActive) {
            var FeeAmount = $("[id*=UniversityPaymentAmount]", $(item)).val();

            FeeAmount = parseFloat(FeeAmount) ? parseFloat(FeeAmount) : 0;
            TotalFee = TotalFee + FeeAmount;
            $("#TotalUniversityFee").val(TotalFee);
            UniversityPaymentBulk.Checkbalance(item)
        })
        $("#TotalUniversityFee").val(TotalFee);

    }

    var checkbalance = function (_this) {

        var item = $(_this).closest("[data-repeater-item]");
        var paymentModeKey = $("[id*=SearchPaymentModeKey]").val();

        if (Resources.PaymentModeCheque != paymentModeKey) {


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

                    //$.alert({
                    //    type: 'yellow',
                    //    title: Resources.Warning,
                    //    content: Message,
                    //    icon: 'fa fa-check-circle-o-',
                    //    buttons: {
                    //        Ok: {
                    //            text: Resources.Ok,
                    //            btnClass: 'btn-success',
                    //            action: function () {

                    //            }
                    //        }
                    //    }
                    //})
                    toastr.warning(Resources.Warning, Message);
                }
            }
        }
    }

    var formSubmit = function () {
        $('#btnSave').hide();
        var $form = $("#frmUniversityPaymentBulk")
        var formData = $form.serializeArray();
        var url = $form.attr("action");
        var checkdata = $("[name*=IsActive]:checked");
        if ($form.valid()) {

            if (checkdata.length > 0) {

                $.ajax({
                    url: url,
                    type: "POST",
                    data: formData,
                    success: function (test) {
                        if (test.IsSuccessful) {
                            $.alert({
                                type: 'green',
                                title: Resources.Success,
                                content: test.Message,
                                icon: 'fa fa-check-circle-o-',
                                buttons: {
                                    Ok: {
                                        text: Resources.Ok,
                                        btnClass: 'btn-success',
                                        action: function () {
                                            window.location.href = $("#hdnUniversityPaymentDetails").val();
                                            $('#btnSave').show();
                                        }
                                    }
                                }
                            })
                        }
                    },
                    error: function (xhr) {
                        $('#btnSave').show();
                    }
                })
            }
            else {
                $.alert({
                    type: 'orange',
                    title: Resources.Warning,
                    content: 'Please Select atleast One !',
                    icon: 'fa fa-exclamation-circle',
                    buttons: {
                        Ok: {
                            text: Resources.Ok,
                            btnClass: 'btn-warning',
                            action: function () {
                                $('#btnSave').show();
                            }
                        }
                    }
                })

            }
        }
        else {
            $('#btnSave').show();
        }
    }

    return {

        GetUniversityPaymentStudentsList: getUniversityPaymentStudentsList,
        GetCourseTypeBySyllabus: getCourseTypeBySyllabus,
        GetCourseByCourseType: getCourseByCourseType,
        GetUniversityByCourse: getUniversityByCourse,
        GetYearsBySyllabus: getYearsBySyllabus,
        CheckIsApplied: checkIsApplied,
        DeleteUniversityFeeOneByOneBulk: deleteUniversityFeeOneByOneBulk,
        CheckPaymentMode: checkPaymentMode,
        CheckFeeTypeMode: checkFeeTypeMode,
        CheckPaymentModeItem: checkPaymentModeItem,
        CheckPaymentModeSubItem: checkPaymentModeSubItem,
        GetBalance: getBalance,
        Checkbalance: checkbalance,
        CalculateFee: calculateFee,
        FormSubmit: formSubmit

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



function UniversityPaymentPagination() {

    $('#page-selection-down').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = $("#hdnTotalRecords").val();
    var Size = JsonData["PageSize"];
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

    $('#page-selection-down').bootpag({
        total: totalPages,
        page: 1,
        maxVisible: 10
    });

    $('#page-selection-down').on("page", function (event, num) {
        UniversityPayment.GetUniversityPaymentStudentsList(JsonData, num, false);
    });

}


