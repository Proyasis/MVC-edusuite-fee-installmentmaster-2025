var JsonData = [], request = null;
var Enquiry = (function () {

    var getEnquiries = function (json, pageIndex, resetPagination) {
        JsonData = json;
        $("#dvRepeaterList").empty();
        $("#dvScheduleContainer").mLoading();
        JsonData["SearchName"] = $("#SearchName").val();
        JsonData["SearchPhone"] = $("#SearchPhone").val();
        JsonData["SearchEmail"] = $("#SearchEmail").val();
        JsonData["SearchFromDate"] = $("#SearchFromDate").val();
        JsonData["SearchToDate"] = $("#SearchToDate").val();
        JsonData["SearchCallStatusKey"] = $("#SearchCallStatusKey").val();
        JsonData["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonData["SearchDepartmentKey"] = $("#SearchDepartmentKey").val();
        JsonData["SearchAcademicTermKey"] = $("#SearchAcademicTermKey").val();
        JsonData["SearchEmployeeKey"] = $("#SearchEmployeeKey").val();
        JsonData["SearchEnquiryStatusKey"] = $("#SearchEnquiryStatusKey").val();
        JsonData["SearchLocation"] = $("#SearchLocation").val();
        JsonData["NatureOfEnquiryKey"] = $("#NatureOfEnquiryKey").val();

        JsonData["TabKey"] = $("#ScheduleTab .active").attr("value");
        JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonData["PageSize"] = 10;
        request = $.ajax({
            url: $("#hdnGetEnquiries").val(),
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
                $("#TotalRecords").html($("#hdnTotalRecords").val());

                $('#ScheduleTab li a[value="0"] .badge').html($("#hdnEnquiryProccessingCount").val());
                $('#ScheduleTab li a[value="1"] .badge').html($("#hdnEnquiryInboxCount").val());
                $('#ScheduleTab li a[value="2"] .badge').html($("#hdnEnquiryOutboxCount").val());

                if (resetPagination) {
                    EnquiryPagination();
                }
                $("#dvScheduleContainer").mLoading("destroy");
            },
            error: function (error) {
                console.log(JSON.stringify(error));
                // $("#dvScheduleContainer").mLoading("destroy");
            }
        });

    }

    var getEnquiryFeadbacks = function (_this, url) {
        $(_this).mLoading();
        setTimeout(function () {
            $.ajax({
                url: url,
                datatype: "json",
                type: "GET",
                contenttype: 'application/json; charset=utf-8',
                async: false,
                success: function (data) {
                    $(_this).html(data);
                    $(_this).mLoading("destroy");
                },
                error: function (xhr) {
                    $(_this).mLoading("destroy");
                }
            });
        }, 500);

    }

    var editEnquiryFeedback = function (key, EnquiryKey) {
        var obj = {};
        obj.id = key;
        obj.EnquiryKey = EnquiryKey;
        $("#dvAddEditEnquiryFeedback").load($("#hdnAddEditEnquiryFeedback").val() + "?" + $.param(obj));
    }

    var formSubmitEnquiryFeedback = function () {
        var form = $("#frmEnquiryFeedback");
        var validate = $(form).valid();
        if (validate) {
            $.ajax({
                url: $(form)[0].action,
                type: $(form)[0].method,
                data: $(form).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {


                        var collapse = $(form).closest(".collapse");
                        var item = $(form).closest("[data-repeater-item]");
                        var url = $("[data-toggle='collapse']", $(item)).data("url");
                        Enquiry.GetEnquiryFeadbacks(collapse, url);


                    } else {

                    }
                }
            });
        }
    }

    var deleteEnquiry = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnDeleteEnquiry").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                window.location.reload();
            }
        });
    }

    var deleteEnquiryFeedback = function (rowkey, _this) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_EnquiryFeedback,
            actionUrl: $("#hdnDeleteEnquiryFeedback").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                var collapse = $(_this).closest(".EnquiryFeedback");
                var item = $(_this).closest("[data-repeater-item]");
                var url = $("[data-toggle='collapse']", $(item)).data("url");
                Enquiry.GetEnquiryFeadbacks(collapse, url);
            }
        });
    }

    var getDepartmentsByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetDepartmentsByBranchId").val(), ddl, Resources.Department, "Departments");

    }

    var getCourseTypeByAcademicTerm = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnCourseTypeByAcademicTerm").val(), ddl, Resources.CourseType, "CourseTypes");
    }

    var getCourseByCourseType = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseByCourseType").val(), ddl, Resources.Course, "Courses");
    }

    var getUniversityByCourse = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetUniversityByCourse").val(), ddl, Resources.University, "Universities");

    }


    var getCallStatusByEnquiryStatus = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCallStatusByEnquiryStatus").val(), ddl, Resources.CallStatus);
    }

    var checkCallStatusDuration = function (Id) {
        if (Id == Resources.EnquiryCallStatusCounselling) {
            $("#ConcellingTimeKey").removeAttr("disabled");
        }
        else {
            $("#ConcellingTimeKey").val("")
            $("#ConcellingTimeKey").attr("disabled", true);
        }
        $("#ConcellingTimeKey").selectpicker('refresh');
        var validator = $($("#ConcellingTimeKey").closest("form")).validate();
        validator.element("#ConcellingTimeKey");
        var response = AjaxHelper.ajax("GET", $("#hdnCheckCallStatusDuration").val() + "/" + Id);
        $("#IsDuration").val(response);

    }

    var editFeedbackPopUp = function (_this) {
        var URL = $(_this).data("url");
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                EnquiryFeedbackPopupReady();
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    toastr.success(Resources.Success, result.Message);
                }
                else {
                    toastr.error(Resources.Failed, result.Message);
                }
                if (result.Feedback != "") {
                    if (result.IsUserBlocked == true) {
                        $.confirm({
                            title: Resources.Warning,
                            content: result.Feedback,
                            type: 'red',
                            buttons: {
                                Ok: {
                                    text: 'Ok',
                                    btnClass: 'btn-danger',
                                    action: function () {
                                        window.location.href = $("#hdnLogin").val();
                                    }
                                }
                            }
                        });
                    }

                    toastr.warning(Resources.Warning, result.Feedback);
                }

                if (typeof EnquirySchedule != 'undefined') {
                    EnquirySchedule.GetEnquirySchedules(null, 1, true, 0)
                } else {
                    Enquiry.GetEnquiries(jsonData);
                }

            },
            formAction: $("#hdnAddEditEnquiryFeedback").val()
        }, URL);

    }


    var getEmployeesByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetEmployeesByBranchId").val(), ddl, Resources.Employee);

    }

    var requestAutoComplete = null;

    var getLeadByText = function (object) {
        var obj = {};
        $("#LeadLink").remove();
        $("<a id='LeadLink' href='#'></a>").insertAfter(object);

        if (object.value == "") {
            $("#LeadList").remove();
        }
        else {
            obj.SearchText = object.value;

            requestAutoComplete = $.ajax({
                url: $("#hdnGetEnquiryLead").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                contentType: "application/json; charset=utf-8",
                async: true,
                beforeSend: function () {
                    if (requestAutoComplete != null) {
                        requestAutoComplete.abort();
                    }
                },
                success: function (List) {
                    LeadDataList = List;
                    $("#LeadList").remove();
                    $("<ul class='AutoCompleteList' id='LeadList'></ul>").insertAfter($(object).closest("div.form-group"));
                    BindLeadList(object)

                }
            });
        }
    }


    var getEnquiryByText = function (object) {
        var obj = {};
        //$("#LeadLink").remove();
        //$("<a id='LeadLink' href='#'></a>").insertBefore(object);

        if (object.value == "") {
            $("#EnquiryList").remove();
        }
        else {
            obj.SearchText = object.value;
            requestAutoComplete = $.ajax({
                url: $("#hdnGetEnquiry").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                async: true,
                contentType: "application/json; charset=utf-8",
                beforeSend: function () {
                    if (requestAutoComplete != null) {
                        requestAutoComplete.abort();
                    }
                },
                success: function (List) {
                    EnquiryDataList = List;
                    $("#EnquiryList").remove();
                    $("<ul class='AutoCompleteList' id='EnquiryList'></ul>").insertAfter($(object).closest("div.form-group"));
                    BindEnquiryList(object)

                }
            });
        }
    }


    function BindLeadList(object) {
        for (i = 0; i < LeadDataList.length; i++) {
            $("#LeadList").append("<li onClick='GetLink(" + i + ")' class='ListItem'>" + LeadDataList[i].Name + " (" + LeadDataList[i].EmployeeName + ") </li>");
        }
    }


    function BindEnquiryList(object) {
        for (i = 0; i < EnquiryDataList.length; i++) {
            $("#EnquiryList").append("<li onClick='FetchEnquiry(" + i + ")' class='ListItem'>" + EnquiryDataList[i].EnquiryName + " (" + EnquiryDataList[i].EmployeeName + ") </li>");
        }
    }

    var formSubmit = function (btn) {

        var form = $(btn).closest("form")[0];
        var url = $(form)[0].action;
        var validator = $(form).validate();
        
        if ($(form).valid()) {
            $("#btnSave").hide();
            $(".section-content").mLoading();
            $("[disabled=disabled]", $(form)).removeAttr("disabled");
            var data = $(form).serializeArray();
            delete data[0];
            setTimeout(function () {
                var response = AjaxHelper.ajax("POST", url,
                    {
                        model: AppCommon.ObjectifyForm(data)
                    });

                if (response.IsSuccessful == true) {

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
                                    window.location.href = $("#hdnEnquiryList").val();
                                }
                            }
                        }
                    })

                }
                else {
                    $("#btnSave").show();
                    toastr.error(Resources.Failed, response.Message);
                    $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                }
                $(".section-content").mLoading("destroy");
            }, 500)
        }
        else {
            validator.focusInvalid();
        }

    }

    var getPhoneNumberLength = function () {
        var response = AjaxHelper.ajax("GET", $("#hdnGetPhoneNumberLength").val() + "?TelephoneCodeKey=" + $("#TelephoneCodeKey").val() + "&TelephoneCodeOptionalKey=" + $("#TelephoneCodeOptionalKey").val());
        //var response = AjaxHelper.ajax("GET", $("#hdnGetPhoneNumberLength").val() + "?MobileNumber=" + mobilenumber + "&TelephoneCodeKey=" + TelephoneCodeKey)

        $("[id*=MinPhoneLength]").val(response.MinPhoneLength);
        $("[id*=MaxPhoneLength]").val(response.MaxPhoneLength);
        $("[id*=MinPhoneLengthOptional]").val(response.MinPhoneLengthOptional);
        $("[id*=MaxPhoneLengthOptional]").val(response.MaxPhoneLengthOptional);
    }

    return {
        GetEnquiries: getEnquiries,
        GetEnquiryFeadbacks: getEnquiryFeadbacks,
        EditEnquiryFeedback: editEnquiryFeedback,
        FormSubmitEnquiryFeedback: formSubmitEnquiryFeedback,
        DeleteEnquiry: deleteEnquiry,
        DeleteEnquiryFeedback: deleteEnquiryFeedback,
        GetDepartmentsByBranchId: getDepartmentsByBranchId,
        GetCourseTypeByAcademicTerm: getCourseTypeByAcademicTerm,
        GetCourseByCourseType: getCourseByCourseType,
        GetUniversityByCourse: getUniversityByCourse,
        EditFeedbackPopUp: editFeedbackPopUp,
        GetCallStatusByEnquiryStatus: getCallStatusByEnquiryStatus,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        GetLeadByText: getLeadByText,
        GetEnquiryByText: getEnquiryByText,
        CheckCallStatusDuration: checkCallStatusDuration,
        FormSubmit: formSubmit,
        GetPhoneNumberLength: getPhoneNumberLength

    }
}());





