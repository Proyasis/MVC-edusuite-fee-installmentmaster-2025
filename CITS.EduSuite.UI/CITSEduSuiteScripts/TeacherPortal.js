var TeacherPortal = (function () {

    var getClasses = function () {
        $("#dvSaveButton").hide();
        var obj = {};
        obj.BranchKey = $("#BranchKey").val();
        obj.BranchKey = parseInt(obj.BranchKey) ? parseInt(obj.BranchKey) : 0;
        $('#dvContent').html("");
        $.ajax({
            type: "GET",
            url: $("#hdnFillClasses").val(),
            dataType: "json",
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.length == 1) {
                    TeacherPortal.GetMenus(obj.BranchKey, data[0].RowKey, data[0].Text)
                } else {
                    data = { BranchKey: obj.BranchKey, Classes: data };
                    $.ajax({
                        type: 'GET',
                        crossDomain: true,
                        url: $("#hdnClassesPath").val() + "?no-cache=" + new Date().getTime(),
                        success: function (response) {
                            AppCommon.HandleBarHelpers();
                            var template = Handlebars.compile(response);
                            var html = template(data);
                            $('#dvContent').html(html);
                            $("#lnkBackToPortal").hide()

                        },
                        error: function (xhr) {

                        },
                        complete: function () {

                        }
                    })
                }

            }
        })
    }
    var getMenus = function (BranchKey, ClassKey, ClassName) {
        var data = {};
        data.ClassDetailsKey = ClassKey;
        data.ClassName = ClassName;
        data.BranchKey = BranchKey;
        $('#dvContent').html("");
        $('#dvContent').mLoading();
        $.ajax({
            type: 'GET',
            crossDomain: true,
            url: $("#hdnMenuPath").val() + "?no-cache=" + new Date().getTime(),
            success: function (response) {
                AppCommon.HandleBarHelpers();
                var template = Handlebars.compile(response);
                var html = template(data);
                $('#dvContent').html(html);
                $("#AttendanceDate").val(moment().format("DD/MM/YYYY"));
                AppCommon.FormatDateInput();
                setBackTo(1, BranchKey, ClassKey, ClassName);
                Colorswap();
            },
            error: function (xhr) {

            },
            complete: function () {

            }
        })



    }
    var getAttendances = function (BranchKey, ClassKey, ClassName) {
        var obj = {};
        obj.BranchKey = BranchKey;
        obj.BranchKey = parseInt(obj.BranchKey) ? parseInt(obj.BranchKey) : 0;
        obj.ClassDetailsKey = ClassKey;
        obj.ClassDetailsKey = parseInt(obj.ClassDetailsKey) ? parseInt(obj.ClassDetailsKey) : 0;
        obj.AttendanceDate = $("#AttendanceDate").val();
        obj.ClassName = ClassName;
        $('#dvContent').html("");
        $('#dvContent').mLoading();
        $.ajax({
            type: "Get",
            url: $("#hdnAttendance").val(),
            //data: obj,
            success: function (responseHtml) {
                $('#dvContent').html(responseHtml);
                $("header.section-header", $('#dvContent')).remove();
                $('#dvContent').mLoading();
                setTimeout(function () {
                    $("#ClassDetailsKey").val(obj.ClassDetailsKey);
                    var dv = $('#dvContent');
                    var cls = $("#ClassDetailsKey");
                    var classkeyss = $("#ClassDetailsKey").val();
                    $("#ClassDetailsKey", $('#dvContent')).val(obj.ClassDetailsKey).selectpicker("refresh").trigger("change");
                    $("#ClassDetailsKey", $('#dvContent')).closest("[class*=col]").hide();
                    setBackTo(2, BranchKey, ClassKey, ClassName, "#btnSave");
                    $('#dvContent').mLoading("destroy");
                }, 500)


                //$.ajax({
                //    type: 'GET',
                //    crossDomain: true,
                //    url: $("#hdnAttendancePath").val(),
                //    success: function (response) {
                //        AppCommon.HandleBarHelpers();
                //        var template = Handlebars.compile(response);
                //        var html = template(obj);
                //        $('#dvContent').html(html);
                //        $('#dvAttendanceContent').prepend(responseHtml);

                //        $(".btn-attendance").each(function () {
                //            $(this).css("color", AppCommon.SetColorByBackgroundIntensity($(this).css("background-color")));
                //        });
                //        Attendance.SetAttendanceTableWidth();

                //        $("#btnSave").on("click", function () {

                //            Attendance.FormSubmit(function () {
                //                TeacherPortal.GetMenus(BranchKey, ClassKey, ClassName);

                //            });
                //        })

                //    },
                //    error: function (xhr) {

                //    },
                //    complete: function () {

                //    }
                //})

            }
        })

    }
    var workUpdateClick = function (BranchKey, ClassKey, ClassName) {
        if (Resources.UserIsTeacher) {
            TeacherPortal.WorkUpdatePopup(BranchKey, ClassKey, ClassName, "A", Resources.EmployeeKey)
        }
        else {
            TeacherPortal.TeachersPopup(BranchKey, ClassKey, ClassName)
        }
    }
    var workUpdatePopup = function (BranchKey, ClassKey, ClassName, EmployeeCode, EmployeeKey) {
        var obj = {};
        obj.EmployeeKey = EmployeeKey;
        var URL = $("#hdnAddEditWorkSchedule").val() + "?" + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function (form) {

                setTimeout(function () {
                    $("#ClassDetailsKey", form).val(ClassKey).selectpicker("refresh").trigger("change");
                    $("#ClassDetailsKey", form).closest("[class*=col]").hide();
                    $(".card-header .form-row", form).append('<button type="button" class="modal-close" data-dismiss="modal" aria-label="Close"><i class="fa fa-close"></i></button>')
                    $(".modal-close", form).show();

                }, 500)
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    toastr.success(Resources.Success, result.Message)
                } else {
                    toastr.error(Resources.Failed, result.Message)
                }
                TeacherPortal.GetMenus(BranchKey, ClassKey, ClassName);

            }
        }, URL);
    }
    var teachersPopup = function (BranchKey, ClassKey, ClassName) {
        var obj = {};
        obj.BranchKey = BranchKey;
        obj.ClassDetailsKey = ClassKey;
        var URL = $("#hdnFillTeachersByClass").val();
        $.ajax({
            type: "GET",
            url: $("#hdnFillTeachersByClass").val(),
            dataType: "json",
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                data = { "Teachers": data }
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnTeachersPath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(data);
                        $.customPopupform.CustomPopup({
                            html: html,
                            modalsize: "modal-md",
                            load: function (dialog) {
                                setTimeout(function () {
                                    $(".class_box a", dialog).on("click", function () {
                                        var value = $(this).data("val");
                                        TeacherPortal.WorkUpdatePopup(BranchKey, ClassKey, ClassName, null, value)
                                        $(dialog).closest(".modal").modal('hide');
                                    });

                                }, 500);
                            },
                            rebind: function (result) {



                            }
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
    var getUnitTest = function (BranchKey, ClassKey, ClassName) {
        var obj = {};
        obj.BranchKey = BranchKey;
        obj.BranchKey = parseInt(obj.BranchKey) ? parseInt(obj.BranchKey) : 0;
        obj.ClassDetailsKey = ClassKey;
        obj.ClassDetailsKey = parseInt(obj.ClassDetailsKey) ? parseInt(obj.ClassDetailsKey) : 0;
        obj.ExamDate = $("#AttendanceDate").val();
        obj.ClassName = ClassName;
        $('#dvContent').html("");
        $('#dvContent').mLoading();
        $.ajax({
            type: "Get",
            url: $("#hdnUnitTest").val(),
            //data: obj,
            success: function (responseHtml) {
                $('#dvContent').html(responseHtml);
                $('#dvContent').mLoading();
                $("header.section-header", $('#dvContent')).remove();
                setTimeout(function () {
                    //$("#ExamDate", $('#dvContent')).val(obj.ExamDate)
                    $("#ClassDetailsKey", $('#dvContent')).val(ClassKey).selectpicker("refresh").trigger("change");
                    $("#ClassDetailsKey", $('#dvContent')).closest("[class*=col]").hide();


                    setBackTo(2, BranchKey, ClassKey, ClassName, "#btnSave");
                    $('#dvContent').mLoading("destroy");
                }, 500)


            }
        })

    }
    var getStudents = function (BranchKey, ClassKey, ClassName, type) {

        var obj = {};
        obj.ClassDetailsKey = ClassKey;
        obj.ClassName = ClassName;
        obj.BranchKey = BranchKey;

        $('#dvContent').html("");
        $('#dvContent').mLoading();
        $.ajax({
            type: "Get",
            url: $("#hdnGetStudents").val(),
            data: obj,
            success: function (data) {
                var resultData = {};
                resultData.FullPath = Resources.FullPath;
                resultData.ApplicationUrl = Resources.ApplicationUrl.replace("~", Resources.FullPath);
                resultData.Students = data;
                resultData.type = type;
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnStudentsFilePath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(resultData);
                        $('#dvContent').html(html);
                        setBackTo(2, BranchKey, ClassKey, ClassName);
                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })
            }
        });



    }
    var diaryUpdatePopup = function (BranchKey, ClassKey, ClassName, ApplicationKey) {
        var obj = {};
        obj.ApplicationKey = ApplicationKey;
        var URL = $("#hdnAddEditStudentDiary").val() + "?" + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function (form) {

                setTimeout(function () {
                    $(".modal-close", form).show();

                }, 500)
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    toastr.success(Resources.Success, result.Message)
                } else {
                    toastr.error(Resources.Failed, result.Message)
                }
                TeacherPortal.GetMenus(BranchKey, ClassKey, ClassName);

            }
        }, URL);
    }
    var viewStudent = function (BranchKey, ClassKey, ClassName, ApplicationKey) {
        var obj = {};
        obj.id = ApplicationKey;
        var URL = $("#hdnViewStudent").val() + "?" + $.param(obj);
        //$.ajax({
        //    type: "Get",
        //    url: URL,
        //    success: function (data) {
        //        $('#dvContent').html(data);
        //    }
        //});
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg w-100",
            load: function (form) {

                setTimeout(function () {
                    $(".breadcrumb", form).replaceWith('<button type="button" class="modal-close" style="margin-top: -15px;" data-dismiss="modal" aria-label="Close"><i class="fa fa-close"></i></button>');

                }, 500)
            }
        }, URL);

    }
    var progressCard = function (RowKey, BranchKey, ClassKey, ClassName, ApplicationKey) {
        $('#dvContent').mLoading();
        //var obj = {};
        //obj.id = ApplicationKey;

        ////$('#dvContent').html("");
        //$('#dvContent').mLoading();
        //$.ajax({
        //    type: "Get",
        //    url: $("#hdnViewProgressCard").val(),
        //    data: obj,
        //    success: function (data) {
        //        var xmlDoc = $.parseXML(data);
        //        data = AppCommon.Xml2Json(xmlDoc)

        //        var resultData = {};
        //        resultData.FullPath = Resources.FullPath;
        //        resultData.ApplicationUrl = Resources.ApplicationUrl.replace("~", Resources.FullPath);
        //        resultData.ProgressCard = data.ProgressCard;

        //        $.ajax({
        //            type: 'GET',
        //            crossDomain: true,
        //            url: $("#hdnProgressCardFilePath").val() + "?no-cache=" + new Date().getTime(),
        //            success: function (response) {
        //                AppCommon.HandleBarHelpers();
        //                ProgressCardHelper();
        //                var template = Handlebars.compile(response);
        //                var html = template(resultData);
        //                $('#dvContent').mLoading("destroy");
        //                $.customPopupform.CustomPopup({
        //                    html: html,
        //                    modalsize: "modal-lg w-100",
        //                    load: function (form) {

        //                        setTimeout(function () {
        //                            $(".breadcrumb", form).replaceWith('<button type="button" class="modal-close" style="margin-top: -15px;" data-dismiss="modal" aria-label="Close"><i class="fa fa-close"></i></button>');

        //                        }, 500)
        //                    }
        //                });
        //                ////$('#dvContent').html(html);
        //                //setBackTo(2, BranchKey, ClassKey, ClassName);
        //            },
        //            error: function (xhr) {

        //            },
        //            complete: function () {

        //            }
        //        })
        //    }
        //});
        var ParentMeetScheduleKey;
        var obj = {};
        obj.ApplicationKey = ApplicationKey;
        obj.id = RowKey;
        var URL = $("#hdnAddEditParentsMeetDetails").val();
        $.ajax({
            type: "Get",
            url: URL,
            data: obj,
            success: function (data) {
                var meetingHtml = data;
                var obj = {};
                obj.id = ApplicationKey;
                var URL = $("#hdnViewStudent").val();
                $.ajax({
                    type: "Get",
                    url: URL,
                    data: obj,
                    success: function (data) {
                        data = data + meetingHtml;
                        //+ "<div id='signature-pad' class='my-3'><canvas style='border:1px solid;margin:0 10%;'/>"
                        //+ '<button type="button" class="btn btn-danger btn-sm clearButton"><i class="fa fa-close"></i></button></div>'
                        //+ "<textarea id='txtRemarks' class='form-control input-control'></textarea>"

                        $.customPopupform.CustomPopup({
                            html: data,
                            modalsize: "modal-lg w-100",
                            //footerText: '<div class="form-group"><input type="button" value="Save" id="btnSave" class="btn btn-sm btn-success mx-1" /><button type="button" data-dismiss="modal" class="btn btn-sm btn-danger mx-1">' + Resources.Cancel + '</button></div>',
                            load: function (form) {
                                $('#dvContent').mLoading("destroy");
                                //setTimeout(function () {
                                $(".breadcrumb", form).replaceWith('<button type="button" class="modal-close" style="margin-top: -15px;" data-dismiss="modal" aria-label="Close"><i class="fa fa-close"></i></button>');
                                Signature();
                                $("#btnSave").on("click", function () {
                                    var $form = $("#frmAddEditParentsMeetDetails");
                                    var formData = $form.serializeToJSON({ associativeArrays: false });

                                    var canvas = $("#signature-pad").find("canvas")[0];
                                    var dataURL = canvas.toDataURL();
                                    var blob = AppCommon.DataURItoBlob(dataURL);
                                    var file = AppCommon.BlobToFile(blob, 'signature', '.png');
                                    formData.ParentMeetSignFile = file;
                                    var dataurl = $form.attr("action");
                                    AjaxHelper.ajaxWithFileAsync("model", "POST", dataurl,
                                        {
                                            model: formData
                                        }, function () {
                                            response = this;
                                            if (response.IsSuccessful) {
                                                ParentMeetScheduleKey = formData.ParentMeetScheduleKey;
                                                toastr.success(Resources.Success, response.Message)
                                                $("#frmAddEditParentsMeetDetails").closest(".modal").modal("hide");
                                                TeacherPortal.GetStudentsForParentsMeet(BranchKey, ParentMeetScheduleKey, ClassKey, ClassName, 3);
                                            } else {
                                                toastr.error(Resources.Failed, response.Message)
                                            }

                                        });

                                })


                                //}, 500)
                            },
                            //rebind: function (result) {
                            //    if (result.IsSuccessful) {
                            //        toastr.success(Resources.Success, result.Message)
                            //    } else {
                            //        toastr.error(Resources.Failed, result.Message)
                            //    }
                            //    setBackTo(3, BranchKey, ClassKey, ClassName, "#btnSave");

                            //}

                        });


                    }
                });

            }
        });








    }
    var studentClick = function (type, BranchKey, ClassKey, ClassName, ApplicationKey) {
        switch (parseInt(type)) {
            case 1://View student
                TeacherPortal.ViewStudent(BranchKey, ClassKey, ClassName, ApplicationKey);
                break;
            case 2://Dairy
                TeacherPortal.DiaryUpdatePopup(BranchKey, ClassKey, ClassName, ApplicationKey);
                break;
            case 3://Parents Meeting
                TeacherPortal.ProgressCard(BranchKey, ClassKey, ClassName, ApplicationKey);
                break;
        }

    }

    var getParentsMeetSchedule = function (BranchKey, ClassKey, ClassName) {

        var obj = {};
        obj.ClassDetailsKey = ClassKey;
        obj.ClassName = ClassName;
        obj.BranchKey = BranchKey;

        $('#dvContent').html("");
        $('#dvContent').mLoading();
        $.ajax({
            type: "Get",
            url: $("#hdnGetParentsMeetSchedule").val(),
            data: obj,
            success: function (data) {
                var resultData = {};
                //resultData.ParentsMeetSchedules = data;

                resultData.ParentsMeetSchedules = data.map(function (item) {
                    if (item.MeettingDate)
                        item.MeettingDate = moment(item.MeettingDate).format("DD/MM/YYYY");
                    if (item.MeettingTime)
                        item.MeettingTime = AppCommon.FormatObjectToTimeAMPM(item.MeettingTime);
                    item.BranchKey = BranchKey;
                    item.ClassName = ClassName;
                    return item;
                });
                //resultData.type = type;
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnGetParentsMeetScheduleFilePath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(resultData);
                        $('#dvContent').html(html);
                        setBackTo(2, BranchKey, ClassKey, ClassName);
                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })
            },

        });



    }

    var getStudentsForParentsMeet = function (BranchKey, RowKey, ClassKey, ClassName, type) {
        var obj = {};
        obj.ClassDetailsKey = ClassKey;
        obj.ClassName = ClassName;
        obj.RowKey = RowKey;
        obj.BranchKey = BranchKey;

        $('#dvContent').html("");
        $('#dvContent').mLoading();
        $.ajax({
            type: "Get",
            url: $("#hdnGetStudentsForParentsMeet").val(),
            data: obj,
            success: function (data) {
                var resultData = {};
                resultData.FullPath = Resources.FullPath;
                resultData.ApplicationUrl = Resources.ApplicationUrl.replace("~", Resources.FullPath);
                resultData.Students = data;
                resultData.type = type;
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: $("#hdnParentsMeetStudentsFilePath").val() + "?no-cache=" + new Date().getTime(),
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        var html = template(resultData);
                        $('#dvContent').html(html);
                        setBackTo(3, BranchKey, ClassKey, ClassName);
                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })
            }
        });



    }

    var employeeTimeTable = function () {
        var href = $("#hdnEmployeeTimeTable").val() + "/"

        window.location.href = href;
    }

    var employeeEvents = function () {
        var href = $("#hdnEventsScheduledList").val() + "/"

        window.location.href = href;
    }
    var getEventDetails = function () {
        var dateNow = new Date();
        var month = AppCommon.ParseMMMYYYYDate($("input#EventMonth").val());
        var EventMonth = moment(month).month() + 1;
        var EventDetailsYear = moment(month).year();
        var currentDay = dateNow.getDate();
        var currentDate = new Date(EventDetailsYear, EventMonth - 1, currentDay);
        $('#calender').fullCalendar('destroy');
        $.ajaxSetup({ async: false });
        $('#calender').fullCalendar({
            themeSystem: 'bootstrap4',
            displayEventTime: false,
            header: {
                left: 'title',
                center: '',
                right: 'today prev,next'
            },
            events: {
                url: $("#hdnGetEventDetails").val(),
                data: {

                    BranchId: function () {
                        return $("#ddlBranch").val();
                    }
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
                //else {

                //    var row = $(this).closest('div.fc-row');
                //    var index = $(this)[0].cellIndex
                //    var event = AjaxHelper.ajax("POST", $("#hdnGetEventDetailsByDate").val(),
                //        {
                //            Date: moment(date),
                //            BranchId: $("#ddlBranch").val()
                //        });
                //    if (event) {
                //        selectedEvent = event;

                //    }
                //    EventDetails.EditPopup(selectedEvent);
                //}

                $('#calendar').fullCalendar('unselect');
            },
            editable: true

        })
    }

    var getWorkUpdates = function (BranchKey, ClassKey, ClassName) {
        var obj = {};
        obj.BranchKey = BranchKey;
        obj.BranchKey = parseInt(obj.BranchKey) ? parseInt(obj.BranchKey) : 0;
        obj.ClassDetailsKey = ClassKey;
        obj.ClassDetailsKey = parseInt(obj.ClassDetailsKey) ? parseInt(obj.ClassDetailsKey) : 0;
        obj.ExamDate = $("#AttendanceDate").val();
        obj.ClassName = ClassName;
        $('#dvContent').html("");
        $('#dvContent').mLoading();
        $.ajax({
            type: "Get",
            url: $("#hdnWorkUpdates").val(),
            //data: obj,
            success: function (responseHtml) {
                $('#dvContent').html(responseHtml);
                $('#dvContent').mLoading();
                $("header.section-header", $('#dvContent')).remove();
                setTimeout(function () {
                    //$("#ExamDate", $('#dvContent')).val(obj.ExamDate)
                    $("#ClassDetailsKey", $('#dvContent')).val(ClassKey).selectpicker("refresh").trigger("change");
                    $("#ClassDetailsKey", $('#dvContent')).closest("[class*=col]").hide();


                    setBackTo(2, BranchKey, ClassKey, ClassName, "#btnSave");
                    $('#dvContent').mLoading("destroy");
                }, 500)


            }
        })

    }
    return {
        GetClasses: getClasses,
        GetMenus: getMenus,
        GetAttendances: getAttendances,
        WorkUpdateClick: workUpdateClick,
        WorkUpdatePopup: workUpdatePopup,
        TeachersPopup: teachersPopup,
        GetUnitTest: getUnitTest,
        GetStudents: getStudents,
        DiaryUpdatePopup: diaryUpdatePopup,
        ViewStudent: viewStudent,
        StudentClick: studentClick,
        ProgressCard: progressCard,
        GetParentsMeetSchedule: getParentsMeetSchedule,
        GetStudentsForParentsMeet: getStudentsForParentsMeet,
        EmployeeTimeTable: employeeTimeTable,
        GetEventDetails: getEventDetails,
        EmployeeEvents: employeeEvents,
        GetWorkUpdates: getWorkUpdates
    }

}());
function setBackTo(type, BranchKey, ClassKey, ClassName, SaveButton) {

    if (type == 1) {
        $("#spnBackToPortal").html("Class")
        $("#lnkBackToPortal").off("click").on("click", function () {
            TeacherPortal.GetClasses();
        }).show();
    } else if (type == 2) {
        $("#spnBackToPortal").html("Menu")
        $("#lnkBackToPortal").off("click").on("click", function () {
            TeacherPortal.GetMenus(BranchKey, ClassKey, ClassName);
        }).show();
        $(SaveButton).on("click", function () {
            localStorage.setItem('teacherPortalRebind', "TeacherPortal.GetMenus(" + BranchKey + ", " + ClassKey + ",'" + ClassName + "')");

        })

    }
    else if (type == 3) {
        $("#spnBackToPortal").html("Parents Meet")
        $("#lnkBackToPortal").off("click").on("click", function () {
            TeacherPortal.GetParentsMeetSchedule(BranchKey, ClassKey, ClassName);
        }).show();
        $(SaveButton).on("click", function () {
            localStorage.setItem('teacherPortalRebind', "TeacherPortal.GetParentsMeetSchedule(" + BranchKey + ", " + ClassKey + ",'" + ClassName + "')");

        })

    }
    $("#spnClassName").html(" - " + ClassName)
}

