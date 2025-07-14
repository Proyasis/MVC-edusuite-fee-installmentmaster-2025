var AccountHeadOpeningBalance = (function () {
    var getAccountHeadOpeningBalance = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAccountHeadOpeningBalance").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                BranchKey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Branch, Resources.Date, Resources.TotalCreditAmount, Resources.TotalDebitAmount, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'OpeningDate', index: 'OpeningDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                //{ key: false, name: 'AccountHeadOpeningBalanceCode', index: 'AccountHeadOpeningBalanceCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TotalCredit', index: 'TotalCredit', editable: true, cellEdit: true, sortable: true, resizable: false, width: 50 },
                { key: false, name: 'TotalDebit', index: 'TotalDebit', editable: true, cellEdit: true, sortable: true, resizable: false, width: 50 },

                { name: 'edit', search: false, index: 'BranchKey', sortable: false, formatter: editLink, resizable: false, width: 100 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            sortname: 'RowKey',
            sortorder: 'desc',
            //loadComplete: function (data) {
            //    $("#grid a[ledger-modal='']").each(function () {
            //        AppCommon.EditGridPopup($(this))
            //    })
            //}
        })

        $("#grid").jqGrid("setLabel", "BranchName", "", "BranchName");
    }

    var editPopup = function (_this) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        //var validator = null
        //var url = $(_this).attr("data-href");

        //$('#myModalContent').load(url, function () {
        //    $.validator.unobtrusive.parse($('#frmAccountHeadOpeningBalance'));
        //    $("#myModal").one('show.bs.modal', function () {

        //        bindAccountHeadOpeningBalanceForm(this);

        //    }).modal({
        //        backdrop: 'static',
        //        keyboard: false
        //    }, 'show');

        //});
        var validator = null
        var url = $(_this).attr("data-href");
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg ",
            load: function () {
                setTimeout(function () {
                    
                    $(".input-group-addon-end").each(function () {
                        AppCommon.SetInputAddOn(this);

                    });
                }, 500)
            },
            submit: function (result) {

                bindAccountHeadOpeningBalanceForm()
               
            }
        }, url);

    }
  

    var debitCreditTotalCalc = function () {
        
        var totalDebit = 0
        var totalCredit = 0
        $("[data-repeater-item]").each(function () {
            var debit = $("[id*=DebitAmount]", this).val()
            debit = parseFloat(debit) ? parseFloat(debit) : 0
            var credit = $("[id*=CreditAmount]", this).val()
            credit = parseFloat(credit) ? parseFloat(credit) : 0
            totalDebit = totalDebit + debit;
            totalCredit = totalCredit + credit;
        })
        $("#lblTotalDebit").val(totalDebit)
        $("#lblTotalCredit").val(totalCredit)
    }

    var getTabletr = function () {
        var heights = []
        $('tr').each(function () {
            heights.push($(this).height());
        }).get();

    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.BranchKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" onclick="AccountHeadOpeningBalance.EditPopup(this)" data-href="' + $("#hdnAddEditAccountHeadOpeningBalance").val() + '/' + rowdata.BranchKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a  class="btn btn-outline-danger btn-sm mx-1" href="#"  onclick="javascript:deleteAccountHeadOpeningBalance(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }


    return {
        GetAccountHeadOpeningBalance: getAccountHeadOpeningBalance,
        EditPopup: editPopup,
        GetTabletr: getTabletr,
        DebitCreditTotalCalc: debitCreditTotalCalc
    }
}());

