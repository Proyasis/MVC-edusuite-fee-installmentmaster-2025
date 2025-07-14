var ajaxRequest = null;
var StudentsPromotion = (function () {

    var getStudentsPromotion = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetStudentsPromotion").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
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
            colNames: [Resources.RowKey, //Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,
                 Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Branch,
                Resources.Course, Resources.AffiliationsTieUps, Resources.Batch , Resources.ClassCode,
                Resources.Promoted, Resources.Completed, Resources.Discontinued, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                //{ key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                //{ key: true, hidden: true, name: 'UniversityMasterKey', index: 'UniversityMasterKey', editable: true },
                //{ key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                //{ key: true, hidden: true, name: 'CourseYear', index: 'CourseYear', editable: true },
                { key: false, hidden: true, name: 'BatchKey', index: 'BatchKey', editable: true },
                { key: false, hidden: true, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'UniversityName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ClassDetailsName', index: 'ClassDetailsName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PromotedCount', index: 'PromotedCount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CompletedCount', index: 'CompletedCount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DiscontinuedCount', index: 'DiscontinuedCount', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        $("#grid").jqGrid("setLabel", "StudentsPromotion", "", "thStudentsPromotion");
    }


    var getRollNumberList = function (jsonData) {
        $("#gridRollNumber").jqGrid('setGridParam', { datatype: 'local' }).trigger('reloadGrid');
        $("#gridRollNumber").jqGrid({
            data: jsonData,
            datatype: 'local',
            mtype: 'Get',

            colNames: [Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.AdmissionNo, Resources.Name, Resources.Year, Resources.RollNumber],
            colModel: [
                { key: false, hidden: true, name: 'ApplicationKey', index: 'ApplicationKey', editable: true },
                { key: false, hidden: true, name: 'StudentYear', index: 'StudentYear', editable: true },
                { key: true, hidden: true, name: 'StudentDivisionAllocationKey', index: 'StudentDivisionAllocationKey', editable: true },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Name', index: 'Name', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentYearText', index: 'StudentYearText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RollNumber', index: 'RollNumber', editable: true, cellEdit: true, sortable: true, resizable: false },

            ],
            pgbuttons: false,
            viewrecords: false,
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
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            sortable: true,
            altclass: 'jqgrid-altrow'

        });
        $("#gridRollNumber").sortableRows();
        $("#gridRollNumber").jqGrid("gridDnD");
        $("#gridRollNumber").jqGrid("setLabel", "rn", "Roll Number")
    }


    var editRollNoPopUp = function (ApplicationKeys) {
        var obj = {};
        obj.ApplicationKeys = ApplicationKeys;
        obj.AcademicTermKey = $("#AcademicTermKey").val() != "" ? $("#AcademicTermKey").val() : 0;
        obj.Branchkey = $("#BranchKey").val() != "" ? $("#BranchKey").val() : 0;
        obj.CourseKey = $("#CourseKey").val() != "" ? $("#CourseKey").val() : 0;
        obj.UniversityMasterKey = $("#UniversityMasterKey").val() != "" ? $("#UniversityMasterKey").val() : 0;
        obj.CourseYear = $("#CourseYear").val() != "" ? $("#CourseYear").val() : 0;
        obj.BatchKey = $("#BatchKey").val() != "" ? $("#BatchKey").val() : 0;
        obj.ClassDetailsKey = $("#DivisionKey").val() != "" ? $("#DivisionKey").val() : 0;

        var URL = $("#hdnGenerateRollnumber").val() + '?' + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {

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




                //Enquiry.GetEnquiries(jsonData);


            },

        }, URL);

    }


    function editLink(cellValue, options, rowdata, action) {
        //var temp = "'" + rowdata.CourseKey + "," + rowdata.UniversityMasterKey + "," + rowdata.BatchKey + "," + rowdata.CourseYear + "," + rowdata.DivisionKey + "," + rowdata.BranchKey + "'";
        //var obj = {};
        //var objDelete = [rowdata.CourseKey, rowdata.UniversityMasterKey, rowdata.AcademicTermKey, rowdata.BatchKey, rowdata.CourseYear, rowdata.ClassDetailsKey, rowdata.BranchKey];
        //obj.CourseKey = rowdata.CourseKey;
        //obj.UniversityMasterKey = rowdata.UniversityMasterKey;
        //obj.AcademicTermKey = rowdata.AcademicTermKey;
        //obj.BatchKey = rowdata.BatchKey;
        //obj.CourseYear = rowdata.CourseYear;
        //obj.ClassDetailsKey = rowdata.ClassDetailsKey;
        //obj.BranchKey = rowdata.BranchKey;
        //obj.IfEdit = true;


        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnAddEditStudentsPromotion").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteStudentsPromotion(' + rowdata.RowKey + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    function formSubmit(data) {
        var $form = $("#frmStudentsPromotion")
        var JsonData = [];
        var formData = $form.serializeArray();
        objectifyForm(formData, data);

        var checkdata = data["StudentsPromotionDetails"].filter(function (i, n) {
            return i.PromotionStatusKey != "";

        });



        if ($form.valid()) {

            if (checkdata.length > 0) {

                $form.mLoading();
                var dataurl = $form.attr("action");
                var response = [];

                response = AjaxHelper.ajax("POST", dataurl,
                           {
                               model: data
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
                                    $form.mLoading("destroy");
                                    window.location.href = $("#hdnStudentPromotionList").val();
                                }
                            }
                        }
                    })

                }
            }
            else {
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


    var loadData = function (json) {
        var model = json;

        //model.AcademicTermKey = $("#AcademicTermKey").val();
       // model.CourseKey = $("#CourseKey").val();
        //model.UniversityMasterKey = $("#UniversityMasterKey").val();
        model.BatchKey = $("#BatchKey").val();
        //model.CourseYear = $("#CourseYear").val();
        //model.DivisionKey = $("#DivisionKey").val();
        model.BranchKey = $("#BranchKey").val();
        model.ClassDetailsKey = $("#ClassDetailsKey").val();
        model.RowKey = $("#RowKey").val();
        $("#dvStudentsList").mLoading();

        ajaxRequest = $.ajax({
            type: "POST",
            url: $("#hdnUrl").val(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            beforeSend: function () {
                if (ajaxRequest != null) {
                    ajaxRequest.abort();
                }
            },
            success: function (result) {
                if (result.IsSuccessful == false) {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("#dvStudentsList").html("")
                $("#dvStudentsList").html(result);
                $("#dvStudentsList").mLoading("destroy");
            },
            error: function (request, status, error) {
                $("#dvStudentsList").mLoading("destroy");
            }
        });
    }

    var resetPromotion = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnResetPromotion").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                StudentsPromotion.LoadData(jsonData);
            }
        });
    }

    return {
        FormSubmit: formSubmit,
        GetStudentsPromotion: getStudentsPromotion,
        GetRollNumberList: getRollNumberList,
        EditRollNoPopUp: editRollNoPopUp,
        LoadData: loadData,
        ResetPromotion: resetPromotion
    }



}());

function deleteStudentsPromotion(rowkey) { 

    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteStudentsPromotion").val(),
        actionValue: rowkey,
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







