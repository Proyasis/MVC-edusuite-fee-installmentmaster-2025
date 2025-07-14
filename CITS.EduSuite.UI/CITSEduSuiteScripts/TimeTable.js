var TimeTable = (function () {

    var getTimeTable = function () {
        var model = {};

        model.DayKey = $("#tab-day li a.active").data('val');
        model.BranchKey = $("#BranchKey").val();

        $.ajax({
            type: "Get",
            url: $("#hdnUrl").val(),
            contentType: "application/json; charset=utf-8",
            data: model,
            success: function (result) {
                if (result.IsSuccessful == false) {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("#dvTimeTableList").html("")
                $("#dvTimeTableList").html(result);

            },
            error: function (request, status, error) {

            }
        });
    }
    var getEmployeeById = function (_this) {

        var obj = {};
        obj.BranchKey = $("#BranchKey").val();
        obj.ClassDetailsKey = $("[name*=ClassDetailsKey]", _this).val();
        var dropdDownControl = $("[name*=EmployeeKey]", _this);
        $.ajax({
            type: "GET",
            url: $("#hdnFillTeachersByClass").val(),
            dataType: "json",
            data: obj,
            contentType: "application/json; charset=utf-8",
            beforeSend: function () {
                $(dropdDownControl).empty();
                $(dropdDownControl).append($("<option loading></option>").val("").html("<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span>  Loading...</span>"));
                $(dropdDownControl).selectpicker('refresh');

            },
            success: function (response) {
                $(dropdDownControl).append($("<option></option>").val("").html(" "));
                $.each(response, function () {
                    $(dropdDownControl).append($("<option></option>").val(this['RowKey']).html(this['Text']));
                });
                $(dropdDownControl).each(function () {
                    if (parseInt(this.dataset.val))
                        $(this).val(this.dataset.val);
                })




            },
            complete: function () {
                $(dropdDownControl).find("option[loading]").remove();
                $(dropdDownControl).selectpicker('refresh');
                $(".bs-caret", _this).hide();
            }
        })
    }


    function formSubmit() {

        var $form = $("#frmAddEditStudentTimeTable")
        var JsonData = [];
        var formData = $form.serializeToJSON({ associativeArrays: false });
        formData = formData[""].map(function (item) {
            item.Day = $("#tab-day li a.active").data('val');
            item.BranchKey = $("#BranchKey").val();
            return item;
        });
        formData = formData.filter(function (item) {
            return item.EmployeeKey || parseInt(item.RowKey);
        });


        // $form.mLoading();


        //if ($form.valid()) {
        var dataurl = $form.attr("action");
        var response = [];

        AjaxHelper.ajaxAsync("POST", dataurl,
            {
                modelList: formData
            }, function () {
                response = this;
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
                                    TimeTable.GetTimeTable();
                                }
                            }
                        }
                    })

                }
                $form.mLoading("destroy");
            });


    }


    return {
        GetTimeTable: getTimeTable,
        GetEmployeeById: getEmployeeById,
        FormSubmit: formSubmit
    }
}());

function TimeTablePopupLoad() {
    AppCommon.FormatDateInput();
    AppCommon.FormatDayInput();
    TimeTable.ShowHideTimeTableDate();
    $("#TimeTableTypeKey").on("change", function () {
        TimeTable.ShowHideTimeTableDate();
    })
    $("#txtTimeTableFrom,#txtTimeTableTo").on("change", function () {
        var TimeTableTypeKey = $("#TimeTableTypeKey").val();
        if (TimeTableTypeKey == Resources.TimeTableTypeFixed) {
            $(this).next("input").val(moment($(this).val(), ["MMM DD"]).format("DD/MM/YYYY"))
        }
        else {
            $(this).next("input").val($(this).val())
        }

    })

}

