var request = null;
var ApplicationPersonal = (function () {
    var Containers = {
        form: function () { return $("#frmApplicationPersonal") }

    }
    var formSubmit = function (btn) {



        var form = $(ApplicationPersonal.Containers.form());
        var validator = $(form).validate();
        var validate = $(form).valid();
        $("[disabled=disabled]", $(form)).removeAttr("disabled");
        if (validate) {
            var response = {};
            const formElem = document.querySelector('#frmApplicationPersonal');
            var formdata = new FormData(formElem);

            AjaxHelper.ajaxFromData({
                type: $(form)[0].method,
                url: $(form)[0].action,
                data: formdata,
                loadingCntrl: form,
                success: function (response) {
                    if (response.IsSuccessful == true) {
                        toastr.success(Resources.Success, response.Message);



                        $("#tab-application li").each(function () {
                            var url = $(this).find("a").data("href");
                            $(this).find("a").attr("data-href", url.replace('0', response.RowKey))
                            $(this).show();
                        })
                        if (!response.HasInstallment) {
                            $("#tab-application li a[href='#FeeInstallment']").parent().hide();
                        }
                        if (!response.HasElective) {
                            $("#tab-application li a[href='#ElectiveBook']").parent().hide();
                        }
                        $("#tab-application  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');

                    }
                    else {
                        toastr.error(Resources.Failed, response.Message);
                        $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                    }
                }
            });

        }
        else {
            validator.focusInvalid();
        }

    }
    var getCourseByCourseType = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseByCourseType").val(), ddl, Resources.Select + Resources.BlankSpace + Resources.Course, "Courses");
    }
    var getUniversityByCourse = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetUniversityByCourse").val(), ddl, Resources.University, "Universities");
    }
    var getYearByMode = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetYearByMode").val(), ddl, null, "AdmittedYear");
    }
    var getCurrentYearByYear = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCurrentYearByYear").val(), ddl, null, "CurrentYear");
    }
    var getCourseTypeByAcademicTerm = function (obj, ddl) {

        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseTypeByAcademicTerm").val(), ddl, Resources.CourseType, "CourseTypes");

    }
    var changeAgeLimitRange = function (From, To) {
        var AgeRegex = $("#Age").data("val-range")
        $("#Age").attr("data-val-range", AgeRegex.replace("0", From).replace("100", To));
        $("#Age").attr("data-val-range-min", From)
        $("#Age").attr("data-val-range-max", To)
        $("form").removeData("validator");
        $("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("form");
    }
    var getAdmissionFee = function () {

        var obj = {};
        obj.id = 0;
        obj.CourseKey = $("#CourseKey").val();
        obj.AcademicTermKey = $("#AcademicTermKey").val();
        obj.AdmissionCurrentYear = $("#AdmissionCurrentYear").val();
        obj.UniversityKey = $("#UniversityKey").val();
        if (obj.UniversityKey != null && obj.UniversityKey != "" && obj.UniversityKey != 0) {
            $("#addEditCourseFee").show();
        }
        else {
            $("#addEditCourseFee").hide();
        }
        //  ApplicationPersonal.GetAdmissionFee(obj);
        var url = $("#hdnGetAdmissionFee").val() + "?" + $.param(obj);
        $("#DivCourseFee").load(url, function () {
            CheckConsession($('#HasConcession')[0])
            CheckOffer($('#HasOffer')[0], true)
            CheckoldPaid($('#AllowOldPaid')[0], true)
        })

    }
    var getOfferDetails = function () {

        $("#OfferName").val("")
        $("#OfferValue").val("")
        $("#OfferKey").val("")


        $.ajax({
            url: $("#hdnGetOfferDetails").val(),
            type: "GET",
            dataType: "JSON",
            //data: obj,
            success: function (result) {
                if (result.IsSuccessful) {
                    $("#OfferName").val(result.OfferName)
                    $("#OfferValue").val(result.OfferValue)
                    $("#OfferKey").val(result.OfferKey)

                    ApplicationPersonal.CalculateOffer();
                } else {
                    EduSuite.AlertMessage({ title: Resources.Warning, content: result.Message, type: 'orange' })
                    $("#HasOffer").prop("checked", false).trigger("change");
                }
            }
        });

    }
    function calculateTotal() {

        $(".inter-state").hide();
        $(".intra-state").hide();
        var TotalAdmissionFeeAmount = 0, TotalConcessionAmount = 0, GrandTotalFeeAmount = 0, TotalCGSTAmount = 0, TotalSGSTAmount = 0, TotalIGSTAmount = 0, TotalOldPaidAmount = 0;
        $("#DivCourseFee [data-repeater-item]").each(function (i) {

            var TotalFeeAmount = 0;
            var AdmissionFeeAmount = $("input[id*=ActualAmount]", $(this)).val();
            var ConcessionAmount = $("input[id*=ConcessionAmount]", $(this)).val();
            var OldPaid = $("input[id*=OldPaid]", $(this)).val();
            //var CGSTAmount = $("input[id*=CGSTAmount]", $(this)).val();
            //var SGSTAmount = $("input[id*=SGSTAmount]", $(this)).val();
            //var IGSTAmount = $("input[id*=IGSTAmount]", $(this)).val();
            //var CessAmount = $("input[id*=CessAmount]", $(this)).val();
            //var CGSTRate = $("input[id*=CGSTRate]", $(this));
            //var SGSTRate = $("input[id*=SGSTRate]", $(this));
            //var IGSTRate = $("input[id*=IGSTRate]", $(this));
            //var CessRate = $("input[id*=CessRate]", $(this));


            AdmissionFeeAmount = AdmissionFeeAmount != "" ? parseFloat(AdmissionFeeAmount) : 0;
            ConcessionAmount = ConcessionAmount != "" ? parseFloat(ConcessionAmount) : 0;
            OldPaid = OldPaid != "" ? parseFloat(OldPaid) : 0;
            if (ConcessionAmount > AdmissionFeeAmount) {
                ConcessionAmount = AdmissionFeeAmount;
                $("input[id*=ConcessionAmount]", $(this)).val(ConcessionAmount)
            }
            //CGSTAmount = CGSTAmount != "" ? parseFloat(CGSTAmount) : 0;
            //SGSTAmount = SGSTAmount != "" ? parseFloat(SGSTAmount) : 0;
            //IGSTAmount = IGSTAmount != "" ? parseFloat(IGSTAmount) : 0;
            //CessAmount = CessAmount != "" ? parseFloat(CessAmount) : 0;
            //CGSTRate = parseFloat($(CGSTRate).val()) ? parseFloat($(CGSTRate).val()) : 0;
            //SGSTRate = parseFloat($(SGSTRate).val()) ? parseFloat($(SGSTRate).val()) : 0;
            //IGSTRate = parseFloat($(IGSTRate).val()) ? parseFloat($(IGSTRate).val()) : 0;
            //CessRate = parseFloat($(CessRate).val()) ? parseFloat($(CessRate).val()) : 0;



            //var GStRate = CGSTRate + SGSTRate;

            //var GstAmount = AdmissionFeeAmount * GStRate / 100;
            //CessAmount = AdmissionFeeAmount * CessRate / 100;
            //CessAmount = CessAmount.toFixed(2);
            //GstAmount = GstAmount != "" ? parseFloat(GstAmount) : 0;          
            //CGSTAmount = (GstAmount / 2).toFixed(2);
            //SGSTAmount = (GstAmount / 2).toFixed(2);
            //CGSTAmount = CGSTAmount != "" ? parseFloat(CGSTAmount) : 0;
            //SGSTAmount = SGSTAmount != "" ? parseFloat(SGSTAmount) : 0;
            //CessAmount = CessAmount != "" ? parseFloat(CessAmount) : 0;

            if (OldPaid > AdmissionFeeAmount) {
                OldPaid = AdmissionFeeAmount;
                $("input[id*=OldPaid]", $(this)).val(OldPaid)
            }

            TotalAdmissionFeeAmount = TotalAdmissionFeeAmount + AdmissionFeeAmount;
            TotalConcessionAmount = TotalConcessionAmount + ConcessionAmount;
            TotalOldPaidAmount = TotalOldPaidAmount + OldPaid;

            //TotalCGSTAmount = TotalCGSTAmount + CGSTAmount;
            //TotalSGSTAmount = TotalSGSTAmount + SGSTAmount;
            //TotalIGSTAmount = TotalIGSTAmount + IGSTAmount;
            // TotalCessAmount = TotalCessAmount + CessAmount;





            //if ((CGSTAmount != null && CGSTAmount != 0) && (SGSTAmount != null && SGSTAmount != 0) && (ConcessionAmount != null && ConcessionAmount != 0)) {
            //    TotalFeeAmount = TotalFeeAmount + CGSTAmount + SGSTAmount + AdmissionFeeAmount - ConcessionAmount;
            //    $(".inter-state").hide();
            //    $(".intra-state").show();
            //    $("[id*=IGSTAmount]", $(this)).val(0);
            //    $("[id*=IGSTRate]", $(this)).val(0);
            //}
            //else {
            //    if ((CGSTAmount != null && CGSTAmount != 0) && (SGSTAmount != null && SGSTAmount != 0)) {
            //        TotalFeeAmount = TotalFeeAmount + CGSTAmount + SGSTAmount + AdmissionFeeAmount;// + CessAmount;
            //        $(".inter-state").hide();
            //        $(".intra-state").show();
            //        $("[id*=IGSTAmount]", $(this)).val(0);
            //        //$("[id*=IGSTRate]", $(this)).val(0);
            //    }
            //    else if (ConcessionAmount != null && ConcessionAmount != 0) {
            //        TotalFeeAmount = TotalFeeAmount + AdmissionFeeAmount - ConcessionAmount;
            //        $("[id*=CGSTAmount]", $(this)).val(0);
            //        //$("[id*=CGSTRate]", $(this)).val(0);
            //        $("[id*=SGSTAmount]", $(this)).val(0);
            //        //$("[id*=SGSTRate]", $(this)).val(0);
            //        $("[id*=IGSTAmount]", $(this)).val(0);
            //        //$("[id*=IGSTRate]", $(this)).val(0);
            //        //$("[id*=CessAmount]", $(this)).val(0);
            //        //$("[id*=CessRate]", $(this)).val(0);
            //    }
            //    else {
            //        TotalFeeAmount = TotalFeeAmount + AdmissionFeeAmount;
            //        $("[id*=CGSTAmount]", $(this)).val(0);
            //        //$("[id*=CGSTRate]", $(this)).val(0);
            //        $("[id*=SGSTAmount]", $(this)).val(0);
            //        //$("[id*=SGSTRate]", $(this)).val(0);
            //        $("[id*=IGSTAmount]", $(this)).val(0);
            //        //$("[id*=IGSTRate]", $(this)).val(0);
            //        //$("[id*=CessAmount]", $(this)).val(0);
            //        //$("[id*=CessRate]", $(this)).val(0);
            //    }
            //}

            if (ConcessionAmount != null && ConcessionAmount != 0) {
                TotalFeeAmount = TotalFeeAmount + AdmissionFeeAmount - ConcessionAmount;

            }
            else {
                TotalFeeAmount = TotalFeeAmount + AdmissionFeeAmount;

            }
            //$("input[id*=CGSTAmount]", $(this)).val(CGSTAmount);
            //$("input[id*=SGSTAmount]", $(this)).val(SGSTAmount);
            //$("input[id*=IGSTAmount]", $(this)).val(IGSTAmount);
            //$("input[id*=CessAmount]", $(this)).val(CessAmount);

            GrandTotalFeeAmount = GrandTotalFeeAmount + TotalFeeAmount
            //FeeAmountRupeeTotal = FeeAmountRupeeTotal + AmountRupee;
            //var yearText = AppCommon.YearNumberToText($("input[id*=AdmissionFeeYear]", $(this)).val())
            //$("label[id*=lblFeeYearName]", $(this)).html(yearText);
            $("input[id*=AdmissionFeeAmount]", $(this)).val(TotalFeeAmount);

        });
        $("#TotalAdmissionFeeAmount").val(TotalAdmissionFeeAmount);
        $("#TotalConcessionAmount").val(TotalConcessionAmount);
        $("#TotalOldPaidAmount").val(TotalOldPaidAmount);
        //$("#TotalCGSTAmount").val(TotalCGSTAmount);
        //$("#TotalSGSTAmount").val(TotalSGSTAmount);
        //$("#TotalCessAmount").val(TotalCessAmount);


        $("#GrandTotalFeeAmount").val(GrandTotalFeeAmount);
        //if (GrandTotalFeeAmount == 0) {
        //    $("#DivAddCourseFee").show();
        //}


    }
    function calculateOffer() {

        var OfferValue = $("#OfferValue").val();

        var TotalAdmissionFeeAmount = 0, TotalConcessionAmount = 0, GrandTotalFeeAmount = 0, TotalCGSTAmount = 0, TotalSGSTAmount = 0;
        $("#DivCourseFee [data-repeater-item]").each(function (i) {
            var TotalFeeAmount = 0;
            var AdmissionFeeAmount = $("input[id*=ActualAmount]", $(this)).val();
            var ConcessionAmount = $("input[id*=ConcessionAmount]", $(this)).val();
            //var CGSTAmount = $("input[id*=CGSTAmount]", $(this)).val();
            //var SGSTAmount = $("input[id*=SGSTAmount]", $(this)).val();
            //var IGSTAmount = $("input[id*=IGSTAmount]", $(this)).val();

            //$(".inter-state").hide();
            //$(".intra-state").hide();

            AdmissionFeeAmount = AdmissionFeeAmount != "" ? parseFloat(AdmissionFeeAmount) : 0;
            ConcessionAmount = ConcessionAmount != "" ? parseFloat(ConcessionAmount) : 0;
            ConcessionAmount = OfferValue / 100 * AdmissionFeeAmount;
            //CGSTAmount = CGSTAmount != "" ? parseFloat(CGSTAmount) : 0;
            //SGSTAmount = SGSTAmount != "" ? parseFloat(SGSTAmount) : 0;
            //IGSTAmount = IGSTAmount != "" ? parseFloat(IGSTAmount) : 0;

            $("input[id*=ConcessionAmount]", $(this)).val((ConcessionAmount != 0 ? ConcessionAmount : ""));
            TotalAdmissionFeeAmount = TotalAdmissionFeeAmount + AdmissionFeeAmount;
            TotalConcessionAmount = TotalConcessionAmount + ConcessionAmount;
            //TotalCGSTAmount = TotalCGSTAmount + CGSTAmount;
            //TotalSGSTAmount = TotalSGSTAmount + SGSTAmount;
            //TotalIGSTAmount = TotalIGSTAmount + IGSTAmount;

            //if ((CGSTAmount != null && CGSTAmount != 0) && (SGSTAmount != null && SGSTAmount != 0) && (ConcessionAmount != null && ConcessionAmount != 0)) {
            //    TotalFeeAmount = TotalFeeAmount + CGSTAmount + SGSTAmount + AdmissionFeeAmount - ConcessionAmount;
            //    $(".inter-state").hide();
            //    $(".intra-state").show();
            //}
            //else {
            //    if ((CGSTAmount != null && CGSTAmount != 0) && (SGSTAmount != null && SGSTAmount != 0)) {
            //        TotalFeeAmount = TotalFeeAmount + CGSTAmount + SGSTAmount + AdmissionFeeAmount;
            //        $(".inter-state").hide();
            //        $(".intra-state").show();
            //    }

            //    else if (ConcessionAmount != null && ConcessionAmount != 0) {
            //        TotalFeeAmount = TotalFeeAmount + AdmissionFeeAmount - ConcessionAmount;
            //        $("[id*=CGSTAmount]", $(this)).val(null);
            //        $("[id*=CGSTRate]", $(this)).val(null);
            //        $("[id*=SGSTAmount]", $(this)).val(null);
            //        $("[id*=SGSTRate]", $(this)).val(null);
            //        $("[id*=IGSTAmount]", $(this)).val(null);
            //        $("[id*=IGSTRate]", $(this)).val(null);
            //    }
            //    else {
            //        TotalFeeAmount = TotalFeeAmount + AdmissionFeeAmount;
            //        $("[id*=CGSTAmount]", $(this)).val(null);
            //        $("[id*=CGSTRate]", $(this)).val(null);
            //        $("[id*=SGSTAmount]", $(this)).val(null);
            //        $("[id*=SGSTRate]", $(this)).val(null);
            //        $("[id*=IGSTAmount]", $(this)).val(null);
            //        $("[id*=IGSTRate]", $(this)).val(null);
            //    }


            //}
            if (ConcessionAmount != null && ConcessionAmount != 0) {
                TotalFeeAmount = TotalFeeAmount + AdmissionFeeAmount - ConcessionAmount;

            }
            else {
                TotalFeeAmount = TotalFeeAmount + AdmissionFeeAmount;

            }
            GrandTotalFeeAmount = GrandTotalFeeAmount + TotalFeeAmount
            //FeeAmountRupeeTotal = FeeAmountRupeeTotal + AmountRupee;
            $("input[id*=AdmissionFeeAmount]", $(this)).val(TotalFeeAmount);

        });
        $("#TotalAdmissionFeeAmount").val(TotalAdmissionFeeAmount);
        $("#TotalConcessionAmount").val(TotalConcessionAmount);
        //$("#TotalCGSTAmount").val(TotalCGSTAmount);
        //$("#TotalSGSTAmount").val(TotalSGSTAmount);
        $("#GrandTotalFeeAmount").val(GrandTotalFeeAmount);


    }
    var courseFeepopup = function () {
        var obj = {};
        obj.CourseKey = $("#CourseKey").val();
        obj.AcademicTermKey = $("#AcademicTermKey").val();
        obj.StartYear = $("#AdmissionCurrentYear").val();
        obj.UniversityKey = $("#UniversityKey").val();
        obj.id = $("#UniversityCourseKey").val();

        var URL = $("#hdnAddEditUniversityCourseAmount").val() + "?id=" + obj.id;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function (popup) {
                setTimeout(function () {
                    //UniversityCourse.GetUniversityCourseAmount();
                    AppCommon.CustomRepeaterRemoteMethod();
                    $("[data-repeater-item]", $(popup)).each(function () {

                        var CenterShareAmountPer = $("[id*=CenterShareAmountPer]", this);
                        var Isuniversity = $("[id*=IsUniversity]", this).val();

                        // if value become a bool when convert to int then check
                        Isuniversity = JSON.parse(Isuniversity.toLowerCase()) ? JSON.parse(Isuniversity.toLowerCase()) : false



                        if (!Isuniversity) {
                            $(CenterShareAmountPer).attr("readonly", true)
                            $(CenterShareAmountPer).val(0)

                        }
                        else {
                            $(CenterShareAmountPer).removeAttr("readonly")
                        }
                    })
                }, 500)
            },
            rebind: function (result) {
                ApplicationPersonal.GetAdmissionFee();
            }
        }, URL);
    }
    var checkMobileNo = function (_this) {

        var obj = {};
        obj.id = 0;
        obj.RowKey = $("#RowKey").val();
        obj.MobileNumber = $(_this).val();
        var span = $(_this).next("span")

        var validator = $("form").validate({


        });


        request = $.ajax({
            url: $("#hdnCheckPhoneExists").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            beforeSend: function () {
                if (request != null) {
                    request.abort();
                }
                validator.element(_this);
            },
            success: function (result) {
                if (!result.IsSuccessful) {

                    span.html('<span class="field-validation-error" data-valmsg-for="MobileNumber" data-valmsg-replace="true">' + result.Message + '</span>')

                }
            }
        });

    }

    var checkSecondLanguage = function () {
        
        var id = $("#CourseTypeKey").val();
        var response = AjaxHelper.ajax("Get", $("#hdnCheckSecondLanguage").val() + "?id=" + id);
        if (response) {
            $("#DivSecondLanguage").show();
        } else {
            $("#DivSecondLanguage").hide();
        }
    }
    return {
        FormSubmit: formSubmit,
        GetCourseByCourseType: getCourseByCourseType,
        GetUniversityByCourse: getUniversityByCourse,
        GetYearByMode: getYearByMode,
        ChangeAgeLimitRange: changeAgeLimitRange,
        GetAdmissionFee: getAdmissionFee,
        GetOfferDetails: getOfferDetails,
        CalculateTotal: calculateTotal,
        CalculateOffer: calculateOffer,
        //GetclassDetails: getclassDetails,
        CourseFeepopup: courseFeepopup,
        GetCourseTypeByAcademicTerm: getCourseTypeByAcademicTerm,
        CheckMobileNo: checkMobileNo,
        GetCurrentYearByYear: getCurrentYearByYear,
        Containers: Containers,
        CheckSecondLanguage: checkSecondLanguage

    }
}());

