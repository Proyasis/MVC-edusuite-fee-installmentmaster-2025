var modelFileHadoverData = {};
var Employee = (function () {

    var getEmployee = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetEmployeeList").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                searchText: function () {
                    return $('#txtsearch').val()
                },
                branchId: function () {
                    return $('#BranchKey').val()
                },
                statusId: function () {
                    return $('#EmployeeStatusKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Employee + Resources.BlankSpace + Resources.Code, Resources.Employee + Resources.BlankSpace + Resources.Name, Resources.Branch, Resources.Department, Resources.Designation, Resources.Category, Resources.Status, Resources.UserName, Resources.MobileNo, Resources.Action],
            colModel: [
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'EmployeeCode', index: 'EmployeeCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FullName', index: 'FullName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DepartmentName', index: 'DepartmentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DesignationName', index: 'DesignationName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeCategoryName', index: 'EmployeeCategoryName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeStatusName', index: 'EmployeeStatusName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AppUserName', index: 'AppUserName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
                //{ key: false, name: 'Gender', index: 'Gender', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ReleiveDate', index: 'ReleiveDate', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ReligionName', index: 'ReligionName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'DateOfBirth', index: 'DateOfBirth', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'JoiningDate', index: 'JoiningDate', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'NationalityName', index: 'NationalityName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'Photo', index: 'Photo', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'EmergencyContactPerson', index: 'EmergencyContactPerson', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ContactPersonRelationship', index: 'ContactPersonRelationship', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'ContactPersonNumber', index: 'ContactPersonNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'BloodGroupName', index: 'BloodGroupName', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'SalutationName', index: 'SalutationName', editable: true, cellEdit: true, sortable: true, resizable: false },
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
            altclass: 'jqgrid-altrow'
        }).contextMenu({
            selector: ".jqgrow .context-menu",
            trigger: 'left',
            build: function ($trigger, e) {
                // this callback is executed every time the menu is to be shown
                // its results are destroyed every time the menu is hidden
                // e is the original contextmenu event
                var $tr = $(e.target).closest("tr.jqgrow"),
                    rowid = $tr.attr("id"),
                    item = $("#grid").jqGrid("getRowData", rowid);
                rowKey = item["RowKey"];


                var menus = {};


                menus.E = { name: Resources.Edit, icon: "fa-edit" }
                menus.V = { name: Resources.View, icon: "fa-eye" }
                menus.D = { name: Resources.Delete, icon: "fa-trash" }
                menus.P = { name: Resources.Permission, icon: "fa-lock" }
                menus.A = { name: Resources.Account, icon: "fa-cogs" }
                menus.EH = { name: Resources.EmployeeHeirarchy, icon: "fa-cogs" }
                //H: { name: Resources.FileHandOver, icon: "fa-handshake-o" }



                return {
                    callback: function (key, options) {
                        switch (key) {
                            case "E":
                                window.location.href = "AddEditEmployee" + AppCommon.EncodeQueryString("id=" + rowKey);
                                break;
                            case "V":
                                href = $("#hdnViewEmployee").val() + "/" + rowid
                                window.location.href = href
                                break;
                            case "D":
                                deleteEmployee(rowKey);
                                break;
                            case "P":
                                window.location.href = $("#hdnAddEditEmployeeUserPermission").val() + AppCommon.EncodeQueryString("id=" + rowKey)
                                break;
                            case "A":
                                //window.location.href = $("#hdnAddEditEmployeeUserAccount").val() + AppCommon.EncodeQueryString("id=" + rowKey)
                                Employee.EditUserAccount(rowKey);
                                break;
                            case "H":
                                window.location.href = $("#hdnEmployeeFileHandover").val() + AppCommon.EncodeQueryString("id=" + rowKey)
                                break;
                            case "EH":
                                Employee.EditEmployeeHeirarchy(rowKey);
                                break;
                            default:
                                href = "#";

                        }
                    },
                    items: menus
                    //items: {
                    //    E: { name: Resources.Edit, icon: "fa-edit" },
                    //    V: { name: Resources.View, icon: "fa-eye" },
                    //    D: { name: Resources.Delete, icon: "fa-trash" },
                    //    P: { name: Resources.Permission, icon: "fa-lock" },
                    //    A: { name: Resources.Account, icon: "fa-cogs" },
                    //    //H: { name: Resources.FileHandOver, icon: "fa-handshake-o" }

                    //}
                }

            }
        });

        $("#grid").jqGrid("setLabel", "EmployeeName", "", "thEmployeeName");
    }

    var editUserAccount = function (rowid) {
        var URL = $("#hdnAddEditEmployeeUserAccount").val() + "?id=" + rowid;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {
                $("#frmEmployeeUserAccount").removeData("validator");
                $("#frmEmployeeUserAccount").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse($("#frmEmployeeUserAccount"));
            },
            rebind: function (result) {
                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
            }
        }, URL);
    }

    var getDepartmentByBranchId = function (Id) {
        $.ajax({
            url: $("#hdnGetDepartmentByBranchId").val(),
            type: "GET",
            dataType: "JSON",
            data: { id: Id },
            success: function (result) {
                $("#ddlDepartment").html(""); // clear before appending new list 
                $("#ddlDepartment").append($('<option></option>').val("").html(Resources.Select + Resources.BlankSpace + Resources.Department));
                $.each(result.Departments, function (i, Department) {
                    $("#ddlDepartment").append(
                        $('<option></option>').val(Department.RowKey).html(Department.Text));
                });
            }
        });
    }

    var afterFormSubmit = function () {

        $("form").bind('ajax:complete', function () {
            //var target = $("#tab-profile  li a.active").attr("data-href")
            //$(".tab-content").load(target, function (result) {
            //    AppCommon.FormatDateInput();
            //    $("form").removeData("validator");
            //    $("form").removeData("unobtrusiveValidation");
            //    $.validator.unobtrusive.parse("form");
            //});
            $("#tab-profile li a.active").parent().next('li').find('a').trigger('click');

        });
    }
    var reloadData = function (json) {
        var target = $("#tab-profile  li a.active").attr("data-href")
        $(".tab-content").load(target, function (result) {
            AppCommon.FormatDateInput();
            $("form").removeData("validator");
            $("form").removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse("form");
        });
    }
    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</a></div>'
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="AddEditEmployee' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>'
        //    + '<a class="btn btn-outline-danger btn-sm"  onclick="javascript:deleteEmployee(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>'
        //    + '<a class="btn btn-success btn-sm" href="' + $("#hdnAddEditEmployeeUserPermission").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-lock" aria-hidden="true"></i>' + Resources.Permission + '</a></div>'
        //    + '<div class="divEditDelete"><a class="btn btn-warning btn-sm" href="' + $("#hdnAddEditEmployeeUserAccount").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-cogs" aria-hidden="true"></i>' + Resources.Account + '</a></div>';

    }

    //File Handover

    var getEmployeeFileHandover = function (Json) {
        modelFileHadoverData = Json;
        $("#gridHandover").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#gridHandover").jqGrid({
            url: $("#hdnGetEmployeeFileHandoverById").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                EmployeeFromKey: function () {
                    return $("#EmployeeFromKey").val();
                },
                EmployeeToKey: function () {
                    return $("#EmployeeToKey").val();
                },
                FileHandoverTypeKey: function () {
                    return $("#FileHandoverTypeKey").val();
                },
                HandOverType: function () {
                    return $("#tab-filehandover li.active a").attr('data-val');
                },
            },
            colNames: [Resources.BlankSpace, Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.FileType, Resources.Name, Resources.Phone, Resources.Email, Resources.Status, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'GridKey', index: 'GridKey', editable: true, formatter: fileHandOverGridKey },
                { key: false, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'FileKey', index: 'FileKey', editable: true },
                { key: false, hidden: true, name: 'IsActive', index: 'IsActive', editable: true },
                { key: false, hidden: true, name: 'FileHandoverTypeKey', index: 'FileHandoverTypeKey', editable: true },
                { key: false, name: 'FileHandoverTypeName', index: 'FileHandoverTypeName', editable: true },
                { key: false, name: 'FileName', index: 'FileName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FileMobile', index: 'FileMobile', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FileEmail', index: 'FileEmail', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'FileStatus', index: 'FileStatus', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'GridKey', sortable: false, formatter: editFileHandoverLink, resizable: false },
            ],
            pager: jQuery('#pagerHandover'),
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
            loadonce: true,
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow'
        })


        $("#gridHandover").jqGrid("setLabel", "ComponentDate", "", "thComponentDate");
    }
    function editFileHandoverLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        var tempRowId = "'" + options.rowId + "'";
        var html = '<div class="divEditDelete">';
        if (rowdata.RowKey == 0) {
            html = html + '<a class="btn btn-outline-primary btn-sm" onclick="Employee.SingleHandoverClickEvent(' + tempRowId + ')"><i class="fa fa-handshake-o"></i>' + Resources.HandOver + '</a>'
        }
        else if (rowdata.IsActive) {
            html = html + '<a class="btn btn-outline-danger btn-sm" onclick="Employee.SingleHandoverResetClickEvent(' + tempRowId + ')"><i class="fa fa-undo" aria-hidden="true"></i>' + Resources.Reset + '</a>'
        }

        html = html + '</div>';
        return html;
        //'<a class="btn btn-outline-primary btn-sm" href="AddEditEmployee' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>'
        //    + '<a class="btn btn-outline-danger btn-sm"  onclick="javascript:deleteEmployee(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a>'
        //    + '<a class="btn btn-success btn-sm" href="' + $("#hdnAddEditEmployeeUserPermission").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-lock" aria-hidden="true"></i>' + Resources.Permission + '</a></div>'
        //    + '<div class="divEditDelete"><a class="btn btn-warning btn-sm" href="' + $("#hdnAddEditEmployeeUserAccount").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-cogs" aria-hidden="true"></i>' + Resources.Account + '</a></div>';

    }

    function fileHandOverGridKey(cellValue, options, rowdata, action) {
        return rowdata.FileHandoverTypeKey + "-" + rowdata.RowKey
    }

    var singleHandoverClickEvent = function (id) {
        var Data = [], status = true;
        var rowData = $("#gridHandover").getRowData(id);

        if (rowData) {
            Data.push(rowData);
            formSubmitHandOver(Data)
        }
        else {
            EduSuite.AlertMessage({
                title: Resources.Warning,
                content: Resources.AttendanceSheetWarningMessage,
                type: 'orange'
            })
        }
    }

    var multipleHandoverSelectedClickEvent = function () {
        var Data = [], status = true;
        var rowIds = $("#gridHandover").jqGrid("getGridParam", "selarrrow");



        if (rowIds.length > 0) {
            rowIds.forEach(function (item) {
                var rowData = $("#gridHandover").getRowData(item);
                Data.push(rowData);

            })
            formSubmitHandOver(Data)
        }
        else {
            EduSuite.AlertMessage({
                title: Resources.Warning,
                content: Resources.AttendanceSheetWarningMessage,
                type: 'orange'
            });
        }

    }

    var multipleHandoverAllClickEvent = function () {
        $.confirm({
            animation: 'zoom',
            closeAnimation: 'left',
            title: Resources.Confirm,
            content: Resources.HandOverAllSubmitConfirmMessage,
            type: 'orange',
            icon: 'fa fa-exclamation-circle',
            buttons: {
                confirm:
                {
                    text: Resources.Confirm,
                    action: function () {
                        var rowData = $("#gridHandover").jqGrid('getGridParam', 'data');
                        formSubmitHandOver(rowData);

                    }
                },

                cancel:
                {
                    text: Resources.Cancel,
                    action: function () {


                    }
                }
            }
        });
    }

    var singleHandoverResetClickEvent = function (id) {
        var Data = [], status = true;
        var rowData = $("#gridHandover").getRowData(id);

        if (rowData) {
            Data.push(rowData);
            ResetHandover(Data)
        }
        else {
            EduSuite.AlertMessage({
                title: Resources.Warning,
                content: Resources.AttendanceSheetWarningMessage,
                type: 'orange'
            })
        }
    }

    var multipleHandoverResetSelectedClickEvent = function () {
        var Data = [], status = true;
        var rowIds = $("#gridHandover").jqGrid("getGridParam", "selarrrow");



        if (rowIds.length > 0) {
            rowIds.forEach(function (item) {
                var rowData = $("#gridHandover").getRowData(item);
                Data.push(rowData);

            })
            ResetHandover(Data);
        }
        else {
            EduSuite.AlertMessage({
                title: Resources.Warning,
                content: Resources.HandOverAtleastOneWarningMessage,
                type: 'orange'
            });
        }

    }

    var multipleHandoverResetAllClickEvent = function () {
        var rowData = $("#gridHandover").jqGrid('getGridParam', 'data');
        ResetHandover(rowData);
    }

    var hideAndShowHandoverButtons = function (id) {
        var id = $("#tab-filehandover li.active a").attr('data-val');
        if (id == "1") {
            $("#dvHandoverSubmitButtons").show();
            $("#dvHandoverResetButtons").hide();
        } else {
            $("#dvHandoverSubmitButtons").hide();
            $("#dvHandoverResetButtons").show();
        }
    }

    var uploadPhoto = function (btn) {
        var form = $(btn).closest("form")[0];
        url = $(form).attr("action");
        if ($(form).valid()) {
            var canvas = $("#canvas")[0];

            Employee.SubmitPhoto(url, AppCommon.DataURItoBlob(canvas.toDataURL()));

        }

    }

    var submitPhoto = function (url, file) {
        var obj = {};
        obj.PhotoFile = AppCommon.BlobToFile(file, $("#EmployeeKey").val(), file.type);
        obj.EmployeeKey = $("#EmployeeKey").val();
        response = AjaxHelper.ajaxWithFile("model", "POST", url,
            {
                model: obj
            });

        if (response.IsSuccessful == true) {
            toastr.success(Resources.Success, response.Message);
            if (location.search.indexOf('q=') == 0)
                window.location.href = $("#hdnEmployeeList").val();
            else
                Employee.ReloadData();
        }
        else {
            $("[data-valmsg-for=error_msg_payment]").html(response.Message);
        }

    }

    var deleteEmployeePhoto = function (rowkey) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_ApplicationPhoto,
            actionUrl: $("#hdnDeleteEmployeePhoto").val(),
            actionValue: rowkey,
            dataRefresh: function () {
                Employee.ReloadData();
            }
        });
    }

    var editEmployeeHeirarchy = function (id) {
        var url = $("#hdnEmployeeHeirarchy").val() + '?id=' + id;
        $.customPopupform.CustomPopup({
            modalsize: "modal-xs ",
            load: function () {
                AppCommon.FormatDateInput();
                AppCommon.CustomRemoteMethod();
            },
            rebind: function () {

            }
        }, url);
    }
    var formEmployeeHeirarchySubmit = function () {
        
        var $form = $("#frmEmployeeHeirarchy")

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

                                $('[data-dismiss="modal"]').trigger('click');
                                $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                            }
                        }
                    }
                })
            }
        }

    }


    return {
        GetEmployee: getEmployee,
        GetDepartmentByBranchId: getDepartmentByBranchId,
        AfterFormSubmit: afterFormSubmit,
        ReloadData: reloadData,
        GetEmployeeFileHandover: getEmployeeFileHandover,
        SingleHandoverClickEvent: singleHandoverClickEvent,
        MultipleHandoverAllClickEvent: multipleHandoverAllClickEvent,
        MultipleHandoverSelectedClickEvent: multipleHandoverSelectedClickEvent,
        SingleHandoverResetClickEvent: singleHandoverResetClickEvent,
        MultipleHandoverResetAllClickEvent: multipleHandoverResetAllClickEvent,
        MultipleHandoverResetSelectedClickEvent: multipleHandoverResetSelectedClickEvent,
        HideAndShowHandoverButtons: hideAndShowHandoverButtons,
        UploadPhoto: uploadPhoto,
        SubmitPhoto: submitPhoto,
        DeleteEmployeePhoto: deleteEmployeePhoto,
        EditUserAccount: editUserAccount,
        EditEmployeeHeirarchy: editEmployeeHeirarchy,
        FormEmployeeHeirarchySubmit: formEmployeeHeirarchySubmit
    }

}());

