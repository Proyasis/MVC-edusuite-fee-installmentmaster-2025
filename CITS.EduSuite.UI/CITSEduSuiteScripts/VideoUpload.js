var JsonModel = [];
var data = {};
var VideoUpload = (function () {

    var getVideos = function () {
        $(".LoadError").html("");
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetVideos").val(),
            datatype: 'json',
            mtype: 'Get',
            styleUI: 'Bootstrap',
            postData: {

                SearchText: function () {
                    return $('#SearchText').val()
                },

            },
            colNames: [Resources.RowKey, Resources.Subject, Resources.VideoTitle, Resources.Videos, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { name: 'SubjectName', search: false, index: 'SubjectName', sortable: true, resizable: false },
                { name: 'VideoTitle', search: false, index: 'VideoTitle', sortable: true, resizable: false },
                { name: 'VideoCount', search: false, index: 'VideoCount', sortable: true, formatter: editCount, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
            autowidth: true,
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            },
            loadError: function (jqXHR, textStatus, errorThrown) {
                $(".LoadError").html("Cannot Connect to Server. Please check your Internet Connection");
            }


        }).contextMenu({
            selector: ".jqgrow .context-menu",
            trigger: 'left',
            build: function ($trigger, e) {
                // this callback is executed every time the menu is to be shown
                // its results are destroyed every time the menu is hidden
                // e is the original contextmenu event
                var $tr = $(e.target).closest("tr.jqgrow"),
                    rowid = $tr.attr("id"),
                    item = $("#grid").jqGrid("getRowData", rowid);
                return {
                    callback: function (key, options) {

                        var href = "";
                        switch (key) {
                            case "E":
                                //href = $("#hdnAddEditVideos").val() + AppCommon.EncodeQueryString('id=' + rowid)
                                href = $("#hdnAddEditVideos").val() + "/" + rowid
                                window.location.href = href;
                                break;
                            case "V":
                                VideoUpload.GetVideosByVideoId(rowid);
                                break;
                            case "D":
                                deleteVideo(rowid);
                                break;



                        }
                    },
                    items: {
                        E: { name: Resources.Edit, icon: "fa-edit" },
                        V: { name: "View", icon: "fa-eye" },
                        D: { name: Resources.Delete, icon: "fa-eye" },

                    }
                }

            }
        });


        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }



    function editCount(cellValue, options, rowdata, action) {

        if (cellValue != 0) {
            return '<a   class="gvCountDefault" onclick="VideoUpload.GetVideosByVideoId(' + rowdata.RowKey + ')" >' + cellValue + '</span>';
        }
        else {
            //return ''
            return '<a   class="gvCountDefault gvCountNoRecords"  >0</span>';
        }
    }

    var getVideoUpload = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();

                var item = $(this);

                var item = $(this);
                $("[id*=RowKey]", $(this)).val(0)
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                $("[data-repeater-list] [data-repeater-item]:last-child .gvCountDefault").remove();
                

            },
            hide: function (remove) {

                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteVideoDetail($(hidden).val());
                }
                else {
                    $(this).slideUp(remove);
                }



            },
            rebind: function (response) {


                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    location.reload();
                }
            },
            data: json,
            repeatlist: 'VideoList',
            defaultValues: json,
            //submitButton: '#btnSave',
            //hasFile: true,
            //Async: true
        });
    }

    var getVideosByVideoId = function (rowkey) {
        var URL = $("#hdnGetVideosByVideoId").val();
        JsonModel = jsonData;

        JsonModel["RowKey"] = rowkey;




        $.customPopupform.CustomPopup({
            modalsize: "modal-lg80",
            ajaxType: "Post",
            ajaxData: JsonModel,
            load: function () {

            },


        }, URL);

    }

    var getVideosByVideoDetailsId = function (rowkey) {
        var URL = $("#hdnGetVideosByVideoDetailsId").val();
        JsonModel = jsonData;

        JsonModel["VideoDetailsKey"] = rowkey;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg80",
            ajaxType: "Post",
            ajaxData: JsonModel,
            load: function () {

            },
        }, URL);

    }

    function formSubmit(data) {


        var $form = $("#frmAddEditVideoUpload")
        var JsonData = [];

        var formData = $form.serializeToJSON({
            associativeArrays: false
        });

       
        if ($form.valid()) {
            var dataurl = $form.attr("action");
            var response = [];

            //response = AjaxHelper.ajax("POST", dataurl,
            //    {
            //        model: formData
            //    });

            AjaxHelper.ajaxWithFileAsync("model", "POST", dataurl,
                {
                    model: formData
                }, function () {
                    response = this;
                    if (response.IsSuccessful) {
                        //ParentMeetScheduleKey = formData.ParentMeetScheduleKey;
                        //toastr.success(Resources.Success, response.Message)
                        //$("#frmAddEditParentsMeetDetails").closest(".modal").modal("hide");
                        //TeacherPortal.GetStudentsForParentsMeet(BranchKey, ParentMeetScheduleKey, ClassKey, ClassName, 3);
                    } else {
                        toastr.error(Resources.Failed, response.Message)
                    }

                });

            //if (typeof response == "string") {
            //    $("[data-valmsg-for=error_msg]").html(response);
            //}
            //else if (response.IsSuccessful) {
            //    $.alert({
            //        type: 'green',
            //        title: Resources.Success,
            //        content: response.Message,
            //        icon: 'fa fa-check-circle-o-',
            //        buttons: {
            //            Ok: {
            //                text: Resources.Ok,
            //                btnClass: 'btn-success',
            //                action: function () {
            //                    //window.location.href = $("#hdnEmployeeClass").val() + "/" + id;
            //                }
            //            }
            //        }
            //    })

            //}

        }
    }


    var ajaxSubmit = function (data) {

        data = data;

        var response = {};
        var $form = $("form");
        var formData = $form.serializeArray();
        objectifyForm(formData, data)
        //var jsonData = JSON.stringify(data);
        var formdata = new FormData();
        toFormData(data, formdata, "");
        var url = $('form').attr('action');
        $.ajax({
            type: "POST",
            url: url,
            data: formdata,
            dataType: 'json',
            async: true,
            processData: false,
            contentType: false,
            cache: false,
            success: function (result) {
                if (typeof result == "string")
                    response.Message = result;
                else {
                    response = result;
                    //response.Message = "Success";
                }
            },

            error: function (request, status, error) {
                console.log(request.responseText);
                if (request.status == 200) {
                    response.Message = "Your session has ended and you have been logged out and should log back in.";
                }
                else
                    response.Message = "Internal server error";
            },
            xhr: function () {
                // get the native XmlHttpRequest object
                var xhr = $.ajaxSettings.xhr();
                // set the onprogress event handler
                xhr.upload.onprogress = function (evt) { console.log('progress', evt.loaded / evt.total * 100) };
                // set the onload event handler
                xhr.upload.onload = function () { console.log('DONE!') };
                // return the customized object
                return xhr;
            },
        });

    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><button type="button" class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</button></div>'
        //return '<div class="divEditDelete"><a class="btn btn-primary btn-sm" href="AddEditApplication' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-sm"  onclick="javascript:deleteApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a><a class="btn btn-warning btn-sm" href="' + $("#hdnViewApplication").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';
    }


    var checkVideoType = function (_this) {

        var item = $(_this).closest("[data-repeater-item]");
        var VideoTypekey = $("[id*=VideoTypeKey]", item).val();
        VideoTypekey = parseInt(VideoTypekey) ? parseInt(VideoTypekey) : 0;

        if (VideoTypekey == Resources.VideoTypeFile) {
            $(".divFileAttachment", item).show();
            $(".divYoutubelink", item).hide();
            $(".divAllowDownload", item).show();

        } else {
            $(".divFileAttachment", item).hide();
            $(".divYoutubelink", item).show();
            $(".divAllowDownload", item).hide();
            $("[id*=IsAllowDownload]", item).val(false);
            
        }
    }
    return {
        GetVideoUpload: getVideoUpload,
        AjaxSubmit: ajaxSubmit,
        GetVideos: getVideos,
        GetVideosByVideoId: getVideosByVideoId,
        GetVideosByVideoDetailsId: getVideosByVideoDetailsId,
        CheckVideoType: checkVideoType,
        FormSubmit: formSubmit

    }
}());




function deleteVideoDetail(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Video,
        actionUrl: $("#hdnDeleteVideoDetails").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            location.reload();
        }
    });
}


function deleteVideo(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Video,
        actionUrl: $("#hdnDeleteVideo").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}