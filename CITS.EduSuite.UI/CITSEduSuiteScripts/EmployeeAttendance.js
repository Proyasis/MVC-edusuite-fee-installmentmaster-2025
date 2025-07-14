var jsonData = [], AttendanceStatusList = [];
var AttendanceGrid = $("#grid"), QuickAttendanceGrid = $("#gridQuick");
var ajaxRequest = null;
var ColumnNamesList = [
    { name: "EmployeeCode", headertext: "Staff Code", dummyData: "1" },
    { name: "AttendanceDate", headertext: "AttendanceDate", dummyData: '01/01/2019' },
    { name: "AttendanceStatus", headertext: "Attendance Status", dummyData: "P" },
    //{ name: "InDateTime", headertext: "InTime", dummyData: '01/01/2019 09:00 am' },
    //{ name: "OutDateTime", headertext: "OutTime", dummyData: '01/01/2019 05:30 pm' },
];
var EmployeeAttendance = (function () {
    //Employee Attendance
    var getEmployeeAttendances = function (json) {
        $(".section-content").mLoading();
        jsonData = json;
        $(AttendanceGrid).jqGrid('setGridParam', { datatype: 'json', page: 1 }).trigger('reloadGrid');
        //$(AttendanceGrid).jqGrid({
        //    url: $("#hdnGetEmployeeAttendanceList").val(),
        //    datatype: 'json',
        //    mtype: 'Get',
        //    postData: {%
        //        employeeId: function () {
        //            return $('#ddlEmployee').val()
        //        },
        //        branchId: function () {
        //            return $('#ddlBranch').val()
        //        }
        //    },
        //    // colNames: [Resources.RowKey, Resources.AddressType, Resources.Address, Resources.City, Resources.Country, Resources.Province, Resources.District, Resources.PostalCode, Resources.PhoneNumber, Resources.MobileNumber, Resources.EmailAddress, Resources.Action],
        //    colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Employee, Resources.AttendanceDate, Resources.InTime, Resources.OutTime, Resources.Duration, Resources.LeaveStatusName, Resources.Status, Resources.Remarks, Resources.Action],
        //    colModel: [
        //        { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
        //         { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
        //        { key: false, hidden: true, name: 'EmployeeKey', index: 'EmployeeKey', editable: true },
        //        { key: false, hidden: true, name: 'AttendanceStatusKey', index: 'AttendanceStatusKey', editable: true },
        //        { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
        //        { key: false, name: 'AttendanceDate', index: 'AttendanceDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
        //        { key: false, name: 'InTime', index: 'InTime', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatTime, sorttype: 'date' },
        //        { key: false, name: 'OutTime', index: 'OutTime', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatTime, sorttype: 'date' },
        //        { key: false, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatTimeDifference },
        //        { key: false, name: 'ClockInStatus', index: 'ClockInStatus', editable: true, cellEdit: true, sortable: true, formatter: formatClockStatus, resizable: false },
        //        { key: false, name: 'AttendanceStatusName', index: 'AttendanceStatusName', formatter: formatStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
        //        { key: false, name: 'Remarks', index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
        //        { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 150 },


        //    ],
        //    pager: jQuery('#pager'),
        //    rowNum: 10,
        //    rowList: [5, 10, 15, 20],
        //    autowidth: true,
        //    height: '100%',
        //    viewrecords: true,
        //    emptyrecords: Resources.NoRecordsToDisplay,
        //    jsonReader: {
        //        root: "rows",
        //        page: "page",
        //        total: "total",
        //        records: "records",
        //        repeatitems: false,
        //        Id: "0"
        //    },
        //    multiselect: false,
        //    loadonce: true,
        //    ignoreCase: true,
        //    altRows: true,
        //    altclass: 'jqgrid-altrow',
        //    loadComplete: function (data) {
        //        $(data.rows).each(function (i) {
        //            var _this = $("#grid tr[id='" + this.RowKey + "'] a[data-modal='']");
        //            if (data.rows.length == 1 && $("#grid tr").length > data.rows.length) {
        //                _this = $("#grid tr:last a[data-modal='']");
        //            }
        //            EmployeeAttendance.EditGridPopup($(_this));
        //            var obj = {};
        //            obj.BranchKey = this.BranchKey;
        //            obj.EmployeeKey = this.EmployeeKey
        //            EmployeeAttendance.SetUrlParam($(_this), obj);
        //        })
        //    }
        //})

        $(AttendanceGrid).jqGrid({
            url: $("#hdnGetEmployeeAttendanceList").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                EmployeeKey: function () {
                    return $('#EmployeeKey').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                FromDate: function () {
                    return $('#SearchFromDate').val()
                }
                ,
                ToDate: function () {
                    return $('#SearchToDate').val()
                },
                AttendanceStatusKey: function () {
                    return $('#AttendanceStatusKey').val()
                }
            },
            // colNames: [Resources.RowKey, Resources.AddressType, Resources.Address, Resources.City, Resources.Country, Resources.Province, Resources.District, Resources.PostalCode, Resources.PhoneNumber, Resources.MobileNumber, Resources.EmailAddress, Resources.Action],
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Employee, Resources.AttendanceDate, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, hidden: true, name: 'EmployeeKey', index: 'EmployeeKey', editable: true },
                { key: false, hidden: true, name: 'AttendanceStatusColor', index: 'AttendanceStatusColor', editable: true },
                { key: false, hidden: true, name: 'AttendanceStatusKey', index: 'AttendanceStatusKey', editable: true },
                { key: false, hidden: true, name: 'ApprovalStatusKey', index: 'ApprovalStatusKey', editable: true },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AttendanceDate', index: 'AttendanceDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'AttendanceStatusName', index: 'AttendanceStatusName', editable: true, cellEdit: true, sortable: true, formatter: formatAttendanceStatus, resizable: false },
                //{ key: false, name: 'ApprovalStatusName', index: 'ApprovalStatusName', formatter: formatStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'Remarks', index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 150 },


            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 50],
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
            multiselect: true,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 50,
            subGridRowExpanded: function (parentRowID, parentRowKey) {
                showChildGrid(parentRowID, parentRowKey, AttendanceGrid)
            },
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $(AttendanceGrid).getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $(AttendanceGrid).collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },
        })


        $(AttendanceGrid).jqGrid("setLabel", "FullName", "", "thFullName");
        $(AttendanceGrid).sortableRows();
        $(AttendanceGrid).jqGrid('gridDnD');
        $(".section-content").mLoading("destroy");

    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        var obj = {};
        obj.id = rowdata.RowKey;
        obj.employeeKey = rowdata.EmployeeKey ? rowdata.EmployeeKey : 0;
        obj.branchKey = rowdata.BranchKey ? rowdata.BranchKey : 0;
        return '<button type="button" class="btn btn-primary btn-sm mx-1" onclick="EmployeeAttendance.EditPopup(this)" data-href="AddEditEmployeeAttendance/' + "?" + $.param(obj) + '" ><i class="fa fa-pencil" aria-hidden="true"></i></a>'
            + '<button type="button" class="btn btn-danger btn-sm mx-1" onclick="javascript:deleteEmployeeAttendance(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
        // return '<div class="dropdown"><button class="btn btn-default dropdown-toggle btn-sm" type="button" data-toggle="dropdown">Action<span class="caret"></span></button><ul class="dropdown-menu"><li><a data-modal="" onclick="EmployeeAttendance.EditPopup(this)"  data-href="/EmployeeAttendance/AddEditEmployeeAttendance/' + rowdata.RowKey + '?employeeKey=' + rowdata.EmployeeKey + '&DepartmentKey=' + $("#ddlDepartments").val() + '&BranchKey=' + $("#ddlBranch").val() + '" ><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a></li><li><a onclick="javascript:deleteEmployeeAttendance(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a></li></ul></div>'
    }
    function formatStatus(cellValue, options, rowdata, action) {
        var html = "";
        switch (rowdata.ApprovalStatusKey) {
            case Resources.ProcessStatusApproved:
                html = '<span class="label label-success">' + cellValue + '</span>';
                break;
            case Resources.ProcessStatusRejected:
                html = '<span class="label label-danger">' + cellValue + '</span>';
                break;
            default:
                html = '<span class="label label-warning">' + cellValue + '</span>';

        }
        return html;
    }

    function formatAttendanceStatus(cellValue, options, rowdata, action) {
        var html = "";
        html = '<span class="label" style="background:' + rowdata.AttendanceStatusColor + ';color:' + checkColorStrength(rowdata.AttendanceStatusColor) + '">' + cellValue + '</span>';
        return html;
    }


    var editPopup = function (_this) {

        var obj = {};
        obj.employeeKey = $("#EmployeeKey").val()
        var url = $(_this).attr("data-href");

        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {
                setTimeout(function () {
                    AttendancePopupLoad()
                }, 500);
            },
            rebind: function (result) {
                $(AttendanceGrid).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, url);
    }

    var hideShowInOutByConfigType = function (value) {
        if (value == Resources.AttendanceConfigTypeMarkPresent) {
            $(".dvInOut").hide();
        }
        else {
            $("#dvLeaveType").show();
        }

    }

    //Quick Attendance
    var getQuickEmployeeAttendances = function (json) {
        $(".section-content").mLoading();
        jsonData = json;
        $(QuickAttendanceGrid).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $(QuickAttendanceGrid).jqGrid({
            url: $("#hdnGetQuickEmployeeAttendance").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                BranchId: function () {
                    return ($('#BranchKey').val() != "" ? $('#BranchKey').val() : 0)
                },
                AttendanceDate: function () {
                    return $('#AttendanceDate').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace,
                Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Employee, Resources.InTime, Resources.OutTime, Resources.Duration, Resources.Action],
            colModel: [
                { key: false, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'EmployeeKey', index: 'EmployeeKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, hidden: true, name: 'AttendancePresentStatusKey', index: 'AttendancePresentStatusKey', editable: true },
                { key: false, hidden: true, name: 'AttendanceConfigType', index: 'AttendanceConfigType', editable: true },
                { key: false, hidden: true, name: 'AttendanceDate', index: 'AttendanceDate', editable: true },
                { key: false, hidden: true, name: 'AttendanceStatusKey', index: 'AttendanceStatusKey', editable: true },
                { key: false, hidden: true, name: 'AttendanceStatusColor', index: 'AttendanceStatusColor', editable: true },
                { key: false, hidden: true, name: 'AttendanceStatusName', index: 'AttendanceStatusName', editable: true, formatter: formatAttendanceStatus },
                { key: false, hidden: true, name: 'AttendanceStatusRemarks', index: 'AttendanceStatusRemarks', editable: true },
                { key: false, hidden: true, name: 'ClockInStatus', index: 'ClockInStatus', editable: true, cellEdit: true, sortable: true, formatter: formatClockStatus, resizable: false },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'InDateTime', index: 'InDateTime', formatter: formatTimeColumn, editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'OutDateTime', index: 'OutDateTime', formatter: formatTimeColumn, editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, formatter: formatTimeDifference, editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: formatClockButton, resizable: false, width: 250 },
            ],
            pager: jQuery('#pagerQuick'),
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
            altclass: 'jqgrid-altrow',

            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 50,
            subGridRowExpanded: function (parentRowID, parentRowKey) {
                showChildGrid(parentRowID, parentRowKey, QuickAttendanceGrid)
            },
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $(AttendanceGrid).getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $(AttendanceGrid).collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },
        })

        $(QuickAttendanceGrid).jqGrid("setLabel", "FullName", "", "thFullName");
        $(".section-content").mLoading("destroy");
    }
    function showChildGrid(parentRowID, parentRowKey, parentGrid) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)
        var rowData = $(parentGrid).jqGrid('getRowData', parentRowKey);

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetEmployeesAttendanceLog").val() + "/" + rowData.RowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: Resources.Status, name: 'AttendancePresentStatusKey', formatter: formatAttendancePresentStatusColumn, width: 100 },
                { label: Resources.Time, name: 'InDateTime', formatter: 'date', formatoptions: { newformat: 'd/m/Y h:i:s A' }, width: 200 },
            ],
            loadonce: true,
            width: 500,
            autowidth: false,
            height: '100%',
            pager: false,
            footer: false,
        });

    }

    function formatTimeColumn(cellValue, options, rowdata, action) {
        var columnName = options.colModel["name"];
        var string = options.gid + "_" + columnName;
        setTimeout(function () {
            if (columnName == "InDateTime") {
                $("td[aria-describedby=" + string + "]").addClass("w3-text-green")
            }
            else {
                $("td[aria-describedby=" + string + "]").addClass("w3-text-red")
            }
        }, 100)
        if (cellValue != null) {
            //return moment(cellValue).format("hh:mm A");
            return moment(cellValue).format("DD/MM/YYYY hh:mm A");


        }
        else {
            if (rowdata.AttendanceConfigType == Resources.AttendanceConfigTypeMarkPresent) {
                return "";
            } else if ((columnName == "InDateTime" && !rowdata.AttendancePresentStatusKey)) {
                setTimeout(function () { $("td[aria-describedby=" + string + "]", $("tr[id=" + options.rowId + "]")).attr("data-digital-clock-date", "") }, 100);
                return "";
            }
            else if (columnName == "OutDateTime" && rowdata.AttendanceStatusKey != Resources.AttendanceStatusAbsent && rowdata.AttendancePresentStatusKey != Resources.AttendancePresentStatusCheckOut && rowdata.AttendancePresentStatusKey) {
                setTimeout(function () { $("td[aria-describedby=" + string + "]", $("tr[id=" + options.rowId + "]")).attr("data-digital-clock-date", "") }, 100);
                return "";

            }
            else if (cellValue) {
                return moment(cellValue).format("DD/MM/YYYY hh:mm:ss A");
            }
        }


        return "";

    }
    function formatAttendancePresentStatusColumn(cellValue, options, rowdata, action) {
        if (rowdata.AttendancePresentStatusKey == Resources.AttendancePresentStatusBreakIn) {

            return "<span style=color:green>Break In</span>"

        }
        else if (rowdata.AttendancePresentStatusKey == Resources.AttendancePresentStatusBreakOut) {

            return "<span style=color:red>Break Out</span>"
        }
        else if (rowdata.AttendancePresentStatusKey == Resources.AttendancePresentStatusCheckIn) {
            return "<span style=color:green>Check In</span>"

        }
        else if (rowdata.AttendancePresentStatusKey == Resources.AttendancePresentStatusCheckOut) {

            return "<span style=color:red>Check Out</span>"
        }

    }
    function formatClockButton(cellValue, options, rowdata, action) { // Comented on 20-Oct-2022
        var temp = options.rowId;
        var tempdelete = "'" + rowdata.RowKey + "'";
        var obj = [];
        obj.temp = temp;
        obj.AttendenceStatusKey = options.AttendenceStatusKey;
        var html = '';
        if (rowdata.RowKey == 0) {
            if (rowdata.AttendanceConfigType == Resources.AttendanceConfigTypeMarkPresent) {
                html = html + "<button type='button' class='btn btn-success btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusCheckIn + ")'><i class='fa fa-sign-in'></i> Present</button>"
                html = html + "<button type='button' class='btn btn-warning btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusHalfday + ',' + Resources.AttendancePresentStatusCheckIn + ")'><i class='fa fa-sign-in'></i> Half Day</button>"

            }
            else {
                html = html + "<button type='button' class='btn btn-success btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusCheckIn + ")'><i class='fa fa-sign-in'></i> Check In</button>"

            }
            html = html + "<button type='button' class='btn btn-danger btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusAbsent + ',' + Resources.AttendancePresentStatusCheckIn + ")'><i class='fa fa-sign-in'></i> Absent</button>"

            if (rowdata.AttendanceStatusKey == Resources.AttendanceStatusLeave) {
                html = html + '<span class="label" style="padding:4px 8px 4px 6px;font-size: 11px;background:#ffa500;color:' + checkColorStrength('#ffa500') + '">' + "Today" + "Leave" + '</span>';
            }
            else if (rowdata.AttendanceStatusKey == Resources.AttendanceStatusOff) {
                html = html + '<span class="label" style="padding:4px 8px 4px 6px;font-size: 11px;background:#ffff00;color:' + checkColorStrength('#ffff00') + '">' + rowdata.AttendanceStatusRemarks + '</span>';
            }
            else if (rowdata.AttendanceStatusKey == Resources.AttendanceStatusWeeklyOff) {
                html = html + '<span class="label" style="padding:4px 8px 4px 6px;font-size: 11px;background:#ffff00;color:' + checkColorStrength('#ffff00') + '">' + rowdata.AttendanceStatusRemarks + '</span>';
            }
            else if (rowdata.AttendanceStatusKey == Resources.AttendanceStatusHoliday) {
                html = html + '<span class="label" style="padding:4px 8px 4px 6px;font-size: 11px;background:#0000ff;color:' + checkColorStrength('#0000ff') + '">' + rowdata.AttendanceStatusRemarks + '</span>';
            }

            //+ "<a type='button' class='btn btn-warning btn-sm' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusLeave + ',' + "" + ")'> Leave</a>"
            //+ "<a type='button' class='btn btn-info btn-sm' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusOff + ',' + "" + ")'> Off</a>"
            // + "<a type='button' class='btn btn-primary btn-sm' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusHoliday + ',' + "" + ")'> Holiday</a>";


        }
        else {

            if (rowdata.AttendancePresentStatusKey != Resources.AttendancePresentStatusCheckOut && rowdata.AttendanceConfigType != Resources.AttendanceConfigTypeMarkPresent) {
                if ((rowdata.AttendanceStatusKey == Resources.AttendanceStatusPresent && rowdata.AttendancePresentStatusKey == Resources.AttendancePresentStatusCheckIn) || (rowdata.AttendanceStatusKey == Resources.AttendanceStatusPresent && rowdata.AttendancePresentStatusKey == Resources.AttendancePresentStatusBreakIn)) {
                    html = html + "<button type='button' class='btn btn-danger btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusCheckOut + ")'><i class='fa fa-sign-out'></i> Check Out</button>"
                }
                else if (rowdata.AttendancePresentStatusKey == Resources.AttendancePresentStatusBreakOut && rowdata.AttendanceStatusKey == Resources.AttendanceStatusPresent) {
                    html = html + "<button type='button' class='btn btn-success btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusBreakIn + ")'><i class='fa fa-sign-out'></i> Break In</button>"
                }
                else {
                    html = html + '<span class="label" style="background:' + rowdata.AttendanceStatusColor + ';color:' + checkColorStrength(rowdata.AttendanceStatusColor) + '">' + rowdata.AttendanceStatusName + '</span>';

                }
                if (rowdata.AttendanceConfigType != Resources.AttendanceConfigTypeInOut && rowdata.AttendancePresentStatusKey != Resources.AttendancePresentStatusBreakOut && rowdata.AttendanceStatusKey != Resources.AttendanceStatusAbsent) {
                    html = html + "<button type='button' class='btn btn-danger btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusBreakOut + ")'><i class='fa fa-sign-in'></i> Break Out</button>"
                }
                // html = rowdata.ClockInStatus == true ? html + "<a type='button' class='btn btn-danger btn-sm' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusBreakIn + ")'><i class='fa fa-sign-out'></i> Clock Out</a>"
                //: html + "<a type='button' class='btn btn-success btn-sm' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusBreakIn + ")'><i class='fa fa-sign-in'></i> Clock In</a>";
            }
            else {
                html = html + '<span class="label" style="padding:4px 8px 4px 6px;font-size: 11px; background:' + rowdata.AttendanceStatusColor + ';color:' + checkColorStrength(rowdata.AttendanceStatusColor) + '">' + rowdata.AttendanceStatusName + '</span>';

            }
            html = html + '<button type="button" class="btn btn-danger btn-sm mx-1" onclick="javascript:deleteEmployeeAttendance(' + tempdelete + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';

        }
        return html

    }

    //function formatClockButton(cellValue, options, rowdata, action) {

    //    var temp = options.rowId;
    //    var obj = [];
    //    obj.temp = temp;
    //    obj.AttendenceStatusKey = options.AttendenceStatusKey;
    //    var html = '<div class="divEditDelete">';
    //    if (rowdata.RowKey == 0) {

    //        //html = html + "<a type='button' class='btn btn-success btn-sm' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusCheckIn + ")'><i class='fa fa-sign-in'></i> Present</a>"
    //        html = html + "<a  class='btn btn-success btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + "," + Resources.AttendanceStatusPresent + ")'><i class='fa fa-sign-in'></i> Present</a>" + "<a  class='btn btn-danger btn-sm mx-1' onclick='javascript:UpdateQuickAttendance(" + temp + "," + Resources.AttendanceStatusAbsent + ")'><i class='fa fa-sign-out'></i> Absent</a>";
    //    }
    //    else {

    //        if (rowdata.RowKey != 0 && rowdata.RowKey != null) {
    //            if (rowdata.AttendanceStatusKey == Resources.AttendanceStatusPresent) {

    //                html = html + '<span class="label mx-1" style="padding:4px 8px 4px 6px;font-size: 11px;background:' + rowdata.AttendanceStatusColor + ';color:' + checkColorStrength(rowdata.AttendanceStatusColor) + '">' + rowdata.EmployeeName + Resources.BlankSpace + "is" + Resources.BlankSpace + rowdata.AttendanceStatusName + '</span>' + '<a class="btn btn-danger btn-sm" onclick="javascript:deleteEmployeeAttendance(' + rowdata.RowKey + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';

    //            }
    //            else {
    //                html = html + '<span class="label mx-1" style="padding:4px 8px 4px 6px;font-size: 11px; background:' + rowdata.AttendanceStatusColor + ';color:' + checkColorStrength(rowdata.AttendanceStatusColor) + '">' + rowdata.EmployeeName + Resources.BlankSpace + "is" + Resources.BlankSpace + rowdata.AttendanceStatusName + '</span>' + '<a class="btn btn-danger btn-sm mx-1" onclick="javascript:deleteEmployeeAttendance(' + rowdata.RowKey + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';



    //            }
    //        }

    //    }
    //    html = html + '</div">';
    //    return html

    //}



    //Attendance Sheet
    var getAttendanceDetail = function (json) {
        jsonData = json;
        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    var item = $(this);
                    $("[id*=RowKey]", item).val(0)
                    AppCommon.FormatSelect2();
                    AppCommon.FormatInputCase();
                    AppCommon.FormatDateInput();
                    AppCommon.FormatTimeInput();
                },

                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        //deleteBankStatementDetailsItem($(hidden).val(), $(this));
                    }
                    else {
                        $(this).slideUp(remove);
                    }
                },

                rebind: function (response) {
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
                                        window.location.href = $("#hdnGetAttendanceSheet").val()// + "/" + response["RowKey"];
                                    }
                                }
                            }
                        })

                    }

                },

                data: json,
                repeatlist: '',
                submitButton: ''
            });

    }
    var getAttendanceSheet = function (json, realod, excelData) {
        $(".section-content").mLoading();
        var response = [];
        var obj = {};
        obj.id = $("#BranchKey").val();
        jsonData = json;
        setTimeout(function () {
            if (excelData) {
                response.rows = excelData;
                response.AttendanceStatuses = AttendanceStatusList;

            }
            else {
                response = AjaxHelper.ajax("POST", $("#hdnGetAttendanceSheet").val() + "?" + $.param(obj),
                    {
                        model: json
                    });
            }
            SheetJsonModel = $.extend(true, [], json);

            var colHeaders = [], colModels = [], groupParam = {};
            groupParam.Start = 0; groupParam.End = 0;


            $("#gridSheet").jqGrid('GridUnload', false);

            var month = AppCommon.ParseMMMYYYYDate($("input#AttendanceDate").val());
            var salaryMonth = month.getMonth() + 1;
            var salaryYear = month.getFullYear();
            var totalDays = (new Date(salaryYear, salaryMonth, 0)).getDate();
            AttendanceStatusList = response.AttendanceStatuses;
            var resultData = AttendanceSheetData(response.rows, response.AttendanceStatuses, totalDays, colModels, colHeaders, groupParam, salaryMonth, salaryYear);

            $("#gridSheet").jqGrid('setGridParam', { datatype: 'local', data: resultData }).trigger("reloadGrid");
            $("#gridSheet").jqGrid({
                datatype: 'local',
                pager: jQuery('#pagerSheet'),
                rowNum: 10,
                rowList: [5, 10, 15, 20],
                colNames: colHeaders,
                colModel: colModels,
                autowidth: true,
                height: '100%',
                viewrecords: true,
                emptyrecords: Resources.NoRecordsToDisplay,
                data: resultData,
                multiselect: true,
                loadonce: true,
                ignoreCase: true,
                altRows: true,
                edit: true,
                cellEdit: true,
                cellsubmit: 'clientArray',
                editurl: 'clientArray',
                altclass: 'jqgrid-altrow',
                loadComplete: function (data) {

                    $(data.rows).each(function () {
                        var id = this.EmployeeKey;
                        this.forEach(function (value, key) {
                            $(AttendanceStatusList).each(function () {
                                if (this.Text == value) {
                                    $("td[aria-describedby=gridSheet_" + key + "]", $("tr[id='" + id + "']")).css({ "background": this.GroupName, "color": checkColorStrength(this.GroupName) });
                                }
                            });
                        })
                    })
                }
            })
            $("#gridSheet").jqGrid('destroyGroupHeader');
            $("#gridSheet").jqGrid('setGroupHeaders', {
                useColSpanStyle: false,
                groupHeaders: [
                    { startColumnName: groupParam.Start, numberOfColumns: groupParam.End, titleText: 'Days' },
                ]
            });
            $("#gridSheet").jqGrid("setLabel", "FullName", "", "thFullName");
            $(".section-content").mLoading("destroy");
        }, 500)
    }

    var getAttendanceSheetInOut = function (json, realod, excelData) {
        $(".section-content").mLoading();
        var response = [];
        jsonData = json;
        setTimeout(function () {
            if (excelData) {
                response.rows = excelData;
                response.AttendanceStatuses = AttendanceStatusList;

            }
            else {
                response = AjaxHelper.ajax("POST", $("#hdnGetAttendanceSheet").val(),
                    {
                        model: json
                    });
            }
            SheetJsonModel = $.extend(true, [], json);

            var colHeaders = [], colModels = [], groupParam = {};
            groupParam.Start = 0; groupParam.End = 0;


            $("#gridSheet").jqGrid('GridUnload', false);

            var month = AppCommon.ParseMMMYYYYDate($("input#AttendanceDate").val());
            var salaryMonth = month.getMonth() + 1;
            var salaryYear = month.getFullYear();
            var totalDays = (new Date(salaryYear, salaryMonth, 0)).getDate();
            AttendanceStatusList = response.AttendanceStatuses;
            var resultData = AttendanceSheetData(response.rows, response.AttendanceStatuses, totalDays, colModels, colHeaders, groupParam, salaryMonth, salaryYear);

            $("#gridSheet").jqGrid('setGridParam', { datatype: 'local', data: resultData }).trigger("reloadGrid");
            $("#gridSheet").jqGrid({
                datatype: 'local',
                pager: jQuery('#pagerSheet'),
                rowNum: 10,
                rowList: [5, 10, 15, 20],
                colNames: colHeaders,
                colModel: colModels,
                autowidth: true,
                height: '100%',
                viewrecords: true,
                emptyrecords: Resources.NoRecordsToDisplay,
                data: resultData,
                multiselect: true,
                loadonce: true,
                ignoreCase: true,
                altRows: true,
                edit: true,
                cellEdit: true,
                cellsubmit: 'clientArray',
                editurl: 'clientArray',
                altclass: 'jqgrid-altrow',
                loadComplete: function (data) {

                    $(data.rows).each(function () {
                        var id = this.EmployeeKey;
                        this.forEach(function (value, key) {
                            $(AttendanceStatusList).each(function () {
                                if (this.Text == value) {
                                    $("td[aria-describedby=gridSheet_" + key + "]", $("tr[id='" + id + "']")).css({ "background": this.GroupName, "color": checkColorStrength(this.GroupName) });
                                }
                            });
                        })
                    })
                }
            })
            $("#gridSheet").jqGrid('destroyGroupHeader');
            $("#gridSheet").jqGrid('setGroupHeaders', {
                useColSpanStyle: false,
                groupHeaders: [
                    { startColumnName: groupParam.Start, numberOfColumns: groupParam.End, titleText: 'Days' },
                ]
            });
            $("#gridSheet").jqGrid("setLabel", "FullName", "", "thFullName");
            $(".section-content").mLoading("destroy");
        }, 500)
    }

    var downloadAttendanceSheet = function () {
        //var grid = $('#gridSheet');
        //var rowIDList = grid.getDataIDs();
        //var row = grid.jqGrid('getGridParam', 'data');;
        //var colNames = [], DataList = [];
        //colNames[0] = "StaffKey";
        //colNames[1] = "EmployeeName";
        //colNames[2] = "AttendanceDate";
        //colNames[3] = "AttendanceStatusKey";
        //colNames[4] = "InTime";
        //colNames[5] = "OutTime";

        //var i = 1, p = 2;
        ////Object.keys(row[0]).forEach(function (key, index) {
        ////    if (i == key) {
        ////        colNames[p] = key.toString();
        ////        p++;
        ////    }

        ////    // Capture Column Names
        ////    i++;
        ////});
        ////colNames[p] = "TotalDays";

        //DataList.push(colNames);
        //var html = "";
        //for (var j = 0; j < rowIDList.length; j++) {
        //    var coldata = [];
        //    row = grid.getRowData(rowIDList[j]);
        //    for (var i = 0 ; i < colNames.length ; i++) {
        //        coldata.push(row[colNames[i]].toString());
        //    }
        //    DataList.push(coldata);
        //}
        //var ws_name = "Attendace Sheet";
        //var wb = new Workbook(), ws = sheet_from_array_of_arrays(DataList);
        ///* add worksheet to workbook */
        //wb.SheetNames.push(ws_name);
        //wb.Sheets[ws_name] = ws;
        //var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary' })
        //saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), "Attendace Sheet.xlsx")

        var data = [];

        var obj = {}
        obj.FileName = "Attendace Sheet"
        obj.ColumnNames = ColumnNamesList;
        var header = [];
        $(obj.ColumnNames).each(function () {
            header.push(this.name)
        })
        data.push(header)
        var item = [];
        $(obj.ColumnNames).each(function () {
            item.push(this.dummyData)
        })
        data.push(item)

        var wb = new Workbook(), ws = sheet_from_array_of_arrays(data);
        /* add worksheet to workbook */
        wb.SheetNames.push(obj.FileName);
        wb.Sheets[obj.FileName] = ws;
        var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary', cellDates: true, dateNF: 'dd/mm/yyyy;@' })
        saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), obj.FileName + ".xlsx")
    }

    var handleFile = function (e) {

        $(".section-content").mLoading()
        var jsonObject = $.extend(true, [], jsonData);
        //Get the files from Upload control
        var files = e.target.files;
        var i, f;
        var result;
        var target = e.target;
        if (files.length == 0) {
            $(".section-content").mLoading("destroy");
        }
        for (i = 0, f = files[i]; i != files.length; ++i) {
            var reader = new FileReader();
            var name = f.name;
            reader.onload = function (e) {
                var data = e.target.result;
                var wb, arr;
                function doit() {
                    try {
                        wb = XLSX.read(data, { type: 'binary', cellDates: true, cellText: false, cellNF: false, dateNF: 'dd/mm/yyyy hh:mm:ss;@' });
                        var DetailJsonData = process_wb(wb)
                       
                        DetailJsonData = DetailJsonData.filter(function (item) {
                            if (item.length > 0) {
                                return item
                            }
                        });
                      
                        jsonObject = EmployeeAttendance.ChangeDropdownKey(jsonObject, DetailJsonData)
                        $.ajax({
                            url: $("#hdnReadExcel").val(),
                            type: "POST",
                            data: { modelList: jsonObject },
                            success: function (result) {
                                $("#dynamicRepeater").html(result)
                                $("[data-input-type =dateTime]").each(function () {

                                    if ($(this).val() != "") {
                                        $(this).val(moment($(this).val(), ["DD/MM/YYYY HH:mm:ss"]).format("DD/MM/YYYY hh:mm:ss A"));
                                    }
                                })
                                $(".section-content").mLoading("destroy");

                            },
                            error: function () {
                                $(".section-content").mLoading("destroy");
                            }
                        });

                    } catch (e) { console.log(e); }
                }

                if (e.target.result.length > 1e6) opts.errors.large(e.target.result.length, function (e) { if (e) doit(); });
                else { doit(); }
            };
            reader.readAsBinaryString(f);

        }
    }
    var changeDropdownKey = function (jsonObject, DetailJsonData) {
        var AttendanceStatuses = jsonObject[0]["AttendanceStatuses"]
        var Employees = jsonObject[0]["Employees"]
        var returnArray = [];
        //jsonObject["EmployeeAttendanceViewModel"].splice(1)
        $(ColumnNamesList).each(function (index) {
            var dummyData;
            var col = this;

            dummyData = col.dummyData;
            jsonObject[0][this.name] = dummyData;
        })
        var obj = jsonObject[0];

        $(DetailJsonData).each(function (i) {
            if (i > 0) {
                if (i > 1) {
                    obj = $.extend(true, {}, obj);

                }
                $(ColumnNamesList).each(function (j) {
                    var value = DetailJsonData[i][j];
                    if (j == 0) {

                        item = $(Employees).filter(function (n, p) {
                            return (value ? value : "").toLowerCase() === (p.GroupName ? p.GroupName : "").toLowerCase();
                        })[0];
                        if (item)
                            obj.EmployeeKey = item.RowKey;
                    }
                    else if (j == 2)
                    {
                         item1 = $(AttendanceStatuses).filter(function (n, p) {
                             return (value ? value : "").toLowerCase() === (p.Text ? p.Text : "").toLowerCase();
                        })[0];
                        if (item1)
                            obj.AttendanceStatusKey = item1.RowKey;
                    }
                    else {
                        obj[this.name] = value;
                    }
                });



                returnArray.push(obj);
            }
        });
        return returnArray;

    }

    function formSubmitMultipleData(data) {
        $(".section-content").mLoading();
        setTimeout(function () {
            $form = $("form");
            var result = ModifyMultipleModel(jsonData, data);

            var dataurl = $($form)[0].action;
            var response = AjaxHelper.ajax("POST", dataurl,
                {
                    modelList: result
                });
            if (response.IsSuccessful == true) {
                toastr.success(Resources.Success, response.Message);

                setAttendanceSheetParam(response);
                EmployeeAttendance.GetAttendanceSheet(response, true);


            }
            else {
                $("[data-valmsg-for=error_msg]").html(response.Message);
            }
            $(".section-content").mLoading("destroy");
        }, 1000);

    }

    var singleProcessClickEvent = function (id) {
        var Data = [], status = true;
        var rowData = $("#gridSheet").getRowData(id);

        Data.push(rowData);
        status = checkAttendanceStatus(rowData)

        if (status) {

            formSubmitMultipleData(Data)
        }
        else {
            EduSuite.AlertMessage({
                title: Resources.Warning,
                content: Resources.AttendanceSheetWarningMessage,
                type: 'orange'
            })
        }
    }

    var multipleProcessClickEvent = function (json) {
        var Data = [], status = true;
        var rowIds = $("#gridSheet").jqGrid("getGridParam", "selarrrow");

        rowIds.forEach(function (item) {
            var rowData = $("#gridSheet").getRowData(item);
            Data.push(rowData);
            if (!checkAttendanceStatus(rowData))
                status = false;
        })

        if (status) {
            formSubmitMultipleData(Data)

        }
        else {
            EduSuite.AlertMessage({
                title: Resources.Warning,
                content: Resources.AttendanceSheetWarningMessage,
                type: 'orange'
            });
            $(".section-content").mLoading('destroy')
        }

    }

    function checkAttendanceStatus(data) {
        var status = true;
        for (var key in data) {
            if (key && parseInt(key)) {
                var value = data[key];
                if (value == "") {
                    status = false;
                }
            }

        }
        return status;
    }

    //Common for Attendance
    function formatTime(cellValue, options, rowdata, action) {
        var columnName = options.colModel["name"];
        var string = options.gid + "_" + columnName;
        setTimeout(function () {
            if (columnName == "InDateTime") {
                $("td[aria-describedby=" + string + "]").addClass("w3-text-green")
            }
            else {
                $("td[aria-describedby=" + string + "]").addClass("w3-text-red")
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

    function formatClockStatus(cellValue, options, rowdata, action) {
        if (cellValue != null) {
            return cellValue == true ? "<span class='label label-success'>Clock In</span>" : "<span class='label label-danger'>Clock Out</span>";
        }
        else {
            return "";
        }
    }

    function formatTimeDifference(cellValue, options, rowdata, action) {
        var span = $("<span/>");
        $(span).addClass("text-blue");
        if (rowdata.InDateTime != null && rowdata.OutDateTime != null) {
            var startTime = moment(rowdata.InDateTime)
            var endTime = moment(rowdata.OutDateTime);
            var duration = moment.duration(endTime.diff(startTime));
            var hours = parseInt(duration.asHours());
            var minutes = parseInt(duration.asMinutes()) - hours * 60;
            var seconds = parseInt(duration.asSeconds()) - (hours * 3600) - (minutes * 60);
            $(span).html((hours + ' H  ' + minutes + ' M '));
            return $(span)[0].outerHTML;
        } else {
            return "";
        }
    }

    //Events
    var getDepartmentsById = function (Id) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetDepartments").val(), ddl, Resources.Department, "Departments");


    }

    var getEmployeeById = function (DepartmentKey, BranchKey) {
        var obj = {};
        obj.DepartmentKey = DepartmentKey;
        obj.BranchKey = BranchKey;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetEmployeeByDepartment_Branch").val(), ddl, Resources.Employee, "Employees");

    }

    var getEmployeesByBranchId = function (Id, ddl) {
        var obj = {};
        obj.id = Id;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetEmployeesByBranchId").val(), ddl, Resources.Employee);

    }

    var attendanceStatusChange = function (_this) {
        var value = $(_this).val();
        var item = $(_this).closest("[data-repeater-item]");
        if (_this[0].checked) {
            $("[data-toggle=buttons] label", item).css({ "background-color": "", "color": "#000" });
            var Backcolor = $(_this).closest("label").data("background");
            var Color = AppCommon.SetColorByBackgroundIntensity(Backcolor);
            $(_this).closest("label").css({ "background-color": Backcolor, "color": Color });
        }




        if (value == Resources.AttendanceStatusLeave) {
            $("#dvLeaveType", $(item)).show();
        }
        else {
            $("#dvLeaveType", $(item)).hide();
            $("select[name*=LeaveTypeKey]", $(item)).val("");
        }
        if (value == Resources.AttendanceStatusPresent || value == Resources.AttendanceStatusHalfday) {
            $(".dvInOut", $(item)).removeClass("invisible");

        } else {
            //$("input[name*=InTime]", $(item)).val("");
            //$("input[name*=OutTime]", $(item)).val("");
            $(".dvInOut", $(item)).addClass("invisible");
        }

    }

    var attendanceStatusChangeSingle = function (_this) {


        var value = $(_this).val();



        if (value == Resources.AttendanceStatusLeave) {
            $("#dvLeaveType").show();
        }
        else {
            $("#dvLeaveType").hide();
            $("select[name*=LeaveTypeKey]").val("");
        }
        if (value == Resources.AttendanceStatusPresent || value == Resources.AttendanceStatusHalfday) {
            $(".dvInOut").show();

        } else {

            //$("input[name*=InTime]").val("");
            //$("input[name*=OutTime]").val("");
            $(".dvInOut").hide();
        }

    }

    //Get Employee For Multiple added by Khaleefa on 09 Feb 2019

    var loadData = function (json) {
        var model = json;
        model.BranchKey = $("#BranchKey").val();
        model.AttendanceDate = $("#AttendanceDate").val();
        model.EmployeeKey = $("#EmployeeKey").val();

        $("#dvEmployeeList").mLoading();
        ajaxRequest = $.ajax({
            type: "POST",
            url: $("#hdnUrl").val(),
            data: model,
            beforeSend: function () {
                if (ajaxRequest != null) {
                    ajaxRequest.abort();
                }
            },
            success: function (result) {
                if (result.IsSuccessful == false) {
                    $("[data-valmsg-for=error_msg]").html(result.Message);
                }
                $("#dvEmployeeList").html(result);

            },
            error: function (request, status, error) {
                $("#dvEmployeeList").mLoading('destroy')
            }
        });
    }

    function formSubmit() {
        var form = $('form');

        var validate = $(form).valid();
        if (validate) {
            $(form).mLoading();
            var TimeKeys = ["InTime", "OutTime"]
            var obj = $(form).serializeToJSON({
                associativeArrays: false
            });

           

            obj = obj[""];
            //obj = $(obj).filter(function (n, item) {
            //    //return parseInt(item.RowKey) || item.AttendanceStatusKey != Resources.AttendanceStatusAbsent; Commented on 17/Oct/2022
            //    return parseInt(item.RowKey);
            //}).map(function (n, item) {
            //    if (item.AttendanceStatusKey == Resources.AttendanceStatusAbsent) {
            //        item.InDateTime = null;
            //        item.OutDateTime = null;
            //    }
            //    return item;
            //}).toArray();
                      

            obj = $(obj).map(function (n, item) {
                if (item.AttendanceStatusKey == Resources.AttendanceStatusAbsent) {
                    item.InDateTime = null;
                    item.OutDateTime = null;
                }
                if (item.InTime != null && item.OutTime != null) {
                    item.InTime = (item['InTime'] != "" ? moment(item['InTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : "09:30");
                    item.OutTime = (item['OutTime'] != "" ? moment(item['OutTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : "18:30");
                }
                return item;
            }).toArray();



            //var pattern = new RegExp(/(0[1-9]:[0-5][0-9]((\ ){0,1})((AM)|(PM)|(am)|(pm)))|([1-9]:[0-5][0-9]((\ ){0,1})((AM)|(PM)|(am)|(pm)))|(1[0-2]:[0-5][0-9]((\ ){0,1})((AM)|(PM)|(am)|(pm)))/);
            //for (var i = 1; i < obj.length; i++) {
            //    if (pattern.test(obj[i]['value'])) {
            //        obj[i]['value'] = (obj[i]['value'] != "" ? moment(obj[i]['value'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
            //    }
            //}
           

            $.ajax({
                url: $(form)[0].action,
                type: $(form)[0].method,
                data: { modelList: obj },
                success: function (result) {
                    if (result.IsSuccessful) {
                        window.location.href = $("#hdnEmployeeAttendanceList").val();

                        // $(AttendanceGrid).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                        //  Load data from the server and place the returned HTML into the matched element
                    } else {
                        $('.modal-content').html(result);
                        //formSubmit();

                    }


                }
            });

        }
        return false;

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
        obj.Title = "Employee Attendance Summary" + ($("#EmployeeKey").val() != "" ? ' - ' + $("#EmployeeKey").find("option:selected").text() : "");
        obj.SubTitle = $("#SearchFromDate").val() === $("#SearchToDate").val() ? $("#SearchFromDate").val() : $("#SearchFromDate").val() + " - " + $("#SearchToDate").val();
        obj.FileName = (obj.Title + (obj.SubTitle ? ("_" + obj.SubTitle) : "")).replace(/-/g, '_');
                        
        obj.ajaxData.SearchFromDate = $('#SearchFromDate').val();
        obj.ajaxData.searchDate = $('#AttendanceDate').val();
        obj.ajaxData.EmployeeKey = $('#EmployeeKey').val();
        obj.ajaxData.BranchKey = $('#BranchKey').val();
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
        GetEmployeeAttendances: getEmployeeAttendances,
        GetDepartmentsByBranchID: getDepartmentsById,
        GetEmployeeByBranchID: getEmployeeById,
        EditPopup: editPopup,
        GetQuickEmployeeAttendances: getQuickEmployeeAttendances,
        GetEmployeesByBranchId: getEmployeesByBranchId,
        GetAttendanceSheet: getAttendanceSheet,
        DownloadAttendanceSheet: downloadAttendanceSheet,
        HandleFile: handleFile,
        SingleProcessClickEvent: singleProcessClickEvent,
        MultipleProcessClickEvent: multipleProcessClickEvent,
        HideShowInOutByConfigType: hideShowInOutByConfigType,
        //HideShowInOutByConfigTypeSingle:hideShowInOutByConfigTypeSingle,
        AttendanceStatusChange: attendanceStatusChange,
        AttendanceStatusChangeSingle: attendanceStatusChangeSingle,
        FormSubmit: formSubmit,
        LoadData: loadData,
        FormSubmitSingle: formSubmitSingle,
        ChangeDropdownKey: changeDropdownKey,
        GetAttendanceDetail: getAttendanceDetail,
        GetExportPrintData: getExportPrintData
    }

}());

//Employee Attendance
function deleteEmployeeAttendance(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeContact,
        actionUrl: $("#hdnDeleteEmployeeAttendance").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $(AttendanceGrid).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            $(QuickAttendanceGrid).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}
function formSubmitSingle() {
    var form = $('#frmEmployeeAttendance');
    $("[disabled]", $(form)).removeAttr("disabled");
    var validate = $(form).valid();
    if (validate) {
        var obj = $(form).serializeArray();
        $(form).mLoading()
        delete obj[0];
        //var pattern = new RegExp(/(0[1-9]:[0-5][0-9]((\ ){0,1})((AM)|(PM)|(am)|(pm)))|([1-9]:[0-5][0-9]((\ ){0,1})((AM)|(PM)|(am)|(pm)))|(1[0-2]:[0-5][0-9]((\ ){0,1})((AM)|(PM)|(am)|(pm)))/);
        //for (var i = 1; i < obj.length; i++) {
        //    if (pattern.test(obj[i]['value'])) {
        //        obj[i]['value'] = (obj[i]['value'] != "" ? moment(obj[i]['value'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
        //    }

        //}
        $.ajax({
            url: $(form)[0].action,
            type: $(form)[0].method,
            data: obj,
            success: function (result) {
                if (result.IsSuccessful) {
                    $('.modal').modal('hide');
                    $(AttendanceGrid).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                    //  Load data from the server and place the returned HTML into the matched element
                } else {
                    $('.modal-content').html(result);
                    //formSubmit();

                }


            }
        });

    }
    return false;

}
function AttendancePopupLoad() {

    AppCommon.AutoHideDropdown();
    AppCommon.FormatDateInput();
    AppCommon.FormatTimeInput();

    EmployeeAttendance.AttendanceStatusChangeSingle($("#AttendanceStatusKey"));


    $("[data-input-type =dateTime]").each(function () {

        if ($(this).val() != "") {
            $(this).val(moment($(this).val(), ["DD/MM/YYYY HH:mm:ss"]).format("DD/MM/YYYY hh:mm A"));
        }
    })


    $("#BranchKey").on("change", function () {
        EmployeeAttendance.GetEmployeesByBranchId($(this).val(), $("#AppUserKey"));
    });
    $("#AttendanceStatusKey").on("change", function () {
        EmployeeAttendance.AttendanceStatusChangeSingle($(this));
    });
}

//Quick Attendance
function UpdateQuickAttendance(rowId, AttendenceStatusKey, AttendancePresentStatusKey) {
    var obj = $('#gridQuick').jqGrid('getRowData', rowId);
    //$.each(obj, function (key) {
    //    //if (key == "InDateTime" || key == "OutTime") {
    //    //    jsonData[key] = obj[key] != "" ? moment(obj[key], ["h:mm A"]).format("HH:mm") : null;
    //    //    //AppCommon.FormatAMPMTo24Hrs(obj[key]) : null;
    //    //}
    //    //else {
    //    jsonData[key] = obj[key];
    //    //}
    //});
    //jsonData["InDateTime"] = obj.InDateTime != "" ? moment(obj.InDateTime, ["h:mm A"]).format("HH:mm") : null;
    //jsonData["OutDateTime"] = obj.OutDateTime != "" ? moment(obj.OutDateTime, ["h:mm A"]).format("HH:mm") : null;
    jsonData["InDateTime"] = obj.InDateTime;// != "" ? moment(obj.InDateTime, ["DD/MM/YYYY HH:mm:ss"]).format("DD/MM/YYYY hh:mm") : null;
    jsonData["OutDateTime"] = obj.OutDateTime;// != "" ? moment(obj.OutDateTime, ["DD/MM/YYYY HH:mm:ss"]).format("DD/MM/YYYY hh:mm") : null;
    if (AttendancePresentStatusKey == Resources.AttendancePresentStatusBreakIn) {
        jsonData["OutDateTime"] = moment().format("DD/MM/YYYY HH:mm:ss");
    }
    var AttendanceDate = $("#AttendanceDate").val();
    //jsonData["AttendanceDate"] = moment(obj.InDateTime).format("DD/MM/YYYY");
    jsonData["AttendanceDate"] = AttendanceDate;
    jsonData["AttendanceStatusKey"] = AttendenceStatusKey;
    jsonData["AttendancePresentStatusKey"] = AttendancePresentStatusKey;
    jsonData["EmployeeKey"] = obj.EmployeeKey;
    jsonData["RowKey"] = obj.RowKey;
    jsonData["ClockInStatus"] = null;
    if (AttendancePresentStatusKey == Resources.AttendancePresentStatusCheckOut) {
        EduSuite.ConfirmWithMultipleParam({
            title: Resources.Confirmation,
            content: Resources.CheckOut_Confirm_Attendance,
            actionUrl: $("#hdnAddEditQuickEmployeeAttendance").val(),
            param: {
                model: jsonData
            },
            dataRefresh: function () {
                $(QuickAttendanceGrid).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        });
    }
    else {
        var response = AjaxHelper.ajax("POST", $("#hdnAddEditQuickEmployeeAttendance").val(),
            {
                model: jsonData
            });

        if (response.IsSuccessful) {
            toastr.success(Resources.Success, response.Message);
            $(QuickAttendanceGrid).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            //  Load data from the server and place the returned HTML into the matched element
        }
        else {
            toastr.error(Resources.Failed, response.Message);
        }
    }

}

//Attendance Sheet
function AttendanceSheetDataFromExcel(data) {
    var ret = [];
    var month = AppCommon.ParseMMMYYYYDate($("input#AttendanceDate").val());
    var monthKey = moment(month).month();
    var yearKey = moment(month).year();
    var totalDays = (new Date(yearKey, monthKey + 1, 0)).getDate();
    for (var i = 1; i < data.length; i++) {
        var dataItem = data[i], item = {}, attDetails = [];
        for (var j = 0; j < dataItem.length; j++) {

            if (data[0][j]) {
                if (parseInt(data[0][j].trim())) {
                    if (parseInt(data[0][j].trim()) <= totalDays) {
                        var childItem = {};
                        childItem["AttendanceDate"] = new Date(yearKey, monthKey, parseInt(data[0][j].trim()));
                        childItem["AttendanceStatusCode"] = data[i][j];
                        attDetails.push(childItem);
                    }
                } else {
                    item[data[0][j]] = data[i][j];
                }


            }


        }
        var m = attDetails.length + 1;
        while (m <= totalDays) {
            var childItem = {};
            childItem["AttendanceDate"] = new Date(yearKey, monthKey, m);
            childItem["AttendanceStatusCode"] = "";
            attDetails.push(childItem);
            m++;
        }
        item["AttendanceDetails"] = attDetails;
        ret.push(item);

    }
    return ret
}

function AttendanceSheetData(data, statusList, totalDays, colModels, colHeaders, groupParam, salaryMonth, salaryYear) {
    var ret = [];
    statusList = CreateAttendanceStatusList(statusList, "Text", "Text");
    groupParam.Start = 1;

    colModels.push(createGridColum("EmployeeKey", 50, null, true, false))
    colModels.push(createGridColum("EmployeeName"))
    colHeaders.push("Emp Code")
    colHeaders.push("Emp Name")

    for (var i = 0; i < data.length; i++) {
        var dataItem = data[i], item = [];

        item["EmployeeKey"] = dataItem["EmployeeKey"];
        item["EmployeeName"] = dataItem["EmployeeName"];

        for (var j = 0; j < dataItem["AttendanceDetails"].length; j++) {
            var DateNumber = new Date(AppCommon.JsonDateToNormalDate(dataItem["AttendanceDetails"][j]["AttendanceDate"])).getDate();
            item[DateNumber] = dataItem["AttendanceDetails"][j]["AttendanceStatusCode"];
        }

        item["TotalOvertime"] = dataItem["TotalOvertime"];
        item["TotalDays"] = totalDays;
        ret.push(item);

    }
    for (var k = 1; k <= totalDays; k++) {
        colModels.push(CreateDropdownColum(k, 20, statusList));
        var dayName = AppCommon.GetDayNameFromDate(new Date(salaryYear, (salaryMonth - 1), k), 1);
        colHeaders.push(k + "<br/>" + dayName.toUpperCase());
        groupParam.End++;
    }
    colModels.push(createGridColum("TotalOvertime", 30, null, null, null, true))
    colModels.push(createGridColum("TotalDays", 30))
    colModels.push(createGridColum("Action", 50, attendanceSheetAction))
    colHeaders.push("OT (hrs)");
    colHeaders.push("Total");
    colHeaders.push("Action")
    return ret
}

function attendanceSheetAction(cellValue, options, rowdata, action) {
    var temp = "'" + rowdata.EmployeeKey + "'";

    var html = '<div class="divEditDelete"><a class="btn btn-primary btn-sm" data-inputtype="submit" title="' + Resources.Process + '" onclick="EmployeeAttendance.SingleProcessClickEvent(' + temp + ')" ><i class="fa fa-floppy-o" aria-hidden="true"></i></a>'
    html = html + "</div>"

    return html;
}

function setAttendanceSheetParam(jsonData) {
    var month = AppCommon.ParseMMMYYYYDate($("input#AttendanceDate").val());
    jsonData["AttendanceMonthKey"] = moment(month).month() + 1;
    jsonData["AttendanceYearKey"] = moment(month).year();
    jsonData["BranchKey"] = $('#BranchKey').val() != "" ? parseInt($('#BranchKey').val()) : 0;
}

function createGridColum(name, width, formatter, key, hidden, editable) {
    var obj = {};
    obj["name"] = name;
    if (width)
        obj["width"] = width;
    if (formatter)
        obj["formatter"] = formatter;
    if (key)
        obj["key"] = key;
    if (hidden)
        obj["hidden"] = hidden;
    if (editable) {
        obj["editable"] = editable;
        obj["cellEdit"] = editable;
        obj["editoptions"] = {
            dataEvents: [{
                type: 'blur',
                fn: function (e) {
                    var row = $(this).closest("tr");
                    var col = $(row).find("td")[0];
                    $(col).trigger("click")
                }
            }
            ]
        }

    }

    return obj;
}

function CreateDropdownColum(name, width, list) {
    var obj = {
        name: 'name',
        index: 'name',
        width: 50,
        sortable: true,
        align: 'center',
        editable: true,
        cellEdit: true,
        onCellSelect: function (rowid) {
        },
        edittype: 'select',
        formatter: 'select',
        editoptions: {
            value: list,
            dataInit: function (elem) {
                CreateAttendanceStatusDropdown(elem);
            },
            dataEvents: [
                {
                    type: 'change'
                    , fn: function (e) {
                        var code = $(this).val();
                        var td = $(this).closest("td");
                        var select = $(this);
                        $(td).css({ "background": "none" });
                        $(AttendanceStatusList).each(function () {
                            if (this.Text == code) {
                                $(select).css({ "background": this.GroupName, "color": checkColorStrength(this.GroupName) });
                                $(td).css({ "background": this.GroupName, "color": checkColorStrength(this.GroupName) })
                            }
                        });
                    }
                },
                {
                    type: 'blur',
                    fn: function (e) {
                        var row = $(this).closest("tr");
                        var col = $(row).find("td")[0];
                        $(col).trigger("click")
                    }
                }
            ]
        },
        buildSelect: function (data) {

        }
    }
    if (name) {
        obj["name"] = obj["index"] = name;
    }
    if (width) {
        obj["width"] = width;
    }
    return obj;
}

function formatAttendanceStatusColumn(cellValue, options, rowdata, action) {
    $("#gridSheet").setCell(options.rowId, options.pos.toString(), '', { 'background': '#ff0000' });
    return cellValue ? cellValue : "";
}

function CreateAttendanceStatusList(list, key, value) {
    var newList = { "": "" };
    $.each(list, function () {
        newList[this[key]] = this[value];
    });
    return newList;
}
function CreateAttendanceStatusDropdown(list) {
    $(list).find("option").each(function () {
        var code = this.value;
        var option = $(this);
        $(AttendanceStatusList).each(function () {
            if (code == "") {
                $(option).css({ "background": "#efefef", "color": "black" });

            }
            else if (this.Text == code) {
                $(option).css({ "background": this.GroupName, "color": checkColorStrength(this.GroupName) });

            }

        });
    });
}

function checkColorStrength(color) {
    return AppCommon.SetColorByBackgroundIntensity(color);
}

function ModifyMultipleModel(Json, data) {
    Json["AttendanceStatuses"] = Json["Departments"] = Json["Employees"] = Json["Branches"] = {};
    var multiJson = [];
    var month = AppCommon.ParseMMMYYYYDate($("input#AttendanceDate").val());
    var monthKey = moment(month).month();
    var yearKey = moment(month).year();
    for (var i = 0; i < data.length; i++) {
        var dataItem = data[i];
        for (var key in dataItem) {

            if (key) {
                if (parseInt(key)) {
                    var newJson = $.extend(true, {}, Json);
                    newJson["AttendanceDate"] = AppCommon.JavascriptDateToServerDate(new Date(yearKey, monthKey, parseInt(key)));
                    newJson["AttendanceStatusCode"] = dataItem[key];
                    newJson["EmployeeKey"] = dataItem["EmployeeKey"];
                    newJson["TotalOvertime"] = dataItem["TotalOvertime"];
                    multiJson.push(newJson);
                }


            }


        }
    }
    return multiJson;
}

//For Excel Download
function Workbook() {
    if (!(this instanceof Workbook)) return new Workbook();
    this.SheetNames = [];
    this.Sheets = {};
}
function sheet_from_array_of_arrays(data, opts) {
    var ws = {};
    var range = { s: { c: 10000000, r: 10000000 }, e: { c: 0, r: 0 } };
    for (var R = 0; R != data.length; ++R) {
        for (var C = 0; C != data[R].length; ++C) {
            if (range.s.r > R) range.s.r = R;
            if (range.s.c > C) range.s.c = C;
            if (range.e.r < R) range.e.r = R;
            if (range.e.c < C) range.e.c = C;
            var cell = { v: data[R][C] };
            if (cell.v == null) continue;
            var cell_ref = XLSX.utils.encode_cell({ c: C, r: R });

            if (typeof cell.v === 'number') cell.t = 'n';
            else if (typeof cell.v === 'boolean') cell.t = 'b';
            else if (cell.v instanceof Date) {
                cell.t = 'n'; cell.z = XLSX.SSF._table[14];
                cell.v = datenum(cell.v);
            }
            else if (cell.v.substr(0, 1) == '=') {
                cell.f = cell.v;
            }
            else cell.t = 's';

            ws[cell_ref] = cell;
        }
    }
    if (range.s.c < 10000000) ws['!ref'] = XLSX.utils.encode_range(range);
    return ws;
}
function s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
}

function to_json(workbook) {
    var result = {};
    workbook.SheetNames.forEach(function (sheetName) {
        var roa = XLSX.utils.sheet_to_json(workbook.Sheets[sheetName], { header: 1, dateNF: 'dd/mm/yyyy hh:mm:ss;@' });
        if (roa.length > 0) result[sheetName] = roa;
    });
    return result;
}


function process_wb(wb, sheetidx) {
    last_wb = wb;

    var sheet = wb.SheetNames[sheetidx || 0];
    var json = to_json(wb)[sheet];

    return json;
}


function deleteBulkEmployeeAttendance(grid) {
    var myGrid = AttendanceGrid;
    if (grid == 1) {
        myGrid = AttendanceGrid;
    }
    else {
        myGrid = QuickAttendanceGrid;
    }
   
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
            actionUrl: $("#hdnDeleteEmployeeAttendanceBulk").val(),
            parameters: obj,
            dataRefresh: function () {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        });
    }
}



