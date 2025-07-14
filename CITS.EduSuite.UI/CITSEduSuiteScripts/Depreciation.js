var Depreciation = (function () {

    var getDepreciation = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetDepreciationList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Asset, Resources.Depreciation, Resources.BookValue, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'AssetDetailsKey', index: 'AssetDetailsKey', editable: true },
                { key: false, name: 'AssetName', index: 'AssetName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Depreciation', index: 'Depreciation', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BookValue', index: 'BookValue', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'AssetDetailsKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
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
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow'
        })

        $("#grid").jqGrid("setLabel", "DepreciationName", "", "thDepreciationName");
    }

    var editPopup = function (_this) {

        var url = $(_this).attr("data-href");
        $.customPopupform.CustomPopup({
            modalsize: "modal-md",
            load: function (modalDailog) {
                form = $("form", $(modalDailog))
                setTimeout(function () {
                    AppCommon.FormatInputCase();
                }, 500)

            },
            rebind: function () {
                $(".modal").modal('hide');
                Depreciation.ViewPopup(null, $("#ViewURL").val())
            }
        }, url);

    }

    var viewPopup = function (_this, urlp) {
        var url = "";
        if (urlp) {
            url = urlp
        }
        else {
            url = $(_this).attr("data-href");
        }
        $("#ViewURL").val(url)
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg90",
            load: function (modalDailog) {
                form = $("form", $(modalDailog))
                setTimeout(function () {
                    AppCommon.FormatInputCase();
                }, 500)

            },
            rebind: function () {
            }
        }, url);
    }

    var validUnit = function () {
        var oldProduction = $("#OldProduction").val()
        oldProduction = parseFloat(oldProduction) ? parseFloat(oldProduction) : 0
        var ProductionLimit = $("#ProductionLimit").val()
        ProductionLimit = parseFloat(ProductionLimit) ? parseFloat(ProductionLimit) : 0
        var ProductionUnit = $("#ProductionUnit").val()
        ProductionUnit = parseFloat(ProductionUnit) ? parseFloat(ProductionUnit) : 0
        if ((ProductionUnit + oldProduction) > ProductionLimit) {
            $("#ProductionUnit").val(ProductionLimit - oldProduction)
            $("#ProductionBalance").val(0)
        }
        else {
            $("#ProductionBalance").val(ProductionLimit - (ProductionUnit + oldProduction))
        }
        Depreciation.ProductionDepreciationCalc()
    }

    var productionDepreciationCalc = function () {
        var limit = $("#ProductionLimit").val()
        limit = parseFloat(limit) ? parseFloat(limit) : 0
        var Unit = $("#ProductionUnit").val()
        Unit = parseFloat(Unit) ? parseFloat(Unit) : 0
        var Amount = $("#PurchaseAmount").val()
        Amount = parseFloat(Amount) ? parseFloat(Amount) : 0
        var depreciation = (Unit / limit) * Amount
        $("#Depreciation").val(depreciation)
    }

    return {
        GetDepreciation: getDepreciation,
        EditPopup: editPopup,
        ViewPopup: viewPopup,
        ProductionDepreciationCalc: productionDepreciationCalc,
        ValidUnit: validUnit
    }
}());


function editLink(cellValue, options, rowdata, action) {
    var temp = "'" + rowdata.RowKey + "'";
    return '<div class="divEditDelete"><a data-modal="" class="btn btn-primary btn-xs" onclick="Depreciation.ViewPopup(this)" data-href="' + $("#hdnViewDepreciation").val() + '/' + rowdata.AssetDetailsKey + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';
}

function deleteDepreciation(rowkey) {
    var result = confirm(Resources.Delete_Confirm_Depreciation);
    if (result == true) {
        var response = AjaxHelper.ajax("POST", $("#hdnDeleteDepreciation").val(),
            {
                id: rowkey
            });

        if (response.Message === Resources.Success) {
            $(".modal").modal('hide');
            Depreciation.ViewPopup(null, $("#ViewURL").val())
        }
        else
            alert(response.Message);
        event.preventDefault();
    }
}


function bindDepreciationForm(dialog) {
    $('#frmDepreciation', dialog).submit(function () {
        var validate = $('#frmDepreciation').valid();
        var form = this;
        if (validate) {
            $(form).mLoading();
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        $('#myModal').modal('hide');
                        window.Depreciation.reload();
                    } else {
                        $('#myModalContent').html(result);
                        bindDepreciationForm(dialog)
                    }
                    $(form).mLoading("destroy");
                }
            });

        }

        return false;

    });
}

