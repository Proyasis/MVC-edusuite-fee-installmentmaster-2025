var ExamResult = (function () {

    var getExamResult = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetExamResult").val(),
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
                }


            },
            colNames: [Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace,
            Resources.Blankspace, Resources.Blankspace, Resources.Branch, Resources.Course, Resources.YearOrSem,
            Resources.Batch, Resources.ExamTerm, Resources.NoOfSubjects, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                { key: true, hidden: true, name: 'UniversityMasterKey', index: 'UniversityMasterKey', editable: true },
                { key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: true, hidden: true, name: 'BatchKey', index: 'BatchKey', editable: true },
                { key: true, hidden: true, name: 'ExamTermKey', index: 'ExamTermKey', editable: true },
                { key: true, hidden: true, name: 'CurrentYear', index: 'CurrentYear', editable: true },
                { key: true, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ExamTermName', index: 'ExamTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfSubject', index: 'NoOfSubject', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'BranchKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
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
            ignoreCase: true,
            sortname: 'BranchKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            },
            onPaging: function () {
                var CurrPage = $(".ui-pg-input", $("#pager")).val();
                $("#grid").setGridParam({ datatype: 'json', page: CurrPage });
                $("#grid").trigger("reloadGrid");
            }
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

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
                }

            },
            colNames: [Resources.RowKey, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace,
            Resources.AdmissionNo, Resources.Name, Resources.MobileNo, Resources.Course, Resources.Batch,
            Resources.CurrentYear, Resources.NoOfSubjects, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: false, hidden: true, name: 'CurrentYear', index: 'CurrentYear', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicantName', index: 'ApplicantName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 }, { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'NoOfCertificate', index: 'NoOfCertificate', editable: true, cellEdit: true, sortable: true, resizable: false },

                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editindivitualLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
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
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    $(this).popupform
                        ({
                            modalsize: 'modal-xl'
                        });
                })

            },
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            },
            onPaging: function () {
                var CurrPage = $(".ui-pg-input", $("#pager")).val();
                $("#grid").setGridParam({ datatype: 'json', page: CurrPage });
                $("#grid").trigger("reloadGrid");
            }
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }

    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }

    var getSubjectDetils = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetSubjectDetails").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                BranchKey: function () { return $('#BranchKey').val() },
                AcademicTermKey: function () { return $('#AcademicTermKey').val() },
                CourseKey: function () { return $('#CourseKey').val() },
                UniversityMasterKey: function () { return $('#UniversityMasterKey').val() },
                BatchKey: function () { return $('#BatchKey').val() },
                ExamYear: function () { return $('#ExamYear').val() },
                ExamTermKey: function () { return $('#ExamTermKey').val() },

            },
            colNames: [Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace,
            Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace,
            Resources.BookName, Resources.Passed, Resources.Failed, Resources.Absent, Resources.Action],

            colModel: [
                { key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                { key: true, hidden: true, name: 'UniversityMasterKey', index: 'UniversityMasterKey', editable: true },
                { key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: true, hidden: true, name: 'BatchKey', index: 'BatchKey', editable: true },
                { key: true, hidden: true, name: 'ExamTermKey', index: 'ExamTermKey', editable: true },
                { key: true, hidden: true, name: 'ExamYear', index: 'ExamYear', editable: true },
                { key: true, hidden: true, name: 'SubjectKey', index: 'SubjectKey', editable: true },
                { key: true, hidden: true, name: 'ExamScheduleKey', index: 'ExamScheduleKey', editable: true },
                { key: false, name: 'SubjectName', index: 'SubjectName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Passed', index: 'Passed', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Failed', index: 'Failed', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Absent', index: 'Absent', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: StudentsLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
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
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    $(this).popupform
                        ({
                            modalsize: 'modal-lg'
                        });
                })

            },
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            },
            onPaging: function () {
                var CurrPage = $(".ui-pg-input", $("#pager")).val();
                $("#grid").setGridParam({ datatype: 'json', page: CurrPage });
                $("#grid").trigger("reloadGrid");
            }

        });
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.BranchKey + "," + rowdata.AcademicTermKey + "'";
        var obj = {};
        obj.BranchKey = rowdata.BranchKey;
        obj.AcademicTermKey = rowdata.AcademicTermKey;
        obj.CourseKey = rowdata.CourseKey;
        obj.UniversityMasterKey = rowdata.UniversityMasterKey;
        obj.BatchKey = rowdata.BatchKey;
        obj.ExamYear = rowdata.CurrentYear;
        obj.ExamTermKey = rowdata.ExamTermKey;
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnGetAllSubjectDetails").val() + '?' + $.param(obj) + '"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Add + '</a></div>';
    }

    function editindivitualLink(cellValue, options, rowdata, action) {

        return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm" onclick="ExamResult.EditIndivdualMarkPopup(' + rowdata.RowKey + ')"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Add + '</a></div>';
    }

    var editIndivdualMarkPopup = function (rowid) {
        var URL = $("#hdnAddEditStudentsMarkListIndividual").val() + "/" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg mw-100 w-75",
            load: function () {

            },
            rebind: function (result) {

            }
        }, URL);
    }

    function StudentsLink(cellvalue, options, rowdata, action) {

        var obj = {};
        obj.BranchKey = rowdata.BranchKey;
        obj.AcademicTermKey = rowdata.AcademicTermKey;
        obj.CourseKey = rowdata.CourseKey;
        obj.UniversityMasterKey = rowdata.UniversityMasterKey;
        obj.BatchKey = rowdata.BatchKey;
        obj.ExamYear = rowdata.ExamYear;
        obj.ExamTermKey = rowdata.ExamTermKey;
        obj.SubjectKey = rowdata.SubjectKey;
        var objDelete = [rowdata.ExamScheduleKey, rowdata.SubjectKey]
        var jsonstring = "'" + $.param(obj) + "'"
        var objEdit = [rowdata.BranchKey, rowdata.AcademicTermKey, rowdata.CourseKey, rowdata.UniversityMasterKey, rowdata.BatchKey, rowdata.ExamYear, rowdata.ExamTermKey, rowdata.SubjectKey]
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm" data-href="' + $("#hdnAddEditStudentsMarkList").val() + '?' + $.param(obj) + '"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Add + '</a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteExamResult(' + objDelete.join(",") + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i>' + Resources.Delete + '</a></div>';
        return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm mr-2" onclick="ExamResult.EditSubjectMarkPopup(' + jsonstring + ')"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Add + '</a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteExamResult(' + objDelete.join(",") + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i>' + Resources.Delete + '</a></div>';

    }

    var editSubjectMarkPopup = function (obj) {
        var URL = $("#hdnAddEditStudentsMarkList").val() + "?" + obj;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg mw-100 w-75",
            load: function () {

            },
            rebind: function (result) {

            }
        }, URL);
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

    var resetExamResult = function (rowkey, ApplicationKey, bulk) {


        if (!bulk) {
            var $form = $("#frmStudentsmarkList")
            var formData = $form.serializeToJSON({ associativeArrays: false });
            var obj = {};
            obj.BranchKey = formData.BranchKey;
            obj.AcademicTermKey = formData.AcademicTermKey;
            obj.CourseKey = formData.CourseKey;
            obj.UniversityMasterKey = formData.UniversityMasterKey;
            obj.BatchKey = formData.BatchKey;
            obj.ExamYear = formData.ExamYear;
            obj.ExamTermKey = formData.ExamTermKey;
            obj.SubjectKey = formData.SubjectKey;

            var jsonstring = "" + $.param(obj) + ""
        }

        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnResetExamResult").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                if (bulk) {
                    $("#frmStudentsmarkList").closest(".modal").modal("hide");
                    ExamResult.EditIndivdualMarkPopup(ApplicationKey);
                    $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                }
                else {
                    $("#frmStudentsmarkList").closest(".modal").modal("hide");
                    ExamResult.EditSubjectMarkPopup(jsonstring);
                    $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                }
            }
        });
    }


    var bindExamStatus = function (item) {
        var ExamStatus = $("select[id*=ExamStatus]", $(item));

        var ExamStatuses = [
            { RowKey: "", Text: Resources.Select },
            { RowKey: Resources.ExamStatusRegulerValue, Text: "Regular" },
            { RowKey: Resources.ExamStatusSupplyValue, Text: "Supply" },
            { RowKey: Resources.ExamStatusImprovementValue, Text: "Improvement" },
        ];
        $(ExamStatus).each(function () {
            var ddl = this;
            item = $(ddl).closest("[data-repeater-item]")
            var value = $(ddl).val();
            $(ddl).empty();
            var ResultStatus = $("input[id*=ResultStatus]", $(item)).val();
            var selectedList = ExamStatuses;
            if (ResultStatus == 'A' || ResultStatus == 'F') {
                selectedList = ExamStatuses.filter(function (selectItem) {
                    return selectItem.RowKey == Resources.ExamStatusSupplyValue;
                });
            }
            else if (ResultStatus == 'P') {
                selectedList = ExamStatuses.filter(function (selectItem) {
                    return selectItem.RowKey && selectItem.RowKey != Resources.ExamStatusSupplyValue;
                });
            } else {
                value = "";
            }

            selectedList.forEach(function (selectItem) {
                $(ddl).append($("<option></option>").val(selectItem['RowKey']).html(selectItem['Text']));
            })
            $(ddl).val(value).selectpicker("refresh");

        });


    }
    function formSubmit(bulk) {

        var $form = $("#frmStudentsmarkList")
        $("[disabled]", $form).removeAttr("disabled");

        var formData = $form.serializeToJSON({ associativeArrays: false });

        formData["ExamResultDetail"] = formData["ExamResultDetail"].filter(function (item) {
            return item.ResultStatus != "N";
        });


        if ($form.valid()) {
            if (formData["ExamResultDetail"].length) {


                var dataurl = $form.attr("action");
                var response = [];

                response = AjaxHelper.ajax("POST", dataurl,
                    {
                        model: formData
                    });
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
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
                                    if (bulk) {
                                        window.location.href = $("#hdnExamResultList").val();
                                    }
                                    else {
                                        $("#frmStudentsmarkList").closest(".modal").modal("hide");
                                        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                                    }

                                }
                            }
                        }
                    })

                }

            }
            else {
                var Message = "Please add atleast one recourd";
                EduSuite.AlertMessage({ title: Resources.Warning, content: Message, type: 'orange' })
            }
        }
    }



    return {
        GetExamResult: getExamResult,
        GetSubjectDetils: getSubjectDetils,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        GetApplication: getApplication,
        ResetExamResult: resetExamResult,
        FormSubmit: formSubmit,
        EditIndivdualMarkPopup: editIndivdualMarkPopup,
        EditSubjectMarkPopup: editSubjectMarkPopup,
        BindExamStatus: bindExamStatus

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


function deleteExamResult(ExamScheduleKey, SubjectKey) {

    var obj = {};
    obj.ExamScheduleKey = ExamScheduleKey;
    obj.SubjectKey = SubjectKey;


    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteExamResult").val(),
        parameters: obj,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }

    });
}



