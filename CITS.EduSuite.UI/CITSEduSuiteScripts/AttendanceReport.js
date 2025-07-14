request = null, removeCols = ["subgrid", "edit"], ApplicationJsonData = [];
var AttendanceReport = (function () {


    var getAttendanceReport = function () {

        var JsonData = $("form").serializeToJSON({

        });
        //JsonData["DateAdded"] = moment("01 " + $("#DateAdded").val(), "DD MMM YYYY").format("DD/MM/YYYY"); // Commented By khaleefa on 07-Dec-2022
        JsonData["DateAddedFrom"] = $("#DateAddedFrom").val();
        JsonData["DateAddedTo"] = $("#DateAddedTo").val();
        JsonData["ApplicationKey"] = 0;
        JsonData["ClassDetailsKey"] = 0;

        if (JsonData["BranchKey"] == "") {
            JsonData["BranchKey"] = 0;
        }

        $('#dvAttendanceReport').html("");
        $('#dvAttendanceReport').mLoading();
        $.ajax({
            type: "POST",
            url: $("#hdnGetAttendanceReport").val() + "?no-cache=" + new Date().getTime(),
            data: JsonData,
            success: function (data) {
                var resultData = {};
                var xmlDoc = $.parseXML(data);
                data = AppCommon.Xml2Json(xmlDoc)
                resultData = data.Attendance;
                if (resultData) {
                    resultData.FullPath = Resources.FullPath;
                    resultData.ApplicationUrl = Resources.ApplicationUrl.replace("~", Resources.FullPath);

                    resultData.AttendanceHeads = resultData.Students[0].AttendanceDetails.map(function (item) {
                        var obj = {};
                        obj.DayNumber = moment(item.AttendanceDate).date();
                        obj.DayName = moment(item.AttendanceDate).format('ddd');
                        obj.MonthName = moment(item.AttendanceDate).format('MMM');
                        obj.Year = moment(item.AttendanceDate).format('YYYY');
                        return obj;
                    });
                    var Multiplicant = 1;
                    $(resultData.Students).each(function (i, item) {
                        if (item.AttendanceDetails) {
                            // Multiplicant = 1 / item.AttendanceDetails[i].AttendanceStatusDetails.length
                            var TotalDays = item.AttendanceDetails.filter(function (subitem) {
                                return parseInt(subitem.AttendanceMasterKey);
                            }).length;
                            var PresentPercent = item.AttendanceDetails.reduce(function (sum, item) {
                                if (item.AttendanceStatusDetails)
                                    var Present = item.AttendanceStatusDetails.filter(function (subitem) {
                                        return parseInt(subitem.AttendanceStatus);
                                    }).reduce(function (subsum, subitem) {
                                        return subsum + Multiplicant;
                                    }, 0);
                                return sum + Present;
                            }, 0);
                            var presentper = TotalDays ? (PresentPercent * 100 / TotalDays) : 0;
                            presentper = parseFloat(presentper) ? parseFloat(presentper) : 0;
                            presentper = AppCommon.RoundTo(presentper, Resources.RoundToDecimalPostion);
                            //item.PresentPercentage = presentper + "(" + PresentPercent + ")";
                            item.PresentPercentage = PresentPercent;
                            var Presents = item.AttendanceDetails.reduce(function (sum, item) {
                                if (item.AttendanceStatusDetails)
                                    var Present = item.AttendanceStatusDetails.filter(function (subitem) {
                                        return parseInt(subitem.AttendanceStatusKey) == Resources.AttendanceStatusPresent;
                                    }).reduce(function (subsum, subitem) {
                                        return subsum + Multiplicant;
                                    }, 0);
                                return sum + Present;
                            }, 0);
                            var Absents = item.AttendanceDetails.reduce(function (sum, item) {
                                if (item.AttendanceStatusDetails)
                                    var Absent = item.AttendanceStatusDetails.filter(function (subitem) {
                                        return parseInt(subitem.AttendanceStatusKey) == Resources.AttendanceStatusAbsent;
                                    }).reduce(function (subsum, subitem) {
                                        return subsum + Multiplicant;
                                    }, 0);
                                return sum + Absent;
                            }, 0);
                            var Leaves = item.AttendanceDetails.reduce(function (sum, item) {
                                if (item.AttendanceStatusDetails)
                                    var Leave = item.AttendanceStatusDetails.filter(function (subitem) {
                                        return parseInt(subitem.AttendanceStatusKey) == Resources.AttendanceStatusLeave;
                                    }).reduce(function (subsum, subitem) {
                                        return subsum + Multiplicant;
                                    }, 0);
                                return sum + Leave;
                            }, 0);
                            var WeekOffs = item.AttendanceDetails.reduce(function (sum, item) {
                                if (item.AttendanceStatusDetails)
                                    var WeekOff = item.AttendanceStatusDetails.filter(function (subitem) {
                                        return parseInt(subitem.AttendanceStatusKey) == Resources.AttendanceStatusWeeklyOff;
                                    }).reduce(function (subsum, subitem) {
                                        return subsum + Multiplicant;
                                    }, 0);
                                return sum + WeekOff;
                            }, 0);
                            var HalfDays = item.AttendanceDetails.reduce(function (sum, item) {
                                if (item.AttendanceStatusDetails)
                                    var HalfDay = item.AttendanceStatusDetails.filter(function (subitem) {
                                        return parseInt(subitem.AttendanceStatusKey) == Resources.AttendanceStatusHalfday;
                                    }).reduce(function (subsum, subitem) {
                                        return subsum + Multiplicant;
                                    }, 0);
                                return sum + HalfDay;
                            }, 0);
                            var Lates = item.AttendanceDetails.reduce(function (sum, item) {
                                if (item.AttendanceStatusDetails)
                                    var Late = item.AttendanceStatusDetails.filter(function (subitem) {
                                        return parseInt(subitem.AttendanceStatusKey) == Resources.AttendanceStatusLate;
                                    }).reduce(function (subsum, subitem) {
                                        return subsum + Multiplicant;
                                    }, 0);
                                return sum + Late;
                            }, 0);

                            item.Presents = parseFloat(Presents) ? parseFloat(Presents) : 0;
                            item.Absents = parseFloat(Absents) ? parseFloat(Absents) : 0;
                            item.Leaves = parseFloat(Leaves) ? parseFloat(Leaves) : 0;
                            item.WeekOffs = parseFloat(WeekOffs) ? parseFloat(WeekOffs) : 0;
                            item.HalfDays = parseFloat(HalfDays) ? parseFloat(HalfDays) : 0;
                            item.Lates = parseFloat(Lates) ? parseFloat(Lates) : 0;
                        }

                    })
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnAttendancePath").val(),
                        success: function (response) {

                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(resultData);
                            $('#dvAttendanceReport').html(html);
                            $("#tblAttendance").tableHeadFixer({ "left": 1 });
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


    var getAttendanceReportConvert = function (json) {


        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAttendanceSummaryConvert").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            colNames: [Resources.RowKey, Resources.RowKey, Resources.AdmissionNo, Resources.Name,
            //Resources.MobileNo, Resources.Course,    Resources.AcademicTerm, Resources.Batch,
            Resources.Present, Resources.Absent, Resources.Present, Resources.Absent],
            colModel: [
                { key: true, hidden: true, name: 'ApplicationKey', index: 'ApplicationKey', editable: true },
                { key: false, hidden: true, name: 'WorkingDays', index: 'WorkingDays', editable: true },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentName', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'StudentMobile', index: 'StudentMobile', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 },
                //{ key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                //{ key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Present', index: 'Present', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: PrecentCalculations },
                { key: false, name: 'Absent', index: 'Absent', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: AbsentCalculations },
                { key: false, name: 'PresentCount1', index: 'PresentCount1', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: UPresentCalculations },
                { key: false, name: 'AbsentCount1', index: 'AbsentCount1', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: UAbsentCalculations },
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
            loadonce: false,
            ignoreCase: true,
            sortname: 'ApplicationKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            }
        });
        jQuery("#grid").jqGrid('destroyGroupHeader');
        $("#grid").jqGrid('setGroupHeaders', {
            useColSpanStyle: false,
            groupHeaders: [
                { startColumnName: "Present", numberOfColumns: 2, titleText: 'Current Days' },
                { startColumnName: "PresentCount1", numberOfColumns: 2, titleText: 'Affiliations / Tie-Ups Days' }
            ]
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }


    function PrecentCalculations(cellValue, options, rowdata, action) {

        var Pcount = rowdata.Present;
        Pcount = parseFloat(Pcount) ? parseFloat(Pcount) : 0;
        var Acount = rowdata.Absent;
        Acount = parseFloat(Acount) ? parseFloat(Acount) : 0;
        var TotalCount = 0;
        var totalWork = $("#UniversityDays").val();
        totalWork = parseFloat(totalWork) ? parseFloat(totalWork) : 0;
        TotalCount = Pcount + Acount;
        var PPercent = parseFloat((Pcount * 100 / (TotalCount != 0 ? TotalCount : 1)).toFixed(2)).toString();
        var APercent = parseFloat((Acount * 100 / (TotalCount != 0 ? TotalCount : 1)).toFixed(2)).toString();
        var PCount1 = parseFloat((totalWork * PPercent / 100).toFixed(2)).toString();
        var ACount1 = parseFloat((totalWork * APercent / 100).toFixed(2)).toString();
        //$(cells[3]).html(parseFloat(Pcount.toFixed(2)).toString() + " (" + PPercent + " %)");

        //$(cells[4]).html(parseFloat(Acount.toFixed(2)).toString() + " (" + APercent + " %)");
        //if (totalWork != "" && totalWork != 0) {
        //    $(cells[5]).html(parseFloat((Math.round(PCount1 * 2) / 2).toFixed(1)).toString() + " (" + PPercent + " %)");
        //    $(cells[6]).html(parseFloat((Math.round(ACount1 * 2) / 2).toFixed(1)).toString() + " (" + APercent + " %)");
        //}


        return "<span style=color:black>" + parseFloat(Pcount.toFixed(2)).toString() + " (" + PPercent + " %)" + "</span>"

    }
    function AbsentCalculations(cellValue, options, rowdata, action) {

        var Pcount = rowdata.Present;
        Pcount = parseFloat(Pcount) ? parseFloat(Pcount) : 0;
        var Acount = rowdata.Absent;
        Acount = parseFloat(Acount) ? parseFloat(Acount) : 0;
        var TotalCount = 0;
        var totalWork = $("#UniversityDays").val();
        totalWork = parseFloat(totalWork) ? parseFloat(totalWork) : 0;
        TotalCount = Pcount + Acount;
        var PPercent = parseFloat((Pcount * 100 / (TotalCount != 0 ? TotalCount : 1)).toFixed(2)).toString();
        var APercent = parseFloat((Acount * 100 / (TotalCount != 0 ? TotalCount : 1)).toFixed(2)).toString();
        var PCount1 = parseFloat((totalWork * PPercent / 100).toFixed(2)).toString();
        var ACount1 = parseFloat((totalWork * APercent / 100).toFixed(2)).toString();

        return "<span style=color:black>" + parseFloat(Acount.toFixed(2)).toString() + " (" + APercent + " %)" + "</span>"

    }
    function UPresentCalculations(cellValue, options, rowdata, action) {

        var Pcount = rowdata.Present;
        Pcount = parseFloat(Pcount) ? parseFloat(Pcount) : 0;
        var Acount = rowdata.Absent;
        Acount = parseFloat(Acount) ? parseFloat(Acount) : 0;
        var TotalCount = 0;
        var totalWork = $("#UniversityDays").val();
        totalWork = parseFloat(totalWork) ? parseFloat(totalWork) : 0;
        TotalCount = Pcount + Acount;
        var PPercent = parseFloat((Pcount * 100 / (TotalCount != 0 ? TotalCount : 1)).toFixed(2)).toString();
        var APercent = parseFloat((Acount * 100 / (TotalCount != 0 ? TotalCount : 1)).toFixed(2)).toString();
        var PCount1 = parseFloat((totalWork * PPercent / 100).toFixed(2)).toString();
        var ACount1 = parseFloat((totalWork * APercent / 100).toFixed(2)).toString();
        if (totalWork != "" && totalWork != 0) {
            return "<span style=color:black>" + parseFloat((Math.round(PCount1 * 2) / 2).toFixed(1)).toString() + " (" + PPercent + " %)" + "</span>"
        }
        else {
            return "";
        }



    }
    function UAbsentCalculations(cellValue, options, rowdata, action) {

        var Pcount = rowdata.Present;
        Pcount = parseFloat(Pcount) ? parseFloat(Pcount) : 0;
        var Acount = rowdata.Absent;
        Acount = parseFloat(Acount) ? parseFloat(Acount) : 0;
        var TotalCount = 0;
        var totalWork = $("#UniversityDays").val();
        totalWork = parseFloat(totalWork) ? parseFloat(totalWork) : 0;
        TotalCount = Pcount + Acount;
        var PPercent = parseFloat((Pcount * 100 / (TotalCount != 0 ? TotalCount : 1)).toFixed(2)).toString();
        var APercent = parseFloat((Acount * 100 / (TotalCount != 0 ? TotalCount : 1)).toFixed(2)).toString();
        var PCount1 = parseFloat((totalWork * PPercent / 100).toFixed(2)).toString();
        var ACount1 = parseFloat((totalWork * APercent / 100).toFixed(2)).toString();

        if (totalWork != "" && totalWork != 0) {
            return "<span style=color:black>" + parseFloat((Math.round(ACount1 * 2) / 2).toFixed(1)).toString() + " (" + APercent + " %)" + "</span>"
        }
        else {
            return "";
        }
    }




    var getApplicationAttendance = function () {

        var JsonData = $("form").serializeToJSON({

        });
        //JsonData["DateAdded"] = moment("01 " + $("#DateAdded").val(), "DD MMM YYYY").format("DD/MM/YYYY"); // Commented By khaleefa on 07-Dec-2022
        JsonData["DateAddedFrom"] = $("#DateAddedFrom").val();
        JsonData["DateAddedTo"] = $("#DateAddedTo").val();
        JsonData["ApplicationKey"] = $("#ApplicationKey").val();
        JsonData["ClassDetailsKey"] = 0;

        if (JsonData["BranchKey"] == "") {
            JsonData["BranchKey"] = 0;
        }

        $('#dvAttendanceReport').html("");
        $('#dvAttendanceReport').mLoading();
        $.ajax({
            type: "POST",
            url: $("#hdnGetAttendanceReport").val() + "?no-cache=" + new Date().getTime(),
            data: JsonData,
            success: function (data) {
                var resultData = {};
                var xmlDoc = $.parseXML(data);
                data = AppCommon.Xml2Json(xmlDoc)
                resultData = data.Attendance;
                if (resultData) {
                    resultData.FullPath = Resources.FullPath;
                    resultData.ApplicationUrl = Resources.ApplicationUrl.replace("~", Resources.FullPath);

                    resultData.AttendanceHeads = resultData.Students[0].AttendanceDetails.map(function (item) {
                        var obj = {};
                        obj.DayNumber = moment(item.AttendanceDate).date();
                        obj.DayName = moment(item.AttendanceDate).format('ddd');
                        obj.MonthName = moment(item.AttendanceDate).format('MMM');
                        obj.Year = moment(item.AttendanceDate).format('YYYY');
                        return obj;
                    });
                    var Multiplicant = 1;
                    $(resultData.Students).each(function (i, item) {
                        if (item.AttendanceDetails) {
                            // Multiplicant = 1 / item.AttendanceDetails[i].AttendanceStatusDetails.length
                            var TotalDays = item.AttendanceDetails.filter(function (subitem) {
                                return parseInt(subitem.AttendanceMasterKey);
                            }).length;
                            var PresentPercent = item.AttendanceDetails.reduce(function (sum, item) {
                                var Present = item.AttendanceStatusDetails.filter(function (subitem) {
                                    return parseInt(subitem.AttendanceStatus);
                                }).reduce(function (subsum, subitem) {
                                    return subsum + Multiplicant;
                                }, 0);
                                return sum + Present;
                            }, 0);
                            var presentper = TotalDays ? (PresentPercent * 100 / TotalDays) : 0;
                            presentper = parseFloat(presentper) ? parseFloat(presentper) : 0;
                            presentper = AppCommon.RoundTo(presentper, Resources.RoundToDecimalPostion);
                            item.PresentPercentage = presentper + "(" + PresentPercent + ")";
                        }

                    })
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnAttendancePath").val(),
                        success: function (response) {

                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(resultData);
                            $('#dvAttendanceReport').html(html);
                            $("#tblAttendance").tableHeadFixer({ "left": 2 });
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

    return {
        GetAttendanceReport: getAttendanceReport,
        GetAttendanceReportConvert: getAttendanceReportConvert,
        GetApplicationAttendance: getApplicationAttendance
    }

}());
