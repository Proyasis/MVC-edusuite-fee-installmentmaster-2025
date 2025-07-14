var WorkSchedule = (function () {

    var loadData = function () {
        var model = {};
        model.SubjectKey = $("#SubjectKey").val();
        model.BatchKey = $("#BatchKey").val();
        model.BranchKey = $("#BranchKey").val();
        model.EmployeeKey = $("#EmployeeKey").val();
        model.ClassDetailsKey = $("#ClassDetailsKey").val();

        $.ajax({
            type: "POST",
            url: $("#hdnUrl").val(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            success: function (result) {
                if (result.IsSuccessful == false) {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("#dvStudentsList").html("")
                $("#dvStudentsList").html(result);

            },
            error: function (request, status, error) {

            }
        });
    }

    var workScheduleDetailsCustomPopup = function (_this, Type) {

        var ListItem = $(_this).closest("[data-repeater-item]");
        var obj = {};
        if (Type.toLowerCase() == "edit") {
            obj.RowKey = $("input[type=hidden][id*=RowKey]", ListItem).val();
        }
        else {
            obj.RowKey = 0
        }
        obj.TopicKey = $("input[type=hidden][id*=TopicKey]", ListItem).val();
        obj.SubjectModuleKey = $("input[type=hidden][id*=SubjectModuleKey]", ListItem).val();
        obj.MasterRowKey = $("input[type=hidden][id*=MasterRowKey]", ListItem).val();
        obj.ProgressStatus = $("input[type=hidden][id*=ProgressStatus]", ListItem).val();

        obj.SubjectKey = $("#SubjectKey").val();
        obj.BatchKey = $("#BatchKey").val();
        obj.BranchKey = $("#BranchKey").val();
        obj.EmployeeKey = $("#EmployeeKey").val();
        obj.ClassDetailsKey = $("#ClassDetailsKey").val();



        var url = $("#hdnAddEditWorkScheduleDetail").val();
        $.customPopupform.CustomPopup({
            ajaxType: "POST",
            ajaxData: { model: obj },
            load: function () {
                setTimeout(function () {

                    $("#frmSModulesTopic").removeData("validator");
                    $("#frmSModulesTopic").removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse($("#frmSModulesTopic"));
                }, 500)
            },

            rebind: function (result) {


            }
        }, url);
    }

    var workScheduleHistoryCustomPopup = function (_this, MasterRowKey) {

        var ListItem = $(_this).closest("[data-repeater-item]");
        var id = $("input[type=hidden][id*=MasterRowKey]", ListItem).val();
        if (typeof MasterRowKey !== "undefined") {
            id = MasterRowKey;
        }

        var url = $("#hdnGetHistoryWorkSchedule").val();
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            ajaxType: "GET",
            ajaxData: { id: id },
            load: function () {
                setTimeout(function () {

                    $("#frmSModulesTopic").removeData("validator");
                    $("#frmSModulesTopic").removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse($("#frmSModulesTopic"));


                }, 500)
            },

            rebind: function (result) {


            }
        }, url);
    }

    var deleteWorkScheduleDetails = function (_this) {
        var ListItem = $(_this).closest("[data-repeater-item]");
        var MasterRowKey = $("input[type=hidden][id*=MasterRowKey]", ListItem).val();
        var rowkey = $("input[type=hidden][id*=RowKey]", ListItem).val();

        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_FeeType,
            actionUrl: $("#hdnDeleteWorkScheduleDetails").val(),
            actionValue: rowkey,
            //{ id: rowkey },
            dataRefresh: function () {
                setTimeout(function () {
                    $("#DivHistoryWorkScheduleDetails").closest(".modal").modal("hide");
                    WorkSchedule.WorkScheduleHistoryCustomPopup(_this);
                    WorkSchedule.LoadData();
                }, 500)

            }
        });
    }

    function formSubmit(data) {

        var $form = $("#frmWorkScheduleDetails")
        var JsonData = [];
        var formData = $form.serializeArray();
        var TimeKeys = ["TimeIn", "TimeOut"]
        //var checkdata = $("[name*=ExamStatus]:checked")
        if ($form.valid()) {

            //if (checkdata.length > 0) {
            //var obj = $form.serializeArray();

            $(formData).each(function (i) {
                if (formData[i]) {
                    var name = formData[i]['name'];
                    var keyName = name.match(/\].*/) ? name.match(/\].*/)[0].replace("].", "") : name;

                    if (TimeKeys.indexOf(keyName) > -1) {
                        formData[i]['value'] = (formData[i]['value'] != "" ? moment(formData[i]['value'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
                    }
                }
            })
            //delete formData[0];
            var dataurl = $form.attr("action");
            var response = [];

            $.ajax({
                url: dataurl,
                datatype: "json",
                type: "POST",
                contenttype: 'application/json; charset=utf-8',
                async: false,
                //data: $form.serializeArray(),
                data: formData,
                success: function (data) {
                    if (typeof response == "string") {
                        $("[data-valmsg-for=error_msg]").html(response);
                    }
                    else if (data.IsSuccessful) {
                        var MasterRowKey = data.MasterRowKey;
                        var RowKey = data.RowKey;
                        $.alert({
                            type: 'green',
                            title: Resources.Success,
                            content: data.Message,
                            icon: 'fa fa-check-circle-o-',
                            buttons: {
                                Ok: {
                                    text: Resources.Ok,
                                    btnClass: 'btn-success',
                                    action: function () {
                                        //window.location.href = $("#hdnStudentPromotionList").val();
                                        setTimeout(function () {
                                            $("#frmWorkScheduleDetails").closest(".modal").modal("hide");
                                            if (RowKey != 0) {
                                                $("#DivHistoryWorkScheduleDetails").closest(".modal").modal("hide");
                                                WorkSchedule.WorkScheduleHistoryCustomPopup("", MasterRowKey);

                                            }
                                            WorkSchedule.LoadData();
                                        }, 500)
                                    }
                                }
                            }
                        })

                    }
                    else {
                        $("[data-valmsg-for=error_msg]").html(data.Message);
                    }
                },
                error: function (xhr) {

                }
            });


        }
    }

    return {
        LoadData: loadData,
        WorkScheduleDetailsCustomPopup: workScheduleDetailsCustomPopup,
        WorkScheduleHistoryCustomPopup: workScheduleHistoryCustomPopup,
        DeleteWorkScheduleDetails: deleteWorkScheduleDetails,
        FormSubmit: formSubmit
    }

}());

