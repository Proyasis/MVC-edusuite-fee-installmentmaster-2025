var ExamSchedule = (function ()
{
    var getExamSchedule = function ()
    {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetExamScheduleListdetails").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                searchText: function ()
                {
                    return $('#txtsearch').val()
                }
                //, searchDate: function () {
                //    var date = $('#txtsearchDate').val()
                //    return date;
                //}
            },
            colNames: [Resources.RowKey,
                //Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,
                //Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,Resources.BlankSpace,              
                Resources.BranchName, Resources.Course, Resources.AffiliationsTieUps, Resources.Batch + Resources.Name,
                Resources.Subject, Resources.ExamDate, Resources.ExamTerm, Resources.NoOfStudents, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                //{ key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                //{ key: true, hidden: true, name: 'UniversityMasterKey', index: 'UniversityMasterKey', editable: true },
                //{ key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                //{ key: true, hidden: true, name: 'BatchKey', index: 'BatchKey', editable: true },
                //{ key: true, hidden: true, name: 'CourseYear', index: 'CourseYear', editable: true },
                //{ key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                //{ key: true, hidden: true, name: 'ExamTermKey', index: 'ExamTermKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'UniversityName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SubjectName', index: 'SubjectName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ExamDate', index: 'ExamDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'ExamTermName', index: 'ExamTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfStudents', index: 'NoOfStudents', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',



        })

        $("#grid").jqGrid("setLabel", "ExamSchedule", "", "thStudentsPromotion");
    }

    function editLink(cellValue, options, rowdata, action)
    {
        var temp = "'" + rowdata.RowKey + "," + rowdata.CourseKey + "," + rowdata.UniversityMasterKey + "," + rowdata.BatchKey + "," + rowdata.CourseYear + "," + rowdata.ClassDetailsKey + "," + rowdata.BranchKey + "," + rowdata.ExamTermKey + "'";

        return '<div class="divEditDelete"><a class="btn btn-primary btn-sm" href="' + $("#hdnAddEditExamSchedule").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-danger btn-sm" href="#"   onclick="javascript:deleteExamSchedule(' + rowdata.RowKey + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    function formSubmit(data)
    {
        debugger
        var $form = $("#frmExamSchedule")
        var JsonData = [];
        var formData = $form.serializeArray();
        var TimeKeys = ["ExamStartTime", "ExamEndTime"]
        var checkdata = $("[name*=IsActive]:checked")
        if ($form.valid())
        {

            if (checkdata.length > 0)
            {
                //var obj = $form.serializeArray();

                $(formData).each(function (i)
                {
                    if (formData[i])
                    {
                        var name = formData[i]['name'];
                        var keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;

                        if (TimeKeys.indexOf(keyName) > -1)
                        {
                            formData[i]['value'] = (formData[i]['value'] != "" ? moment(formData[i]['value'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
                        }
                    }
                })
                //delete formData[0];


                var dataurl = $form.attr("action");
                var response = [];

                $.ajax({
                    url: dataurl,
                    datatype: "json",
                    type: "POST",
                    contenttype: 'application/json; charset=utf-8',
                    async: false,
                    //data: $form.serializeArray(),
                    data: formData,
                    success: function (data)
                    {
                        if (typeof response == "string")
                        {
                            $("[data-valmsg-for=error_msg]").html(response);
                        }
                        else if (data.IsSuccessful)
                        {
                            $.alert({
                                type: 'green',
                                title: Resources.Success,
                                content: data.Message,
                                icon: 'fa fa-check-circle-o-',
                                buttons: {
                                    Ok: {
                                        text: Resources.Ok,
                                        btnClass: 'btn-success',
                                        action: function ()
                                        {
                                            window.location.href = $("#hdnStudentPromotionList").val();
                                        }
                                    }
                                }
                            })

                        }
                    },
                    error: function (xhr)
                    {

                    }
                });
            }
            else
            {
                $.alert({
                    type: 'orange',
                    title: Resources.Warning,
                    content: 'Please Select atleast One !',
                    icon: 'fa fa-exclamation-circle',
                    buttons: {
                        Ok: {
                            text: Resources.Ok,
                            btnClass: 'btn-warning',

                        }
                    }
                })

            }

        }
    }


    var loadData = function (json)
    {
        var model = json;

        model.AcademicTermKey = $("#AcademicTermKey").val();
        model.CourseKey = $("#CourseKey").val();
        model.UniversityMasterKey = $("#UniversityMasterKey").val();
        model.BatchKey = $("#BatchKey").val();
        model.CourseYear = $("#CourseYear").val();
        model.BranchKey = $("#BranchKey").val();
        model.ExamTermKey = $("#ExamTermKey").val();
        model.SubjectKey = $("#SubjectKey").val();
        model.RowKey = $("#RowKey").val();
        $.ajax({
            type: "POST",
            url: $("#hdnUrl").val(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            success: function (result)
            {
                if (result.IsSuccessful == false)
                {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("#dvStudentsList").html("")
                $("#dvStudentsList").html(result);

            },
            error: function (request, status, error)
            {

            }
        });
    }
    var resetExamSchedule = function (rowkey, ExamKey)
    {
        var obj = {};
        obj.RowKey = rowkey;
        obj.ExamKey = ExamKey;

        var result = EduSuite.Confirm2({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnResetExamSchedule").val(),
            parameters: obj,
            //actionValue: rowkey,
            dataRefresh: function ()
            {
                ExamSchedule.LoadData(jsonData);
            }
        });

    }

    var getBatchByClass = function (obj, ddl)
    {
        $(ddl).html("");
        $(ddl).append($('<option></option>').val("").html(Resources.Batch));
        $.ajax({
            url: $("#hdnFillBatch").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (result)
            {
                $.each(result.Batches, function (i, Batch)
                {
                    $(ddl).append(
                        $('<option></option>').val(Batch.RowKey).html(Batch.Text));
                });
            }
        });

    }

    var getclassDetails = function (obj, ddl)
    {
        $(ddl).html("");
        $.ajax(
            {
                url: $("#hdnFillSubjects").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                contentType: "application/json; charset=utf-8",
                success: function (result)
                {
                    debugger
                    $.each(result.Subjects, function (i, Subjects)
                    {
                        $(ddl).append(
                            $('<option></option>').val(Subjects.RowKey).html(Subjects.Text));
                    });


                    //$(ddl).selectpicker('refresh');

                }
            });
    }
    return {
        FormSubmit: formSubmit,
        GetExamSchedule: getExamSchedule,
        ResetExamSchedule: resetExamSchedule,
        LoadData: loadData,
        GetBatchByClass: getBatchByClass,
        GetclassDetails: getclassDetails
    }



}());

function deleteExamSchedule(RowKey)
{
    var obj = {};

    obj.RowKey = RowKey;

    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteExamSchedule").val(),
        parameters: obj,
        dataRefresh: function ()
        {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }

    });
}

function objectifyForm(formArray, returnArray)
{//serialize data function

    for (var i = 1; i < formArray.length; i++)
    {

        var subModelName = '', index = 0, keyName = ''
        var name = formArray[i]['name'];
        var tempName = name.match(/^[^\]}),]*/) ? name.match(/^[^\]}),]*/)[0] : name;
        keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;
        var arr = tempName.split('[');
        if (arr.length == 2)
        {
            subModelName = arr[0];
            index = arr[1];
        }
        else
        {
            keyName = arr[0];
        }
        if (subModelName == "")
        {
            returnArray[name] = formArray[i]['value'];
        }
        else
        {

            if (!returnArray[subModelName])
            {
                returnArray[subModelName] = [];
            }
            if (!returnArray[subModelName][index])
            {
                returnArray[subModelName][index] = $.extend(true, {}, returnArray[subModelName][0]) || {};
            }
            if ($("input[name='" + name + "']")[0] && $($("input[name='" + name + "']")[0]).is(':checkbox'))
            {
                returnArray[subModelName][index][keyName] = $($("input[name='" + name + "']")[0]).prop('checked');
            }
            else
            {
                returnArray[subModelName][index][keyName] = formArray[i]['value'];
            }
        }

    }

}















































