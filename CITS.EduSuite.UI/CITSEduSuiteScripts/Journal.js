var container = "#dynamicRepeater";
var Journal = (function () {
    var getJournal = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetJournal").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                BranchKey: function () {
                    return $('#BranchKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.Branch, Resources.Date, Resources.Remarks,
                //Resources.JournalCode,
                 Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'JournalDate', index: 'JournalDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'Remark', index: 'Remark', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'JournalCode', index: 'JournalCode', editable: true, cellEdit: true, sortable: true, resizable: false },

                { name: 'edit', search: false, index: 'BranchKey', sortable: false, formatter: editLink, resizable: false, width: 100 },
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
            sortname: 'RowKey',
            sortorder: 'desc',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                })

            }
        })

        $("#grid").jqGrid("setLabel", "journal", "", "thjournalName");
    }

    var getJournalDetail = function (json) {

        $(container).repeater(
            {
                show: function () {
                    $(this).slideDown();
                    AppCommon.CustomRepeaterRemoteMethod();
                    AppCommon.FormatDateInput();
                    $("[id*=Debit]").on("input", function () {
                        item = $(this).closest("[data-repeater-item]")
                        credit = $("[id*=Credit]", item)
                        credit.val("")
                        Journal.DebitCreditTotalCalc()
                    })
                    $("[id*=Credit]").on("input", function () {
                        item = $(this).closest("[data-repeater-item]")
                        debit = $("[id*=Debit]", item)
                        debit.val("")
                        Journal.DebitCreditTotalCalc()
                    })
                    Journal.GetAccountHeadById($("[id*=AccountGroupKey]", this))
                    AppCommon.FormatSelect2();
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('[id*=RowKey]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteJournalItem($(hidden).val(), $(this));
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
                                        $(".modal").modal("hide")
                                        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger("reloadGrid", [{ current: true }]);
                                    }
                                }
                            }
                        })

                    }

                },
                data: json,
                repeatlist: 'JournalDetails',
                submitButton: '',
                defaultValues: json,
                hasFile: true
            });
    }

    //var editPopup = function (_this) {

    //    // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
    //    //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
    //    var validator = null
    //    var url = $(_this).attr("data-href");

    //    $('#myModalContent').load(url, function () {
    //        $.validator.unobtrusive.parse($('#frmJournal'));
    //        $("#myModal").one('show.bs.modal', function () {


    //        }).modal({
    //            backdrop: 'static',
    //            keyboard: false
    //        }, 'show');

    //    });

    //}
    var editPopup = function (rowid) {

       
        var validator = null
      

        var url = $("#hdnAddEditJournal").val() + "?id=" + rowid;

        $.customPopupform.CustomPopup({
            modalsize: "modal-lg ",
            load: function () {
                setTimeout(function () {
                    $(".input-group-addon-end").each(function () {
                        //$.validator.unobtrusive.parse($('#frmJournal'));
                        $("form").removeData("validator");
                        $("form").removeData("unobtrusiveValidation");
                        $.validator.unobtrusive.parse("form");
                        AppCommon.SetInputAddOn(this);
                    });
                }, 500)
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.reload();
                }
            }
        }, url);

    }

    var debitCreditTotalCalc = function () {
        var totalDebit = 0
        var totalCredit = 0
        $("[data-repeater-item]").each(function () {
            var debit = $("[id*=Debit]", this).val()
            debit = parseFloat(debit) ? parseFloat(debit) : 0
            var credit = $("[id*=Credit]", this).val()
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

    var journalSubmit = function (_this) {

        var form = $(_this).closest("form");
        var validator = $(form).validate();
        var validate = $(form).valid();
        var totalDebit = $("#lblTotalDebit").val();
        totalDebit = parseFloat(totalDebit) ? parseFloat(totalDebit) : 0
        var totalCredit = $("#lblTotalCredit").val();
        totalCredit = parseFloat(totalCredit) ? parseFloat(totalCredit) : 0
        if (totalDebit != totalCredit) {
            $("[data-valmsg-for=error_msg]").html("Total Must have equal")
            $(form).mLoading("destroy");
            validate = false;
        }
        if (validate) {
            $(form).find(":checkbox").each(function () {
                var input = $(this).next("input");
                if (input[0]) {
                    $(input).val(this.checked);
                }
            })
            $("[disabled]", $(form)).removeAttr("disabled");
            var formData = new FormData($(form)[0]);
            $(form).mLoading();

            $.ajax({
                url: $(form)[0].action,
                type: $(form)[0].method,
                data: formData,
                contentType: false,
                cache: false,
                processData: false,
                success: function (response) {
                    if (response.IsSuccessful) {
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
                                        $('#myModal').modal('hide');
                                        window.location.reload();
                                        $(form).mLoading("destroy");
                                    }
                                }
                            }
                        })
                    }
                    else {
                        $('#myModalContent').html(response);
                        $("[data-valmsg-for=error_msg]").html(response.Message);
                    }

                    $(form).mLoading("destroy");
                },
                error: function (request, status, error) {
                    console.log(request.responseText);
                }
            });
        }
        else {
            validator.focusInvalid();
        }
    }

    var getAccountHeadById = function (_this)
    {
        
        var item = $(_this).closest("[data-repeater-item]")
        var obj = {}
        obj.Key = $(_this).val();
        AppCommon.BindDropDownbyId(obj, $("#hdnFillAccountHead").val(), $("select[id*=AccountHeadKey]", $(item)), Resources.AccountHead);
    } 

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-xs" onclick="Journal.EditPopup(this)" data-href="' + $("#hdnAddEditJournal").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a><a  class="btn btn-outline-danger btn-xs" href="#"  onclick="javascript:deleteJournal(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i>' + Resources.Delete + '</a></div>';
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mr-1" onclick="Journal.EditPopup(' + rowdata.RowKey+')"><i class="fa fa-pencil" aria-hidden="true"></i></a><a  class="btn btn-outline-danger btn-sm" href="#"  onclick="javascript:deleteJournal(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';

    }


    return {
        GetJournal: getJournal,
        EditPopup: editPopup,
        GetTabletr: getTabletr,
        DebitCreditTotalCalc: debitCreditTotalCalc,
        GetJournalDetail: getJournalDetail,
        JournalSubmit: journalSubmit,
        GetAccountHeadById: getAccountHeadById
    }
}());

function deleteJournal(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Journal,
        actionUrl: $("#hdnDeleteJournal").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteJournalItem(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_OrderProcess,
        actionUrl: $("#hdnDeleteJournalItem").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $(_this).remove()
            Journal.DebitCreditTotalCalc()
        }
    });
}




