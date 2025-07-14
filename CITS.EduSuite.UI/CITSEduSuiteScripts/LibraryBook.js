
var Books = (function () {

    var getBooks = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBooksList").val(),
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
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Book, Resources.Author, Resources.BookCategory, Resources.BookIssueType, Resources.Language, Resources.Publisher, Resources.NoOfBooks, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'BookName_Optional', index: 'BookName_Optional', editable: true },
                { key: false, name: 'BookName', index: 'BookName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatBookName },
                { key: false, name: 'AuthorName', index: 'AuthorName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BookCategoryName', index: 'BookCategoryName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BookIssueTypeName', index: 'BookIssueTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'LanguageName', index: 'LanguageName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PublisherName', index: 'PublisherName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfBooks', index: 'NoOfBooks', editable: true, cellEdit: true, sortable: true, resizable: false },

                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 300 },
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
            multiselect: true,
            loadonce: false,
            ignoreCase: true,
            altRows: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                })

            }
        })

        $("#grid").jqGrid("setLabel", "BookName", "", "thBookName");
    }

    function formatBookName(cellValue, option, rowdata, action) {

        var returnvalue;
        if (rowdata.BookName_Optional != "" && rowdata.BookName_Optional != null) {

            returnvalue = rowdata.BookName_Optional + " ( " + rowdata.BookName + " ) ";
        }
        else {
            returnvalue = rowdata.BookName
        }
        return returnvalue;
    }


    var getSubRackByRack = function (obj, ddl) {
        $(ddl).html("");
        $(ddl).append($('<option></option>').val("").html(Resources.Rack));
        $.ajax(
            {
                url: $("#hdnFillSubRack").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    $.each(result, function (i, SubRack) {
                        $(ddl).append(
                            $('<option></option>').val(SubRack.RowKey).html(SubRack.Text));
                    });
                }
            });
    }
    var getRackByBranch = function (obj, ddl) {
        $(ddl).html("");
        $(ddl).append($('<option></option>').val("").html(Resources.Rack));
        $.ajax(
            {
                url: $("#hdnFillRack").val(),
                type: "GET",
                dataType: "JSON",
                data: obj,
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    $.each(result, function (i, Rack) {
                        $(ddl).append(
                            $('<option></option>').val(Rack.RowKey).html(Rack.Text));
                    });
                }
            });
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1"  href="AddEditLibraryBook/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a><a class="btn btn-outline-danger btn-sm mx-1" onclick="javascript:deleteBook(' + temp + ');return false;" ><i class="fa fa-trash pointer" aria-hidden="true"></i></a><a class="btn btn-outline-primary btn-sm mx-1" class="bg-success" data-modal=""  data-href="' + $("#hdnAddEditBookCopy").val() + '/0?BookId=' + rowdata.RowKey + '" ><i class="fa fa-copy" aria-hidden="true"></i></a><a class="btn btn-outline-primary btn-sm mx-1" class="bg-success"   onclick="Books.GenerateBookBarcode(' + temp + ')" ><i class="fa fa-barcode" aria-hidden="true"></i></a></div>';

        //var temp = "'" + rowdata.RowKey + "'";
        //var url = "'AddEditLibraryBook/" + rowdata.RowKey + "'";
        //var urlBookCopy = "'" + $("#hdnAddEditBookCopy").val() + '/' + 0 + '?BookId=' + rowdata.RowKey + "'";
        //return '<div class="btn-group"><button onclick="location.href=' + url + '" class="btn btn-primary btn-sm"><i class="fa fa-pencil" aria-hidden="true"></i></button>'
        //    + '<button onclick="javascript:deleteBook(' + temp + ');return false;" class="btn btn-danger btn-sm">  <i class="fa fa-trash" aria-hidden="true"></i> </button><button data-modal="" onclick="BookCopies.EditPopup(' + urlBookCopy + ')" class="btn btn-warning btn-sm"  data-href="' + $("#hdnAddEditBookCopy").val() + '/0?BookId=' + rowdata.RowKey + '" ><i class="fa fa-copy" aria-hidden="true"></i></button><button class="btn btn-success btn-sm" data-modal="" onclick="Books.GenerateBookBarcode(' + temp + ')"  ><i class="fa fa-barcode" aria-hidden="true"></i></button></div>';


    }

    var generateBookBarcode = function (id) {
        var obj = {};
        obj.BookIds = id;
        $.ajax({
            type: "POST",
            url: $("#hdnGetBooksById").val(),
            datatype: "json",
            data: obj,
            success: function (result) {
                var data = {};
                data.rows = result;
                var url = $("#hdnBookBarcodePath").val();
                $.ajax({
                    type: 'GET',
                    crossDomain: true,
                    url: url,
                    success: function (response) {
                        AppCommon.HandleBarHelpers();
                        var template = Handlebars.compile(response);
                        html = template(data);
                        $("").printArea({
                            html: html,
                            load: function (body) {
                                $(".barcode", body).each(function () {
                                    var code = $(this).html();
                                    $(this).barcode(code, "code128", {
                                        bgColor: "#FFFFFF",
                                        color: "#000000",
                                        barWidth: "1",
                                        barHeight: "50",
                                        posx: 100,
                                        posy: 50,
                                    });
                                })

                            },
                            paperSize: "A4"
                        });

                    },
                    error: function (xhr) {

                    },
                    complete: function () {

                    }
                })

            }


        });



    }

    return {
        GetBooks: getBooks,
        GenerateBookBarcode: generateBookBarcode
    }

}());

function deleteBook(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Book,
        actionUrl: $("#hdnDeleteBook").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}