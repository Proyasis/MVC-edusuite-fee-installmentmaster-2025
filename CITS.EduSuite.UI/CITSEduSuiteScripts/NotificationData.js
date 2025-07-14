var NotificationData = (function () {


    var getNotification = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');

        $("#grid").jqGrid({
            url: $("#hdnGetNotification").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                SearchText: function () {
                    return $('#SearchText').val()
                },
                SearchFromDate: function () {
                    var date = $('#txtsearchFromDate').val()
                    return date;
                },
                SearchToDate: function () {
                    var date = $('#txtsearchToDate').val()
                    return date;
                },
                NotificationType: function () {
                    return $("#tab-componets li a.active").data('val')
                }


            },
            colNames: [Resources.RowKey, Resources.NotificationTitle, Resources.NotificationContent, Resources.Date, Resources.CreatedBy],//, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'PushNotificationTitle', index: 'PushNotificationTitle', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'PushNotificationContent', index: 'PushNotificationContent', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CreatedDate', index: 'CreatedDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'AddedByText', index: 'AddedByText', editable: true, cellEdit: true, sortable: true, resizable: false },
                // { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 100 },

            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
            altRows: true,
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
            sortname: 'RowKey',
            sortorder: 'desc',
            altclass: 'jqgrid-altrow',
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
                //},
                //onPaging: function () {
                //    var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //    $("#grid").setGridParam({ datatype: 'json', page: CurrPage });
                //    $("#grid").trigger("reloadGrid");
            }
        })
        //jQuery("#grid").jqGrid('setFrozenColumns');

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.Rowkey + "'";
        return "";
        // return '<div class="divEditDelete"><a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteBatch(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }
    return {
        GetNotification: getNotification
    }
}());