function CheckOffer(_this, IsEvent) {

    if (_this.checked) {
        $("input#HasConcession:checked").prop('checked', false).trigger("change")
        $(".OfferDetails").show();
        $(".ConcessionCoulumns").show();


        var obj = {};
        if (IsEvent)
            ApplicationPersonal.GetOfferDetails();

    }
    else {
        $(".OfferDetails").hide();
        $(".ConcessionCoulumns input[type=text]").each(function () {
            $(this).val("");


        })
        ApplicationPersonal.CalculateTotal();
        $(".ConcessionCoulumns").hide();
    }
    $("#DivCourseFee [data-repeater-item]").each(function () {
        $("[id*=HasConcession]", $(this)).val(_this.checked.toString());
    })
}

function CheckConsession(_this, IsEvent) {

    if (_this.checked) {
        $("input#HasOffer:checked").prop('checked', false).trigger("change")
        $(".ConcessionCoulumns").show();

    }
    else {
        $(".ConcessionCoulumns input[type=text]").each(function () {
            $(this).val("");
        })
        ApplicationPersonal.CalculateTotal();
        $(".ConcessionCoulumns").hide();
    }
    $("#DivCourseFee [data-repeater-item]").each(function () {
        $("[id*=HasConcession]", $(this)).val(_this.checked.toString());
    })
}


function CheckoldPaid(_this, IsEvent) {

    if (_this.checked) {

        $(".OldPaidColumns").show();

    }
    else {
        $(".OldPaidColumns input[type=text]").each(function () {
            $(this).val("");
        })
        ApplicationPersonal.CalculateTotal();
        $(".OldPaidColumns").hide();
    }
    //$("#DivCourseFee [data-repeater-item]").each(function () {
    //    $("[id*=AllowOldPaid]", $(this)).val(_this.checked.toString());
    //})
}