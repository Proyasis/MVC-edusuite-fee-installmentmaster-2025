var NotificationTemplate = (function () {

    var getNotificationTemplates = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetNotificationTemplates").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.NotificationTemplate, Resources.SMSStatus, Resources.EmailStatus, Resources.SMSTemplate, Resources.EmailTemplate],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: false },
                { key: false, name: 'NotificationTemplateName', index: 'NotificationTemplateName', editable: false, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AutoSMSText', index: 'AutoSMSText', sortable: true, align: 'center', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AutoEmailText', index: 'AutoEmailText', sortable: true, align: 'center', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'editSMS', search: false, index: 'editSMS', sortable: false, formatter: editSMSLink, resizable: false, width: 50 },
                { name: 'editEmail', search: false, index: 'editEmail', sortable: false, formatter: editEmailLink, resizable: false, width: 50 }],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 30, 100],
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
            edit: true,
            cellEdit: true,
            cellsubmit: 'remote',
            editurl: 'NotificationTemplate/UpdateNotificationStatus', //$("#hdnUpdateNotificationStatus").val(),
            altclass: 'jqgrid-altrow'
        })
        $("#grid").jqGrid('AutoSMS', rowid, succesfunc, url, extraparam, aftersavefunc, errorfunc, afterrestorefunc);
        $("#grid").jqGrid("setLabel", "LocationName", "", "thLocationName");
    }

    function editSMSLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnAddEditSMSNotification").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a></div>';
    }

    function editEmailLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" href="' + $("#hdnAddEditEmailNotification").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a></div>';
    }

    function CreateNotificationStatusDropdown(list) {
        $(list).find("option").each(function () {
            var code = this.value;
            var option = $(this);
            if (code == "null") {
                $(option).css({ "background": "#efefef", "color": "black" });
            }
            else if (code == "false") {
                $(option).css({ "background": "red", "color": "white" });
            }
            else if (code == "true") {
                $(option).css({ "background": "green", "color": "white" });
            }
        });
    }

    function CreateNotificationStatusList(list, key, value) {
        var newList = {};
        $.each(list, function () {
            if (this[key] == 0)
                newList[null] = this[value];
            else if (this[key] == 1)
                newList[false] = this[value];
            else if (this[key] == 2)
                newList[true] = this[value];
        });
        return newList;
    }

    return {
        GetNotificationTemplates: getNotificationTemplates
    }
}());


