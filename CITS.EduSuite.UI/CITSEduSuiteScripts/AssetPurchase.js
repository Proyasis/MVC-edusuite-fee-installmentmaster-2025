var container = "#dynamicRepeater";

var AssetPurchase = (function () {
    var getAssetPurchase = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnAssetPurchaseList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                branchkey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.OrderNumber, Resources.PartyName, Resources.TotalAmount, Resources.AmountPaid, Resources.BalanceAmount, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'OrderNumber', index: 'OrderNumber', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'PartyName', index: 'PartyName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'TotalAmount', index: 'TotalAmount', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal', width: 100 },
                { key: false, name: 'AmountPaid', index: 'AmountPaid', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal', width: 100 },
                { key: false, name: 'BalanceAmount', index: 'BalanceAmount', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal', width: 100 },
             { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [10, 20, 50, 100],
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
            altclass: 'jqgrid-altrow',
            sortname: 'RowKey',
            sortorder: 'desc',
            loadComplete: function (data) {
                $("#grid a[data-payment-modal='']").each(function () {
                    AppCommon.PaymentPopupWindow(this, $("#hdnAddEditSalesOrderPayment").val(), Resources.Add + Resources.BlankSpace + Resources.Purchase + Resources.BlankSpace + Resources.Payment);
                }),
                $("#grid a[data-modal='']").each(function () {
                    $(this).popupform
                        ({
                            modalsize: 'modal-lg',
                            load: function () {
                                setTimeout(function () {
                                    $(".input-group-addon-end").each(function () {
                                        AppCommon.SetInputAddOn(this);
                                    });
                                }, 500)
                            }
                        });
                })
            }

        })

        $("#grid").jqGrid("setLabel", "AssetPurchaseName", "", "thAssetPurchaseName");
    }

    var GetAssetPurchaseDetail = function (json) {
        $('.repeater').repeater(
         {
             show: function () {
                 $(this).slideDown();
                 $("[id*=UnitKey]", $(this)).val(Resources.UOMFeet);
                 var item = $(this);
                 AppCommon.FormatSelect2();
                 if ($("#StateKey").val() == $("#PartyStateKey").val()) {
                     $("input", $(".IGST")).val("")
                     $(".IGST").hide();
                     $(".nonIGST").show();
                 }
                 else {
                     $("input", $(".nonIGST")).val("")
                     $(".IGST").show();
                     $(".nonIGST").hide();
                 }
             },

             hide: function (remove) {
                 var self = $(this).closest('[data-repeater-item]').get(0);
                 var hidden = $(self).find('input[type=hidden]')[0];
                 if ($(hidden).val() != "" && $(hidden).val() != "0") {
                     deleteAssetPurchaseDetailsItem($(hidden).val(), $(this));
                 }
                 else {
                     $(this).slideUp(remove);

                 }

                 setTimeout(function () { AssetPurchase.GetTotalAmount(); }, 500);


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
                                     window.location.href = $("#hdnAssetPurchaseList").val()// + "/" + response["RowKey"];
                                 }
                             }
                         }
                     })

                 }

             },

             data: json,
             repeatlist: 'AssetPurchaseDetails',
             submitButton: '#btnSave',
         });

    }

    var getPartyById = function (Id, BranchId) {

        BranchId = BranchId != "" ? BranchId : "0";
        $ddlPartyKey = $("[id*=PartyKey]");
        $ddlPartyKey.html(""); // clear before appending new list 
        $ddlPartyKey.append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Party));
        if (Id != "") {
            $.ajax({
                url: $("#hdnGetPartyByPartyType").val(),
                type: "GET",
                dataType: "JSON",
                data: { id: Id, Branch: BranchId },
                success: function (result) {
                    $.each(result.Parties, function (i, Party) {
                        $ddlPartyKey.append(
                            $('<option></option>').val(Party.RowKey).html(Party.Text));
                    });
                    $("#StateKey").val(result.StateKey)
                }
            });
        }
    }

    var getPartDetailsById = function (Id) {
        if (Id != "") {

            $.ajax({
                url: $("#hdnGetPartDetailsByPartyKey").val(),
                type: "GET",
                dataType: "JSON",
                data: { id: Id },
                success: function (result) {
                    $.each(result.PartyDetails_Order, function (i, PartyDetails_Order) {
                        $("#txtMob").val(PartyDetails_Order.MobileNumber1);
                    });
                }
            });
        } else {
            $("#txtMob").val("");
        }
    }

    var editPopup = function (_this) {
        var validator = null
        var url = $(_this).attr("data-href") + "?partyKey=" + $("#PartyKey").val();

        $('#myModalContent').load(url, function () {
            $.validator.unobtrusive.parse($('#frmPartyPendingAccount'));
            $("#myModal").one('show.bs.modal', function () {

                bindOrderCashFlowForm(this);

            }).modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');

        });

    }

    var getRawMaterialDetailsById = function (_this) {
        var Id = $(_this).val()
        $closestItem = $(_this).closest("[data-repeater-item]")
        var BranchId = $("#BranchKey").val() != "" ? $("#BranchKey").val() : "0";

        if (Id != "") {
            $.ajax({
                url: $("#hdnGetRawMaterialDetailsById").val(),
                type: "GET",
                dataType: "JSON",
                data: { id: Id, Branch: BranchId },
                success: function (result) {
                    $closestItem.find("[id*=DepreciationMethodKey]").val(result.DepreciationMethodKey);
                    AssetPurchase.HideProduction($closestItem)
                    if ($("#StateKey").val() == $("#PartyStateKey").val()) {
                        $closestItem.find("[id*=CGSTPer]").val(result.CGSTPer);
                        $closestItem.find("[id*=SGSTPer]").val(result.SGSTPer);
                    }
                    else {
                        $closestItem.find("[id*=IGSTPer]").val(result.IGSTPer);
                    }
                    $closestItem.find("[id*=IsTax]").val(result.IsTax);
                    var RateTypeKey = $closestItem.find("[id*=RateTypeKey]");
                    $closestItem.find("select[id*=RateTypeKey] option[value=1]").remove();


                    $(".input-group-addon-end").each(function () {
                        AppCommon.SetInputAddOn(this);
                    });
                    AssetPurchase.GetRateTypes($(_this));
                }
            });
        }
    }

    var GetMobileNumberByPartyKey = function () {
        var PartyKeyData = $("#PartyKey").val();

        if (PartyKeyData != "") {
            $.ajax({
                url: $("#hdnGetMobileNumberByPartyKey").val(),
                type: "GET",
                dataType: "JSON",
                data: { id: PartyKeyData },
                success: function (result) {
                    $.each(result.PartyDetails_Order, function (i, PartyDetails_Order) {
                        $("#PartyMobileNumber").val(PartyDetails_Order.MobileNumber1);
                        $("#PartyTotalAmount").val(PartyDetails_Order.PartyTotalAmount);
                        $("#PartyTotalAmountPaid").val(PartyDetails_Order.PartyTotalAmountPaid);
                        $("#PartyTotalBalanceAmount").val(PartyDetails_Order.PartyTotalBalanceAmount);
                        $("#GSTINNumber").val(PartyDetails_Order.GSTINNumber);
                        $("#PartyStateKey").val(PartyDetails_Order.StateKey);
                        var oldAdvanceBalance = $("#OldAdvanceBalance").val() != "" ? parseFloat($("#OldAdvanceBalance").val()) : 0;
                        var balanceAmount = $("#BalanceAmount").val() != "" ? parseFloat($("#BalanceAmount").val()) : 0;
                        var RowKey = $("#RowKey").val() != "" ? parseInt($("#RowKey").val()) : 0;
                        if (RowKey != 0) {
                            $("#OldAdvanceBalance").val(oldAdvanceBalance);
                        }
                        else {
                            $("#OldAdvanceBalance").val(PartyDetails_Order.OldAdvanceBalance);
                        }
                        if (PartyDetails_Order.PartyTotalBalanceAmount != 0 && PartyDetails_Order.PartyTotalBalanceAmount != null && !isNaN(PartyDetails_Order.PartyTotalBalanceAmount)) {
                            $("#addEditCashFlow").show()
                        }
                        else {
                            $("#addEditCashFlow").hide()
                        }
                    });
                    if ($("#StateKey").val() == $("#PartyStateKey").val()) {
                        $("input", $(".IGST")).val("")
                        $(".IGST").hide();
                        $(".nonIGST").show();
                    }
                    else {
                        $("input", $(".nonIGST")).val("")
                        $(".IGST").show();
                        $(".nonIGST").hide();
                    }
                    AssetPurchase.GetTotalAmount()
                }
            });

            $("#DivMobileNumber").attr("style", "Display:block");
            $("#divPartyAmountDetails").attr("style", "Display:block");

        } else {
            $("#PartyMobileNumber").val("");
            $("#DivMobileNumber").attr("style", "Display:none");
            $("#divPartyAmountDetails").attr("style", "Display:none");
            return false;
        }
    }



    var getTotalAmount = function () {
        var taxableSum = 0, noTaxableSum = 0, sumCGSTAmt = 0, sumSGSTAmt = 0, sumIGSTAmt = 0;
        $.each($('input[dataamount]'), function (index) {
            var rowCGSTPer = $('input[name = "AssetPurchaseDetails[' + index + '].CGSTPer"]')[0];
            var rowCGSTAmt = $('input[name = "AssetPurchaseDetails[' + index + '].CGSTAmt"]')[0];
            var rowIGSTAmt = $('input[name = "AssetPurchaseDetails[' + index + '].IGSTAmt"]')[0];
            var rowIGSTPer = $('input[name = "AssetPurchaseDetails[' + index + '].IGSTPer"]')[0];
            var rowSGSTPer = $('input[name = "AssetPurchaseDetails[' + index + '].SGSTPer"]')[0];
            var rowSGSTAmt = $('input[name = "AssetPurchaseDetails[' + index + '].SGSTAmt"]')[0];
            var RowTotalGrand = $('input[name = "AssetPurchaseDetails[' + index + '].RowTotalGrand"]')[0];
            var rowTotalGST = $('input[name = "AssetPurchaseDetails[' + index + '].RowTotalGST"]')[0];
            var isTax = $('input[name = "AssetPurchaseDetails[' + index + '].IsTax"]')[0];

            var RateTypeKey = $('select[name="AssetPurchaseDetails[' + index + '].RateTypeKey"]').val();
            var Amount = $('input[name="AssetPurchaseDetails[' + index + '].Amount"]').val();
            var Qty = $('input[name="AssetPurchaseDetails[' + index + '].Quantity"]').val();
            var RateTypeUnitLengthVal = $('input[name="AssetPurchaseDetails[' + index + '].RateTypeUnitLength"]').val();

            Qty = parseFloat(Qty) ? parseFloat(Qty) : 1;

            RateTypeKey = parseInt(RateTypeKey) ? parseInt(RateTypeKey) : 0;

            var UnitAmount = this.value;
            $('input[name = "AssetPurchaseDetails[' + index + '].RowTotal"]').val("");

            if (Qty != "" && UnitAmount != "") {
                var UnitAmount = 0;
                if (RateTypeKey == Resources.SqFeetRateType) {

                    UnitAmount = (parseFloat(NewStockItem.val()) * parseFloat(Amount));

                } else if (RateTypeKey == Resources.SqWidthRateType || RateTypeKey == Resources.SqHeightRateType) {

                    UnitAmount = (parseFloat(RateTypeUnitLengthVal) * parseFloat(Amount));


                } else {

                    UnitAmount = Qty * parseFloat(Amount);

                }

                UnitAmount = AppCommon.RoundTo(UnitAmount, Resources.RoundToDecimalPostion).toString();

                var isTaxVal = JSON.parse(isTax.value.toLowerCase());
                if (isTaxVal) {
                    taxableSum = (taxableSum + parseFloat(UnitAmount));
                } else {
                    noTaxableSum = (noTaxableSum + parseFloat(UnitAmount));
                }
                $('input[name = "AssetPurchaseDetails[' + index + '].RowTotal"]')[0].value = UnitAmount;

                rowCGSTAmt.value = AppCommon.RoundTo(AssetPurchase.CalculateTax(UnitAmount, rowCGSTPer.value, "per"), 2);
                sumCGSTAmt = (sumCGSTAmt + parseFloat(rowCGSTAmt.value));

                rowSGSTAmt.value = AppCommon.RoundTo(AssetPurchase.CalculateTax(UnitAmount, rowSGSTPer.value, "per"), 2);
                sumSGSTAmt = (sumSGSTAmt + parseFloat(rowSGSTAmt.value));

                rowIGSTAmt.value = AppCommon.RoundTo(AssetPurchase.CalculateTax(UnitAmount, rowIGSTPer.value, "per"), 2);
                sumIGSTAmt = (sumIGSTAmt + parseFloat(rowIGSTAmt.value));
                RowTotalGrand.value = parseFloat(UnitAmount) + parseFloat(rowCGSTAmt.value) + parseFloat(rowSGSTAmt.value) + parseFloat(rowIGSTAmt.value);
                rowTotalGST.value = parseFloat(rowCGSTAmt.value) + parseFloat(rowSGSTAmt.value) + parseFloat(rowIGSTAmt.value);
            }
        });
        $("#CGSTAmt").val(parseFloat((sumCGSTAmt).toFixed(2)).toString());
        $("#SGSTAmt").val(parseFloat((sumSGSTAmt).toFixed(2)).toString());
        $("#IGSTAmt").val(parseFloat((sumIGSTAmt).toFixed(2)).toString());

        var discount = $("#Discount").val() != "" ? parseFloat($("#Discount").val()) : 0;
        var roundOff = $("#RoundOff").val() != "" ? parseFloat($("#RoundOff").val()) : 0;

        var CGSTAmt = $("#CGSTAmt").val() != "" ? parseFloat($("#CGSTAmt").val()) : 0;

        var SGSTAmt = $("#SGSTAmt").val() != "" ? parseFloat($("#SGSTAmt").val()) : 0;
        var IGSTAmt = $("#IGSTAmt").val() != "" ? parseFloat($("#IGSTAmt").val()) : 0;

        var AmountPaid = $("#AmountPaid").val() != "" ? parseFloat($("#AmountPaid").val()) : 0;


        $("#TaxableAmount").val(parseFloat((taxableSum).toFixed(2)).toString());

        var taxableAmount = $("#TaxableAmount").val() != "" ? parseFloat($("#TaxableAmount").val()) : 0;
        var nonTaxableAmount = $("#NonTaxableAmount");
        nonTaxableAmount.val(parseFloat((noTaxableSum).toFixed(2)).toString());
        nonTaxableAmountVal = $("#NonTaxableAmount").val();
        nonTaxableAmountVal = parseFloat(nonTaxableAmountVal) ? parseFloat(nonTaxableAmountVal) : 0;
        $("#SubTotal").val(parseFloat((nonTaxableAmountVal + taxableAmount + (CGSTAmt + SGSTAmt + IGSTAmt))).toFixed(2).toString());
        var subTotal = $("#SubTotal").val() != "" ? parseFloat($("#SubTotal").val()) : 0;
        $("#TotalAmount").val(AppCommon.RoundTo(subTotal - discount - roundOff, 2))
        var totalAmount = $("#TotalAmount").val() != "" ? parseFloat($("#TotalAmount").val()) : 0;
        var oldAdvance = $("#OldAdvanceBalance").val() != "" ? parseFloat($("#OldAdvanceBalance").val()) : 0;
        var newOldAdvance = $("#OldAdvanceBalance").val() != "" ? parseFloat($("#OldAdvanceBalance").val()) : 0;
        var BalanceAmount = parseFloat(((totalAmount - AmountPaid - newOldAdvance).toFixed(2))).toString();
        $("#BalanceAmount").val(isNaN(BalanceAmount) == true ? AppCommon.RoundTo(totalAmount, 2) : AppCommon.RoundTo(BalanceAmount, 2));


    }

    var calculateTax = function (TotalAmt, percentage, action) {
        TotalAmt = isNaN(TotalAmt) ? "" : TotalAmt;
        percentage = isNaN(percentage) ? "" : percentage;

        var TotalAmt = TotalAmt != "" ? parseFloat(TotalAmt) : 0; var tax = 0;
        if (action == "per") {
            percentage = percentage != "" ? parseFloat(percentage) : 0;
            tax = (TotalAmt * percentage) / 100;
        } else if (action == "amt") {
            TotalAmt = TotalAmt != 0 ? parseFloat(TotalAmt) : 1;
            percentage = percentage != "" ? parseFloat(percentage) : 0;
            tax = percentage * 100 / TotalAmt;
        }
        return tax;
    }

    var hideProduction = function (item) {
        var methodKey = $("[id*=DepreciationMethodKey]", item).val()
        if (methodKey == 3) {
            $("[id*=ProductionLimit]", item).closest('div').show()
        }
        else {
            $("[id*=ProductionLimit]", item).closest('div').hide()
        }
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-primary btn-xs"  href="AddEditAssetPurchase/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>&nbsp;' + Resources.Edit + '</a>' + '<a class="btn btn-warning btn-xs" data-payment-modal="" data-href="' + $("#hdnAddEditSalesOrderPayment").val() + '/' + rowdata.RowKey + '"  ><i class="fa fa-credit-card" aria-hidden="true"></i>' + Resources.Payment + '</a><a data-modal="" class="btn btn-primary btn-xs" data-href="' + $("#hdnViewPurchase").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>&nbsp;' + Resources.View + '</a>';
    }



    var getRateTypes = function (item) {

        var _this = $(item).closest('div[data-repeater-item]');
        $ddlRateTypeKey = $("[id*=RateTypeKey]", $(_this));

        var Id = $("[id*=RawMaterialKey]", $(_this)).val();
        Id = parseInt(Id) ? parseInt(Id) : 0;

        if (Id != "") {
            $.ajax({
                url: $("#hdnFillRateTypes").val(),
                type: "GET",
                dataType: "JSON",
                data: {
                    id: Id,
                },
                success: function (result) {
                    $ddlRateTypeKey.html(""); // clear before appending new list 
                    $.each(result.RateTypes, function (i, RateType) {
                        $ddlRateTypeKey.append(
                            $('<option></option>').val(RateType.RowKey).html(RateType.Text));
                    });
                }
            });
        }
    }

    return {
        GetAssetPurchase: getAssetPurchase,
        GetAssetPurchaseDetail: GetAssetPurchaseDetail,
        GetTotalAmount: getTotalAmount,
        GetMobileNumberByPartyKey: GetMobileNumberByPartyKey,
        GetPartyById: getPartyById,
        GetRawMaterialDetailsById: getRawMaterialDetailsById,
        GetPartDetailsById: getPartDetailsById,
        CalculateTax: calculateTax,
        EditPopup: editPopup,
        GetRateTypes: getRateTypes,
        HideProduction: hideProduction
    }
}());

