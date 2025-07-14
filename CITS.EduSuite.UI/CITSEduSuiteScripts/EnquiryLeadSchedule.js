var JsonData = [], request = null;
var EnquirySchedule = (function () {
    var getEnquirySchedules = function (json, pageIndex, resetPagination, Action) {
        if (json)
            JsonData = json;
        $("#dvScheduleContainer").mLoading();
        if (resetPagination)
            $('.badge').html("<i class='fa fa-circle-o-notch fa-spin'></i>")

        JsonData["SearchName"] = $("#SearchName").val();
        JsonData["SearchPhone"] = $("#SearchPhone").val();
        JsonData["SearchEmail"] = $("#SearchEmail").val();
        JsonData["SearchFromDate"] = $("#SearchFromDate").val();
        JsonData["SearchToDate"] = $("#SearchToDate").val();
        JsonData["SearchCallStatusKey"] = $("#SearchCallStatusKey").val();
        JsonData["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonData["SearchEmployeeKey"] = $("#SearchEmployeeKey").val();
        JsonData["SearchCallTypeKey"] = $("#SearchCallTypeKey").val();
        JsonData["SearchScheduleStatusKey"] = $("#SearchScheduleStatusKey").val();
        //JsonData["SearchAcademicTermKey"] = $("#SearchAcademicTermKey").val();
        JsonData["ModuleKey"] = $("#ModuleKey").val();

        //JsonData["ScheduleStatusKey"] = $(".ScheduleTab .active").attr("value");
        JsonData["ScheduleStatusKey"] = $("#ScheduleStatusKey").val();

        JsonData["IsShortListed"] = $("#IsShortListed2").prop("checked");
        JsonData["SearchLocation"] = $("#SearchLocation").val();
        if (Action != 0) {

            $(".ScheduleCount").removeAttr("style");
            $('div[tabvalue="' + Action + '"]').css("background", "white");
            JsonData["ScheduleStatusKey"] = Action;
        }
        else {
            if ($(".ScheduleTab .active").attr("value") != Resources.ScheduleStatusToday) {
                $(".ScheduleCount").hide();
            }
            else {
                $(".ScheduleCount span").html("Loading...");
                $(".ScheduleCount").removeAttr("style");
            }
        }
        JsonData["ScheduleSelectTypeKey"] = $(".ScheduleTab1 .active").attr("value");


        JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonData["PageSize"] = $("#PageSize").val();

        request = $.ajax({
            url: $("#hdnGetEnquirySchedules").val(),
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
                //$("#dvRepeaterList").html(data);
                //$("#TotalRecords").html($("#hdnTotalRecords").val());
                $("#dvRepeaterList").html(data).contextMenu({
                    selector: "tr",
                    trigger: 'right',
                    build: function ($trigger, e) {
                        var row = $(e.target).closest("tr");
                        var obj = {};
                        var FeedbackKey = $("[name*=FeedbackKey]", row).val();
                        var RowKey = $("[name*=RowKey]", row).val();
                        var ScheduleTypeKey = $("[name*=ScheduleTypeKey]", row).val();
                        ScheduleTypeKey = parseInt(ScheduleTypeKey) ? parseInt(ScheduleTypeKey) : 0;
                        var MobileNumber = $("[name*=MobileNumber]", row).val();
                        var items = {};
                        if (ScheduleTypeKey)
                            items.E = { name: Resources.Edit + Resources.BlankSpace + Resources.Enquiry, icon: "fa-edit" }
                        else
                            items.E = { name: Resources.Edit + Resources.BlankSpace + Resources.EnquiryLead, icon: "fa-edit" }
                        items.EF = { name: Resources.Edit + Resources.BlankSpace + Resources.Feedback, icon: "fa-edit" }
                        items.CH = { name: Resources.CallHistory, icon: "fa-history" }
                        items.WT = { name: "WhatsApp", icon: "fa-whatsapp" }
                        return {
                            callback: function (key, options) {

                                var selected = options.$selected;

                                var href = "";
                                obj = {};
                                switch (key) {
                                    case "E":
                                        var url = "";
                                        obj.id = RowKey;
                                        if (ScheduleTypeKey) {

                                            url = $("#hdnAddEditEnquiry").val() + "?" + $.param(obj);
                                        }
                                        else {
                                            url = $("#hdnAddEditEnquiryLead").val() + "?" + $.param(obj);
                                        }
                                        window.open(url, '_blank');

                                        break;
                                    case "EF":
                                        obj.id = $("[name*=FeedbackKey]", row).val();
                                        obj.id = FeedbackKey;
                                        if (ScheduleTypeKey) {

                                            obj.EquiryKey = RowKey;
                                            $(selected).attr("data-url", $("#hdnAddEditEnquiryFeedback").val() + "?" + $.param(obj));
                                        }
                                        else {
                                            obj.LeadKey = RowKey;
                                            $(selected).attr("data-url", $("#hdnAddEditEnquiryLeadFeedback").val() + "?" + $.param(obj));
                                        }

                                        Enquiry.EditFeedbackPopUp($(selected))
                                        break;
                                    case "CH":
                                        EnquiryScheduleHistory.GetAllHistoryByMobileNumber(MobileNumber, 1)
                                        break;
                                    case "WT":
                                        var url = "";
                                        url = "https://wa.me/" + MobileNumber;
                                        window.open(url, '_blank');
                                        break;
                                    default:
                                        href = "";

                                }
                            },
                            items: items
                        }

                    }
                });
                $("#TotalRecords").html($("#hdnTotalRecords").val());

                //if (jsonData.ScheduleStatusKey == Resources.ScheduleStatusUnAllocated || jsonData.ScheduleStatusKey == Resources.ScheduleStatusNewLead) { // Commeted By Khaleefa on 10 Dec2022
                //    $(".hideCol").hide();
                //    $(".checkboxColumn").show();

                //}
                //else {
                //    $(".hideCol").show();
                //    $(".checkboxColumn").hide();
                //}


                if (jsonData.ScheduleSelectTypeKey == 0) {
                    $(".hideCol").show();
                    $(".checkboxColumn").hide();
                }
                else {
                    if (jsonData.ScheduleStatusKey != Resources.ScheduleStatusHistory) {
                        $(".hideCol").hide();
                        $(".checkboxColumn").show();
                    }
                    else {
                        $(".hideCol").show();
                        $(".checkboxColumn").hide();
                    }
                }



                if (resetPagination) {
                    SchedulePagination(Action);
                }

                $("#dvScheduleContainer").mLoading("destroy");
            },
            error: function (xhr) {
                if (xhr.statusText != "abort")
                    $("#dvScheduleContainer").mLoading("destroy");
            }
        });

    }
    var getEmployeesByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetEmployeeByBranchId").val(), ddl, Resources.Employee);
    }
    var getCallStatusByEnquiryStatus = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetScheduleCallStatusByEnquiryStatus").val(), ddl, Resources.CallStatus);

    }

    var viewFeedbackPopUp = function (_this) {
        var URL = $(_this).data("url");
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            headerText: '<i data-dismiss="modal" class="fa fa-remove close"></i>'
                + '<h4 class="modal-title" id="myModalLabel">' + Resources.View + Resources.BlankSpace + Resources.CallHistory + '</h4>',
            footerText: '<div class="col-sm-12">'
                + '<div class="form-group">'
                + '<button type="button" data-dismiss="modal" class="btnForm btnCancel btn btn-sm btn-default pull-right">' + Resources.Cancel + '</button>'
                + '</div></div>',
            load: function () {
                $("table th:last-child, table td:last-child", $(".modal")).remove();
            }

        }, URL);

    }

    var callHistoryReportPopup = function () {
        JsonData["SearchName"] = $("#SearchName").val();
        JsonData["SearchPhone"] = $("#SearchPhone").val();
        JsonData["SearchEmail"] = $("#SearchEmail").val();
        JsonData["SearchFromDate"] = $("#SearchFromDate").val();
        JsonData["SearchToDate"] = $("#SearchToDate").val();
        JsonData["SearchCallStatusKey"] = $("#SearchCallStatusKey").val();
        JsonData["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonData["SearchEmployeeKey"] = $("#SearchEmployeeKey").val();
        JsonData["SearchCallTypeKey"] = $("#SearchCallTypeKey").val();
        JsonData["SearchScheduleStatusKey"] = $("#SearchScheduleStatusKey").val();
        JsonData["ScheduleStatusKey"] = $(".ScheduleTab .active").attr("value");
        JsonData["ScheduleSelectTypeKey"] = $(".ScheduleTab1 .active").attr("value");


        var URL = $("#hdnComposeMailWithData").val();
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg80",
            load: function () {
                $('#mailContentData').html("")
                var $form = $("#frmComposeMessage");
                $form.find(".modal-body").mLoading();
                $.ajax({
                    url: $("#hdnGetScheduleSummary").val(),
                    data: JsonData,
                    datatype: "json",
                    type: "post",
                    contenttype: 'application/json; charset=utf-8',
                    async: true,
                    success: function (data) {

                        if ($("#SearchEmployeeKey").val() != "") {
                            var IncomingHTML = "<div class='DurationLabel'> Incoming  <span class='CallDuration' style='color:white;background:#0303bf;position: relative;  left: 5px;'></span> </div>"
                            var OutGoingHTML = "<div class='DurationLabel'>Outgoing <span  class='CallDuration'  style='color:white;background:#0ac378;position: relative;  left: 5px;'></span> </div>"
                            var WakingHTML = "<div class='DurationLabel'>Walking <span  class='CallDuration'  style='color:white;background:#b50000;position: relative;  left: 5px;'></span></div>"
                            $('#mailContentData').html("");
                            $('#mailContentData').append("<div >" + IncomingHTML + OutGoingHTML + WakingHTML + "</div>");
                            $('#mailContentData').append(CallHistoryReportDataWithSingle(data, parseInt($("#SearchEmployeeKey").val())));


                        }
                        else {
                            var IncomingHTML = "<div class='DurationLabel'> Incoming  <span class='CallDuration' style='color:white;background:#0303bf;position: relative;  left: 5px;'></span> </div>"
                            var OutGoingHTML = "<div class='DurationLabel'>Outgoing <span  class='CallDuration'  style='color:white;background:#0ac378;position: relative;  left: 5px;'></span> </div>"
                            var WakingHTML = "<div class='DurationLabel'>Walking <span  class='CallDuration'  style='color:white;background:#b50000;position: relative;  left: 5px;'></span></div>"
                            $('#mailContentData').html("");
                            $('#mailContentData').append("<div >" + IncomingHTML + OutGoingHTML + WakingHTML + "</div>");
                            $('#mailContentData').append(CallHistoryReportData(data));

                        }
                        var EmployeeName = $("#SearchEmployeeKey").val() != "" ? $("#SearchEmployeeKey option:selected").html() : "";
                        $("#MessageSubject").val(generateReportTitle(EmployeeName))
                        $("#MessageSubject").attr("readonly", "readonly")
                        $form.find(".modal-body").mLoading("destroy");
                    },
                    error: function (xhr) {
                        $form.find(".modal-body").mLoading("destroy");
                    }
                });
            },
            rebind: function (result) {

            }
        }, URL);
    }
    var copyCallHistoryReport = function () {
        containerid = "modalCallHistoryContent";
        if (document.selection) {
            var range = document.body.createTextRange();
            range.moveToElementText(document.getElementById(containerid));
            range.select().createTextRange();
            document.execCommand("copy");

        } else if (window.getSelection) {
            var range = document.createRange();
            range.selectNode(document.getElementById(containerid));
            window.getSelection().removeAllRanges();
            window.getSelection().addRange(range);
            document.execCommand("copy");
            alert("content copied")
        }
    }

    var getHistory = function (obj) {

        $("#ClosedFeedbackList").mLoading();

        request = $.ajax({
            url: $("#hdnGetClosedHistory").val(),
            data: obj,
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

                $("#ClosedFeedbackList").html(data);
                $("#ClosedFeedbackList").mLoading("destroy");

            }
        });

    }

    var checkAllocatedCount = function () {
        var CheckedLength = $("[id*=checkmark][type=checkbox]:checked").length;
        var CheckboxLength = $("[id*=checkmark][type=checkbox]").length;

        if (CheckedLength > 0) {
            $("#btnAllocateLead").show();
            if (CheckboxLength == CheckedLength) {
                $("#chkCheckAll").prop("checked", true);

            }
            else {
                $("#chkCheckAll").prop("checked", false);
            }
        } else {
            $("#btnAllocateLead").hide();
            $("#chkCheckAll").prop("checked", false);
        }
    }

    var allocateMultipleLeadPopup = function () {
        //var URL = $("#hdnAllocateMultipleLead").val();
        //$.customPopupform.CustomPopup({
        //    load: function () {
        //        setTimeout(function () {
        //            var CheckedList = $("[id*=checkmark][type=checkbox]:checked").toArray().map(function (item) {
        //                return $(item).attr("leadkey");
        //            });
        //            $("#LeadAllocationStaffKey").val(CheckedList.join(","));
        //        }, 500);
        //    },
        //    close: function () {
        //        $("[id*=checkmark][type=checkbox]").prop("checked", false);
        //        $("[id*=checkmark][type=checkbox]").trigger("change");
        //    },
        //    rebind: function (result) {
        //        if (result.IsSuccessful) {

        //            toastr.success(result.Message, Resources.Success);

        //            EnquirySchedule.GetEnquirySchedules(jsonData, $("#page-selection li.active a").html(), false, 8);
        //        }
        //        else {
        //            toastr.success(result.Message, Resources.Failed);
        //        }

        //    }
        //}, URL);

        // Commented By Khaleefa on 10 Dec 2022
        var ScheduleSelectTypeKey = $(".ScheduleTab1 .active").attr("value");
        var obj = {};
        obj.ScheduleSelectTypeKey = ScheduleSelectTypeKey;
        obj.ModuleKey = $("#ModuleKey").val();
        url = $("#hdnAllocateMultipleLead").val() + '?' + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {

            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    toastr.success(result.Message, Resources.Success);
                    window.location.reload();
                }
            }
        }, url);
    }

    var exportEnquirySchedules = function (json, pageIndex, resetPagination, Action) {
        if (json)
            JsonData = json;
        $("#dvScheduleContainer").mLoading();
        if (resetPagination)
            $('.badge').html("<i class='fa fa-circle-o-notch fa-spin'></i>")

        JsonData["SearchName"] = $("#SearchName").val();
        JsonData["SearchPhone"] = $("#SearchPhone").val();
        JsonData["SearchEmail"] = $("#SearchEmail").val();
        JsonData["SearchFromDate"] = $("#SearchFromDate").val();
        JsonData["SearchToDate"] = $("#SearchToDate").val();
        JsonData["SearchCallStatusKey"] = $("#SearchCallStatusKey").val();
        JsonData["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonData["SearchEmployeeKey"] = $("#SearchEmployeeKey").val();
        JsonData["SearchCallTypeKey"] = $("#SearchCallTypeKey").val();
        JsonData["SearchScheduleStatusKey"] = $("#SearchScheduleStatusKey").val();
        //JsonData["SearchAcademicTermKey"] = $("#SearchAcademicTermKey").val();
        JsonData["ModuleKey"] = $("#ModuleKey").val();

        //JsonData["ScheduleStatusKey"] = $(".ScheduleTab .active").attr("value");
        JsonData["ScheduleStatusKey"] = $("#ScheduleStatusKey").val();

        JsonData["IsShortListed"] = $("#IsShortListed2").prop("checked");
        JsonData["SearchLocation"] = $("#SearchLocation").val();
        if (Action != 0) {

            $(".ScheduleCount").removeAttr("style");
            $('div[tabvalue="' + Action + '"]').css("background", "white");
            JsonData["ScheduleStatusKey"] = Action;
        }
        else {


            if ($(".ScheduleTab .active").attr("value") != Resources.ScheduleStatusToday) {
                $(".ScheduleCount").hide();
            }
            else {
                $(".ScheduleCount span").html("Loading...");
                $(".ScheduleCount").removeAttr("style");
            }


        }


        JsonData["ScheduleSelectTypeKey"] = $(".ScheduleTab1 .active").attr("value");

        //var tat = $(".ScheduleTab .active").attr("value");
        //var tati = $(".ScheduleTab li a span .active").html();

        //var tat = $(".ScheduleTab .active").closest('li').wrap("<a><span></span></a>").parent().html();
        //var tssat = $(".ScheduleTab .active").closest('li').wrap("<a></a>").parent().html();
        //var tssssat = $(".ScheduleTab .active").closest('li').wrap("<span></span>").parent().html();




        JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonData["PageSize"] = $("#PageSize").val();

        request = $.ajax({
            url: $("#hdnEnquiryLeadSchedulePrint").val(),
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

                var obj = {};
                //obj.Title = $('#SearchScholarshipTypeKey').find("option:selected").text();
                //obj.SubTitle = $('#SearchBranchKey').find("option:selected").text();
                obj.FileName = "Enquiry Call Schedule";

                obj.JSONData = data;
                var columnModel = [
                    { name: 'Name', index: 'Name', headertext: "Name", },
                    { name: 'MobileNumber', index: 'MobileNumber', headertext: "Mobile Number" },
                    { name: 'CallTypeName', index: 'CallTypeName', headertext: "Call Type" },
                    { name: 'CallStatusName', index: 'CallStatusName', headertext: "Call Status" },
                    { name: 'CallDuration', index: 'CallDuration', headertext: "Call Duration" },
                    { name: 'FeedbackCreatedDate', index: 'FeedbackCreatedDate', headertext: "Feedback Date" },
                    { name: 'NextCallScheduleDate', index: 'NextCallScheduleDate', headertext: "Next Call Schedule" },
                    { name: 'Feedback', index: 'Feedback', headertext: "Feedback" },
                    { name: 'CreatedBy', index: 'CreatedBy', headertext: "Post By" },

                ];
                obj.JSONData = $(obj.JSONData).map(function (n, item) {

                    if (item.FeedbackCreatedDate) {
                        item.FeedbackCreatedDate = moment(item.FeedbackCreatedDate).format("YYYY-MM-DD");
                    }
                    if (item.NextCallScheduleDate) {
                        item.NextCallScheduleDate = moment(item.NextCallScheduleDate).format("YYYY-MM-DD");
                    }
                    if (item.CallDuration) {
                        var Minutes = moment.duration(item.CallDuration).minutes();
                        var Second = moment.duration(item.CallDuration).seconds(item.CallDuration);
                        Minutes = parseInt(Minutes) ? parseInt(Minutes) : 0;
                        Second = parseInt(Second) ? parseInt(Second) : 0;



                        if (Second > 0 && Minutes > 0) {
                            item.CallDuration = Minutes + "Minutes and " + Second + "Seconds"
                        }
                        else if (Second > 0 && Minutes <= 0) {
                            item.CallDuration = Second + " Seconds"
                        }
                        else if (Second <= 0 && Minutes > 0) {
                            item.CallDuration = Minutes + " Minutes"
                        }
                    }
                    return item
                })
                obj.ColumnNames = columnModel;
                //AppCommon.PrintJSON(obj);
                AppCommon.ExportJSONToExcel(obj);

                if (resetPagination) {
                    SchedulePagination(Action);
                }
                $("#dvScheduleContainer").mLoading("destroy");
            },
            error: function (xhr) {
                if (xhr.statusText != "abort")
                    $("#dvScheduleContainer").mLoading("destroy");
            }
        });
    }

    var checkIsFeedback = function (_this) {
        if (_this.checked) {
            $("#dvFeedback").show();
        }
        else {
            $("#dvFeedback").hide();
        }
    }



    return {
        GetEnquirySchedules: getEnquirySchedules,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        CallHistoryReportPopup: callHistoryReportPopup,
        CopyCallHistoryReport: copyCallHistoryReport,
        GetCallStatusByEnquiryStatus: getCallStatusByEnquiryStatus,
        ViewFeedbackPopUp: viewFeedbackPopUp,
        GetHistory: getHistory,
        CheckAllocatedCount: checkAllocatedCount,
        AllocateMultipleLeadPopup: allocateMultipleLeadPopup,
        ExportEnquirySchedules: exportEnquirySchedules,
        CheckIsFeedback: checkIsFeedback
    }
}());

