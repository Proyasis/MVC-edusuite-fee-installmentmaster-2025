var ajaxNotificationCall = null;
var PushNotification = (function () {
    var pushNotificationInit = function () {
        var pushNotificationHub = $.connection.notificationHub;
        // Need at least one callback for events to be raised on the hub
        pushNotificationHub.client.void = function () { };
        $.connection.hub.logging = true;
        var icon = window.location.protocol + "//" + window.location.host + Resources.EduSuiteIco;
        pushNotificationHub.client.pushNotification = function (title, message, type) {
            toastr.options = {
                "closeButton": false,
                "debug": false,
                "newestOnTop": false,
                "progressBar": false,
                "positionClass": "toast-top-right",
                "preventDuplicates": true,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }
            if (type == "success")
                toastr.success(message, title, { timeOut: 10000 });
            else if (type == "error")
                toastr.error(message, title, { timeOut: 10000 });
            else if (type == "warning")
                toastr.warning(message, title, { timeOut: 10000 });
            else if (type == "info")
                toastr.info(message, title, { timeOut: 10000 });

            var options = {
                title: title,
                options: {
                    body: message,
                    icon: icon,
                    lang: 'en-US',
                    tag: 'uniqueTag'
                }
            };
            console.log(options);
            AppCommon.EasyNotify(options);
            var count = $("#spnPushNotificationCount").html().trim();
            count = parseInt(count) ? parseInt(count) : 0;
            $("#spnPushNotificationCount").html((++count).toString());

            if (count > 0) {
                $("#spnPushNotificationCount").closest("a").addClass("active")
            }
            count = $("#spnUreadNotification").html().trim();
            count = parseInt(count) ? parseInt(count) : 0;
            $("#spnUreadNotification").html((++count).toString() + " pending");
        }
        $.connection.hub.start().done(function () {
            //pushNotificationHub.server.onConnected();
         
        });

    }
    var getNotifications = function (pageIndex) {
        var obj = {};
        obj.pageIndex = pageIndex;
        obj.pageSize = 5;
        ajaxNotificationCall = $.ajax({
            url: $("#hdnGetLatestNotification").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            beforeSend: function (xhr) {
                if (ajaxNotificationCall) {
                    ajaxNotificationCall.abort();
                }
                if (pageIndex == 1) {
                    $("#listNotification").html("")
                }
                $("#listNotification  .loading").remove();
                if ($("#listNotification").find("li").length > 0) {
                    $("#listNotification li:last").after($("<li class='loading text-center'><a> Loading...</a></li>").fadeIn('slow')).data("loading", true);
                }
                else {
                    $("#listNotification").append($("<li class='loading text-center'><a> Loading...</a></li>").fadeIn('slow')).data("loading", true);
                }
            },
            success: function (result) {
                if (result) {
                    var $results = $("#listNotification");
                    $("#listNotification .loading").fadeOut('fast', function () {
                        $(this).remove();
                    });
                    var $ul = $("<ul/>")

                    var $data = $ul.append(result.records);
                    $data.find("li").hide();
                    $results.removeData("loading");
                    $data.find("li").each(function () {
                        $results.append(this);
                        var arr = $(".time", this).html().split(/\/|\s|:/);// split string and create array.
                        var date = new Date(arr[2], arr[1] - 1, arr[0], arr[3], arr[4], arr[5]);
                        // $(".time", this).html(AppCommon.TimeSince(date));

                        $(this).fadeIn();
                    })
                    $("#spnUreadNotification").html(result.unreadRecordCount.toString() + " pending");
                    $results.closest("div").scroll(function () {
                        var $this = $(this);
                        if (!$results.data("loading") && $results.find("li").length < result.recordCount) {

                            if ($this.scrollTop() + $this.height() == $this[0].scrollHeight) {
                                getNotifications(result.pageIndex + 1);
                            }
                        }
                    });
                }
            }
        });
    }
    var getLastestNotificationCount = function () {
        ajaxNotificationCall = $.ajax({
            url: $("#hdnGetLatestNotificationCount").val(),
            type: "GET",
            dataType: "JSON",
            data: {},
            beforeSend: function (xhr) {
            },
            success: function (result) {
                if (result && result.TotalLatestRecords > 0) {
                    //$("#spnPushNotificationCount").val(result.TotalLatestRecords.toString());
                    $("#hdnPushNotificationCount").val(result.TotalLatestRecords.toString());
                    $("#spnPushNotificationCount").html(result.TotalLatestRecords.toString());
                    $("#spnPushNotificationCount").closest("a").addClass("active")
                }

            }
        });
    }

    return {
        PushNotificationInit: pushNotificationInit,
        GetNotifications: getNotifications,
        GetLastestNotificationCount: getLastestNotificationCount
    }
}())