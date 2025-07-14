var BankTransaction = (function () {

    var getBankTransactionByType = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBankTransactionsByType").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                BankTransactionTypeKey: function () {
                    return $('#BankTransactionTypeKey').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                SearchText: function () {
                    return $('#txtsearch').val()
                },
                SearchDate: function () {
                    return $('#SearchDate').val()
                },
                SearchBankAccountKey: function () {
                    return $('#SearchBankAccountKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.VoucherNumber, Resources.Branch, Resources.BankTransactionType, Resources.Date, Resources.FromBankAccount, Resources.ToBankAccount, Resources.Amount,
            //Resources.BankCharge,
            Resources.Remarks, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'BankTransactionTypeKey', index: 'BankTransactionTypeKey', editable: true },
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, name: 'ReceiptNumber', index: 'ReceiptNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BankTransactionTypeName', index: 'BankTransactionTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TransactionDate', index: 'TransactionDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'FromBankAccountName', index: 'FromBankAccountName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ToBankAccountName', index: 'ToBankAccountName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Amount', index: 'Amount', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'currencyFmatter' },
                //{ key: false, name: 'BankCharge', index: 'BankCharge', editable: true, cellEdit: true, sortable: true, resizable: false },
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
            //loadComplete: function (data) {
            //    $("#grid a[data-modal='']").each(function () {

            //    })
            //    $(data.rows).each(function (i) {
            //        var _this = $("#grid tr[id='" + i + "'] a[data-modal='']");
            //        if ($("#grid tr").length > data.rows.length) {
            //            _this = $("#grid tr:last a[data-modal='']");
            //        }
            //        AppCommon.EditGridPopup($(_this));
            //        var obj = {};
            //        obj.BankTransactionTypeKey = $("#tab-componets li.active a").attr('data-val');
            //        obj.BranchKey = this.BranchKey;
            //        BankTransaction.SetUrlParam($(_this), obj);
            //    })
            //}
        })


        $("#grid").jqGrid("setLabel", "TransactionDate", "", "thTransactionDate");


    }
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        var printtemp = "'" + rowdata.BranchKey + "'" + ',' + "'" + rowdata.ReceiptNumber + "'" + ',' + "'" + Resources.InvoicePrintTypeOrderReceipt + "'";
        // return '<div class="divEditDelete"><a class="btn btn-primary btn-xs" data-modal="" data-href="AddEditBankTransaction/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-danger btn-xs" onclick="javascript:deleteBankTransaction(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a></div>';

        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" onclick="BankTransaction.EditPopup(' + rowdata.BankTransactionTypeKey + ',' + rowdata.RowKey + ')" ><i class="fa fa-pencil" aria-hidden="true"></i></a><a  class="btn btn-outline-danger btn-sm mx-1" href="#"  onclick="javascript:deleteBankTransaction(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-success btn-sm mx-1 btnPrint" onclick="changePrintHtml(' + rowdata.BankTransactionTypeKey + ');PrintInvoice.PrintReceipt(null, ' + printtemp + ')"><i class="fa fa-print" aria-hidden="true"></i></a></div>';

    }

    function hideAndShowColumns(value) {
        jQuery("#grid").jqGrid('showCol', ["FromBankAccountName", "ToBankAccountName", "BankCharge"]);
        switch (parseInt(value)) {

            case 1:
                jQuery("#grid").jqGrid('hideCol', ["FromBankAccountName", "BankCharge"]);
                break;
            case 2:
                jQuery("#grid").jqGrid('hideCol', ["ToBankAccountName", "BankCharge"]);
                break;


        }

        jQuery("#list").jqGrid('showCol', ["colModel1_name", "colModel2_name"]);
    }
    var checkBankTransactionType = function (Type) {

        switch (parseInt(Type || 0)) {
            case Resources.BankTransactionTypeDeposit:
                $(".divToBankAccount").show();
                $(".divFromBankAccount,.divBankCharge").hide();
                break;
            case Resources.BankTransactionTypeWithdrawal:
                $(".divFromBankAccount").show();
                $(".divToBankAccount,.divBankCharge").hide();
                break;
            case Resources.BankTransactionTypeAccountTransfer:
                $(".divToBankAccount,.divFromBankAccount,.divBankCharge").show();
                $(".divAmount").removeClass("col-sm-4").addClass("col-sm-2")
                break;
            default:
                $(".divToBankAccount,.divFromBankAccount,.divBankCharge").show();
                break;
        }

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
        obj.bankTransactionTypeKey = data.BankTransactionTypeKey ? data.BankTransactionTypeKey : 0;
        obj.branchKey = data.BranchKey ? data.BranchKey : 0;
        var url = $(lnk).data("href");
        url = url + "?" + $.param(obj);
        $(lnk).attr("data-href", url);
    }

    var editPopup = function (bankTransactionTypeKey, Id) {

        var obj = {};
        obj.bankTransactionTypeKey = bankTransactionTypeKey;
        if ($("#BranchKey").val())
            obj.branchKey = $("#BranchKey").val();
        //obj.Id = Id;
        url = $("#hdnAddEditBankTransaction").val() + "/" + Id + "?" + $.param(obj);

        //var url = $("#hdnAddEditBankTransaction").val() + '?' + $.param(obj);
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


    var getBankByBank = function (_this) {
        var obj = {};
        obj.Id = $(_this).val();
        obj.Id = parseInt(obj.Id) ? parseInt(obj.Id) : 0;
        if (obj.Id != 0) {

            obj.BranchKey = $("[id*=BranchKey]", $("#frmBankTransaction")).val();
            AppCommon.BindDropDownbyId(obj, $("#hdnGetBankAccountById").val(), $("#ToBankAccountKey"), Resources.BankAccount);
        }
    }


    var getBalance = function (RowKey, BankAccountKey, branchKey, BankTransactionTypeKey) {
        var url = $("#hdnGetBalance").val()
        var response = AjaxHelper.ajax("GET", $("#hdnGetBalance").val() + "?RowKey=" + RowKey + "&BankAccountKey=" + BankAccountKey + "&branchKey=" + branchKey + "&BankTransactionTypeKey=" + BankTransactionTypeKey)

        $("#AccountHeadBalance").val(response);
    }


    return {
        GetBankTransactionByType: getBankTransactionByType,
        HideAndShowColumns: hideAndShowColumns,
        CheckBankTransactionType: checkBankTransactionType,
        GetBankByBank: getBankByBank,
        SetUrlParam: setUrlParam,
        EditPopup: editPopup,
        GetBalance: getBalance
    }

}());

function deleteBankTransaction(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Journal,
        actionUrl: $("#hdnDeleteBankTransaction").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}

function changePrintHtml(key) {
    if (key == 2) {

        $("#hdnReceiptPath").val($("#hdnDummyReceiptPath").val())
    }
    else {
        $("#hdnReceiptPath").val($("#hdnVoucherPath").val())
    }
}