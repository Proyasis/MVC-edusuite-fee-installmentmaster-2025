window.chartColors = {
    red: '#dc9999',
    orange: 'orange',
    yellow: 'yellow',
    green: '#99dc99',
    blue: 'blue',
    purple: 'purple',
    grey: 'grey'
};
var StudentPortal = (function () {


    var getPersonalDetails = function () {
        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;
        $.ajax({
            typ: "Get",
            url: $("#hdnGetPersonalDetails").val(),
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {


                var xmlDoc = $.parseXML(data);
                result = AppCommon.Xml2Json(xmlDoc)
                result = result.PersonalDetails;

                result.FullPath = Resources.FullPath;
                result.ApplicationUrl = Resources.ApplicationUrl.replace("~", Resources.FullPath);

                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnPersonalDetailsPath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(result);
                        $('#dvContent').html(html);
                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })
            }
        })

    }

    var getAttendanceDetaills = function () {

        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ClassDetailsKey = $("#ClassDetailsKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;
        obj.ClassDetailsKey = parseInt(obj.ClassDetailsKey) ? parseInt(obj.ClassDetailsKey) : 0;
        if (obj.ApplicationKey != 0) {

            $.ajax({
                url: $("#hdnAttendanceDetails").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: obj,
                success: function (data) {

                    var xmlDoc = $.parseXML(data);
                    result = AppCommon.Xml2Json(xmlDoc)
                    result = result.AttendacneTotalCount;
                    var PresentPercent = AppCommon.RoundTo((result.Present * 100 / result.TotalCount), Resources.RoundToDecimalPostion)

                    var AbsentPercent = AppCommon.RoundTo((result.Absent * 100 / result.TotalCount), Resources.RoundToDecimalPostion)

                    $("#presentPerc").html(PresentPercent + " %");
                    $("#AbsentPerc").html(AbsentPercent + " %");
                    $("#spnAttendancePercentage").html(PresentPercent);
                    drawdoughnutChartData($("#divAttendanceCount"), [parseFloat(result.Present).toString(), parseFloat(result.Absent).toString()], ["Present (" + PresentPercent + "%)", "Absent (" + AbsentPercent + "%)"], result.ClassDetailsKey)
                    //drawdoughnutBarChartData($("#divUnitTestChart"), [parseFloat(result.Present).toString(), parseFloat(result.Absent).toString()], ["Present (" + PresentPercent + "%)", "Absent (" + AbsentPercent + "%)"], result.ClassDetailsKey)


                }
            });

        }
    }

    var getExamResults = function () {

        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;

        if (obj.ApplicationKey != 0) {

            $.ajax({
                url: $("#hdnGetExamResults").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: obj,
                success: function (data) {

                    var xmlDoc = $.parseXML(data);
                    result = AppCommon.Xml2Json(xmlDoc)
                    result = result.ExamResult;
                    if (result.ExamResultDetails) {


                        var oTblReport = $("#tblAllExamResult")

                        oTblReport.DataTable({
                            data: result.ExamResultDetails,
                            width: '100%',
                            bAutoWidth: false,
                            columns: [
                                { data: "ExamName", title: "ExamName" },
                                { data: "SubjectName", title: "SubjectName" },
                                { data: "Mark", title: "Mark" },
                                { data: "ResultStatus", title: "ResultStatus" },

                            ]
                        });


                        //$('#tblAllExamResult').DataTable({
                        //    data: JSON.stringify(result.ExamResultDetails),
                        //    columns: [
                        //        { title: "ExamName" },
                        //        { title: "SubjectName" },
                        //        { title: "Mark" },
                        //        { title: "ResultStatus" },
                        //    ]
                        //});
                    }
                }
            });

        }
    }

    var getEventDetails = function () {
        var ApplicationKey = $("#ApplicationKey").val();
        ApplicationKey = parseInt(ApplicationKey) ? parseInt(ApplicationKey) : 0;
        var dateNow = new Date();
        var month = moment().month()
        var EventMonth = moment(month).month() + 1;
        var EventDetailsYear = moment(month).year();
        var currentDay = dateNow.getDate();
        var currentDate = new Date(EventDetailsYear, EventMonth - 1, currentDay);
        $('#calendar').fullCalendar('destroy');
        $.ajaxSetup({ async: false });
        $('#calendar').fullCalendar({
            themeSystem: 'bootstrap4',
            displayEventTime: false,
            header: {
                left: 'title',
                center: '',
                right: 'today prev,next'
            },
            events: {
                url: $("#hdnGetAllEvents").val(),
                data: {

                    ApplicationKey: ApplicationKey
                }


            },
            eventRender: function (event, element, view) {

                //if (event.start && !event.start.isBefore(moment()))
                //{
                //var deleteHtml = $('<a/>').html('<i class="fa fa-trash pointer" aria-hidden="true"></i></a>').css(
                //    {
                //        "position": "absolute", "right": "3px", "top": "0", "color": "#fff", "z-index": "2"
                //    }).on("click", function () {
                //        deleteEventDetails(event.eventID);
                //        return false;
                //    });
                //return $(element).addClass("label").append($(deleteHtml));
                //} else
                //{
                //    return $(element);
                //}

            },

            contentHeight: 400,
            defaultDate: currentDate,
            nextDayThreshold: '00:00:00',
            eventLimit: true,
            eventColor: '#378006',
            eventClick: function (calEvent, jsEvent, view) {
                var selectedEvent = calEvent;
                var date = selectedEvent.end ? selectedEvent.end : selectedEvent.start;
                //if (date.isBefore(moment())) {
                //    $('#calendar').fullCalendar('unselect');
                //    return false;
                //}
                //EventDetails.EditPopup(selectedEvent);
            },
            selectable: true,
            dayClick: function (date) {
                var selectedEvent = {
                    eventID: 0,
                    title: '',
                    description: '',
                    start: date,
                    end: date,
                    allDay: false,
                    color: ''
                };
                if (date.isBefore(moment())) {
                    $('#calendar').fullCalendar('unselect');
                    return false;
                }

                $('#calendar').fullCalendar('unselect');
            },
            editable: true

        })
    }

    var getSubjects = function () {

        var obj = {};

        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;

        $('#divSubjects').html("");
        $.ajax({
            type: "GET",
            url: $("#hdnFillApplicationSubjects").val(),
            dataType: "json",
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {

                data = { ApplicationKey: obj.ApplicationKey, Subjects: data };
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnApplicationSubjectsPath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(data);
                        $('#divSubjects').html(html);
                        $("#tab-Subject li a").eq(0).addClass('active');

                        StudentPortal.GetUnitTestExamResult($("#tab-Subject li a.active").data('val'));

                        $('#tab-Subject li a[data-toggle="tab"]').on('shown.bs.tab', function (e) {

                            StudentPortal.GetUnitTestExamResult($("#tab-Subject li a.active").data('val'));
                        });
                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })


            }
        })
    }

    var getUnitTestExamResult = function (SubjectKey) {

        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;
        obj.SubjectKey = parseInt(SubjectKey);
        if (obj.ApplicationKey != 0) {

            $.ajax({
                url: $("#hdnGetUnitExamResults").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: obj,
                success: function (data) {

                    var xmlDoc = $.parseXML(data);
                    result = AppCommon.Xml2Json(xmlDoc)
                    result = result.ExamResult;
                    var labels = result.ExamResultDetails.map(function (item) {
                        return item.ExamDate;
                    })
                    var datas = result.ExamResultDetails.map(function (item) {

                        return parseFloat(item.Mark) ? parseFloat(item.Mark) : 0;
                    })
                    var colors = result.ExamResultDetails.map(function (item) {

                        return item.ResultStatus == "P" ? "#40fb579c" : "#fb4040ad";
                    })
                    drawdoughnutBarChartData($("#divUnitTestChart"), datas, labels, colors, result.ClassDetailsKey)


                }
            });

        }
    }

    var getNotifications = function () {

        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ClassDetailsKey = $("#ClassDetailsKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;
        obj.ClassDetailsKey = parseInt(obj.ClassDetailsKey) ? parseInt(obj.ClassDetailsKey) : 0;
        $('#divStudentNotifications').html("");
        if (obj.ApplicationKey != 0) {

            $.ajax({
                url: $("#hdnGetNotification").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: obj,
                success: function (data) {

                    var xmlDoc = $.parseXML(data);
                    result = AppCommon.Xml2Json(xmlDoc)
                    result = result.Notification;
                    result = result.NotificationResult;

                    data = { ApplicationKey: obj.ApplicationKey, NotificationResult: result };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnStudentNotificationsPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#divStudentNotifications').html(html);

                        },
                        error: function (xhr) {

                        },
                        complete: function () {

                        }
                    })

                }
            });

        }
    }

    var getTotalFeeDetails = function () {

        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ClassDetailsKey = $("#ClassDetailsKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;
        obj.ClassDetailsKey = parseInt(obj.ClassDetailsKey) ? parseInt(obj.ClassDetailsKey) : 0;
        $('#divTotalFeeDetails').html("");
        if (obj.ApplicationKey != 0) {

            $.ajax({
                url: $("#hdnBindTotalFeeDetails").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: obj,
                success: function (data) {

                    var xmlDoc = $.parseXML(data);
                    result = AppCommon.Xml2Json(xmlDoc)
                    result = result.TotalFees;
                    result = result.TotalFeeDetails;

                    data = { ApplicationKey: obj.ApplicationKey, TotalFeeDetails: result };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnStudentTotalFeePath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#divTotalFeeDetails').html(html);

                        },
                        error: function (xhr) {

                        },
                        complete: function () {

                        }
                    })

                }
            });

        }
    }

    var getInstallmentFeeDetails = function () {

        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ClassDetailsKey = $("#ClassDetailsKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;
        obj.ClassDetailsKey = parseInt(obj.ClassDetailsKey) ? parseInt(obj.ClassDetailsKey) : 0;
        $('#divInstallmentDetails').html("");
        if (obj.ApplicationKey != 0) {

            $.ajax({
                url: $("#hdnBindInstallmentFeeDetails").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: obj,
                success: function (data) {
                    var LastIndex = 0;
                    var rowSpan = 0;
                    var xmlDoc = $.parseXML(data);
                    result = AppCommon.Xml2Json(xmlDoc)
                    result = result.TotalInstallmentFees;
                    result = result.TotalInstallmentFeeDetails;
                    if (result) {
                        result.map(function (item, i) {
                            if (i > 0 && item.FeeTypeKey == result[i - 1].FeeTypeKey) {
                                item.FeeTypeName = null;

                            }
                            else {
                                item.RowSpan = result.filter(function (subitem) {
                                    return subitem.FeeTypeKey == item.FeeTypeKey;
                                }).length;
                            }
                            return item;
                        })

                        data = { ApplicationKey: obj.ApplicationKey, TotalInstallmentFeeDetails: result };
                        $.ajax({
                            type: 'GET',
                            crossDomain: true,
                            url: $("#hdnStudentInstallmentFeePath").val() + "?no-cache=" + new Date().getTime(),
                            success: function (response) {
                                AppCommon.HandleBarHelpers();
                                var template = Handlebars.compile(response);
                                var html = template(data);
                                $('#divInstallmentDetails').html(html);

                            },
                            error: function (xhr) {

                            },
                            complete: function () {

                            }
                        })
                    }
                    else
                    {
                        $("#divMainInstallment").hide();
                    }

                }
            });

        }
    }


    var getFeePaymentDetails = function () {

        var obj = {};
        obj.ApplicationKey = $("#ApplicationKey").val();
        obj.ClassDetailsKey = $("#ClassDetailsKey").val();
        obj.ApplicationKey = parseInt(obj.ApplicationKey) ? parseInt(obj.ApplicationKey) : 0;
        obj.ClassDetailsKey = parseInt(obj.ClassDetailsKey) ? parseInt(obj.ClassDetailsKey) : 0;
        $('#divPaymentDetails').html("");
        if (obj.ApplicationKey != 0) {

            $.ajax({
                url: $("#hdnBindFeePaymentDetails").val(),
                type: "GET",
                dataType: "JSON",
                async: false,
                data: obj,
                success: function (data) {

                    var xmlDoc = $.parseXML(data);
                    result = AppCommon.Xml2Json(xmlDoc)
                    result = result.TotalPayment;
                    result = result.PaymentDetails;

                    data = { ApplicationKey: obj.ApplicationKey, PaymentDetails: result };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnStudentFeePaymentPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#divPaymentDetails').html(html);

                        },
                        error: function (xhr) {

                        },
                        complete: function () {

                        }
                    })

                }
            });

        }
    }

    return {
        GetPersonalDetails: getPersonalDetails,
        GetAttendanceDetaills: getAttendanceDetaills,
        GetExamResults: getExamResults,
        GetEventDetails: getEventDetails,
        GetSubjects: getSubjects,
        GetUnitTestExamResult: getUnitTestExamResult,
        GetNotifications: getNotifications,
        GetInstallmentFeeDetails: getInstallmentFeeDetails,
        GetTotalFeeDetails: getTotalFeeDetails,
        GetFeePaymentDetails: getFeePaymentDetails
    }
}());



