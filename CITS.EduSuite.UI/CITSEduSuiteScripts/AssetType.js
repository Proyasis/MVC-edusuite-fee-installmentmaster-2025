var AssetType = (function () {
    var getAssetType = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAssetType").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.AssetType, Resources.IsTaxable, Resources.CGSTPer, Resources.SGSTPer, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'AssetTypeName', index: 'AssetTypeName', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-center' },
                { key: false, name: 'IsTax', index: 'IsTax', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-center' },
                { key: false, name: 'CGSTPer', index: 'CGSTPer', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal' },
                { key: false, name: 'SGSTPer', index: 'SGSTPer', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-right-decimal' },
                { key: false, name: 'IsActive', index: 'IsActive', editable: true, cellEdit: true, sortable: true, resizable: false, classes: 'text-center' },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow'
        })

        $("#grid").jqGrid("setLabel", "AssetTypeName", "", "thAssetTypeName");
    }
    var editPopup = function (_this) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        var url = $(_this).attr("data-href");

        $('#myModalContent').load(url, function () {
            $.validator.unobtrusive.parse($('#frmAssetType'));
            $("#myModal").one('show.bs.modal', function () {
                AppCommon.FormatInputCase();
                bindAssetTypeForm(this);

            }).modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');

        });

    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-primary btn-sm mx-1" onclick="AssetType.EditPopup(this)" data-href="' + $("#hdnAddEditAssetType").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-danger btn-sm mx-1" onclick="javascript:deleteAssetType(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> </a></div>';


    }

    var HSNCodeDetails = function (_this) {
        var obj = {};
        obj.id = $(_this).val();
        $.ajax({
            url: $("#hdnGetHSNCodeDetails").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            success: function (result) {
                $("input[type=text][id*=HSNCode]").val(result.HSNSACCode);
                $("input[type=text][id*=IGSTAssetTypePer]").val(result.HSNIGSTPer);
                $("input[type=text][id*=CGSTAssetTypePer]").val(result.HSNCGSTPer);
                $("input[type=text][id*=SGSTAssetTypePer]").val(result.HSNSGSTPer);
            }
        });
    }

    return {
        GetAssetType: getAssetType,
        EditPopup: editPopup,
        HSNCodeDetails: HSNCodeDetails
    }
}());

function deleteAssetType(rowkey) {
    var result = confirm(Resources.Delete_Confirm_AssetType);
    if (result == true) {
        var response = AjaxHelper.ajax("POST", $("#hdnDeleteAssetType").val(),
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

function bindAssetTypeForm(dialog) {
    $('#frmAssetType', dialog).submit(function () {
        var validate = $('#frmAssetType').valid();
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
                        window.location.reload();
                    } else {
                        $('#myModalContent').html(result);
                        bindAssetTypeForm(dialog)
                    }
                    $(form).mLoading("destroy");
                }
            });

        }

        return false;

    });
}