function ProgressCardHelper() {
    Handlebars.registerHelper('attendanceChart', function (data) {
        return "";
        //return drawPieChart([parseFloat(data.Present).toString(), parseFloat(data.Absent).toString()], ["Present" + "(" + parseFloat(data.Present).toString() + ")", "Absent" + "(" + parseFloat(data.Absent).toString() + ")"]);
    });
    Handlebars.registerHelper('compareChart', function (data) {
        //var dataset = [];
        //var Labels = [];
        //var InternalExamIds = [];
        //data.forEach(function (item) {
        //    if (Labels.indexOf(item.BookCode) == -1) {
        //        Labels.push(item.BookCode.substring(0, 25));
        //    }
        //    if (InternalExamIds.indexOf(item.InternalExamId) == -1) {
        //        InternalExamIds.push(item.InternalExamId);
        //    }

        //});

        //$(InternalExamIds).each(function (i, item) {
        //    var obj = {};
        //    obj.label = data.filter(function (key) {
        //        return key.InternalExamId == item;
        //    }).map(function (key) {
        //        return key.InternalExamTermName;
        //    })[0];
        //    obj.data = data.filter(function (key) {
        //        return key.InternalExamId == item;
        //    }).map(function (key) {
        //        return key.Percentage;
        //    });
        //    obj.backgroundColor = Samples.utils.color(i);

        //    dataset.push(obj)
        //})
        //return drawBarChart(dataset, Labels);
        return "";
    });
}

