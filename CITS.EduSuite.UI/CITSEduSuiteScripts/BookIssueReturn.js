ajaxRequest = null;
var BookIssueReturn = (function () {

    var getBookIssueReturn = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBookIssueReturnList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                ApplicationTypeKey: function () {
                    return $('#ApplicationTypeKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.CardId, Resources.Name, Resources.IssueDate, Resources.DueDate, Resources.NumberOfBookTaken, Resources.NumberOfBookReturn, Resources.BalanceBooks, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'CardId', index: 'CardId', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MemberName', index: 'MemberName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IssueDate', index: 'IssueDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'DueDate', index: 'DueDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'NumberofBooks', index: 'NumberofBooks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NumberofReturnBooks', index: 'NumberofReturnBooks', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NumberofBalanceBooks', index: 'NumberofReturnBooks', editable: true, cellEdit: true, formatter: formatBalanceBooks, sortable: true, resizable: false },

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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                })

            }
        })

        $("#grid").jqGrid("setLabel", "CardId", "", "thCardId");
    }

    var editPopup = function (_this) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        var url = $(_this).attr("data-href");

        $('#myModalContent').load(url, function () {
            $.validator.unobtrusive.parse($('#frmBookReturn'));
            $("#myModal").one('show.bs.modal', function () {

                bindBooReturnForm(this);

            }).modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');

        });

    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        // return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1"  href="AddEditBookIssue/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBookIssueReturn(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a><a data-modal="" class="btn btn-outline-primary btn-sm mx-1"  data-href="' + $("#hdnAddEditBookReturn").val() + '/' + rowdata.RowKey + '"><i class="fa fa-reply" aria-hidden="true"></i></a></div>';
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1"  href="BookIssueList/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBookIssueReturn(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a><a  class="btn btn-outline-primary btn-sm mx-1"  onclick="BookIssueReturn.BookIssueReturnForm(' + temp + ');return false;"><i class="fa fa-reply" aria-hidden="true"></i></a></div>';



        var temp = "'" + rowdata.RowKey + "'";

        if (rowdata.NumberofBooks != rowdata.NumberofReturnBooks) {
            return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1"  href="BookIssueList/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBookIssueReturn(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a><a  class="btn btn-outline-primary btn-sm mx-1"  onclick="BookIssueReturn.BookIssueReturnForm(' + temp + ');return false;"><i class="fa fa-reply" aria-hidden="true"></i></a></div>';

        }
        else {
            return '<div class="divEditDelete"><a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBookIssueReturn(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';

        }


    }
    function formatBalanceBooks(cellvalue, option, rowdata, action) {
        var string = '';
        if (rowdata.NumberofReturnBooks) {
            string = parseInt((rowdata.NumberofBooks - rowdata.NumberofReturnBooks).toFixed(2)).toString();
        }
        return string;
    }
    var bookIssueReturnForm = function (rowid) {
        var URL = $("#hdnAddEditBookReturn").val() + "?id=" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg mw-100 w-75",
            load: function () {
                $("#frmBookReturn").removeData("validator");
                $("#frmBookReturn").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse($("#frmBookReturn"));
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);
    }

    var getBookByCatagory = function (_this) {
        var item = $(_this).closest("[data-repeater-item]");
        var obj = {};
        obj.BookCategoryKey = $(_this).val() != "" ? $(_this).val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnFillBook").val(), $("select[id*=BookKey]", $(item)), Resources.Book);
    }
    var getBookCopyByBook = function (_this) {
        var item = $(_this).closest("[data-repeater-item]");
        var obj = {};
        obj.BookKey = $(_this).val() != "" ? $(_this).val() : 0;
        obj.RowKey = $("[id*=RowKey]", $(item)).val() != "" ? $("[id*=RowKey]", $(item)).val() : 0;
        AppCommon.BindDropDownbyId(obj, $("#hdnFillBookCopy").val(), $("select[id*=BookCopyKey]", $(item)), Resources.BookCopy);
    }

    var getCardDetails = function () {
        var model = {};
        model.CardId = $("#CardId").val();
        model.Id = $("#RowKey").val();
        //model.ApplicationKey = parseInt(ApplicationKey) ? parseInt(ApplicationKey) : 0;
        if (model.CardId != "" && model.CardId != null) {

            ajaxRequest = $.ajax({
                type: "GET",
                url: $("#hdnAddEditBookIssue").val(),
                contentType: "application/json; charset=utf-8",
                data: model,
                beforeSend: function () {
                    if (ajaxRequest != null) {
                        ajaxRequest.abort();
                    }
                },
                success: function (result) {
                    if (result.IsSuccessful == false) {
                        $("[data-valmsg-for=error_msg]").html(result.Message);

                    }
                    $("#DivIssueDetails").html("")
                    $("#DivIssueDetails").html(result);
                },
                error: function (request, status, error) {

                }
            });
        }
    }


    var getIssueDetails = function (change) {

        var cardId = $("#CardId").val()
        var issueDate = $("#IssueDate").val()
        var response = AjaxHelper.ajax("GET", $("#hdnCheckCardIdExists").val() + "?cardId=" + cardId + "&issueDate=" + issueDate)
        $("#NumberofBooks").val(response["NumberofBooks"]);
        $("#NumberofBooksRemain").val(response["NumberofBooksRemain"]);
        $("#NumberofBooksDay").val(response["NumberofBooksDay"]);
        $("#NumberofBooksDayRemain").val(response["NumberofBooksDayRemain"]);
        jsonData = AjaxHelper.ajax("GET", $("#hdnGetBookIssueReturnByValues").val() + "?cardId=" + cardId + "&issueDate=" + issueDate)
        if (parseInt(jsonData["RowKey"]) != 0) {
            if (change) {
                //window.location.href = $("#hdnAddEditBookReturnIssue").val() + "/" + jsonData["RowKey"];
                $("#RowKey").val(parseInt(jsonData["RowKey"]))
                $("#RowKey", $("#frmAddEditBookIssue")).val(parseInt(jsonData["RowKey"]))
                $("#DivIssueDetails").mLoading()
                //$("#DivIssueDetails").load($("#hdnAddEditBookReturnIssue").val());

                var obj = {};
                obj.Id = $("#RowKey").val();
                obj.CardId = cardId;
                $("#DivIssueDetails").load($("#hdnAddEditBookReturnIssue").val() + "?" + $.param(obj), function () {
                    $("#DivIssueDetails").mLoading("destroy")
                })
            }

        }
        else {

            $("#RowKey").val(parseInt(jsonData["RowKey"]))
            $("#RowKey", $("#frmAddEditBookIssue")).val(parseInt(jsonData["RowKey"]))

            $("[data-repeater-item]").remove()
            $("[data-repeater-create]").trigger("click")
        }
    }

    function formSubmit() {


        var $form = $("#frmAddEditBookIssue")
        var JsonData = [];

        var formData = $form.serializeToJSON({
            associativeArrays: false
        });

        if ($form.valid()) {
            var dataurl = $form.attr("action");
            var response = [];

            response = AjaxHelper.ajax("POST", dataurl,
                {
                    model: formData
                });
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
                                window.location.href = $("#hdnBookReturnIssueList").val();
                                //$("#frmAddEditBookIssue").closest(".modal").modal("hide")
                                //$("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                            }
                        }
                    }
                })

            }

        }
    }

    var fillFine = function (_this) {
        var item = $(_this).closest('[data-repeater-item]');
        var ReturnDate = $("input[id*=ReturnDate]", $(item)).val();
        if (ReturnDate != null && ReturnDate != "") {
            var RowKey = $("input[id*=RowKey]", $(item)).val();
            var obj = {};
            obj.ReturnDate = ReturnDate;
            obj.RowKey = RowKey;
            $.ajax({
                url: $("#hdnAutoFillFine").val(),
                type: "Get",
                dataType: "Json",
                data: obj,
                success: function (result) {
                    $("input[id*=IfAnyFine]", $(item)).val(result);
                }

            })
        }
        else {
            $("input[id*=IfAnyFine]", $(item)).val("");
        }
    }

    return {
        GetBookIssueReturn: getBookIssueReturn,
        EditPopup: editPopup,
        GetBookByCatagory: getBookByCatagory,
        GetBookCopyByBook: getBookCopyByBook,
        BookIssueReturnForm: bookIssueReturnForm,
        GetCardDetails: getCardDetails,
        GetIssueDetails: getIssueDetails,
        FormSubmit: formSubmit,
        FillFine: fillFine
    }

}());

