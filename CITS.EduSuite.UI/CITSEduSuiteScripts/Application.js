window.chartColors = {
    red: '#dc9999',
    orange: 'orange',
    yellow: 'yellow',
    green: '#99dc99',
    blue: 'blue',
    purple: 'purple',
    grey: 'grey'
};

var Application = (function () {

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
                    return $('#CourseKey').val()
                },
                UniversityKey: function () {
                    return $('#UniversityKey').val()
                },
                HasPhoto: function () {
                    return $("input[type=checkbox][name*=HasPhoto]").prop("checked")
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Photo, Resources.Branch, Resources.AdmissionNo, Resources.Name, Resources.MobileNo, Resources.Course,// Resources.AffiliationsTieUps,
            Resources.CurrentYear, Resources.Batch, Resources.Gender, Resources.ClassCode, Resources.StudentStatus, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'ApplicationStatusKey', index: 'ApplicationStatusKey', editable: true },
                { key: false, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: false, hidden: true, name: 'CurrentYear', index: 'CurrentYear', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, name: 'ApplicantPhoto', index: 'ApplicantPhoto', formatter: formatPhoto, sortable: false, resizable: false, width: 60 },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicantName', index: 'ApplicantName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, formatter: formatCourseUniversityYear, resizable: false, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Gender', index: 'Gender', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
                { key: false, name: 'ClassDetailsName', index: 'ClassDetailsName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicationStatusName', index: 'ApplicationStatusName', editable: true, formatter: formatColor, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 100, align: 'center' },

            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [10, 50, 100, 250, 500, 1000],
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
            rownumbers: true,
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
        }).contextMenu({
            selector: ".jqgrow .context-menu",
            trigger: 'left',
            build: function ($trigger, e) {
                // this callback is executed every time the menu is to be shown
                // its results are destroyed every time the menu is hidden
                // e is the original contextmenu event
                var $tr = $(e.target).closest("tr.jqgrow"),
                    rowid = $tr.attr("id"),
                    item = $("#grid").jqGrid("getRowData", rowid);
                var items = {
                    E: { name: Resources.Edit, icon: "fa-edit" },
                    V: { name: Resources.View, icon: "fa-eye" },
                    D: { name: Resources.Delete, icon: "fa-trash" },
                    AS: { name: Resources.ApplicationSchedule, icon: "fa-phone" }
                }
                var IsApplicationSchedule = $("#IsApplicationSchedule").val();
                IsApplicationSchedule = IsApplicationSchedule ? JSON.parse(IsApplicationSchedule.toLowerCase()) : false;
                if (!IsApplicationSchedule) {
                    delete items.AS;
                }
                return {
                    callback: function (key, options) {

                        var href = "";
                        switch (key) {
                            case "E":
                                href = "AddEditApplication/" + rowid
                                window.location.href = href;
                                break;
                            case "V":
                                href = $("#hdnViewApplication").val() + "/" + rowid
                                window.location.href = href;
                                break;
                            case "D":
                                deleteApplication(rowid);
                                break;
                            case "AS":
                                ApplicationSchedule.EditApplicationSchedulePopup(rowid, 0, Resources.ScheduleTypeApplication)
                                break;
                            default:
                                href = "";

                        }
                    },
                    items: items
                }

            }
        });
        //jQuery("#grid").jqGrid('setFrozenColumns');
        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
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

    var getInterestedEnquiry = function () {
        $("#gridInterested").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#gridInterested").jqGrid({
            url: $("#hdnGetInterestedEnquiry").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                SearchText: function () {
                    return $('#txtSearchInterestedName').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.Name, Resources.MobileNo, Resources.Email, Resources.Course, Resources.Date, Resources.CreatedBy, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'UniversityName', index: 'UniversityName', editable: true },
                { key: false, name: 'EnquiryName', index: 'EnquiryName', editable: true, cellEdit: true, sortable: false, resizable: false },
                { key: false, name: 'PhoneNumber', index: 'PhoneNumber', editable: true, cellEdit: true, sortable: false, resizable: false },
                { key: false, name: 'EmailAddress', index: 'EmailAddress', editable: true, cellEdit: true, sortable: false, resizable: false },
                //{ key: false, name: 'AcademicTermName', index: 'AcademicTermName', editable: true, cellEdit: true, sortable: false, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: false, resizable: false, formatter: formatCourseUniversityYear },
                { key: false, name: 'NextCallSchedule', index: 'NextCallSchedule', editable: true, cellEdit: false, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: false, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editInterestedLink, resizable: false, width: 150 },
            ],
            pager: jQuery('#pagerInterested'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
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
            altRows: true,
            altclass: 'jqgrid-altrow',


        })

        $("#gridInterested").jqGrid("setLabel", "Name", "", "thName");
    }

    var getDepartmentByBranchId = function (Id) {
        $.ajax({
            url: $("#hdnGetDepartmentByBranchId").val(),
            type: "GET",
            dataType: "JSON",
            data: { id: Id },
            success: function (result) {
                $("#ddlDepartment").html(""); // clear before appending new list 
                $("#ddlDepartment").append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Department));
                $.each(result.Departments, function (i, Department) {
                    $("#ddlDepartment").append(
                        $('<option></option>').val(Department.RowKey).html(Department.Text));
                });
            }
        });
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

    function formatGender(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        if (rowdata.Gender == 2) {
            return '<span  ><i class="fa fa-female w3-text-pink" aria-hidden="true"></i> ' + "Female" + '</span>';
        }
        else {
            return '<span  ><i class="fa fa-male w3-text-indigo" aria-hidden="true"></i> ' + "Male" + '</span>';
        }
        return cellValue;
    }

    function formatPhoto(cellValue, options, rowdata, action) {
        var ApplicantPhoto = rowdata.ApplicantPhoto.substring(1, rowdata.ApplicantPhoto.length);
        ApplicantPhoto = "'" + ApplicantPhoto + "'";
        if (rowdata.StudentPhotoPath != null) {
            return '<img src = ' + ApplicantPhoto + '  style="width: 30px;height: 30px;border-radius:100%;border:solid 1px;">';
        }
        else {
            var ApplicantPhoto = "'/Images/avatar-2-64.png'";
            return '<img src = ' + ApplicantPhoto + ' style="width: 30px;height: 30px;border-radius:100%">';
        }
        //var url = rowdata.StudentPhotoPath ? ApplicantPhoto : "'/Images/avatar-2-64.png'";

    }
    var getEmployeesByBranchId = function (Id, ddl) {
        $(ddl).html(""); // clear before appending new list 
        $(ddl).append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Employee));
        $(ddl).select2("val", "");

        var response = AjaxHelper.ajax("GET", $("#hdnGetEmployeesByBranchId").val() + "/" + Id);
        var $optgroup;
        $.each(response, function (i, Employee) {

            //if (response[i - 1] == undefined || Employee.GroupName != response[i - 1]["GroupName"]) {
            //    $optgroup = "";
            //    $optgroup = $("<optgroup>", {
            //        label: Employee.GroupName
            //    });

            //    $optgroup.appendTo(ddl);
            //}
            var $option = $("<option>", {
                text: Employee.Text,
                value: Employee.RowKey
            });
            $option.appendTo(ddl);

        });

    }

    var reloadData = function (json) {
        var target = $("#tab-application li a.active").attr("data-href")
        if (target) {
            $("#tabContent").load(target, function (result) {
                EnableTabs();
                ContentLoad();
                // Application.AfterFormSubmit();

            });
        } else {
            window.location.reload();
        }
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><button type="button" class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</button></div>'
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="AddEditApplication' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm"  onclick="javascript:deleteApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a><a class="btn btn-warning btn-sm" href="' + $("#hdnViewApplication").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';
    }

    function editInterestedLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" onclick="Application.FetchApplicationFromEnquiry(this,' + temp + ')"><i class="fa fa-download" aria-hidden="true"></i>' + Resources.FetchData + '</a></div>';
    }

    var uploadPhoto = function (btn) {
        var form = $(btn).closest("form")[0];
        url = $(form).attr("action");
        if ($(form).valid()) {
            var canvas = $("#canvas")[0];

            Application.SubmitPhoto(url, AppCommon.DataURItoBlob(canvas.toDataURL()));

        }

    }

    var submitPhoto = function (url, file) {
        var obj = {};
        obj.PhotoFile = AppCommon.BlobToFile(file, $("#ApplicationKey").val(), file.type);
        obj.ApplicationKey = $("#ApplicationKey").val();
        response = AjaxHelper.ajaxWithFile("model", "POST", url,
            {
                model: obj
            });

        if (response.IsSuccessful == true) {
            toastr.success(Resources.Success, response.Message);
            if (location.search.indexOf('q=') == 0)
                window.location.href = $("#hdnApplicationList").val();
            else
                Application.ReloadData();
        }
        else {
            $("[data-valmsg-for=error_msg_payment]").html(response.Message);
        }

    }

    var deleteApplicantPhoto = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationPhoto,
            actionUrl: $("#hdnDeleteApplicantPhoto").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                Application.ReloadData();
            }
        });
    }

    var fetchApplicationFromEnquiry = function (_this, id) {
        DisableTabs();
        $(_this).closest(".modal").modal("hide");
        $(".tab-content").mLoading();
        $(".tab-content").load($("#hdnFetchApplicationFromEnquiry").val() + "/" + id, function (result) {
            ContentLoad();
            EnableTabs();
            $(".tab-content").mLoading("destroy");

        });
    }

    var editSchedulePopup = function (rowid) {
        var URL = $("#hdnAddEditApplicationSchedule").val() + "/" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {

            },
            rebind: function (result) {

            }
        }, URL);
    }

    var getEnrollmentDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                Application.ItemLoad($(this));
                //$("[id*=ApplicationKey]", $(this)).val("").removeAttr("disabled")
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    //deleteSalesOrderInvoiceItem($(hidden).val(), $(this));
                }
                else {
                    $(this).slideUp(remove);
                }
            },
            rebind: function (response) {
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    $("#frmAddEditEnrollmentNo").closest(".modal").modal("hide")
                    $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                }

            },
            data: json,
            repeatlist: 'EnRollmentNoDetailsViewModel',
            submitButton: '#btnSave',
            defaultValues: json
        });
    }

    var itemLoad = function (_this) {
        $("[id*=ApplicationKey]", $(_this)).on("change", function () {
            Application.GetStudentDetailsByStudentKey($(this).val(), $(_this));
        });
    }

    var getStudentDetailsByStudentKey = function (Id, item) {
        if (Id != "") {

            $.ajax({
                url: $("#hdnGetStudentDetailsByStudentKey").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: { id: Id },
                success: function (result) {
                    $.each(result.StudentDetailsModel, function (i, StudentDetailsModel) {



                        $("[id*=AdmissionNo]", $(item)).val(StudentDetailsModel.AdmissionNo);
                        $("[id*=StudentEnrollmentNo]", $(item)).val(StudentDetailsModel.StudentEnrollmentNo);
                        $("[id*=ExamRegisterNo]", $(item)).val(StudentDetailsModel.ExamRegisterNo);

                        //$("[id*=AdmissionNo]", $(item)).attr("disabled", true)
                        //$("[id*=StudentEnrollmentNo]", $(item)).attr("disabled", true)
                        //$("[id*=ExamRegisterNo]", $(item)).attr("disabled", true)
                    });

                }
            });

        }
    }

    var getAttendanceAcademicPerfomance = function () {
        var Id = $("#RowKey").val();
        if (Id != "") {

            $.ajax({
                url: $("#hdnAttendanceAcademicPerfomance").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: { id: Id },
                success: function (result) {
                    $(result).each(function (i) {
                        result[i] = $(this).map(function (n, item) {
                            var obj = {};
                            $(item).each(function () {
                                obj[this.Key] = this.Value;
                            });
                            return obj;
                        });
                    })
                    $("[data-performanceyear]").each(function () {
                        var dataSet = this.dataset;
                        var resultData = $(result[0]).filter(function (n, item) {
                            return item.StudentYear == dataSet.performanceyear;
                        })[0];
                        var resultData1 = $(result[1]).filter(function (n, item) {
                            return item.ExamYear == dataSet.performanceyear;
                        }).toArray();
                        if (typeof resultData != "undefined") {
                            var PresentPercent = AppCommon.RoundTo((resultData.Present * 100 / resultData.TotalCount), Resources.RoundToDecimalPostion)

                            var AbsentPercent = AppCommon.RoundTo((resultData.Absent * 100 / resultData.TotalCount), Resources.RoundToDecimalPostion)

                            drawPieChart($("#dvAttendance", this), [parseFloat(resultData.Present).toString(), parseFloat(resultData.Absent).toString()], ["Present (" + PresentPercent + "%)", "Absent (" + AbsentPercent + "%)"], resultData.ClassDetailsKey)
                        }
                        loadExamResultData($("#tableExamResult", this), resultData1);
                    })

                }
            });

        }
    }

    var applicationAttendancePopup = function (id) {
        var obj = {};
        obj.ApplicationKey = $("#RowKey").val();
        obj.ClassKeys = [];
        obj.ClassKeys.push(id)
        $.customPopupform.CustomPopup({
            ajaxType: "post",
            ajaxData: obj,
            modalsize: "modal-xl  mw-100 w-100"

        }, $("#hdnGetApplicationAttendance").val());

    }

    var updateStudentPhoneNo = function (RowKey) {
        var url = $("#hdnUpdateStudentPhonNo").val() + "/" + RowKey;
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {

            },
            rebind: function (result) {
                window.location.reload();
            }
        }, url);
    }

    var getApplicationDetails = function (Id) {



        $('#dvContent').html("");
        $('.section-content').mLoading();
        $.ajax({
            type: "Get",
            url: $("#hdnGetStudentNavigation").val(),
            data: { id: Id },
            success: function (data) {
                var resultData = {};
                resultData = data;
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnGetStudentNavigationPath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(resultData);
                        $('#dvStudentNav').html(html);
                        if (Id) {



                            ajaxRequest = $.ajax({
                                type: "GET",
                                url: $("#hdnViewApplicationDetails").val(),
                                contentType: "application/json; charset=utf-8",
                                data: { id: Id },

                                success: function (result) {
                                    if (result.IsSuccessful == false) {
                                        $("[data-valmsg-for=error_msg]").html(result.Message);

                                    }
                                    $('.section-content').mLoading("destroy");
                                    $("#DivApplicationDetails").html("")
                                    $("#DivApplicationDetails").html(result);
                                },
                                error: function (request, status, error) {
                                    $('.section-content').mLoading("destroy");
                                }
                            });
                        }
                    },
                    error: function (xhr) {
                        $('.section-content').mLoading("destroy");
                    },
                    complete: function () {
                    }
                })
            }
        });



    }


    return {
        GetApplication: getApplication,
        GetInterestedEnquiry: getInterestedEnquiry,
        GetDepartmentByBranchId: getDepartmentByBranchId,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        ReloadData: reloadData,
        UploadPhoto: uploadPhoto,
        SubmitPhoto: submitPhoto,
        DeleteApplicantPhoto: deleteApplicantPhoto,
        FetchApplicationFromEnquiry: fetchApplicationFromEnquiry,
        EditSchedulePopup: editSchedulePopup,
        GetEnrollmentDetails: getEnrollmentDetails,
        GetStudentDetailsByStudentKey: getStudentDetailsByStudentKey,
        ItemLoad: itemLoad,
        GetAttendanceAcademicPerfomance: getAttendanceAcademicPerfomance,
        ApplicationAttendancePopup: applicationAttendancePopup,
        UpdateStudentPhoneNo: updateStudentPhoneNo,
        GetApplicationDetails: getApplicationDetails
    }

}());

