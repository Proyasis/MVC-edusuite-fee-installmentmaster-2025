var JsonData = [], request = null;
var BulkEmailSMS = (function () {


    var GetSendEmail = function (json) {
        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    var item = $(this);

                },
                hide: function (remove) {

                    $(this).slideUp(remove);
                },
                rebind: function (response) {
                    $(".modal-body").mLoading("destroy");
                    //$("#selectedApplicantsDatePreview").html("");
                    //$(".IsSelect").prop("checked",false);



                    $('[data-dismiss="modal"]').trigger("click");
                    if (typeof response == "string") {
                        $("[data-valmsg-for=error_msg]").html(response);
                    }
                    else if (response.IsSuccessful) {
                        toastr.success(Resources.Success, response.Message);

                    }

                },
                data: json,
                repeatlist: 'BulkEmailList',
                submitButton: '#btnSave',
                hasFile: true
            });
    }




    var GetSendSMS = function (json) {
        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    var item = $(this);

                },
                hide: function (remove) {

                    $(this).slideUp(remove);
                },
                rebind: function (response) {
                    $(".modal-body").mLoading("destroy");
                    //$("#selectedApplicantsDatePreview").html("");
                    //$(".IsSelect").prop("checked",false);



                    $('[data-dismiss="modal"]').trigger("click");
                    if (typeof response == "string") {
                        $("[data-valmsg-for=error_msg]").html(response);
                    }
                    else if (response.IsSuccessful) {
                        toastr.success(Resources.Success, response.Message);

                    }

                },
                data: json,
                repeatlist: 'BulkSMSList',
                submitButton: '#btnSave',
                hasFile: true
            });
    }

    var GetBulkEmailSMS = function (json, pageIndex, resetPagination) {
        JsonData = json;

        var JsonData = $("form").serializeToJSON({

        });

        //JsonData["FilterTypeKey"] = $(".BulkEmailSMSTab .active").attr("value");

        //JsonData["SearchUserTypeKey"] = $("#SearchUserTypeKey").val();
        //JsonData["SearchBranchKey"] = $("#SearchBranchKey").val();
        //JsonData["SearchCountryKey"] = $("#SearchCountryKey").val();
        //JsonData["SearchInTakeKey"] = $("#SearchInTakeKey").val();
        //JsonData["SearchApplicationStatusKey"] = $("#SearchApplicationStatusKey").val();        
        //JsonData["SearchText"] = $("#SearchText").val();

        JsonData["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonData["PageSize"] = 20;
        $("#divContent").mLoading();

        request = $.ajax({
            url: $("#hdnGetBulkEmailSMS").val(),
            data: JsonData,
            datatype: "json",
            type: "post",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            beforeSend: function () {
                if (request != null) {
                    request.abort();

                }


            },

            success: function (data) {


                $("#divContent").html(data);

                if (resetPagination) {
                    BulkSmsEmailPagination();
                }

                $("#divContent").mLoading("destroy");

                $("#RoleKeys option").each(function () {
                    checkApplicant($(this).attr("datakey"));


                });


            },
            error: function (xhr) {
                isError = true;
                if (xhr.statusText != "abort")
                    $("#divContent").mLoading("destroy");
            }
        });

    }


    var GetBulkEmailPopUp = function (json) {
        var URL = $("#hdnGetBulkEmailPopUp").val();

        var RowKeys = $("#RowKeys option").map(function (index) {
            var opt = [];

            return opt[index] = $(this).val();
        }).get();

        var RoleKeys = $("#RoleKeys option").map(function (index) {
            var opt = [];

            return opt[index] = $(this).val();
        }).get();


        var Emails = $("#Emails option").map(function (index) {
            var opt = [];

            return opt[index] = $(this).val();
        }).get();


        json.RowKeys = JSON.parse(JSON.stringify(RowKeys));
        json.RoleKeys = JSON.parse(JSON.stringify(RoleKeys));
        json.Emails = JSON.parse(JSON.stringify(Emails));


        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            ajaxType: "Post",
            ajaxData: json,
            load: function () {



            },


        }, URL);

    }

    var GetBulkSMSPopUp = function (json) {
        var URL = $("#hdnGetBulkSMSPopUp").val();

        var RowKeys = $("#RowKeys option").map(function (index) {
            var opt = [];

            return opt[index] = $(this).val();
        }).get();

        var RoleKeys = $("#RoleKeys option").map(function (index) {
            var opt = [];

            return opt[index] = $(this).val();
        }).get();


        var SMS = $("#SMS option").map(function (index) {
            var opt = [];

            return opt[index] = $(this).val();
        }).get();


        json.RowKeys = JSON.parse(JSON.stringify(RowKeys));
        json.RoleKeys = JSON.parse(JSON.stringify(RoleKeys));
        json.SMS = JSON.parse(JSON.stringify(SMS));


        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            ajaxType: "Post",
            ajaxData: json,
            load: function () {



            },


        }, URL);

    }


    return {
        GetBulkEmailSMS: GetBulkEmailSMS,
        GetBulkEmailPopUp: GetBulkEmailPopUp,
        GetBulkSMSPopUp: GetBulkSMSPopUp,
        GetSendEmail: GetSendEmail,
        GetSendSMS: GetSendSMS
    }

}());






