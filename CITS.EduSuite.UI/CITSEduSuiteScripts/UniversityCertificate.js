var UniversityCertificate = (function () {
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
            colNames: [Resources.RowKey, Resources.Blankspace, Resources.Blankspace, Resources.Blankspace, Resources.Branch, Resources.AdmissionNo,
            Resources.Name, Resources.MobileNo, Resources.Course,
            Resources.AcademicTerm, Resources.Batch,
            Resources.NoOfCertificate, Resources.NoOfRecived, Resources.NoOfIssued, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, hidden: true, name: 'AcademicTermKey', index: 'AcademicTermKey', editable: true },
                { key: false, hidden: true, name: 'CurrentYear', index: 'CurrentYear', editable: true },
                { key: false, hidden: true, name: 'CourseDuration', index: 'CourseDuration', editable: true },
                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'ApplicantName', index: 'ApplicantName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'MobileNumber', index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'CourseName', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatCourseUniversityYear, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true, formatter: formatCurrentYear, sortable: true, resizable: false },
                { key: false, name: 'BatchName', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfCertificate', index: 'NoOfCertificate', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfRecieved', index: 'NoOfRecieved', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'NoOfVerified', index: 'NoOfVerified', editable: true, cellEdit: true, sortable: true, resizable: false },
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
            loadonce: false,
            ignoreCase: true,
            sortname: 'RowKey',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            loadComplete: function (data) {
                $("#grid a[data-modal='']").each(function () {
                    AppCommon.EditGridPopup($(this))
                });

            },
            gridComplete: function () {
                $(".jqgrow:odd").hover(
                    function () { $(this).removeClass("oddRow"); },
                    function (event) { $(this).addClass("oddRow"); }
                );
            }
            //,
            //onPaging: function () {
            //    var CurrPage = $(".ui-pg-input", $("#pager")).val();
            //    $("#grid").setGridParam({ datatype: 'json', page: CurrPage });
            //    $("#grid").trigger("reloadGrid");
            //}
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }

    var getUniversityCertificateDetails = function (json) {

        $('.repeater').repeater({
            show: function () {
                $(this).slideDown();
                //$("[id*=OriginalIssuedDate]", $(this)).val(moment(new Date()).format("DD/MM/YYYY"))
                AppCommon.FormatDateInput();
                AppCommon.CustomRepeaterRemoteMethod();
                AppCommon.FormatInputCase();
                $("input[type=text]", $(this)).each(function () { $(this).val("") })
                $("[href]", $(this)).hide();
            },
            hide: function (remove) {
                var self = $(this).closest('[data-repeater-item]').get(0);
                var hidden = $(self).find('input[type=hidden]')[0];
                if ($(hidden).val() != "" && $(hidden).val() != "0") {
                    resetUniversityCertificate($(hidden).val(), $(this));
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
                    $("#frmUniversityCertificate").closest(".modal").modal("hide")
                    $("#grid").jqGrid("setGridParam", { datatype: 'json' }).trigger("reloadGrid");
                }

            },
            data: json,
            repeatlist: 'UniversityCertificateDetails',
            submitButton: '#btnSave',
            defaultValues: json,
            hasFile: true
        });
    }

    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        //return '<div class="divEditDelete"><a data-modal="" class="btn btn-outline-primary btn-sm" data-href="' + $("#hdnAddEditUniversityCertificate").val() + '/' + rowdata.RowKey + '"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + Resources.AddCertificate + '</a></div>';

        return '<div class="divEditDelete"><a  class="btn btn-outline-primary btn-sm" onclick="UniversityCertificate.EditPopup(' + rowdata.RowKey + ')"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + Resources.AddCertificate + '</a></div>';

    }

    var editPopup = function (Id) {


        var obj = {};
        obj.Id = Id;
        //var url = $("#hdnAddEditUniversityCertificate").val() + '?' + $.param(obj);
        url = $("#hdnAddEditUniversityCertificate").val() + "/" + Id;
        var validator = null

        $.customPopupform.CustomPopup({
            modalsize: "modal-lg",
            load: function () {

            },
            rebind: function (result) {
                if (result.IsSuccessful) {
                    $('#myModal').modal('hide');
                    $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                }
            }
        }, url);

    }

    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
        var Coursetext = rowdata.CourseName + " - " + rowdata.UniversityName //+ " - " + yeartext
        return Coursetext;
    }
    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }



    return {
        GetApplication: getApplication,
        GetUniversityCertificateDetails: getUniversityCertificateDetails,
        EditPopup: editPopup

    }
}());

var resetUniversityCertificate = function (rowkey, _this) {
    var result = EduSuite.Confirm({
        title: Resources.Confirmation,
        content: Resources.Delete_Confirm_Enquiry,
        actionUrl: $("#hdnResetUniversityCertificate").val(),
        actionValue: rowkey,
        dataRefresh: function (response) {
            var item = $(_this).closest("[data-repeater-item]");
            if (response.IsSuccessful)
                $(item).remove();
            $("#grid").jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
        }
    });
};

