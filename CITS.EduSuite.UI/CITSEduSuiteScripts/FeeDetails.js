var FeeDetails = (function () {

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
            colNames: [Resources.RowKey, Resources.SlNo, Resources.Branch, Resources.AdmissionNo, Resources.Name, Resources.MobileNo, Resources.Course, Resources.CurrentYear, Resources.Batch,
                //Resources.IsTax,
                Resources.IsInstallment,
            //Resources.IsConsession,
            Resources.TotalFee, Resources.TotalPaid, Resources.BalanceDue, Resources.Action],
            colModel: [
                { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { key: false, name: 'RowNumber', index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },

                { key: false, name: 'BranchName', index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Name', index: 'Name', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Mobile', index: 'Mobile', editable: true, cellEdit: true, sortable: true, resizable: false },
                { key: false, name: 'Course', index: 'Course', editable: true, cellEdit: true, formatter: formatCourseUniversityYear, sortable: true, resizable: false, width: 250 },
                { key: false, name: 'CurrentYearText', index: 'CurrentYearText', editable: true, cellEdit: true,  sortable: true, resizable: false },
                { key: false, name: 'Batch', index: 'Batch', editable: true, cellEdit: true, sortable: true, resizable: false },
                //{ key: false, name: 'IsTax', index: 'IsTax', editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false, align: 'center' },
                { key: false, name: 'HasInstallment', index: 'HasInstallment', editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false, align: 'center' },
                //{ key: false, name: 'IsConsessionText', index: 'IsConsessionText', editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
                { key: false, name: 'TotalFee', index: 'TotalFee', editable: true, cellEdit: true, sortable: true, resizable: false, align: 'right', formatter: 'currencyFmatter' },
                { key: false, name: 'TotalPaid', index: 'TotalPaid', editable: true, cellEdit: true, formatter: formatColor, sortable: true, resizable: false, align: 'right'  },
                { key: false, name: 'BalanceFee', index: 'BalanceFee', editable: true, cellEdit: true,  sortable: false, resizable: false, align: 'right' },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false, width: 250, align: 'center' },
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


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm" href="' + $("#hdnApplicationFeePaymentList").val() + '/' + rowdata.RowKey + '"><i class="fa fa-plus-circle" aria-hidden="true"></i>' + "Add Fee" + '</a></div>';
    }
    function formatBalanceFee(cellValue, options, rowdata, action) {
        var string;
        //string = parseFloat(((rowdata.TotalFee - rowdata.OldPaid) - rowdata.TotalPaid).toFixed(2)).toString();
        string = parseFloat(((rowdata.StudentTotalFee - rowdata.OldPaid) - rowdata.TotalPaid).toFixed(2));
        string = AppCommon.FormatCurrency(string);
        return string;
    }
   
    function formatColor(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;

        var string;
        //string = parseFloat((rowdata.TotalPaid + rowdata.OldPaid).toFixed(2)).toString();
        string = parseFloat((rowdata.TotalPaid + rowdata.OldPaid).toFixed(2));
        string = AppCommon.FormatCurrency(string);
        
        if (rowdata.TotalPaid == rowdata.StudentTotalFee) {
            return '<span  class="label label-success">' + string + '</span>';
        }
        else {
            return '<span  class="label label-danger">' + string + '</span>';
        }
        return cellValue;
    }
    function formatCurrentYear(cellValue, options, rowdata, action) {

        return yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
    }
    function formatCourseUniversityYear(cellValue, options, rowdata, action) {
       // var yeartext = AppCommon.GetYearDescriptionByCodeDetails(rowdata.CourseDuration, rowdata.CurrentYear, rowdata.AcademicTermKey)
        var Coursetext = rowdata.Course + " - " + rowdata.Affiliations //+ " - " + yeartext
        return Coursetext;
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

    function formatNumberWithCommas(cellValue, option, rowdata, action) {        
        var amount = parseFloat(cellValue) ? parseFloat(cellValue) : 0;
        amount = amount.toLocaleString();
        return amount;
    }


    return {
        GetApplication: getApplication

    }

}());


function ContentLoad() {
    AppCommon.FormatInputCase();
    AppCommon.FormatDateInput();
    $("form").removeData("validator");
    $("form").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("form");
    $("form").on("submit", function () {
        $(".section-content").mLoading();
    }).bind('ajax:complete', function () {
        $(".section-content").mLoading("destroy");

    });
}
function EnableTabs() {
    $("#tab-application li").each(function () {
        $(this).removeClass("disabled");
    });
}
function DisableTabs() {
    $("#tab-application li").each(function () {
        $(this).addClass("disabled");
    });
}





