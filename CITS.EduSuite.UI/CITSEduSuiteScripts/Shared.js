var SharedRequest = null;
var EnquiryScheduleHistory = (function () {
    var searchPhoneNumberPopup = function (_this) {
        $.customPopupform.CustomPopup({
            load: function () {
                $("#txtMobileNumber").val("");
                $("#ClosedFeedbackList").empty();
                $("#Feedback").val("");
                $("#MobileNumberSearchHistory").show();
            }
        }, $(_this).attr("data-url"));


    }
    var getAllHistoryByMobileNumber = function (MobileNumber, SearchType) {
        var obj = {};
        obj.MobileNumber = MobileNumber;
        obj.SearchType = SearchType;
        if (SearchType != 1) {
            $("#MobileNumberSearchHistory").hide();
        }
        $.customPopupform.CustomPopup({
            ajaxData: obj,
            ajaxType: "Post",
            modalsize: "modal-lg",
        }, $("#hdnGetHistory").val());
    }


    var getProductiveCallsCount = function (obj) {
        SharedRequest = $.ajax({
            url: $("#hdnProductiveCallsCount").val(),
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
                $("#idProduuctiveCallsCount").html("Productive Calls:" + data.TotalRecords)
                $("#idProduuctiveCallsCount").css("background", data.ColorCode);
            }
        });
    }


    return {
        SearchPhoneNumberPopup: searchPhoneNumberPopup,
        GetAllHistoryByMobileNumber: getAllHistoryByMobileNumber,
        GetProductiveCallsCount: getProductiveCallsCount
    }
}());

var GetBackup = function () {

    var $form = $("#frmGetBackup")
    var obj = {};
    obj.DbName = $("#txtDbName").val();
    obj.Location = $("#txtDbLocation").val();

    if (obj.Location != null && obj.Location != "") {

        var dataurl = $form.attr("action");

        dataurl = dataurl + "?" + $.param(obj)
        $(".modal-body").mLoading();
        $.ajax({
            type: "Post",
            url: dataurl,
            success: function (response) {

                var Successmsg = "Your Database Backup Successfully Downloaded to :" + obj.Location + " " + response;
                if (response) {
                    $.alert({
                        type: 'green',
                        title: Resources.Success,
                        content: Successmsg,
                        icon: 'fa fa-check-circle-o-',
                        buttons: {
                            Ok: {
                                text: Resources.Ok,
                                btnClass: 'btn-success',
                                action: function () {
                                    $(".modal-body").mLoading("destroy");
                                    $("#frmGetBackup").closest(".modal").modal("hide")

                                }
                            }
                        }
                    })



                }
                else {
                    $(".modal-body").mLoading("destroy");
                }
            }
        })

    }
    else {
        $("#errdiv").html("Please Enter Valid FilePath");
        $("#errdiv").css("color", "red");


    }

}

var GetExportPrintData = function (type, url, filename, title) {
        $(".section-content").mLoading();
    var sortColumnName = $("#grid").jqGrid('getGridParam', 'sortname');
    var sortOrder = $("#grid").jqGrid('getGridParam', 'sortorder'); // 'desc' or 'asc'
    var rows = $("#grid").getRowData().length;
    //var row = $("#grid").jqGrid('getGridParam', 'records'); // All Records
    var page = $('#grid').getGridParam('page');


    var obj = {};
    obj.ajaxData = $("form").serializeToJSON({

    });
    obj.ajaxData.sidx = sortColumnName;
    obj.ajaxData.sord = sortOrder;
    obj.ajaxData.page = page;
    obj.ajaxData.rows = rows;
    obj.ajaxType = "POST";
    obj.ajaxUrl = $(url).val();
    obj.ContainerId = "#grid";
    obj.FileName = filename;
    obj.Title = title;
    AppCommon.ExportPrintAjax(obj, type)

   

}

