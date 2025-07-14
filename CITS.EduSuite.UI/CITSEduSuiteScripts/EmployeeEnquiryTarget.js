var EmployeeEnquiryTarget = (function () {

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
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                EmployeeStatusKey: function () {
                    return $('#EmployeeStatusKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Employee + Resources.BlankSpace + Resources.Code,
            Resources.Employee + Resources.BlankSpace + Resources.Name, Resources.Branch, Resources.Department,
                Resources.Designation, Resources.Status, Resources.MobileNo, Resources.CommonTarget, Resources.AllowMonthlyTarget, Resources.IsActive, Resources.Action],
            colModel: [
                { key: false, hidden: true, name: 'BranchKey', index: 'BranchKey', editable: true },
                { key: true, hidden: true, name: 'EmployeeKey', index: 'EmployeeKey', editable: true },
                { key: false, name: 'EmployeeCode', index: 'EmployeeCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DepartmentName', index: 'DepartmentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'DesignationName', index: 'DesignationName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'EmployeeStatusName', index: 'EmployeeStatusName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CommonTarget', index: 'CommonTarget', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AllowMonthlyTarget', index: 'AllowMonthlyTarget', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatYesorNO },
                { key: false, name: 'IsActive', index: 'IsActive', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatYesorNO },
                { name: 'edit', search: false, index: 'EmployeeKey', sortable: false, formatter: editLink, resizable: false },
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
            sortname: 'EmployeeKey',
            sortorder: 'desc',
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
                EmployeeKey = item["EmployeeKey"];


                var menus = {};

                menus.E = { name: Resources.SetEnquiryTarget, icon: "fa-edit" }
                //menus.D = { name: Resources.Delete, icon: "fa-trash" }

                return {
                    callback: function (key, options) {
                        switch (key) {
                            case "E":
                                EmployeeEnquiryTarget.EditEmployeeEnquiryTarget(EmployeeKey);
                                break;

                            case "D":
                                deleteEmployee(rowKey);
                                break;
                            default:
                                href = "#";

                        }
                    },
                    items: menus
                }

            }
        });

        $("#grid").jqGrid("setLabel", "EmployeeName", "", "thEmployeeName");
    }

    function formatYesorNO(cellValue, option, rowdata, action) {

        if (cellValue == true) {
            return '<i  class="fa fa-check" aria-hidden="true"></i>';
        }
        else {
            return '<i  class="fa fa-times" aria-hidden="true"></i>';
        }
        return cellValue;
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</a></div>'

    }

    var editEmployeeEnquiryTarget = function (id) {
        var url = $("#hdnAddEditEmployeeEnquiryTarget").val() + '?id=' + id;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {

                EmployeeEnquiryTarget.GetEnquiryTargetDetails()
                EmployeeEnquiryTarget.CheckMonthlyTarget($("#AllowMonthlyTarget")[0]);
            },
            rebind: function () {

            }
        }, url);
    }

    var checkMonthlyTarget = function (_this) {
        if (_this.checked) {
            $("#dynamicRepeater").show();
        } else {
            $("#dynamicRepeater").hide();

        }
    }

    var getEnquiryTargetDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();

                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                $("[id*=RowKey]", $(this)).val(0)
                $("[id*=EnquiryTargetKey]", $(this)).val(0)
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    deleteEnquiryTargetDetails($(hidden).val(), $(this));
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
                    toastr.success(Resources.Success, response.Message);

                    $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                }

            },
            data: json,
            repeatlist: 'EmployeeEnquiryTargetDetailsViewModel',
            submitButton: '',
        });
    }


    function formSubmit() {


        var $form = $("#frmAddEditEmployeeEnquiryTarget")

        var formData = $form.serializeToJSON({
            associativeArrays: false
        });
        if (!formData.AllowMonthlyTarget)
            formData.EmployeeEnquiryTargetDetailsViewModel = [];

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
                                toastr.success(Resources.Success, response.Message);
                                $("#frmAddEditEmployeeEnquiryTarget").closest(".modal").modal("hide")
                                $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");


                            }
                        }
                    }
                })

            }

        }
    }

    function monthOrYearChanged(ddlMonth) {

        var item = $(ddlMonth).closest("[data-repeater-item]")
        var MonthValue = $(ddlMonth).val();
        var SelectDate = new Date(MonthValue + "-01");

        var currentMonth = SelectDate.getMonth() + 1;
        var currentYear = SelectDate.getFullYear();

        $("input[id*=TargetYear]", item).val(currentYear)
        $("input[id*=TargetMonth]", item).val(currentMonth)
    }

    return {
        GetEmployee: getEmployee,
        EditEmployeeEnquiryTarget: editEmployeeEnquiryTarget,
        CheckMonthlyTarget: checkMonthlyTarget,
        GetEnquiryTargetDetails: getEnquiryTargetDetails,
        FormSubmit: formSubmit,
        MonthOrYearChanged: monthOrYearChanged
    }

}());

var deleteEnquiryTargetDetails = function (rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Enquiry,
        actionUrl: $("#hdnDeleteEnquiryTargetDetails").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");
            if (response.IsSuccessful)
                $(item).remove();

        }
    });
};