var EnquiryDashBoard = (function () {
    var getEnquiryCounts = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $("#FromDate").val();
        obj.ToDate = $("#ToDate").val();
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.AppUserKey = $("#AppUserKey").val();
        obj.FetchType = "EnquiryCounts";


        $.ajax({
            typ: "Get",
            url: $("#hdnEnquiryCounts").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata()[0];
                if (data.length != 0) {
                    if (data) {
                        data.DateTimeNow = moment().format("YYYY-MM-DD")
                        data.DateTimeTomorrow = moment().add(1, "days").format("YYYY-MM-DD")
                        data.ScheduleStatusNewLead = Resources.ScheduleStatusNewLead;
                        data.ScheduleStatusUnAllocated = Resources.ScheduleStatusUnAllocated;
                        data.ScheduleStatusToday = Resources.ScheduleStatusToday;
                        data.ScheduleStatusPending = Resources.ScheduleStatusPending;
                        data.ScheduleStatusUpcoming = Resources.ScheduleStatusUpcoming;
                        data.ScheduleStatusHistory = Resources.ScheduleStatusHistory;
                        data.ScheduleStatusTomorrow = Resources.ScheduleStatusTomorrow;
                        data.ScheduleStatusTodayReshceduled = Resources.ScheduleStatusTodayReshceduled;
                        data.ScheduleStatusCounsellingSchedule = Resources.ScheduleStatusCounsellingSchedule;
                        data.ScheduleStatusShortlisted = Resources.ScheduleStatusShortlisted;
                        data.ScheduleStatusTodayReshceduled = Resources.ScheduleStatusTodayReshceduled;
                        data.ScheduleStatusShortlistPending = Resources.ScheduleStatusShortlistPending;


                        $.ajax({
                            type: 'GET',
                            crossDomain: true,
                            url: $("#hdnEnquiryCountsPath").val() + "?no-cache=" + new Date().getTime(),
                            success: function (response) {
                                AppCommon.HandleBarHelpers();
                                var template = Handlebars.compile(response);
                                var html = template(data);
                                $('#dvEnquiryCounts').html(html);
                            },
                            error: function (xhr) {

                            },
                            complete: function () {

                            }
                        })
                    }
                }
                else {
                    $('#dvEnquiryCounts').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getLeadStageFlows = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "LeadStageFlows";

        obj.FromDate = $('#reportrange').data('daterangepicker').startDate.format("YYYY-MM-DD");;
        obj.ToDate = $('#reportrange').data('daterangepicker').endDate.format("YYYY-MM-DD");;
        $("#funnel").mLoading();
        $.ajax({
            typ: "Get",
            url: $("#hdnLeadStageFlows").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {

                data = data.formatdata()[0];
                if (data) {
                    var chartdata = [];
                    data.TotalLead = data.TotalLead ? data.TotalLead : 0;
                    data.LeadToEnquiry = data.LeadToEnquiry ? data.LeadToEnquiry : 0;
                    data.EnquiryIntrested = data.EnquiryIntrested ? data.EnquiryIntrested : 0;
                    data.EnquiryIntrested += data.EnquiryAdmission;
                    data.EnquiryAdmission = data.EnquiryAdmission ? data.EnquiryAdmission : 0;

                    chartdata = [
                        ["Leads", (data.TotalLead ? data.TotalLead : 0)],
                        ["Enquiry", (data.LeadToEnquiry ? data.LeadToEnquiry : 0)],
                        ["Intrested", (data.EnquiryIntrested ? data.EnquiryIntrested : 0)],
                        ["Admission", (data.EnquiryAdmission ? data.EnquiryAdmission : 0)],
                    ];

                    $("#funnel").mLoading("destroy");
                    var options = {

                        isCurved: true,
                        block: {
                            highlight: true,
                            fill: {
                                type: 'gradient',

                            },
                        },
                        chart: {
                            animate: 200,
                            curve: {
                                enabled: true,
                            },
                        },
                        label: {
                            format: '{l}\n{f}',
                            getFormatter: function (a) {
                                console.log(a)
                            }
                        },
                    }
                    var chart = new D3Funnel("#funnel");
                    chart.draw(chartdata, options);

                }
                else {
                    $('#dvLeadStageFlows').html(Resources.DashBoardPermisionMessage);
                }
            }

        })

    }

    var getLeadSource = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $("#FromDate").val();
        obj.ToDate = $("#ToDate").val();
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "LeadSource";
        $("#dvLeadSource").mLoading();
        $.ajax({
            typ: "Get",
            url: $("#hdnLeadSource").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();
                if (data.length != 0) {
                    data = { LeadSource: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnEnquiryLeadSourcePath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvLeadSource').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvLeadSource").mLoading("destroy");
                        }
                    })
                }
                else {
                    $('#dvLeadSource').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getEnquiryRecentCalls = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $("#FromDate").val();
        obj.ToDate = $("#ToDate").val();
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "RecentCalls";

        $("#divRecentCalls").mLoading();
        $.ajax({
            typ: "Get",
            url: $("#hdnEnquiryRecentCalls").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                data = data.formatdata();
                if (data.length != 0) {
                    data = { RecentCalls: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnEnquiryRecentCallsPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#divRecentCalls').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#divRecentCalls").mLoading("destroy");
                        }
                    })
                }
                else {
                    $('#divRecentCalls').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getEnquirySurvey = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        var Selected = $('#hdnEnquirySurveyYear').val();
        obj.FromDate = moment(Selected).startOf('year').format("YYYY-MM-DD");
        obj.ToDate = moment(Selected).endOf('year').format("YYYY-MM-DD");

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "MonthlyGraph";

        $("#dvEnquirySurvey").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnEnquiryMonthlyGraph").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();
                if (data.length != 0) {
                    var label = data.map(function (item) {
                        return moment(item.MonthNumber, 'M').format('MMMM');
                    });

                    $("#dvEnquirySurvey").mLoading("destroy");
                    var chart = c3.generate({
                        bindto: '#dvEnquirySurvey',
                        data: {
                            json: data,
                            keys: {
                                value: ['FollowUp', 'Intrested', 'Closed', 'Admission'],
                            },
                            columns: ['FollowUp', 'Intrested', 'Closed', 'Admission'],
                            type: 'bar',
                            groups: [
                                ['FollowUp', 'Intrested', 'Closed', 'Admission']
                            ]
                        },
                        axis: {
                            x: {
                                type: 'category',
                                categories: label,
                                tick: {
                                    rotate: 50
                                },
                            }
                        },
                    });
                }
                else {
                    $('#dvEnquirySurvey').html(Resources.DashBoardPermisionMessage);
                }


            }
        })

    }

    var getEnquiryEmployeeCounts = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $("#FromDate").val();
        obj.ToDate = $("#ToDate").val();
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.AppUserKey = $("#AppUserKey").val();
        obj.FetchType = "EmployeeCounts";
        $("#dvEnquiryEmployeeCounts").mLoading();
        $.ajax({
            typ: "Get",
            url: $("#hdnEnquiryEmployeeCounts").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();
                if (data.length != 0) {
                    data = { LeadSource: data };
                    $(data.LeadSource).each(function (i, item) {
                        item.TotalEnquiryCount = parseInt(item.TotalFollowUpCount) + parseInt(item.TotalAdmissionTakenCount) + parseInt(item.TotalIntrestedCount) + parseInt(item.TotalClosedCount);
                        item.RepeatedCallsCount = parseInt(item.TotalCallsCount) - parseInt(item.TotalEnquiryCount);
                    });

                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnEnquiryEmployeeCountsPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvEnquiryEmployeeCounts').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvEnquiryEmployeeCounts").mLoading("destroy");
                        }
                    })
                }
                else {
                    $('#dvEnquiryEmployeeCounts').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }


    return {
        GetEnquiryCounts: getEnquiryCounts,
        GetLeadStageFlows: getLeadStageFlows,
        GetLeadSource: getLeadSource,
        GetEnquiryRecentCalls: getEnquiryRecentCalls,
        GetEnquirySurvey: getEnquirySurvey,
        GetEnquiryEmployeeCounts: getEnquiryEmployeeCounts
    }
}());

