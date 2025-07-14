var JsonModel = [], request = null;
var GvWidth;
var tableStudentSummary;
var LibraryReport = (function () {




    // Book Summery Start

    var getLibraryBookSummary = function (json) {

        JsonModel = json;
        //var newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
        //$.extend(newPostData, JsonData)
        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'BookName', headerText: Resources.Book + Resources.BlankSpace + Resources.Name, index: 'BookName', editable: true, cellEdit: true, sortable: true, resizable: false },
            //{ key: false, name: 'BookCode', headerText: Resources.Name, index: 'BookCode', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AuthorName', headerText: Resources.Author, index: 'AuthorName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BookCategoryName', headerText: Resources.BookCategory, index: 'BookCategoryName  ', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BookIssueTypeName', headerText: Resources.BookIssueType, index: 'BookIssueTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'LanguageName', headerText: Resources.Language, index: 'LanguageName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PublisherName', headerText: Resources.Publisher, index: 'PublisherName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'RackName', headerText: Resources.Wardrobe, index: 'RackName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SubRackName', headerText: Resources.WardrobeRack, index: 'SubRackName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateAdded', index: 'DateAdded', headerText: Resources.Date, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'TotalCopy', index: 'TotalCopy', headerText: Resources.TotalCopy, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'IssuedCount', index: 'IssuedCount', headerText: Resources.TotalIssued, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'NotIssuedCount', index: 'NotIssuedCount', headerText: Resources.NotIssued, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CreatedBy', index: 'CreatedBy', headerText: Resources.CreatedBy, editable: true, cellEdit: true, sortable: true, resizable: false },
            //{ key: false, name: 'DamageCount', index: 'DamageCount', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Activetext', index: 'Activetext', headerText: Resources.IsActive, editable: true, cellEdit: true, sortable: true, resizable: false },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetLibraryBookSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [10, 15, 50, 100],
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            footerrow: true,
            userDataOnFooter: true,
            loadComplete: function (data) {

                LibraryReport.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showBookCopyDetails,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

  

    function showBookCopyDetails(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)
        
        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');


        $("#" + childGridID).jqGrid({
            url: $("#hdnGetBookCopies").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { key: false, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { label: Resources.BookCopySlNo, name: 'BookCopySlNo', index: 'BookCopySlNo' },
                { label: Resources.ISBN, name: 'ISBN', index: 'ISBN' },
                { label: Resources.BookBarCode, name: 'BookBarCode', index: 'BookBarCode' },
                { label: Resources.BookEdition, name: 'BookEdition', index: 'BookEdition' },
                { label: Resources.BookPrintYear, name: 'BookPrintYear', index: 'BookPrintYear' },
                { label: Resources.NoOfPages, name: 'NoOfPages', index: 'NoOfPages' },
                { label: Resources.BookPrice, name: 'BookPrice', index: 'BookPrice', formatter: RupeeIcon },
                { label: Resources.FineAmount, name: 'FineAmount', index: 'FineAmount', formatter: RupeeIcon },
                { label: Resources.BookStatus, name: 'BookStatusName', index: 'BookStatusName', formatter: CellBookStatus },
                { label: Resources.IssueStatus, name: 'IssueStatus', index: 'IssueStatus', formatter: CellIssueStatus }


            ],
            autoResizing: { minColWidth: 80 },
            autowidth: true,
            shrinkToFit: true,
            loadonce: true,
            width: 1000,
            height: '100%',
            pager: false,
            footer: false
        });

    }



    function CellIssueStatus(cellValue, option, rowdata, action) {
        switch (rowdata.IssueStatus) {
            case "Available":
                return '<span class="badge badge-success">' + cellValue + '</span>';
                break;
            default:
                return '<span class="badge badge-danger">' + cellValue + '</span>';
                break;
        }


    }

    function CellBookStatus(cellValue, option, rowdata, action) {
        switch (rowdata.BookStatusName) {
            case "Book Active":
                return '<span class="badge badge-success">' + cellValue + '</span>';
                break;
            default:
                return '<span class="badge badge-danger">' + cellValue + '</span>';
                break;
        }

    }

    // Book Summery End

    // Member Plan Summary Start

    var getMemberPlanSummary = function (json) {

        JsonModel = json;
        //var newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
        //$.extend(newPostData, JsonData)
        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'MemberName', headerText: Resources.Name, index: 'MemberName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CardId', headerText: Resources.CardId, index: 'CardId', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ApplicationTypeName', headerText: Resources.ApplicationType, index: 'ApplicationTypeName  ', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MemberTypeName', headerText: Resources.MemberType, index: 'MemberTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BorrowerTypeName', headerText: Resources.BorrowerType, index: 'BorrowerTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MobileNo', headerText: Resources.MobileNumber, index: 'MobileNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EmailAddress', headerText: Resources.EmailAddress, index: 'EmailAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', headerText: Resources.Gender, index: 'Gender', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateAdded', index: 'DateAdded', headerText: Resources.Date, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Descreption', index: 'Descreption', headerText: Resources.Descreption, editable: true, cellEdit: true, sortable: true, resizable: false, width: 300 },
            { key: false, name: 'CreatedBy', index: 'CreatedBy', headerText: Resources.CreatedBy, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'NumberOfBooksAllowed', index: 'NumberOfBooksAllowed', headerText: Resources.AllowedBooks, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'TotalIssued', index: 'TotalIssued', headerText: Resources.TotalIssued, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BalanceReturn', index: 'BalanceReturn', headerText: Resources.BalanceReturned, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Activetext', index: 'Activetext', headerText: Resources.IsActive, editable: true, cellEdit: true, sortable: true, resizable: false },
            { name: 'edit', search: false, index: 'RowKey', headerText: Resources.Action, sortable: false, formatter: editLink, resizable: false },
        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetMemberPlanSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [10, 15, 50, 100],
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            footerrow: true,
            userDataOnFooter: true,
            loadComplete: function (data) {

                LibraryReport.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: false, // set the subGrid property to true to show expand buttons for each row


        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

    function editLink(cellValue, options, rowdata, action) {

        return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm mx-1"  onclick="LibraryReport.GetMemberDetails(' + rowdata.RowKey + ');return false;"><i class="fa fa-eye" aria-hidden="true"></i></a></div>';
    }

    var getMemberDetails = function (RowKey) {
        var URL = $("#hdnMemberDetails").val() + "?RowKey=" + RowKey;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg  mw-100 w-75",
            load: function () {

            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);
    }

    // Member Plan Summary End



    // Book Issue Summary Start

    var getBookIssueSummary = function (json) {

        JsonModel = json;
        //var newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
       // $.extend(newPostData, JsonData)
        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'BookCopySlNo', headerText: Resources.Book + Resources.BlankSpace + Resources.Name, index: 'BookCopySlNo', editable: true, cellEdit: true, sortable: true, resizable: false },        
            { key: false, name: 'BookName', headerText: Resources.Book + Resources.BlankSpace + Resources.Name, index: 'BookName', editable: true, cellEdit: true, sortable: true, resizable: false },

            { key: false, name: 'MemberName', headerText: Resources.IssuedTo, index: 'MemberName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CardId', headerText: Resources.CardId, index: 'CardId', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ApplicationTypeName', headerText: Resources.ApplicationType, index: 'ApplicationTypeName  ', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MemberTypeName', headerText: Resources.MemberType, index: 'MemberTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BorrowerTypeName', headerText: Resources.BorrowerType, index: 'BorrowerTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MobileNo', headerText: Resources.MobileNumber, index: 'MobileNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EmailAddress', headerText: Resources.EmailAddress, index: 'EmailAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', headerText: Resources.Gender, index: 'Gender', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateAdded', index: 'DateAdded', headerText: Resources.Date, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Descreption', index: 'Descreption', headerText: Resources.Descreption, editable: true, cellEdit: true, sortable: true, resizable: false, width: 300 },           
            { key: false, name: 'AuthorName', headerText: Resources.Author, index: 'AuthorName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BookCategoryName', headerText: Resources.BookCategory, index: 'BookCategoryName  ', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BookIssueTypeName', headerText: Resources.BookIssueType, index: 'BookIssueTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'LanguageName', headerText: Resources.Language, index: 'LanguageName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PublisherName', headerText: Resources.Publisher, index: 'PublisherName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'RackName', headerText: Resources.Wardrobe, index: 'RackName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SubRackName', headerText: Resources.WardrobeRack, index: 'SubRackName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'IssuedDate', index: 'IssuedDate', headerText: Resources.IssueDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'DueDate', index: 'DueDate', headerText: Resources.DueDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ReturnDate', index: 'ReturnDate', headerText: Resources.ReturnDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'BookStatusName', headerText: Resources.BookStatus, index: 'BookStatusName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'FineAmount', headerText: Resources.FineAmount, index: 'FineAmount', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Remarks', headerText: Resources.Status, index: 'Remarks', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CreatedBy', index: 'CreatedBy', headerText: Resources.IssuedBy, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Activetext', index: 'Activetext', headerText: Resources.IsActive, editable: true, cellEdit: true, sortable: true, resizable: false },
          
        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetBookIssueSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [10, 15, 50, 100],
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            footerrow: true,
            userDataOnFooter: true,
            loadComplete: function (data) {

                LibraryReport.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: false, // set the subGrid property to true to show expand buttons for each row


        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

   
    // Book Issue Summary End


    // Common queries Start



    var getExportPrintData = function (type, url, filename, title) {

        var obj = {};
        obj.ajaxData = $("form").serializeToJSON({

        });

        obj.ajaxData.sidx = "RowKey";
        obj.ajaxData.sord = "asc"
        obj.ajaxData.page = 0;
        obj.ajaxData.rows = 0
        obj.ajaxType = "POST";
        obj.ajaxUrl = $(url).val();
        obj.ContainerId = "#grid";
        obj.FileName = filename;
        obj.Title = title;
        AppCommon.ExportPrintAjax(obj, type)


    }

    var getPrintData = function () {

        Columns = []
        $('#Columns option').each(function (index, value) {
            if (this.selected == true) {
                Columns.push(this.value);
            }
        });


    


  
        JsonModel["DateAdded"] = $("#DateAdded").val();
        JsonModel["rows"] = $(".ui-pg-selbox").val();
        JsonModel["page"] = $(".ui-pg-input").val();
        JsonModel["sidx"] = $("#grid").jqGrid('getGridParam', 'sortname');
        JsonModel["sord"] = $("#grid").jqGrid('getGridParam', 'sortorder');

        $.ajax({
            url: $("#hdnGetExportStudentsSummaryReport").val(),
            dataType: "JSON",
            type: "POST",
            data: JSON.stringify(JsonModel),
            async: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                AppCommon.JsonToPrint(result, Columns);
            }
        });



    }

    function formatYesorNO(cellValue, option, rowdata, action) {

        if (cellValue == "Yes") {
            return '<i  class="fa fa-check" aria-hidden="true"></i>';
        }
        else {
            return '<i  class="fa fa-times" aria-hidden="true"></i>';
        }
        return cellValue;
    }

    function RupeeIcon(cellValue, option, rowdata, action) {
        return '<i  class="fa fa-inr" aria-hidden="true"></i>' + cellValue;
    }

    var getCustomizedColumns = function (obj) {
        var removeCols = ["subgrid", "edit"]
        var colList = $("#grid").jqGrid('getGridParam', 'colModel');
        colList = $(colList).filter(function (n, item) {
            return removeCols.indexOf(item.name) === -1;
        }).map(function (n, item) {
            return item.name;
        });
        var SelectedList = $('#ShowHideColumns').selectpicker('val');
        var CountSelection = SelectedList.length;


        $("#grid").hideCol($(colList).not(SelectedList));
        $("#grid").showCol(SelectedList);


        if (CountSelection <= 6) {
            jQuery("#grid").setGridWidth("1000");
        }
        else {
            jQuery("#grid").setGridWidth(CountSelection * 150);
        }

    }

    function TextHilightwithbadge(cellValue, options, rowdata, action) {

        if (cellValue != 0) {

            return "<span class='badge badge-pill badge-success'>" + cellValue + "</span>"
        }
        else {
            return cellValue
        }


    }

    // Common queries End



    return {

        GetCustomizedColumns: getCustomizedColumns,
        GetExportPrintData: getExportPrintData,
        GetPrintData: getPrintData,
        GetLibraryBookSummary: getLibraryBookSummary,
        GetMemberPlanSummary: getMemberPlanSummary,
        GetMemberDetails: getMemberDetails,
        GetBookIssueSummary: getBookIssueSummary
    }

}());



function GenerateShowHideColumnList(data, DefaultColumns) {

    data = data.filter(function (n, p) {
        return !n.hidden
    })
    var ddl = $("#ShowHideColumns");
    $(ddl).html("")
    $(ddl).val('default').selectpicker("refresh");
    $.each(data, function (i, item) {
        $(ddl).append(
            $('<option ' + (DefaultColumns.indexOf(item.name) > -1 ? "selected=true" : "") + '></option>').val(item.name).html(item.headerText));
    });
    $(ddl).selectpicker("refresh");
}