function EnquiryPagination() {
    $('#page-selection-up,#page-selection-down').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = jsonData["PageSize"];
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

    $('#page-selection-up,#page-selection-down').bootpag({
        total: totalPages,
        page: 1,
        maxVisible: 10
    });
    $('#page-selection-up').on("page", function (event, num) {
        Enquiry.GetEnquiries(JsonData, num, false);
    });

}

var LeadDataList;
function GetLink(index) {
    $("#EnquiryName").val("");
    $("#MobileNumber").val(LeadDataList[index].MobileNumber);
    $("#TelephoneCodeKey").val(LeadDataList[index].TelephoneCodeKey);
    $("#MobileNumberOptional").val(LeadDataList[index].TelephoneCodeOptionalKey);
    $("#PhoneNumber").val(LeadDataList[index].PhoneNumber);
    $("#EmailAddress").val(LeadDataList[index].EmailAddress);
    $("#EnquiryEducationQualification").val("");
    $("#EnquiryLeadKey").val("");

    $("#LeadList").remove();
    $("#LeadLink").attr("onclick", "FetchLead(" + index + ")");
    $("#LeadLink").html("Fetch Lead (" + LeadDataList[index].Name + ")");
}

function FetchLead(index) {
    $("#EnquiryName").val(LeadDataList[index].Name);
    $("#MobileNumber").val(LeadDataList[index].MobileNumber);
    $("#TelephoneCodeKey").val(LeadDataList[index].TelephoneCodeKey);
    $("#MobileNumberOptional").val(LeadDataList[index].TelephoneCodeOptionalKey);
    $("#PhoneNumber").val(LeadDataList[index].PhoneNumber);
    $("#EmailAddress").val(LeadDataList[index].EmailAddress);
    $("#EnquiryEducationQualification").val(LeadDataList[index].Qualification);
    $("#EnquiryLeadKey").val(LeadDataList[index].RowKey);
    $("#NatureOfEnquiryKey").val(Resources.NatureOfEnquiryLead);

}


