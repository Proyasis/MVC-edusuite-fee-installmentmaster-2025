var CourseSubject = (function ()
{
    var getCourseSubject = function ()
    {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetCourseSubject").val(),
            datatype: 'json',
            mtype: 'POST',
            postData: {
                searchText: function ()
                {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Course + Resources.Name, Resources.AffiliationsTieUps, Resources.AcademicTerm, Resources.NoOfSubjects, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                { key: true, hidden: true, name: 'UniversityKey', index: 'UniversityKey', editable: true },
                { key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                 { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'UniversityName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },               
                { key: false, name: 'AcademicTermName', index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfSubject', index: 'NoOfSubject', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',



        })

        $("#grid").jqGrid("setLabel", "CourseSubjectName", "", "thCourseSubjectName");
    }
    var getCourseSubjectDetail = function (json)
    {
        $('#dynamicRepeater').repeater(
            {
                show: function ()
                {
                    $(this).slideDown();
                    AppCommon.CustomRepeaterRemoteMethod();
                    //$("[id*=IsActive][type=checkbox]", $(this)).prop("checked", true);
                    //$("[id*=HasStudyMaterial][type=checkbox]", $(this)).prop("checked", true);
                    $("[id*=IsActive]", $(this)).prop("checked", true).trigger("change");
                    $("[id*=HasStudyMaterial]", $(this)).prop("checked", true).trigger("change");
                    $("[id*=RowKey][type=hidden]", $(this)).val(0);
                    $("[id*=SubjectKey][type=hidden]", $(this)).val(0);
                    $("#dvStudyMaterials", $(this)).html("");

                },
                hide: function (remove)
                {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0")
                    {
                        deletCourseSubjectDetailsItem($(hidden).val(), $(this));
                    }
                    else
                    {
                        $(this).slideUp(remove);
                    }
                },
                rebind: function (response)
                {
                    if (typeof response == "string")
                    {
                        $("[data-valmsg-for=error_msg]").html(response);
                    }
                    else if (response.IsSuccessful)
                    {
                        $.alert({
                            type: 'green',
                            title: Resources.Success,
                            content: response.Message,
                            icon: 'fa fa-check-circle-o-',
                            buttons: {
                                Ok: {
                                    text: Resources.Ok,
                                    btnClass: 'btn-success',
                                    action: function ()
                                    {
                                        window.location.href = $("#hdnCourseSubjectList").val();
                                    }
                                }
                            }
                        })

                    }

                },
                data: json,
                repeatlist: 'CourseSubjectDetailViewModel',
                submitButton: '',
                defaultValues: json
            });
    }

    function editLink(cellValue, options, rowdata, action)
    {
        var temp = rowdata.CourseKey + '' + ',' + '' + rowdata.UniversityKey + '' + ',' + '' + rowdata.AcademicTermKey;
        //var obj = {};
        //var objDelete = [rowdata.CourseKey, rowdata.UniversityKey, rowdata.AcademicTermKey];
        //obj.CourseId = rowdata.CourseKey;
        //obj.UniversityId = rowdata.UniversityKey;
        //obj.AcademicTermKey = rowdata.AcademicTermKey;
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mr-1" href="' + $("#hdnAddEditCourseSubject").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteCourseSubjectAll(' + rowdata.RowKey + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    var loadData = function (json)
    {
       
        var model = json;

        model.AcademicTermKey = $("#AcademicTermKey").val();
        model.CourseKey = $("#CourseKey").val();
        model.UniversityMasterKey = $("#UniversityMasterKey").val();
        //model.CourseYearKey = $("#CourseYearKey").val();
        model.CourseYearKey = $("#tab-classyear li a.active").data('val');
       
        model.RowKey = $("#RowKey").val();

        
        $.ajax({
            type: "POST",
            url: $("#hdnUrl").val(),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            success: function (result)
            {
                if (result.IsSuccessful == false)
                {
                    $("[data-valmsg-for=error_msg]").html(result.Message);

                }
                $("#dvCourseSubjectDetails").html("")
                $("#dvCourseSubjectDetails").html(result);

            },
            error: function (request, status, error)
            {

            }
        });
    }

    function formSubmit(data)
    {
        $("#dvCourseSubjectDetails").mLoading();
        var $form = $("#frmCourseSubject")
        $form.mLoading();
        var JsonData = [];
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        formData.CourseYearKey = $("#tab-classyear li a.active").data('val');
        
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
                success: function (response) {
                    if (typeof response == "string") {
                        $("[data-valmsg-for=error_msg]").html(response);
                    }
                    else if (response.IsSuccessful) {
                        toastr.success(Resources.Success, response.Message);
                        var CourseYear = response.CourseYear.length;
                        if (response.CourseYearKey == CourseYear) {
                            window.location.href = $("#hdnCourseSubjectList").val();
                        }
                        else {
                            $("#tab-classyear ul  li a.active").parent().nextAll('li:visible').first().find('a').trigger('click');
                        }
                        $form.mLoading("destroy");
                        //$.alert({
                        //    type: 'green',
                        //    title: Resources.Success,
                        //    content: data.Message,
                        //    icon: 'fa fa-check-circle-o-',
                        //    buttons: {
                        //        Ok: {
                        //            text: Resources.Ok,
                        //            btnClass: 'btn-success',
                        //            action: function ()
                        //            {
                        //                window.location.href = $("#hdnCourseSubjectList").val();
                        //            }
                        //        }
                        //    }
                        //})

                    }
                },
                error: function (xhr) {

                }
            });

            $("#dvCourseSubjectDetails").mLoading("destroy");
        }
        else {
            $form.mLoading("destroy");
            $("#dvCourseSubjectDetails").mLoading("destroy");
        }
       
    }

    var studyMaterials = function (form, index, IsModal)
    {
        
        var data;
        var data = $(form).serializeToJSON({
            associativeArrays: false
        });
        var obj = {};
        if (data["CourseSubjectDetailViewModel"] && index != undefined)
        {
            data = data["CourseSubjectDetailViewModel"][index];
        }
        if (data["StudyMaterials"])
        {
            if (data["StudyMaterials"].length == 1 && !IsModal)
            {
                if (data.SubjectName)
                {
                    if (data.RowKey == 0)
                    {
                        data["StudyMaterials"][0].StudyMaterialCode = data.SubjectCode
                        data["StudyMaterials"][0].StudyMaterialName = data.SubjectName
                    }
                }
                else
                {
                    data["StudyMaterials"] = [];
                }
            }
            obj.modelList = data["StudyMaterials"];
        }
        else if (data.SubjectName)
        {
            obj.modelList = [{ StudyMaterialCode: data.SubjectCode, StudyMaterialName: data.SubjectName }]
        }
        else
        {
            obj.modelList = [];
        }
        if (index != undefined)
        {
            obj.index = index;
        }
        else
        {
            obj.index = $("#index").val();
        }
        var valid = true;
        if (IsModal)
            valid = $(form).valid();
        //var response = AjaxHelper.ajax("POST", $("#hdnGetRoomDetails").val(),{model:data});
        if (valid)
        {
            
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: $("#hdnGetStudyMaterials").val(),
                data: JSON.stringify(obj),
                success: function (result)
                {
                    $("#dvStudyMaterials", $("#dvCourseSubjectDetailsList [data-repeater-item]").eq(obj.index)).html(result);
                    AppCommon.FormatInputCase();
                    setTimeout(function ()
                    {

                    }, 500)
                    if (IsModal)
                        $(form).closest(".modal").modal("hide")
                },
                error: function (request, status, error)
                {

                }

            });
        }
    }


    var studyMaterialCustomPopup = function (studyMaterialData, _this, index)
    {
        
        var url = $("#hdnAddEditStudyMaterialsDetails").val();
        $.customPopupform.CustomPopup({
            ajaxType: "POST",
            ajaxData: { model: studyMaterialData, index: index },
            load: function ()
            {
                setTimeout(function ()
                {
                    
                    CourseSubject.GetStudyMaterialsDetails($("#frmStudyMaterialDetails").serializeToJSON({

                    }))
                    $("#frmStudyMaterialDetails").removeData("validator");
                    $("#frmStudyMaterialDetails").removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse($("#frmStudyMaterialDetails"));


                }, 500)
            },
            submit: function ()
            {
                
                CourseSubject.StudyMaterials($("#frmStudyMaterialDetails"), null, true);
            },
            rebind: function (result)
            {


            }
        }, url);
    }

    var getStudyMaterialsDetails = function (json)
    {
        
        $("#dynamicRepeaterStudyMaterial").repeater(
            {
                show: function ()
                {
                    $(this).slideDown();
                    var item = this;
                    AppCommon.FormatSelect2();
                    AppCommon.CustomRemoteMethod();
                    //$("select[name*=DealerKey]", this).on("change", function ()
                    //{
                    //    $("input[name*=DealerName]", item).val($(this).find("option:selected").text())
                    //})
                    $("[id*=RowKey][type=hidden]", $(this)).val(0);
                    $("[id*=SubjectKey][type=hidden]", $(this)).val(0);
                    $("[id*=IsActive][type=hidden]", $(this)).val("true");


                },
                hide: function (remove)
                {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $('[name*=RowKey]', $(self));
                    var RowKey = $(hidden).val()
                    if ($(hidden).val() != "" && $(hidden).val() != "0")
                    {
                        deleteStudyMaterialItem($(hidden).val(), $(this), remove);
                    }
                    else
                    {
                        $(this).slideUp(remove);
                        setTimeout(function ()
                        {
                            CourseSubject.StudyMaterials($("#frmStudyMaterialDetails"), null);
                        }, 500)
                    }


                },
                data: json,
                repeatlist: 'StudyMaterials',
                defaultValues: json
            });
    }

    var studyMaterialDetailsLoad = function (jsonData, item, _this)
    {
        
        var url = $("#hdnGetStudyMaterials").val();
        $.ajax({
            type: "POST",
            data: jsonData,
            url: url,
            success: function (response)
            {
                $("[dvStudyMaterials]", item).html(response)

                $(_this).closest(".modal").modal("hide");
            },
            error: function ()
            {
            }
        });
    }

    var hasStudyMaterialEvent = function (_this)
    {
        var item = $(_this).closest("[data-repeater-item]");
        var HasStudyMaterial = $("input[type=checkbox][id*=HasStudyMaterial]", item)[0]
        var SubjectKey = $("input[type=hidden][id*=SubjectKey]", item)[0]

        if (HasStudyMaterial.checked)
        {
            CourseSubject.StudyMaterials($(_this).closest("form"), $(item).index());
        }
        else
        {
            if (SubjectKey.value != 0)
            {
                deleteStudyMaterialAll(SubjectKey.value, _this);
            }
            else
            {
                $("#dvStudyMaterials", $(item)).html("");
            }
        }
    }

    var getCourseYear = function (obj, ddl) {
        
        //$(ddl).html("");
        //$(ddl).append($('<option></option>').val("").html(Resources.Year));
        //$.ajax({
        //    url: $("#hdnFillCourseYear").val(),
        //    type: "GET",
        //    dataType: "JSON",
        //    data: obj,
        //    contentType: "application/json; charset=utf-8",
        //    success: function (result) {
        //        $.each(result, function (i, Year) {
        //            $(ddl).append(
        //                $('<option></option>').val(Year.RowKey).html(Year.Text));

        //            $("ul").append("<li>Appended item</li>");
        //        });
        //    }
        //});

        var obj = {};
        obj.AcademicTermKey = $("#AcademicTermKey").val() != "" ? $("#AcademicTermKey").val() : 0;
        obj.key = $("#CourseKey").val() != "" ? $("#CourseKey").val() : 0;
        $.ajax({
            url: $("#hdnFillCourseYear").val(),
            type: "GET",
            data: obj,
            success:function(result)
            {
                $("#dvCourseYear").html("")
                $("#dvCourseYear").html(result);
            }
        })
    }
    return {
        GetCourseSubject: getCourseSubject,
        GetCourseSubjectDetail: getCourseSubjectDetail,
        LoadData: loadData,
        StudyMaterials: studyMaterials,
        FormSubmit: formSubmit,
        StudyMaterialCustomPopup: studyMaterialCustomPopup,
        GetStudyMaterialsDetails: getStudyMaterialsDetails,
        StudyMaterialDetailsLoad: studyMaterialDetailsLoad,
        HasStudyMaterialEvent: hasStudyMaterialEvent,
        GetCourseYear: getCourseYear
    }
}());

