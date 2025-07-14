(function ($) {
    'use strict';
    $.customPopupform = (function () {

        var customPopup = function (fig, url) {

            var modalsize = fig.modalsize || "modal-md";
            var templates = fig.templates || {
                modal: $('<div class="modal fade in" />'),
                modalDailog: $('<div class="modal-dialog ' + modalsize + '" />'),
                modalContent: $('<div class="modal-content" />'),
                modalBody: $('<div class="modal-body" />'),
                modalFooter: $('<div class="modal-footer" />'),
                modalHeader: $('<div class="modal-header"><i data-dismiss="modal" class="fa fa-remove close"></i><h4 class="modal-title"></h4></div>'),
            }

            var form = fig.form || null;

            var formAction = fig.formAction || null;

            var headerText = fig.headerText || "";
            var footerText = fig.footerText || "";
            var currentModal = null;
            var currentDailog = null;
            var modalLoadForm = null;

            var ajaxType = fig.ajaxType || "Get";
            var ajaxData = fig.ajaxData || {};

            var load = fig.load || function () {
            };
            var close = fig.close || function () {
            };

            var rebind = fig.rebind || function () {
            };
            var submit = fig.submit || function (form) {
                formSubmit();
            };
            var bindFormDailog = function (Dailog) {
                if (form) {
                    $("#" + $(form).prop("id"), $(Dailog)).off("submit").on("submit", function (e) {
                        var btn = $(form).find("input.btnSubmit,input:submit");
                        $(btn).attr("disabled", true);
                        var doSomething = setTimeout(function () {
                            clearTimeout(doSomething);
                            $(btn).removeAttr("disabled");
                        }, 2000);
                        submit();
                        e.preventDefault();
                        return false;
                    })
                }
            }
            var formSubmit = function () {
                var validator = $(form).validate();
                var validate = $(form).valid();
                if (validate) {

                    $(currentDailog).mLoading();
                    var btn = $(form).find("input.btnSubmit,input:submit");
                    $(btn).hide();
                    $("[disabled]", $(form)).removeAttr("disabled");
                    $.ajax({
                        url: $(form)[0].action,
                        type: $(form)[0].method,
                        data: $(form).serialize(),
                        success: function (result) {
                            if (result.IsSuccessful) {
                                var btn = $(form).find("input.btnSubmit,input:submit");
                                $(btn).show();
                                rebind(result);
                                $(currentModal).modal('hide');

                            } else {
                                var btn = $(form).find("input.btnSubmit,input:submit");
                                $(btn).show();
                                $(modalLoadForm).html(result);
                                form = $("form", $(modalLoadForm));

                                $('select:not(.ui-pg-selbox)').selectpicker();

                                bindFormDailog($(currentDailog)[0]);
                            }
                            $(currentDailog).mLoading("destroy");
                        },
                        error: function (xhr) {
                            console.log(xhr.responseText);
                            $(currentDailog).mLoading("destroy");
                            var btn = $(form).find("input.btnSubmit,input:submit");
                            $(btn).show();
                        }
                    });
                }
                else {
                    validator.focusInvalid();
                }

            }
            var generateModal = function (url) {

                var modal = templates.modal;
                var modalDailog = templates.modalDailog;
                var modalContent = templates.modalContent;
                var modalBody = templates.modalBody;
                modalLoadForm = !headerText && !footerText ? $(modalContent) : $(modalBody);
                var modalHeader = templates.modalHeader;
                var modalFooter = templates.modalFooter;

                if (headerText)
                    $(modalHeader).find(".modal-title").html(headerText);
                if (footerText)
                    $(modalFooter).html(footerText);



                if (!headerText && !footerText) {
                    $(modalDailog).append($(modalLoadForm))
                }
                else {
                    $(modalContent).append($(modalHeader))
                    $(modalContent).append($(modalLoadForm))
                    $(modalContent).append($(modalFooter))
                    $(modalDailog).append($(modalContent));
                }
                currentDailog = $(modalDailog);
                $(modal).append($(modalDailog))
                currentModal = $(modal);

                try {
                    $(modal).one('shown.bs.modal', function () {
                        $(modalDailog).mLoading();
                        $(modalDailog).css("min-height", "200px")
                        ajaxData = ajaxData
                        $.ajax({
                            url: url,
                            type: ajaxType,
                            data: ajaxData,
                            datatype: "JSON",
                            success: function (result) {
                                if (result != "") {
                                    $(modalLoadForm).html(result)
                                    form = null;
                                    form = form ? form : $(modalLoadForm).find("form");
                                    if (formAction) {
                                        $(form).attr("action", formAction);
                                    }

                                    if (form)
                                        $.validator.unobtrusive.parse($(form)[0]);
                                    load(modalDailog);

                                    $('select:not(.ui-pg-selbox)').selectpicker();


                                    bindFormDailog(modalDailog);

                                    var btn = $(this).find("input.btnSubmit");


                                }
                                else {
                                    $(modal).modal("hide")
                                }
                            },
                            error: function (xhr) {

                            },
                            complete: function () {
                                setTimeout(function () {
                                    $(modalDailog).mLoading("destroy");
                                    $(modalDailog).css("min-height", "0px")
                                }, 500)
                            }
                        });

                    }).one('hidden.bs.modal', function () {
                        $(this).remove();
                        $(currentDailog).remove();
                    }).one('hidden.bs.modal', function () {
                        close()
                    }).modal({
                        backdrop: 'static',
                        keyboard: false
                    }, 'show');
                    //$(modal).one('show.bs.modal', function () {
                    //    $(modalDailog).mLoading();
                    //    $(modalDailog).css("min-height", "200px")
                    //    $(modalLoadForm).load(url, function () {
                    //        if ($(modalLoadForm).html() != "") {


                    //            form = null;
                    //            form = form ? form : $(this).find("form");
                    //            if (formAction) {
                    //                $(form).attr("action", formAction);
                    //            }

                    //            if (form)
                    //                $.validator.unobtrusive.parse($(form)[0]);
                    //            load();
                    //            bindFormDailog(modalDailog);
                    //            setTimeout(function () {
                    //                $(modalDailog).mLoading("destroy");
                    //                $(modalDailog).css("min-height", "0px")
                    //            }, 500)
                    //        }
                    //        else {
                    //            $(modal).modal("hide")
                    //        }
                    //    })

                    //}).one('hidden.bs.modal', function () {
                    //    $(this).remove();
                    //    $(currentDailog).remove();
                    //}).modal({
                    //    backdrop: 'static',
                    //    keyboard: false
                    //}, 'show');

                }
                catch (e) {
                    $(modalDailog).mLoading("destroy");
                }



            }
            if (url)
                generateModal(url)


            return {
                GenerateModal: generateModal
            }
        }
        return {
            CustomPopup: customPopup
        }
    }());
    $.fn.popupform = function (fig) {
        fig = fig || {};

        var $self = this;
        var this_ = $(this)[0];


        $self.on('off').on('click', function () {
            this_ = $(this);
            var url = $(this).attr("data-href");
            $.customPopupform.CustomPopup(fig).GenerateModal(url);
        });

        return this;
    };
}(jQuery));