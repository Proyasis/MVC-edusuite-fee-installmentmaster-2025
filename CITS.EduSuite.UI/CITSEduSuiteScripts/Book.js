var Book = (function () {
    var getBook = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBook").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.Affiliations + Resources.Sla + Resources.TieUps, Resources.Course + Resources.Name, Resources.AcademicTerm, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'CourseKey', index: 'CourseKey', editable: true },
                { key: true, hidden: true, name: 'UniversityKey', index: 'UniversityKey', editable: true },
                { key: true, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: false, name: 'UniversityName', index: 'UniversityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AcademicTermName', index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
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



        })

        $("#grid").jqGrid("setLabel", "BookName", "", "thBookName");
    }
    var getBookAdvanceDetail = function (json) {

        $('#dynamicRepeater').repeater(
            {
                show: function () {
                    $(this).slideDown();
                    AppCommon.CustomRepeaterRemoteMethod();
                    $("[id*=HasBook][type=checkbox]", $(this)).prop("checked", true);
                    $("[id*=IsActive][type=checkbox]", $(this)).prop("checked", true);
                },
                hide: function (remove) {
                    var self = $(this).closest('[data-repeater-item]').get(0);
                    var hidden = $(self).find('input[type=hidden]')[0];
                    if ($(hidden).val() != "" && $(hidden).val() != "0") {
                        deletBookDetailsItem($(hidden).val(), $(this));
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
                                        window.location.href = $("#hdnBookList").val();
                                    }
                                }
                            }
                        })

                    }

                },
                data: json,
                repeatlist: 'BookDetails',
                submitButton: '#btnSave',
                defaultValues: json
            });
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = rowdata.CourseKey + '' + ',' + '' + rowdata.UniversityKey + '' + ',' + '' + rowdata.AcademicTermKey;
        var obj = {};
        var objDelete = [rowdata.CourseKey, rowdata.UniversityKey, rowdata.AcademicTermKey];
        obj.CourseId = rowdata.CourseKey;
        obj.UniversityId = rowdata.UniversityKey;
        obj.AcademicTermKey = rowdata.AcademicTermKey;
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnAddEditBook").val() + '?' + $.param(obj) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteBook(' + objDelete.join(",") + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }
    return {
        GetBook: getBook,
        GetBookAdvanceDetail: getBookAdvanceDetail
    }
}());

function deleteBook(Coursekey, UniversityKey, AcademicTermKey) {
    var obj = {};
    obj.Coursekey = Coursekey;
    obj.UniversityKey = UniversityKey;
    obj.AcademicTermKey = AcademicTermKey;

    var result = EduSuite.Confirm2({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Book,
        actionUrl: $("#hdnDeleteBook").val(),
        parameters: obj,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}



function deletBookDetailsItem(rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Book,
        actionUrl: $("#hdnDeleteBookItem").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            window.location.reload();
        }
    });
}