function SchedulePagination(Action) {
    $('#page-selection').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = jsonData["PageSize"];
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

    $('#page-selection').bootpag({
        total: totalPages,
        page: 1,
        maxVisible: 10
    }).on("page", function (event, num) {
        EnquirySchedule.GetEnquirySchedules(JsonData, num, false, Action);
    });


}

function CallHistoryReportData(modelData) {
    var employees = modelData["Employees"];
    var callStatuses = modelData["ScheduleCallStatuses"];
    var callTypes = modelData["CallTypes"];
    var table = $("<table id='tblScheduleSummary' class='table table-bordered table-striped ReportTable'/>");
    var thead = $("<thead/>");
    var tbody = $("<tbody/>");
    var tfoot = $("<tfoot/>");
    var thRow = $("<tr/>");

    var dataCount = modelData["CallCountList"];
    var dataDuration = modelData["CallDurationList"];
    var colspan = callStatuses.length + callTypes.length + 3;

    var thCol = $("<th/>");
    $(thCol).html("Employee");
    $(thRow).append(thCol);


    $.each(callStatuses, function () {
        thCol = $("<th/>");
        $(thCol).html(this.Text);
        $(thRow).append(thCol);
    });
    thCol = $("<th/>");
    $(thCol).html("Total Calls");
    $(thRow).append(thCol);
    thCol = $("<th/>");
    $(thCol).html("Rescheduled");
    $(thRow).append(thCol);
    thCol = $("<th/>");
    $(thCol).html("Total");
    $(thRow).append(thCol);
    thCol = $("<th/>");

    $(thCol).html("Productive Calls");
    $(thRow).append(thCol);

    $.each(callTypes, function () {
        thCol = $("<th/>");
        $(thCol).html(this.Text);
        $(thRow).append(thCol);
    });

    thCol = $("<th/>");
    $(thCol).html("Total Duration");
    $(thRow).append(thCol);

    $(thead).append(thRow)

    var grandTotalDuration = 0, grandTotalCount = 0, grandTotalRecheduleCount = 0, grandTotalOriginalCount = 0, grandTotalIncomingProductivityCount = 0, grandTotalOutgoingProductivityCount = 0, grandTotalWalkingProductivityCount = 0, TotalCallsCount = 0;

    for (var i = 0; i < employees.length; i++) {
        var row = $("<tr>");


        //col-1 EmployeeName     
        var column = $("<td>");
        $(column).html(employees[i].Text);
        $(row).append(column);



        //col-2 CallStatus
        var TotalCount = 0;

        for (var j = 0; j < callStatuses.length; j++) {
            var newCountData = $.extend(true, [], dataCount);
            newCountData = newCountData.filter(function (p, n) {
                return p.EmployeeKey === employees[i].RowKey && p.EnquiryCallStatusKey === callStatuses[j].RowKey;
            });
            column = $("<td>");
            if (newCountData[0] && newCountData[0].TotalRecords) {
                TotalCount = parseInt(newCountData[0].TotalRecords) ? TotalCount + parseInt(newCountData[0].TotalRecords) : TotalCount;
                $(column).html(newCountData[0].TotalRecords);
                grandTotalOriginalCount += newCountData[0].TotalRecords;
            }
            $(row).append(column);

        }


        //col-3 TotalCalls
        column = $("<td>");
        if (TotalCount != 0) {
            $(column).html(TotalCount);
            grandTotalCount = grandTotalCount + TotalCount;
        }
        $(row).append(column);

        newCountData = $.extend(true, [], dataCount);
        newCountData = newCountData.filter(function (p, n) {
            return p.EmployeeKey === employees[i].RowKey && !p.EnquiryCallStatusKey;
        });



        //col-4 Rescheduled
        column = $("<td>");
        if (newCountData[0] && newCountData[0].TotalRecords) {
            var RemainCount = TotalCount - newCountData[0].TotalRecords;
            grandTotalRecheduleCount += RemainCount;
            $(column).html("<span style='color:red'>" + RemainCount + "</span>");
        }
        $(row).append(column);



        //col-5 Total
        column = $("<td>");
        if (newCountData[0] && newCountData[0].TotalRecords) {
            $(column).html(newCountData[0].TotalRecords);
            TotalCallsCount = TotalCallsCount + newCountData[0].TotalRecords;
        }
        $(row).append(column);







        //col-6 ProductiveCalls
        column = $("<td>");
        if (newCountData[0] && newCountData[0].TotalRecords) {
            $(modelData.ProductiveIncomingCallList).each(function (index, value) {
                if (modelData.ProductiveIncomingCallList[index].EmployeeKey == employees[i].RowKey) {

                    var IncomingHTML = "<span class='CallDuration' style='color:white;background:#0303bf'>" + modelData.ProductiveIncomingCallList[index].TotalRecords + "</span> "
                    var OutGoingHTML = " <span  class='CallDuration'  style='color:white;background:#0ac378'>" + modelData.ProductiveOutgoingCallList[index].TotalRecords + "</span>"
                    var WakingHTML = "<span  class='CallDuration'  style='color:white;background:#b50000;'>" + modelData.ProductiveWalkingList[index].TotalRecords + "</span>"

                    $(column).html(IncomingHTML + OutGoingHTML + WakingHTML);
                    grandTotalIncomingProductivityCount = grandTotalIncomingProductivityCount + modelData.ProductiveIncomingCallList[index].TotalRecords;
                    grandTotalOutgoingProductivityCount = grandTotalOutgoingProductivityCount + modelData.ProductiveOutgoingCallList[index].TotalRecords;
                    grandTotalWalkingProductivityCount = grandTotalWalkingProductivityCount + modelData.ProductiveWalkingList[index].TotalRecords;



                }
            });
        }

        $(row).append(column);





        //col-7 CallTypes
        var TotalDuration = 0;
        for (var k = 0; k < callTypes.length; k++) {

            var newDurationData = $.extend(true, [], dataDuration);
            newDurationData = newDurationData.filter(function (p, n) {
                return p.EmployeeKey === employees[i].RowKey && p.CallTypeKey === callTypes[k].RowKey;
            });
            var column = $("<td>");
            if (newDurationData[0] && newDurationData[0].TotalRecords) {
                $(column).html(ticksToDisplayTime(newDurationData[0].TotalRecords));
                TotalDuration = parseInt(newDurationData[0].TotalRecords) ? TotalDuration + parseInt(newDurationData[0].TotalRecords) : TotalDuration;
            }
            $(row).append(column);

        }



        //col-8 TotalDuration
        column = $("<td>");
        if (TotalDuration != 0) {
            $(column).html(ticksToDisplayTime(TotalDuration));
            grandTotalDuration = grandTotalDuration + TotalDuration;

        }



        $(row).append(column);
        $(tbody).append(row);
    }


    //Footer-1 Total
    var tfCol = $("<th/>");
    var tfRow = $("<tr/>");
    $(tfCol).html("Total");

    $(tfRow).append(tfCol);


    //Footer-2 CallStatuses
    $.each(callStatuses, function () {
        tfCol = $("<th/>");
        var CallStatuskey = this.RowKey;
        var newCountData = $.extend(true, [], dataCount);
        var totalCalls = 0;
        newCountData = newCountData.filter(function (p, n) {
            return p.EnquiryCallStatusKey === CallStatuskey;
        })
        $(newCountData).each(function () {
            totalCalls = parseInt(this.TotalRecords) ? totalCalls + parseInt(this.TotalRecords) : totalCalls;
        });
        if (totalCalls != 0)
            $(tfCol).html(totalCalls);
        $(tfRow).append(tfCol);
    });






    //Footer-3 TotalCalls

    tfCol = $("<th/>");
    if (grandTotalCount != 0)
        $(tfCol).html(grandTotalCount);
    $(tfRow).append(tfCol);





    //Footer-4 Rescheduled
    tfCol = $("<th/>");
    if (grandTotalRecheduleCount != 0)
        $(tfCol).html("<span style='color:red'>" + grandTotalRecheduleCount + "</span>");
    $(tfRow).append(tfCol);



    //Footer-5 Total
    tfCol = $("<th/>");
    if (TotalCallsCount != 0)
        $(tfCol).html(TotalCallsCount);
    $(tfRow).append(tfCol);




    //Footer-6 ProductiveCalls

    tfCol = $("<th/>");

    var IncomingHTML = "<span  class='CallDuration' style='color:white;background:#0303bf'>" + grandTotalIncomingProductivityCount + "</span>";
    var OutgoingHTML = "<span  class='CallDuration' style='color:white;background:#0ac378'>" + grandTotalOutgoingProductivityCount + "</span>";
    var WalkingHTML = "<span  class='CallDuration' style='color:white;background:#b50000'>" + grandTotalWalkingProductivityCount + "</span>";


    $(tfCol).html(IncomingHTML + OutgoingHTML + WalkingHTML);
    $(tfRow).append(tfCol);




    //Footer-7 CallTypes
    $.each(callTypes, function () {
        tfCol = $("<th/>");
        var CallTypekey = this.RowKey;
        var newDurationData = $.extend(true, [], dataDuration);
        var totalDurations = 0;
        newDurationData = newDurationData.filter(function (p, n) {
            return p.CallTypeKey === CallTypekey;
        })
        $(newDurationData).each(function () {
            totalDurations = parseInt(this.TotalRecords) ? totalDurations + parseInt(this.TotalRecords) : totalDurations;
        });
        if (totalDurations != 0)
            $(tfCol).html(ticksToDisplayTime(totalDurations));
        $(tfRow).append(tfCol);
    });


    //Footer-8 TotalDuration
    tfCol = $("<th/>");
    if (grandTotalDuration != 0)
        $(tfCol).html(ticksToDisplayTime(grandTotalDuration));
    $(tfRow).append(tfCol);

    $(tfoot).append(tfRow)







    $(table).append(thead);
    $(table).append(tbody);
    $(table).append(tfoot);
    var div = $("<div/>");
    $(div).css("overflow-y", "auto");


    //Intrested Enquiries

    var IntrestedEnquiries = modelData["IntrestedList"];
    var TableRow = "";
    var TotalRecords = "<h4 style='color:red'> Total Counsellings: " + IntrestedEnquiries.length + "</h2>";
    var TableHead = "<tr><th>Name</th><th>Place</th><th>Branch</th><th>Schedule Date</th> <th> Councelling Time </th></tr>";
    TableRow = TableRow + TableHead;
    $.each(IntrestedEnquiries, function (key, value) {
        if (value.LocationName == null)
            value.LocationName = "";

        TableRow = TableRow + "<tr><td>" + value.EnquiryName + "</td> <td>" + value.LocationName + "</td><td>" + value.BranchName + "</td><td>" + AppCommon.JsonDateToNormalDate2(value.LastCallScheduleDate) + "</td> <td>" + (value.CouncellingTime && value.CouncellingTime != null ? value.CouncellingTime : "") + "</td>  </tr>"

    });

    var IntrestedTable = TotalRecords + "<table  class='table table-bordered table-striped'>" + TableRow + "</table>";


    //Todays Councelling

    var TodaysCouncelling = modelData["TodaysCouncelling"];
    var TableRow = "";
    var TotalRecords = "<h4 style='color:red'> Todays Counsellings: " + TodaysCouncelling.length + "</h2>";
    var TableHead = "<tr><th>Name</th><th>Place</th><th>Branch</th><th>Schedule Date</th> <th> Councelling Time </th> <th> Counselled By </th><th> Scheduled By </th></tr>";
    TableRow = TableRow + TableHead;
    $.each(TodaysCouncelling, function (key, value) {
        if (value.LocationName == null)
            value.LocationName = "";

        TableRow = TableRow + "<tr><td>" + value.EnquiryName + "</td> <td>" + value.LocationName + "</td><td>" + value.BranchName + "</td><td>" + AppCommon.JsonDateToNormalDate2(value.LastCallScheduleDate) + "</td> <td>" + (value.CouncellingTime && value.CouncellingTime != null ? value.CouncellingTime : "") + "</td> <td>" + value.CounselledBy + "</td> <td>" + value.ScheduledBy + "</td>  </tr>"

    });

    var TodaysCouncellingTable = TotalRecords + "<table  class='table table-bordered table-striped'>" + TableRow + "</table>";




    $(div).append(table);
    $(div).append(IntrestedTable);
    $(div).append(TodaysCouncellingTable);



    return div;
}



