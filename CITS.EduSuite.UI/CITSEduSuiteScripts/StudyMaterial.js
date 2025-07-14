var StudyMaterial = (function () {

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
            colNames: [Resources.RowKey, Resources.Branch, Resources.AdmissionNo, Resources.Name, Resources.MobileNo, Resources.Course, Resources.CurrentYear,
            Resources.Batch, Resources.TotalStudyMaterial, Resources.AvailableStudyMaterial, Resources.IssuedStudyMaterial, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentName', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudentMobile', index: 'StudentMobile', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'TotalBooks', index: 'TotalBooks', editable: true, cellEdit: true, sortable: true, resizable: false, width: 200, formatter: SetColorA },
                { key: false, name: 'AvailableBooks', index: 'AvailableBooks', editable: true, cellEdit: true, sortable: true, resizable: false, width: 200, formatter: SetColorB },
                { key: false, name: 'IssuedBooks', index: 'IssuedBooks', editable: true, cellEdit: true, sortable: true, resizable: false, width: 200, formatter: SetColorC },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 200 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 50, 100, 250, 500, 1000],
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
            }
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }


    var getBookDetails = function () {
        $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#grid").jqGrid({
            url: $("#hdnGetBookDetails").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {

                SubjectYear: function () {
                    return $('#SubjectYear').val()
                },
                StudyMaterialName: function () {
                    return $('#txtSearchBookName').val()
                },
                ApplicationKey: function () {
                    return $('#ApplicationKey').val()
                },

            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.BlankSpace, Resources.StudyMaterialCode, Resources.StudyMaterialName, Resources.AcademicTerm, Resources.Status, Resources.Action],

            colModel: [
                { key: false, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'StudyMaterialKey', index: 'StudyMaterialKey', editable: true },
                { key: false, hidden: true, name: 'SubjectYear', index: 'SubjectYear', editable: true },
                { key: false, hidden: true, name: 'IssuedDate', index: 'IssuedDate', editable: true },
                { key: false, hidden: true, name: 'IssuedByText', index: 'IssuedByText', editable: true },
                { key: false, name: 'StudyMaterialCode', index: 'StudyMaterialCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudyMaterialName', index: 'StudyMaterialName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SubjectYearText', index: 'SubjectYearText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'IsIssued', index: 'IsIssued', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: GetBookStatus },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: deleteLink, resizable: false, width: 250 },
            ],
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [5, 10, 15, 20],
            hidegrid: false,
            height: '100%',
            viewrecords: true,
            rownumbers: true,
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
            altRows: true,
            altclass: 'jqgrid-altrow',
            beforeSelectRow: function (rowid, e) {
                var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", grid);
                if (cbsdis.length === 0) {
                    return true;    // allow select the row
                } else {
                    return false;   // not allow select the row
                }
            },
            onSelectAll: function (aRowids, status) {
                if (status) {
                    // uncheck "protected" rows
                    var cbs = $("tr.jqgrow > td > input.cbox:disabled", grid[0]);
                    cbs.removeAttr("checked");
                }
            },
            loadComplete: function (data) {
                for (i = 0, count = data.rows.length; i < count; i += 1) {
                    if (data.rows[i].IsIssued) {
                        $("#grid").jqGrid('setSelection', data.rows[i].StudyMaterialKey, false);
                        $("input#jqg_grid_" + data.rows[i].StudyMaterialKey).prop("disabled", true)
                    }
                }
            },

        });
    }

    var getAllBooks = function () {
        $("#gridBooks").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        $("#gridBooks").jqGrid({
            url: $("#hdnGetAllBooks").val(),
            datatype: 'json',
            mtype: 'Get',
            postData: {
                SubjectYear: function () {
                    return $('#SubjectYear', $("#frmBindAllBooks")).val()
                },
                StudyMaterialName: function () {
                    return $('#txtSearchBookName').val()
                },
                ApplicationKey: function () {
                    return $('#ApplicationKey').val()
                },

            },
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.StudyMaterialCode, Resources.AcademicTerm, Resources.StudyMaterialName],
            colModel: [
                { key: false, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: true, hidden: true, name: 'StudyMaterialKey', index: 'StudyMaterialKey', editable: true },
                { key: false, hidden: true, name: 'SubjectYear', index: 'SubjectYear', editable: true },
                { key: false, name: 'StudyMaterialCode', index: 'StudyMaterialCode', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'SubjectYearText', index: 'SubjectYearText', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'StudyMaterialName', index: 'StudyMaterialName', editable: true, cellEdit: true, sortable: true, resizable: false },

            ],
            pgbuttons: false,
            viewrecords: false,
            pgtext: "",
            pginput: false,
            scroll: true,
            hidegrid: false,
            height: 300,
            viewrecords: true,
            rownumbers: true,
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
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            },
        });
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnAddEditStudyMaterial").val() + '/' + rowdata.RowKey + '"><i class="fa fa-book" aria-hidden="true"></i>' + Resources.Issue + '</a></div>';
    }

    function deleteLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-danger btn-sm" href="#"   onclick="javascript:deleteStudyMaterial(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
    }
    function formatCourseUniversityYear(cellValue, options, rowdata, action) {

        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }
    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }

    function GetBookStatus(cellValue, options, rowdata, action) {
        var html = "";
        if (rowdata.IsIssued == true) {
            var issueddate = moment(rowdata.IssuedDate).format('DD MMMM  YYYY');
            return html = '<span class="label label-danger">' + "Issued By " + rowdata.IssuedByText + " On " + issueddate + '</span>';
        }
        else if (rowdata.IsIssued == false && rowdata.IsAvailable == true) {
            return html = '<span class="label label-success">' + "Available" + '</span>';
        }
        else {
            return html = '<span class="label label-warning">' + "Not Available" + '</span>';
        }
    }
    function SetColorA(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        return '<span  class="badge badge-success">' + cellValue + '</span>';
    }
    function SetColorB(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        return '<span  class="badge badge-info">' + cellValue + '</span>';
    }
    function SetColorC(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        return '<span  class="badge badge-danger">' + cellValue + '</span>';
    }

    var viewAllBooks = function (_this) {

        var id = $("#ApplicationKey").val();
        //var url = $("#hdnAddEditStudentsCertificateReturn").val() + '?' + $.param(_this);
        var url = $("#hdnBookList").val() + '/' + id;
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {
                setTimeout(function () {
                    viewAllStudyMaterialLoad();
                }, 500)
            },
            submit: function () {

                var s = jQuery("#gridBooks").jqGrid('getGridParam', 'selarrrow');
                if (s.length) {
                    $("#StudyMaterialKeys", $("#frmBindAllBooks")).val(s)

                    var form = $("#frmBindAllBooks");
                    var data = $(form).serializeToJSON({
                        associativeArrays: true
                    });
                    AjaxHelper.ajaxAsync("POST", $("#hdnBookList").val(), data, function () {
                        response = this;
                        if (response.IsSuccessful) {
                            toastr.success(response.Message, Resources.Success);
                            $('.modal').modal('hide');
                            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                        } else {
                            toastr.error(response.Message, Resources.Failed);
                        }
                    })
                }
                else {
                    var msg = "Please select atleast 1 Study Material"
                    EduSuite.AlertMessage({ title: Resources.Warning, content: msg, type: 'orange' })
                }

            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#frmBindAllBooks').modal('hide');
                    $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                }
            }
        }, url);
    }
    return {
        GetApplication: getApplication,
        GetBookDetails: getBookDetails,
        GetAllBooks: getAllBooks,
        ViewAllBooks: viewAllBooks
    }

}());
function deleteStudyMaterial(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_StudyMaterial,
        actionUrl: $("#hdnDeleteStudyMaterial").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}

function viewAllStudyMaterialLoad() {
    StudyMaterial.GetAllBooks();
    AppCommon.FormatInputCase()

    $("#txtSearchBookName", $("#frmBindAllBooks")).on("input", function () {
        StudyMaterial.GetAllBooks();
    });

    $("#iconSpan").click(function () {
        $('#txtSearchBookName').val('');
        StudyMaterial.GetAllBooks();
    });

    $("#SubjectYear", $("#frmBindAllBooks")).on("change", function () {
        StudyMaterial.GetAllBooks();
    });


    //$("#btnSave", $("#frmBindAllBooks")).on("click", function () {

    //    var s = jQuery("#gridBooks").jqGrid('getGridParam', 'selarrrow');

    //        $("#StudyMaterialKeys", $("#frmBindAllBooks")).val(s)


    //})
}





