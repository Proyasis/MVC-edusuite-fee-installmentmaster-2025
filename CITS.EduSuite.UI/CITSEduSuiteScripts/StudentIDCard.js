var JsonData = [], request = null;
var StudentIDCards = (function () {

    var getStudentIDCardDetails = function (json) {

        //$('.repeater').repeater(
        //    {
        //        show: function () {
        //            $(this).slideDown();
        //            AppCommon.CustomRepeaterRemoteMethod();
        //            //$("[id*=HasBook][type=checkbox]", $(this)).prop("checked", true);
        //            //$("[id*=IsActive][type=checkbox]", $(this)).prop("checked", true);
        //        },
        //        hide: function (remove)
        //        {
        //            var self = $(this).closest('[data-repeater-item]').get(0);
        //            var hidden = $(self).find('input[type=hidden]')[0];

        //        },
        //        rebind: function (response) {
        //            if (typeof response == "string")
        //            {
        //                $("[data-valmsg-for=error_msg]").html(response);
        //            }

        //                StudentIDCards.GetStudentIDCards(json);



        //        },
        //        data: json,
        //        repeatlist: 'StudentIDCardList',
        //        submitButton: '#btnSave',
        //        defaultValues: json
        //    });
    }

    var getStudentIDCards = function (json, pageIndex, resetPagination) {
        JsonData = json;
        JsonData["SearchAdmissionNo"] = $("#SearchAdmissionNo").val();
        JsonData["SearchName"] = $("#SearchName").val();
        JsonData["SearchCourseTypeKey"] = $("#SearchCourseTypeKey").val();
        JsonData["SearchCourseKey"] = $("#SearchCourseKey").val();
        JsonData["SearchUniversityKey"] = $("#SearchUniversityKey").val();
        JsonData["SearchBatchKey"] = $("#SearchBatchKey").val();
        JsonData["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonData["PageSize"] = 5;
        $("#dvRepeaterList").empty();
        $("#dvScheduleContainer").mLoading();

        request = $.ajax({
            url: $("#hdnGetStudentIDCardsList").val(),
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
                $(".ChkIsRecieved").prop("checked", false);
                $(".IssuedTrue").prop("checked", true);

                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");

                if (resetPagination) {
                    StudentIdCardPagination(JsonData);
                }
                $("#dvScheduleContainer").mLoading("destroy");
            },
            error: function (error) {
                console.log(JSON.stringify(error));
                // $("#dvScheduleContainer").mLoading("destroy");
            }
        });

    }





    var getPrintStudentIDCards = function (json) {
        JsonData = json;
        JsonData["PrintApplicationKeys"] = $("#PrintApplicationKeys").val();
        JsonData["SearchAdmissionNo"] = $("#SearchAdmissionNo").val();
        JsonData["SearchName"] = $("#SearchName").val();
        JsonData["SearchCourseTypeKey"] = $("#SearchCourseTypeKey").val();
        JsonData["SearchCourseKey"] = $("#SearchCourseKey").val();
        JsonData["SearchUniversityKey"] = $("#SearchUniversityKey").val();
        JsonData["SearchBatchKey"] = $("#SearchBatchKey").val();

        //$("#PrintDiv").empty();
        //$("#PrintDiv").mLoading();
        //request = $.ajax({
        //    url: $("#hdnPrintStudentIdCard").val(),
        //    data: JsonData,
        //    datatype: "json",
        //    type: "post",
        //    contenttype: 'application/json; charset=utf-8',
        //    async: true,
        //    beforeSend: function () {
        //        if (request != null) {
        //            request.abort();
        //        }
        //    },
        //    success: function (data) {
        //        $("#PrintDiv").html(data);

        //        $("#PrintDiv").mLoading("destroy");
        //    },
        //    error: function (error) {
        //        console.log(JSON.stringify(error));
        //        // $("#dvScheduleContainer").mLoading("destroy");
        //    }
        //});
        //$("#PrintDiv").mLoading();
        var URL = $("#hdnPrintStudentIdCard").val();

        $.customPopupform.CustomPopup({
            ajaxType: "POST",
            ajaxData: JsonData,
            modalsize: "modal-lg",
            load: function () {
              
            },
            rebind: function () {
               
                StudentIDCards.GetStudentIDCards(json);
            }
        }, URL)

    }


    var getCourseType = function (obj, ddl) {
        $(ddl).html("");
        $(ddl).append($('<option></option>').val("").html(Resources.CourseType));
        $.ajax({
            url: $("#hdnGetCourseType").val(),
            type: "GET",
            dataType: "JSON",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                $.each(result.CourseTypes, function (i, CourseType) {
                    $(ddl).append(
                        $('<option></option>').val(CourseType.RowKey).html(CourseType.Text));
                });
            }
        });
    }


    var getCourseByCourseType = function (obj, ddl) {
        $(ddl).html("");
        $(ddl).append($('<option></option>').val("").html(Resources.Course));
        $.ajax(
            {
                url: $("#hdnGetCourseByCourseType").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    $.each(result.Courses, function (i, Course) {
                        $(ddl).append(
                            $('<option></option>').val(Course.RowKey).html(Course.Text));
                    });
                }
            });
    }

    var getUniversityByCourse = function (obj, ddl) {
        $(ddl).html("");
        $(ddl).append($('<option></option>').val("").html(Resources.University));

        $.ajax({
            url: $("#hdnUniversityByCourse").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                $.each(result.Universities, function (i, University) {
                    $(ddl).append(
                        $('<option></option>').val(University.RowKey).html(University.Text));
                });
            }
        });

    }


    var resetStudentIDCard = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnResetStudentIDCard").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                StudentIDCards.GetStudentIDCards(jsonData);



            }
        });
    }



    return {
        GetStudentIDCards: getStudentIDCards,
        GetCourseType: getCourseType,
        GetCourseByCourseType: getCourseByCourseType,
        GetUniversityByCourse: getUniversityByCourse,
        ResetStudentIDCard: resetStudentIDCard,
        GetPrintStudentIDCards: getPrintStudentIDCards,
        GetStudentIDCardDetails: getStudentIDCardDetails
    }

}());


function StudentIdCardPagination(jsonData) {
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
    
        StudentIDCards.GetStudentIDCards(jsonData, num);

    });
}