// #region Old query
//var jsonData = [], request = null;
//$(".TableHeadId").hide();
//var ExamSchedule = (function ()
//{


//    var getExamScheduleStudents = function (json, pageIndex, resetPagination)
//    {
//        $(".TableHeadId").hide();
//        if ($("#SearchExamSubjectKey").val() != "")
//        {          
//            JsonData = json;
//            JsonData["SearchAcademicTermKey"] = $("#SearchAcademicTermKey").val();
//            JsonData["SearchCourseTypeKey"] = $("#SearchCourseTypeKey").val();
//            JsonData["SearchCourseKey"] = $("#SearchCourseKey").val();
//            JsonData["SearchUniversityKey"] = $("#SearchUniversityKey").val();
//            JsonData["SearchExamYearKey"] = $("#SearchExamYearKey").val();
//            JsonData["SearchExamSubjectKey"] = $("#SearchExamSubjectKey").val();
//            JsonData["SearchBatchKey"] = $("#SearchBatchKey").val();
//            JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
//            JsonData["PageSize"] = 10;
//            $("#dvRepeaterList").empty();
//            $("#dvScheduleContainer").mLoading();

//            request = $.ajax({
//                url: $("#hdnGetExamScheduleStudentsList").val(),
//                data: JsonData,
//                datatype: "json",
//                type: "post",
//                contenttype: 'application/json; charset=utf-8',
//                async: true,
//                beforeSend: function ()
//                {
//                    if (request != null)
//                    {
//                        request.abort();
//                    }
//                },
//                success: function (data)
//                {
//                    $("#dvRepeaterList").html(data);