var StudentDashBoard = (function () {

    var getStudentCounts = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $("#FromDate").val();
        obj.ToDate = $("#ToDate").val();
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "StudentsCount";


        $.ajax({
            typ: "Get",
            url: $("#hdnStudentsCount").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata()[0];
                if (data.length != 0) {
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnStudentsCountPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvStudentCount').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {

                        }
                    })
                }
                else {
                    $('#dvStudentCount').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getYearlyAdmissionGraph = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        var Selected = $('#hdnEnquirySurveyYear').val();
        obj.FromDate = moment(Selected).startOf('year').format("YYYY-MM-DD");
        obj.ToDate = moment(Selected).endOf('year').format("YYYY-MM-DD");

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "YearlyAdmissionGraph";

        $("#dvYearlyAdmissionGraph").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnYearlyAdmissionGraph").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();

                if (data.length != 0) {
                    var label = data.map(function (item) {
                        return item.year;
                    });

                    $("#dvYearlyAdmissionGraph").mLoading("destroy");

                    var chart = c3.generate({
                        bindto: '#dvYearlyAdmissionGraph',
                        data: {
                            json: data,
                            keys: {
                                value: ['OnGoing', 'Completed', 'Droped'],
                            },
                            columns: ['OnGoing', 'Completed', 'Droped'],
                            type: 'bar',
                            groups: [
                                ['OnGoing', 'Completed', 'Droped']
                            ]
                        },
                        axis: {
                            x: {
                                type: 'category',
                                categories: label,
                                tick: {
                                    rotate: 50
                                },
                            }
                        },
                    });

                }
                else {
                    $('#dvYearlyAdmissionGraph').html(Resources.DashBoardPermisionMessage);
                }

            }
        })

    }

    var getAdmissionByCourseType = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        var Selected = $('#hdnStudentsCountByCourseType').val();
        obj.FromDate = moment(Selected).startOf('year').format("YYYY-MM-DD");
        obj.ToDate = moment(Selected).endOf('year').format("YYYY-MM-DD");

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "AdmissionByCourseType";



        $("#dvStudentCountByCourseType").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnStudentCountByCourseType").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {

                var columnNames = [];
                data = data.formatdata();
                if (data.length != 0) {
                    data = data.map(function (item) {
                        var arr = [];
                        arr.push(item.CourseTypeName);
                        columnNames.push(item.CourseTypeName);
                        arr.push(item.Total);
                        return arr;
                    });

                    $("#dvStudentCountByCourseType").mLoading("destroy");

                    var chart = c3.generate({
                        bindto: '#dvStudentCountByCourseType',
                        data: {
                            columns: data,
                            type: 'pie',
                            onclick: function (d, i) { console.log("onclick", d, i); },
                            onmouseover: function (d, i) { console.log("onmouseover", d, i); },
                            onmouseout: function (d, i) { console.log("onmouseout", d, i); }
                        }
                    });
                }
                else {
                    $('#dvStudentCountByCourseType').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getStudentCountByCourse = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        var Selected = $('#hdnStudentsCountByCourse').val();

        obj.FromDate = moment(Selected).startOf('year').format("YYYY-MM-DD");
        obj.ToDate = moment(Selected).endOf('year').format("YYYY-MM-DD");
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "StudentCountByCourse";

        $("#dvStudentCountByCourse").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnStudentCountByCourse").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();
                if (data.length != 0) {
                    data = { CourseDetails: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnStudentCountByCoursePath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvStudentCountByCourse').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvStudentCountByCourse").mLoading("destroy");
                        }
                    })

                }
                else {
                    $('#dvStudentCountByCourse').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getAbsentList = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $('#AbsentListreportrange').data('daterangepicker').startDate.format("YYYY-MM-DD");;
        obj.ToDate = $('#AbsentListreportrange').data('daterangepicker').endDate.format("YYYY-MM-DD");;

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "TodayAbsentList";

        $("#dvAbsentList").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnTodayAbsentList").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();
                if (data.length != 0) {
                    data = { AbsentList: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnStudentAbsentListPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvAbsentList').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvAbsentList").mLoading("destroy");
                        }
                    })
                }
                else {
                    $('#dvAbsentList').html(Resources.DashBoardPermisionMessage);
                }

            }
        })

    }

    var getStudentDiaryNotes = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();


        obj.FromDate = $('#StudentDiaryreportrange').data('daterangepicker').startDate.format("YYYY-MM-DD");;
        obj.ToDate = $('#StudentDiaryreportrange').data('daterangepicker').endDate.format("YYYY-MM-DD");;

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "StudentDiaryDetails";

        $("#dvStudentDiaryNotes").mLoading();
        $.ajax({
            typ: "Get",
            url: $("#hdnStudentDiaryDetails").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                data = data.formatdata();
                if (data.length != 0) {
                    data = { StudentDiary: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnStudentDiaryDetailsPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvStudentDiaryNotes').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvStudentDiaryNotes").mLoading("destroy");
                        }
                    })

                }
                else {
                    $('#dvStudentDiaryNotes').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getNewAdmissionStudents = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $('#NewAdmissionStudentsreportrange').data('daterangepicker').startDate.format("YYYY-MM-DD");;
        obj.ToDate = $('#NewAdmissionStudentsreportrange').data('daterangepicker').endDate.format("YYYY-MM-DD");;

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "NewAdmissionStudents";

        $("#dvNewAdmissionStudents").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnStudentNewAdmission").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                data = data.formatdata();
                if (data.length != 0) {
                    data = { StudentList: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnStudentNewAdmissionPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvNewAdmissionStudents').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvNewAdmissionStudents").mLoading("destroy");
                        }
                    })

                }
                else {
                    $('#dvNewAdmissionStudents').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }
    return {
        GetStudentCounts: getStudentCounts,
        GetYearlyAdmissionGraph: getYearlyAdmissionGraph,
        GetAdmissionByCourseType: getAdmissionByCourseType,
        GetStudentCountByCourse: getStudentCountByCourse,
        GetAbsentList: getAbsentList,
        GetStudentDiaryNotes: getStudentDiaryNotes,
        GetNewAdmissionStudents: getNewAdmissionStudents
    }
}());