function deleteApplication(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Application,
        actionUrl: $("#hdnDeleteApplication").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

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
    $(".main-table").clone(true).appendTo('#table-scroll').addClass('clone');
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

function drawPieChart(chartArea, data, labels, eventKey) {

    var config = {
        type: 'pie',
        data: {
            datasets: [{
                data: data,
                backgroundColor: [

                    window.chartColors.green,
                    window.chartColors.red
                ],
                label: 'Attendance'
            }],
            labels: labels
        },
        options: {
            responsive: false,
            bezierCurve: false,
            animation: false,
            showDatapoints: true,
            maintainAspectRatio: false,
            legend: {
                display: true,
                position: 'bottom',
                labels: {
                    fontColor: "black",
                    fontStyle: "bold",
                }
            },
            layout: {
                padding: {
                    left: 50,
                    right: 0,
                    top: 0,
                    bottom: 0
                }
            }
        }
    };


    var ctx = $(chartArea).find("canvas")[0].getContext('2d');
    $($(chartArea).find("canvas")).on("click", function (evn) {
        Application.ApplicationAttendancePopup(eventKey)
    });
    window.PieChart = new Chart(ctx, config);



}
function loadExamResultData($table, data) {
    var cols = [{
        title: 'Subject',
        field: 'SubjectName'
    }];
    var keys = $.extend({}, data[0], true);
    if (keys) {
        delete keys.SubjectName;
        delete keys.ExamYear;
        for (var key in keys) {
            cols.push({
                title: key,
                field: key,
                formatter: function (value) {
                    if (value) {
                        var arr = value.split('|')
                        var Mark = arr[0];
                        var Status = arr[1];
                        var Max = arr[2];

                        var color = "";
                        if (Status == "P")
                            color = "green";
                        else if (Status == "F")
                            color = "red";
                        else if (Status == "A") {
                            color = "orange";
                            Mark = Status;
                        }
                        return '<div style="color: ' + color + '">' + Mark + '</div>'
                    }
                }
            })
        }
        $table.bootstrapTable({
            data: data, columns: cols
        })
    }
}




