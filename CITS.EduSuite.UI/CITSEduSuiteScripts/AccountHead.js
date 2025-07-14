var AccountHead = (function () {
    var getAccountHead = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAccountHead").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                SearchAccountGroupKey: function () {
                    return $('#SearchAccountGroupKey').val()
                },
                SearchAccountHeadTypeKey: function () {
                    return $('#SearchAccountHeadTypeKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.blankSpace, Resources.AccountGroup, Resources.AccountHeadType, Resources.AccountHeadName, Resources.AccountHeadCode,
            Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'IsSystemAccount', index: 'IsSystemAccount', editable: true },
                { key: false, name: 'AccountGroupName', index: 'AccountGroupName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AccountHeadTypeName', index: 'AccountHeadTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AccountHeadName', index: 'AccountHeadTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AccountHeadCode', index: 'AccountHeadCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 50],
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
            loadonce: false,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            sortname: 'RowKey',
            sortorder: 'desc',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this), AccountHeadPopupLoad)
                })
            }
        })

        $("#grid").jqGrid("setLabel", "AccountHeadName", "", "thAccountHeadName");
    }

    var editPopup = function (_this) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        var url = $(_this).attr("data-href");

        $('#myModalContent').load(url, function () {
            $.validator.unobtrusive.parse($('#frmAccountHead'));
            $("#myModal").one('show.bs.modal', function () {

                bindAccountHeadForm(this);

            }).modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');

        });

    }


    var getTabletr = function () {
        var heights = []
        $('tr').each(function () {
            heights.push($(this).height());
        }).get();

    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        if (rowdata.IsSystemAccount != true) {
            return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1"  data-href="' + $("#hdnAddEditAccountHead").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteAccountHead(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>' + ' </a></div>';
        }
        else {
            return '<div class="divEditDelete"></div>';
        }
    }


    //Details Popup

    var ledgerDetailPopup = function (_this) {
        var url = $(_this).data("url");
        var Name = $(_this).data("name");
        var Code = $(_this).data("code");
        if (parseFloat($(_this).html().trim())) {
            var obj = {};
            if (Code) {
                obj.id = Code;               
            }
            obj.fromDate = $("#FromDate").val();
            obj.toDate = $("#ToDate").val();
            obj.branchkey = $("#BranchKey").val();
            url = url + "?" + $.param(obj);
            $.customPopupform.CustomPopup({
                modalsize: "modal-lg",
                headerText: '<h4 class="modal-title" id="myModalLabel">' + Resources.View + Resources.BlankSpace + Name + '</h4>',
                footerText: '<div class="col-sm-12">'
                    + '<div class="form-group">'
                    + '<div class="pull-left">'
                    + '<button id="btnExportDetail" type="button" class="btn btn-success btn-sm" title="Excel"><i class="fa fa-file-excel-o"></i></button>'
                    + '<button id="btnPrintDetail" type="button" class="btn btn-outline-primary btn-sm" title="Print"><i class="fa fa-print"></i></button>'
                    + '</div>'
                    + '<button type="button" data-dismiss="modal" class="btn btn-sm btn-danger mx-1 pull-right">' + Resources.Cancel + '</button>'
                    + '</div></div>',
                load: function (modalDailog) {
                    var modalBody = $(modalDailog).find(".modal-body");
                    $(modalBody).addClass("paddingNone")
                    $("#ledgerTable", $(modalDailog)).addClass("form-master")
                    var obj = { HtmlCntrl: $(modalBody) };
                    obj.ContainerId = $('#ledgerTable', $(modalDailog)),
                        obj.Title = Name + ($("#BranchKey").find("option:selected").text() ? ' - ' + $("#BranchKey").find("option:selected").text() : "");
                    obj.SubTitle = $("#FromDate").val() === $("#ToDate").val() ? $("#FromDate").val() : $("#FromDate").val() + " - " + $("#ToDate").val();
                    obj.FileName = (obj.Title + "_" + obj.SubTitle).replace(/-/g, '_')

                    $("#btnExportDetail", $(modalDailog)).on("click", function () {
                        AppCommon.ExportTableToExcel(obj);
                    });
                    $("#btnPrintDetail", $(modalDailog)).on("click", function () {
                        AppCommon.PrintHtml(obj);
                    });
                }
            }, url);
        }
    }

    return {
        GetAccountHead: getAccountHead,
        EditPopup: editPopup,
        LedgerDetailPopup: ledgerDetailPopup,
    }
}());

function deleteAccountHead(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_AccountHead,
        actionUrl: $("#hdnDeleteAccountHead").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function bindAccountHeadForm(dialog) {
    $('#frmAccountHead', dialog).submit(function () {
        var validate = $('#frmAccountHead').valid();
        if (validate) {
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
                        bindAccountHeadForm(dialog)
                    }
                }
            });

        }

        return false;

    });
}

function AccountHeadPopupLoad() {
    AppCommon.FormatDateInput();
    AppCommon.FormatInputCase()

    var RowKey = $("#RowKey").val();
    RowKey = parseInt(RowKey) ? parseInt(RowKey) : 0;
    if (RowKey != 0) {
        var AccountGroupKey = $("#AccountGroupKey").val();

        var HideDaily = $("#HideDaily").prop("checked")
        HideDaily = JSON.parse(HideDaily) ? JSON.parse(HideDaily) : false

        var HideFuture = $("#HideFuture").prop("checked")
        HideFuture = JSON.parse(HideFuture) ? JSON.parse(HideFuture) : false

        if (HideFuture) {
            $("#AccountHeadDiv").hide();
        }
        else {
            $("#AccountHeadDiv").show();
        }

        if (HideDaily) {
            $("#dvPaymentReciept").hide();
        }
        else {
            var AccountGroupKey = $("#AccountGroupKey").val();
            if (AccountGroupKey == Resources.AccountGroupIncome || AccountGroupKey == Resources.AccountGroupExpenses) {
                $("#dvPaymentReciept").hide();
            }
            else {
                $("#dvPaymentReciept").show();
            }
        }
    }
    else {
        $("#dvPaymentReciept").hide();
    }

    $("#AccountGroupKey", $("#frmAccountHead")).on("change", function () {
        var obj = {};
        obj.key = $(this).val();
        AppCommon.BindDropDownbyId(obj, $("#hdnFillAccountHeadType").val(), $("#AccountHeadTypeKey"), Resources.AccountHeadType)

        if (obj.key == Resources.AccountGroupIncome || obj.key == Resources.AccountGroupExpenses) {
            $("#dvPaymentReciept").hide();
        } else {
            $("#dvPaymentReciept").show();
        }
    });

    $("#HideFuture").change(function () {
        $("#FutureAccountHeadKeys").find("option").attr("selected", false);

        if (this.checked) {
            $("#AccountHeadDiv").hide();
        }
        else {
            $("#AccountHeadDiv").show();
        }

    });


    $("#HideDaily").change(function () {

        if (this.checked) {
            $("#dvPaymentReciept").hide();
        }
        else {
            var AccountGroupKey = $("#AccountGroupKey").val();
            if (AccountGroupKey == Resources.AccountGroupIncome || AccountGroupKey == Resources.AccountGroupExpenses) {
                $("#dvPaymentReciept").hide();
            }
            else {
                $("#dvPaymentReciept").show();
            }
        }

    });
}