function drawPieChart(data, labels) {

    var config = {
        type: 'pie',
        data: {
            datasets: [{
                data: data,
                backgroundColor: [

                    window.chartColors.green,
                    window.chartColors.red
                ],
                label: 'Attendance'
            }],
            labels: labels
        },
        options: {
            responsive: false,
            bezierCurve: false,
            animation: false,
            showDatapoints: true,
            maintainAspectRatio: false,
            legend: {
                display: true,
                position: 'bottom',
                labels: {
                    fontSize: 30,
                    fontColor: "black",
                    fontStyle: "bold",
                }
            },
            layout: {
                padding: {
                    left: 50,
                    right: 0,
                    top: 0,
                    bottom: 0
                }
            }
        }
    };

    var chartContainer = $("<div height='600' width='600'/>")
    var chartArea = $("<canvas height='600' width='600'/>")
    $(chartContainer).append(chartArea)
    $("body").append(chartContainer)
    var ctx = chartArea[0].getContext('2d');
    window.PieChart = new Chart(ctx, config);
    $(chartContainer).remove();
    return window.PieChart.toBase64Image();




}

function drawBarChart(data, labels) {
    var chartContainer = $("<div/>")
    var chartArea = $("<canvas width='1360'  height='350'/>")
    $(chartContainer).append(chartArea)
    $("body").append(chartContainer)
    var ctx = chartArea[0].getContext('2d');
    var data = {
        labels: labels,
        datasets: data
    };
    var options = {
        responsive: false,
        bezierCurve: false,
        animation: false,
        showDatapoints: true,
        maintainAspectRatio: false,
        legend: {
            display: true,
            position: 'bottom',
            labels: {
                fontSize: 14,
                fontColor: "black",
                fontStyle: "bold",
            }
        },
        //annotation: {
        //    annotations: [{
        //        type: 'line',
        //        mode: 'horizontal',
        //        scaleid: 'y-axis-0',
        //        value: monthlyminimumtarget,
        //        bordercolor: '#00800075',
        //        borderwidth: 2,
        //        label: {
        //            enabled: false,
        //            content: 'target',
        //            fontcolor: "black",
        //            backgroundcolor: "#00800075",
        //            position: "left"
        //        }
        //    }]
        //},
        animation: false,

        scales: {
            xAxes: [{
                ticks: {

                    autoSkip: false,
                    fontColor: "#000",
                    fontSize: 13
                },
                barPercentage: 1,
                maxBarThickness: 200,
                categoryPercentage: 0.3,
                beginAtZero: true,
                gridLines: {
                    display: false,
                    color: "#000"
                },
                afterFit: function (scaleInstance) {
                    scaleInstance.width = 100; // sets the width to 100px
                }
            }],
            yAxes: [{
                ticks: {
                    stepSize: 20,
                    fontColor: "#000",
                    fontSize: 13,
                    suggestedMax: 100,
                    beginAtZero: true,
                    callback: function (value, index, values) {
                        if (Math.floor(value) === value) {
                            return value;
                        }
                    }
                },
                gridLines: {
                    display: false,
                    color: "#000"
                },
            }]
        }
    };

    window.BarChart = new Chart(ctx, {
        type: 'bar',
        data: data,
        options: options
    });
    $(chartContainer).remove();
    return window.BarChart.toBase64Image();

}