var EnquiryDataList;
function FetchEnquiry(index) {
    $("#EnquiryList").remove();
    $("#SearchName").val(EnquiryDataList[index].EnquiryName);
    $("#SearchEnquiryStatusKey").val(EnquiryDataList[index].EnquiryStatusKey);
    Enquiry.GetEnquiries(jsonData);
    $("#SearchEnquiryStatusKey").val("");
}

$(document).on('click', 'body', function (e) {
    if (e.target.id != "SearchName") {
        $("#EnquiryList").remove();
    }

    if (e.target.id != "EnquiryName" || e.target.id != "EnquiryPhone") {
        $("#LeadList").remove();
    }

});


function EnquiryFeedbackPopupReady() {
    AppCommon.FormatDateInput();
    AppCommon.FormatTimeInput();
    AppCommon.FormatDurationInput();
    AppCommon.FormatInputCase();

    $("#EnquiryStatusKey").on("change", function () {
        if (this.value == "4") {
            $("#NextCallSchedule").val("");
            $("#NextCallSchedule").attr("disabled", true)
        }
        else {
            $("#NextCallSchedule").removeAttr("disabled");
        }
        var validator = $($(this).closest("form")).validate();
        validator.element("#NextCallSchedule");
        var obj = {};
        obj.EnquiryStatusKey = $("#EnquiryStatusKey").val();
        Enquiry.GetCallStatusByEnquiryStatus(obj, $("#EnquiryCallStatusKey"));
    });
    $("#EnquiryCallStatusKey").on("change", function () {
        Enquiry.CheckCallStatusDuration($(this).val());
    });



}