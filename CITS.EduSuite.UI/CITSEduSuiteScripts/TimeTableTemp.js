var TimeTableTemp = (function () {
    var getTimeTableTempMaster = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetTimeTableTempMaster").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtSearchName').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Employee, Resources.FromDate, Resources.ToDate, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FromDate', index: 'FromDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'ToDate', index: 'ToDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },

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
            //loadComplete: function (data) {
            //    $("#grid a[data-modal='']").each(function () {
            //        AppCommon.EditGridPopup($(this))
            //    });

            //}
        })

        $("#grid").jqGrid("setLabel", "UniversityCourseName", "", "thUniversityCourseName");
    }
    var getTimeTableTemp = function () {
        var model = {};
        model.RowKey = $("#RowKey").val();
        model.EmployeeKey = $("#EmployeeKey").val();
        model.Day = $("#tab-day li a.active").data('val');

        $.ajax({
            type: "Get",
            url: $("#hdnUrl").val(),
            contentType: "application/json; charset=utf-8",
            data: model,
            success: function (result) {
                if (result.IsSuccessful == false) {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("#dvTimeTableDetails").html("")
                $("#dvTimeTableDetails").html(result);

            },
            error: function (request, status, error) {

            }
        });
    }
    var getEmployeeById = function (_this) {

        var obj = {};
        obj.BranchKey = $("#BranchKey").val();
        var dropdDownControl = $("[name*=EmployeeKey]", _this);
        $.ajax({
            type: "GET",
            url: $("#hdnFillAllTeachers").val(),
            dataType: "json",
            data: obj,
            contentType: "application/json; charset=utf-8",
            beforeSend: function () {
                $(dropdDownControl).empty();
                $(dropdDownControl).append($("<option loading></option>").val("").html("<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span>  Loading...</span>"));
                $(dropdDownControl).selectpicker('refresh');

            },
            success: function (response) {
                response = response.filter(function (item) {
                    var EmployeeKey = $("#EmployeeKey").val();
                    return item.RowKey != EmployeeKey;
                })
                $(dropdDownControl).append($("<option></option>").val("").html(" "));
                $.each(response, function () {
                    $(dropdDownControl).append($("<option></option>").val(this['RowKey']).html(this['Text']));
                });
                $(dropdDownControl).each(function () {
                    if (parseInt(this.dataset.val))
                        $(this).val(this.dataset.val);
                })




            },
            complete: function () {
                $(dropdDownControl).find("option[loading]").remove();
                $(dropdDownControl).selectpicker('refresh');
                $(".bs-caret", _this).hide();
            }
        })
    }


    function formSubmit() {

        var $form = $("#frmAddEditTimeTableTemp")
        var JsonData = [];
        var formData = $form.serializeToJSON({ associativeArrays: false });






        if ($form.valid()) {
            $form.mLoading();

            if (formData[""])
                formData.TimeTableTempDetailsModel = formData[""].map(function (item) {
                    item.Day = $("#tab-day li a.active").data('val');
                    return item;
                }).filter(function (item) {
                    return item.ToEmployeeKey || parseInt(item.RowKey);
                });

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
                                               window.location.href = $("#hdnTimeTableTempList").val();
                                           }
                                       }
                                   }
                               })

                           }
                           $form.mLoading("destroy");
                       });


        }
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-primary btn-sm mx-1" data-href="' + $("#hdnAddEditAttendanceTypeMaster").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-danger btn-sm mx-1" href="#"   onclick="javascript:deleteAttendanceTypeMaster(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
        return '<div class="divEditDelete"><a class="btn btn-primary btn-sm mx-1" href="' + $("#hdnAddEditTimeTableTemp").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-danger btn-sm mx-1" href="#"   onclick="javascript:deleteTimeTableTemp(' + rowdata.RowKey + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';

    }
    return {
        GetTimeTableTemp: getTimeTableTemp,
        GetEmployeeById: getEmployeeById,
        FormSubmit: formSubmit,
        GetTimeTableTempMaster: getTimeTableTempMaster
    }
}());


function deleteTimeTableTemp(rowkey) {

    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Attendance,
        actionUrl: $("#hdnDeleteTimeTableTempMaster").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}
