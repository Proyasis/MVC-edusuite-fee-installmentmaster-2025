var CashTransaction = (function () {

    var getCashTransactions = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetCashTransactions").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                BranchKey: function () {
                    return $('#FromBranchKey').val()
                },
                SearchText: function () {
                    return $('#txtsearch').val()
                },
                SearchDate: function () {
                    return $('#SearchDate').val()
                },
            },
            colNames: [Resources.RowKey, Resources.Date, Resources.FromBranch, Resources.ToBranch, Resources.Amount,
            //Resources.BankCharge,
            Resources.Remarks, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'TransactionDate', index: 'TransactionDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'FromBranchName', index: 'FromBranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ToBranchName', index: 'ToBranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Amount', index: 'Amount', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'currencyFmatter' },
                { key: false, name: 'Remarks', index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [10, 20, 50, 100].unique(),
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

        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" onclick="CashTransaction.EditPopup(' + rowdata.RowKey + ')" ><i class="fa fa-pencil" aria-hidden="true"></i></a><a  class="btn btn-outline-danger btn-sm mx-1" href="#"  onclick="javascript:deleteCashTransactions(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';

    }

    var getBankAccountById = function (Id, ddl) {
        $(ddl).html(""); // clear before appending new list 
        $(ddl).append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.BankAccount));
        if (Id != "") {
            var response = AjaxHelper.ajax("GET", $("#hdnGetBankAccountById").val() + "/" + Id);
            $.each(response, function (i, BankAccount) {
                $(ddl).append(
                    $('<option></option>').val(BankAccount.RowKey).html(BankAccount.Text));
            });
        }
    }

    var setUrlParam = function (lnk, data) {
        var obj = {};
        obj.CashTransactionsTypeKey = data.CashTransactionsTypeKey ? data.CashTransactionsTypeKey : 0;
        obj.branchKey = data.BranchKey ? data.BranchKey : 0;
        var url = $(lnk).data("href");
        url = url + "?" + $.param(obj);
        $(lnk).attr("data-href", url);
    }

    var editPopup = function (Id) {

        var obj = {};

        if ($("#BranchKey").val())
            obj.branchKey = $("#BranchKey").val();
        obj.Id = Id;
        //url = $("#hdnAddEditCashFlow").val() + "/" + Id + "?" + $.param(obj);

        var url = $("#hdnAddEditCashTransaction").val() + '?' + $.param(obj);
        var validator = null

        $.customPopupform.CustomPopup({
            modalsize: "modal-md ",
            load: function () {

            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.reload();
                }
            }
        }, url);

    }


    var branchChangeEvent = function (_this) {


        var obj = {}
        obj.id = $("#FromBranchKey", $("#frmCashTransaction")).val();
        AppCommon.BindDropDownbyId(obj, $("#hdnGetToBranchById").val(), $("#ToBranchKey"), Resources.Branch)

        var RowKey = $("#RowKey").val();
        CashTransaction.GetBalance(RowKey, $("#FromBranchKey", $("#frmCashTransaction")).val())
        CashTransaction.Checkbalance();


    }


    var getBalance = function (RowKey, branchKey) {
        var BranchKey = $("#FromBranchKey", $("#frmCashTransaction")).val();
       
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?RowKey=" + RowKey + "&branchKey=" + BranchKey)

        $("#AccountHeadBalance").val(response);
    }


    function checkbalance() {

        var amount = $("#Amount").val();
        amount = amount != "" ? parseFloat(amount) : 0;
        var paidAmount = parseFloat(amount)
        var AccountHeadBalance = parseFloat($("#AccountHeadBalance").val())

        var RowKey = $("#RowKey").val();
        RowKey = parseInt(RowKey) ? parseInt(RowKey) : 0;

        var oldAmount = $("#OldAmount").val();
        oldAmount = parseFloat(oldAmount) ? parseFloat(oldAmount) : 0

        var BankTransactionTypeKey = $("#BankTransactionTypeKey").val();
        BankTransactionTypeKey = parseInt(BankTransactionTypeKey) ? parseInt(BankTransactionTypeKey) : 0;
        var Message = "";

        Message = "Due to InSufficent Balance in Our account Please Check";
        AccountHeadBalance = AccountHeadBalance - amount;

        if (AccountHeadBalance < 0) {
            $("#Amount").val(oldAmount ? oldAmount : "")
            $.alert({
                type: 'yellow',
                title: Resources.Warning,
                content: Message,
                icon: 'fa fa-check-circle-o-',
                buttons: {
                    Ok: {
                        text: Resources.Ok,
                        btnClass: 'btn-success',
                        action: function () {
                            // window.location.href = $("#hdnEmployeeList").val();
                        }
                    }
                }
            })

        }

    }


    return {
        GetCashTransactions: getCashTransactions,
        BranchChangeEvent: branchChangeEvent,
        SetUrlParam: setUrlParam,
        EditPopup: editPopup,
        GetBalance: getBalance,
        Checkbalance: checkbalance
    }

}());

function deleteCashTransactions(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Journal,
        actionUrl: $("#hdnDeleteCashTransaction").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}

