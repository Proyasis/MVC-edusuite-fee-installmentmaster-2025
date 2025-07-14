request = null, removeCols = ["subgrid", "edit"], ApplicationJsonData = [];
var EmployeeAttendanceReport = (function () {



    //Old Report -24/Aug/2023
    //var getAttendanceReport = function () {
    //    var JsonData = $("form").serializeToJSON({
    //    });       
    //    var daysInMonth = new moment(JsonData["DateAdded"]).daysInMonth();
    //    var MonthKey = new moment(JsonData["DateAdded"]).month() + 1;
    //    var YearKey = new moment(JsonData["DateAdded"]).year();
    //    JsonData["EmployeeKey"] = 0;
    //    var colModelList = [
    //        { key: true, hidden: true, frozen: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
    //        { key: false, name: 'EmployeeCode', frozen: true, headerText: Resources.EmployeeCode, index: 'EmployeeCode', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
    //        { key: false, name: 'EmployeeName', frozen: true, headerText: Resources.EmployeeName, index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 150 },

    //    ];
    //        colModelList.push({ key: false, name: 'WorkingDays', frozen: true, headerText: Resources.WorkingDays, index: 'WorkingDays', formatter: formatWorkingDaysColumn, editable: true, cellEdit: true, sortable: true, resizable: false, width: 80 });
    //        colModelList.push({ key: false, name: 'Present', frozen: true, headerText: Resources.Present, index: 'Present', formatter: formatPresentPercentageColumn, editable: true, cellEdit: true, sortable: true, resizable: false, width: 80 });
    //        colModelList.push({ key: false, name: 'Absent', frozen: true, headerText: Resources.Absent, index: 'Absent', formatter: formatAbsentPercentageColumn, editable: true, cellEdit: true, sortable: true, resizable: false, width: 80 });

    //        for (var i = 1; i <= daysInMonth; i++) {
    //            var obj = { key: false, pivote: true, editable: true, cellEdit: true, sortable: false, resizable: false, width: 38, formatter: formatAttendanceColumn, align: 'center' };
    //            var dateString = (MonthKey < 10 ? "0" + MonthKey : MonthKey) + "/" + (i < 10 ? "0" + i.toString() : i.toString()) + "/" + YearKey;
    //            obj.name = i.toString();
    //            obj.index = i.toString();
    //            var weekDay = moment(dateString).weekday();
    //            obj.headerText = "<pre " + (weekDay == 0 ? "data-weekend" : "") + " style='font-size: 11px;position: absolute;top: 0;white-space: initial;height: 30px;text-align: center;'>" + moment(dateString).format('ddd') + "\r\n" + i.toString() + "</pre>";
    //            colModelList.push(obj);
    //            removeCols.push(obj.name);
    //        }

    //        GenerateShowHideColumnList(colModelList, ["EmployeeCode", "EmployeeName", "WorkingDays", "Present", "Absent"])
    //    var colNames = colModelList.map(function (item) {
    //        return item.headerText;
    //    });
    //    $("#grid").jqGrid('GridUnload');
    //    var gridWidth = $(".jqGrid_wrapper").width();
    //    $("#grid").jqGrid({

    //        url: $("#hdnGetEmployeeAttendanceReport").val(),
    //        datatype: 'json',
    //        mtype: 'Post',
    //        postData: JsonData,
    //        contenttype: 'application/json; charset=utf-8',
    //        beforeProcessing: function (data) {
    //            data.rows = $(data.rows).map(function (n, item) {
    //                var obj = {};
    //                $(item).each(function () {
    //                    obj[this.Key] = this.Value;
    //                });
    //                return obj;
    //            });
    //        },

    //        colNames: colNames,
    //        colModel: colModelList,
    //        scroll: false,
    //        pager: jQuery('#pager'),
    //        rowNum: 10,
    //        rowList: [10, 15, 50, 100],
    //        autowidth: 1,
    //        shrinkToFit: 1,
    //        width: gridWidth,
    //        height: '100%',
    //        viewrecords: true,
    //        emptyrecords: Resources.NoRecordsToDisplay,
    //        jsonReader:
    //        {
    //            root: "rows",
    //            page: "page",
    //            total: "total",
    //            records: "records",
    //            repeatitems: false,
    //            Id: "0"
    //        },
    //        multiselect: false,
    //        loadonce: true,
    //        ignoreCase: true,
    //        altRows: true,
    //        altclass: 'jqgrid-altrow',
    //        loadComplete: function (data) {
    //            $("[data-weekend]").each(function () {
    //                var id = $(this).closest("th").prop("id");
    //                $(this).closest("th").css("background", "#f1e4c5");
    //                $("[aria-describedby=" + id + "]").css("background", "#f1e4c5");
    //            })
    //            EmployeeAttendanceReport.GetCustomizedColumns();

    //        },
    //        onPaging: function () {
    //            //var CurrPage = $(".ui-pg-input", $("#pager")).val();
    //            //var Rows = $(".ui-pg-selbox").val();                
    //            // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
    //        }
    //    })        
    //    $("#grid").jqGrid('setFrozenColumns');
    //    $("#grid").jqGrid("setLabel", "AdmissionNo", "", "thAdmissionNo");   
    //}

    var getAttendanceReport = function () {

        var JsonData = $("form").serializeToJSON({

        });

        JsonData["DateAddedFrom"] = $("#DateAddedFrom").val();
        JsonData["DateAddedTo"] = $("#DateAddedTo").val();
        JsonData["EmployeeKey"] = 0;

        if (JsonData["BranchKey"] == "") {
            JsonData["BranchKey"] = 0;
        }

        $('#dvAttendanceReport').html("");
        $('#dvAttendanceReport').mLoading();
        $.ajax({
            type: "POST",
            url: $("#hdnGetEmployeeAttendanceReport").val() + "?no-cache=" + new Date().getTime(),
            data: JsonData,
            success: function (data) {
                var resultData = {};
                var xmlDoc = $.parseXML(data);
                data = AppCommon.Xml2Json(xmlDoc)
                resultData = data.Attendance;
                if (resultData) {
                    resultData.FullPath = Resources.FullPath;
                    resultData.ApplicationUrl = Resources.ApplicationUrl.replace("~", Resources.FullPath);

                    resultData.AttendanceHeads = resultData.Employee[0].AttendanceDetails.map(function (item) {
                        var obj = {};
                        obj.DayNumber = moment(item.AttendanceDate).date();
                        obj.DayName = moment(item.AttendanceDate).format('ddd');
                        obj.MonthName = moment(item.AttendanceDate).format('MMM');
                        obj.Year = moment(item.AttendanceDate).format('YYYY');
                        return obj;
                    });
                    var Multiplicant = 1;
                    $(resultData.Employee).each(function (i, item) {
                        if (item.AttendanceDetails) {
                            // Multiplicant = 1 / item.AttendanceDetails[i].AttendanceStatusDetails.length
                            var TotalDays = item.AttendanceDetails.length;

                            var PresentDays = item.AttendanceDetails.filter(function (subitem) {
                                if (subitem.AttendanceStatusKey == "1") {
                                    return true;
                                }
                            }).length;
                            var AbsentDays = item.AttendanceDetails.filter(function (subitem) {
                                if (subitem.AttendanceStatusKey == "2") {
                                    return true;
                                }
                            }).length;
                            var LeaveDays = item.AttendanceDetails.filter(function (subitem) {
                                if (subitem.AttendanceStatusKey == "3") {
                                    return true;
                                }
                            }).length;
                            var OffDays = item.AttendanceDetails.filter(function (subitem) {
                                if (subitem.AttendanceStatusKey == "4") {
                                    return true;
                                }
                            }).length;
                            var WeeklyOffDays = item.AttendanceDetails.filter(function (subitem) {
                                if (subitem.AttendanceStatusKey == "5") {
                                    return true;
                                }
                            }).length;
                            var Holydays = item.AttendanceDetails.filter(function (subitem) {
                                if (subitem.AttendanceStatusKey == "6") {
                                    return true;
                                }
                            }).length;
                            var HalfDays = item.AttendanceDetails.filter(function (subitem) {
                                if (subitem.AttendanceStatusKey == "7") {
                                    return true;
                                }
                            }).length;
                            var UnMarkedDays = item.AttendanceDetails.filter(function (subitem) {
                                if (subitem.AttendanceStatusKey == "0") {
                                    return true;
                                }
                            }).length;
                            //var PresentPercent = item.AttendanceDetails.reduce(function (sum, item) {
                            //    var Present = item.AttendanceStatusDetails.filter(function (subitem) {
                            //        return parseInt(subitem.AttendanceStatus);
                            //    }).reduce(function (subsum, subitem) {
                            //        return subsum + Multiplicant;
                            //    }, 0);
                            //    return sum + Present;
                            //}, 0);
                            //var PresentPercent = 0;
                            //var presentper = TotalDays ? (PresentPercent * 100 / TotalDays) : 0;
                            //presentper = parseFloat(presentper) ? parseFloat(presentper) : 0;
                            //presentper = AppCommon.RoundTo(presentper, Resources.RoundToDecimalPostion);
                            //item.PresentPercentage = presentper + "(" + PresentPercent + ")";

                            item.TotalDays = parseInt(TotalDays) ? parseInt(TotalDays) : 0;
                            item.PresentDays = parseInt(PresentDays) ? parseInt(PresentDays) : 0;
                            item.AbsentDays = parseInt(AbsentDays) ? parseInt(AbsentDays) : 0;
                            item.LeaveDays = parseInt(LeaveDays) ? parseInt(LeaveDays) : 0;
                            item.OffDays = parseInt(OffDays) ? parseInt(OffDays) : 0;
                            item.WeeklyOffDays = parseInt(WeeklyOffDays) ? parseInt(WeeklyOffDays) : 0;
                            item.Holydays = parseInt(Holydays) ? parseInt(Holydays) : 0;
                            item.HalfDays = parseInt(HalfDays) ? parseInt(HalfDays) : 0;
                            item.UnMarkedDays = parseInt(UnMarkedDays) ? parseInt(UnMarkedDays) : 0;
                        }

                    })
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnEmployeeAttendancePath").val(),
                        success: function (response) {

                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(resultData);
                            $('#dvAttendanceReport').html(html);
                            $("#tblAttendance").tableHeadFixer({ "left": 9 });
                            $(".btn-attendance").each(function () {
                                $(this).css("color", AppCommon.SetColorByBackgroundIntensity($(this).css("background-color")));
                            })
                        },
                        error: function (xhr) {

                        },
                        complete: function () {

                        }
                    })
                } else {
                    $('#dvAttendanceReport').mLoading("destroy");
                }
            }
        });



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

        if (cellValue) {
            if (cellValue.toUpperCase() == "P") {
                html = "<span style='color:green;font-weight:bold;font-size: 11px'>" + cellValue + "</span>"
            }
            else if (cellValue.toUpperCase() == "A") {
                html = "<span style='color:red;font-weight:bold;font-size: 11px'>" + cellValue + "</span>"
            } else {
                html = cellValue.replace("P", "<span style='color:green;font-weight:bold;font-size: 11px'>P</span>").replace("A", "<span style='color:red;font-weight:bold;font-size: 11px'>A</span>")
            }

        } else {
            html = "";
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
        GetAttendanceReport: getAttendanceReport,
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