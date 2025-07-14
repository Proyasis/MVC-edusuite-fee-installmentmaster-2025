var ajaxRequest = null;

var FeeRefund = (function () {

    var getFeeRefundDetails = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetStudentFeeRefund").val(),
            datatype: 'json',
            mtype: 'GET',
            postData: {
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                BatchKey: function () {
                    return $('#BatchKey').val()
                },
                CourseKey: function () {
                    return $('#CourseKey').val()
                },
                UniversityKey: function () {
                    return $('#UniversityKey').val()
                },
                SearchText: function () {
                    return $('#txtsearch').val()
                },
                FromDate: function () {
                    return $('#txtSearchFromDate').val()
                },
                ToDate: function () {
                    return $('#txtSearchToDate').val()
                },
            },
            colNames: [Resources.RowKey, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace,
                Resources.Blankspace, Resources.Branch, Resources.AdmissionNo, Resources.Name, Resources.MobileNo, Resources.Course, Resources.CurrentYear,
                Resources.Batch, Resources.Amount, Resources.Date, Resources.Remarks, Resources.Status, Resources.Action],
            colModel: [

                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: false, hidden: true, name: 'CurrentYear', index: 'CurrentYear', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, hidden: true, name: 'CourseName', index: 'CourseName', editable: true },
                { key: false, hidden: true, name: 'UniversityName', index: 'UniversityName', editable: true },
                { key: false, hidden: true, name: 'ProcessStatus', index: 'ProcessStatus', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicantName', index: 'ApplicantName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentMobile', index: 'StudentMobile', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, formatter: formatCourseUniversityYear, sortable: true, resizable: false, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'RefundAmount', index: 'RefundAmount', editable: true, cellEdit: true, sortable: true, resizable: false, align: 'center' },
                { key: false, name: 'RefundDate', index: 'RefundDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'Remarks', index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ProcessStatusName', index: 'ProcessStatusName', editable: true, cellEdit: true, sortable: true, resizable: false },
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
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',



        })

        $("#grid").jqGrid("setLabel", "InternalExamSchedule", "", "thStudentsPromotion");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        var obj = {}
        obj.id = rowdata.RowKey;
        obj.ApplicationKey = rowdata.ApplicationKey;

        if (rowdata.ProcessStatus == Resources.ProcessStatusPending) {
            return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnStudentFeeRefundList").val() + '?' + $.param(obj) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm  mx-1" href="#"   onclick="javascript:deleteFeeRefund(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-primary btn-sm  mx-1" href="#"   onclick="javascript:processFeeRefund(' + temp + ',' + Resources.ProcessStatusApproved + ');return false;"><i class="fa fa-thumbs-up" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-primary btn-sm  mx-1" href="#"   onclick="javascript:processFeeRefund(' + temp + ',' + Resources.ProcessStatusRejected + ');return false;"><i class="fa fa-thumbs-down" aria-hidden="true"></i></a></div>';
        }
        else {
            if (rowdata.ProcessStatus == Resources.ProcessStatusRejected) {
                return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnStudentFeeRefundList").val() + '?' + $.param(obj) + '"><i class="fa fa-eye" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm  mx-1" href="#"   onclick="javascript:deleteFeeRefund(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';

            }
            else {
                return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnStudentFeeRefundList").val() + '?' + $.param(obj) + '"><i class="fa fa-eye" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm  mx-1" href="#"   onclick="javascript:deleteFeeRefund(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-info btn-sm  mx-1" href="#"   onclick="FeeRefund.FeeReceiptPopup(' + temp + ');return false;"><i class="fa fa-print" aria-hidden="true"></i></a></div>';

            }
        
        }
    }
    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }
    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }

    var getApplicationDetails = function () {
        var model = {};
        model.AdmissionNo = $("#AdmissionNo").val();
        var ApplicationKey = $("#ApplicationKey").val();
        var RowKey = $("#RowKey").val();
        model.ApplicationKey = parseInt(ApplicationKey) ? parseInt(ApplicationKey) : 0;
        model.RowKey = parseInt(RowKey) ? parseInt(RowKey) : 0;

        if (model.AdmissionNo != "" && model.AdmissionNo != null || (model.ApplicationKey != 0)) {

            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnStudentApplication").val(),
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

    var addEditFeeRefund = function (id, ApplicationKey) {


        var obj = {};
        id = parseInt(id) ? parseInt(id) : 0;
        //obj.id = id;
        obj.ApplicationKey = ApplicationKey;
        var url = $("#hdnAddEditFeeRefund").val() + "/" + id + "?" + $.param(obj);
        $("#dvAddEditFeeRefund").mLoading();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response) {
                    $("#dvAddEditFeeRefund").html(response)
                    $.validator.unobtrusive.parse("form");
                    FeeRefund.ReturnAdvanceList();

                }
                else {
                    $("#dvAddEditFeeRefund").mLoading("destroy");
                }
            }
        })

    }

    var returnAdvanceList = function () {
        var obj = {};
        obj.id = $("#RowKey", $("#frmFeeRefund")).val();
        obj.ApplicationKey = $("#ApplicationKey", $("#frmFeeRefund")).val();
        $("#dvadvanceList").mLoading()
        $("#dvadvanceList").load($("#hdnReturnAdvanceList").val() + "?" + $.param(obj), function () {
            FeeRefund.AutoDeduct()
            $("[id*=ReturnAmount]").on("input", function () {
                var item = $(this).closest("[advanceRepeater]")
                var paid = $(this).val()
                var beforechangePaid = $("[id*=BeforeTakenAdvance]", item).val()
                var advanceBalance = $("[advanceBalance]", item)
                paid = parseFloat(paid) ? parseFloat(paid) : 0
                beforechangePaid = parseFloat(beforechangePaid) ? parseFloat(beforechangePaid) : 0
                var totalAdvance = 0
                var RefundAmount = $("#RefundAmount").val()
                RefundAmount = parseFloat(RefundAmount) ? parseFloat(RefundAmount) : 0
                $("[id*=ReturnAmount]").each(function () {
                    var returnAmt = parseFloat($(this).val()) ? parseFloat($(this).val()) : 0
                    totalAdvance = totalAdvance + returnAmt
                })
                var balAdvance = RefundAmount - totalAdvance + paid
                $(advanceBalance).html(beforechangePaid - paid)
                if (balAdvance < paid) {
                    $(this).val(balAdvance)
                    $(advanceBalance).html(beforechangePaid - balAdvance)
                }
                else {
                    if (beforechangePaid < paid) {
                        $(this).val(beforechangePaid)
                        $(advanceBalance).html(0)
                    }
                }
                FeeRefund.DeductAdvance();
                FeeRefund.ValidatePaidAmount()
            })
            $("[id*=IsDeduct]").on("change", function () {
                var item = $(this).closest("[advanceRepeater]")
                var paid = $("[id*=ReturnAmount]", item)
                var beforechangePaid = $("[id*=BeforeTakenAdvance]", item).val()
                var advanceBalance = $("[advanceBalance]", item)
                beforechangePaid = parseFloat(beforechangePaid) ? parseFloat(beforechangePaid) : 0
                var totalAdvance = $("[totaladvance]").html()
                totalAdvance = parseFloat(totalAdvance) ? parseFloat(totalAdvance) : 0
                var RefundAmount = $("#RefundAmount").val()
                RefundAmount = parseFloat(RefundAmount) ? parseFloat(RefundAmount) : 0
                var balAdvance = RefundAmount - totalAdvance
                if ($(this)[0].checked) {
                    if (totalAdvance >= RefundAmount) {
                        $(this).prop("checked", false)
                        $(advanceBalance).html(beforechangePaid)
                        $(paid).val(0)
                        $(paid).attr('readonly', true)
                    }
                    else {
                        if (balAdvance < beforechangePaid) {
                            $(advanceBalance).html(beforechangePaid - balAdvance)
                            $(paid).val(balAdvance)
                        }
                        else {
                            $(advanceBalance).html(0)
                            $(paid).val(beforechangePaid)
                        }
                        $(paid).removeAttr('readonly')
                    }
                }
                else {
                    $(advanceBalance).html(beforechangePaid)
                    $(paid).val(0)
                    $(paid).attr('readonly', true)
                }
                FeeRefund.DeductAdvance();
                FeeRefund.ValidatePaidAmount()
            })

            $("#dvadvanceList").mLoading("destroy")
        })
    }

    var deductAdvance = function () {
        var advanceAmount = 0;
        var totalAdvance = 0;
        var totalBalance = 0;
        $("[advanceRepeater]").each(function () {
            var item = $(this).find("[id*=IsDeduct]")
            var advanceBalanceControl = $("[advanceBalance]", $(this))
            var paidControl = $("[id*=ReturnAmount]", $(this))
            var beforechangePaid = $("[id*=BeforeTakenAdvance]", $(this)).val()
            beforechangePaid = parseFloat(beforechangePaid) ? parseFloat(beforechangePaid) : 0
            if (item[0].checked) {
                var amount = $(this).find("[id*=ReturnAmount]").val()
                amount = amount != "" ? parseFloat(amount) : 0;
                advanceAmount = advanceAmount + amount;
                $(paidControl).removeAttr('readonly')
            }
            else {
                $(advanceBalanceControl).html(beforechangePaid)
                $(paidControl).val(0)
                $(paidControl).attr('readonly', true)
            }
            var paid = $(paidControl).val()
            var advanceBalance = $(advanceBalanceControl).html()
            advanceBalance = parseFloat(advanceBalance) ? parseFloat(advanceBalance) : 0
            paid = parseFloat(paid) ? parseFloat(paid) : 0
            totalAdvance = totalAdvance + paid;
            totalBalance = totalBalance + advanceBalance;
        })
        $("input#SalaryAdvance").val(advanceAmount);
        $("[totaladvance]").html(totalAdvance);
        $("[totalbalance]").html(totalBalance);
        $("input#TotalRefundedAmount").val(totalAdvance);
    }

    var validatePaidAmount = function () {
        var totalAdvance = $("[totaladvance]").html()
        totalAdvance = parseFloat(totalAdvance) ? parseFloat(totalAdvance) : 0
        var totalbalance = $("[totalbalance]").html()
        totalbalance = parseFloat(totalbalance) ? parseFloat(totalbalance) : 0
        var paid = $("#RefundAmount").val()
        paid = parseFloat(paid) ? parseFloat(paid) : 0
        var advance = totalAdvance + totalbalance
        if (paid > advance) {
            $("#RefundAmount").val(advance)
        }
    }

    var autoDeduct = function () {
        var paid = $("#RefundAmount").val()
        paid = parseFloat(paid) ? parseFloat(paid) : 0
        $("[advanceRepeater]").each(function () {
            var $returnAmount = $("[id*=ReturnAmount]", this)
            var $isDeduct = $("[id*=IsDeduct]", this)
            var returnAmountVal = $("[id*=ReturnAmount]", this).val()
            returnAmountVal = parseFloat(returnAmountVal) ? parseFloat(returnAmountVal) : 0
            var advancebalance = $("[advancebalance]", this).html()
            advancebalance = parseFloat(advancebalance) ? parseFloat(advancebalance) : 0
            var totalAdvance = returnAmountVal + advancebalance
            if (paid == 0) {
                $isDeduct.attr("checked", false)
                $returnAmount.val(0)
            }
            else if (paid >= totalAdvance) {
                $isDeduct.prop("checked", true)
                $returnAmount.val(totalAdvance)
                paid = paid - totalAdvance
            }
            else {
                $isDeduct.prop("checked", true)
                $returnAmount.val(paid)
                paid = 0
            }
            var beforechangePaid = $("[id*=BeforeTakenAdvance]", this).val()
            var $advanceBalance = $("[advanceBalance]", this)
            returnAmountVal = parseFloat($returnAmount.val()) ? parseFloat($returnAmount.val()) : 0
            beforechangePaid = parseFloat(beforechangePaid) ? parseFloat(beforechangePaid) : 0
            $($advanceBalance).html(beforechangePaid - returnAmountVal)
            if (beforechangePaid < returnAmountVal) {
                $(this).val(beforechangePaid)
                $($advanceBalance).html(0)
            }
        })
        FeeRefund.DeductAdvance();
    }

    var checkPaymentMode = function (PayMode) {
        $("#dvPaymentModes").show();
        $("#dvPaymentModeSub").hide();
        switch (parseInt(PayMode || 0)) {
            case 2:
                $(".divChequeDetails,.divBankAccount").show();
                $(".divCardDetails,.divReferenceNo").hide();

                break;
            case 3:
                $("#dvPaymentModeSub").show();
                $(".divBankAccount").show();
                $(".divCardDetails,.divChequeDetails,.divReferenceNo").hide();
                break;

            default:
                $("#dvPaymentModes,.divChequeDetails,.divCardDetails,.divBankAccount,.divReferenceNo").hide();
                break;
        }
    }

    var checkPaymentModeSub = function (PayMode) {
        $("#dvPaymentModes").show();

        switch (parseInt(PayMode || 0)) {
            case 1:
                $(".divChequeDetails,.divBankAccount").show();
                $(".divCardDetails,.divReferenceNo").hide();

                break;
            case 2:
                $(".divCardDetails,.divBankAccount").show();
                $(".divChequeDetails,.divReferenceNo").hide();
                break;

            default:

                $("#dvPaymentModes,.divBankAccount,.divReferenceNo").show();

                $(".divChequeDetails,.divCardDetails").hide();
                break;
        }

    }
    var getBalance = function (paymentMode, PurchaseOrderPaymentRowKey, BankAccountKey, branchKey) {
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?PaymentModeKey=" + paymentMode + "&RowKey=" + PurchaseOrderPaymentRowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey)

        $("#BankAccountBalance").val(response);
    }

    function formSubmit() {
        $('#btnPayment').hide();
        var $form = $("#frmFeeRefund")
        var JsonData = [];
        $("[disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {

            var id = formData['ApplicationKey'];

            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
            if (typeof response == "string") {
                $("[data-valmsg-for=error_msg]").html(response);
                $('#btnPayment').show();
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
                                location.href = $("#hdnFeeRefundList").val();
                            }
                        }
                    }
                })

            }

        }
        else {
            $('#btnPayment').show();
        }
        $('#btnPayment').show();
    }

    var feeReceiptPopup = function (Id) {
        window.open($("#hdnViewFeePrint").val() + "/" + Id, '_blank');
    }


    return {
        GetApplicationDetails: getApplicationDetails,
        AddEditFeeRefund: addEditFeeRefund,
        DeductAdvance: deductAdvance,
        ValidatePaidAmount: validatePaidAmount,
        AutoDeduct: autoDeduct,
        ReturnAdvanceList: returnAdvanceList,
        CheckPaymentMode: checkPaymentMode,
        GetBalance: getBalance,
        CheckPaymentModeSub: checkPaymentModeSub,
        FormSubmit: formSubmit,
        GetFeeRefundDetails: getFeeRefundDetails,
        FeeReceiptPopup: feeReceiptPopup
    }

}());


function deleteFeeRefund(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Branch,
        actionUrl: $("#hdnDeleteFeeRefund").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function processFeeRefund(rowkey, ProcessStatus) {
    var obj = {};
    obj.Id = rowkey;
    obj.ProcessStatus = ProcessStatus;
    var ContentMsg = "";
    if (ProcessStatus == Resources.ProcessStatusApproved) {
        ContentMsg = "Are you Sure to Approve this Refund ?";
    }
    else {
        ContentMsg = "Are you Sure to Reject this Refund ?";
    }
    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: ContentMsg,
        actionUrl: $("#hdnApproveFeeRefund").val(),
        parameters: obj,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }

    });
}