var AccountDashBoard = (function () {

    var getAccountsCounts = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $("#FromDate").val();
        obj.ToDate = $("#ToDate").val();
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "AccountsCount";

        $("#dvAccountsCount").mLoading();
        $.ajax({
            typ: "Get",
            url: $("#hdnAccountsCount").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata()[0];
                if (data.length != 0) {


                    var IncomeDiffrence = data.Income - data.PreIncome;
                    var IncomePercentage = IncomeDiffrence * 100 / (IncomeDiffrence >= 0 ? (data.Income ? data.Income : 1) : (data.PreIncome ? data.PreIncome : 1));
                    data.IncomePercentage = data.Income ? AppCommon.RoundTo(IncomePercentage, Resources.RoundToDecimalPostion) : 0;


                    var ExpenseDiffrence = data.Expense - data.PreExpense;
                    var ExpensePercentage = ExpenseDiffrence * 100 / (ExpenseDiffrence >= 0 ? (data.Expense ? data.Expense : 1) : (data.PreExpense ? data.PreExpense : 1));
                    data.ExpensePercentage = data.Expense ? AppCommon.RoundTo(ExpensePercentage, Resources.RoundToDecimalPostion) : 0;

                    var CashBalanceDiffrence = data.CashBalance - data.PreCashBalance;
                    var CashBalancePercentage = CashBalanceDiffrence * 100 / (CashBalanceDiffrence >= 0 ? (data.CashBalance ? data.CashBalance : 1) : (data.PreCashBalance ? data.PreCashBalance : 1));
                    data.CashBalancePercentage = data.CashBalance ? AppCommon.RoundTo(CashBalancePercentage, Resources.RoundToDecimalPostion) : 0;

                    var BankBalanceDiffrence = data.BankBalance - data.PreBankBalance;
                    var BankBalancePercentage = BankBalanceDiffrence * 100 / (BankBalanceDiffrence >= 0 ? (data.BankBalance ? data.BankBalance : 1) : (data.PreBankBalance ? data.PreBankBalance : 1));
                    data.BankBalancePercentage = data.BankBalance ? AppCommon.RoundTo(BankBalancePercentage, Resources.RoundToDecimalPostion) : 0;


                    var Profit = data.Income - data.Expense;
                    var PreProfit = data.PreIncome - data.PreExpense;

                    var ProfitDiffrence = Profit - PreProfit;
                    var ProfitPercentage = ProfitDiffrence * 100 / (ProfitDiffrence >= 0 ? (Profit ? Profit : 1) : (PreProfit ? PreProfit : 1));
                    data.ProfitPercentage = Profit ? AppCommon.RoundTo(ProfitPercentage, Resources.RoundToDecimalPostion) : 0;
                    data.Profit = Profit;
                    data.PreProfit = PreProfit;

                    if (Profit < 0) {
                        data.LossProfit = Math.abs(Profit);
                    }




                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnAccountsCountPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvAccountsCount').html(html);

                            $("#dvAccountsCount").mLoading("destroy");
                        },
                        error: function (xhr) {

                        },
                        complete: function () {

                        }
                    })

                }
                else {
                    $('#dvAccountsCount').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getIncomeExpenseChart = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        var Selected = $('#hdnIncomeExpenseChart').val();
        obj.FromDate = moment(Selected).startOf('year').format("YYYY-MM-DD");
        obj.ToDate = moment(Selected).endOf('year').format("YYYY-MM-DD");

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "IncomeExpenseChart";

        $("#dvIncomeExpenseChart").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnIncomeandExpenseChart").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();

                if (data.length != 0) {
                    var label = data.map(function (item) {
                        return moment(item.MonthNumber, 'M').format('MMM');
                    });

                    $("#dvIncomeExpenseChart").mLoading("destroy");
                    var chart = c3.generate({
                        bindto: '#dvIncomeExpenseChart',
                        data: {
                            json: data,
                            keys: {
                                value: ['Income', 'Expense'],
                            },
                            columns: ['Income', 'Expense'],
                            type: 'bar'

                        },
                        axis: {
                            x: {
                                type: 'category',
                                categories: label,

                            }
                        },
                    });

                }
                else {
                    $('#dvIncomeExpenseChart').html(Resources.DashBoardPermisionMessage);
                }

            }
        })

    }

    var getCashFlowStatement = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        var Selected = $('#hdnCashFlowStatement').val();
        obj.FromDate = moment(Selected).startOf('year').format("YYYY-MM-DD");
        obj.ToDate = moment(Selected).endOf('year').format("YYYY-MM-DD");

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "CashFlowStatementChart";

        $("#dvCashFlowStatement").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnCashFlowGraph").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();
                if (data.length != 0) {
                    var Income = ["In"]
                    var Expense = ["Out"];
                    var Profit = ["Balance"]
                    var label = data.filter(function (item) {
                        return !item.CashFlowTypeKey
                    });
                    label.forEach(function (labelItem) {
                        var IncomeAmount = data.filter(function (item) {
                            return item.CashFlowTypeKey == 1 && item.MonthNumber == labelItem.MonthNumber;
                        }).map(function (item) {
                            return item.CashBalance;
                        });
                        IncomeAmount = parseFloat(IncomeAmount) ? parseFloat(IncomeAmount) : 0;
                        var ExpenseAmount = data.filter(function (item) {
                            return item.CashFlowTypeKey == 2 && item.MonthNumber == labelItem.MonthNumber;
                        }).map(function (item) {

                            return item.CashBalance;
                        });
                        ExpenseAmount = parseFloat(ExpenseAmount) ? parseFloat(ExpenseAmount) : 0;
                        var ProfitAmount = parseFloat(IncomeAmount) + parseFloat(ExpenseAmount);
                        Income.push(IncomeAmount);
                        Expense.push(ExpenseAmount);
                        Profit.push(ProfitAmount);
                    });
                    var label = data.map(function (item) {
                        return moment(item.MonthNumber, 'M').format('MMM');
                    });
                    $("#dvCashFlowStatement").mLoading("destroy");
                    var chart = c3.generate({
                        bindto: '#dvCashFlowStatement',
                        data: {

                            columns: [
                                Income,
                                Expense,
                                Profit
                            ],
                            type: 'bar',
                            types: {
                                Balance: 'line'
                            },
                            groups: [
                                ['In', 'Out']
                            ]
                        },
                        axis: {
                            x: {
                                type: 'category',
                                categories: label,

                            }
                        },
                        grid: {
                            y: {
                                lines: [{ value: 0 }]
                            }
                        }
                    });

                }
                else {
                    $('#dvCashFlowStatement').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getRecentTransaction = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $("#FromDate").val();
        obj.ToDate = $("#ToDate").val();
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "RecentTransactions";

        $("#divRecentTransaction").mLoading();
        $.ajax({
            typ: "Get",
            url: $("#hdnRecentTransaction").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                data = data.formatdata();
                if (data.length != 0) {
                    data = { RecentTransaction: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnRecentTransactionPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#divRecentTransaction').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#divRecentTransaction").mLoading("destroy");
                        }
                    })
                }
                else {
                    $('#divRecentTransaction').html(Resources.DashBoardPermisionMessage);
                }
            }
        })

    }

    var getChequeDetails = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $('#Chequereportrange').data('daterangepicker').startDate.format("YYYY-MM-DD");;
        obj.ToDate = $('#Chequereportrange').data('daterangepicker').endDate.format("YYYY-MM-DD");;

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "ChequeDetails";

        $("#dvChequeDetails").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnChequeDetails").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata();
                if (data.length != 0) {
                    data = { ChequeDetails: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnChequeDetailsPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvChequeDetails').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvChequeDetails").mLoading("destroy");
                        }
                    })

                }
                else {
                    $('#dvChequeDetails').html(Resources.DashBoardPermisionMessage);
                }


            }
        })

    }

    var getIncomeExpenseCounts = function () {
        var obj = {};

        obj.BranchKey = $("#BranchKey").val();
        obj.FromDate = $('#IncomeExpensereportrange').data('daterangepicker').startDate.format("YYYY-MM-DD");;
        obj.ToDate = $('#IncomeExpensereportrange').data('daterangepicker').endDate.format("YYYY-MM-DD");;

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "AccountsCount";

        $("#dvIncomeExpenseDetails").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnAccountsCount").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata()[0];
                if (data.length != 0) {

                    data.IEDateText = $("#hdnIncomeExpenseDateText").val();


                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnIncomeExpenseDetailsPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvIncomeExpenseDetails').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvIncomeExpenseDetails").mLoading("destroy");
                        }
                    })

                }
                else {
                    $('#dvIncomeExpenseDetails').html(Resources.DashBoardPermisionMessage);
                }


            }
        })

    }

    var getSalaryDetails = function () {
        var obj = {};
        debugger
        obj.BranchKey = $("#BranchKey").val();
        //obj.FromDate = $('#Salaryreportrange').data('daterangepicker').startDate.format("YYYY-MM-DD");;
        //obj.ToDate = $('#Salaryreportrange').data('daterangepicker').endDate.format("YYYY-MM-DD");;
        var Selected = $('#hdnsalaryDetails').val();
        obj.FromDate = moment(Selected).startOf('month').format("YYYY-MM-DD");
        obj.ToDate = moment(Selected).endOf('month').format("YYYY-MM-DD");

        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.FetchType = "SalaryDetails";

        $("#dvSalaryDetails").mLoading();

        $.ajax({
            typ: "Get",
            url: $("#hdnAccountsCount").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                data = data.formatdata()[0];
                if (data.length != 0) {
                    data.PendingSalary = data.TotalGenratedSalary - data.TotalPaidSalary;
                    if (data.PendingSalary > 0)
                        data.PendingSalary = data.PendingSalary.toFixed(2);
                    if (data.AdvanceSalary > 0)
                        data.AdvanceSalary = data.AdvanceSalary.toFixed(2);
                    if (data.TotalGenratedSalary > 0)
                        data.TotalGenratedSalary = data.TotalGenratedSalary.toFixed(2);
                    if (data.TotalPaidSalary > 0)
                        data.TotalPaidSalary = data.TotalPaidSalary.toFixed(2);
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnSalaryDetailsPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvSalaryDetails').html(html);
                        },
                        error: function (xhr) {

                        },
                        complete: function () {
                            $("#dvSalaryDetails").mLoading("destroy");
                        }
                    })

                }
                else {
                    $('#dvSalaryDetails').html(Resources.DashBoardPermisionMessage);
                }


            }
        })

    }

    return {
        GetAccountsCounts: getAccountsCounts,
        GetIncomeExpenseChart: getIncomeExpenseChart,
        GetCashFlowStatement: getCashFlowStatement,
        GetRecentTransaction: getRecentTransaction,
        GetChequeDetails: getChequeDetails,
        GetIncomeExpenseCounts: getIncomeExpenseCounts,
        GetSalaryDetails: getSalaryDetails
    }

}());



