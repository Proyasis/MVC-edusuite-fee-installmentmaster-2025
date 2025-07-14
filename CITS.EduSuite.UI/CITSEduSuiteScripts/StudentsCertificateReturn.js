var StudentsCertificateReturn = (function () {
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
            colNames: [Resources.RowKey, Resources.BlankSpace, Resources.BlankSpace, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Branch,
                Resources.AdmissionNo, Resources.Name, Resources.MobileNo, Resources.Course,// Resources.AffiliationsTieUps,
                Resources.CurrentYear, Resources.Batch,
                Resources.NoOfCertificate, Resources.NoOfRecived, Resources.NoOfVerified, Resources.NoOfReturned, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'AvailableCertificates', index: 'AvailableCertificates', editable: true },
                { key: false, hidden: true, name: 'RecievePending', index: 'RecievePending', editable: true },
                { key: false, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: false, hidden: true, name: 'CurrentYear', index: 'CurrentYear', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicantName', index: 'ApplicantName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfCertificate', index: 'NoOfCertificate', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AvailableCertificates', index: 'AvailableCertificates', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfVerified', index: 'NoOfVerified', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfReturned', index: 'NoOfReturned', editable: true, cellEdit: true, sortable: true, resizable: false },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250 },
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
            onPaging: function () {
                var CurrPage = $(".ui-pg-input", $("#pager")).val();
                $("#grid").setGridParam({ datatype: 'json', page: CurrPage });
                $("#grid").trigger("reloadGrid");
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 50,
            subGridRowExpanded: showChildGrid,

            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },
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


                var objVerify = {}; var objRecive = {}; var objReturn = {}; var objTempReturn = {};
                objVerify.id = item.RowKey;
                objVerify.ReturnTypeKey = Resources.CertificateVerified;
                objVerify.IsTemporary = false;

                objRecive.id = item.RowKey;
                objRecive.ReturnTypeKey = Resources.CertificateReceived;
                objRecive.IsTemporary = false;

                objReturn.id = item.RowKey;
                objReturn.ReturnTypeKey = Resources.CertificateReturned;
                objReturn.IsTemporary = false;

                objTempReturn.id = item.RowKey;
                objTempReturn.ReturnTypeKey = Resources.CertificateReturned;
                objTempReturn.IsTemporary = true;

                var menus = {};
                if (item.NoOfVerified < item.NoOfCertificate) {
                    menus.V = { name: Resources.Verify, icon: "fa-pencil-square-o" }
                }
                if (item.RecievePending >= 1) {
                    menus.R = { name: Resources.Receive, icon: "fa-pencil-square-o" }
                }
                if (item.AvailableCertificates >= 1) {
                    menus.RT = { name: Resources.Return, icon: "fa-pencil-square-o" }
                }
                //if (item.NoOfVerified >= 1) {
                //    menus.TR = { name: "Temp return", icon: "fa-pencil-square-o" }
                //}
                return {
                    callback: function (key, options) {

                        var href = "";
                        switch (key) {
                            case "V":
                                StudentsCertificateReturn.EditPopup(objVerify)
                                break;
                            case "R":
                                StudentsCertificateReturn.EditPopup(objRecive)
                                break;
                            case "RT":
                                StudentsCertificateReturn.EditPopup(objReturn)
                                break;
                                //case "TR":
                                //    StudentsCertificateReturn.EditPopup(objTempReturn)
                                //    break;

                            default:
                                href = "";

                        }
                    },
                    items: menus

                }
            }

        });



        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

    function showChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');


        $("#" + childGridID).jqGrid({
            url: $("#hdnGetCertificateDetailsByApplication").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { key: false, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'LastKey', index: 'LastKey', editable: true },
                { key: false, hidden: true, name: 'ListCount', index: 'ListCount', editable: true },
                { label: Resources.EducationQualification, name: 'EducationQualificationName', width: 100 },
                { label: Resources.EducationQualificationUniversity, name: 'EducationQualificationUniversity', width: 100 },
                { label: Resources.IssueDate, name: 'IssuedDate', formatter: 'date', formatoptions: { newformat: 'd/m/Y' }, width: 100 },
                 { label: Resources.Status, name: 'CertificateStatusName', width: 100 },
                { label: Resources.Blankspace, name: 'Action', search: false, index: 'RowKey', sortable: false, formatter: editSubGridLink, resizable: false, width: 250 },

            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }
    function editSubGridLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        
        if (rowdata.RowKey == rowdata.LastKey && rowdata.ListCount > 1) {
            return '<div class="divEditDelete"><a class="btn btn-outline-danger btn-sm mx-1" href="#"   onclick="javascript:resetStudentsCertificateReturn(' + temp + ');return false;"><i class="fa fa-trash" aria-hidden="true"></i></a></div>';
        }
        else {
            return "";
        }
    }

    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }
    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }
    function editLink(cellValue, options, rowdata, action) {

        return '<div class="divEditDelete"><button type="button" class="btn btn-outline-primary btn-sm context-menu"><i class="fa fa-bars" />' + Resources.Action + '</button></div>'


        //var objVerify = {}; var objRecive = {}; var objReturn = {}; var objTempReturn = {};
        //objVerify.id = rowdata.RowKey;
        //objVerify.ReturnTypeKey = Resources.CertificateVerified;
        //objVerify.IsTemporary = false;

        //objRecive.id = rowdata.RowKey;
        //objRecive.ReturnTypeKey = Resources.CertificateReceived;
        //objRecive.IsTemporary = false;

        //objReturn.id = rowdata.RowKey;
        //objReturn.ReturnTypeKey = Resources.CertificateReturned;
        //objReturn.IsTemporary = false;

        //objTempReturn.id = rowdata.RowKey;
        //objTempReturn.ReturnTypeKey = Resources.CertificateReturned;
        //objTempReturn.IsTemporary = true;

        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-secondary btn-sm" data-href="' + $("#hdnAddEditStudentsCertificateReturn").val() + '?' + $.param(objVerify) + '"><i class="fa fa-pencil-square-o" aria-hidden="true"></i>' + Resources.Verify + '</a>' + '<a data-modal="" class="btn btn-outline-info btn-sm" data-href="' + $("#hdnAddEditStudentsCertificateReturn").val() + '?' + $.param(objRecive) + '"><i class="fa fa-undo" aria-hidden="true"></i>' + Resources.Receive + '</a>' + '<a data-modal="" class="btn btn-outline-info btn-sm" data-href="' + $("#hdnAddEditStudentsCertificateReturn").val() + '?' + $.param(objReturn) + '"><i class="fa fa-undo" aria-hidden="true"></i>' + Resources.Return + '</a></div>';

        //var html = '<div class="divEditDelete">';
        //if (rowdata.NoOfVerified < rowdata.NoOfCertificate) {
        //    //html = html + "<a type='button' class='btn btn-success btn-sm' onclick='javascript:UpdateQuickAttendance(" + temp + ',' + Resources.AttendanceStatusPresent + ',' + Resources.AttendancePresentStatusCheckIn + ")'><i class='fa fa-sign-in'></i> Present</a>"
        //    html = html + "<a data-modal='' class='btn btn-outline-secondary btn-sm' data-href='" + $('#hdnAddEditStudentsCertificateReturn').val() + '?' + $.param(objVerify) + "'><i class='fa fa-pencil-square-o' aria-hidden='true'></i> " + Resources.Verify + " </a>"

        //}
        //if (rowdata.NoOfRecieved >= 1) {
        //    html = html + "<a data-modal='' class='btn btn-outline-secondary btn-sm' data-href='" + $('#hdnAddEditStudentsCertificateReturn').val() + '?' + $.param(objRecive) + "'><i class='fa fa-pencil-square-o' aria-hidden='true'></i> " + Resources.Receive + " </a>"

        //}
        //if (rowdata.NoOfTempReturned >= 1) {
        //    html = html + "<a data-modal='' class='btn btn-outline-secondary btn-sm' data-href='" + $('#hdnAddEditStudentsCertificateReturn').val() + '?' + $.param(objReturn) + "'><i class='fa fa-pencil-square-o' aria-hidden='true'></i> " + Resources.Return + " </a>"

        //}
        //if (rowdata.NoOfVerified >= 1) {
        //    html = html + "<a data-modal='' class='btn btn-outline-secondary btn-sm' data-href='" + $('#hdnAddEditStudentsCertificateReturn').val() + '?' + $.param(objTempReturn) + "'><i class='fa fa-pencil-square-o' aria-hidden='true'></i> Temp return </a>"

        //}
        //html = html + '</div">';
        //return html

    }
    var editPopup = function (_this) {
        
        var url = $("#hdnAddEditStudentsCertificateReturn").val() + '?' + $.param(_this);
        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {
                setTimeout(function () {
                    StudentCertificateReturnLoad();
                }, 500)
            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    window.location.reload();
                }
            }
        }, url);
    }

    var resetStudentsCertificateReturn = function (rowkey, ApplicationKey, _this) {
        var result = EduSuite.Confirm({
            title: Resources.Confirmation,
            content: Resources.Delete_Confirm_Enquiry,
            actionUrl: $("#hdnResetStudentsCertificateReturn").val(),
            actionValue: rowkey,
            dataRefresh: function (response) {
                var modal = $(_this).closest(".modal");
                if (response.IsSuccessful)
                    $.customPopupform.CustomPopup({
                        modalsize: "modal-md",
                        load: function () {
                            $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                            setTimeout(function () {
                                StudentCertificateReturnLoad();
                            }, 500)

                        },
                        rebind: function (result) {
                            if (result.IsSuccessful) {
                                toastr.success(Resources.Success, result.Message);
                            }
                            else {
                                toastr.error(Resources.Failed, result.Message);
                            }
                            $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                        },

                    },
                    $("#hdnAddEditStudentsCertificateReturn").val() + "/" + ApplicationKey);
                $(modal).modal("hide");

            }
        });
    }

    function checkVerify(chk) {

        $("[data-repeater-item]").each(function (index) {
            
            var Verified = $("input[name*=IsVerified]", $(this)).val();
            var object = $("input[type=checkbox][name*=CertificateStatus]", $(this))[0];
            //var Object1 = $('input[name*=' + name + ']', $(this))[0];
            if (chk.checked) {
                //if (Verified.toLowerCase() != "true") {
                //    //$(object).prop("disabled", true);
                //}
                //else {
                //    $(object).prop("disabled", false);
                //}
                if (object != undefined) {
                    if (Verified.toLowerCase() == "true") {
                        object.checked = chk.checked;
                    }
                    else {
                        object.checked = false;
                    }
                }
            }
            else {
                $(object).prop("disabled", false);
                object.checked = false;
            }


        })

    };

    return {
        GetApplication: getApplication,
        ResetStudentsCertificateReturn: resetStudentsCertificateReturn,
        EditPopup: editPopup,
        CheckVerify: checkVerify
    }
}());