function Signature() {
    $('#signature-pad').signaturePad({ drawOnly: true, lineTop: 200 });

}

function Colorswap() {
    //var colorArray = ['#FF6633', '#FFB399', '#FF33FF', '#FFFF99', '#00B3E6',
    //     '#E6B333', '#3366E6', '#B34D4D',
    //     '#80B300', '#809900', '#E6B3B3', '#6680B3', '#66991A',
    //     '#FF99E6', '#FF1A66', '#E6331A', '#33FFCC',
    //     '#66994D', '#B366CC', '#4D8000', '#B33300', '#CC80CC',
    //     '#66664D', '#991AFF', '#E666FF', '#4DB3FF', '#1AB399',
    //     '#E666B3', '#33991A', '#CC9999', '#00E680',
    //     '#4D8066', '#809980', '#E6FF80', '#999933',
    //     '#FF3380', '#CCCC00', '#66E64D', '#4D80CC', '#9900B3',
    //     '#E64D66', '#4DB380', '#FF4D4D', '#99E6E6', '#6666FF'];
    var colorArray = ['#9400D3', '#4B0082', '#0000FF', '#00FF00', '#FFFF00', '#FF7F00', '#FF0000'].sort(() => .5 - Math.random());

    $(".teacher-portal .box").each(function (i) {
        var color = colorArray[i];
        $(this).css("background-color", color)
        $(this).css("color", AppCommon.SetColorByBackgroundIntensity(color))
    })

}