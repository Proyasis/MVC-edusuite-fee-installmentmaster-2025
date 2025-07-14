var request = null;
var ApplicationWebForm = (function () {

    var getApplicationWebForm = function () {

        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');

        $("#grid").jqGrid({
            url: $("#hdnGetApplicationWebForm").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                ApplicantName: function () {
                    return $('#txtSearchApplicantName').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                WebFormStatusKey: function () {
                    return $('#WebFormStatusKey').val()
                },
                EnquiryStatusKey: function () {
                    return $('#EnquiryStatusKey').val()
                }
            },
            colNames: [Resources.RowKey, Resources.RowKey, Resources.Branch, Resources.Name, Resources.MobileNo, Resources.Course,
                Resources.Gender, Resources.Status, Resources.EnquiryStatus, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'ConvertedToApplication', index: 'ConvertedToApplication', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicantName', index: 'ApplicantName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, formatter: formatCourseUniversityYear, resizable: false, width: 250 },
                { key: false, name: 'Gender', index: 'Gender', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
                { key: false, name: 'number', index: 'number', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatFormStatus },
                { key: false, name: 'EnquiryStatusName', index: 'EnquiryStatusName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatWebFormEnquiryStatus },

                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 100, align: 'center' },

            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 50, 100, 250, 500, 1000],
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
            multiselect: true,
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

            }
        }).contextMenu({
            selector: ".jqgrow .context-menu",
            trigger: 'left',
            build: function ($trigger, e) {

                var $tr = $(e.target).closest("tr.jqgrow"),
                    rowid = $tr.attr("id"),
                    item = $("#grid").jqGrid("getRowData", rowid);

                var items = {
                    V: { name: "Convert to Application", icon: "fa-arrow-right" },
                    D: { name: Resources.Delete, icon: "fa-trash" }
                }
                var convertedToApplication = item.ConvertedToApplication ? JSON.parse(item.ConvertedToApplication.toLowerCase()) : false;
                if (convertedToApplication) {
                    delete items.V;
                    delete items.D;
                }
                return {
                    callback: function (key, options) {
                        var url = $("#hdnAddEditApplicationPersonal").val();
                        var href = "";
                        switch (key) {
                            case "E":
                                href = "AddEditApplication?ApplicationWebFormKey=" + rowid
                                window.location.href = href;
                                break;
                            case "V":
                                href = $("#hdnAddEditApplicationPersonal").val() + "?ApplicationWebFormKey=" + rowid
                                window.location.href = href;
                                break;
                            case "D":
                                deleteApplicationWebForm(rowid);
                                break;

                            default:
                                href = "";

                        }
                    },
                    items: items
                }

            }
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        if (rowdata.CourseName == null && rowdata.UniversityName == null) {
            Coursetext = "";
        }
        else if (rowdata.UniversityName == null) {
            Coursetext = rowdata.CourseName;
        }
        else if (rowdata.CourseName != null && rowdata.UniversityName != null) {
            Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName;
        }
        return Coursetext;
    }

    function formatGender(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        if (rowdata.Gender == 2) {
            return '<span  ><i class="fa fa-female w3-text-pink" aria-hidden="true"></i> ' + "Female" + '</span>';
        }
        else {
            return '<span  ><i class="fa fa-male w3-text-indigo" aria-hidden="true"></i> ' + "Male" + '</span>';
        }
        return cellValue;
    }
    function formatFormStatus(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        if (rowdata.ConvertedToApplication) {
            return '<span  >' + "Converted To Application" + '</span>';
        }
        else {
            return '<span  > ' + "Pending" + '</span>';
        }
        return cellValue;
    }

    function formatWebFormEnquiryStatus(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        if (rowdata.EnquiryStatusName) {
            return cellValue;
        }
        else {
            return '<span style="color:red" > ' + "Not in Enquiry" + '</span>';
        }
        return cellValue;
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><button type="button" class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</button></div>'
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="AddEditApplication' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i></a>' + '<a class="btn btn-outline-danger btn-sm"  onclick="javascript:deleteApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i></a><a class="btn btn-warning btn-sm" href="' + $("#hdnViewApplication").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';
    }

    var formSubmit = function (btn) {

        var form = $(btn).closest("form")[0];
        var url = $(form)[0].action;
        var validator = $(form).validate();
        var validate = $(form).valid();
        if ($(form).valid()) {
            $(".section-content").mLoading();
            $("[disabled=disabled]", $(form)).removeAttr("disabled");
            var data = $(form).serializeArray();
            delete data[0];
            setTimeout(function () {
                var response = AjaxHelper.ajax("POST", url,
                    {
                        model: AppCommon.ObjectifyForm(data)
                    });

                if (response.IsSuccessful == true) {

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
                                    window.location.href = $("#hdnAddEditApplicationWebForm").val();
                                }
                            }
                        }
                    })

                }
                else {
                    toastr.error(Resources.Failed, response.Message);
                    $("[data-valmsg-for=error_msg_payment]").html(response.Message);
                }
                $(".section-content").mLoading("destroy");
            }, 500)
        }
        else {
            validator.focusInvalid();
        }

    }

    var getCourseByCourseType = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseByCourseType").val(), ddl, Resources.Select + Resources.BlankSpace + Resources.Course, "Courses");
    }
    var getUniversityByCourse = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetUniversityByCourse").val(), ddl, Resources.University, "Universities");
    }
    var getYearByMode = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetYearByMode").val(), ddl, null, "AdmittedYear");
    }
    var getCourseTypeByAcademicTerm = function (obj, ddl) {
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseTypeByAcademicTerm").val(), ddl, Resources.CourseType, "CourseTypes");
    }
    var checkMobileNo = function (_this) {
        var obj = {};
        obj.id = 0;
        obj.RowKey = $("#RowKey").val();
        obj.MobileNumber = $(_this).val();
        var span = $(_this).next("span")
        var validator = $("form").validate({

        });
        request = $.ajax({
            url: $("#hdnCheckPhoneExists").val(),
            type: "GET",
            dataType: "JSON",
            data: obj,
            beforeSend: function () {
                if (request != null) {
                    request.abort();
                }
                validator.element(_this);
            },
            success: function (result) {
                if (!result.IsSuccessful) {
                    span.html('<span class="field-validation-error" data-valmsg-for="MobileNumber" data-valmsg-replace="true">' + result.Message + '</span>')
                }
            }
        });
    }

    return {
        GetApplicationWebForm: getApplicationWebForm,
        FormSubmit: formSubmit,
        GetCourseByCourseType: getCourseByCourseType,
        GetUniversityByCourse: getUniversityByCourse,
        GetCourseTypeByAcademicTerm: getCourseTypeByAcademicTerm,
        CheckMobileNo: checkMobileNo,
    }

}());


function deleteApplicationWebForm(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Application,
        actionUrl: $("#hdnDeleteApplicationWebForm").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}