var Shared = (function () {


    var getAttendanceTypeByDate = function (obj) {
        var url = $("#hdnFillAttendanceTypeByDate").val()
        var dropdDownControl = $("#AttendanceTypeKey");
        var val = $(dropdDownControl).val();
        if (!val) {
            val = obj.AttendanceTypeKey;
        }
        var btn = $("#btnSave");
        $(btn).removeAttr("disabled");
        $.ajax({
            type: "POST",
            url: url,
            dataType: "json",
            data: JSON.stringify(obj),
            contentType: "application/json; charset=utf-8",
            beforeSend: function () {
                $(dropdDownControl).empty();
                $(dropdDownControl).append($("<option loading></option>").val("").html("<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span>  Loading...</span>"));
                $(dropdDownControl).selectpicker('refresh');

            },
            success: function (response) {
                var message = response.Message;
                response = response.SelectList;
                $(dropdDownControl).append($("<option></option>").val("").html(Resources.Select + Resources.BlankSpace + Resources.AttendanceType));
                $.each(response, function () {
                    $(dropdDownControl).append($("<option></option>").val(this['RowKey']).html(this['Text']));
                });

                if (val) {
                    $(dropdDownControl).val(val)
                }
                else if (response.length == 1) {
                    val = response.map(function (item) {
                        return item.RowKey
                    });
                    $(dropdDownControl).val(val);
                }
                if (response.length == 1) {
                    $(dropdDownControl).val(response[0].RowKey).attr("disabled", true);
                }
                else {
                    if (response.length == 0) {
                        toastr.error(message, Resources.Failed)
                        $(btn).attr("disabled", true);
                    }
                    $(dropdDownControl).removeAttr("disabled");
                }
            },
            complete: function () {
                $(dropdDownControl).find("option[loading]").remove();
                $(dropdDownControl).selectpicker('refresh');
                $(dropdDownControl).trigger("change");
            }
        })
    }
    var getProfileDetails = function (url) {
        $("#dvProfileDetails").mLoading()
        $("#dvProfileDetails").load(url);
    }


    var fileViewPopUp = function (DocumentType, RowDataKey, IfDownload, FilePath) {

        var obj = {};
        obj.DocumentType = DocumentType;
        obj.RowDataKey = RowDataKey;
        obj.IfDownload = IfDownload;
        obj.FilePath = FilePath;
        var URL = $("#hdnViewFile").val();
       
        if (IfDownload == "false") {
            $.customPopupform.CustomPopup({
                ajaxType: "POST",
                ajaxData: { model: obj },
                modalsize: "modal-md  mw-100",
                load: function () {
                    setTimeout(function () {
                      
                        $("#frmfileView").removeData("validator");
                        $("#frmfileView").removeData("unobtrusiveValidation");
                        $.validator.unobtrusive.parse($("#frmfileView"));
                    }, 500)
                   

                },
                rebind: function (result) {
                   
                }
            }, URL);
          
        }
        else {

            $.ajax({
                type: "POST",
                url: $("#hdnViewFile").val(),
                data: { model: obj },
                dataType: "JSON",
                success: function (result) {
                 
                },
                error: function (request, status, error) {

                }
            });

        }

    }

    var downloadDocuments = function (DocumentType, RowDataKey, IfDownload, FilePath) {

        var obj = {};
        obj.DocumentType = DocumentType;
        obj.RowDataKey = RowDataKey;
        obj.IfDownload = IfDownload;
        obj.FilePath = FilePath;



    }

    var getDocumentTrackPopup = function (DocumentType, RowDataKey) {
        var URL = $("#hdnDocumentTrackList").val();
        var obj = {}
        obj.RowDataKey = RowDataKey;
        obj.DocumentType = DocumentType;
                     
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg mw-100 w-75",
            ajaxType: "get",
            ajaxData: obj,
            load: function () {
                Shared.GetDocumentTrack();
            },


        }, URL);

    }

    var getDocumentTrack = function () {

        $("#gridStudentView").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');

        $("#gridStudentView").jqGrid({
            url: $("#hdnGetDocumentTrackList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                SearchText: function () {
                    return $('#txtSearchText').val()
                },
                DocumentType: function () {
                    return $('#DocumentType').val()
                },
                RowDataKey: function () {
                    return $('#RowDataKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Branch,
                Resources.AdmissionNo, Resources.Name, Resources.MobileNo, Resources.Course, 
            Resources.CurrentYear, Resources.Batch, Resources.StudentStatus],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'ApplicationStatusKey', index: 'ApplicationStatusKey', editable: true },
                { key: false, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: false, hidden: true, name: 'CurrentYear', index: 'CurrentYear', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentName', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, formatter: formatCourseUniversityYear, resizable: false, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicationStatusName', index: 'ApplicationStatusName', editable: true, formatter: formatColor, cellEdit: true, sortable: true, resizable: false },
                  
            ],
            pager: jQuery('#StudentViewpager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
            altRows: true,
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
            multiselect: true,
            loadonce: false,
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altclass: 'jqgrid-altrow',
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
                //},
                //onPaging: function () {
                //    var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //    $("#grid").setGridParam({ datatype: 'json', page: CurrPage });
                //    $("#grid").trigger("reloadGrid");
            }
        });
        //jQuery("#grid").jqGrid('setFrozenColumns');
        $("#gridStudentView").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }
    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }
    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)

        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext

        if (rowdata.CourseName == null && rowdata.UniversityName == null) {
            Coursetext = "";
        }
        else if (rowdata.UniversityName == null) {
            Coursetext = rowdata.CourseName;
        }
        else if (rowdata.CourseName != null && rowdata.UniversityName != null) {
            Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName;
        }

        return Coursetext;
    }

    function formatColor(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        if (rowdata.ApplicationStatusKey == Resources.ApplicationeStatusOngoing) {
            return '<span  class="label label-success">' + cellValue + '</span>';
        }
        else if (rowdata.ApplicationStatusKey == Resources.ApplicationeStatusCompleted) {
            return '<span  class="label label-primary">' + cellValue + '</span>';
        }
        else {
            return '<span  class="label label-danger">' + cellValue + '</span>';
        }
        return cellValue;
    }

    return {
        GetAttendanceTypeByDate: getAttendanceTypeByDate,
        GetProfileDetails: getProfileDetails,
        FileViewPopUp: fileViewPopUp,
        DownloadDocuments: downloadDocuments,
        GetDocumentTrack: getDocumentTrack,
        GetDocumentTrackPopup: getDocumentTrackPopup
    }
}());