function deleteAssetPurchase(rowkey) {
    var result = confirm(Resources.Delete_Confirm_AssetPurchase);
    if (result == true) {
        var response = AjaxHelper.ajax("POST", $("#hdnDeleteAssetPurchase").val(),
                    {
                        id: rowkey
                    });

        if (response.Message === Resources.Success) {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
        else
            alert(response.Message);
        event.preventDefault();
    }
}

function deleteAssetPurchaseDetailsItem(rowkey, _this) {
    var result = cEduSuite.Confirm
        ({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_OrderProcess,
            actionUrl: $("#hdnDeleteAssetPurchaseItem").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                window.location.reload(false);
            }
        });
}

function bindOrderCashFlowForm(dialog) {
    $('#frmPartyPendingAccount', dialog).submit(function () {
        var validate = $('#frmPartyPendingAccount').valid();
        var form = this;
        if (validate) {
            $(form).mLoading();
            var checkedCount = $("[pendingAccountRepeater] input.chkClearAccount:checked").length;
            var chkCount = $("[pendingAccountRepeater] input.chkClearAccount").length;
            //if (checkedCount == 0 && chkCount != 0) {
            //    $("[data-valmsg-for=paymenterror_msg]").html(Resources.ErrorSelectPayment)
            //    return false;
            //}
            $("#CashFlowMaster").find('select, input, textarea').removeAttr('disabled');
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        AssetPurchase.GetMobileNumberByPartyKey()
                        $('#myModal').modal('hide');
                    } else {
                        $('#myModalContent').html(result);
                        bindCashFlowForm(dialog)
                    }
                    $(form).mLoading("destroy");
                }
            });

        }

        return false;

    });
}

