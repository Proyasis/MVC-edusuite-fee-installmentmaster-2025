var Asset = (function () {

    var getAssetDetail = function (json) {

        $('.repeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    AppCommon.FormatDateInput();
                    AppCommon.CustomRepeaterRemoteMethod();
                    var d = new Date();
                    d = d.getDate() + '/' + (d.getMonth() + 1) + '/' + d.getFullYear();
                    $controls = $(this).find("[id*=PeriodTypeKey]")
                    $(this).find("[id*=PeriodTypeKey]").val(3)
                    $(this).find("[id*=PurchasingDate]").val(d)
                    $(this).find("[id*=IsActive]")[0].checked = true
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteAssetDetails($(hidden).val(), $(this));
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
                                        window.location.href = $("#hdnAssetList").val();
                                    }
                                }
                            }
                        })

                    }

                },
                data: json,
                repeatlist: 'AssetDetailList',
                submitButton: '#btnSave',
            });
    }


    var getAsset = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAsset").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Asset + Resources.BlankSpace + Resources.Head, Resources.Asset + Resources.BlankSpace + Resources.Head + Resources.BlankSpace + Resources.Code, Resources.Asset + Resources.BlankSpace + Resources.Count, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'MasterKey', index: 'MasterKey', editable: true },
                { key: false, name: 'AccountHeadName', index: 'AccountHeadName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AccountHeadCode', index: 'AccountHeadName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AssetCount', index: 'AssetCount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'MasterKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [Resources.PagingRowNum,10, 20, 50, 100].unique(),
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
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    $(this).popupform
                        ({
                            modalsize: 'modal-lg'
                        });
                })

            }
        })

        $("#grid").jqGrid("setLabel", "AssetName", "", "thAssetName");
    }

    var getAccountHead = function (ddl) {

        $ddl = ddl;
        $ddl.html(""); // clear before appending new list 
        $ddl.append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.AssetType));
        $.ajax({
            url: $("#hdnGetAccountHead").val() + "?key=" + $("#AccountHeadTypeKey").val(),
            type: "GET",
            dataType: "JSON",
            data: {},
            success: function (result) {
                $.each(result.AssetTypes, function (i, Head) {
                    $ddl.append(
                        $('<option></option>').val(Head.MasterKey).html(Head.Text));
                });
            }
        });
    }

    var ckickLeft = function () {
        var showItem = 0;
        var hideItem = 0;
        $("[data-repeater-item]:visible").each(function () {
            showItem = showItem + 1;
            if (showItem == 1) {
                $(this).show()
                var attr = $(this).prev().attr('data-repeater-item');
                if (typeof attr !== typeof undefined && attr !== false) {
                    $(this).prev().show()
                    $(this).next().next().hide()
                }
            }
        });
    }

    var ckickRight = function () {
        var showItem = 0;
        var hideItem = 0;
        $("[data-repeater-item]:visible").each(function () {
            showItem = showItem + 1;
            if (showItem == 3) {
                $(this).show()
                var attr = $(this).next().attr('data-repeater-item');
                if (typeof attr !== typeof undefined && attr !== false) {
                    $(this).next().show()
                    $(this).prev().prev().hide()
                }
            }
        });
    }



    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.MasterKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-primary btn-xs" href="' + $("#hdnAddEditAsset").val() + '/' + rowdata.MasterKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-xs" href="#"   onclick="javascript:deleteAsset(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i>' + Resources.Delete + '</a>'+'<a data-modal="" class="btn btn-primary btn-xs" data-href="' + $("#hdnViewAsset").val() + '/' + rowdata.MasterKey + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';
    }
    return {
        GetAsset: getAsset,
        GetAssetDetail: getAssetDetail,
        GetAccountHead: getAccountHead,
        CkickLeft: ckickLeft,
        CkickRight: ckickRight
    }
}());

function deleteAsset(rowkey) {
    var result = cEduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Asset,
        actionUrl: $("#hdnDeleteAsset").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteAssetDetails(rowkey) {
    var result = cEduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Asset,
        actionUrl: $("#hdnDeleteAssetDetails").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            window.location.reload();
        }
    });
}





