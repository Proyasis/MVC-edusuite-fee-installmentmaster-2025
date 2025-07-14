var AttendanceType = (function () {
    var getAttendanceType = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetAttendanceTypeDetails").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                searchText: function () {
                    return $('#txtsearch').val()
                }
            },
            colNames: [Resources.RowKey, Resources.AttendanceType, Resources.StartTime, Resources.EndTime, //Resources.GraceTime,
            Resources.IsActive, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
               { key: false, name: 'AttendanceTypeName', index: 'AttendanceTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StartTime', index: 'StartTime', editable: true, cellEdit: true, formatter: formatTimeColumn, sortable: true, resizable: false },
                { key: false, name: 'EndTime', index: 'EndTime', editable: true, cellEdit: true, formatter: formatTimeColumn, sortable: true, resizable: false },
                //{ key: false, name: 'GraceTime', index: 'GraceTime', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsActiveText', index: 'IsActiveText', editable: true, cellEdit: true, sortable: true, resizable: false },
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
                });

            }
        })

        $("#grid").jqGrid("setLabel", "UniversityCourseName", "", "thUniversityCourseName");
    }
    
    var addEditAttendanceType = function (id) {

        var URL = $("#hdnAddEditAttendanceType").val() + "?id=" + id;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {
                AppCommon.FormatInputCase();
                AppCommon.FormatSelect2();
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);

        //var obj = {};
        //id = parseInt(id) ? parseInt(id) : 0;
        //var url = $("#hdnAddEditAttendanceType").val() + "/" + id + "?" + $.param(obj);
        //$("#dvAddEditAttendanceType").mLoading();
        //$.ajax({
        //    type: "Get",
        //    url: url,
        //    success: function (response) {
        //        if (response) {
        //            $("#dvAddEditAttendanceType").html(response)
        //            $.validator.unobtrusive.parse("form");
        //            AppCommon.FormatInputCase();
        //            AppCommon.FormatSelect2();
        //            $("#btnCancel", $("#dvAddEditAttendanceType")).on("click", function () {
        //                AttendanceType.GetAttendanceType();
        //                AttendanceType.AddEditAttendanceType(null);
        //                return false;
        //            })

        //        }
        //        else {
        //            $("#dvAddEditAttendanceType").mLoading("destroy");
        //        }
        //    }
        //})

    }

    var formSubmit = function (btn) {

        var $form = $("#frmAttendanceType")
        var JsonData = [];
        var StartTime = ["StartTime"]
        var EndTime = ["EndTime"]
        $("[disabled]", $form).removeAttr("disabled");
        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if ($form.valid()) {

            formData['StartTime'] = (formData['StartTime'] != "" ? moment(formData['StartTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
            formData['EndTime'] = (formData['EndTime'] != "" ? moment(formData['EndTime'].toUpperCase(), ["hh:mm A"]).format("HH:mm") : null);
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
                                window.location.reload();
                            }
                        }
                    }
                })

            }

        }

    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        if (rowdata.RowKey == 1)
        {
            return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm mx-1"  onclick="AttendanceType.AddEditAttendanceType(' + rowdata.RowKey + ')" ><i class="fa fa-pencil" aria-hidden="true"></i></a></div>';
        }
        else
        {
            return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1"  onclick="AttendanceType.AddEditAttendanceType(' + rowdata.RowKey + ')" ><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:deleteAttendanceTypeAll(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
        }

    }

    function formatTimeColumn(cellValue, options, rowdata, action) {

        if (cellValue != null) {
            return AppCommon.FormatObjectToTimeAMPM(cellValue);
        }
        else {
            return "";
        }


    }

    return {
        GetAttendanceType: getAttendanceType,
        FormSubmit: formSubmit,
        AddEditAttendanceType: addEditAttendanceType
    }
}());

function deleteAttendanceTypeAll(RowKey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_UniversityCourse,
        actionUrl: $("#hdnDeleteAttendanceTypeAll").val(),
        actionValue: RowKey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

