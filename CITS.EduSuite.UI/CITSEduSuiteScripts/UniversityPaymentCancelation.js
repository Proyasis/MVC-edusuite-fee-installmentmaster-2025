var UniversityPaymentCancelation = (function () {

    var getUniversityPaymentCancelations = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetUniversityCancelation").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                SearchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Date, Resources.Name, Resources.VoucherNumber, Resources.Amount,
            //Resources.BankCharge,
            Resources.Remarks, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'ApplicationKey', index: 'ApplicationKey', editable: true },
                { key: false, name: 'CancelDate', index: 'CancelDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'StudentName', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'VoucherNo', index: 'VoucherNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TotalAmount', index: 'TotalAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Remarks', index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [5, 10, 20, 50, 100],
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
            altRows: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altclass: 'jqgrid-altrow',

        })


        $("#grid").jqGrid("setLabel", "TransactionDate", "", "thTransactionDate");


    }
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        // return '<div class="divEditDelete"><a class="btn btn-primary btn-xs" data-modal="" data-href="AddEditCashTransactions/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-xs" onclick="javascript:deleteCashTransactions(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a></div>';

        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" onclick="UniversityPaymentCancelation.EditPopup(' + rowdata.RowKey + ',' + rowdata.ApplicationKey + ')" ><i class="fa fa-pencil" aria-hidden="true"></i></a><a  class="btn btn-outline-danger btn-sm mx-1" href="#"  onclick="javascript:deleteUniversityPaymentCancelation(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';

    }


    var editPopup = function (Id, ApplicationKey, UniversityPaymentDetailsKey) {

        var obj = {};


        obj.ApplicationKey = ApplicationKey;
        obj.UniversityPaymentDetailsKey = UniversityPaymentDetailsKey;
        //obj.Id = Id;
        url = $("#hdnAddEditUniversityPaymentCancelation").val() + "/" + Id + "?" + $.param(obj);

        //var url = $("#hdnAddEditUniversityPaymentCancelation").val() + '?' + $.param(obj);
        var validator = null

        $.customPopupform.CustomPopup({
            modalsize: "modal-lg ",
            load: function () {

                UniversityPaymentCancelation.CheckServiceCharge();
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.href = $("#hdnUniversityPaymentCancelationList").val();
                }
            }
        }, url);

    }

    var getUniversityFees = function () {

        $("#DivDayToDayUniversityFees").mLoading();
        var JsonData = $("form").serializeToJSON({

        });
        JsonData.page = 1
        JsonData.rows = 999,
            JsonData.sord = "desc",
            JsonData.sidx = "RowKey"
        $.ajax({
            type: "POST",
            url: $("#hdnBindUniversityPaymentList").val(),
            data: JsonData,
            success: function (result) {
                $("#DivDayToDayUniversityFees").html("")
                $("#DivDayToDayUniversityFees").html(result);
                $("#DivDayToDayUniversityFees").mLoading("destroy");
            }


        })
    }

    var checkServiceCharge = function () {

        //$("#IfServiceCharge").prop("checked", true);
        // $("#IfServiceCharge").val(true);

        //var ifServiceCharge = $("#IfServiceCharge").val();
        var ifServiceCharge = $("#IfServiceCharge").prop("checked");

        //ifServiceCharge = JSON.parse(ifServiceCharge) ? JSON.parse(ifServiceCharge) : true;

        if (ifServiceCharge) {
            $(".divServiceCharge").show();
        }
        else {
            $(".divServiceCharge").hide();
            $("#AccountHeadKey,#ServiceFee").each(function () {
                $(this).val("")
            });

            $("#AccountHeadKey").val("").selectpicker("refresh");

        }
        UniversityPaymentCancelation.CalculateDeductionFee();
    }

    var calculateDeductionFee = function () {

        var TotalFee = $("#TotalAmount").val();
        TotalFee = parseFloat(TotalFee) ? parseFloat(TotalFee) : 0;

        var ServiceFee = $("#ServiceFee").val();
        ServiceFee = parseFloat(ServiceFee) ? parseFloat(ServiceFee) : 0;
        if (ServiceFee > TotalFee) {
            ServiceFee = TotalFee;
            $("#ServiceFee").val(ServiceFee)
        }
        var DeductionFee = TotalFee - ServiceFee;
        DeductionFee = parseFloat(DeductionFee) ? parseFloat(DeductionFee) : 0;
        $("#TotalDeductionFee").val(DeductionFee);
    }


    function formSubmit() {

        var $form = $("#frmUniversityPaymentCancelation")

        $("[disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {

            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
            if (typeof response == "string") {
                $("[data-valmsg-for=error_msg]").html(response);
            }
            else if (response.IsSuccessful) {

                $('#myModal').modal('hide');
                window.location.href = $("#hdnUniversityPaymentCancelationList").val();

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
                //                StudentLate.BindStudentLateDetails();
                //                StudentLate.AddEditStudentLate(0, id);
                //            }
                //        }
                //    }
                //})

            }

        }
    }

    return {
        GetUniversityPaymentCancelations: getUniversityPaymentCancelations,
        EditPopup: editPopup,
        GetUniversityFees: getUniversityFees,
        CheckServiceCharge: checkServiceCharge,
        CalculateDeductionFee: calculateDeductionFee,
        FormSubmit: formSubmit
    }

}());

function deleteUniversityPaymentCancelation(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Journal,
        actionUrl: $("#hdnDeleteUniversityPaymentCancelation").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}

