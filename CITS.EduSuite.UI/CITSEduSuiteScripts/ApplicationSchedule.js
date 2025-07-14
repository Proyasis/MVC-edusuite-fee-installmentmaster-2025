var JsonModel = [], request = null, Pagination = null;

var ApplicationSchedule = (function () {


    var applicationSchedulePartial = function (json, pageIndex, resetPagination) {

        $("#followupContent").mLoading();

        JsonModel = json;
        Pagination = resetPagination;

        JsonModel["SearchAnyText"] = $("#SearchAnyText").val();
        JsonModel["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonModel["SearchProcessStatusKey"] = $("#SearchProcessStatusKey").val();
        JsonModel["SearchStudentStatusKey"] = $("#SearchStudentStatusKey").val();
        JsonModel["SearchUniversityKey"] = $("#SearchUniversityKey").attr("value");
        JsonModel["SearchBatchKey"] = $("#SearchBatchKey").attr("value");
        JsonModel["SearchCourseKey"] = $("#SearchCourseKey").attr("value");
        JsonModel["SearchCallTypeKey"] = $("#SearchCallTypeKey").attr("value");
        JsonModel["SearchApplicationCallStatusKey"] = $("#SearchApplicationCallStatusKey").attr("value");
        JsonModel["SearchApplicationScheduleTypeKey"] = $("#SearchApplicationScheduleTypeKey").attr("value");
        JsonModel["SearchFrom"] = $("#SearchDateFrom").val();
        JsonModel["SearchTo"] = $("#SearchDateTo").val();

        var fromdate = moment(JsonModel["SearchFrom"], 'DD-MM-YYYY').toDate();
        JsonModel["SearchFrom"] = moment(fromdate).format('YYYY-MM-DD');

        var todate = moment(JsonModel["SearchTo"], 'DD-MM-YYYY').toDate();
        JsonModel["SearchTo"] = moment(todate).format('YYYY-MM-DD');


        JsonModel["StartIndex"] = pageIndex;
        JsonModel["PageSize"] = 10;

        JsonModel["FetchKey"] = 1;
        JsonModel["SearchTabKey"] = $(".FollowupTab .active").attr("value");



        if (request != null) {
            request.abort();
        }


        request = $.ajax({
            url: $("#hdnApplicationScheduleDetails").val(),
            type: 'POST',
            data: JsonModel,
            cache: false,
            async: true,
            beforeSend: function () {
                if (request != null) {
                    request.abort();
                }

            },
        }).done(function (result) {
            $('#followupContent').html(result);


            selection.CheckList();

            if (resetPagination) {
                //FollowupPaging();
            }

            $("#followupContent").mLoading("destroy");




        });


        for (i = 0; i <= 12; i++) {
            ApplicationSchedule.ApplicationScheduleCounts(jsonData, i, resetPagination);
        }






    }

    var requestFollowupCount = [];
    var applicationScheduleCounts = function (json, value, resetpaging) {



        JsonModel = json;
        JsonModel["SearchAnyText"] = $("#SearchAnyText").val();
        JsonModel["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonModel["SearchProcessStatusKey"] = $("#SearchProcessStatusKey").val();
        JsonModel["SearchStudentStatusKey"] = $("#SearchStudentStatusKey").val();
        JsonModel["SearchUniversityKey"] = $("#SearchUniversityKey").attr("value");
        JsonModel["SearchBatchKey"] = $("#SearchBatchKey").attr("value");
        JsonModel["SearchCourseKey"] = $("#SearchCourseKey").attr("value");
        JsonModel["SearchCallTypeKey"] = $("#SearchCallTypeKey").attr("value");
        JsonModel["SearchApplicationCallStatusKey"] = $("#SearchApplicationCallStatusKey").attr("value");
        JsonModel["SearchApplicationScheduleTypeKey"] = $("#SearchApplicationScheduleTypeKey").attr("value");

        JsonModel["SearchDateFrom"] = $("#SearchDateFrom").val();
        JsonModel["SearchDateTo"] = $("#SearchDateTo").val();



        //JsonModel["FetchKey"] = 1;
        //JsonModel["SearchTabKey"] = $(".FollowupTab .active").attr("value");

        JsonModel["SearchTabKey"] = value;


        JsonModel["FetchKey"] = 2;

        requestFollowupCount[value] = $.ajax({
            url: $("#hdnApplicationScheduleCounts").val(),
            type: 'POST',
            data: JsonModel,
            cache: false,
            async: true,
            beforeSend: function () {
                if (requestFollowupCount[value] != null) {
                    requestFollowupCount[value].abort();
                }
            },
        }).done(function (result) {


            var ifcloased = $("#hidifCloesed").val();
            if (typeof ifcloased === "undefined") {
                $('.FollowupTab a[value="' + value + '"] .badge ').html(result);


                if ($(".FollowupTab .active").attr("value") == value) {
                    $("#hdnTotalRecords").val(result);
                    if (resetpaging) {
                        FollowupPaging();
                    }
                }
            }
            else {
                $("#hdnTotalRecords").val(result);
                if (resetpaging) {
                    FollowupPaging();
                }
            }

        });

    }

    var editApplicationSchedulePopup = function (ApplicationKey, Id, ApplicationScheduleTypeKey) {
        var obj = {};
        obj.ApplicationKey = ApplicationKey;
        obj.ApplicationScheduleTypeKey = ApplicationScheduleTypeKey;
        obj.Id = Id;
        //url = $("#hdnAddEditApplicationSchedule").val() + "/" + Id + "?" + $.param(obj);
        url = $("#hdnAddEditApplicationSchedule").val() + "?" + $.param(obj);
        var validator = null
        $.customPopupform.CustomPopup({
            modalsize: "modal-md ",
            load: function () {

            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.reload();
                }
            }
        }, url);

    }
    function deleteApplicationSchedule(rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Agent,
            actionUrl: $("#hdnDeleteApplicationschedule").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                window.location.reload();
            }
        });
    }

    var checkCallStatusDuration = function (Id) {
        var response = AjaxHelper.ajax("GET", $("#hdnCheckDuration").val() + "/" + Id);
        $("#IsDuration").val(response);
    }

    return {
        ApplicationSchedulePartial: applicationSchedulePartial,
        ApplicationScheduleCounts: applicationScheduleCounts,
        EditApplicationSchedulePopup: editApplicationSchedulePopup,
        DeleteApplicationSchedule: deleteApplicationSchedule,
        CheckCallStatusDuration: checkCallStatusDuration,
    }
}());



function FollowupPaging() {
    $('#page-selection-down').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = 10;
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

    $('#page-selection-down').bootpag({
        total: totalPages,
        page: 1,
        maxVisible: 10
    });


    $('#page-selection-down').off("page").on("page", function (event, num) {
        ApplicationSchedule.ApplicationSchedulePartial(jsonData, num, false);

    });

}