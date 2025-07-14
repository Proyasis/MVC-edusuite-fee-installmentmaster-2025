var AttendanceCategory = (function () {
    var getAttendanceCategory = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAttendanceCategory").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Name, Resources.Code, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'MasterRowKey', index: 'MasterRowKey', editable: true },
                { key: false, name: 'AttendanceCategoryName', index: 'AttendanceCategoryName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AttendanceCategoryCode', index: 'AttendanceCategoryCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
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
            altclass: 'jqgrid-altrow'
        })

        $("#grid").jqGrid("setLabel", "AttendanceCategoryName", "", "thAttendanceCategoryName");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.MasterRowKey + "'";
        var html = '<a class="btn btn-primary btn-sm mx-1" onclick="AttendanceCategory.EditPopup(this)" data-href="' + $("#hdnAddEditAttendanceCategory").val() + '/' + rowdata.MasterRowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a >';
        if (rowdata.MasterRowKey != 1) {
            html = html + '<a class="btn btn-danger btn-sm mx-1" href="#"   onclick="javascript:deleteAttendanceCategory(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>';
        }
        return html;
    }

    var editPopup = function (_this) {
        var URL = $(_this).data("href");
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                setTimeout(function () {
                    AttendaceCategoryPopupLoad();
                    
                }, 500);
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);
    }
    function checkCheckBox(_this) {
        if (_this.id == "OverTimeFormulaKey") {
            if (_this.selectedIndex == 0) {
                $("#MinOvertime").attr("disabled", "disabled");
                $("#MaxOvertime").attr("disabled", "disabled");
            }
            else {
                $("#MinOvertime").removeAttr("disabled", "disabled");
                $("#MaxOvertime").removeAttr("disabled", "disabled");
            }
        }
        else {
            if (_this.checked) {
                if (_this.id == "CalcuteHalfDay") {
                    $("#HalfDayMins").removeAttr("disabled", "disabled");
                }
                else if (_this.id == "CalculateAbsent") {
                    $("#AbsentMins").removeAttr("disabled", "disabled");
                }
                else if (_this.id == "PartCalculateHalfDay") {
                    $("#PartHalfDayMins").removeAttr("disabled", "disabled");
                }
                else if (_this.id == "PartCalculateAbsent") {
                    $("#PartAbsentMins").removeAttr("disabled", "disabled");
                }
                else if (_this.id == "MarkAsAbsentForLate") {
                    $("#AbsentDayType").removeAttr("disabled", "disabled");
                    $("#AbsentDayType").selectpicker("refresh");
                    $("#ContiousLateDay").removeAttr("disabled", "disabled");
                }
                else if (_this.id == "MarkHalfdayForLaterGoing") {
                    $("#HalfDayLateByMins").removeAttr("disabled", "disabled");
                }
                else if (_this.id == "MarkHalfdayForEarlyGoing") {
                    $("#HalfDayEarlyByMins").removeAttr("disabled", "disabled");
                }
            }
            else {
                if (_this.id == "CalcuteHalfDay") {
                    $("#HalfDayMins").attr("disabled", "disabled");
                }
                else if (_this.id == "CalculateAbsent") {
                    $("#AbsentMins").attr("disabled", "disabled");
                }
                else if (_this.id == "PartCalculateHalfDay") {
                    $("#PartHalfDayMins").attr("disabled", "disabled");
                }
                else if (_this.id == "PartCalculateAbsent") {
                    $("#PartAbsentMins").attr("disabled", "disabled");
                }
                else if (_this.id == "MarkAsAbsentForLate") {
                    $("#AbsentDayType").attr("disabled", "disabled");
                    $("#AbsentDayType").selectpicker("refresh");
                    $("#ContiousLateDay").attr("disabled", "disabled");

                }
                else if (_this.id == "MarkHalfdayForLaterGoing") {
                    $("#HalfDayLateByMins").attr("disabled", "disabled");
                }
                else if (_this.id == "MarkHalfdayForEarlyGoing") {
                    $("#HalfDayEarlyByMins").attr("disabled", "disabled");
                }
            }
        }
    }

    var attendanceCategoryWeekOffDetails = function () {
        $("#dvWeekOffRepeater").repeater({
            show: function () {
                $(this).slideDown();
                $("[name*=RowKey]", this).val("0");
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();

            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteAttendanceCategoryWeekOff($(hidden).val(), $(this));
                }
                else {
                    $(this).slideUp(remove);
                }

            },

            repeatlist: 'AttendanceCategoryWeekOffs'

        });
    }

    return {
        GetAttendanceCategory: getAttendanceCategory,
        EditPopup: editPopup,
        CheckCheckBox: checkCheckBox,
        AttendanceCategoryWeekOffDetails: attendanceCategoryWeekOffDetails
    }
}());

function deleteAttendanceCategory(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_AttendanceCategory,
        actionUrl: $("#hdnDeleteAttendanceCategory").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteAttendanceCategoryWeekOff(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_AttendanceCategory,
        actionUrl: $("#hdnDeleteAttendanceCategoryWeekOff").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            window.location.reload();
        }
    });
}

function AttendaceCategoryPopupLoad() {
    AppCommon.FormatInputCase();
    AppCommon.CustomRepeaterRemoteMethod();
    AttendanceCategory.AttendanceCategoryWeekOffDetails();
   
    $(".input-group-addon-end").each(function () {
        AppCommon.SetInputAddOn(this);
    });
    AttendanceCategory.CheckCheckBox($("#OverTimeFormulaKey")[0]);
    AttendanceCategory.CheckCheckBox($("#CalcuteHalfDay")[0]);
    AttendanceCategory.CheckCheckBox($("#CalculateAbsent")[0]);
    AttendanceCategory.CheckCheckBox($("#PartCalculateHalfDay")[0]);
    AttendanceCategory.CheckCheckBox($("#PartCalculateAbsent")[0]);
    AttendanceCategory.CheckCheckBox($("#MarkAsAbsentForLate")[0]);
    AttendanceCategory.CheckCheckBox($("#MarkHalfdayForLaterGoing")[0]);
    AttendanceCategory.CheckCheckBox($("#MarkHalfdayForEarlyGoing")[0]);


}



