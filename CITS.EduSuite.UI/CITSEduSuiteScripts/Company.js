var Company = (function () {

    var getCompany = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetCompany").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()

                }
            },
            colNames: [Resources.RowKey, Resources.CompanyName, Resources.CompanySubName, Resources.WebSite, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'CompanyName', index: 'CompanyName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CompanySubName', index: 'CompanySubName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Website', index: 'Website', editable: true, cellEdit: true, sortable: true, resizable: false },
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
                    AppCommon.EditGridPopup($(this), CompanyPopupLoad);
                })
            }
        })

        $("#grid").jqGrid("setLabel", "CompanyName", "", "thCompanyName");
    }
          

    var deleteCompanyLogo = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationPhoto,
            actionUrl: $("#hdnDeleteCompanyLogo").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                $("#CompanyLogos").removeAttr('src');
                $("#CompanyLogos").show();
                $("#btnPhotoDelete").hide();
            }
        });
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditCompany").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>';
    }

    var formSubmit = function () {
        var $form = $("#frmCompany");

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
        GetCompany: getCompany,       
        FormSubmit: formSubmit,
        DeleteCompanyLogo: deleteCompanyLogo
    }
}());

function deleteCompany(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Company,
        actionUrl: $("#hdnDeleteCompany").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}


function CompanyPopupLoad() {

    AppCommon.FormatInputCase();

    $("#CountryKey").on("change", function () {
        Company.GetProvinceAndCodeById($(this).val());

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
                $('#CompanyLogos').attr('src', e.target.result);
            }
            reader.readAsDataURL(this.files[0]);
        }
    });

}


