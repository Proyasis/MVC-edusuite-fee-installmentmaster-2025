var ajaxRequest = null;
var Attendance = (function () {
    var getAttendance = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAttendance").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }, SearchFromDate: function () {
                    var date = $('#SearchFromDate').val()
                    return date;
                }, searchDate: function () {
                    var date = $('#AttendanceDate').val()
                    return date;
                }, ClassDetailsKey: function () {
                    return $('#ClassDetailsKey').val()
                }, BatchKey: function () {
                    return $('#BatchKey').val()
                }, AttendanceStatusKey: function () {
                    return $('#AttendanceStatusKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace,
            Resources.BlankSpace, Resources.BlankSpace, Resources.SlNo, Resources.Name, Resources.ClassCode,
            Resources.Course + Resources.Name,
            Resources.Batch, Resources.AttendanceStatus, Resources.AttendanceDate, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'AttendanceDate', index: 'AttendanceDate', editable: true, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },

                { key: false, hidden: true, name: 'AttendanceStatusKey', index: 'AttendanceStatusKey', editable: true },
                { key: false, hidden: true, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, name: 'SlNo', index: 'SlNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentName', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ClassDetailsName', index: 'ClassDetailsName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AttendanceStatusName', index: 'AttendanceStatusName', editable: true, cellEdit: true, formatter: formatStatus, sortable: true, resizable: false },
                { key: false, name: 'AttendanceDate', index: 'AttendanceDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [10, 50, 100, 250, 500],
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
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            }
        })

        $("#grid").jqGrid("setLabel", "AttendanceName", "", "thAttendanceName");
    }

    function formatStatus(cellValue, option, rowdata, action) {

        //if (rowdata.AttendanceStatusKey == 1) {
        //    return '<span  ><i class="fa fa-check-square-o w3-text-green" aria-hidden="true"></i> ' + cellValue + '</span>';
        //}
        //else {
        //    return '<span  ><i class="fa fa-times-circle-o w3-text-red" aria-hidden="true"></i> ' + cellValue + '</span>';
        //}

        if (rowdata.AttendanceStatusKey == 1) {
            return '<span class="w3-text-green"><i class="fa fa-check-square-o w3-text-green" aria-hidden="true"></i> ' + 'Present' + '</span>';
        }
        else {
            return '<span class="w3-text-red"><i class="fa fa-times-circle-o w3-text-red" aria-hidden="true"></i> ' + 'Absent' + '</span>';
        }
        return cellValue;
    }

    var getAttendanceQuick = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAttendanceQuick").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }, searchDate: function () {
                    var date = $('#txtsearchDate').val()
                    return date;
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,
            Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BranchName, Resources.Division + Resources.Name,
            Resources.Course + Resources.Name, Resources.Affiliations + Resources.Sla + Resources.TieUps, Resources.Batch + Resources.Name, Resources.Year + Resources.Name,
            Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                { key: true, hidden: true, name: 'UniversityMasterKey', index: 'UniversityMasterKey', editable: true },
                { key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: true, hidden: true, name: 'BatchKey', index: 'BatchKey', editable: true },
                { key: true, hidden: true, name: 'AttendanceYear', index: 'AttendanceYear', editable: true },
                { key: true, hidden: true, name: 'DivisionKey', index: 'DivisionKey', editable: true },
                { key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },

                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DivisionName', index: 'DivisionName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'UniversityName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'YearName', index: 'YearName', editable: true, cellEdit: true, sortable: true, resizable: false },
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

        $("#grid").jqGrid("setLabel", "AttendanceName", "", "thAttendanceName");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + AppCommon.JsonDateToServerDate(rowdata.AttendanceDate) + "'," + rowdata.CourseKey + "," + rowdata.UniversityMasterKey + "," + rowdata.BatchKey + "," + rowdata.AttendanceYear + "," + rowdata.ClassDetailsKey + "," + rowdata.BranchKey + "," + rowdata.AttendanceTypeKey + "," + rowdata.AttendanceTypeDescreptionKey;
        var obj = {};
        var objDelete = ["'" + AppCommon.JsonDateToServerDate(rowdata.AttendanceDate) + "'", rowdata.CourseKey, rowdata.UniversityMasterKey, rowdata.BatchKey, rowdata.AttendanceYear, rowdata.ClassDetailsKey, rowdata.BranchKey, rowdata.AttendanceTypeKey, rowdata.AttendanceTypeDescreptionKey];

        obj.IsUpdate = true;
        //obj.CourseKey = rowdata.CourseKey;
        //obj.UniversityMasterKey = rowdata.UniversityMasterKey;
        //obj.AcademicTermKey = rowdata.AcademicTermKey;
        obj.BatchKey = rowdata.BatchKey;
        obj.AttendanceDate = AppCommon.JsonDateToNormalDate(rowdata.AttendanceDate);
        obj.AttendanceYear = rowdata.AttendanceYear;
        obj.ClassDetailsKey = rowdata.ClassDetailsKey;
        obj.BranchKey = rowdata.BranchKey;
        obj.AttendanceTypeKey = rowdata.AttendanceTypeKey;
        //obj.IfEdit = true;


        return '<div class="divEditDelete"><a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteAttendance(' + rowdata.RowKey + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }
    var checkConfirmAttendance = function () {
        var $form = $("#frmAttendance")
        var JsonData = [];
        var formData = $form.serializeToJSON({ associativeArrays: false });
        var AttendanceDetails = $.extend(true, [], formData.AttendanceDetails);
        $form.mLoading();
        $(formData.AttendanceDetails).each(function (i, item) {
            if (item.AttendanceStatusDetailsViewModel) {
                item.AttendanceStatusDetailsViewModel = item.AttendanceStatusDetailsViewModel.filter(function () {
                    return true;
                });
                item.AttendanceStatusDetailsViewModel.splice(1);
            }
        });
        var AbsentList = formData.AttendanceDetails.filter(function (item) {
            return item.AttendanceStatusDetailsViewModel && !item.AttendanceStatusDetailsViewModel[0].AttendanceStatus;
        });
        if ($form.valid()) {
            if (AbsentList.length > 0) {
                var content = "";
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnAbsentListFilePath").val(),
                    async: false,
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        content = template({ AbsentStudents: AbsentList });
                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })
                $.confirm({
                    title: Resources.AbsentStudents,
                    content: content,
                    type: 'orange',
                    icon: 'fa fa-exclamation-circle',
                    useBootstrap: true,
                    columnClass: 'col-md-4 col-md-offset-8 col-xs-4 col-xs-offset-8',
                    containerFluid: true,
                    theme: 'bootstrap',
                    buttons: {

                        confirm:
                        {
                            text: Resources.Confirm,
                            btnClass: "btn-danger",
                            action: function (jc) {
                                Attendance.FormSubmit($form, formData);

                            }

                        },

                        cancel:
                        {
                            btnClass: "btn-dark",
                            text: Resources.Cancel,
                            action: function () {
                                $form.mLoading("destroy");
                            }
                        }
                    }
                });
            } else {
                Attendance.FormSubmit($form, formData);
            }
        }
    }

    function formSubmit($form, formData) {

        var dataurl = $form.attr("action");
        var response = [];

        AjaxHelper.ajaxAsync("POST", dataurl,
            {
                model: formData
            }, function () {
                response = this;
                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message)
                    var teacherPortalRebind = localStorage.getItem('teacherPortalRebind');
                    if (teacherPortalRebind) {
                        var teacherPortalRebind = eval('(' + teacherPortalRebind + ')');

                    } else {
                        Attendance.LoadData();
                    }



                }
                $form.mLoading("destroy");
            });

    }

    var loadData = function () {
        var model = {};
        $("#dynamicRepeater").mLoading();

        model.AttendanceDate = $("#AttendanceDate").val();
        model.BranchKey = $("#BranchKey").val();
        model.BatchKey = $("#BatchKey").val();
        model.ClassDetailsKey = $("#ClassDetailsKey").val();


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
            },
            error: function (request, status, error) {

            }
        });


    }
    var setCheckboxStatusColor = function (_this) {
        //var elmnt = $(".table-attendance td:last-child")[0];
        //elmnt.scrollIntoView();

        var chk = $("#IfPresent")[0];
        var firstColumn;
        var checkedCntrls = $("input[type=checkbox][id*=AttendanceStatus]:not(:disabled):checked");
       
        if (_this) {
            _this.checked = !_this.checked;
            var item = $(_this).closest("[data-repeater-item]");
            var label = $(_this).closest("label");
            var AttendanceStatusKey = $("[name*=AttendanceStatusKey]", label).val();
            AttendanceStatusKey = parseInt(AttendanceStatusKey) ? parseInt(AttendanceStatusKey) : 0;
            if (!AttendanceStatusKey)
                $(_this).nextAll("label").html("")
            if (_this.checked) {
                var obj = {};

                obj.RowKey = $("[name*=AttendanceDetailRowKey]", label).val();
                obj.ApplicationKey = $("[name*=ApplicationKey]", item).val();
                obj.AttendanceTypeKey = $("[name*=AttendanceTypeKey]", label).val();
                obj.AttendanceDate = $("#AttendanceDate").val();
                AjaxHelper.ajaxAsync("POST", $("#hdnCheckAttendanceBlocked").val(),
                    obj, function () {
                        response = this;
                        /* if (response.IsSuccessful) {*/
                        if (_this.checked) {
                            $(_this).closest("label").css("background-color", "green")
                            $(_this).nextAll("label").html("P")
                            $(_this).closest("label").css("color", AppCommon.SetColorByBackgroundIntensity("green"));
                            $("[name*=AttendanceStatusKey]", label).val(Resources.AttendanceStatusPresent);
                        } else {
                            $(_this).closest("label").css("background-color", "red")
                            $(_this).nextAll("label").html("A")
                            $(_this).closest("label").css("color", AppCommon.SetColorByBackgroundIntensity("red"));
                            $("[name*=AttendanceStatusKey]", label).val(Resources.AttendanceStatusAbsent);
                        }
                        $(_this).closest("tr").find("td").eq(0).css("background-color", "#e6f1ff")
                        //} else {
                        //    _this.checked = false;
                        //    //$(_this).closest("label").css("background-color", "white")
                        //    //$(_this).closest("tr").find("td").eq(0).css("background-color", "")
                        //    $.alert({

                        //        title: Resources.Warning,
                        //        content: response.Message,
                        //        type: "warning",
                        //        icon: "",
                        //        buttons: {

                        //            Ok:
                        //                {
                        //                    text: Resources.Ok,
                        //                    btnClass: "btn-warning"
                        //                }
                        //        }
                        //    });
                        //}
                    });
            }
            else {
                $(_this).closest("label").css("background-color", "white")
                $(_this).closest("tr").find("td").eq(0).css("background-color", "#fff")
                $("[name*=AttendanceStatusKey]", $(_this).closest("label")).val(Resources.AttendanceStatusAbsent);
                $("[name*=AttendanceStatusKey]", $(_this)).val(Resources.AttendanceStatusAbsent);

            }

        }
        else {
            //$("input[type=checkbox][id*=AttendanceStatus]:not(:disabled)").css("background-color", "white");
            //if (chk.checked) {
            //    $(checkedCntrls).prop("checked", true);
            //    $(checkedCntrls).nextAll("label").html("P")
            //    $(checkedCntrls).closest("label").css("background-color", "green")
            //    $(checkedCntrls).closest("label").each(function () {
            //        $("[name*=AttendanceStatusKey]", this).val(Resources.AttendanceStatusPresent);
            //    })
            //} else {
            //    $(checkedCntrls).prop("checked", false);
            //    //$(checkedCntrls).nextAll("label").html("A")
            //    $(checkedCntrls).closest("label").css("background-color", "white")
            //    $(checkedCntrls).closest("label").each(function () {
            //        $("[name*=AttendanceStatusKey]", this).val(Resources.AttendanceStatusAbsent);
            //    })

            //}

            $("#dvStudentsList [data-repeater-item]").each(function () {
                
                var chk1 = $("input[type=checkbox][name*=AttendanceStatus]", $(this))
                var AttendanceDetailRowKey = $("input[type=hidden][name*=AttendanceDetailRowKey]", $(this)).val();
                AttendanceDetailRowKey = parseInt(AttendanceDetailRowKey) ? parseInt(AttendanceDetailRowKey) : 0;
                if (AttendanceDetailRowKey == 0) {
                    if (chk.checked) {
                        $(chk1).prop("checked", true);
                        $(chk1).nextAll("label").html("P")
                        $(chk1).closest("label").css("background-color", "green")
                        $(chk1).closest("label").each(function () {
                            $("[name*=AttendanceStatusKey]", this).val(Resources.AttendanceStatusPresent);
                        })
                    }
                    else {
                        $(chk1).prop("checked", false);
                        //$(checkedCntrls).nextAll("label").html("A")
                        $(chk1).closest("label").css("background-color", "white")
                        $(chk1).closest("label").each(function () {
                            $("[name*=AttendanceStatusKey]", this).val(Resources.AttendanceStatusAbsent);
                        })
                    }
                    $("#divchkPresent").show();
                }
                else {
                    $("#divchkPresent").hide();
                }

            })

        }

    }

    var getExportPrintData = function (type, url, filename, title, beforeProcessing) {

        var sortColumnName = $("#grid").jqGrid('getGridParam', 'sortname');
        var sortOrder = $("#grid").jqGrid('getGridParam', 'sortorder'); // 'desc' or 'asc'
        var rows = $("#grid").getRowData().length;
        //var row = $("#grid").jqGrid('getGridParam', 'records'); // All Records
        var page = $('#grid').getGridParam('page');


        var obj = {};
        obj.ajaxData = $("form").serializeToJSON({

        });
        var br = $("#ClassDetailsKey").find("option:selected").text();
        var br1 = $("#ClassDetailsKey").val();
        obj.Title = "Attendance Summary" + ($("#ClassDetailsKey").val() != "" ? ' - ' + $("#ClassDetailsKey").find("option:selected").text() : "");
        obj.SubTitle = $("#SearchFromDate").val() === $("#AttendanceDate").val() ? $("#SearchFromDate").val() : $("#SearchFromDate").val() + " - " + $("#AttendanceDate").val();
        obj.FileName = (obj.Title + (obj.SubTitle ? ("_" + obj.SubTitle) : "")).replace(/-/g, '_');


        obj.ajaxData.SearchText = $('#txtsearch').val();
        obj.ajaxData.SearchFromDate = $('#SearchFromDate').val();
        obj.ajaxData.searchDate = $('#AttendanceDate').val();
        obj.ajaxData.ClassDetailsKey = $('#ClassDetailsKey').val();
        obj.ajaxData.BatchKey = $('#BatchKey').val();
        obj.ajaxData.AttendanceStatusKey = $('#AttendanceStatusKey').val();
        obj.ajaxData.sidx = sortColumnName;
        obj.ajaxData.sord = sortOrder;
        obj.ajaxData.page = page;
        obj.ajaxData.rows = rows;
        obj.ajaxType = "POST";
        obj.ajaxUrl = $(url).val();
        obj.ContainerId = "#grid";
        /*obj.FileName = filename;*/
        obj.beforeProcessing = beforeProcessing ?? false;
        /*obj.Title = title;*/
        AppCommon.ExportPrintAjax(obj, type)


    }

    return {
        FormSubmit: formSubmit,
        CheckConfirmAttendance: checkConfirmAttendance,
        GetAttendance: getAttendance,
        GetAttendanceQuick: getAttendanceQuick,
        LoadData: loadData,
        SetCheckboxStatusColor: setCheckboxStatusColor,
        GetExportPrintData: getExportPrintData
    }



}());

