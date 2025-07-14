var JsonModel = [], request = null;
var GvWidth;
var tableActivityLogSummary;
var ActivityLogSummary = (function () {

   
    var getActivityLogSummary = function (json) {



        JsonModel = json;
        var newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
        $.extend(newPostData, JsonData)
        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: newPostData
        }).trigger('reloadGrid');

        var colModelList = [


 { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'ActivityDate', index: 'ActivityDate', headerText: Resources.ActivityDate, editable: true, cellEdit: true, sortable: true, resizable: false,  },
 { key: false, name: 'Status', headerText: Resources.Status, index: 'Status', editable: true, cellEdit: true, sortable: true, resizable: false },
 { key: false, name: 'HostName', headerText: Resources.HostName, index: 'HostName', editable: true, cellEdit: true, sortable: true, resizable: false },
 { key: false, name: 'UserID', headerText: Resources.UserID, index: 'UserID', editable: true, cellEdit: true, sortable: true, resizable: false },
 { key: false, name: 'MenuName', headerText: Resources.Menu, index: 'MenuName', editable: true, cellEdit: true, sortable: true, resizable: false },
 { key: false, name: 'MenuAction', headerText: Resources.MenuAction, index: 'MenuAction', editable: true, cellEdit: true, sortable: true, resizable: false },
 { key: false, name: 'ActionDone', headerText: Resources.ActionDone, index: 'ActionDone', editable: true, cellEdit: true, sortable: true, resizable: false },


        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetActivityLogReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: newPostData,
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
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            loadComplete: function (data) {
             
                ActivityLogSummary.GetCustomizedColumns();


            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "ActivityLog", "", "thActivityLog");
        //$("#grid").jqGrid("showHideColumnMenu");



    }
   
    var getExportPrintData = function (type, url, filename, title, beforeProcessing) {

        var sortColumnName = $("#grid").jqGrid('getGridParam', 'sortname');
        var sortOrder = $("#grid").jqGrid('getGridParam', 'sortorder'); // 'desc' or 'asc'
        var rows = $("#grid").getRowData().length;
        //var row = $("#grid").jqGrid('getGridParam', 'records'); // All Records
        var page = $('#grid').getGridParam('page');

        var obj = {};
        obj.ajaxData = $("form").serializeToJSON({

        });

        obj.ajaxData.sidx = sortColumnName;
        obj.ajaxData.sord = sortOrder;
        obj.ajaxData.page = page;
        obj.ajaxData.rows = rows;
        obj.ajaxType = "POST";
        obj.ajaxUrl = $(url).val();
        obj.ContainerId = "#grid";
        obj.FileName = filename;
        obj.beforeProcessing = beforeProcessing ?? false;
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

        JsonModel["RowKey"] = $("#RowKey").val();
        JsonModel["ActivityDate"] = $("#ActivityDate").val();
        
        JsonModel["Status"] = $("#Status").val();
        JsonModel["HostName"] = $("#HostName").val();    
        JsonModel["UserID"] = $("#UserID").val();
        JsonModel["MenuName"] = $("#MenuName").val();
        JsonModel["MenuAction"] = $("#MenuAction").val();
        JsonModel["ActionDone"] = $("#ActionDone").val();
        JsonModel["rows"] = $(".ui-pg-selbox").val();
        JsonModel["page"] = $(".ui-pg-input").val();
        JsonModel["sidx"] = $("#grid").jqGrid('getGridParam', 'sortname');
        JsonModel["sord"] = $("#grid").jqGrid('getGridParam', 'sortorder');

        $.ajax({
            url: $("#hdnGetExportActivityLogSummaryReport").val(),
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


    function RupeeIcon(cellValue, option, rowdata, action) {
        return '<i  class="fa fa-inr" aria-hidden="true"></i>' + cellValue;
    }

    function formatTimeColumn(cellValue, options, rowdata, action) {
        // var columnName = options.colModel["name"];
        //var string = options.gid + "_" + columnName;
        //setTimeout(function () {
        //    if (columnName == "InDateTime") {
        //        $("td[aria-describedby=" + string + "]").addClass("text-green")
        //    }
        //    else {
        //        $("td[aria-describedby=" + string + "]").addClass("text-red")
        //    }
        //}, 100)
        if (cellValue != null) {
            //return moment(cellValue).format("hh:mm A");
            return moment(cellValue).format("hh:mm A");


        }
        //else {

        //    if ((columnName == "InDateTime" && rowdata.ClockInStatus == false)) {
        //        setTimeout(function () { $("td[aria-describedby=" + string + "]", $("tr[id=" + options.rowId + "]")).attr("data-digital-clock-date", "") }, 100);
        //    }
        //    else if ((columnName == "OutDateTime" && rowdata.ClockInStatus == true)) {
        //        setTimeout(function () { $("td[aria-describedby=" + string + "]", $("tr[id=" + options.rowId + "]")).attr("data-digital-clock-date", "") }, 100);
        //    }
        //    return "";
        //}


    }



    // Common queries Start
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

    // Common queries End

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm" data-href="' + $("#hdnAddEditBranch").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteBranch(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }

    return {
        GetActivityLogSummary: getActivityLogSummary,
        GetCustomizedColumns: getCustomizedColumns,
        GetExportPrintData: getExportPrintData,
        GetPrintData: getPrintData,
       
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