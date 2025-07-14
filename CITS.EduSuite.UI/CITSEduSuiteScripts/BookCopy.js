

var BookCopies = (function () {

    var getBookCopies = function (Id) {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBookCopiesList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                BookId: function () {
                    return Id;
                }
            },
            colNames: [Resources.RowKey, Resources.SlNo, Resources.ISBN, Resources.PrintYear, Resources.Edition, Resources.Price, Resources.FineAmount, Resources.IsIssued,  Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BookCopySlNo', index: 'BookCopySlNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ISBN', index: 'ISBN', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BookPrintYear', index: 'BookCopySlNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BookEdition', index: 'BookEdition', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BookPrice', index: 'BookPrice', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FineAmount', index: 'FineAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsIssued', index: 'IsIssued', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'RackName', index: 'RackName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BookStatusName', index: 'BookStatusName', editable: true, cellEdit: true, sortable: true, resizable: false },

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

        $("#grid").jqGrid("setLabel", "BookCopyName", "", "thBookCopyName");
    }

    var editPopup = function (_this) {

        var validator = null
        var url = $(_this).attr("data-href");

        $('#myModalContent').load(url, function () {
            $.validator.unobtrusive.parse($('#frmBookCopy'));
            $("#myModal").one('show.bs.modal', function () {

                bindForm(this);

            }).modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');

        });

    }

    var validateControl = function (cntrl) {
        var ErrorMsg = $("<span/>");
        var ErrorCntrl = $(cntrl).next("span").find("span");
        var cssError = $(ErrorCntrl).attr("data-valmsg-for");
        $(ErrorMsg).attr("for", cssError);
        var initialVal = $(cntrl).attr("data-initial-val");
        initialVal = initialVal != undefined ? initialVal : "";
        initialVal = initialVal != undefined ? initialVal : "";
        var required = $(cntrl).attr("data-val");
        var requiredMsg = $(cntrl).attr("data-val-required");
        var regx = $(cntrl).attr("data-val-regex-pattern");
        if (regx != undefined)
            var RegularEmp = new RegExp(regx);
        var regxMsg = $(cntrl).attr("data-val-regex");
        if (required == "true" && requiredMsg != undefined && $(cntrl).val() == initialVal) {
            $(ErrorMsg).html(requiredMsg);
            $(ErrorCntrl).removeClass("field-validation-valid").addClass("field-validation-error")
            $(ErrorCntrl).find("span").remove();
            $(ErrorCntrl).append(ErrorMsg);
            return false;

        }
        else {
            $(ErrorCntrl).removeClass("field-validation-error").addClass("field-validation-valid")
            $(ErrorCntrl).find("span").remove();

        }
        if (regx != undefined && $(cntrl).val() != "" && RegularEmp.exec($(cntrl).val()) == null) {
            $(ErrorMsg).html(regxMsg);
            $(ErrorCntrl).removeClass("field-validation-valid").addClass("field-validation-error")
            $(ErrorCntrl).find("span").remove();
            $(ErrorCntrl).append(ErrorMsg);
            return false;

        }
        else {
            $(ErrorCntrl).removeClass("field-validation-error").addClass("field-validation-valid")
            $(ErrorCntrl).find("span").remove();
        }
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" data-modal=""   data-href="' + $("#hdnAddEditBookCopy").val() + '/' + rowdata.RowKey + '?BookId=' + rowdata.BookKey + '" ><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBookCopy(' + temp + ');return false;" ><i class="fa fa-trash pointer" aria-hidden="true"></i></a></div>';
    }

    return {
        GetBookCopies: getBookCopies,
        EditPopup: editPopup,
        ValidateControl: validateControl
    }

}());

function deleteBookCopy(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_BookCopy,
        actionUrl: $("#hdnDeleteBookCopy").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function bindForm(dialog) {
    $('#frmBookCopy', dialog).submit(function () {
        var validate = $('#frmBookCopy').valid();
        var form = this;
        if (validate) {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.IsSuccessful) {
                        $('#myModal').modal('hide');
                        // location.reload();
                        //alert("success")
                        var url = window.location.pathname.split("/");
                        var controller = url[2];
                        if (controller.toLowerCase() == "librarybooklist") {
                            location.reload();
                        }
                        else {
                            $('#replacetarget').load("/BookCopy/BookCopyList/" + result.BookKey);
                        }

                        //  Load data from the server and place the returned HTML into the matched element
                    } else {
                        $('#myModalContent').html(result);
                        bindForm(dialog);

                    }


                }
            });

        }
        return false;

    });

}

function ValidateForm() {
    var Status = true;
    $("#frmBookCopy input[type=text],#frmBookCopy select").each(function () {
        Status = BookCopies.ValidateControl($(this)[0]);
    })
    Status = Status != undefined ? Status : true;
    return Status;
}