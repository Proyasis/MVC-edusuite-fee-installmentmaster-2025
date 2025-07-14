var CourseTransfer = (function () {

    var getApplication = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetApplications").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                ApplicantName: function () {
                    return $('#txtSearchApplicantName').val()
                },
                MobileNumber: function () {
                    return $('#txtSearchMobileNumber').val()
                },
                BranchKey: function () {
                    return $('#BranchKey').val()
                },
                BatchKey: function () {
                    return $('#BatchKey').val()
                },
                CourseKey: function () {
                    return $('#CourseKey').val()
                },
                UniversityKey: function () {
                    return $('#UniversityKey').val()
                }

            },
            colNames: [Resources.RowKey, Resources.Branch, Resources.AdmissionNo,
            Resources.Name, Resources.MobileNo, Resources.Course,
                Resources.AcademicTerm, Resources.Batch, "Transfer History", Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentName', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentMobile', index: 'StudentMobile', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: false, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TransferCount', index: 'TransferCount', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseTransferCount },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
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
            loadonce: false,
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
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
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showCourseTranferDetailsGrid,
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

    function showCourseTranferDetailsGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetStudentTotalFeeDetails").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { key: false, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'FromAcademicTermName', index: 'FromAcademicTermName', editable: true },
                { key: false, hidden: true, name: 'ToAcademicTermName', index: 'ToAcademicTermName', editable: true },
                { key: false, hidden: true, name: 'FromCourseName', index: 'FromCourseName', editable: true },
                { key: false, hidden: true, name: 'ToCourseName', index: 'ToCourseName', editable: true },
                { key: false, hidden: true, name: 'FromUniversityName', index: 'FromUniversityName', editable: true },
                { key: false, hidden: true, name: 'ToUniversityName', index: 'ToUniversityName', editable: true },
                { label: "From", name: 'FromAcademicTermName', formatter: formatFromCourseName },
                { label: "To", name: 'ToCourseName', formatter: formatToCourseName },
                { label: Resources.Remarks, name: 'Remarks' },
                { label: Resources.Date, name: 'TransferDate', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            ],
            loadonce: true,
            width: 500,
           
            pager: false,
            footer: false,
            rownumbers: true
        });

    }

    function formatFromCourseName(cellValue, option, rowdata, action) {
        return rowdata.FromCourseName + " " + rowdata.FromUniversityName + " (" + rowdata.FromAcademicTermName + ") ";
    }
    function formatToCourseName(cellValue, option, rowdata, action) {
        return rowdata.ToCourseName + " " + rowdata.ToUniversityName + " (" + rowdata.ToAcademicTermName + ") ";
    }


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";

        return '<div class="divEditDelete"><a  class="btn btn-outline-warning btn-sm ml-3" onClick="CourseTransfer.CourseTransferPopup(' + rowdata.RowKey + ')"><i class="fa fa-share" aria-hidden="true"></i>' + Resources.Transfer + '</a> </div>';



    }

    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }

    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }

    function formatCourseTransferCount(cellValue, options, rowdata, action) {
        if (rowdata.TransferCount > 0) {
            return "Yes (" + rowdata.TransferCount + ")";
        } else {
            return "No";
        }

    }



    var CourseTransferPopup = function (RowKey) {
        var obj = {};
        obj.ApplicationKey = RowKey;
        obj.id = 0;
        var url = $("#hdnAddEditCourseTransfer").val() + "?" + $.param(obj);
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg mw-100 w-75",
            //modalsize: "modal-md",
            load: function () {
                setTimeout(function () {

                }, 500)
            },
            rebind: function (result) {
                //if (result.IsSuccessful) {
                //    $('#myModal').modal('hide');
                //    window.location.reload();
                //}
            }
        }, url);
    }



    function formSubmit() {

        var $form = $("#frmAddEditCourseTransfer")

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
            if (!response.IsSuccessful) {
                $("[data-valmsg-for=error_msg]").html(response.Message);
            }
            else if (response.IsSuccessful) {
                var successmsg = "Course Transfer Successfully";
                $.alert({
                    type: 'green',
                    title: Resources.Success,
                    content: successmsg,
                    icon: 'fa fa-check-circle-o-',
                    buttons: {
                        Ok: {
                            text: Resources.Ok,
                            btnClass: 'btn-success',
                            action: function () {
                                window.location.href = $("#hdnCourseTransferList").val();
                            }
                        }
                    }
                })

            }

        }



    }

    var getCourseByCourseType = function () {


        var obj = {};
        obj.AcademicTermKey = $("#ToAcademicTermKey").val();
        obj.CourseTypeKey = $("#CourseTypeKey").val();
        obj.FromCourseKey = $("#FromCourseKey").val();
        obj.FromAcademicTermKey = $("#FromAcademicTermKey").val();
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseByCourseType").val(), $("[id*=ToCourseKey]"), Resources.Course);
    }
    var getUniversityByCourse = function () {

        var obj = {};
        obj.AcademicTermKey = $("#ToAcademicTermKey").val();
        obj.CourseKey = $("#ToCourseKey").val();

        AppCommon.BindDropDownbyId(obj, $("#hdnGetUniversity").val(), $("[id*=ToUniverisityKey]"), Resources.University);

    }

    var getCourseTypeByAcademicTerm = function () {

        var obj = {};
        obj.AcademicTermKey = $("#ToAcademicTermKey").val();
        //AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseTypeByAcademicTerm").val(), ddl, Resources.CourseType, "CourseTypes");
        AppCommon.BindDropDownbyId(obj, $("#hdnGetCourseTypeByAcademicTerm").val(), $("[id*=CourseTypeKey]"), Resources.CourseType);

    }


    return {
        GetApplication: getApplication,
        CourseTransferPopup: CourseTransferPopup,
        FormSubmit: formSubmit,
        GetCourseByCourseType: getCourseByCourseType,
        GetUniversityByCourse: getUniversityByCourse,
        GetCourseTypeByAcademicTerm: getCourseTypeByAcademicTerm,


    }
}());



function deleteCourseTransfer(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Application,
        actionUrl: $("#hdnDeleteCourseTransfer").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