function deleteCourseSubjectAll(id)
{

    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_CourseSubject,
        actionUrl: $("#hdnDeleteCourseSubjectAll").val(),
        actionValue: id,
        dataRefresh: function ()
        {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deletCourseSubjectDetailsItem(rowkey, _this)
{
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Subject,
        actionUrl: $("#hdnDeleteCourseSubjectItem").val(),
        actionValue: rowkey,
        dataRefresh: function (response)
        {
            var item = $(_this).closest("[data-repeater-item]");
            if (response.IsSuccessful)
            {
                $(item).remove();

            }
        }
    });
}

function deleteStudyMaterialItem(rowkey, _this)
{
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_CourseSubject,
        actionUrl: $("#hdnDeleteStudyMaterial").val(),
        actionValue: rowkey,
        dataRefresh: function (response)
        {
            var item = $(_this).closest("[data-repeater-item]");

            if (response.IsSuccessful)
            {
                $(item).remove();
               
            }
        }
    });
}


function deleteStudyMaterialAll(rowkey, _this)
{
    _this.checked = true;
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_StudyMaterialAll,
        actionUrl: $("#hdnDeleteStudyMaterialAll").val(),
        actionValue: rowkey,
        dataRefresh: function (response)
        {
            var item = $(_this).closest("[data-repeater-item]");

            if (response.IsSuccessful)
            {              
                $("#dvStudyMaterials", $(item)).html("");
                _this.checked = false;
            }
        }
    });
}