function StudentCertificateReturnLoad() {
    AppCommon.FormatInputCase()
    AppCommon.FormatDateInput();
    Shared.GetProfileDetails('@Url.Action("StudentDetail", "Shared", new { id = (Model.ApplicationKey), AdmissionNo = "" })')

    $("[data-repeater-item]").each(function (Index) {
        //var CheckedList = $("input[type=checkbox][name*=IsOriginalReturn]:checked,input[type=checkbox][name*=IsVerified]:checked");

        //$(CheckedList).each(function () {
        //    $(this).prop("disabled", true);
        //});
        var repeatlist = 'StudentCertificatedetails';
        $(this).find('[name]').each(function () {
            var oldIndex = $(this).attr('name').match(/\[([0-9]*)\]/) && $(this).attr('name').match(/\[([0-9]*)\]/).length > 0 ? $(this).attr('name').match(/\[([0-9]*)\]/)[1] : null;
            if (oldIndex) {
                var newName = $(this).attr('name');
                newName = newName.replace(repeatlist + "[" + oldIndex + "]", repeatlist + "[" + Index + "]")
                $(this).attr('name', newName);
            }
            var span = $($(this).next('span.form-control-error-text')[0]).find('span')[0];
            if (span == undefined) {
                span = $($(this).closest("div.form-col").find('span.form-control-error-text')).find('span')[0];
            }
            var attr = $(span).attr("data-valmsg-for");
            $(this).attr("id", $(this).attr("name"));
            $(span).attr("data-valmsg-for", $(this).attr("name"));
            $(this).parent().find('label').attr("for", $(this).attr("name"));
        });
    });
}

function CheckReturn(_this) {

    var item = $(_this).closest("[data-repeater-item]");

    var PR = $("#IsPermenant").prop("checked");
    if (PR) {
        var Verified = $("input[name*=IsVerified]", $(item)).val();
        if (Verified.toLowerCase() != "true") {
            $(_this).prop('checked', false);
            EduSuite.AlertMessage({ title: Resources.Warning, content: "This certificate can't be permenently return without verify !.Please verify certificate", type: 'orange' })
        }

    }


};


function resetStudentsCertificateReturn(rowkey) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Division,
        actionUrl: $("#hdnResetStudentsCertificateReturn").val(),
        actionValue: rowkey,
        dataRefresh: function () {
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
}