var container = "#dynamicRepeater";
var jsonData = null;
var ColumnNamesList = [
            { name: "ReferenceKey", headertext: "Reference No", dummyData: "123" },
            { name: "TransactionDate", headertext: "Transaction Date", dummyData: '01/01/2018' },
            { name: "Particulars", headertext: "Particulars", dummyData: 'Abcd' },
            { name: "Amount", headertext: "Amount", dummyData: '100' },
            { name: "BankTransactionTypeKey", headertext: "Transaction Type", dummyData: 'CASH' },
            { name: "CashFlowTypeKey", headertext: "Amount Type", dummyData: 'IN' },
];
var BankStatement = (function () {
    var getBankStatements = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBankStatementList").val(),
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
            colNames: [Resources.RowKey, Resources.Bank, Resources.Branch, Resources.Month, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BankAccountName', index: 'BankAccountName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BankStatementMonth', index: 'BankStatementMonth', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'M Y' } },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 100 },
            ],
            pager: jQuery('#pager'),
            rowNum: Resources.PagingRowNum,
            rowList: [Resources.PagingRowNum,10, 20, 50, 100],
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

        })

        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
    }


    var getBankStatementDetail = function (json) {
        jsonData = json;
        $('.repeater').repeater(
         {
             show: function () {
                 $(this).slideDown();
                 var item = $(this);
                 $("[id*=RowKey]", item).val(0)
                 AppCommon.FormatSelect2();
                 AppCommon.FormatInputCase();
                 AppCommon.FormatDateInput();
             },

             hide: function (remove) {
                 var self = $(this).closest('[data-repeater-item]').get(0);
                 var hidden = $(self).find('input[type=hidden]')[0];
                 if ($(hidden).val() != "" && $(hidden).val() != "0") {
                     deleteBankStatementDetailsItem($(hidden).val(), $(this));
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
                                     window.location.href = $("#hdnBankStatementList").val()// + "/" + response["RowKey"];
                                 }
                             }
                         }
                     })

                 }

             },

             data: json,
             repeatlist: 'BankStatementDetails',
             submitButton: '',
         });

    }

    var getBank = function () {

        $ddl = $("#BankKey");
        $ddl.html(""); // clear before appending new list 
        $ddl.append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Bank));
        $.ajax({
            url: $("#hdnFillBank").val(),
            type: "GET",
            dataType: "JSON",
            data: {},
            success: function (result) {
                $.each(result.Banks, function (i, Bank) {
                    $ddl.append(
                        $('<option></option>').val(Bank.RowKey).html(Bank.Text));
                });
            }
        });
    }

    var handleFile = function (e) {
        $(".RightContentMaster").mLoading()
        var jsonObject = $.extend(true, {}, jsonData);
        //Get the files from Upload control
        var files = e.target.files;
        var i, f;
        var result;
        var target = e.target;
        for (i = 0, f = files[i]; i != files.length; ++i) {
            var reader = new FileReader();
            var name = f.name;
            reader.onload = function (e) {
                var data = e.target.result;
                var wb, arr;
                function doit() {
                    try {
                        wb = XLSX.read(data, { type: 'binary' });
                        var DetailJsonData = process_wb(wb)
                        BankStatement.ChangeDropdownKey(jsonObject, DetailJsonData)
                        $.ajax({
                            url: $("#hdnReadExcel").val(),
                            type: "POST",
                            data: jsonObject,
                            success: function (result) {
                                $("#dynamicRepeater").html(result)
                                $(".RightContentMaster").mLoading("destroy");

                            },
                            error: function () {
                                $(".RightContentMaster").mLoading("destroy");
                            }
                        });

                    } catch (e) { console.log(e); }
                }

                if (e.target.result.length > 1e6) opts.errors.large(e.target.result.length, function (e) { if (e) doit(); });
                else { doit(); }
            };
            reader.readAsBinaryString(f);

        }
    }

    var downloadExcel = function () {
        var data = [];

        var obj = {}
        obj.FileName = "Bank Statement"
        obj.ColumnNames = ColumnNamesList;
        var header = [];
        $(obj.ColumnNames).each(function () {
            header.push(this.name)
        })
        data.push(header)
        var item = [];
        $(obj.ColumnNames).each(function () {
            item.push(this.dummyData)
        })
        data.push(item)

        var wb = new Workbook(), ws = sheet_from_array_of_arrays(data);
        /* add worksheet to workbook */
        wb.SheetNames.push(obj.FileName);
        wb.Sheets[obj.FileName] = ws;
        var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary' })
        saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), obj.FileName + ".xlsx")
    }

    var changeDropdownKey = function (jsonObject, DetailJsonData) {
        var bankTransaction = jsonObject["BankStatementDetails"][0]["BankTransactionTypes"]
        var cashFlowType = jsonObject["BankStatementDetails"][0]["CashFlowTypes"]

        jsonObject["BankStatementDetails"].splice(1)
        $(ColumnNamesList).each(function (index) {
            var dummyData;
            var col = this;
            if (index == 4) {
                dummyData = bankTransaction.filter(function (n, p) {
                    return col.dummyData.toLowerCase() === n.GroupName.toLowerCase()
                })[0]
            }
            else if (index == 5) {
                dummyData = cashFlowType.filter(function (n, p) {
                    return col.dummyData.toLowerCase() === n.Text.toLowerCase()
                })[0]
            }
            else {
                dummyData = col.dummyData;
            }
            jsonObject["BankStatementDetails"][0][this.name] = dummyData;
        })
        var obj = jsonObject["BankStatementDetails"][0];
        var isClone = false;

        $(DetailJsonData).each(function (index) {
            if (index > 0) {

                var itemCode = bankTransaction.filter(function (n, p) {
                    return DetailJsonData[index][4].toLowerCase() === n.GroupName.toLowerCase()
                })[0]
                var CashFlowKey = cashFlowType.filter(function (n, p) {
                    return DetailJsonData[index][5].toLowerCase() === n.Text.toLowerCase()
                })[0]
                obj.ReferenceKey = DetailJsonData[index][0]
                obj.TransactionDate = DetailJsonData[index][1]
                obj.Particulars = DetailJsonData[index][2]
                obj.Amount = DetailJsonData[index][3].replace(/\,/g, '')
                if (itemCode)
                    obj.BankTransactionTypeKey = itemCode.RowKey ? itemCode.RowKey : null;
                if (CashFlowKey)
                    obj.CashFlowTypeKey = CashFlowKey.RowKey ? CashFlowKey.RowKey : 0;
                if (isClone)
                    jsonObject["BankStatementDetails"].push(obj)

                obj = $.extend(true, {}, obj);
                isClone = true;
            }
        });


    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnAddEditBankStatement").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBankStatement(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i>  </a></div>';
    }

    return {
        GetBankStatements: getBankStatements,
        GetBank: getBank,
        GetBankStatementDetail: getBankStatementDetail,
        HandleFile: handleFile,
        ChangeDropdownKey: changeDropdownKey,
        DownloadExcel: downloadExcel

    }
}());




function deleteBankStatement(rowkey) {
    //var result = confirm(Resources.Delete_Confirm_BankStatement);
    //if (result == true) {
    //    var response = AjaxHelper.ajax("POST", $("#hdnDeleteBankStatement").val(),
    //                {
    //                    id: rowkey
    //                });

    //    if (response.Message === Resources.Success) {
    //        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
    //    }
    //    else
    //        alert(response.Message);
    //    event.preventDefault();
    //}

    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_BankStatement,
        actionUrl: $("#hdnDeleteBankStatement").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteBankStatementDetailsItem(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_BankStatement,
        actionUrl: $("#hdnDeleteBankStatementItem").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            window.location.reload();
        }
    });
}



