var Holiday = (function () {
    var getHolidays = function () {
        var dateNow = new Date();
        var month = AppCommon.ParseMMMYYYYDate($("input#HolidayMonth").val());
        var holidayMonth = moment(month).month() + 1;
        var holidayYear = moment(month).year();
        var currentDay = dateNow.getDate();
        var currentDate = new Date(holidayYear, holidayMonth - 1, currentDay);
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
                url: $("#hdnGetHolidays").val(),
                data: {

                    BranchKey: function () {
                        return $("#BranchKey").val();
                    }
                }


            },
            eventRender: function (event, element, view) {

                //if (event.start && !event.start.isBefore(moment()))
                //{
                var deleteHtml = $('<a/>').html('<i class="fa fa-trash pointer" aria-hidden="true"></i></a>').css(
                    {
                        "position": "absolute", "right": "3px", "top": "0", "color": "#fff", "z-index": "2"
                    }).on("click", function () {
                        deleteHoliday(event.eventID);
                        return false;
                    });
                return $(element).addClass("label").append($(deleteHtml));
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
                Holiday.EditPopup(selectedEvent);
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
                //if (date.isBefore(moment())) {
                //    $('#calendar').fullCalendar('unselect');
                //    return false;
                //}
                //else {

                var row = $(this).closest('div.fc-row');
                var index = $(this)[0].cellIndex
                var event = AjaxHelper.ajax("POST", $("#hdnGetHolidayByDate").val(),
                    {
                        Date: moment(date),
                        BranchKey: $("#BranchKey").val()
                    });
                if (event) {
                    selectedEvent = event;

                }
                Holiday.EditPopup(selectedEvent);
                //}

                $('#calendar').fullCalendar('unselect');
            },
            editable: true

        })
    }

    var showHideHolidayDate = function () {
        var HolidayTypeKey = $("#HolidayTypeKey").val();
        HolidayTypeKey = parseInt(HolidayTypeKey) ? parseInt(HolidayTypeKey) : 0;
        if (HolidayTypeKey == Resources.HolidayTypeFixed) {
            $("#txtHolidayFrom,#txtHolidayTo").attr("data-input-type", "day").attr("readonly", true);
            $("#txtHolidayFrom,#txtHolidayTo").datepicker("remove")
            $("#txtHolidayFrom,#txtHolidayTo").inputmask("remove")
            AppCommon.FormatDayInput();
        }
        else {
            $("#txtHolidayFrom,#txtHolidayTo").attr("data-input-type", "date").removeAttr("readonly");
            $("#txtHolidayFrom,#txtHolidayTo").datepicker("remove")
            AppCommon.FormatDateInput();
        }
    }
    function editPopup(selectedEvent) {
        var obj = {};
        if (selectedEvent.eventID != 0) {
            selectedEvent.start = new Date();
        }
        obj.id = selectedEvent.eventID;
        obj.Date = selectedEvent.start;
        obj.branchKey = $("#BranchKey").val();
        /*var url = $("#hdnAddEditHoliday").val() + "/" + selectedEvent.eventID + "?branchKey=" + $("#BranchKey").val();*/
        var url = $("#hdnAddEditHoliday").val() + '?' + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-sm",
            load: function () {
                setTimeout(function () {
                    HolidayPopupLoad()
                }, 500)

            },
            rebind: function () {
                Holiday.GetHolidays();
            }
        }, url);
    }

    function deleteHoliday(rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Holiday,
            actionUrl: $("#hdnDeleteHoliday").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                Holiday.GetHolidays();
            }
        });
    }
    return {
        GetHolidays: getHolidays,
        ShowHideHolidayDate: showHideHolidayDate,
        EditPopup: editPopup
    }
}());

function HolidayPopupLoad() {
    AppCommon.FormatDateInput();
    AppCommon.FormatDayInput();
    Holiday.ShowHideHolidayDate();
    $("#HolidayTypeKey").on("change", function () {
        Holiday.ShowHideHolidayDate();
    })
    $("#txtHolidayFrom,#txtHolidayTo").on("change", function () {
        var HolidayTypeKey = $("#HolidayTypeKey").val();
        if (HolidayTypeKey == Resources.HolidayTypeFixed) {
            $(this).next("input").val(moment($(this).val(), ["MMM DD"]).format("DD/MM/YYYY"))
        }
        else {
            $(this).next("input").val($(this).val())
        }

    })

}

