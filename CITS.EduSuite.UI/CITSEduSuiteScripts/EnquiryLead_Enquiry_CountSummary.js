var JsonModel = [], request = null;
var GvWidth;
var tableStudentSummary;
var griddatacount = [50, 100, 1000, 2500, 5000];

var EnquiryLeadCountSummary = (function () {

    // Enquiry Lead Summary Start

    var getEnquiryLeadSummary = function (json) {

        JsonModel = json;

        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        $("#grid").jqGrid({
            colNames: [Resources.SlNo, Resources.Employee, "", "Total Lead", "Pending Lead", "Processed Lead", "Closed Lead", "Added Enquiry", "Added Application", ""],
            colModel: [

                { key: false, name: 'RowNumber', index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeName', index: 'FirstName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TotalLead', index: 'TotalLead', hidden: true, editable: true, cellEdit: true, sorttype: "number", sortable: false, resizable: false },
                { key: false, name: 'TotalLead', index: 'TotalLead', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: TotalLeadCount },
                { key: false, name: 'PendingLead', index: 'PendingLead', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: PendingLeadCount },
                { key: false, name: 'ProcessedLead', index: 'ProcessedLead', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: ProcessedCount },
                { key: false, name: 'ClosedLead', index: 'ClosedLead', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: ClosedCount },
                { key: false, name: 'AddedEnquiry', index: 'AddedEnquiry', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: AddedEnquiryCount },
                { key: false, name: 'AddedApplication', index: 'AddedApplication', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: AddedApplicationLeadCount },
                { key: false, name: 'RowKey', index: 'RowKey', hidden: true, editable: true, cellEdit: true, sorttype: "number", sortable: false, resizable: false },
            ],
            url: $("#hdnEnquiryLeadSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',

            pager: jQuery('#pager'),
            rowNum: 50,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },

        })

        $("#grid").jqGrid("setLabel", "rn", "Sl.No");

    }

    function TotalLeadCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }
    function PendingLeadCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="2" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="2" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function ProcessedCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="3" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="3" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function ClosedCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="4" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="4" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function AddedEnquiryCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="5" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="5" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function AddedApplicationLeadCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="6" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="6" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function CounselingCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="7" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="7" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function CounselingCompletedCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="8" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="8" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function AddedApplicationEnquiryCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a data-key="4" data-val="9" EmployeeKey=' + rowdata.RowKey + '  class="callsCount FollowUpCallsCount" onclick="EnquiryLeadCountSummary.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            return '<a data-key="4" data-val="9" EmployeeKey=' + rowdata.RowKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    // Enquiry Lead Summary End



    // Enquiry Summary Start

    var getEnquirySummary = function (json) {

        JsonModel = json;

        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        $("#grid").jqGrid({
            colNames: [Resources.SlNo, Resources.Employee, "", "Total Enquiry", "FollowUp", "Counseling", "Counseling Completed", "Closed", "Added Application",""],
            colModel: [

                { key: false, name: 'RowNumber', index: 'RowNumber', editable: true, cellEdit: true, sortable: false, resizable: false },
                { key: false, name: 'EmployeeName', index: 'FirstName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TotalEnquiry', index: 'TotalEnquiry', hidden: true, editable: true, cellEdit: true, sorttype: "number", sortable: false, resizable: false },
                { key: false, name: 'TotalEnquiry', index: 'TotalEnquiry', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: TotalLeadCount },
                { key: false, name: 'FollowUp', index: 'FollowUp', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: ProcessedCount },
                { key: false, name: 'Counseling', index: 'Counseling', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: CounselingCount },
                { key: false, name: 'CounselingCompleted', index: 'CounselingCompleted', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: CounselingCompletedCount },
                { key: false, name: 'Closed', index: 'Closed', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: ClosedCount },
                { key: false, name: 'AddedApplication', index: 'AddedApplication', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: AddedApplicationEnquiryCount },
                { key: false, name: 'EmployeeKey', index: 'EmployeeKey', hidden: true, editable: true, cellEdit: true, sorttype: "number", sortable: false, resizable: false },
            ],
            url: $("#hdnEnquirySummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',

            pager: jQuery('#pager'),
            rowNum: 50,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },

        })

        $("#grid").jqGrid("setLabel", "rn", "Sl.No");

    }



    // Enquiry Summary End



    var getCallReportDetailed = function (_this, pageIndex, resetPagination) {
        var URL = $("#hdnGetCallReportsDetailed").val();


        $.customPopupform.CustomPopup({
            modalsize: "modal-xl  mw-100 w-100",
            ajaxType: "Post",
            load: function () {


                EnquiryLeadCountSummary.GetCallReportDetailedData(_this, pageIndex, resetPagination);

            },


        }, URL);

    }

    var getCallReportDetailedData = function (_this, pageIndex, resetPagination) {
        $(".modal-body").mLoading();
        var URL = $("#hdnGetCallReportsData").val();


        JsonModel["SearchEnquiryStatusKey"] = $(_this).attr("data-val");

        //JsonModel["SearchScheduledEmployeeKey"] = "";

        if ($("#EmployeeFilterTypeKey").val() == 1 || $("#EmployeeFilterTypeKey").val() == 3) {
            var EmployeeKey = $(_this).attr("employeekey");
            if (EmployeeKey != "") {
                JsonModel["SearchEmployeeKey"] = EmployeeKey;
            }
            else {
                JsonModel["SearchEmployeeKey"] = $("#SearchEmployeeKey").val();
            }
        }
        else {

            var ScheduledEmployeeKey = $(_this).attr("employeekey");
            if (ScheduledEmployeeKey != "") {
                JsonModel["SearchScheduledEmployeeKey"] = ScheduledEmployeeKey;
            }
            else {
                JsonModel["SearchScheduledEmployeeKey"] = $("#SearchScheduledEmployeeKey").val();
            }

        }

        JsonModel["SearchFromDate"] = $("#SearchFromDate").val();
        JsonModel["SearchToDate"] = $("#SearchToDate").val();


        JsonModel["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonModel["PageSize"] = 10;


        request = $.ajax({
            url: URL,
            data: JsonModel,
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
                $("#reportData").html(data);

                //JsonModel["SearchEmployeeKey"] = "";
                //JsonModel["SearchScheduledEmployeeKey"] = "";
                //$("#TotalRecords").html($("#hdnTotalRecords").val());
                if (resetPagination) {
                    CallReportPagination(_this);
                }
                $(".modal-body").mLoading("destroy");
            },
            error: function (xhr) {
                console.log(xhr.responseText);
                //  $("#dvScheduleContainer").mLoading("destroy");
            }
        });
    }

    function CallReportPagination(_this) {

        $('#page-selection-up,#page-selection-down').empty();
        var totalRecords = $("#hdnTotalRecords").val();
        totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
        var Size = jsonData["PageSize"];
        var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

        $('#page-selection-up,#page-selection-down').bootpag({
            total: totalPages,
            page: 1,
            maxVisible: 30
        });

        $('#page-selection-up,#page-selection-down').on("page", function (event, num) {

            EnquiryLeadCountSummary.GetCallReportDetailedData(_this, num)
        });
    }

    function getHistoryByMobileNumber(obj) {

        var SelectedCount = 0;
        $('#CustomizeColumn option').each(function (value, object) {
            if ($(object).prop("selected") == true) {
                SelectedCount = SelectedCount + 1;
            }
        })

        $('#ReportTable > tbody > tr').eq(obj.RowIndex).after("<tr class='tempRow'> <td id='FeedbackCol' colspan=" + SelectedCount + 5 + ">  <table class='Feedback'>   <span style='position: absolute; right: 0px;  left: 0px; margin: auto; width: 107px;margin-top: -8px; font-size: 12px;  color: #277919;'><i style='color: #417d5c; margin-left: -30px !important; margin: auto; right: 0px; left: 0px; margin-top: -5px;position: absolute; font-size: 24px;' class='fa fa-spinner fa-spin fa-3x fa-fw'></i> Loading all History...</span> </table></td></tr>");

        $("tr.tempRow").animate({ height: 44 }, 150);

        SharedRequest = $.ajax({
            url: $("#hdnGetReportHistory").val(),
            data: obj,
            datatype: "json",
            type: "post",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            beforeSend: function () {
                if (SharedRequest != null) {
                    SharedRequest.abort();
                }
            },
            success: function (data) {

                $(".tempRow").remove();
                $('#ReportTable > tbody > tr').eq(obj.RowIndex).after("<tr class='tempRow'> <td id='FeedbackCol' colspan=" + SelectedCount + 5 + ">  <i onclick='RemoveSubTable()' class='fa fa-remove removeSubTable'></i> <table class='Feedback'>" + data + "</table></td></tr>");



            }
        });

    }



    return {
        GetEnquiryLeadSummary: getEnquiryLeadSummary,
        GetEnquirySummary: getEnquirySummary,
        GetCallReportDetailed: getCallReportDetailed,
        GetCallReportDetailedData: getCallReportDetailedData,
        GetHistoryByMobileNumber: getHistoryByMobileNumber
    }

}());

function RemoveSubTable() {
    $(".tempRow").remove();
}

function ShowHideEmptyRecords() {

    var dataIDs = jQuery("#grid").getDataIDs();
    for (i = 0; i < dataIDs.length + 1; i++) {

        var CellValue = $('#grid').jqGrid('getCell', i, '2')

        var rowData = jQuery("#grid").jqGrid('getRowData', i);
        var COLL_1_VALUE = $('#grid').jqGrid('getCell', i, '2');
        var COLL_2_VALUE = $('#grid').jqGrid('getCell', i, '3');

        if ($("#ShowEmptyRecord").is(':checked')) {
            if (CellValue == 0) {
                $("#" + i).css("display", "none");
            }
        }
        else {
            $("#" + i).removeAttr("style");
        }
    }

}