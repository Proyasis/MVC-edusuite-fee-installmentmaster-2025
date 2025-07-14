var SyllabusAndStudyMaterial = (function () {
    var getSyllabusAndStudyMaterial = function () {


        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetSyllabusAndStudyMaterial").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                AcademicTermKey: function () {
                    return $('#AcademicTermKey').val()
                },
                CourseKey: function () {
                    return $('#CourseKey').val()
                },
                UniversityMasterKey: function () {
                    return $('#UniversityMasterKey').val()
                },
                SubjectYear: function () {
                    return $('#SubjectYear').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.Subject + Resources.Code, Resources.SubjectName,// Resources.Course + Resources.Name, Resources.AffiliationsTieUps,
            Resources.AcademicTerm,
            Resources.IsElective, Resources.HasStudyMaterial, Resources.StudyMaterialCount,
            Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'SubjectKey', index: 'SubjectKey', editable: true },
                { key: false, hidden: true, name: 'UniversityCourse', index: 'UniversityCourse', editable: true },
                { key: false, hidden: true, name: 'StudyMaterialCount', index: 'StudyMaterialCount', editable: true },
                { key: false, name: 'SubjectCode', index: 'SubjectCode', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'SubjectName', index: 'SubjectName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 160 },
                //{ key: false, name: 'Course', index: 'Course', editable: true, cellEdit: true, sortable: true, resizable: false, width: 160 },
                //{ key: false, name: 'University', index: 'University', editable: true, cellEdit: true, sortable: true, resizable: false, width: 180 },
                { key: false, name: 'SubjectYearText', index: 'SubjectYearText', editable: true, cellEdit: true, sortable: true, resizable: false, width: 110 },
                { key: false, name: 'IsElective', index: 'IsElective', editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false, width: 80, align: 'center' },
                { key: false, name: 'HasStudyMaterial', index: 'HasStudyMaterial', editable: true, cellEdit: true, formatter: formatYesorNOWithCount, sortable: true, resizable: false, width: 80, align: 'center' },
                //{ key: false, name: 'IsCommonSubject', index: 'IsCommonSubject', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudyMaterialCount', index: 'StudyMaterialCount', editable: true, cellEdit: true, sortable: true, resizable: false, width: 150 },
                { name: 'edit', search: false, index: 'SubjectKey', sortable: false, formatter: editLink, resizable: false, width: 250, align: 'center' },
            ],
            pager: jQuery('#pager'),
            rowNum: 50,
            rowList: [50,100,500],
            hidegrid: false,
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
            sortname: 'SubjectName',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            grouping: true,
            groupingView: {
                groupField: ['UniversityCourse', 'SubjectYearText'],
                groupColumnShow: [false],
                groupText: [ // user the name of a column with curly braces to use it in a summary expression. 
                    // {0} is the formula placeholder for the column (defined by the summaryType property
                    " <b>{0}</b> ",
                    " <b>{0}</b> ",
                    
                ],
                groupSummary: [false, false],
                groupCollapse: false,
                groupDataSorted: true,
                plusicon: 'ace-icon fa fa-plus-square purple',
                minusicon: 'ace-icon fa fa-minus-square-o red'

            },




            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                });

            },


        })

        $("#grid").jqGrid("setLabel", "CourseSubjectName", "", "thCourseSubjectName");
    }

    function formatYesorNOWithCount(cellValue, option, rowdata, action) {

        var StudyMaterialCount = parseInt(rowdata.StudyMaterialCount) ? parseInt(rowdata.StudyMaterialCount) : 0;
        if (cellValue == "Yes") {
            return '<i  class="fa fa-check" aria-hidden="true"></i>' ;
        }
        else {
            return '<i  class="fa fa-times" aria-hidden="true"></i>';
        }
        return cellValue;
    }

    function formatYesorNO(cellValue, option, rowdata, action) {

       
        if (cellValue == "Yes") {
            return '<i  class="fa fa-check" aria-hidden="true"></i>';
        }
        else {
            return '<i  class="fa fa-times" aria-hidden="true"></i>';
        }
        return cellValue;
    }

    var getStudyMaterialDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
                //$("[id*=OriginalIssuedDate]", $(this)).val(moment(new Date()).format("DD/MM/YYYY"))
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                $("[id*=IsActive]", $(this)).prop("checked", true).trigger("change");
                $("[id*=IsActive][type=hidden]", $(this)).val("true");
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteStudyMaterialItem($(hidden).val(), $(this));
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
                    $("#frmAddEditStudyMaterial").closest(".modal").modal("hide")
                    $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                }

            },
            data: json,
            repeatlist: 'StudyMaterials',
            submitButton: '#btnSave',
            defaultValues: json
        });
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.SubjectKey + "'";
        var HasStudyMaterial = rowdata.HasStudyMaterial;
        if (HasStudyMaterial.toLowerCase() == 'yes') {
            return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm" data-href="' + $("#hdnAddEditStudyMaterial").val() + '/' + rowdata.SubjectKey + '"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + Resources.StudyMaterial + '</a>' + '<a  class="btn btn-outline-warning btn-sm ml-3" href="' + $("#hdnAddEditSubjectModules").val() + '/' + rowdata.SubjectKey + '"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + Resources.Syllabus + '</a></div>';

        }
        else {
            return '<div class="divEditDelete"><a  class="btn btn-outline-warning btn-sm ml-3" href="' + $("#hdnAddEditSubjectModules").val() + '/' + rowdata.SubjectKey + '"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + Resources.Syllabus + '</a></div>';

        }
    }


    var getSubjectModuleDetail = function (json) {

        $('#dynamicRepeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    AppCommon.CustomRepeaterRemoteMethod();
                    $("[id*=IsActive]", $(this)).prop("checked", true).trigger("change");
                    $("[id*=HasTopics]", $(this)).prop("checked", false).trigger("change");
                    $("[id*=RowKey][type=hidden]", $(this)).val(0);
                    $("[id*=SubjectKey][type=hidden]", $(this)).val(0);
                    $("#dvModuleTopics", $(this)).html("");

                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deletSubjectModuleDetailItem($(hidden).val(), $(this));
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
                                        window.location.href = $("#hdnSyllabusAndStudyMaterialList").val();
                                    }
                                }
                            }
                        })

                    }

                },
                data: json,
                repeatlist: 'SubjectModulesModel',
                submitButton: '',
            });
    }



    function formSubmit(data) {

        var $form = $("#frmSubjectModules")
        var JsonData = [];
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {
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
                                        window.location.href = $("#hdnSyllabusAndStudyMaterialList").val();
                                    }
                                }
                            }
                        })

                    }
                },
                error: function (xhr) {

                }
            });


        }
    }



    var moduleTopics = function (form, index, IsModal) {
        var data;
        var data = $(form).serializeToJSON({
            associativeArrays: false
        });
        var obj = {};
        if (data["SubjectModulesModel"] && index != undefined) {
            data = data["SubjectModulesModel"][index];
        }
        if (data["ModulesTopicModel"]) {
            if (data["ModulesTopicModel"].length == 1 && !IsModal) {
                if (data.SubjectName) {
                    if (data.RowKey == 0) {
                        data["ModulesTopicModel"][0].TopicName = data.ModuleName
                    }
                }
                else {
                    data["ModulesTopicModel"] = [];
                }
            }
            obj.modelList = data["ModulesTopicModel"];
        }
        else if (data.ModuleName) {
            obj.modelList = [{ TopicName: data.ModuleName }]
        }
        else {
            obj.modelList = [];
        }
        if (index != undefined) {
            obj.index = index;
        }
        else {
            obj.index = $("#index").val();
        }
        var valid = true;
        if (IsModal)
            valid = $(form).valid();
        //var response = AjaxHelper.ajax("POST", $("#hdnGetRoomDetails").val(),{model:data});
        if (valid) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: $("#hdnGetModuleTopics").val(),
                data: JSON.stringify(obj),
                success: function (result) {
                    $("#dvModuleTopics", $("#dvCourseSubjectDetailsList [data-repeater-item]").eq(obj.index)).html(result);
                    AppCommon.FormatInputCase();
                    setTimeout(function () {

                    }, 500)
                    if (IsModal)
                        $(form).closest(".modal").modal("hide")
                },
                error: function (request, status, error) {

                }

            });
        }
    }


    var moduleTopicCustomPopup = function (studyMaterialData, _this, index) {
        var url = $("#hdnAddEditModuleTopics").val();
        $.customPopupform.CustomPopup({
            ajaxType: "POST",
            ajaxData: { model: studyMaterialData, index: index },
            load: function () {
                setTimeout(function () {
                    SyllabusAndStudyMaterial.GetModuleTopicsDetails($("#frmSModulesTopic").serializeToJSON({

                    }))
                    $("#frmSModulesTopic").removeData("validator");
                    $("#frmSModulesTopic").removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse($("#frmSModulesTopic"));


                }, 500)
            },
            submit: function () {
                SyllabusAndStudyMaterial.ModuleTopics($("#frmSModulesTopic"), null, true);
            },
            rebind: function (result) {


            }
        }, url);
    }

    var getModuleTopicsDetails = function (json) {
        $("#dynamicRepeaterModuleTopics").repeater(
            {
                show: function () {
                    $(this).slideDown();
                    var item = this;
                    AppCommon.FormatSelect2();
                    AppCommon.CustomRemoteMethod();
                    //$("select[name*=DealerKey]", this).on("change", function ()
                    //{
                    //    $("input[name*=DealerName]", item).val($(this).find("option:selected").text())
                    //})
                    $("[id*=RowKey][type=hidden]", $(this)).val(0);
                    $("[id*=SubjectModuleKey][type=hidden]", $(this)).val(0);
                    $("[id*=IsActive][type=hidden]", $(this)).val("true");


                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $('[name*=RowKey]', $(self));
                    var RowKey = $(hidden).val()
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteModuleTopic($(hidden).val(), $(this), remove);
                    }
                    else {
                        $(this).slideUp(remove);
                        setTimeout(function () {
                            SyllabusAndStudyMaterial.ModuleTopics($("#frmSModulesTopic"), null);
                        }, 500)
                    }


                },
                data: json,
                repeatlist: 'ModulesTopicModel',
                defaultValues: json
            });
    }

    var studyMaterialDetailsLoad = function (jsonData, item, _this) {

        var url = $("#hdnGetModuleTopics").val();
        $.ajax({
            type: "POST",
            data: jsonData,
            url: url,
            success: function (response) {
                $("[dvModuleTopics]", item).html(response)

                $(_this).closest(".modal").modal("hide");
            },
            error: function () {
            }
        });
    }

    var hasSubjectModuleEvent = function (_this) {

        var item = $(_this).closest("[data-repeater-item]");
        var HasStudyMaterial = $("input[type=checkbox][id*=HasTopics]", item)[0]
        var RowKey = $("input[type=hidden][id*=RowKey]", item)[0]

        if (HasStudyMaterial.checked) {
            SyllabusAndStudyMaterial.ModuleTopics($(_this).closest("form"), $(item).index());
        }
        else {
            if (RowKey.value != 0) {
                deleteModuleTopicAll(RowKey.value, _this);
            }
            else {
                $("#dvModuleTopics", $(item)).html("");
            }
        }
    }









    return {
        GetSyllabusAndStudyMaterial: getSyllabusAndStudyMaterial,
        GetStudyMaterialDetails: getStudyMaterialDetails,
        ModuleTopics: moduleTopics,
        ModuleTopicCustomPopup: moduleTopicCustomPopup,
        GetModuleTopicsDetails: getModuleTopicsDetails,
        FormSubmit: formSubmit,
        HasSubjectModuleEvent: hasSubjectModuleEvent,
        GetSubjectModuleDetail: getSubjectModuleDetail
    }
}());


var deleteStudyMaterialItem = function (rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_StudyMaterial,
        actionUrl: $("#hdnDeleteStudyMaterial").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");

            if (response.IsSuccessful) {
                $(item).remove();

            }
        }
    });
}

function deletSubjectModuleDetailItem(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Module,
        actionUrl: $("#hdnDeleteSubjectModules").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");
            if (response.IsSuccessful) {
                $(item).remove();

            }
        }
    });
}

function deleteModuleTopic(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ModuleTopic,
        actionUrl: $("#hdnDeleteModuleTopic").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");

            if (response.IsSuccessful) {
                $(item).remove();

            }
        }
    });
}

function deleteModuleTopicAll(rowkey, _this) {
    _this.checked = true;
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ModuleTopicAll,
        actionUrl: $("#hdnDeleteModuleTopicAll").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");

            if (response.IsSuccessful) {
                $("#dvModuleTopics", $(item)).html("");
                _this.checked = false;
            }
        }
    });
}