function drawChart() {

    var data = [];
    data = [
        ["Applicants", 12000],
        ["Pre-screened", 4000],
        ["Interviewed", 2500],
        ["Hired", 1500]
    ];


    var options = {

        isCurved: true,
        // bottomPinch: 2,
        fillType: "gradient",
        hoverEffects: true,
        //dynamicArea: true
    }
    var chart = new D3Funnel("#funnel");
    chart.draw(data, options);
};


function FormatAmountDatePicker() {
    var start = moment().startOf('month');
    var end = moment().endOf('month');
    $('#spnReportrange').html("This Month");
    $('#spnIncomeExpenseReportrange').html("This Month");
    $("#hdnIncomeExpenseDateText").val("This Month");
    $('#spnSalaryReportrange').html("This Month");
    $('#reportrange').daterangepicker({
        startDate: start,
        endDate: end,

        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, function (start, end, label) {
        if (label == 'Custom Range')
            label = start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY')
        $('#spnReportrange').html(label);
        EnquiryDashBoard.GetLeadStageFlows();
    });

    $('#ESreportrange').datepicker({
        startView: 2,
        format: 'yyyy',
        minViewMode: 2,
        autoclose: true,

    }).on("changeDate", function (e) {
        $('#hdnEnquirySurveyYear').val(moment(e.date).format("YYYY-MM-DD"));
        $('#ESspnReportrange').html(moment(e.date).format("YYYY"));
        EnquiryDashBoard.GetEnquirySurvey();
    });




    $('#ECreportrange').daterangepicker({
        startDate: start,
        endDate: end,

        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, function (start, end, label) {
        if (label == 'Custom Range')
            label = start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY')
        $('#ECspnReportrange').html(label);
        EnquiryDashBoard.GetEnquiryEmployeeCounts();
    });


    $('#ESStudentCountCourse').datepicker({
        startView: 2,
        format: 'yyyy',
        minViewMode: 2,
        autoclose: true,

    }).on("changeDate", function (e) {
        $('#hdnStudentsCountByCourse').val(moment(e.date).format("YYYY-MM-DD"));
        $('#ESspnStudentCountByCourse').html(moment(e.date).format("YYYY"));
        StudentDashBoard.GetStudentCountByCourse();


    });

    $('#ESStudentCountCourseType').datepicker({
        startView: 2,
        format: 'yyyy',
        minViewMode: 2,
        autoclose: true,

    }).on("changeDate", function (e) {
        $('#hdnStudentsCountByCourseType').val(moment(e.date).format("YYYY-MM-DD"));
        $('#ESspnStudentCountByCourseType').html(moment(e.date).format("YYYY"));
        StudentDashBoard.GetAdmissionByCourseType();
    });




    $('#StudentDiaryreportrange').daterangepicker({
        startDate: start,
        endDate: end,

        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, function (start, end, label) {
        if (label == 'Custom Range')
            label = start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY')
        $('#spnStudentDiaryReportrange').html(label);
        StudentDashBoard.GetStudentDiaryNotes();
    });


    $('#AbsentListreportrange').daterangepicker({
        startDate: start,
        endDate: end,

        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, function (start, end, label) {
        if (label == 'Custom Range')
            label = start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY')
        $('#spnAbsentListReportrange').html(label);
        StudentDashBoard.GetAbsentList();
    });

    $('#NewAdmissionStudentsreportrange').daterangepicker({
        startDate: start,
        endDate: end,

        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, function (start, end, label) {
        if (label == 'Custom Range')
            label = start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY')
        $('#spnNewAdmissionStudentsReportrange').html(label);
        StudentDashBoard.GetNewAdmissionStudents();
    });


    $('#ESCashFlowStatement').datepicker({
        startView: 2,
        format: 'yyyy',
        minViewMode: 2,
        autoclose: true,

    }).on("changeDate", function (e) {
        $('#hdnCashFlowStatement').val(moment(e.date).format("YYYY-MM-DD"));
        $('#ESspnCashFlowStatement').html(moment(e.date).format("YYYY"));
        AccountDashBoard.GetCashFlowStatement();
    });

    $('#ESIncomeExpenseChart').datepicker({
        startView: 2,
        format: 'yyyy',
        minViewMode: 2,
        autoclose: true,

    }).on("changeDate", function (e) {
        $('#hdnIncomeExpenseChart').val(moment(e.date).format("YYYY-MM-DD"));
        $('#ESspnIncomeExpenseChart').html(moment(e.date).format("YYYY"));
        AccountDashBoard.GetIncomeExpenseChart();
    });

    $('#Chequereportrange').daterangepicker({
        startDate: start,
        endDate: end,

        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, function (start, end, label) {
        if (label == 'Custom Range')
            label = start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY')
        $('#spnChequeReportrange').html(label);
        AccountDashBoard.GetChequeDetails();
    });

    $('#IncomeExpensereportrange').daterangepicker({
        startDate: start,
        endDate: end,
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, function (start, end, label) {
        if (label == 'Custom Range')
            label = start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY')
        $('#spnIncomeExpenseReportrange').html(label);
        $("#hdnIncomeExpenseDateText").val(label);
        AccountDashBoard.GetIncomeExpenseCounts();
    });


    //$('#Salaryreportrange').daterangepicker({
    //    startDate: start,
    //    endDate: end,

    //    ranges: {
    //        'Today': [moment(), moment()],
    //        'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
    //        'Last 7 Days': [moment().subtract(6, 'days'), moment()],
    //        'Last 30 Days': [moment().subtract(29, 'days'), moment()],
    //        'This Month': [moment().startOf('month'), moment().endOf('month')],
    //        'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
    //    }
    //}, function (start, end, label) {
    //    if (label == 'Custom Range')
    //        label = start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY')
    //    $('#spnSalaryReportrange').html(label);
    //    AccountDashBoard.GetSalaryDetails();
    //});



    $('#Salaryreportrange').datepicker({
        format: "mm-yyyy",
        startView: "months",
        minViewMode: "months",
        autoclose: true,

    }).on("changeDate", function (e) {
        debugger
        var a = moment(e.date).format("YYYY-MM-DD");
        var b = moment(e.date).format("YYYY-MM");

        $('#hdnsalaryDetails').val(a);
        $('#spnSalaryReportrange').html(b);
        AccountDashBoard.GetSalaryDetails();
    });

    $("#AppUserKey").on("change", function () {
        //EnquiryDashBoard.GetEnquiryEmployeeCounts();
    })
}


