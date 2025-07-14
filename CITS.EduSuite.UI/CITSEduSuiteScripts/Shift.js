var jsonData = [];
var Shift = (function () {

    var getShift = function (json) {
        jsonData = json;
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetShift").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.ShiftName, Resources.ShiftCode, Resources.BeginTime, Resources.EndTime, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'MasterRowKey', index: 'MasterRowKey', editable: true },
                //{ key: false, hidden: true, name: 'Break1BeginTime', index: 'Break1BeginTime', editable: true, formatter: formatTime },
                //{ key: false, hidden: true, name: 'Break1EndTime', index: 'Break1EndTime', editable: true, formatter: formatTime },
                //{ key: false, hidden: true, name: 'Break2BeginTime', index: 'Break2BeginTime', editable: true, formatter: formatTime },
                //{ key: false, hidden: true, name: 'Break2EndTime', index: 'Break2EndTime', editable: true, formatter: formatTime },
                { key: false, name: 'ShiftName', index: 'ShiftName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ShiftCode', index: 'ShiftCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BeginTime', index: 'BeginTime', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatTime },
                { key: false, name: 'EndTime', index: 'EndTime', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatTime },
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

        $("#grid").jqGrid("setLabel", "ShiftName", "", "thShiftName");
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.MasterRowKey + "'";
        var html = '<a class="btn btn-primary btn-sm mx-1" data-href="' + $("#hdnAddEditShift").val() + '/' + rowdata.MasterRowKey + '" onclick="Shift.EditPopup(this);return false;"><i class="fa fa-pencil" aria-hidden="true"></i></a>';
        if (rowdata.RowKey != 1) {
            html = html + '<a class="btn btn-danger btn-sm mx-1" href="#" onclick="javascript:deleteShift(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>';
        }
        return html;
    }

    function formatTime(cellValue, options, rowdata, action) {
        var columnName = options.colModel["name"];
        var string = options.gid + "_" + columnName;
        setTimeout(function () {
            if (columnName == "BeginTime") {
                $("td[aria-describedby=" + string + "]").addClass("text-green")
            }
            else {
                $("td[aria-describedby=" + string + "]").addClass("text-red")
            }
        }, 100)
        if (cellValue != null) {
            //(cellValue.Hours).toLocaleString('en-US', { minimumIntegerDigits: 2, useGrouping: false }) + ":" + (cellValue.Minutes).toLocaleString('en-US', { minimumIntegerDigits: 2, useGrouping: false })
            return AppCommon.FormatObjectToTimeAMPM(cellValue);
        }
        else {
            return "";
        }
    }
    var editPopup = function (_this) {
        var url = $(_this).attr("data-href");
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                setTimeout(function () {
                    ShiftPopupLoad();
                }, 500)
            },
            submit: function () {
                var form = $('#frmShift');

                var validate = $(form).valid();

                if (validate) {
                    $(form).mLoading();
                    var obj = $(form).serializeToJSON({
                        associativeArrays: false
                    });

                    ChangeTimeFormat(obj);
                    $.ajax({
                        url: $(form)[0].action,
                        type: $(form)[0].method,
                        data: obj,
                        success: function (result) {
                            if (result.IsSuccessful) {
                                $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                                $(form).closest(".modal").modal("hide");
                            }
                            else {
                                $(form).closest(".modal-content").html(result);
                            }
                            $(form).mLoading("destroy");

                        },
                        error: function (xhr) {
                            $(form).mLoading("destroy");
                        }
                    });
                }
            }
        }, url);
    }
    var enableDisableControls = function (_this) {
        var row = $("[data-disable=" + $(_this).prop("id"));
        if (_this.checked) {
            $("input[type=text],select", row).prop('disabled', false);

        }
        else {
            $("input[type=text],select", row).val("")
            $("input[type=text],select", row).prop('disabled', true);
        }
        $("select", row).selectpicker("refresh");
    }
    var shiftWeekOffDetails = function () {
        $("#dvBreakRepeater").repeater({
            show: function () {
                $(this).slideDown();
                $("[name*=RowKey]", this).val("0");
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatDateInput();
                AppCommon.FormatInputCase();
                AppCommon.FormatTimeInput();
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

            repeatlist: 'ShiftBreaks'

        });
    }
    return {
        GetShift: getShift,
        EditPopup: editPopup,
        EnableDisableControls: enableDisableControls,
        ShiftWeekOffDetails: shiftWeekOffDetails
    }

}());

function deleteShift(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Shift,
        actionUrl: $("#hdnDeleteShift").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteAttendanceCategoryWeekOff(rowkey,_this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Shift,
        actionUrl: $("#hdnDeleteShiftBreak").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            
            var item = $(_this).closest("[data-repeater-item]");
            if (response.IsSuccessful)
                $(item).remove();
        }
    });
}

function ShiftPopupLoad() {
    $("[data-input-type =time]").each(function () {
        if ($(this).val() != "") {
            $(this).val(moment($(this).val(), ["HH:mm:ss"]).format("hh:mm A"));
        }
    });
    AppCommon.CustomRepeaterRemoteMethod();

    AppCommon.FormatDateInput();
    AppCommon.FormatInputCase();
    AppCommon.FormatTimeInput();
    Shift.ShiftWeekOffDetails();

    $("input[type=checkbox]#IfBreak1,input[type=checkbox]#IfBreak2,input[type=checkbox]#PartialDay").each(function () {
        Shift.EnableDisableControls(this);
    })
}

function ChangeTimeFormat(obj) {
    for (var key in obj) {
        if ($.isPlainObject(obj[key])) {
            ChangeTimeFormat(obj[key]);
        } else if ($.isArray(obj[key])) {
            $.each(obj[key], function () {
                ChangeTimeFormat(this);
            });

        }
        else {
            //if (key == "BeginTime" || key == "EndTime") {
            if (key.includes("Time")) {
                obj[key] = obj[key] != "" ? moment(obj[key], ["h:mm:ss A"]).format("HH:mm:ss") : null;
            }
            else {
                obj[key] = obj[key];
            }
        }
    };
}