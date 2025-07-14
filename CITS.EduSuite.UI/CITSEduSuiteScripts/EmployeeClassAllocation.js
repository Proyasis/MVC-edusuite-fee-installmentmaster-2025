var ajaxRequest = null;
var EmployeeClassAllocation = (function () {

    var getEmployeeClassAllocation = function (json) {
        $('.repeater').repeater(
           {
               show: function () {
                   $(this).slideDown();
                   AppCommon.CustomRepeaterRemoteMethod();
                   AppCommon.FormatInputCase();
                   $("[name*=RowKey]", $(this)).val(0)

                   $("[name*=SubjectKeys]", $(this)).selectpicker();
               },
               hide: function (remove) {
                   var self = $(this).closest('[data-repeater-item]').get(0);
                   var hidden = $(self).find('input[type=hidden]')[0];
                   if ($(hidden).val() != "" && $(hidden).val() != "0") {
                       deleteClassAllocation($(hidden).val(), $(this));
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
                       //  $("#tab-profile  li a.active").parent().next('li').find('a').trigger('click');
                   }
                   //Employee.ReloadData();
               },
               data: json,
               repeatlist: 'TeacherClassAllocationModel',
               submitButton: ''
           });
    }
    var editSubjectPopUp = function () {
        //var obj = {};
        //obj.ClassDetailsKeys = ClassDetailsKeys;
        //obj.EmployeeKey = $("#EmployeeKey").val() != "" ? $("#EmployeeKey").val() : 0;

        var URL = $("#hdnGetSubjectDetails").val() // + '?' + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function () {

            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    toastr.success(Resources.Success, result.Message);
                     $("#tab-profile  li a.active").parent().next('li').find('a').trigger('click');
                }
                else {
                    toastr.error(Resources.Failed, result.Message);
                }

            },
            ajaxType: "Post",
            ajaxData: AppCommon.ObjectifyForm($("form").serializeArray())
        }, URL);

    }

    var checkIncharge = function (_this) {
        if (_this.checked) {
            var item = $(_this).closest("[data-repeater-item]");

            var obj = {}
            obj.EmployeeKey = $("#EmployeeKey").val();
            obj.ClassDetailsKey = $('select[id*=ClassDetailsKey]', $(item)).val();
            obj.BatchKey = $('select[id*=BatchKey]', $(item)).val();

            
            $.ajax({
                url: $("#hdnCheckIncharge").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                success: function (result) {
                    if (result.IsSuccessful) {
                        //$("input[type=checkbox][name*=InCharge]", $(item)).prop("checked", true).trigger("change");
                        $("[data-repeater-item]").each(function (index) {

                        });    
                    } else {
                        $(_this).prop("checked", false)
                        EduSuite.AlertMessage({ title: Resources.Warning, content: result.Message, type: 'orange' })
                        //$("input[type=checkbox][name*=InCharge]", $(item)).prop("checked", false).trigger("change");
                    }
                }
            });
        }
    }

    var getSubjectByClass = function (_this) {      
        var obj = {};
        obj.ClassDetailsKey = $(_this).val() != "" ? $(_this).val() : 0;
        obj.EmployeesKey = $("#EmployeesKey").val() != "" ? $("#EmployeesKey").val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetBatchDetails").val(), $("select[id*=BatchKey]"), Resources.Batch);
        AppCommon.BindDropDownbyId(obj, $("#hdnGetSubjectDetails").val(), $("select[id*=SubjectKeys]"), Resources.Subject);       
    }
    var getBatchByClass = function (_this) {      
        var obj = {};
        obj.ClassDetailsKey = $(_this).val() != "" ? $(_this).val() : 0;
        obj.EmployeesKey = $("#EmployeesKey").val() != "" ? $("#EmployeesKey").val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetSubjectDetails").val(), $("select[id*=SubjectKeys]"), Resources.Subject);
    }

    var addEditClassAllocation = function (id, EmployeesKey) {

        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.EmployeesKey = EmployeesKey;
        var url = $("#hdnAddEditEmployeeClassAllocation").val() + "/" + id + "?" + $.param(obj);
        $("#dvAddEditClassAllocation").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvAddEditClassAllocation").html(response)
                    $.validator.unobtrusive.parse("form");
                    //$("#frmClassAllocation", $("#dvAddEditClassAllocation")).on("submit", function () {
                    //    formSubmit();
                    //    return false;
                    //})
                    //ApplicationFeePayment.BindTotalFeeDetails();
                    //ApplicationFeePayment.BindInstallmentFeeDetails();
                }
                else {
                    $("#dvAddEditClassAllocation").mLoading("destroy");
                }
            }
        })

    }
    var bindEmployeeClassDetails = function () {
        $("#dvClassDetails").mLoading()
        $("#dvClassDetails").load($("#hdnBindEmployeeClassDetails").val());
    }

    function formSubmit() {
        $("#btnSave").hide();
        var $form = $("#frmClassAllocation")
        var JsonData = [];

        var formData = $form.serializeToJSON({
            associativeArrays: false
        });

        var id = formData['EmployeesKey'];
        if ($form.valid()) {
            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
            $("#btnSave").hide();
            if (response.IsSuccessful) {
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
                                window.location.href = $("#hdnEmployeeClass").val() + "/" + id;
                                $("#btnSave").show();
                            }
                        }
                    }
                })

            }
            else if (!response.IsSuccessful) {
                $("[data-valmsg-for=error_msg]").html(response.Message);
                $("#btnSave").show();
            }

        }
        else {
            $("#btnSave").show();
        }

        //var $form = $("#frmClassAllocation")

        //if ($form.valid()) {
        //    var dataurl = $form.attr("action");
        //    var response = [];

        //    $.ajax({
        //        url: dataurl,
        //        datatype: "json",
        //        type: "POST",
        //        contenttype: 'application/json; charset=utf-8',
        //        async: false,
        //        data: $form.serializeArray(),
        //        success: function (response) {
        //            if (typeof response == "string") {
        //                $("[data-valmsg-for=error_msg]").html(response);
        //            }
        //            else if (response.IsSuccessful) {
        //                $.alert({
        //                    type: 'green',
        //                    title: Resources.Success,
        //                    content: response.Message,
        //                    icon: 'fa fa-check-circle-o-',
        //                    buttons: {
        //                        Ok: {
        //                            text: Resources.Ok,
        //                            btnClass: 'btn-success',
        //                            action: function () {
        //                                 $("#tab-profile  li a.active").parent().next('li').find('a').trigger('click');
        //                            }
        //                        }
        //                    }
        //                })
        //            }
        //        },
        //        error: function (xhr) {

        //        }
        //    });
        //}
    }

    var loadData = function () {
        

        var model = {};
        $("#dynamicRepeater").mLoading();
        model.EmployeeCode = $("#EmployeeCode").val();
        model.EmployeeKey = $("#EmployeeKey").val();
        if (model.EmployeeCode != "" && model.EmployeeCode != null || (model.EmployeeKey != 0)) {


            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnUrl").val(),
                contentType: "application/json; charset=utf-8",
                data: model,
                beforeSend: function () {
                    if (ajaxRequest != null) {
                        ajaxRequest.abort();
                    }
                },
                success: function (result) {
                    if (result.IsSuccessful == false) {
                        $("[data-valmsg-for=error_msg]").html(result.Message);

                    }
                    $("#DivEmployeeDetails").html("")
                    $("#DivEmployeeDetails").html(result);
                },
                error: function (request, status, error) {

                }
            });

        }
    }
    function deleteClassAllocation(Rowkey, _this) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ClassAllocation,
            actionUrl: $("#hdnDeleteClassAllocation").val(),
            actionValue: Rowkey,
            dataRefresh: function () {
                EmployeeClassAllocation.LoadData();
            }
        });

    }

    return {
        FormSubmit: formSubmit,
        EditSubjectPopUp: editSubjectPopUp,
        GetEmployeeClassAllocation: getEmployeeClassAllocation,
        CheckIncharge: checkIncharge,
        GetSubjectByClass: getSubjectByClass,
        GetBatchByClass: getBatchByClass,
        LoadData: loadData,
        AddEditClassAllocation: addEditClassAllocation,
        BindEmployeeClassDetails: bindEmployeeClassDetails,
        DeleteClassAllocation: deleteClassAllocation
    }
}());

