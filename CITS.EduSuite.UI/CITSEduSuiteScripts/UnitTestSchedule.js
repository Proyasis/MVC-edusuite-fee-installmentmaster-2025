var UnitTestSchedule = (function () {
    var getUnitTestSchedule = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetUnitTestSchedule").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                SearchDate: function () {
                    return $('#SearchDate').val()                    
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                ClassDetailsKey: function () {
                    return $('#ClassDetailsKey').val()
                },
                BatchKey: function () {
                    return $('#BatchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,
                Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BranchName, Resources.ClassCode, Resources.Batch + Resources.Name,
                Resources.Subject, Resources.SUbjectModule,
                Resources.NoOfTopics, Resources.ExamDate, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
                { key: true, hidden: true, name: 'SubjectKey', index: 'SubjectKey', editable: true },
                { key: true, hidden: true, name: 'SubjectModuleKey', index: 'SubjectModuleKey', editable: true },
                { key: true, hidden: true, name: 'BatchKey', index: 'BatchKey', editable: true },
                { key: true, hidden: true, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
                { key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ClassDetailsName', index: 'ClassDetailsName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SubjectName', index: 'SubjectName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SubjectModuleName', index: 'SubjectModuleName', editable: true, cellEdit: true, sortable: true, resizable: false },           
                { key: false, name: 'ModuleTopicsCount', index: 'ModuleTopicsCount', editable: true, cellEdit: true, sortable: true, resizable: false },
                 { key: false, name: 'ExamDate', index: 'ExamDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [10, 50, 100, 500].unique(),
            autowidth: true,
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
            sortname: 'RowKey',
            sortorder: 'desc',
            altclass: 'jqgrid-altrow',



        })

        $("#grid").jqGrid("setLabel", "UnitTestSchedule", "", "thStudentsPromotion");
    }

    function editLink(cellValue, options, rowdata, action) {
             return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnAddEditUnitTestSchedule").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteUnitTest(' + rowdata.RowKey + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    function formSubmit(data) {
        
        var $form = $("#frmUnitTestSchedule")
        var JsonData = [];
        var formData = $form.serializeArray();
        //var TimeKeys = ["ExamStartTime", "ExamEndTime"]
        //var checkdata = $("[name*=ExamStatus]:checked")
        if ($form.valid()) {
            $("#btnSave").hide();
            //if (checkdata.length > 0) {
                //var obj = $form.serializeArray();

                $(formData).each(function (i) {
                    if (formData[i]) {
                        var name = formData[i]['name'];
                        var keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;

                        //if (TimeKeys.indexOf(keyName) > -1) {
                        //    formData[i]['value'] = (formData[i]['value'] != "" ? moment(formData[i]['value'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
                        //}
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
                    success: function (data) {
                        if (typeof response == "string") {
                            $("[data-valmsg-for=error_msg]").html(response);
                        }
                        else if (data.IsSuccessful) {
                            $.alert({
                                type: 'green',
                                title: Resources.Success,
                                content: data.Message,
                                icon: 'fa fa-check-circle-o-',
                                buttons: {
                                    Ok: {
                                        text: Resources.Ok,
                                        btnClass: 'btn-success',
                                        action: function () {
                                            window.location.href = $("#hdnStudentPromotionList").val();
                                        }
                                    }
                                }
                            })

                        }
                    },
                    error: function (xhr) {

                    }
                });
            //}
            //else {
            //    $.alert({
            //        type: 'orange',
            //        title: Resources.Warning,
            //        content: 'Please Select atleast One !',
            //        icon: 'fa fa-exclamation-circle',
            //        buttons: {
            //            Ok: {
            //                text: Resources.Ok,
            //                btnClass: 'btn-warning',

            //            }
            //        }
            //    })

            //}

        }
    }


    var loadData = function (json) {
        var model = json;

        model.SubjectKey = $("#SubjectKey").val();
        model.SubjectModuleKey = $("#SubjectModuleKey").val();
        model.BatchKey = $("#BatchKey").val();
        //model.CourseYear = $("#CourseYear").val();
        model.ExamDate = $("#ExamDate").val()//.split(",");
        model.BranchKey = $("#BranchKey").val();
        model.ModuleTopicKeys = $("#ModuleTopicKeys").val();
        model.ClassDetailsKey = $("#ClassDetailsKey").val();
        model.RowKey = $("#RowKey").val();
        $.ajax({
            type: "POST",
            url: $("#hdnUrl").val(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            success: function (result) {
                if (result.IsSuccessful == false) {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("#dvStudentsList").html("")
                $("#dvStudentsList").html(result);

            },
            error: function (request, status, error) {

            }
        });
    }
    var resetUnitTestSchedule = function (rowkey, UnitTestKey) {
        var obj = {};
        obj.RowKey = rowkey;
        obj.UnitTestKey = UnitTestKey;

        var result = EduSuite.Confirm2({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnResetUnitTestSchedule").val(),
            parameters: obj,
            //actionValue: rowkey,
            dataRefresh: function () {
                UnitTestSchedule.LoadData(jsonData);
            }
        });

    }

    var getBatchByClass = function (obj, ddl) {
        $(ddl).html("");
        $(ddl).append($('<option></option>').val("").html(Resources.Batch));
        $.ajax({
            url: $("#hdnFillBatch").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                $.each(result.Batches, function (i, Batch) {
                    $(ddl).append(
                        $('<option></option>').val(Batch.RowKey).html(Batch.Text));
                });
            }
        });

    }

    var getclassDetails = function (obj, ddl) {
        $(ddl).html("");
        $.ajax(
            {
                url: $("#hdnFillClassDetails").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                //contentType: "application/json; charset=utf-8",
                success: function (result) {
                    
                    $.each(result.ClassDetails, function (i, ClassDetails) {
                        $(ddl).append(
                            $('<option></option>').val(ClassDetails.RowKey).html(ClassDetails.Text));
                    });


                    $(ddl).selectpicker('refresh');

                }
            });
    }

    function setExamResultStatus() {
        
        $("#DivUnitTestResult [data-repeater-item]").each(function (i) {

            //var item = $(_this).closest("[data-repeater-item]");
            var AbsentStatus = $("input[type=checkbox][id*=AbsentStatus]", $(this)).is(":checked");
            var Mark = $("input[id*=Mark]", $(this))[0];
            var MinimumMark = $("input[id*=MinimumMark]").val();
            var MaximumMark = $("input[id*=MaximumMark]").val();
            var ResultStatus = $("input[id*=ResultStatus]", $(this))[0];

            if (AbsentStatus == true) {
                $("#spnResultStatus", $(this)).html("Absent").attr("style", "color:orange;");

                ResultStatus.value = "A";
                Mark.value = "";
                Mark.disabled = true;
            }
            else {
                Mark.disabled = false;

                if (Mark.value == "") {
                    $("#spnResultStatus", $(this)).html("No Result").attr("style", "color:black;");

                    ResultStatus.value = "N";
                }

                else if (parseFloat(Mark.value) < parseFloat(MinimumMark)) {
                    if (parseFloat(Mark.value) < parseFloat(0)) {
                        Mark.value = 0;
                    }
                    $("#spnResultStatus", $(this)).html("Fail").attr("style", "color:red;");

                    ResultStatus.value = "F";
                }
                else if (parseFloat(Mark.value) >= parseFloat(MinimumMark)) {
                    if (parseFloat(Mark.value) > parseFloat(MaximumMark)) {
                        Mark.value = MaximumMark;
                    }
                    $("#spnResultStatus", $(this)).html("Pass").attr("style", "color:green;");

                    ResultStatus.value = "P";
                }
            }

        });

    }
    return {
        FormSubmit: formSubmit,
        GetUnitTestSchedule: getUnitTestSchedule,
        ResetUnitTestSchedule: resetUnitTestSchedule,
        LoadData: loadData,
        GetBatchByClass: getBatchByClass,
        GetclassDetails: getclassDetails,
        SetExamResultStatus: setExamResultStatus
    }



}());

function deleteUnitTest(RowKey) {
    var obj = {};

    obj.RowKey = RowKey;

    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteUnitTest").val(),
        parameters: obj,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }

    });
}

function objectifyForm(formArray, returnArray) {//serialize data function

    for (var i = 1; i < formArray.length; i++) {

        var subModelName = '', index = 0, keyName = ''
        var name = formArray[i]['name'];
        var tempName = name.match(/^[^\]}),]*/) ? name.match(/^[^\]}),]*/)[0] : name;
        keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;
        var arr = tempName.split('[');
        if (arr.length == 2) {
            subModelName = arr[0];
            index = arr[1];
        }
        else {
            keyName = arr[0];
        }
        if (subModelName == "") {
            returnArray[name] = formArray[i]['value'];
        }
        else {

            if (!returnArray[subModelName]) {
                returnArray[subModelName] = [];
            }
            if (!returnArray[subModelName][index]) {
                returnArray[subModelName][index] = $.extend(true, {}, returnArray[subModelName][0]) || {};
            }
            if ($("input[name='" + name + "']")[0] && $($("input[name='" + name + "']")[0]).is(':checkbox')) {
                returnArray[subModelName][index][keyName] = $($("input[name='" + name + "']")[0]).prop('checked');
            }
            else {
                returnArray[subModelName][index][keyName] = formArray[i]['value'];
            }
        }

    }

}


function UnitTestResultLoad() {

    UnitTestSchedule.SetExamResultStatus();
    $("input[id*=AbsentStatus]", $("#DivUnitTestResult")).change(function () {
        
        UnitTestSchedule.SetExamResultStatus();
    });

    $("input[id*=Mark]", $("#DivUnitTestResult")).keydown(function (e) {
        
        if (e.keyCode == '40') {
            var tr = $(this).closest(tr).next();
            $("input[id*=Mark]", $(tr)).focus();
        }
        if (e.keyCode == '38') {
            var tr = $(this).closest(tr).prev();
            $("input[id*=Mark]", $(tr)).focus();
        }

    });

    $("input[id*=Mark]", $("#DivUnitTestResult")).on('input', function (e) {
        
        UnitTestSchedule.SetExamResultStatus();
    })
}