//                    $(".TableHeadId").show();

//                    $("form").removeData("validator");
//                    $("form").removeData("unobtrusiveValidation");
//                    $.validator.unobtrusive.parse("form");

//                    ExamSchedule.CheckIsApplied();

//                    if (resetPagination)
//                    {
//                        ExamSchedulePagination();
//                        $('.pagination  li').removeClass("active");
//                        $('[data-lp="' + JsonData["PageIndex"] + '"]').addClass("active");
//                        $('.pagination  li.next').removeClass("active");
//                    }

//                    $("#dvScheduleContainer").mLoading("destroy");
//                },
//                error: function (error) {
//                    console.log(JSON.stringify(error));
//                    // $("#dvScheduleContainer").mLoading("destroy");
//                }
//            });
//        }
//    }



//    var getCourseTypeBySyllabus = function (obj, ddl)
//    {
//        $(ddl).html("");
//        $(ddl).append($('<option></option>').val("").html(Resources.CourseType));
//        $.ajax({
//            url: $("#hdnGetCourseTypeBySyllabus").val(),
//            type: "GET",
//            data: obj,
//            dataType: "JSON",
//            contentType: "application/json; charset=utf-8",
//            success: function (result) {
//                $.each(result.CourseTypes, function (i, CourseType)
//                {
//                    $(ddl).append(
//                        $('<option></option>').val(CourseType.RowKey).html(CourseType.Text));
//                });
//            }
//        });
//    }

//    var getCourseByCourseType = function (obj, ddl)
//    {
//        $(ddl).html("");
//        $(ddl).append($('<option></option>').val("").html(Resources.Course));
//        $.ajax(
//            {
//                url: $("#hdnGetCourseByCourseType").val(),
//                type: "GET",
//                dataType: "JSON",
//                data: obj,
//                contentType: "application/json; charset=utf-8",
//                success: function (result) {
//                    $.each(result.Courses, function (i, Course)
//                    {
//                        $(ddl).append(
//                            $('<option></option>').val(Course.RowKey).html(Course.Text));
//                    });
//                }
//            });
//    }

//    var getUniversityByCourse = function (obj, ddl)
//    {
//        $(ddl).html("");
//        $(ddl).append($('<option></option>').val("").html(Resources.University));
//        $.ajax({
//            url: $("#hdnUniversityByCourse").val(),
//            type: "GET",
//            dataType: "JSON",
//            data: obj,
//            contentType: "application/json; charset=utf-8",
//            success: function (result) {
//                $.each(result.Universities, function (i, University) {
//                    $(ddl).append(
//                        $('<option></option>').val(University.RowKey).html(University.Text));
//                });
//            }
//        });

//    }



//    var getYearsBySyllabus = function (obj, ddl)
//    {
//        $(ddl).html("");
//        $(ddl).append($('<option></option>').val("").html(Resources.Year));
//        $.ajax({
//            url: $("#hdnGetYearsBySyllabus").val(),
//            type: "GET",
//            dataType: "JSON",
//            data: obj,
//            contentType: "application/json; charset=utf-8",
//            success: function (result)
//            {
//                $.each(result.CourseYears, function (i, CourseYears)
//                {
//                    $(ddl).append($('<option></option>').val(CourseYears.RowKey).html(CourseYears.Text));
//                });
//            }
//        });
//    }

//    var getExamSubjects = function (obj, ddl)
//    {
//        $(ddl).html("");
//        $(ddl).append($('<option></option>').val("").html(Resources.Subject));
//        $.ajax({
//            url: $("#hdnGetExamSubjects").val(),
//            type: "GET",
//            dataType: "JSON",
//            data: obj,
//            contentType: "application/json; charset=utf-8",
//            success: function (result)
//            {
//                $.each(result.ExamSubjects, function (i, ExamSubjects) {
//                    $(ddl).append($('<option></option>').val(ExamSubjects.RowKey).html(ExamSubjects.Text));
//                });
//            }
//        });
//    }

