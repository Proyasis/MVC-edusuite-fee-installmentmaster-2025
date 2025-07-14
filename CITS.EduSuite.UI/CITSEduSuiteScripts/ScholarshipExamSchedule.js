
var JsonData = [];

var ScholarshipExamSchedule = (function () {

    var getScholarshipExamSchedule = function () {


        var scheduleStatus = $("#tab-componets li a.active").data('val');
        scheduleStatus = scheduleStatus ? JSON.parse(scheduleStatus) : false;
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetScholarship").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: {

                SearchName: function () {
                    return $('#SearchName').val()
                },
                SearchPhone: function () {
                    return $('#SearchPhone').val()
                },
                SearchFromDate: function () {
                    return $('#SearchFromDate').val()
                },
                SearchToDate: function () {
                    return $('#SearchToDate').val()
                },
                SearchDistrictKey: function () {
                    return $('#SearchDistrictKey').val()
                },
                SearchBranchKey: function () {
                    return $('#SearchBranchKey').val()
                },
                SearchScholarshipTypeKey: function () {
                    return $('#SearchScholarshipTypeKey').val()
                },
                SearchSubBranchKey: function () {

                    return $('#SubBranchKey').val()
                },
                ScheduleStatus: function () {

                    return $("#tab-componets li a.active").data('val')
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.Name, Resources.MobileNo, Resources.center, Resources.City, Resources.District, Resources.Scholarship + Resources.BlankSpace + Resources.Type, Resources.Email, Resources.RegNo, Resources.ExamDate, Resources.ExamCenter, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'ScholarshipExamScheduleKey', index: 'ScholarshipExamScheduleKey', editable: true },
                { key: false, name: 'ScholarShipName', index: 'ScholarShipName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'DistrictName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LocationName', index: 'LocationName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DistrictName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ScholarshipTypeName', index: 'ScholarshipTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmailAddress', index: 'EmailAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ExamRegNo', index: 'ExamRegNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ExamDate', index: 'ExamDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'ExamCentername', index: 'ExamCentername', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 20, 50, 100, 250, 500, 1000],
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
            altRows: true,
            altclass: 'jqgrid-altrow',
            //loadComplete: function (data) {

            //    for (i = 0, count = data.rows.length; i < count; i += 1) {
            //        if (data.rows[i].ScholarshipExamScheduleKey != 0) {
            //            //$("#grid").jqGrid('setSelection', data.rows[i].RowKey, false);
            //            $("input#jqg_grid_" + data.rows[i].RowKey).remove()
            //        }
            //    }
            ////},
            //beforeSelectRow: function (rowid, e) {
            //    
            //    //var rowData = $("#grid").jqGrid('getRowData', rowid);
            //    //var selectIds = $("#grid").jqGrid("getGridParam", "selarrrow")
            //    //if (selectIds[0])
            //    //    var rowFirstData = $("#grid").jqGrid('getRowData', selectIds[0]);
            //    //var Isvalid = !rowFirstData || rowData.EnquiryKey == 0 ? true : false;
            //    //if ($("#jqg_grid_" + rowid).attr("disabled") || !Isvalid) {

            //    //    e.stopImmediatePropagation()
            //    //    $("#jqg_grid_" + rowid)[0].checked = false;
            //    //    return false;
            //    //}
            //    //return true;

            //    var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", grid);
            //    if (cbsdis.length === 0) {
            //        return true;    // allow select the row
            //    } else {
            //        return false;   // not allow select the row
            //    }

            //},
            //onSelectAll: function (aRowids, status) {
            //    
            //    if (status) {
            //        // uncheck "protected" rows
            //        var cbs = $("tr.jqgrow > td > input.cbox:disabled", grid[0]);
            //        cbs.removeAttr("checked");
            //    }
            //    else {
            //        var cbs = $("tr.jqgrow > td > input.cbox:disabled", grid[0]);
            //        cbs.attr("checked");

            //    }
            //},
            //onSelectRow: function (id) {
            //    if ($("input[id*=jqg_grid_]:checked").length == 0) {
            //        //SalesOrderMasterKey = 0;
            //    }
            //}
        })
        if (scheduleStatus) {
            $('#grid').jqGrid('hideCol', ['cb']);
            $('#grid').jqGrid('showCol', ['ExamRegNo', 'ExamDate', 'ExamCentername', 'edit']);

        } else {
            $('#grid').jqGrid('showCol', ['cb']);
            $('#grid').jqGrid('hideCol', ['ExamRegNo', 'ExamDate', 'ExamCentername', 'edit']);
            var $grid = $("#grid"),
            newWidth = $grid.closest(".ui-jqgrid").parent().width();
            $grid.jqGrid("setGridWidth", newWidth, true);
        }
        $("#grid").jqGrid("setLabel", "ScholarshipName", "", "thScholarshipName");
    }

    function formSubmit(data) {
        
        var $form = $("#frmScholarshipExamSchedule")
        var JsonData = [];
        var formData = $form.serializeArray();
        var TimeKeys = ["ExamStartTime", "ExamEndTime"]
        var checkdata = $("[name*=IsActive]:checked")
        if ($form.valid()) {

            if (checkdata.length > 0) {
                //var obj = $form.serializeArray();

                $(formData).each(function (i) {
                    if (formData[i]) {
                        var name = formData[i]['name'];
                        var keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;

                        if (TimeKeys.indexOf(keyName) > -1) {
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
                                            window.location.href = $("#hdnScholarshipExamScheduleList").val();
                                        }
                                    }
                                }
                            })

                        }
                    },
                    error: function (xhr) {

                    }
                });
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

        model.Branchkey = $("#Branchkey").val();
        model.DistrictKey = $("#DistrictKey").val();
        model.ScholarshipTypeKey = $("#ScholarshipTypeKey").val();
        //model.RowKey = $("#RowKey").val();
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



    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.ScholarshipExamScheduleKey + "'";

        if (rowdata.ScholarshipExamScheduleKey != 0) {
            //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" target="_blank" href="' + $("#hdnGetHallticket").val() + '/' + rowdata.RowKey + '"><i class="fa fa-print" aria-hidden="true"></i></a>';

            return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" target="_blank" href="' + $("#hdnGetHallticket").val() + '/' + rowdata.RowKey + '"><i class="fa fa-print" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteScholarship(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
        }
        else {
            return "";

        }
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditScholarship").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteScholarship(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>' + '<a data-modal="" class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnGetHallticket").val() + '/' + rowdata.RowKey + '"><i class="fa fa-print" aria-hidden="true"></i></a>';
    }


    var printExamSchedule = function () {
        var postData = {};
        var valid = true;
        postData.SearchName = $('#SearchName').val();
        postData.SearchPhone = $('#SearchPhone').val()
        postData.SearchFromDate = $('#SearchFromDate').val()
        postData.SearchToDate = $('#SearchToDate').val()
        postData.SearchDistrictKey = $('#SearchDistrictKey').val()
        postData.SearchBranchKey = $('#SearchBranchKey').val()
        postData.SearchScholarshipTypeKey = $('#SearchScholarshipTypeKey').val()
        postData.SearchSubBranchKey = $('#SubBranchKey').val()
        postData.ScheduleStatus = $("#tab-componets li a.active").data('val')
        postData.sidx = "RowKey"; postData.sord = "asc"; postData.page = 0; postData.rows = 0;
        var validator = $("form").validate();
        valid = valid && validator.element('#SearchBranchKey');
        valid = valid && validator.element('#SearchScholarshipTypeKey');
        if (valid) {
            $.ajax({
                type: "POST",
                url: $("#hdnGetScholarship").val(),
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(postData),
                success: function (response) {
                    var obj = {};
                    obj.Title = $('#SearchScholarshipTypeKey').find("option:selected").text();
                    obj.SubTitle = $('#SearchBranchKey').find("option:selected").text();
                    obj.JSONData = response.rows;
                    var columnModel = [
                    { name: 'ScholarShipName', index: 'ScholarShipName', headertext: Resources.Name },
                    { name: 'MobileNumber', index: 'MobileNumber', headertext: Resources.MobileNo },
                    { name: 'ExamRegNo', index: 'ExamRegNo', headertext: Resources.RegNo },
                    { name: 'Signature', index: 'Signature', headertext: "Signature" }
                    ];
                    obj.JSONData = $(obj.JSONData).map(function (n, item) {
                        item.Signature = null;
                        return item
                    })
                    obj.ColumnNames = columnModel;
                    AppCommon.PrintJSON(obj);
                    //AppCommon.ExportJSONToExcel(obj);

                    
                },
                error: function (request, status, error) {

                }
            });
        }
    }

    return {
        GetScholarshipExamSchedule: getScholarshipExamSchedule,
        LoadData: loadData,
        FormSubmit: formSubmit,
        PrintExamSchedule: printExamSchedule


    }
}());

function deleteScholarship(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Scholarship,
        actionUrl: $("#hdnDeleteScholarshipExam").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}

function ScholarshipPopupLoad() {
    $(".selectpicker").selectpicker()
    $("#DistrictKey").on("change", function () {

        var obj = {};
        obj.id = $(this).val() != "" ? $(this).val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetBranchByDistrict").val(), $("#BranchKey"), Resources.center, "Branches");

    });



}


