var ajaxRequest;
var AjaxHelper = (function () {
    function ajax(type, url, data) {
        var response = {};
        $('#btnSave').hide();
        $.ajax({

            type: type,
            url: url,
            //dataType: "json",
            async: false,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(data),

            success: function (result) {
                $('#btnSave').show();
                if (typeof result == "string")
                    response.Message = result;
                else {
                    response = result;
                    //response.Message = "Success";
                }
            },
            error: function (request, status, error) {
                console.log(request.responseText);
                $('#btnSave').show();
                if (request.status == 200) {
                    response.Message = "Your session has ended and you have been logged out and should log back in.";
                }
                else
                    response.Message = "Internal server error";
            }
        });
        return response;
    }

    function ajaxAsync(type, url, data, success, error) {
        var response = {};
        $('#btnSave').hide();
        $.ajax({

            type: type,
            url: url,
            //dataType: "json",
            async: true,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(data),

            success: function (result) {
                $('#btnSave').show();
                if (success)
                    success.call(result)
            },
            error: function (request, status, error) {
                $('#btnSave').show();
                console.log(request.responseText);
                if (request.status == 200) {
                    response.Message = "Your session has ended and you have been logged out and should log back in.";
                }
                else
                    response.Message = "Internal server error";
                if (error)
                    error.call(response)
            }
        });

    }

    function ajaxWithFile(model, type, url, data) {
        var response = {};
        var formdata = new FormData();

        toFormData(data[model], formdata, "");
        $.ajax({
            type: type,
            url: url,
            data: formdata,
            dataType: 'json',
            async: false,
            processData: false,
            contentType: false,
            cache: false,
            success: function (result) {
                $('#btnSave').show();
                if (typeof result == "string")
                    response.Message = result;
                else {
                    response = result;
                    //response.Message = "Success";
                }
            },
            error: function (request, status, error) {
                $('#btnSave').show();
                console.log(request.responseText);
                if (request.status == 200) {
                    response.Message = "Your session has ended and you have been logged out and should log back in.";
                }
                else
                    response.Message = "Internal server error";
            }
        });
        return response;
    }

    function ajaxWithFileAsync(model, type, url, data, success, error) {
        $(".progress-bar").attr("style", "width:0%");

        $(".section-content").mLoading();
        var response = {};

        $("#progressBar").show();
        //toFormData(data[model], formdata, "");
        const formElem = document.querySelector('form');
        var formdata = new FormData(formElem);

        $.ajax({
            type: type,
            url: url,
            data: formdata,
            dataType: 'json',
            async: true,
            processData: false,
            contentType: false,
            cache: false,
            success: function (result) {
                if (success)
                    success.call(result)
            },
            xhr: function () {
                // get the native XmlHttpRequest object
                var xhr = $.ajaxSettings.xhr();
                // set the onprogress event handler
                xhr.upload.onprogress = function (evt) {


                    console.log('progress', evt.loaded / evt.total * 100)
                    var progressCount = parseInt(evt.loaded / evt.total * 100)
                    $(".progress-bar").html(progressCount + "%");
                    $(".progress-bar").attr("style", "width:" + progressCount + "%");
                    $(".section-content").mLoading();

                };

                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4) {
                        toastr.success(Resources.Success, response.Message);
                        $(".section-content").mLoading("destroy");
                        var redirectURL = $("form").attr("RedirectUrl");

                        setTimeout(function () {
                            if (redirectURL != undefined) {
                                window.location.href = redirectURL;
                            }

                        }, 500);

                        return response;
                    }
                }

                // set the onload event handler
                //xhr.upload.onload = function () { console.log('DONE!') };
                // return the customized object
                return xhr;
            },
            error: function (request, status, error) {
                console.log(request.responseText);
                if (request.status == 200) {
                    response.Message = "Your session has ended and you have been logged out and should log back in.";
                }
                else
                    response.Message = "Internal server error";
                //if (error)
                //    error.call(response)
            }
        });

    }

    function ajaxFromData(obj) {
        var response = {};
        var defaults = {
            type: "POST",
            url: "",
            async: true,
            data: {},
            loadingCntrl: $("#formAppender"),
            success: function () {
            },
            error: function () {

            }
        };


        var options = $.extend(true, {}, defaults, obj)

        $.ajax({
            type: options.type,
            url: options.url,
            data: options.data,
            dataType: 'json',
            async: options.async,
            processData: false,
            contentType: false,
            cache: false,
            success: function (result) {
                options.success.call(null, result);

            },
            beforeSend: function () {

                $(options.loadingCntrl).mLoading()

                if (ajaxRequest != null) {
                    ajaxRequest.abort();
                    ajaxRequest = null;
                }
            },
            xhr: function () {
                // get the native XmlHttpRequest object
                var xhr = $.ajaxSettings.xhr();
                // set the onprogress event handler
                xhr.upload.onprogress = function (evt) {


                };

                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4) {
                        //toastr.success(Resources.Success, response.Message);



                        return response;
                    }
                }


                return xhr;
            },
            error: function (request, status, error) {
                console.log(request.responseText);
                if (request.status == 200) {
                    response.Message = "Your session has ended and you have been logged out and should log back in.";
                }
                else
                    response.Message = "Internal server error";
                options.error.call(null, request);
                $(options.loadingCntrl).mLoading("destroy")


            },
            complete: function () {
                $(options.loadingCntrl).mLoading("destroy")
            }
        });



    }

    return {
        ajax: ajax,
        ajaxAsync: ajaxAsync,
        ajaxWithFile: ajaxWithFile,
        ajaxWithFileAsync: ajaxWithFileAsync,
        ajaxFromData: ajaxFromData
    };
})();

function toFormData(obj, form, nameSpace) {
    var fd = form || new FormData();
    var formKey;

    for (var property in obj) {
        if (obj.hasOwnProperty(property) && obj[property] != undefined) {
            if (nameSpace) {
                formKey = nameSpace + '[' + property + ']';
            } else {
                formKey = property;
            }

            // if the property is an object, but not a File, use recursivity.
            if (obj[property] instanceof Date) {
                fd.append(formKey, obj[property].toISOString());
            }
            else if (typeof obj[property] === 'object' && !(obj[property] instanceof File)) {
                toFormData(obj[property], fd, formKey);
            } else { // if it's a string or a File object
                fd.append(formKey, obj[property]);
            }
        }
    }

    return fd;
}