function CallHistoryReportDataWithSingle(modelData, EmployeeKey) {
    var employees = modelData["Employees"].filter(function (p, n) {
        return p.RowKey === EmployeeKey;
    });
    var callStatuses = modelData["ScheduleCallStatuses"];
    var callTypes = modelData["CallTypes"];
    var div = $("<div/>");
    $(div).css("overflow-y", "auto");

    var table = $("<table class='table table-bordered table-striped'/>");
    var thead = $("<thead/>");
    var tbody = $("<tbody/>");
    var tfoot = $("<tfoot/>");
    var thRow = $("<tr/>");
    var dataCount = modelData["CallCountList"];
    var dataDuration = modelData["CallDurationList"];

    var thCol = $("<th style='width: 50%;'/>");
    $(thCol).html("Call Status");
    $(thRow).append(thCol);


    thCol = $("<th/>");
    $(thCol).html("Count");
    $(thRow).append(thCol);

    $(thead).append(thRow)

    var grandTotalDuration = 0, grandTotalCount = 0
    var row = $("<tr>");
    var TotalCount = 0;
    for (var j = 0; j < callStatuses.length; j++) {
        row = $("<tr>");
        var newCountData = $.extend(true, [], dataCount);
        newCountData = newCountData.filter(function (p, n) {
            return p.EmployeeKey === EmployeeKey && p.EnquiryCallStatusKey === callStatuses[j].RowKey;
        });
        column = $("<td>");
        $(column).html(callStatuses[j].Text);
        $(row).append(column);

        column = $("<td>");
        if (newCountData[0] && newCountData[0].TotalRecords) {
            TotalCount = parseInt(newCountData[0].TotalRecords) ? TotalCount + parseInt(newCountData[0].TotalRecords) : TotalCount;
            $(column).html(newCountData[0].TotalRecords);
        }
        $(row).append(column);
        $(tbody).append(row)
    }

    row = $("<tr>");
    column = $("<th>");
    $(column).html("Total Count");
    $(row).append(column);

    column = $("<th>");
    if (TotalCount != 0) {
        $(column).html(TotalCount);
    }
    $(row).append(column);
    $(tfoot).append(row);



    row = $("<tr>");
    column = $("<th>");
    $(column).html("Rescheduled");
    $(row).append(column);
    var newCountData = $.extend(true, [], dataCount);
    newCountData = newCountData.filter(function (p, n) {
        return p.EmployeeKey === EmployeeKey && !p.EnquiryCallStatusKey;
    });

    column = $("<th>");
    if (newCountData[0] && newCountData[0].TotalRecords) {
        var RemainCount = TotalCount - newCountData[0].TotalRecords;
        $(column).html("<span style='color:red'>" + RemainCount + "</span>");
    }
    $(row).append(column);
    $(tfoot).append(row)









    row = $("<tr>");
    column = $("<th>");
    $(column).html("Total");
    $(row).append(column);

    column = $("<th>");
    if (newCountData[0] && newCountData[0].TotalRecords) {
        $(column).html(newCountData[0].TotalRecords);
    }
    $(row).append(column);
    $(tfoot).append(row);



    row = $("<tr>");
    column = $("<th>");
    $(column).html("Productive Calls");
    $(row).append(column);

    column = $("<th>");
    if (newCountData[0] && newCountData[0].TotalRecords) {

        $(modelData.ProductiveIncomingCallList).each(function (index, value) {
            if (modelData.ProductiveIncomingCallList[index].EmployeeKey == EmployeeKey) {
                var IncomingHTML = "<span class='CallDuration' style='color:white;background:#0303bf'>" + modelData.ProductiveIncomingCallList[index].TotalRecords + "</span> "
                var OutGoingHTML = " <span  class='CallDuration'  style='color:white;background:#0ac378'>" + modelData.ProductiveOutgoingCallList[index].TotalRecords + "</span>"
                var WakingHTML = "<span  class='CallDuration'  style='color:white;background:red;    margin-left: 3px;'>" + modelData.ProductiveWalkingList[index].TotalRecords + "</span>"


                $(column).html(IncomingHTML + OutGoingHTML + WakingHTML);

            }
        });



    }
    $(row).append(column);
    $(tfoot).append(row);


    $(table).append(thead);
    $(table).append(tbody);
    $(table).append(tfoot);

    $(div).append(table);


    table = $("<table class='table table-bordered table-striped'/>");
    thead = $("<thead/>");
    tbody = $("<tbody/>");
    tfoot = $("<tfoot/>");

    thRow = $("<tr/>");
    thCol = $("<th style='width: 50%;'/>");
    $(thCol).html("Call Type");
    $(thRow).append(thCol);


    thCol = $("<th/>");
    $(thCol).html("Duration");
    $(thRow).append(thCol);

    $(thead).append(thRow)




    var TotalDuration = 0;
    for (var k = 0; k < callTypes.length; k++) {
        row = $("<tr>");
        var newDurationData = $.extend(true, [], dataDuration);
        newDurationData = newDurationData.filter(function (p, n) {
            return p.EmployeeKey === EmployeeKey && p.CallTypeKey === callTypes[k].RowKey;
        });
        column = $("<td>");
        $(column).html(callTypes[k].Text);
        $(row).append(column);

        var column = $("<td>");
        if (newDurationData[0] && newDurationData[0].TotalRecords) {
            $(column).html(ticksToDisplayTime(newDurationData[0].TotalRecords));
            TotalDuration = parseInt(newDurationData[0].TotalRecords) ? TotalDuration + parseInt(newDurationData[0].TotalRecords) : TotalDuration;
        }
        $(row).append(column);
        $(tbody).append(row);
    }
    row = $("<tr>");
    column = $("<th>");
    $(column).html("Total Duration");
    $(row).append(column);

    column = $("<th>");
    if (TotalDuration != 0) {
        $(column).html(ticksToDisplayTime(TotalDuration));
    }
    $(row).append(column);
    $(tfoot).append(row);






    $(table).append(thead);
    $(table).append(tbody);
    $(table).append(tfoot);
    $(div).append(table);

    //
    var IntrestedEnquiries = modelData["IntrestedList"];
    var newIntrestedEnquiries = [];
    newIntrestedEnquiries = IntrestedEnquiries.filter(function (p, n) {
        return p.EmployeeKey === EmployeeKey
    });
    var TableRow = "";
    var TotalRecords = "<h4 style='color:red'> Total Counselling: " + newIntrestedEnquiries.length + "</h2>";
    var TableHead = "<tr><th>Name</th><th>Place</th><th>Schedule Date</th> <th> Councelling Time </th></tr>";
    TableRow = TableRow + TableHead;

    $.each(newIntrestedEnquiries, function (key, value) {
        if (value.LocationName == null)
            value.LocationName = "";

        TableRow = TableRow + "<tr><td>" + value.EnquiryName + "</td> <td>" + value.LocationName + "</td><td>" + AppCommon.JsonDateToNormalDate2(value.LastCallScheduleDate) + "</td> <td>" + value.CouncellingTime + "</td>  </tr>"

    });

    var IntrestedTable = TotalRecords + "<table  class='table table-bordered table-striped'>" + TableRow + "</table>";


    //Todays Councelling
    var TodaysCouncelling = modelData["TodaysCouncelling"];
    var newTodaysCouncelling = [];
    newTodaysCouncelling = TodaysCouncelling.filter(function (p, n) {
        return p.EmployeeKey === EmployeeKey
    });
    var TableRow = "";
    var TotalRecords = "<h4 style='color:red'> Todays Counselling: " + newTodaysCouncelling.length + "</h2>";
    var TableHead = "<tr><th>Name</th><th>Place</th><th>Schedule Date</th> <th> Councelling Time </th></tr>";
    TableRow = TableRow + TableHead;

    $.each(newTodaysCouncelling, function (key, value) {
        if (value.LocationName == null)
            value.LocationName = "";

        TableRow = TableRow + "<tr><td>" + value.EnquiryName + "</td> <td>" + value.LocationName + "</td><td>" + AppCommon.JsonDateToNormalDate2(value.LastCallScheduleDate) + "</td> <td>" + value.CouncellingTime + "</td>  </tr>"

    });

    var TodaysCouncellingTable = TotalRecords + "<table  class='table table-bordered table-striped'>" + TableRow + "</table>";


    $(table).append(thead);
    $(table).append(tbody);
    $(table).append(tfoot);
    $(div).append(table);
    $(div).append(IntrestedTable);
    $(div).append(TodaysCouncellingTable);

    return div;
}