function setPuchaseOrderPopupUrl(item) {
    var obj = {};
    obj.MaterialTypeKey = $("select[id*=MaterialTypeKey]", $(item)).val();

    var PassUrl = $("#addEditPass", $(item)).data("href");
    if (PassUrl) {
        PassUrl = PassUrl.split('?')[0]
        //var ddlWidth = $("select[id*=Width]", $(item))[0]
        //obj.Width = ddlWidth[ddlWidth.selectedIndex].attributes.length > 1 ? ddlWidth[ddlWidth.selectedIndex].attributes[1].nodeValue : null;
        //obj.WidthUOM = ddlWidth[ddlWidth.selectedIndex].attributes.length > 0 ? ddlWidth[ddlWidth.selectedIndex].attributes[0].nodeValue : null;
        PassUrl = PassUrl + "?" + $.param(obj);
        $("#addEditPass", $(item)).attr("data-href", PassUrl);
        if ($("select[id*=MaterialTypeKey]", $(item)).val() != "") {
            $("#addEditPass", $(item)).show();
        }
        else {
            $("#addEditPass", $(item)).hide();
        }
    }
}

function purchaseWidthAndPassPopup(_this) {
    $("#addEditPass", $(_this)).popupform({

        load: function () {
            AppCommon.FormatInputCase();
        },
        rebind: function () {
            $.ajax({
                url: $("#hdnRawMaterialByMaterialTypeId").val(),
                type: "GET",
                dataType: "JSON",
                data: { id: 0 },
                async: false,
                success: function (result) {
                    $('[data-repeater-item]').each(function () {
                        var RowKey = $('input[name*=RowKey]', $(this));
                        var ddl = $('select[id*=AssetTypeKey]', $(this));
                        if (RowKey != "") {
                            var oldValue = $(ddl).val();
                            $(ddl).empty()

                            ddl.append($('<option></option>').val("").html("Select Asset Type"));
                            $.each(result.AssetTypes, function (index) {
                                ddl.append($('<option></option>').val(this.RowKey).html(this.Text));
                            });
                            $(ddl).val(oldValue);
                        }
                    });

                }
            });
        }
    });

}

$(document).ready(function () {
    $.each($('div[data-repeater-item]'), function (index) {
        var rowkey = $('input[name="AssetPurchaseDetails[' + index + '].RowKey"]')[0];
        var AssetType = $('select[name="AssetPurchaseDetails[' + index + '].AssetTypeKey"]')[0];
        var Quantity = $('input[name="AssetPurchaseDetails[' + index + '].Quantity"]')[0];

        if (parseInt(rowkey.value) > 0) {
            $(AssetType).attr("Disabled", "Disabled");
            $(Quantity).attr("Disabled", "Disabled");
        }
    });
});