function deleteAccountHeadOpeningBalance(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_AccountHeadOpeningBalance,
        actionUrl: $("#hdnDeleteAccountHeadOpeningBalance").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function bindAccountHeadOpeningBalanceForm() {
    var form = $('#frmAccountHeadOpeningBalance');
    var validate = $(form).valid();
       
        if (validate) {
            $(form).mLoading();
            var totalDebit = $("#lblTotalDebit").val();
            totalDebit = parseFloat(totalDebit) ? parseFloat(totalDebit) : 0
            var totalCredit = $("#lblTotalCredit").val();
            totalCredit = parseFloat(totalCredit) ? parseFloat(totalCredit) : 0
            if (totalDebit != totalCredit) {
                $("[data-valmsg-for=error_msg]").html("Total Must have equal")
                $(form).mLoading("destroy");
                return false;
            }
            var AccountReceivable = $("#AccountReceivable").val()
            AccountReceivable = parseFloat(AccountReceivable) ? parseFloat(AccountReceivable) : 0;
            var AccountPayable = $("#AccountPayable").val()
            AccountPayable = parseFloat(AccountPayable) ? parseFloat(AccountPayable) : 0;
            var AdvancePayable = $("#AdvancePayable").val()
            AdvancePayable = parseFloat(AdvancePayable) ? parseFloat(AdvancePayable) : 0;
            var AdvanceReceivable = $("#AdvanceReceivable").val()
            AdvanceReceivable = parseFloat(AdvanceReceivable) ? parseFloat(AdvanceReceivable) : 0;
            var Inventory = $("#Inventory").val()
            Inventory = parseFloat(Inventory) ? parseFloat(Inventory) : 0;
            var isReturn = true
            $("[data-repeater-item]").each(function () {
                var AccountHeadKey = $("[id*=AccountHeadKey]", $(this)).val()
                var debit = $("[id*=DebitAmount]", $(this)).val()
                debit = parseFloat(debit) ? parseFloat(debit) : 0;
                var credit = $("[id*=CreditAmount]", $(this)).val()
                credit = parseFloat(credit) ? parseFloat(credit) : 0;
                AccountHeadKey = parseInt(AccountHeadKey) ? parseInt(AccountHeadKey) : 0;
                if (AccountHeadKey == Resources.HeadAccountReceivable && (debit + credit) != AccountReceivable && (debit + credit) != 0) {
                    $("[data-valmsg-for=error_msg]").html("Total Debtors Receivable Opening is not equal")
                    $(form).mLoading("destroy");
                    isReturn = false;
                }
                else if (AccountHeadKey == Resources.HeadAccountsPayable && (debit + credit) != AccountPayable && (debit + credit) != 0) {
                    $("[data-valmsg-for=error_msg]").html("Total Creditors Payable Opening is not equal")
                    $(form).mLoading("destroy");
                    isReturn = false;
                }
                else if (AccountHeadKey == Resources.HeadAdvanceReceivable && (debit + credit) != AccountReceivable && (debit + credit) != 0) {
                    $("[data-valmsg-for=error_msg]").html("Total Creditors Advance Opening is not equal")
                    $(form).mLoading("destroy");
                    isReturn = false;
                }
                else if (AccountHeadKey == Resources.HeadAdvancePayable && (debit + credit) != AdvancePayable && (debit + credit) != 0) {
                    $("[data-valmsg-for=error_msg]").html("Total Debtors Advance Opening is not equal")
                    $(form).mLoading("destroy");
                    isReturn = false;
                }
                else if (AccountHeadKey == Resources.HeadInventory && (debit + credit) != Inventory && (debit + credit) != 0) {
                    $("[data-valmsg-for=error_msg]").html("Total Stock Value Opening is not equal")
                    $(form).mLoading("destroy");
                    isReturn = false;
                }
            })
            if (isReturn) {
                $.ajax({
                    url: form[0].action,
                    type: form[0].method,
                    data: $(form).serialize(),
                    success: function (result) {
                        if (result.IsSuccessful) {
                            $(form).closest('.modal').modal('hide');
                            window.location.reload();
                        } else {
                            //$('#myModalContent').html(result);
                            //bindAccountHeadOpeningBalanceForm(dialog)
                        }
                        $(form).mLoading("destroy");
                    }
                });
            }

        }

        return false;

}