function deleteEmployee(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Employee,
        actionUrl: $("#hdnDeleteEmployee").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function formSubmitHandOver(data) {
    var $form = $("#frmHandOover");
    if ($form.valid()) {
        $(".section-content").mLoading();
        setTimeout(function () {
            $form = $("form");
            var result = ModifyMultipleHandoverModel(data);

            var dataurl = $($form)[0].action;
            var response = AjaxHelper.ajax("POST", dataurl,
                {
                    modelList: result
                });
            if (response.IsSuccessful == true) {
                Employee.GetEmployeeFileHandover(modelFileHadoverData);
            }

            $(".section-content").mLoading("destroy");

        }, 1000);

    }
}

function ModifyMultipleHandoverModel(data) {
    var returnData = [];
    for (var i = 0; i < data.length; i++) {
        var modelItem = $.extend(true, {}, modelFileHadoverData);
        var dataItem = data[i];
        for (var key in dataItem) {
            modelItem[key] = dataItem[key];
        }
        modelItem["EmployeeFromKey"] = $("#EmployeeFromKey").val();
        modelItem["EmployeeToKey"] = $("#EmployeeToKey").val();
        returnData.push(modelItem);

    }
    return returnData;
}

function ResetHandover(Data) {
    $.confirm({
        animation: 'zoom',
        closeAnimation: 'left',
        title: Resources.Confirm,
        content: Resources.HandOverResetConfirmMessage,
        type: 'orange',
        icon: 'fa fa-exclamation-circle',
        buttons: {
            confirm:
            {
                text: Resources.Confirm,
                action: function () {
                    var keys = [];
                    $(Data).each(function () {
                        if (this["IsActive"])
                            keys.push(this["RowKey"]);
                    })
                    var response = AjaxHelper.ajax("POST", $("#hdnResetHandover").val(),
                        {
                            keyList: keys
                        });
                    if (response.IsSuccessful == true) {
                        Employee.GetEmployeeFileHandover(modelFileHadoverData);
                    }
                }
            },

            cancel:
            {
                text: Resources.Cancel,
                action: function () {


                }
            }
        }
    });

}
