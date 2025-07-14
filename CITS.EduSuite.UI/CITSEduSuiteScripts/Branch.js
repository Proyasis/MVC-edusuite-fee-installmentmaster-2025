var Branch = (function () {

    var getBranch = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBranch").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()

                }
            },
            colNames: [Resources.RowKey, Resources.Branch + Resources.BlankSpace + Resources.Code, Resources.Branch + Resources.BlankSpace + Resources.Name, Resources.Address, Resources.City + Resources.BlankSpace + Resources.Name, Resources.District + Resources.BlankSpace + Resources.Name, Resources.PhoneNumber, Resources.EmailAddress, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BranchCode', index: 'BranchCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AddressLine1', index: 'AddressLine1', editable: true, cellEdit: true, sortable: true, resizable: false },
                // { key: false, name: 'AddressLine2', index: 'AddressLine2', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'AddressLine3', index: 'AddressLine3', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CityName', index: 'CityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DistrictName', index: 'DistrictName', editable: true, cellEdit: true, sortable: true, resizable: false },
                // { key: false, name: 'PostalCode', index: 'PostalCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PhoneNumber1', index: 'PhoneNumber1', editable: true, cellEdit: true, sortable: true, resizable: false },
                // { key: false, name: 'PhoneNumber2', index: 'PhoneNumber2', editable: true, cellEdit: true, sortable: true, resizable: false },
                // { key: false, name: 'FaxNumber', index: 'FaxNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmailAddress', index: 'EmailAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ContactPersonName', index: 'ContactPersonName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ContactPersonPhone', index: 'ContactPersonPhone', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'DepartmentCount', index: 'DepartmentCount', editable: true, cellEdit: true, sortable: true, resizable: false },

                //{ key: false, name: 'OpeningCashBalance', index: 'OpeningCashBalance', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'CurrentCashBalance', index: 'CurrentCashBalance', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
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
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this), BranchPopupLoad);
                })
            }
        })

        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
    }

    var getProvinceAndCodeById = function (Id) {
        var obj = {};
        obj.id = $("#CountryKey").val() != "" ? $("#CountryKey").val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetProvinceAndCodeByCountry").val(), $("#ProvinceKey"), Resources.Province);
    }

    var getDistrictById = function (Id) {

        $("#DistrictKey").html(""); // clear before appending new list 
        $("#DistrictKey").append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.District));

        $.ajax({
            url: $("#hdnGetDistrictByProvince").val(),
            type: "GET",
            dataType: "JSON",
            data: { id: Id },
            success: function (result) {
                $.each(result.Districts, function (i, District) {
                    $("#DistrictKey").append(
                        $('<option></option>').val(District.RowKey).html(District.Text));
                });
            }
        });
    }

    var deleteBranchLogo = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationPhoto,
            actionUrl: $("#hdnDeleteBranchLogo").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                $("#BranchLogos").removeAttr('src');
                $("#BranchLogos").show();
                $("#btnPhotoDelete").hide();
            }
        });
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditBranch").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBranch(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
    }

    var formSubmit = function () {
        var $form = $("#frmBranch");

        var formdata = $form.serializeToJSON({
            associativeArrays: false
        })

        if ($form.valid()) {

            var url = $form.attr("action");


            AjaxHelper.ajaxWithFileAsync("model", "POST", url, { model: formdata }, function () {
                response = this;
                if (response.IsSuccessful == true) {

                    //$.alert({
                    //    type: 'green',
                    //    title: Resources.Success,
                    //    content: response.Message,
                    //    icon: 'fa fa-check-circle-o-',
                    //    buttons: {
                    //        Ok: {
                    //            text: Resources.Ok,
                    //            btnClass: 'btn-success',
                    //            action: function () {
                    //                $('[data-dismiss="modal"]').trigger('click');
                    //                window.location.reload();
                    //            }
                    //        }
                    //    }
                    //})
                   
                }
                else {
                    toastr.error(Resources.Failed, response.Message);
                    $("[data-valmsg-for=error_msg_payment]").html(response.Message);

                    $("[data-valmsg-for=error_msg]").html(response.Message);

                }
                //$('[data-dismiss="modal"]').trigger('click');
                //$("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            });

            $('[data-dismiss="modal"]').trigger('click');
            window.location.reload();
        }
    }


    return {
        GetBranch: getBranch,
        GetProvinceAndCodeById: getProvinceAndCodeById,
        GetDistrictById: getDistrictById,
        FormSubmit: formSubmit,
        DeleteBranchLogo: deleteBranchLogo
    }
}());

function deleteBranch(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Branch,
        actionUrl: $("#hdnDeleteBranch").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}


function BranchPopupLoad() {

    AppCommon.FormatInputCase();

    $("#CountryKey").on("change", function () {
        Branch.GetProvinceAndCodeById($(this).val());

    });

    $("#ProvinceKey").on("change", function () {
        var obj = {};
        obj.id = $(this).val() != "" ? $(this).val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnGetDistrictByProvince").val(), $("#DistrictKey"), Resources.District);
    });
    $(".telephone-code").each(function () {
        AppCommon.SetInputAddOn($(this));
    })

    $("#PhotoFile").change(function () {
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#BranchLogos').attr('src', e.target.result);
            }
            reader.readAsDataURL(this.files[0]);
        }
    });

}