function deleteAttendance(RowKey) {
    var obj = {};

    //obj.AttendanceDate = AppCommon.JsonDateToServerDate(AttendanceDate);
    //obj.CourseKey = CourseKey,
    //obj.UniversityMasterKey = UniversityMasterKey,
    //obj.BatchKey = BatchKey,
    //obj.AttendanceYear = AttendanceYear;
    //obj.ClassDetailsKey = ClassDetailsKey;
    //obj.BranchKey = BranchKey;
    //obj.AttendanceTypeKey = AttendanceTypeKey;
    //obj.AttendanceTypeDescreptionKey = AttendanceTypeDescreptionKey;

    obj.RowKey = RowKey;
    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteAttendance").val(),
        parameters: obj,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}
function deleteBulkAttendance() {

    var myGrid = $('#grid');
    var ids = $("#grid").jqGrid('getDataIDs');
    var selRowIds = myGrid.jqGrid("getGridParam", "selarrrow"), n, rowData;
    var TotalRecords = myGrid.getGridParam("reccount")

    var Arraystring = selRowIds.toString();
    var Arrayjoin = selRowIds.join(', ');;


    if (TotalRecords > 0) {
        var obj = {};
        obj.Keys = Arraystring;
        var result = EduSuite.Confirm2({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Attendance,
            actionUrl: $("#hdnDeleteAttendanceBulk").val(),
            parameters: obj,
            dataRefresh: function () {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        });
    }
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




