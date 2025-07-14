var BankAccount = (function () {

    var getBankAccounts = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBankAccounts").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Branch, Resources.AccountNumber, Resources.Bank, Resources.Branch, Resources.NameInAccount, Resources.AccountType, Resources.IsActive, Resources.Action],

            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 100 },
                { key: false, name: 'AccountNumber', index: 'AccountNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BankName', index: 'BankName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchLocation', index: 'BranchLocation', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'IFSCCode', index: 'IFSCCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'MICRCode', index: 'MICRCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NameInAccount', index: 'NameInAccount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AccountTypeName', index: 'AccountTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'OpeningAccountBalance', index: 'OpeningAccountBalance', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'CurrentAccountBalance', index: 'CurrentAccountBalance', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
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
            loadonce: false,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this), BankAccountPopupLoad);
                })
            }
        })

        $("#grid").jqGrid("setLabel", "AccountNumber", "", "thAccountNumber");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditBankAccount").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBankAccount(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>';
    }

    var getBrnachByBranch = function (_this) {
        var obj = {};
        obj.Id = $(_this).val();
        AppCommon.BindDropDownbyId(obj, $("#hdnGetBankBranches").val(), $("#BranchKeys"), Resources.Branch);
    }

    return {
        GetBankAccounts: getBankAccounts,
        GetBrnachByBranch: getBrnachByBranch

    }
}());

function deleteBankAccount(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_BankAccount,
        actionUrl: $("#hdnDeleteBankAccount").val(),
        actionValue: rowkey,
        dataRefresh: function () { $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); }
    });
}

function BankAccountPopupLoad() {
    AppCommon.FormatInputCase();
    AppCommon.FormatDateInput();
}