function ticksToDisplayTime(ticksInSecs) {
    var ticks = ticksInSecs;
    var hh = Math.floor(ticks / 3600);
    var mm = Math.floor((ticks % 3600) / 60);
    var ss = ticks % 60;

    return (pad(hh, 2) + ":" + pad(mm, 2) + ":" + pad(ss, 2));
}

function pad(n, width) {
    var n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join('0') + n;
}


function generateReportTitle(employee) {
    var employeeName = employee;
    employeeName = employeeName && employeeName != "" ? " of " + employeeName : "";
    var title = "Call History Report";
    if ($("#SearchFromDate").val() != "" && $("#SearchFromDate").val() == $("#SearchToDate").val()) {
        title = title + employeeName + " on " + $("#SearchFromDate").val()
    }
    else if ($("#SearchFromDate").val() != "" && $("#SearchToDate").val() == "") {
        title = title + employeeName + " from " + $("#SearchFromDate").val()
    }
    else if ($("#SearchFromDate").val() == "" && $("#SearchToDate").val() != "") {
        title = title + employeeName + " until " + $("#SearchToDate").val()
    }
    else if ($("#SearchFromDate").val() != "" && $("#SearchToDate").val() != "") {
        title = title + employeeName + " " + $("#SearchFromDate").val() + " to " + $("#SearchToDate").val()
    }
    else {
        title = title + employeeName;
    }
    return title;
}