//    var deleteExamSchedule = function (rowkey)
//    {
//        var result = EduSuite.Confirm({
//            title: Resources.Confirmation,
//            content: Resources.Delete_Confirm_ExamSchedule,
//            actionUrl: $("#hdnDeleteExamSchedule").val(),
//            actionValue: rowkey,
//            dataRefresh: function (Result)
//            {
//                $('[key="' + rowkey + '"] .examCentreClassDisabled').addClass("examCentreClass");
//                $('[key="' + rowkey + '"] .examTermClassDisabled').addClass("examTermClass");

//                $('[key="' + rowkey + '"] .examCentreClass').removeClass("examCentreClassDisabled");
//                $('[key="' + rowkey + '"] .examTermClass').addClass("examTermClassDisabled");
                
//                $('[key="' + rowkey + '"] .examAppliedDateClassDisabled').addClass("examAppliedDateClass");
//                $('[key="' + rowkey + '"] .examAppliedDateClass').removeClass("examCentreClassDisabled");

//                $('[key="' + rowkey + '"] input').removeAttr("disabled");
//                $('[key="' + rowkey + '"] select').removeAttr("disabled");

//                $('[key="' + rowkey + '"] input[type="text"]').val("");
//                $('[key="' + rowkey + '"] select').val("");
//                $('[key="' + rowkey + '"] .rowKeyClass').val(0);
//                $('[key="' + rowkey + '"] .ScheduleStatusClass').val("False");

//                $('[key="' + rowkey + '"] .ChkIsAppliedDisabledClass').prop('checked', false);
//                $('[key="' + rowkey + '"] .ChkIsAppliedDisabledClass').addClass("ChkIsAppliedClass");
//                $('[key="' + rowkey + '"] .ChkIsAppliedClass').removeClass("ChkIsAppliedDisabledClass");

//                $('[deleteKey="' + rowkey + '"]').remove();

//                ExamSchedule.CheckIsApplied();
//            }
//        });
//    }



//    var checkIsApplied = function ()
//    {
//        var Status = false;
//        var Count = 0;
//        $("#IsAppliedCount").html("");
//        $(".ChkIsApplied").each(function ()
//        {           
//            if(this.checked==false)
//            {
//                Status = true;
               
//            }
//            else
//            {
//                if($(this).hasClass("ChkIsAppliedClass"))
//                {
//                    Count = Count + 1;
//                }
//            }

//        });

//        if(Status==false)
//        {
//            $("#IsApplied").prop('checked', true);
//        }
//        else
//        {
//            $("#IsApplied").prop('checked', false);
//        }

//        if( Count>0)
//        {
//            $("#IsAppliedCount").html("("+Count+")");
//        }

//    }


//    //var deleteExamSchedule = function (rowkey)
//    //{
//    //    var result = confirm(Resources.Delete_Confirm_EnquiryFeedback);
//    //    if (result == true) {
//    //        var response = AjaxHelper.ajax("POST", $("#hdnDeleteExamSchedule").val(),
//    //                    {
//    //                        id: rowkey
//    //                    });

//    //        if (response.Message === Resources.Success)
//    //        {
//    //            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
//    //        }
//    //        else
//    //            alert(response.Message);
//    //        event.preventDefault();
//    //    }
//    //}



   
//    return {
//        GetExamScheduleStudents:getExamScheduleStudents,
//        GetCourseTypeBySyllabus:getCourseTypeBySyllabus,
//        GetCourseByCourseType: getCourseByCourseType,
//        GetUniversityByCourse: getUniversityByCourse,
//        GetYearsBySyllabus: getYearsBySyllabus,
//        GetExamSubjects: getExamSubjects,
//        DeleteExamSchedule: deleteExamSchedule,
//        CheckIsApplied: checkIsApplied
//    }

//}());

//function ExamSchedulePagination()
//{
   
//    $('#page-selection-down').empty();
//    var totalRecords = $("#hdnTotalRecords").val();
//    totalRecords = $("#hdnTotalRecords").val();
//    var Size = JsonData["PageSize"];
//    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

//    $('#page-selection-down').bootpag({
//        total: totalPages,
//        page: 1,
//        maxVisible: 10
//    });

//    $('#page-selection-down').on("page", function (event, num)
//    {
//        ExamSchedule.GetExamScheduleStudents(JsonData, num, false);
//    });

//}

// #endregion Old query