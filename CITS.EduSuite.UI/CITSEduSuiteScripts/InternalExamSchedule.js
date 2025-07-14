var InternalExamSchedule = (function ()
{
    var getInternalExamSchedule = function ()
    {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetInternalExamSchedule").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                SearchText: function ()
                {
                    return $('#txtsearch').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                BatchKey: function () {
                    return $('#BatchKey').val()
                }               
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,
                Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Branch,
                Resources.Course, Resources.AcademicTerm, Resources.Batch, Resources.ExamTerm, Resources.ClassCode,
                Resources.NoOfSubjects, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                { key: true, hidden: true, name: 'UniversityMasterKey', index: 'UniversityMasterKey', editable: true },
                { key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: true, hidden: true, name: 'BatchKey', index: 'BatchKey', editable: true },
                { key: true, hidden: true, name: 'CourseYear', index: 'CourseYear', editable: true },
                { key: true, hidden: true, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
                { key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: true, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: true, hidden: true, name: 'InternalExamTermKey', index: 'InternalExamTermKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'InternalExamTermName', index: 'InternalExamTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ClassDetailsNames', index: 'ClassDetailsNames', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfSubjects', index: 'NoOfSubjects', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        $("#grid").jqGrid("setLabel", "InternalExamSchedule", "", "thStudentsPromotion");
    }

    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }
    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CourseYear, rowdata.AcademicTermKey)
    }

    function editLink(cellValue, options, rowdata, action)
    {
        var temp = "'" + rowdata.RowKey + "," + rowdata.CourseKey + "," + rowdata.UniversityMasterKey + "," + rowdata.BatchKey + "," + rowdata.CourseYear + "," + rowdata.ClassDetailsKey + "," + rowdata.BranchKey + "," + rowdata.InternalExamTermKey + "'";

        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mr-1" href="' + $("#hdnAddEditInternalExamSchedule").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteInternalExamSchedule(' + rowdata.RowKey + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    function formSubmit(data)
    {
        
        var $form = $("#frmInternalExamSchedule")
        var JsonData = [];
        var formData = $form.serializeArray();
        var TimeKeys = ["ExamStartTime", "ExamEndTime"]
        var checkdata = $("[name*=ExamStatus]:checked")
        if ($form.valid())
        {

            if (checkdata.length > 0)
            {
                $('#btnSave').hide();
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
                            $('#btnSave').show();
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
                                            $('#btnSave').show();
                                        }
                                    }
                                }
                            })

                        }
                    },
                    error: function (xhr)
                    {
                        $('#btnSave').show();
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
        model.DivisionKeys = $("#DivisionKeys").val()//.split(",");
        model.BranchKey = $("#BranchKey").val();
        model.InternalExamTermKey = $("#InternalExamTermKey").val();
        model.ClassDetailsKeys = $("#ClassDetailsKeys").val();
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

    var resetInternalExamSchedule = function (rowkey, InternalExamKey)
    {
        var obj = {};
        obj.RowKey = rowkey;
        obj.InternalExamKey = InternalExamKey;

        var result = EduSuite.Confirm2({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnResetInternalExamSchedule").val(),
            parameters: obj,
            //actionValue: rowkey,
            dataRefresh: function ()
            {
                InternalExamSchedule.LoadData(jsonData);
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
                url: $("#hdnFillClassDetails").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                //contentType: "application/json; charset=utf-8",
                success: function (result)
                {
                    
                    $.each(result.ClassDetails, function (i, ClassDetails)
                    {
                        $(ddl).append(
                            $('<option></option>').val(ClassDetails.RowKey).html(ClassDetails.Text));
                    });


                    $(ddl).selectpicker('refresh');

                }
            });
    }
    return {
        FormSubmit: formSubmit,
        GetInternalExamSchedule: getInternalExamSchedule,
        ResetInternalExamSchedule: resetInternalExamSchedule,
        LoadData: loadData,
        GetBatchByClass: getBatchByClass,
        GetclassDetails: getclassDetails
    }



}());

function deleteInternalExamSchedule(RowKey)
{
    var obj = {};

    obj.RowKey = RowKey;

    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteInternalExamSchedule").val(),
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







