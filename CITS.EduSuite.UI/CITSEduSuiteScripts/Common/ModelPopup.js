var EduSuite = function () {
    var options = {
        title: '',
        content: '',
        actionUrl: '',
        actionValue: '',
        targerGrid: '',
        type: '',
        icon: '',
        btnClass: ''

    }



    var confirm = function (options) {


        $.confirm({
            title: options.title,
            content: options.content,
            type: 'orange',
            icon: 'fa fa-exclamation-circle',
            useBootstrap: true,
            theme:'bootstrap',
            buttons: {

                confirm:
                {
                    text: Resources.Confirm,
                    btnClass:"btn-danger",
                    action: function (jc) {
                        var response = AjaxHelper.ajax("POST", options.actionUrl,
                            {
                                id: options.actionValue
                            });
                        if (response.Message) {
                            if (response.Message === Resources.Success) {
                                toastr.success(Resources.Success, response.Message);
                                options.dataRefresh(response);

                                }
                                else
                                    toastr.error(Resources.Failed, response.Message);
                            }
                            event.preventDefault();
                        }
                    },

                cancel:
                {
                    btnClass: "btn-dark",
                    text: Resources.Cancel,
                    action: function () {
                        if (options.cancel) {
                            options.cancel();
                        }
                    }
                }
            }
        });
    };

    var confirm2 = function (options) {
        $.confirm({
    
            title: options.title,
            content: options.content,
            type: 'orange',
            icon: 'fa fa-exclamation-circle',
            useBootstrap: true,
            theme: 'bootstrap',
            buttons: {
                confirm:
                {
                    text: Resources.Confirm,
                    btnClass: "btn-danger",
                    action: function () {
                        var response = AjaxHelper.ajax("POST", options.actionUrl,
                            options.parameters);


                            if (response.Message) {
                                if (response.Message === Resources.Success) {
                                    toastr.success(Resources.Success, response.Message);
                                    options.dataRefresh();

                                }
                                else
                                    toastr.error(Resources.Failed, response.Message);
                            }
                            event.preventDefault();



                        }
                    },

                cancel:
                {
                    btnClass: "btn-dark",
                    text: Resources.Cancel,
                    action: function () {
                        if (options.cancel) {
                            options.cancel();
                        }
                    }
                }
            }
        });
    };

    var alertMessage = function (options) {


        $.alert({

            title: options.title,
            content: options.content,
            type: options.type,
            icon: options.icon,
            buttons: {

                Ok:
                    {
                        text: Resources.Ok,
                        btnClass: options.btnClass
                    }
            }
        });
    }
    var confirmWithMultipleParam = function (options) {


        $.confirm({

            animation: 'zoom',
            closeAnimation: 'left',

            title: options.title,
            content: options.content,
            type: 'orange',
            icon: 'fa fa-exclamation-circle',
            buttons: {

                confirm:
                {
                    text: Resources.Confirm,
                    action: function () {
                        $(".jconfirm-box").mLoading();
                        AjaxHelper.ajaxAsync("POST", options.actionUrl,
                            options.param,
                            function () {
                                response = this;
                                if (response.Message === Resources.Success) {
                                    toastr.success(Resources.Success, response.Message);
                                    options.dataRefresh();

                                }
                                else {
                                    toastr.error(Resources.Failed, response.Message);
                                }
                                $(".jconfirm-box").mLoading("destroy");
                            }
                        );


                        event.preventDefault();

                    }
                },

                cancel:
                {
                    text: Resources.Cancel,
                    action: function () {


                    }
                }
            }
        });
    };

    return {
        Confirm: confirm,
        Confirm2: confirm2,
        AlertMessage: alertMessage,
        ConfirmWithMultipleParam: confirmWithMultipleParam,

    };

}();
