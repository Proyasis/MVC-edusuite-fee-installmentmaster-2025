
var AccountTransaction = (function () {

    var getAccountTransactions = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAccountTransactions").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                branchId: function () {
                    return $('#ddlBranch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Branch, Resources.Date, Resources.Ledger, Resources.Amount, Resources.PartyType, Resources.Party, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: true, hidden: true, name: 'TransactionStatusKey', index: 'TransactionStatusKey', editable: true },
                { key: false, name: 'TransactionTypeKey', index: 'TransactionTypeKey', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatTransactionType, width: 15 },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false},
                { key: false, name: 'TransactionDate', index: 'TransactionDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'AccountLedgerName', index: 'AccountLedgerName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Amount', index: 'Amount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PartyTypeName', index: 'PartyTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PartyName', index: 'PartyName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TransactionStatusName', index: 'TransactionStatusName', formatter: formatStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editAccountTransactionLink, resizable: false, width: 200 },
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
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $(data.rows).each(function (i) {
                    var _this = $("#grid tr[id='" + i + "'] a[data-modal='']");
                    if ($("#grid tr").length > data.rows.length) {
                        _this = $("#grid tr:last a[data-modal='']");
                    }
                    AppCommon.EditGridPopup($(_this));
                    var obj = {};
                    obj.BranchKey = this.BranchKey;
                    AccountTransaction.SetUrlParam($(_this), obj);
                })
                $("#grid a[data-payment-modal='']").each(function () {
                    AppCommon.PaymentPopupWindow($("a[data-payment-modal='']"), $("#hdnAddEditAccountTransactionPayment").val(), Resources.Add + Resources.BlankSpace + Resources.Transactions + Resources.BlankSpace + Resources.Payment);
                })

            }
        })

        $("#grid").jqGrid("setLabel", "PaymentName", "", "thPaymentName");
    }

    var getPartyById = function (Id, Branch) {
        $ddl = $("#ddlParty");
        $ddl.html(""); // clear before appending new list 
        $ddl.append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Party));
        $.ajax({
            url: $("#hdnGetPartyByPartyType").val(),
            type: "GET",
            dataType: "JSON",
            data: { id: Id, BranchId: Branch },
            success: function (result) {
                $.each(result.Parties, function (i, Party) {
                    $ddl.append(
                        $('<option></option>').val(Party.RowKey).html(Party.Text));
                });
            }
        });
    }

    var getTransactionTypeById = function (LedgerId) {
        if (LedgerId != "") {
            $TransactionType = $("#TransactionTypeKey");
            var response = AjaxHelper.ajax("GET", $("#hdnGetTransactionTypeByLedger").val() + "/" + LedgerId, {

            });
            $TransactionType.val(response)
        }
    }


    function formatStatus(cellValue, options, rowdata, action) {
        var html = "";
        switch (rowdata.TransactionStatusKey) {
            case Resources.ProcessStatusApproved:
                html = '<span class="label label-success">' + cellValue + '</span>';
                break;
            case Resources.ProcessStatusRejected:
                html = '<span class="label label-danger">' + cellValue + '</span>';
                break;
            default:
                html = '<span class="label label-warning">' + cellValue + '</span>';

        }
        return html;
    }


    function editAccountTransactionLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        var editUrl = $("#hdnAddEditAccountTransaction").val() + "/" + rowdata.RowKey
        var html = '<div class="divEditDelete"><a class="btn btn-primary btn-sm" data-modal="" data-href="' + editUrl + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>'
            + '<a class="btn btn-danger btn-sm" onclick="javascript:deleteAccountTransaction(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a>';
        if (rowdata.TransactionStatusKey == Resources.ProcessStatusApproved) {
            html = html + '<a class="btn btn-warning btn-sm" data-payment-modal="" data-href="' + $("#hdnAddEditAccountTransactionPayment").val() + '/' + rowdata.RowKey + '"  ><i class="fa fa-credit-card" aria-hidden="true"></i>' + Resources.Payment + '</a>'
        }
        html = html + "</div>";
        return html;
    }

    function formatTransactionType(cellValue, options, rowdata, action) {

        return cellValue == Resources.CashFlowTypeIn ? "<i class='text-green fa fa-arrow-down'></i>" : "<i class='text-red fa fa-arrow-up'></i>";
    }

    var setUrlParam = function (lnk, data) {
        var obj = {};
        obj.branchKey = data.BranchKey ? data.BranchKey : 0;
        var url = $(lnk).data("href");
        url = url + "?" + $.param(obj);
        $(lnk).attr("data-href", url);
    }

    return {
        GetAccountTransactions: getAccountTransactions,
        GetPartyById: getPartyById,
        GetTransactionTypeById: getTransactionTypeById,
        SetUrlParam:setUrlParam
    }

}());

function deleteAccountTransaction(rowkey) {

    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Transaction,
        actionUrl: $("#hdnDeleteAccountTransaction").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}
