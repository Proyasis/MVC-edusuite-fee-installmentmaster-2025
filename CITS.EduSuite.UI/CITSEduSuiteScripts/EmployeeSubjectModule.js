 var ajaxRequest = null;
var EmployeeSubjectModule = (function () {

   
 

    var employeeSubjectModuleCustomPopup = function (_this) {

        var ListItem = $(_this).closest("[data-repeater-item]");
        var obj = {};
    
        obj.SubjectKey = $("input[type=hidden][id*=SubjectKey]", ListItem).val();
        obj.TeacherClassAllocationKey = $("input[type=hidden][id*=TeacherClassAllocationKey]", ListItem).val();
        obj.EmployeeKey = $("input[type=hidden][id*=EmployeeKey]", ListItem).val();
        obj.ClassDetailsKey = $("input[type=hidden][id*=ClassDetailsKey]", ListItem).val();

     


        var url = $("#hdnAddEditEmployeeSubjectModule").val();
        $.customPopupform.CustomPopup({
            ajaxType: "POST",
            ajaxData: { model: obj },
            load: function () {
                setTimeout(function () {

                    $("#frmSModulesTopic").removeData("validator");
                    $("#frmSModulesTopic").removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse($("#frmSModulesTopic"));
                }, 500)
            },

            rebind: function (result) {
                EmployeeSubjectModule.BindEmployeeSubjectModule();

            }
        }, url);
    }
      
    var getEmployeeDetails = function () {

        var model = {};
        model.EmployeeCode = $("#EmployeeCode").val();
        if (model.EmployeeCode) {

            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnEmployeeSubjectModule").val(),
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
                    $("#DivCourseFee").html("")
                    $("#DivCourseFee").html(result);
                },
                error: function (request, status, error) {

                }
            });
        }
    }

    var bindEmployeeSubjectModule = function () {
        $("#dvInstallmentDetails").mLoading()
        $("#dvInstallmentDetails").load($("#hdnBindEmployeeSubjectModule").val());
    }
   
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.Rowkey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-primary btn-sm mx-1" data-href="' + $("#hdnAddEditBatch").val() + '/' + rowdata.Rowkey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-danger btn-sm mx-1" href="#"   onclick="javascript:deleteBatch(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    return {
        EmployeeSubjectModuleCustomPopup: employeeSubjectModuleCustomPopup,
        GetEmployeeDetails: getEmployeeDetails,
        BindEmployeeSubjectModule: bindEmployeeSubjectModule,
           }

}());

