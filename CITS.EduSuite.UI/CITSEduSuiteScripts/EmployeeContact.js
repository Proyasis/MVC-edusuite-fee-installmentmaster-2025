
var EmployeeContacts = (function () {

    var getEmployeeContacts = function (json) {

        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    var item = $(this);
                   /* AppCommon.AutoHideDropdown();*/
                    AppCommon.CustomRepeaterRemoteMethod();
                    AppCommon.FormatInputCase();
                    setTimeout(function () {
                        //$("[id*=ProvinceKey],[id*=DistrictKey]", $(item)).each(function () { $('option', this).not(':eq(0), :selected').remove() });
                        //$(".telephone-code", $(item)).each(function () { $(this).html("") });
                        //EmployeeContacts.GetProvinceAndCodeById($("[id*=CountryKey]", $(item)).val(), $(item));
                        //$("[id*=CountryKey]").on("change", function () {
                        //    EmployeeContacts.GetProvinceAndCodeById($(this).val(), $(item));
                        //});
                        //EmployeeContacts.GetProvinceAndCodeById($("[id*=ProvinceKey]", $(item)).val(), $(item));
                        //$("[id*=ProvinceKey]").on("change", function () {
                        //    EmployeeContacts.GetDistrictById($(this).val(), $(item));
                        //});
                    }, 500)

                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteEmployeeContact($(hidden).val());
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
                         $("#tab-profile  li a.active").parent().next('li').find('a').trigger('click');
                    }

                },
                data: json,
                defaultValues: json,
                repeatlist: 'EmployeeContacts',
                submitButton: '#btnSave'
            });
    }
    var getProvinceById = function (_this) {
        var item = $(_this).closest("[data-repeater-item]");
        var obj = {};
        obj.id = $(_this).val() != "" ? $(_this).val() : 0;
       // AppCommon.BindDropDownbyId(obj, $("#hdnGetProvinceById").val(), $("select[name*=ProvinceKey]", $(item)), Resources.Province);
        //AppCommon.BindDropDownbyId(obj, $("#hdnGetProvinceById").val(), $("select[id*=ProvinceKey]", $(item)), Resources.Select + Resources.BlankSpace + Resources.Course;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetProvinceById").val(), $("select[id*=ProvinceKey]", $(item)), Resources.Province);
        //var Id = $(_this).val() != "" ? $(_this).val() : 0;
      
       

        //$.ajax({
        //    url: $("#hdnGetProvinceById").val(),
        //    type: "GET",
        //    dataType: "JSON",
        //    data: { id: Id },
        //    success: function (result) {
        //        $("select[id*=ProvinceKey]", $(item)).html(""); // clear before appending new list 
        //        $("select[id*=ProvinceKey]", $(item)).append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Province));
        //        $.each(result.Provinces, function (i, Province) {
        //            $("select[id*=ProvinceKey]", $(item)).append(
        //                $('<option></option>').val(Province.RowKey).html(Province.Text));
        //        });
        //        $("select[id*=ProvinceKey]", $(item)).selectpicker('refresh');
        //    }
        //});
    }
   
    var getDistrictById = function (_this) {

        var item = $(_this).closest("[data-repeater-item]");
        var obj = {};
        obj.id = $(_this).val() != "" ? $(_this).val() : 0;
       // AppCommon.BindDropDownbyId(obj, $("#hdnGetDistrictByProvince").val(), $("select[id*=DistrictKey]", $(item)), Resources.District);
        AppCommon.BindDropDownbyId(obj, $("#hdnGetDistrictByProvince").val(), $("select[id*=DistrictKey]", $(item)), Resources.District);
        //var Id = $(_this).val() != "" ? $(_this).val() : 0;
               
        //$.ajax({
        //    url: $("#hdnGetDistrictByProvince").val(),
        //    type: "GET",
        //    dataType: "JSON",
        //    data: { id: Id },
        //    success: function (result) {
        //        $("select[id*=DistrictKey]", $(item)).html(""); // clear before appending new list 
        //        $("select[id*=DistrictKey]", $(item)).append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.District));
        //        $.each(result.Districts, function (i, Districts) {
        //            $("select[id*=DistrictKey]", $(item)).append(
        //                $('<option></option>').val(Districts.RowKey).html(Districts.Text));
        //        });
        //        $("select[id*=DistrictKey]", $(item)).selectpicker('refresh');
        //    }
        //});
       
    }

 


    return {
        GetEmployeeContacts: getEmployeeContacts,
        GetProvinceById: getProvinceById,
        GetDistrictById: getDistrictById
    }

}());

function deleteEmployeeContact(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_EmployeeContact,
        actionUrl: $("#hdnDeleteEmployeeContact").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            Employee.ReloadData();
        }
    });
}




