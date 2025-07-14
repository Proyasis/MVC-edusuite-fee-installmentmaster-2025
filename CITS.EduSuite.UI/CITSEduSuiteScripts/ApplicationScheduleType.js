var ApplicationScheduleType = (function () {
    var getApplicationScheduleType = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetApplicationScheduleType").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.ScheduleType, Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'IsSystem', index: 'IsSystem', editable: true },
                { key: false, name: 'ScheduleTypeName', index: 'ScheduleTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActive', index: 'IsActive', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
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
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                })

            }
        })

        $("#grid").jqGrid("setLabel", "ApplicationScheduleTypeName", "", "thApplicationScheduleTypeName");
    }

    var editPopup = function (_this) {

        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        var validator = null
        var url = $(_this).attr("data-href");

        $('#myModalContent').load(url, function () {
            $.validator.unobtrusive.parse($('#frmApplicationScheduleType'));
            $("#myModal").one('show.bs.modal', function () {

                bindLocationForm(this);

            }).modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');

        });

    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        if (rowdata.IsSystem)
            return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditApplicationScheduleType").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a></div>';
        else
            return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm mx-1" data-href="' + $("#hdnAddEditApplicationScheduleType").val() + '/' + rowdata.RowKey + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteApplicationScheduleType(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';

    }
    return {
        GetApplicationScheduleType: getApplicationScheduleType,
        EditPopup: editPopup
    }
}());

function deleteApplicationScheduleType(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_ApplicationScheduleType,
        actionUrl: $("#hdnDeleteApplicationScheduleType").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function bindLocationForm(dialog) {
    $('#frmApplicationScheduleType', dialog).submit(function () {
        var validate = $('#frmApplicationScheduleType').valid();
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
                        bindLocationForm(dialog)
                    }
                }
            });

        }

        return false;

    });
}