function generateReportPDF(htmls, item, IsUpload) {
    var div = $("<div/>");
    $(div).attr("id", "dvPayslipPdf").css("background", "#fff");
    $("body").append(div);
    var htmlLength = htmls.length, i = 0;
    var doc = new jsPDF("p", "mm", "a4");
    htmls.forEach(function (html) {
        $("#dvPayslipPdf").html("").html(html);
        var divHeight = $(div).height();
        var divWidth = $(div).width();
        var ratio = divHeight / divWidth;
        html2canvas($("#dvPayslipPdf"), {
            onrendered: function (canvas) {
                try {
                    var ctx = canvas.getContext('2d');

                    ctx.webkitImageSmoothingEnabled = false;
                    ctx.mozImageSmoothingEnabled = false;
                    ctx.imageSmoothingEnabled = false;
                    ctx.font = "14px san-serif";

                    var imgData = canvas.toDataURL("image/png");

                    var width = doc.internal.pageSize.width;
                    var height = doc.internal.pageSize.height;
                    height = ratio * width;
                    if (i > 0)
                        doc.addPage();

                    doc.addImage(imgData, 'PNG', 10, 10, width - 20, height - 20);

                    //doc.save('sample-file.pdf');
                    if (IsUpload) {
                        var pdfFile = AppCommon.BlobToFile(doc.output('blob'), item.toString(), 'application/pdf'); //returns raw body of resulting PDF returned as a string as per the plugin documentation.
                        var pdfFiles = [];
                        pdfFiles["file"] = pdfFile;
                        var response = AjaxHelper.ajaxWithFile("file", "POST", $("#hdnUploadPdfPayslip").val(),
                            { file: pdfFiles }
                        );
                    }
                    if (htmlLength - 1 == i) {
                        if (!IsUpload) {
                            doc.save(item + ".pdf");
                        }
                        $("#dvPayslipPdf").remove();
                    }
                    i++;
                }
                catch (e) {

                }

            }
        });
    })
}

