var jsonReconciliation = null

var BankReconciliation = (function () {

    var getBankStatementList = function () {
        var appendStatement = [];
        $("[bank-statement-panel]").each(function (statementItem) {
            if ($(this).html().trim() != "") {
                if (!$(this).is("[used]")) {
                    var bankStatementReference = $("[id*=ReferenceKey]", $(this)).val()
                    var bankStatementDate = $("[id*=TransactionDate]", $(this)).val()
                    var bankStatementAmount = $("[id*=Amount]", $(this)).val()
                    var bankStatementCashFlowType = $("[id*=CashFlowTypeKey]", $(this)).val()
                    var bankstatementItem = $(this)
                    var bankStatementItemHtml = $(this).html()
                    var statementlistitem = bankstatementItem.closest("[data-repeater-item]")
                    var leftStatementKey = $("[id*=BankStatementKey]", $(this)).val()
                    var isMatch = false;
                    var isUsed = false;
                    $("[default-bankdetails-panel]").each(function () {
                        if ($(this).html().trim() != "") {
                            var defaultbankItem = $(this)
                            var defaultBankReference = $("[id*=ReferenceKey]", defaultbankItem).val()
                            var defaultBankDate = $("[id*=TransactionDate]", defaultbankItem).val()
                            var defaultBankAmount = $("[id*=Amount]", defaultbankItem).val()
                            var defaultBankCashFlowType = $("[id*=CashFlowTypeKey]", defaultbankItem).val()
                            var defaultlistitem = defaultbankItem.closest("[data-repeater-item]")
                            var rightBankPanel = $("[bank-statement-panel]", defaultlistitem)
                            var rightStatementKey = $("[id*=BankStatementKey]", rightBankPanel).val()
                            //defaultlistitem.attr("data-index", "3")
                            if ((defaultBankReference == bankStatementReference) || (bankStatementDate == defaultBankDate && bankStatementAmount == defaultBankAmount && bankStatementCashFlowType == defaultBankCashFlowType)) {
                                if (isUsed == false) {
                                    bankstatementItem.html("")
                                    $("[bank-statement-panel]", defaultlistitem).html(bankStatementItemHtml)
                                    $("[bank-statement-panel]", defaultlistitem).attr("used", "")
                                    defaultlistitem.find("[event]").html("")
                                    defaultlistitem.find("[event]").append(
                                                       '<a match href="javascript:" class="btn btn-success btn-xs" onclick="BankReconciliation.MatchItem($(this))">' +
                                                            '<span class="fa fa-check"></span>' +
                                                       '</a>')

                                    var defaultbankdetails = $("[default-bankdetails-panel]", statementlistitem).html()
                                    var defaultbankdetailItem = $("[default-bankdetails-panel]", statementlistitem)
                                    if (!defaultbankdetails) {
                                        statementlistitem.remove()
                                    }
                                    else if (leftStatementKey != rightStatementKey) {
                                        statementlistitem.find("[event]").html("")
                                        statementlistitem.find("[event]").append(
                                                       '<a remove href="javascript:" class="btn btn-outline-danger btn-xs" onclick="BankReconciliation.RemoveItem($(this))">' +
                                                        '<span class="fa fa-remove"></span>' +
                                                       '</a>')
                                    }
                                    defaultlistitem.attr("data-index", "0")
                                    isUsed = true;
                                    isMatch = true;
                                }
                            }
                        }
                    })
                    if (isMatch == false) {
                        bankstatementItem.html("")
                        var listItem = bankstatementItem.closest("[data-repeater-item]")
                        listItem = listItem.clone(true);
                        bankstatementItem.closest("[data-repeater-item]").remove();
                        listItem.attr("data-index", "2")
                        var defaultbankdetails = $("[default-bankdetails-panel]", listItem).html()
                        var defaultbankdetailItem = $("[default-bankdetails-panel]", listItem)
                        if (!defaultbankdetails) {
                            listItem.remove()
                        }
                        else {
                            listItem.find("[event]").html("")
                            listItem.find("[event]").append(
                                               '<a remove href="javascript:" class="btn btn-outline-danger btn-xs" onclick="BankReconciliation.RemoveItem($(this))">' +
                                                    '<span class="fa fa-remove"></span>' +
                                               '</a>')
                        }

                        appendStatement.push(bankStatementItemHtml)
                    }
                }
            }
        })
        $("[used]").removeAttr("used")
        appendStatement = appendStatement;
        $(appendStatement).each(function (i) {
            var item = appendStatement[i]
            $("[data-repeater-list]").append(
                            '<div data-index="2" data-repeater-item class="form-row">' +
                                  '<div leftitem class="col-5">' +
                                        '<div bank-statement-panel >' +
                                            item +
                                        '</div>' +
                                  '</div>' +
                                  '<div class="col-1">' +
                                       '<a move class="btn btn-info btn-xs" onclick="BankReconciliation.MoveItem($(this))">' +
                                            '<span class="fa fa-arrow-right"></span>' +
                                       '</a>' +
                                  '</div>' +
                                  '<div rightitem class="col-5">' +
                                  '</div>' +
                            '</div>')
        })
        BankReconciliation.SortAndIndexing();
    }

    var moveItem = function (_this) {
        if ($(_this).is("[move]")) {
            var listItem = $(_this).closest("[data-repeater-item]")
            listItem.attr("data-index", "1")
            var statementItem = $("[leftitem]", listItem).html()
            var rightitem = $("[rightitem]", listItem)
            var event = $(".fa-arrow-right", listItem)
            statementItem = statementItem.replace(/BankStatementDetails/g, "DefaultBankPaymentDetails");
            rightitem.hide();
            statementItem = statementItem.replace(/bank-statement-panel/g, "default-bankdetails-panel");
            rightitem.html(statementItem)
            $("[id*=ProcessStatusKey]", rightitem).val(Resources.ProcessStatusApproved)
            $("[id*=TransactionTypeKey]", rightitem).val(Resources.OtherBankTransactionKey)
            $("[id*=IsReconcile]", rightitem).val(true)
            var accountHead = $("select[id*=AccountHeadKey]", rightitem)
            accountHead.show()
            accountHead.selectpicker()
            rightitem.show("slide", {
                direction: "left", complete: function () {
                    BankReconciliation.SortAndIndexing();
                    accountHead.selectpicker('show');
                }
            }, 1000);

            event.removeClass("fa-arrow-right")
            event.addClass("fa-arrow-left")
            _this.removeAttr("move")
            _this.attr("undo", "")
            $(".card-heading", listItem).addClass("disablePanel")
            $(".card-body", listItem).addClass("disablePanel")
        }
        else {
            BankReconciliation.UndoItem(_this)
        }

    }

    var undoItem = function (_this) {
        var listItem = $(_this).closest("[data-repeater-item]")
        var statementItem = $("[leftitem]", listItem).html()
        var rightitem = $("[rightitem]", listItem)
        var event = $(".fa-arrow-left", listItem)
        listItem.attr("data-index", "2")
        $("[id*=TransactionTypeKey]", rightitem).val(0)
        $("[id*=IsReconcile]", rightitem).val(false)
        rightitem.hide("slide", {
            direction: "left", complete: function () {
                BankReconciliation.SortAndIndexing();
                rightitem.html("")
            }
        }, 1000);
        event.removeClass("fa-arrow-left")
        event.addClass("fa-arrow-right")
        _this.removeAttr("undo")
        _this.attr("move", "")
        $(".card-heading", listItem).removeClass("disablePanel")
        $(".card-body", listItem).removeClass("disablePanel")
    }

    var matchItem = function (_this) {
        var listItem = $(_this).closest("[data-repeater-item]")
        var rightitem = $("[rightitem]", listItem)
        var leftItem = $("[leftitem]", listItem)
        var bankStatementKey = $("[id*=BankStatementKey]", leftItem).val()
        if ($(_this).is("[match]")) {
            var event = $(".fa-check", listItem)
            $("[id*=ProcessStatusKey]", rightitem).val(Resources.ProcessStatusApproved)
            $("[id*=IsReconcile]", rightitem).val(true)
            $("[id*=BankStatementKey]", rightitem).val(bankStatementKey)
            event.removeClass("fa-check")
            event.addClass("fa-undo")
            _this.removeAttr("match")
            _this.attr("undoMatchItem", "")
            _this.removeClass("btn-success")
            _this.addClass("btn-outline-primary")
            $(".card-heading", listItem).addClass("disablePanel")
            $(".card-body", listItem).addClass("disablePanel")
        }
        else {
            var event = $(".fa-undo", listItem)
            $("[id*=TransactionTypeKey]", listItem).val(0)
            $("[id*=ProcessStatusKey]", rightitem).val(Resources.ProcessStatusPending)
            $("[id*=IsReconcile]", rightitem).val(false)
            $("[id*=BankStatementKey]", rightitem).val(0)
            event.removeClass("fa-undo")
            event.addClass("fa-check")
            _this.removeAttr("undoMatchItem")
            _this.attr("match", "")
            _this.removeClass("btn-outline-primary")
            _this.addClass("btn-success")
            $(".card-heading", listItem).removeClass("disablePanel")
            $(".card-body", listItem).removeClass("disablePanel")
        }
    }
    var removeItem = function (_this) {
        var listItem = $(_this).closest("[data-repeater-item]")
        var rightitem = $("[rightitem]", listItem)
        if ($(_this).is("[remove]")) {
            var event = $(".fa-remove", listItem)
            $("[id*=ProcessStatusKey]", rightitem).val(Resources.ProcessStatusRejected)
            $("[id*=IsReconcile]", rightitem).val(true)
            event.removeClass("fa-remove")
            event.addClass("fa-undo")
            _this.removeAttr("remove")
            _this.attr("undoRemoveItem", "")
            _this.removeClass("btn-danger")
            _this.addClass("btn-outline-primary")
            $(".card-heading", rightitem).addClass("disablePanel")
            $(".card-body", rightitem).addClass("disablePanel")
        }
        else {
            var event = $(".fa-undo", listItem)
            $("[id*=ProcessStatusKey]", rightitem).val(Resources.ProcessStatusPending)
            $("[id*=IsReconcile]", rightitem).val(false)
            event.removeClass("fa-undo")
            event.addClass("fa-remove")
            _this.removeAttr("undoRemoveItem")
            _this.attr("remove", "")
            _this.removeClass("btn-outline-primary")
            _this.addClass("btn-danger")
            $(".card-heading", rightitem).removeClass("disablePanel")
            $(".card-body", rightitem).removeClass("disablePanel")
        }
    }

    var selectItem = function (selectStatementKey) {
        var index = $("#searchTag").val()
        var _this = $("[searchIndex=" + index + "]", $("[data-repeater-list]"))
        var listItem = $(_this).closest("[data-repeater-item]")
        var rightitem = $("[bank-statement-panel]", listItem)
        var rightitemhtml = rightitem.html()
        var isUsed = false;
        $("[bank-statement-panel]").each(function () {
            var bankStatementKey = $("[id*=BankStatementKey]", $(this)).val()
            var defaultStatementKey = $("[id*=BankStatementKey]", rightitem).val()
            if (selectStatementKey == bankStatementKey && isUsed == false) {
                isUsed = true;
                var MatchlistItem = $($(this)).closest("[data-repeater-item]")
                var selectHtml = $(this).html()
                $(this).html(rightitemhtml)
                rightitem.html(selectHtml)
                $("[event]", MatchlistItem).html("")
                $("[event]", listItem).html("")
                if (rightitemhtml != "") {
                    MatchlistItem.attr("data-index", "0")
                    MatchlistItem.find("[event]").append(
                                           '<a match href="javascript:" class="btn btn-success btn-xs" onclick="BankReconciliation.MatchItem($(this))">' +
                                                '<span class="fa fa-check"></span>' +
                                           '</a>')
                }
                else {
                    MatchlistItem.attr("data-index", "3")
                    MatchlistItem.find("[event]").append(
                                           '<a remove href="javascript:" class="btn btn-outline-danger btn-xs" onclick="BankReconciliation.RemoveItem($(this))">' +
                                                '<span class="fa fa-remove"></span>' +
                                           '</a>')
                }
                if (defaultStatementKey != selectStatementKey) {
                    if (selectHtml != "") {
                        listItem.attr("data-index", "0")
                        listItem.find("[event]").append(
                                               '<a match href="javascript:" class="btn btn-success btn-xs" onclick="BankReconciliation.MatchItem($(this))">' +
                                                    '<span class="fa fa-check"></span>' +
                                               '</a>')
                    }
                    else {
                        listItem.attr("data-index", "3")
                        listItem.find("[event]").append(
                                               '<a remove href="javascript:" class="btn btn-outline-danger btn-xs" onclick="BankReconciliation.RemoveItem($(this))">' +
                                                    '<span class="fa fa-remove"></span>' +
                                               '</a>')
                    }
                }
            }
        })
        BankReconciliation.SortAndIndexing();
        $("[data-dismiss=modal]").click()

    }

    var matchStatementPopup = function (_this, obj, JsonData) {
        var index = _this.attr("searchIndex")
        $("#searchTag").val(index)
        var validator = null

        $(JsonData["BankStatementDetails"]).each(function () {
            var TransactionDate = AppCommon.JsonDateToNormalDate(this.TransactionDate)
            this.TransactionDate = TransactionDate;
        });
        var CheckDate = obj.TransactionDate.split('/');
        CheckDate = CheckDate[0] + '/' + CheckDate[1] + '/' + CheckDate[2].split(' ').slice(0, 1);
        JsonData["BankStatementDetails"] = JsonData["BankStatementDetails"].filter(function (n, p) {
            return n.Amount == obj.Amount && n.TransactionDate == CheckDate && n.CashFlowTypeKey == obj.CashFlowType
        })
        var url = $("#hdnMatchedStatementList").val();

        $.customPopupform.CustomPopup({
            ajaxType: "POST",
            ajaxData: JsonData,
            modalsize: "modal-lg mw-100 w-75",
            load: function () {
                setTimeout(function () {
                    $(".input-group-addon-end").each(function () {
                        AppCommon.SetInputAddOn(this);
                    });
                }, 500)
            },
            rebind: function () {
            }
        }, url);
    }

    var viewReconcilePopup = function (_this,obj) {

        var validator = null
        var url = $(_this).attr("data-href");

        $.customPopupform.CustomPopup({
            ajaxData: obj,
            modalsize: "modal-lg mw-100 w-75",
            load: function () {
                setTimeout(function () {
                    $(".input-group-addon-end").each(function () {
                        AppCommon.SetInputAddOn(this);
                    });
                }, 500)
            },
            rebind: function () {
            }
        }, url);
    }

    var sortAndIndexing = function () {
        var $list = $("[data-repeater-list]");
        $list.find('[data-repeater-item]').sort(function (a, b) {
            return +a.dataset.index - +b.dataset.index;
        })
       .appendTo($list);

        $list.find('[bank-statement-panel]').each(function (index) {
            var nonIndex = 0;
            if ($(this).html() != "") {
                index = index - nonIndex
                $(this).find('[name]').each(function () {
                    var oldIndex = $(this).attr('name').match(/\[([0-9]*)\]/) && $(this).attr('name').match(/\[([0-9]*)\]/).length > 0 ? $(this).attr('name').match(/\[([0-9]*)\]/)[1] : null;
                    if (oldIndex)
                        $(this).attr('name', $(this).attr('name').replace(oldIndex, index, toString()));
                    $(this).attr('id', $(this).attr('name'));
                });
                $(this).find('[data-valmsg-for]').each(function () {
                    // match non empty brackets (ex: "[foo]")
                    // var oldIndex = $(this).attr('name').match(/\[([0-9]*)\]/)[1];
                    var oldIndex = $(this).data('valmsg-for').match(/\[([0-9]*)\]/) && $(this).data('valmsg-for').match(/\[([0-9]*)\]/).length > 0 ? $(this).data('valmsg-for').match(/\[([0-9]*)\]/)[1] : null;
                    if (oldIndex)
                        $(this).data('valmsg-for', $(this).data('valmsg-for').replace(oldIndex, index, toString()));
                });
            }
            else {
                nonIndex = nonIndex + 1;
            }
        });

        $list.find('[default-bankdetails-panel]').each(function (index) {
            var nonIndex = 0;
            if ($(this).html() != "") {
                index = index - nonIndex
                $(this).find('[name]').each(function () {
                    // match non empty brackets (ex: "[foo]")
                    // var oldIndex = $(this).attr('name').match(/\[([0-9]*)\]/)[1];
                    var oldIndex = $(this).attr('name').match(/\[([0-9]*)\]/) && $(this).attr('name').match(/\[([0-9]*)\]/).length > 0 ? $(this).attr('name').match(/\[([0-9]*)\]/)[1] : null;
                    if (oldIndex)
                        $(this).attr('name', $(this).attr('name').replace(oldIndex, index, toString()));
                    $(this).attr('id', $(this).attr('name'));
                });
                $(this).find('[data-valmsg-for]').each(function () {
                    var oldIndex = $(this).data('valmsg-for').match(/\[([0-9]*)\]/) && $(this).data('valmsg-for').match(/\[([0-9]*)\]/).length > 0 ? $(this).data('valmsg-for').match(/\[([0-9]*)\]/)[1] : null;
                    if (oldIndex)
                        $(this).data('valmsg-for', $(this).data('valmsg-for').replace(oldIndex, index, toString()))
                });
            }
            else {
                nonIndex = nonIndex + 1;
            }
        });

        $("form").removeData("validator");
        $("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("form");

    }

    return {
        GetBankStatementList: getBankStatementList,
        MoveItem: moveItem,
        UndoItem: undoItem,
        MatchItem: matchItem,
        RemoveItem: removeItem,
        MatchStatementPopup: matchStatementPopup,
        SelectItem: selectItem,
        SortAndIndexing: sortAndIndexing,
        ViewReconcilePopup:viewReconcilePopup
    }
}());