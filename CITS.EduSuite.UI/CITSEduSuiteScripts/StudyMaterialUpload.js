var JsonModel = [];
var data = {};
var StudyMaterialUpload = (function () {

    var getStudyMaterials = function () {
        $(".LoadError").html("");
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetStudyMaterial").val(),
            datatype: 'json',
            mtype: 'Get',
            styleUI: 'Bootstrap',
            postData: {

                SearchText: function () {
                    return $('#SearchText').val()
                },

            },
            colNames: [Resources.RowKey, Resources.Subject, Resources.StudyMaterialTitle, Resources.StudyMaterialDescriptions, Resources.StudyMaterials, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { name: 'SubjectName', search: false, index: 'SubjectName', sortable: true, resizable: false },
                { name: 'StudyMaterialTitle', search: false, index: 'StudyMaterialTitle', sortable: true, resizable: false },               
                { name: 'StudyMaterialDecription', search: false, index: 'StudyMaterialDecription', sortable: true, resizable: false },
                { name: 'StudyMaterialCount', search: false, index: 'StudyMaterialCount',  sortable: true, resizable: false },
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
                                href = $("#hdnAddEditStudyMaterial").val()+"/" + rowid
                                window.location.href = href;
                                break;
                            case "V":
                                StudyMaterialUpload.GetStudyMaterialsByStudyMaterialId(rowid);
                                break;
                            case "D":
                                deleteStudyMaterial(rowid);
                                break;
                        }
                    },
                    items: {
                        E: { name: Resources.Edit, icon: "fa-edit" },   
                        V: { name: "View", icon: "fa-eye" },
                        D: { name: Resources.Delete, icon: "fa-trash" },

                    }
                }

            }
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }


    

    function editDownload(cellValue, options, rowdata, action) {
        if (cellValue != 0) {

            return '<a target="_black" href="' + $("#hdnStudyMaterialUrlconst").val() + "/" + rowdata.RowKey + "/" + cellValue + '" >Download</a> '

        }
    }
  

    var getStudyMaterialUpload = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();

                var item = $(this);
                $("[id*=RowKey]", $(this)).val(0)
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                //$("[id*=IsAllowPreview]", $(this)).val(false)
                //$("[id*=IsAllowDownload]", $(this)).val(false)
                //$("[id*=IsActive]", $(this)).val(true)
                
            },
            hide: function (remove) {
              
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0")
                {
                    deleteStudyMaterialDetail($(hidden).val());
                }
                else
                {
                    $(this).slideUp(remove);
                }



            },
            rebind: function (response) {
             

                if (typeof response == "string") {
                    $("[data-valmsg-for=error_msg]").html(response);
                }
                else if (response.IsSuccessful) {
                    toastr.success(Resources.Success, response.Message);
                    location.href = $("#hdnStudyMaterialList").val();
                }
            },
            data: json,
            defaultValues: json,
            submitButton: '#btnSave',
            hasFile: true,
            
        });
    }



    var getStudyMaterialsByStudyMaterialId = function (rowkey) {
        var URL = $("#hdnGetStudyMaterialsByStudyMaterialId").val();
        JsonModel = jsonData;

        JsonModel["RowKey"] = rowkey;

  


        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            ajaxType: "Post",
            ajaxData: JsonModel,
            load: function () {
             
            },


        }, URL);

    }


    var getStudyMaterialsByStudyMaterialDetailsId = function (rowkey) {
        var URL = $("#hdnGetStudyMaterialsByStudyMaterialDetailsId").val();
        JsonModel = jsonData;

        JsonModel["StudyMaterialDetailsKey"] = rowkey;

     


        $.customPopupform.CustomPopup({
            modalsize: "modal-lg80",
            ajaxType: "Post",
            ajaxData: JsonModel,
            load: function () {

            },


        }, URL);

    }




    var ajaxSubmit = function (data)
    {

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


    var getStudyMaterialDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
                //$("[id*=OriginalIssuedDate]", $(this)).val(moment(new Date()).format("DD/MM/YYYY"))
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();

                $("[id*=RowKey]", $(this)).val(0)

                $("[id*=ModuleTopicKey]", $(this)).selectpicker();
                var obj = {};
                obj.SubjectModuleKey = $("#SubjectModuleKey").val() != "" ? $("#SubjectModuleKey").val() : 0;
                AppCommon.BindDropDownbyId(obj, $("#hdnFillModuleTopics").val(), $("[id*=ModuleTopicKey]"), Resources.ModuleTopics);
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteWorkScheduleDetails($(hidden).val(), $(this));
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
                    if ($("#dvInstallmentDetails")[0]) {
                        WorkSchedule.BindWorkScheduleDetails();
                        WorkSchedule.AddEditWorkSchedule(null, response.EmployeeKey);
                    }
                    $(".modal").modal("hide");
                }

            },
            data: json,
            repeatlist: 'WorkscheduleSubjectmodel',
            submitButton: '#btnSave',
            defaultValues: json,
            hasFile: true
        });
    }

    return {
        GetStudyMaterialUpload: getStudyMaterialUpload,
        AjaxSubmit: ajaxSubmit,
        GetStudyMaterials: getStudyMaterials,
        GetStudyMaterialsByStudyMaterialId: getStudyMaterialsByStudyMaterialId

    }
}());




function deleteStudyMaterialDetail(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_StudyMaterial,
        actionUrl: $("#hdnDeleteStudyMaterialDetails").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            location.reload();
        }
    });
}


function deleteStudyMaterial(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_StudyMaterial,
        actionUrl: $("#hdnDeleteStudyMaterial").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            location.reload();
        }
    });
}