function AddSelectedRecord(index, object, RowKey, Name, Mobile, Email, RoleKey) {
    var SelectedLenghth = $('#RowKeys option').length;
    if (SelectedLenghth == 0) {
        $("#RowKeys").html("");

    }
    if (SelectedLenghth >= 100) {
        $(object).prop("checked", false);

        alert("100 Records Allowed at a time");
        //return false;
    }

    var mobileNo = Mobile;
    if (mobileNo.length != 12) {
        Mobile = "91" + Mobile
    }
   


        if ($(object).prop("checked") == true) {
            $('#RowKeys').append($("<option selected='true' ></option>").attr("value", RowKey).text(RowKey));
            $('#RoleKeys').append($("<option datakey='" + RowKey + '_' + RoleKey + "' selected='true' ></option>").attr("value", RoleKey).text(RoleKey));
            $('#Emails').append($("<option datakey='" + RowKey + '_' + RoleKey + "' selected='true' ></option>").attr("value", Email).text(Email));
            $('#SMS').append($("<option datakey='" + RowKey + '_' + RoleKey + "' selected='true' ></option>").attr("value", Mobile).text(Mobile));


            //$('#selectedApplicantsDatePreview').append("<li  applicantkey='" + RowKey + '_' + RoleKey + "' >" + Name + "-" + text + " <i onclick='deleteSelectedApplicant(" + RowKey, RoleKey + ")' class='fa fa-remove ListRemove'></i> </li>");
            $('#selectedApplicantsDatePreview').append("<li datakey='" + RowKey + '_' + RoleKey + "'>" + Email + " " + Mobile + "-" + " <i onclick='deleteSelectedRecord(" + RowKey + ',' + RoleKey + ")' class='fa fa-remove ListRemove'></i> </li>");
        }
        else {
            $("#RowKeys option[value='" + RowKey + "']").remove();
            $("#RoleKeys option[datakey='" + RowKey + '_' + RoleKey + "']").remove();
            $("#Emails option[datakey='" + RowKey + '_' + RoleKey + "']").remove();
            $("#SMS option[datakey='" + RowKey + '_' + RoleKey + "']").remove();

            $("#selectedApplicantsDatePreview li[datakey='" + RowKey + '_' + RoleKey + "']").remove();
        }

    SelectedLenghth = $('#RowKeys option').length;

    if (SelectedLenghth == 0) {


        $("#RowKeys").val("");
        $("#RoleKeys").html("");
        $("#Emails").html("");
        $("#SMS").html("");
    }


}

function deleteSelectedRecord(RowKey, RoleKey) {
    $("#RowKeys option[value='" + RowKey + "']").remove();
    $("#RoleKeys option[datakey='" + RowKey + '_' + RoleKey + "']").remove();
    $("#Emails option[datakey='" + RowKey + '_' + RoleKey + "']").remove();
    $("#SMS option[datakey='" + RowKey + '_' + RoleKey + "']").remove();
    $("input[datakey=" + RowKey + '_' + RoleKey + "]").prop("checked", false);

    $("#selectedApplicantsDatePreview li[datakey='" + RowKey + '_' + RoleKey + "']").remove();

}





function BulkSmsEmailPagination() {

    $('#page-selection-up,#page-selection-down').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = 3;
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

    $('#page-selection-up,#page-selection-down').bootpag({
        total: totalPages,
        page: 1,
        maxVisible: 30
    });

    $('#page-selection-up,#page-selection-down').on("page", function (event, num) {

        BulkEmailSMS.GetBulkEmailSMS(JsonData, num, false);
    });
}


function checkApplicant(DataKey) {
    $("input[datakey=" + DataKey + "]").prop("checked", true);
}