function drawdoughnutChartData(chartArea, data, labels, eventKey) {

    var doughnutChartData = {
        labels: labels,
        datasets: [{
            backgroundColor: ["#304ffe", "#ffa601"],
            data: data,
            label: "Total Students"
        }, ]

    };
    var doughnutChartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        cutoutPercentage: 65,
        rotation: -9.4,
        animation: {
            duration: 2000
        },
        legend: {
            display: false
        },
        tooltips: {
            enabled: true
        },
    };
    var studentCanvas = $("#student-doughnut-chart").get(0).getContext("2d");
    var studentChart = new Chart(studentCanvas, {
        type: 'doughnut',
        data: doughnutChartData,
        options: doughnutChartOptions
    });
}


function drawdoughnutBarChartData(chartArea, data, labels, colors, eventKey) {
    if (window.expenseChart)
        window.expenseChart.destroy();
    var barChartData = {
        labels: labels,
        datasets: [{
            backgroundColor: colors,
            data: data,

        }, ]
    };
    var barChartOptions = {
        maintainAspectRatio: false,
        animation: {
            duration: 2000
        },
        legend: {
            display: false
        },
        scales: {

            xAxes: [{
                display: true,
                ticks: {
                    display: true,
                    padding: 0,
                    fontColor: "#646464",
                    fontSize: 14,
                },
                gridLines: {
                    display: true,
                    color: '#e1e1e1',
                }
            }],
            yAxes: [{
                display: true,
                ticks: {
                    display: true,
                    autoSkip: false,
                    fontColor: "#646464",
                    beginAtZero: true,
                    callback: function (value) {
                        var ranges = [{
                            divider: 1e6,
                            suffix: 'M'
                        },
                          {
                              divider: 1e3,
                              suffix: 'k'
                          }
                        ];

                        function formatNumber(n) {
                            for (var i = 0; i < ranges.length; i++) {
                                if (n >= ranges[i].divider) {
                                    return (n / ranges[i].divider).toString() + ranges[i].suffix;
                                }
                            }
                            return n;
                        }
                        return formatNumber(value);
                    }
                }
            }]
        },
        elements: {}
    };
    var expenseCanvas = $("#expense-bar-chart").get(0).getContext("2d");
    window.expenseChart = new Chart(expenseCanvas, {
        type: 'horizontalBar',
        data: barChartData,
        options: barChartOptions
    });
}