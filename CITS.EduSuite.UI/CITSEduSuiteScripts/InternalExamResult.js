var InternalExamResult = (function () {

    var getInternalExamResult = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetInternalExamResult").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                SearchEmployeeKey: function () {
                    return $('#SearchEmployeeKey').val()
                },
                BranchKey: function () {
                    return $('#SearchBranchKey').val()
                },
                ClassDetailsKey: function () {
                    return $('#ClassDetailsKey').val()
                },
                BatchKey: function () {
                    return $('#BatchKey').val()
                }

            },
            colNames: [Resources.InternalExamKey, Resources.Blankspace, Resources.Branch, Resources.ClassCode, Resources.AffiliationsTieUps, Resources.Course,
                Resources.Batch, Resources.AcademicTerm,Resources.ExamTerm,  Resources.NoOfSubjects, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'InternalExamKey', index: 'InternalExamKey', editable: true },
                { key: true, hidden: true, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ClassDetailsName', index: 'ClassDetailsName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'UniversityName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ExamYearText', index: 'ExamYearText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'InternalExamTermName', index: 'InternalExamTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfSubject', index: 'NoOfSubject', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
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
    
    var getSubjectDetils = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetSubjectDetails").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                InternalExamKey: function () {
                    return $('#InternalExamKey').val()
                },
                ClassDetailsKey: function () {
                    return $('#ClassDetailsKey').val()
                },
            },
            colNames: [Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,
                Resources.SubjectName, Resources.Passed, Resources.Failed, Resources.Absent, Resources.Action],

            colModel: [
            { key: false, hidden: true, name: 'InternalExamKey', index: 'InternalExamKey', editable: true },
                { key: false, hidden: true, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
                { key: false, hidden: true, name: 'AddedResults', index: 'AddedResults', editable: true },
                { key: true, hidden: true, name: 'SubjectKey', index: 'SubjectKey', editable: true },
                { key: true, hidden: true, name: 'ExamYear', index: 'ExamYear', editable: true },
                { key: true, hidden: true, name: 'InternalExamDetailsKey', index: 'InternalExamDetailsKey', editable: true },
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
        var temp = "'" + rowdata.InternalExamKey + "," + rowdata.ClassDetailsKey + "'";
        var obj = {};
        obj.InternalExamKey = rowdata.InternalExamKey;
        obj.ClassDetailsKey = rowdata.ClassDetailsKey;
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnGetAllSubjectDetails").val() + '?' + $.param(obj) + '"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Add + '</a></div>';
    }

    function StudentsLink(cellvalue, options, rowdata, action) {
        var temp = "'" + rowdata.InternalExamKey + "," + rowdata.SubjectKey + "," + rowdata.ClassDetailsKey + "'";
        var obj = {};
        obj.InternalExamKey = rowdata.InternalExamKey;
        obj.SubjectKey = rowdata.SubjectKey;
        obj.ClassDetailsKey = rowdata.ClassDetailsKey;
        var objDelete = [rowdata.InternalExamKey, rowdata.SubjectKey, rowdata.InternalExamDetailsKey]
        var objEdit = [rowdata.InternalExamKey, rowdata.SubjectKey, rowdata.ClassDetailsKey]
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm" data-href="' + $("#hdnAddEditStudentsMarkList").val() + '?' + $.param(obj) + '"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Add + '</a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteInternalExamResult(' + objDelete.join(",") + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
        if (rowdata.AddedResults > 0) {
            return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm mr-1" onclick="InternalExamResult.AddEditMark(' + objEdit.join(",") + ')"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Add + '</a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteInternalExamResult(' + objDelete.join(",") + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
        }
        else {
            return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm mr-1" onclick="InternalExamResult.AddEditMark(' + objEdit.join(",") + ')"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Add + '</a></div>';

        }

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

    var addEditMark = function (InternalExamKey, SubjectKey, ClassDetailsKey) {
        var obj = {};
        obj.InternalExamKey = InternalExamKey;
        obj.SubjectKey = SubjectKey;
        obj.ClassDetailsKey = ClassDetailsKey;

        var URL = $("#hdnAddEditStudentsMarkList").val() + "?" + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg mw-100 w-75",
            load: function () {

            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);
    }

    return {
        GetInternalExamResult: getInternalExamResult,
        GetSubjectDetils: getSubjectDetils,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        AddEditMark: addEditMark
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
       
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');

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


function deleteInternalExamResult(InternalExamKey, SubjectKey, InternalExamDetailsKey) {
    
    var obj = {};
    obj.InternalExamKey = InternalExamKey;
    obj.InternalExamDetailsKey = InternalExamDetailsKey;
    obj.SubjectKey = SubjectKey;


    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteInternalExamResult").val(),
        parameters: obj,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }

    });
}



