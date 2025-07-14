var DivisionAllocation = (function () {

    var getDivisionAllocation = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetDivisionAllocation").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                searchText: function () {
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
            Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Branch, Resources.ClassCode,
            Resources.Course, Resources.Year, Resources.Batch,// Resources.Year,
            Resources.NoOfStudents, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                { key: true, hidden: true, name: 'UniversityMasterKey', index: 'UniversityMasterKey', editable: true },
                { key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: true, hidden: true, name: 'BatchKey', index: 'BatchKey', editable: true },
                { key: true, hidden: true, name: 'CourseYear', index: 'CourseYear', editable: true },
                { key: true, hidden: true, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
                { key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ClassDetailsName', index: 'ClassDetailsName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, formatter: formatCourseUniversityYear, sortable: true, resizable: false, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'CourseYearName', index: 'CourseYearName', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        $("#grid").jqGrid("setLabel", "ClassDetailsName", "", "thClassDetailsName");
    }

    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CourseYear, rowdata.AcademicTermKey)
    }
    function formatCourseUniversityYear(cellValue, options, rowdata, action) {

        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName
        return Coursetext;
    }
    var getRollNumberList = function (jsonData) {
        $("#gridRollNumber").jqGrid('setGridParam', { datatype: 'local' }).trigger('reloadGrid');
        $("#gridRollNumber").jqGrid({
            data: jsonData,
            datatype: 'local',
            mtype: 'Get',

            colNames: [Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.AdmissionNo, Resources.Name, Resources.RollNumber, Resources.RollNumber],
            colModel: [
                { key: false, hidden: true, name: 'ApplicationKey', index: 'ApplicationKey', editable: true },
                { key: false, hidden: true, name: 'StudentYear', index: 'StudentYear', editable: true },
                { key: true, hidden: true, name: 'StudentDivisionAllocationKey', index: 'StudentDivisionAllocationKey', editable: true },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Name', index: 'Name', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'StudentYearText', index: 'StudentYearText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RollNumber', index: 'RollNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RollNoCode', index: 'RollNoCode', editable: true, cellEdit: true, sortable: true, resizable: false },

            ],
            pgbuttons: false,
            viewrecords: false,
            rownumbers: true,
            rownumWidth: 100,
            pgtext: "",
            pginput: false,
            scroll: true,
            hidegrid: false,
            height: 300,
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            multiselect: false,
            loadonce: true,
            ignoreCase: true,
            //sortname: 'RollNumber',
            //sortorder: 'asc',
            altRows: true,
            sortable: true,
            altclass: 'jqgrid-altrow'

        });
        $("#gridRollNumber").sortableRows();
        $("#gridRollNumber").jqGrid("gridDnD");
        $("#gridRollNumber").jqGrid("setLabel", "rn", "Sl No")
    }

    var editRollNoPopUp = function () {

        var $form = $("#frmDivisionAllocation")
        var JsonData = [];
        //var formData = $form.serializeArray();
        //objectifyForm(formData, data);
        var fromData = $form.serializeToJSON({ associativeArrays: false });
        var checkdata = $("[name*=IsActive]:checked")
        if ($form.valid()) {

            if (checkdata.length > 0) {
                var URL = $("#hdnGenerateRollnumber").val();
                $.customPopupform.CustomPopup({
                    modalsize: "modal-lg",
                    ajaxType: "POST",
                    ajaxData: { model: fromData },
                    load: function () {
                        setTimeout(function () {
                            AppCommon.FormatInputCase()
                            var $form = $("#frmDivisionAllocation")
                            var formData = $form.serializeToJSON({
                                associativeArrays: false
                            });
                            //DivisionAllocation.GetRollNumberList(formData["DivisionAllocationDetails"]);
                            DivisionAllocation.GetRollNumberList(jsonData["DivisionAllocationDetails"]);
                            jsonData["DivisionAllocationDetails"] = jsonData["DivisionAllocationDetails"].map(function (item) {
                                item.IsActive = item.Rollnumber;
                                return item;
                            })
                            $("#btnSave", $("#frmGenerateRollnumber")).on("click", function () {
                                $('#btnSave', $("#frmGenerateRollnumber")).hide();
                                DivisionAllocation.FormSubmit(jsonData);
                                
                            })
                        }, 500)

                    },
                    rebind: function (result) {
                        if (result.IsSuccessful) {
                            toastr.success(Resources.Success, result.Message);
                        }
                        else {
                            toastr.error(Resources.Failed, result.Message);
                        }
                        if (result.Feedback != "") {
                            if (result.IsUserBlocked == true) {
                                $.confirm({
                                    title: Resources.Warning,
                                    content: result.Feedback,
                                    type: 'red',
                                    buttons: {
                                        Ok: {
                                            text: 'Ok',
                                            btnClass: 'btn-danger',
                                            action: function () {
                                                window.location.href = $("#hdnLogin").val();
                                            }
                                        }
                                    }
                                });
                            }

                            toastr.warning(Resources.Warning, result.Feedback);
                        }

                        DivisionAllocation.GetRollNumberList(jsonData);
                    },

                }, URL);
            }
            else {
                EduSuite.AlertMessage({ title: Resources.Warning, content: "Please select atleast one Record !.", type: 'orange' })
                return false;
            }

            

        }

    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.CourseKey + "," + rowdata.UniversityMasterKey + "," + rowdata.BatchKey + "," + rowdata.CourseYear + "," + rowdata.ClassDetailsKey + "," + rowdata.BranchKey + "'";
        var obj = {};
        var objDelete = [rowdata.CourseKey, rowdata.UniversityMasterKey, rowdata.AcademicTermKey, rowdata.BatchKey, rowdata.CourseYear, rowdata.ClassDetailsKey, rowdata.BranchKey];
        obj.CourseKey = rowdata.CourseKey;
        obj.UniversityMasterKey = rowdata.UniversityMasterKey;
        obj.AcademicTermKey = rowdata.AcademicTermKey;
        obj.BatchKey = rowdata.BatchKey;
        obj.CourseYear = rowdata.CourseYear;
        obj.ClassDetailsKey = rowdata.ClassDetailsKey;
        obj.BranchKey = rowdata.BranchKey;
        obj.IfEdit = true;



        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnAddEditDivisionAllocation").val() + '?' + $.param(obj) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteDivisionAllocation(' + objDelete.join(",") + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    function formSubmit(data) {
        var $form = $("#frmGenerateRollnumber")

        if ($form.valid()) {
            
            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    modelList: data["DivisionAllocationDetails"], ClassDetailsKey: data.ClassDetailsKey, BatchKey: data.BatchKey,
                });
            if (typeof response == "string") {
                $("[data-valmsg-for=error_msg]").html(response);
                $('#btnSave', $("#frmGenerateRollnumber")).show();
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
                                window.location.href = $("#hdnDivisionAllocationList").val();
                                $('#btnSave', $("#frmGenerateRollnumber")).show();
                            }
                        }
                    }
                })

            }

        }
    }

    var loadData = function (json) {
        var model = json;

        model.AcademicTermKey = $("#AcademicTermKey").val();
        model.CourseKey = $("#CourseKey").val();
        model.UniversityMasterKey = $("#UniversityMasterKey").val();
        model.BatchKey = $("#BatchKey").val();
        model.CourseYear = $("#CourseYear").val();
        model.DivisionKey = $("#DivisionKey").val();
        model.BranchKey = $("#BranchKey").val();
        model.ClassDetailsKey = $("#ClassDetailsKey").val();
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

    var resetDivisionAllocation = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnResetDivisionAllocation").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                DivisionAllocation.LoadData(jsonData);
            }
        });
    }

    function setRollNumber(_this, Ischeck) {

        var currRollNumber = 0;
        var GenderPriority = $("#GenderPriority").val();
        var GenderPriorityCount = $("#GenderPriorityCount").val();
        GenderPriorityCount = parseInt(GenderPriorityCount) ? parseInt(GenderPriorityCount) : 0;
        var item = $(_this).closest("[data-repeater-item]");
        var Gender = $("input[name*=GenderKey]", item).val();
        Gender = parseInt(Gender) ? parseInt(Gender) : 0;
        var rollnumberlist = [];
        rollnumberlist = $("#DivstudentDetails input[type=checkbox][name*=IsActive]").toArray().filter(function (chk) {
            var Rollnumber = $("input[name*=RollNumber]", $(chk).closest("[data-repeater-item]")).val();
            Rollnumber = parseInt(Rollnumber) ? parseInt(Rollnumber) : 0;

            return $(item).index() != $(chk).closest("[data-repeater-item]").index() && Rollnumber;
        }).map(function (chk) {
            var Rollnumber = $("input[name*=RollNumber]", $(chk).closest("[data-repeater-item]")).val();
            Rollnumber = parseInt(Rollnumber) ? parseInt(Rollnumber) : 0;
            return Rollnumber;
        });

        var rollnumberGenderlist = $("#DivstudentDetails input[type=checkbox][name*=IsActive]").toArray().filter(function (chk) {
            var Rollnumber = $("input[name*=RollNumber]", $(chk).closest("[data-repeater-item]")).val();
            Rollnumber = parseInt(Rollnumber) ? parseInt(Rollnumber) : 0;

            var GenderKey = $("input[name*=GenderKey]", $(chk).closest("[data-repeater-item]")).val();
            GenderKey = parseInt(GenderKey) ? parseInt(GenderKey) : 0;

            return $(item).index() != $(chk).closest("[data-repeater-item]").index() && Rollnumber && GenderKey == Gender;
        }).map(function (chk) {
            var Rollnumber = $("input[name*=RollNumber]", $(chk).closest("[data-repeater-item]")).val();
            Rollnumber = parseInt(Rollnumber) ? parseInt(Rollnumber) : 0;
            return Rollnumber;
        });



        var missingValues = [];
        var max_of_array = rollnumberlist.length ? Math.max.apply(Math, rollnumberlist) : 0;
        var max_of_gender_array = rollnumberGenderlist.length ? Math.max.apply(Math, rollnumberGenderlist) : 0;
        max_of_array = Gender == GenderPriority && max_of_gender_array < GenderPriorityCount ? max_of_gender_array : (GenderPriority != Gender && !max_of_gender_array ? GenderPriorityCount + max_of_array : max_of_array);
        if (Ischeck) {



            var checkStatus = $("input[type=checkbox][name*=IsActive]", item).is(":checked");
            currRollNumber = max_of_array + 1;
            if (checkStatus == true) {

                $("input[name*=RollNumber]", item).val(currRollNumber);
            }
            else {
                currRollNumber = $("input[name*=RollNumber]", item).val();
                currRollNumber = parseInt(currRollNumber) ? parseInt(currRollNumber) : 0;
                $("input[name*=RollNumber]", item).val("");
                $("#DivstudentDetails input[type=checkbox][name*=IsActive]:checked:not([disabled])").filter(function (i, chk) {
                    var Rollnumber = $("input[name*=RollNumber]", $(chk).closest("[data-repeater-item]")).val();
                    Rollnumber = parseInt(Rollnumber) ? parseInt(Rollnumber) : 0;
                    return $(item).index() != $(chk).closest("[data-repeater-item]").index() && Rollnumber;
                }).each(function (i, chk) {
                    var Rollnumber = $("input[name*=RollNumber]", $(chk).closest("[data-repeater-item]")).val();
                    Rollnumber = parseInt(Rollnumber) ? parseInt(Rollnumber) : 0;
                    if (currRollNumber < Rollnumber) {
                        Rollnumber--;
                        if (rollnumberlist.indexOf(currRollNumber) == -1)
                            $("input[name*=RollNumber]", $(chk).closest("[data-repeater-item]")).val(Rollnumber);
                    }
                });
            }
        }
        else if ($("input[name*=RollNumber]", item).val()) {
            $("input[name*=IsActive]", item).prop("checked", true);
            currRollNumber = $("input[name*=RollNumber]", item).val();
            currRollNumber = parseInt(currRollNumber) ? parseInt(currRollNumber) : 0;

            for (var i = (Gender == GenderPriority ? 1 : (GenderPriorityCount + 1)); i < max_of_gender_array; i++) {
                if (rollnumberGenderlist.indexOf(i) == -1) {
                    missingValues.push(i);
                }
            }
            if (!(Gender != GenderPriority && max_of_array && currRollNumber == max_of_array + 1)) {


                if (missingValues.length > 0 && missingValues.indexOf(currRollNumber) == -1) {
                    if (rollnumberlist.indexOf(missingValues[0]) > -1) {
                        $("input[name*=RollNumber]", item).val(max_of_array + 1);
                    } else {
                        $("input[name*=RollNumber]", item).val(missingValues[0]);
                    }
                } else if (GenderPriority != Gender && !max_of_gender_array) {
                    $("input[name*=RollNumber]", item).val(GenderPriorityCount + 1);
                } else if (GenderPriority == Gender && currRollNumber > GenderPriorityCount) {
                    if (rollnumberlist.indexOf(currRollNumber) > -1) {
                        $("input[name*=RollNumber]", item).val(max_of_array + 1);
                    }
                }
                else if (!missingValues.length || (missingValues.length && missingValues.indexOf(currRollNumber) == -1)) {
                    var max = Gender == GenderPriority && max_of_gender_array < GenderPriorityCount ? max_of_gender_array : (GenderPriority != Gender && !max_of_gender_array ? GenderPriorityCount + max_of_array : max_of_array);
                    $("input[name*=RollNumber]", item).val(max + 1);
                }
                if (rollnumberlist.indexOf(currRollNumber) > -1) {
                    var max = Gender == GenderPriority && max_of_gender_array < GenderPriorityCount ? max_of_gender_array : (GenderPriority != Gender && !max_of_gender_array ? GenderPriorityCount + max_of_array : max_of_array);
                    $("input[name*=RollNumber]", item).val(max + 1);
                }
            }

        }
        else {
            $("input[name*=IsActive]", item).prop("checked", false);
        }

    }

    function reloaddata() {

        $("[data-repeater-item]").each(function (Index) {

            var CheckedList = $("input[type=checkbox][name*=IsActive]:checked");

            var roll = $("input[name*=RollNumber]", $(this)).val();
            var chk = $("input[type=checkbox][name*=IsActive]", $(this)).is(":checked");
            if (chk == true) {
                $("input[type=checkbox][name*=IsActive]", $(this)).prop("disabled", true);
            }

            $(CheckedList).each(function () {
                $(this).prop("disabled", true);
            });

            var RowKey = $("input[name*=StudentDivisionAllocationKey]", $(this)).val();
            RowKey = parseInt(RowKey) ? parseInt(RowKey) : 0;
            if (RowKey != 0) {
                $("input[name*=RollNumber]", $(this)).prop("readonly", true);
            }
        });
    }

    return {
        FormSubmit: formSubmit,
        GetDivisionAllocation: getDivisionAllocation,
        GetRollNumberList: getRollNumberList,
        EditRollNoPopUp: editRollNoPopUp,
        ResetDivisionAllocation: resetDivisionAllocation,
        LoadData: loadData,
        SetRollNumber: setRollNumber,
        Reloaddata: reloaddata
    }



}());

function deleteDivisionAllocation(CourseKey, UniversityMasterKey, AcademicTermKey, BatchKey, CourseYear, ClassDetailsKey, BranchKey) {
    var obj = {};

    obj.CourseKey = CourseKey,
        obj.UniversityMasterKey = UniversityMasterKey,
        obj.AcademicTermKey = AcademicTermKey,
        obj.BatchKey = BatchKey,
        obj.CourseYear = CourseYear;
    obj.ClassDetailsKey = ClassDetailsKey;
    obj.BranchKey = BranchKey;

    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteDivisionAllocation").val(),
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