function deleteBookIssueReturn(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_BookIssue,
        actionUrl: $("#hdnDeleteBookIssueReturn").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function deleteBookIssueReturnDetails(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_BookIssue,
        actionUrl: $("#hdnDeleteBookIssueReturnDetails").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");
            if (response.IsSuccessful)
                $(item).remove();
        }
    });
}

function bindBooReturnForm(dialog) {
    $('#frmBookReturn', dialog).submit(function () {
        var validate = $('#frmBookReturn').valid();
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
                        bindBooReturnForm(dialog)
                    }
                }
            });

        }

        return false;

    });
}

var BookIssueReturnDetails = (function () {

    var getBookIssueReturnDetails = function (json) {
        $('.repeater').repeater(
            {
                show: function () {
                    var availCount = $('.repeater [data-repeater-item]').length - 1;
                    availCount = availCount != "" ? parseInt(availCount) : 0;
                    var todayCount = $("#NumberofBooksDay").val()
                    var todayCountRemain = $("#NumberofBooksDayRemain").val()
                    todayCount = todayCount != "" ? parseInt(todayCount) : 0;
                    BookIssueReturn.GetBookCopyByBook(this)
                    if (availCount != todayCount && todayCountRemain != 0) {
                        $(this).slideDown();
                    }
                    else {
                        if (availCount == 0) {

                            $(this).slideDown();
                            return false;
                        }
                        else {
                            var msg = "Today Allowed Book is Over";
                            EduSuite.AlertMessage({ title: Resources.Warning, content: msg, type: 'orange' })
                            $(this).remove();
                            return false;
                        }

                    }

                    //AppCommon.CustomRemoteMethod();
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    var multipleRow = $("#hdnMultiplerowsCount")
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deleteBookIssueReturnDetails($(hidden).val(), $(this));
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
                                        window.location.href = $("#hdnBookReturnIssueList").val();// + "/" + response["RowKey"];
                                    }
                                }
                            }
                        })

                    }

                },
                data: json,
                repeatlist: 'BookIssueReturnDetails',
                submitButton: '',

            });
    }


    return {
        GetBookIssueReturnDetails: getBookIssueReturnDetails
    }

}());





