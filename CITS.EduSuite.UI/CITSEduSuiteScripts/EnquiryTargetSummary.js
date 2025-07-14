request = null, removeCols = ["subgrid", "edit"], ApplicationJsonData = [];
var EnquiryTargetSummary = (function () {



    // Student Summery Start


    var getEnquiryTargetSummary = function () {

        var JsonData = $("form").serializeToJSON({

        });

        var daysInMonth = new moment(JsonData["DateAddedFrom"]).daysInMonth();
        var monthsInYear = 12;
        var MonthKey = new moment(JsonData["DateAddedFrom"]).month() + 1;
        var YearKey = new moment(JsonData["DateAddedFrom"]).year();
        JsonData["DateAddedFrom"] = "JAN " + JsonData["DateAddedFrom"];
        JsonData["EmployeeKey"] = 0;
        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'CommonTarget', index: 'CommonTarget', editable: true },
            { key: false, name: 'EmployeeCode', frozen: true, headerText: Resources.EmployeeCode, index: 'EmployeeCode', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
            { key: false, name: 'EmployeeName', headerText: Resources.Employee, index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 180 },

        ];
        for (var i = 1; i <= monthsInYear; i++) {
            var obj = { key: false, pivote: true, editable: true, cellEdit: true, sortable: false, resizable: false, width: 100, formatter: formatAttendanceColumn, align: 'center' };
            var dateString = (MonthKey < 10 ? "0" + MonthKey : MonthKey) + "/" + (i < 10 ? "0" + i.toString() : i.toString()) + "/" + YearKey;
            obj.name = i.toString();
            obj.index = i.toString();
            //var weekDay = moment(dateString).weekday();
            //var currMonthName = moment().format('MMMM');
            //var month = moment(i.toString(), 'M').format('MMMM')
            //var prevMonthName = moment().subtract(i.toString(), "month").format('MMMM');
            //obj.headerText = "<pre " + (weekDay == 0 ? "data-weekend" : "") + " style='font-size: 11px;position: absolute;top: 0;white-space: initial;height: 30px;text-align: center;'>" + moment(dateString).format('mmm') + "\r\n" + i.toString() + "</pre>";
            obj.headerText = "<pre style='font-size: 1em;position: relative;bottom: 6px;;height: auto;text-align: center;'>" + moment(i.toString(), 'M').format('MMMM') + "\r\n" + i.toString() + "</pre>";
            colModelList.push(obj);
            removeCols.push(obj.name);
        }

        GenerateShowHideColumnList(colModelList, ["EmployeeCode", "EmployeeName", "WorkingDays", "Present", "Absent"])
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid('GridUnload');
        //var gridWidth = $(".jqGrid_wrapper").width();
        $("#grid").jqGrid({

            url: $("#hdnGetEnquiryTargetSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },

            colNames: colNames,
            colModel: colModelList,
            scroll: false,
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [10, 15, 50, 100],
            autowidth: true,
            shrinkToFit: 1,
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
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
            loadComplete: function (data) {
                $("[data-weekend]").each(function () {
                    var id = $(this).closest("th").prop("id");
                    $(this).closest("th").css("background", "#f1e4c5");
                    $("[aria-describedby=" + id + "]").css("background", "#f1e4c5");
                })
                EnquiryTargetSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }



        })

        $("#grid").jqGrid('setFrozenColumns');
        $("#grid").jqGrid("setLabel", "AdmissionNo", "", "thAdmissionNo");


    }

    var getEmployeeAttendance = function (JsonData) {
        ApplicationJsonData = JsonData;
        JsonData["DateAdded"] = null;
        var colModelList = [
            { key: true, hidden: true, frozen: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'MonthName', frozen: true, headerText: Resources.Month, index: 'MonthName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
            { key: false, name: 'WorkingDays', frozen: true, headerText: Resources.WorkingDays, index: 'WorkingDays', formatter: formatWorkingDaysColumn, editable: true, cellEdit: true, sortable: true, resizable: false, width: 60 },
            { key: false, name: 'Present', frozen: true, headerText: Resources.Present, index: 'Present', formatter: formatPresentPercentageColumn, editable: true, cellEdit: true, sortable: true, resizable: false, width: 60 },
            { key: false, name: 'Absent', frozen: true, headerText: Resources.Absent, index: 'Absent', formatter: formatAbsentPercentageColumn, editable: true, cellEdit: true, sortable: true, resizable: false, width: 60 },

        ];

        for (var i = 1; i <= 31; i++) {
            var obj = { key: false, pivote: true, editable: true, cellEdit: true, sortable: false, resizable: false, width: 38, formatter: formatAttendanceColumn, align: 'center' };
            obj.name = i.toString();
            obj.index = i.toString();
            obj.headerText = i.toString();
            colModelList.push(obj);
        }
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#gridApplication").jqGrid('GridUnload');
        var gridWidth = $("#gridApplication").closest(".jqGrid_wrapper").width();
        $("#gridApplication").jqGrid({

            url: $("#hdnGetEmployeeAttendanceReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                }).toArray();
                var TotalPresent = 0;
                var TotalPresentPercent = 0;
                var TotalAbsent = 0;
                var TotalAbsentPercent = 0;
                $(data.rows).each(function (n, item) {
                    obj.rowdata = item;
                    obj.type = "P";
                    calculatePercentageByType(obj)
                    TotalPresent = TotalPresent + obj.ItemCount;
                    TotalPresentPercent = TotalPresentPercent + obj.Percentage;
                });

                $(data.rows).each(function (n, item) {
                    obj.rowdata = item;
                    obj.type = "A";
                    calculatePercentageByType(obj)
                    TotalAbsent = TotalAbsent + obj.ItemCount;
                    TotalAbsentPercent = TotalAbsentPercent + obj.Percentage;
                });
                var TotalWorkingDays = TotalPresent + TotalAbsent;
                $("#spnTotalWorkingDays").html(TotalWorkingDays);
                $("#spnTotalPresent").html(TotalPresent + "(" + AppCommon.RoundTo(TotalPresentPercent, Resources.RoundToDecimalPostion) + "%)");
                $("#spnTotalAbsent").html(TotalAbsent + "(" + AppCommon.RoundTo(TotalAbsentPercent, Resources.RoundToDecimalPostion) + "%)");
            },

            colNames: colNames,
            colModel: colModelList,
            scroll: false,
            pager: jQuery('#pagerApplication'),
            rowNum: 10,
            rowList: [10, 15, 50, 100],

            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
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
            loadComplete: function (data) {
                $("[data-weekend]").each(function () {
                    var id = $(this).closest("th").prop("id");
                    $(this).closest("th").css("background", "#f1e4c5");
                    $("[aria-describedby=" + id + "]").css("background", "#f1e4c5");
                })

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }



        })
        //$("#gridApplication").jqGrid('setFrozenColumns');
        $("#gridApplication").jqGrid("setLabel", "AdmissionNo", "", "thAdmissionNo");


    }


    function formatAttendanceAction(cellValue, options, rowdata, action) {
        var id = "'" + rowdata.ApplicationKey + "'";
        return '<button type="button" onclick="AttendanceReport.ApplicationAttendancePopup(' + id + ')" class="btn btn-primary btn-sm" style="font-size: 10px;" >' + Resources.View + '</button>';


    }
    function formatAttendanceColumn(cellValue, options, rowdata, action) {
        var html = "";
        var admissionTaken = "", TargetCount = "";
        if (cellValue) {
            var array = cellValue.split("/");
            admissionTaken = array[1]
            TargetCount = array[0]
            if (TargetCount.toUpperCase() != "") {
                TargetCount = "<span style='color:green;font-weight:bold;font-size: 11px'>" + TargetCount + "</span>"
            }
            if (admissionTaken.toUpperCase() != "") {
                admissionTaken = "<span style='color:red;font-weight:bold;font-size: 11px'>" + admissionTaken + "</span>"
            } else {
                admissionTaken = "<span style='color:red;font-weight:bold;font-size: 11px'>" + "_" + "</span>"
            }
            html = TargetCount + " / " + admissionTaken;

        } else {
            if (rowdata.CommonTarget) {
                TargetCount = "<span style='color:green;font-weight:bold;font-size: 11px'>" + rowdata.CommonTarget + "</span>"
                admissionTaken = "<span style='color:red;font-weight:bold;font-size: 11px'>" + "_" + "</span>"
                html = TargetCount + " / " + admissionTaken;
            } else {
                html = "";
            }

        }
        return html;


    }
    function formatPresentPercentageColumn(cellValue, options, rowdata, action) {
        var html = "";
        var obj = {};
        obj.rowdata = rowdata;
        obj.type = "P";
        calculatePercentageByType(obj);
        if (obj.Percentage) {
            html = obj.ItemCount.toString() + " (" + AppCommon.RoundTo(obj.Percentage, Resources.RoundToDecimalPostion) + "%)";
        } else {
            html = "";
        }
        return html;


    }
    function formatAbsentPercentageColumn(cellValue, options, rowdata, action) {
        var html = "";
        var obj = {};
        obj.rowdata = rowdata;
        obj.type = "A";
        calculatePercentageByType(obj);
        if (obj.Percentage) {
            html = obj.ItemCount.toString() + " (" + AppCommon.RoundTo(obj.Percentage, Resources.RoundToDecimalPostion) + "%)";
        } else {
            html = "";
        }
        return html;

    }

    function formatWorkingDaysColumn(cellValue, options, rowdata, action) {
        var html = "";
        var Count = 0;
        for (var key in rowdata) {
            if (parseInt(key) && rowdata[key]) {
                Count++;
            }


        }
        if (Count) {
            html = Count.toString();
        } else {
            html = "";
        }
        return html;

    }
    function calculatePercentageByType(obj) {
        var Count = 0;
        var itemCount = 0;
        var aType = obj.type == "P" ? "A" : "P";
        for (var key in obj.rowdata) {
            if (parseInt(key) && obj.rowdata[key]) {
                if (obj.rowdata[key] == obj.type) {
                    itemCount++;
                }
                else if (obj.rowdata[key] != aType) {
                    var list = obj.rowdata[key].split("/");
                    var list0 = list[0];
                    var list1 = list[1];
                    var value0 = list0.replace(/\D/g, '');
                    value0 = parseInt(value0) ? parseInt(value0) : 0;
                    var value1 = list1.replace(/\D/g, '');
                    value1 = parseInt(value1) ? parseInt(value1) : 0;
                    itemCount = itemCount + (1 * ((obj.type == "P" ? value0 : value1) / (value0 + value1)));
                }
                Count++;
            }


        }
        obj.ItemCount = itemCount;
        obj.Percentage = itemCount * 100 / Count;
        var UnversityCount = $("#UniversityAttendanceCount").val();
        UnversityCount = parseInt(UnversityCount) ? parseInt(UnversityCount) : 0;
        obj.UniversityCount = obj.Percentage * UnversityCount / 100;


    }

    var applicationAttendancePopup = function (id) {
        var obj = $("form").serializeToJSON({

        });
        obj.ApplicationKey = id;
        $.customPopupform.CustomPopup({
            ajaxType: "post",
            ajaxData: obj,
            modalsize: "modal-xl  mw-100 w-100"

        }, $("#hdnGetApplicationAttendance").val());

    }
    var getExportPrintData = function (type, url, filename, title) {

        var obj = {};
        obj.ajaxData = $("form").serializeToJSON({

        });
        //var AttendanceType = $("[id*=rblAttendanceType]:checked").val();
        //AttendanceType = parseInt(AttendanceType) ? parseInt(AttendanceType) : 0;
        obj.ajaxData["DateAdded"] = moment("01 " + $("#DateAdded").val(), "DD MMM YYYY").format("DD/MM/YYYY");
        //if (AttendanceType)
        //    obj.ajaxData["UniversityDays"] = parseFloat(obj.ajaxData["UniversityDays"]) ? parseFloat(obj.ajaxData["UniversityDays"]) : 0;
        //else
        obj.ajaxData["UniversityDays"] = null;
        obj.beforeProcessing = function () {
            var data = this;
            data.rows = $(data.rows).map(function (n, item) {
                var obj = {};
                $(item).each(function () {
                    obj[this.Key] = this.Value;
                });
                return obj;
            });
        }

        obj.ajaxData.sidx = "RowKey";
        obj.ajaxData.sord = "asc"
        obj.ajaxData.page = 0;
        obj.ajaxData.rows = 0
        obj.ajaxType = "POST";
        obj.ajaxUrl = $(url).val();
        obj.ContainerId = "#grid";
        obj.FileName = filename;
        obj.Title = title;
        //obj.SubTitle = $("#UniversityDays").val() ? "University Days : " + $("#UniversityDays").val() : null;
        AppCommon.ExportPrintAjax(obj, type)


    }
    var getExportPrintEmployeeData = function (type, url, filename, title) {

        var obj = {};
        obj.ajaxData = ApplicationJsonData;
        obj.ajaxData["DateAdded"] = null;
        obj.beforeProcessing = function () {
            var data = this;
            data.rows = $(data.rows).map(function (n, item) {
                var obj = {};
                $(item).each(function () {
                    obj[this.Key] = this.Value;
                });
                return obj;
            });
        }

        obj.ajaxData.sidx = "RowKey";
        obj.ajaxData.sord = "asc"
        obj.ajaxData.page = 0;
        obj.ajaxData.rows = 0
        obj.ajaxType = "POST";
        obj.ajaxUrl = $(url).val();
        obj.ContainerId = "#gridApplication";
        obj.FileName = filename;
        obj.Title = title;
        //obj.SubTitle = $("#hdnApplicantName").val();
        AppCommon.ExportPrintAjax(obj, type)


    }

    // Common queries Start
    var getCustomizedColumns = function (obj) {

        var colList = $("#grid").jqGrid('getGridParam', 'colModel');
        colList = $(colList).filter(function (n, item) {
            return removeCols.indexOf(item.name) === -1;
        }).map(function (n, item) {
            return item.name;
        });
        var SelectedList = $('#ShowHideColumns').selectpicker('val');
        var CountSelection = SelectedList.length;


        $("#grid").hideCol($(colList).not(SelectedList));
        $("#grid").showCol(SelectedList);


        //if (CountSelection <= 6) {
        //    jQuery("#grid").setGridWidth("1000");
        //}
        //else {
        //    jQuery("#grid").setGridWidth(CountSelection * 150);
        //}

    }

    // Common queries End


    return {
        GetEnquiryTargetSummary: getEnquiryTargetSummary,
        GetCustomizedColumns: getCustomizedColumns,
        GetExportPrintData: getExportPrintData,
        GetExportPrintEmployeeData: getExportPrintEmployeeData,
        GetEmployeeAttendance: getEmployeeAttendance,
        ApplicationAttendancePopup: applicationAttendancePopup
    }

}());



function GenerateShowHideColumnList(data, DefaultColumns) {

    data = data.filter(function (n, p) {
        return !n.hidden
    })
    var ddl = $("#ShowHideColumns");
    $(ddl).html("")
    $(ddl).val('default').selectpicker("refresh");
    $.each(data, function (i, item) {
        $(ddl).append(
            $('<option ' + (DefaultColumns.indexOf(item.name) > -1 ? "selected=true" : "") + '></option>').val(item.name).html(item.headerText));
    });
    $(ddl).selectpicker("refresh");
}