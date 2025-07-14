var JsonModel = [], request = null;
var GvWidth;
var tableStudentSummary;
var griddatacount = [10, 50, 100, 1000, 2500, 5000];

var StudentSummary = (function () {



    // Student Summery Start


    var getStudentSummary = function (json) {

        JsonModel = json;

        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentMobile', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentEmail', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ApplicationNo', index: 'ApplicationNo', headerText: Resources.ApplicationNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CasteName', index: 'CasteName', headerText: Resources.Caste, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CommunityType', index: 'CommunityType', headerText: Resources.CommunityType, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BloodGroup', index: 'BloodGroup', headerText: Resources.BloodGroup, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CatagoryName', index: 'CatagoryName', headerText: Resources.RegistrationCatagory, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'Gender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, name: 'IsTaxText', index: 'IsTaxText', headerText: Resources.IsTax, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'IsInstallmentText', index: 'IsInstallmentText', headerText: Resources.IsInstallment, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'IsConsessionText', index: 'IsConsessionText', headerText: Resources.IsConsession, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'TotalFee', index: 'StudentTotalFee', headerText: Resources.TotalFee, editable: true, cellEdit: true, sortable: true, resizable: false },

        ];

        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudentsSummaryReports").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,

            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            //rownumbers: true,
            //rownumWidth:50,
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }
        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "rn", "Sl.No");

    }

    var getExportPrintData = function (type, url, filename, title, beforeProcessing) {

        var sortColumnName = $("#grid").jqGrid('getGridParam', 'sortname');
        var sortOrder = $("#grid").jqGrid('getGridParam', 'sortorder'); // 'desc' or 'asc'
        var rows = $("#grid").getRowData().length;
        //var row = $("#grid").jqGrid('getGridParam', 'records'); // All Records
        var page = $('#grid').getGridParam('page');

        var obj = {};
        obj.ajaxData = $("form").serializeToJSON({

        });

        obj.ajaxData.sidx = sortColumnName;
        obj.ajaxData.sord = sortOrder;
        obj.ajaxData.page = page;
        obj.ajaxData.rows = rows;
        obj.ajaxType = "POST";
        obj.ajaxUrl = $(url).val();
        obj.ContainerId = "#grid";
        obj.FileName = filename;
        obj.beforeProcessing = beforeProcessing ?? false;
        obj.Title = title;
        AppCommon.ExportPrintAjax(obj, type)

    }

    var getPrintData = function () {

        Columns = []
        $('#Columns option').each(function (index, value) {
            if (this.selected == true) {
                Columns.push(this.value);
            }
        });


        var CourseKeyList = "";


        $("#CourseKeyMultiSelect").each(function () {
            var selected = $("option:selected", this).map(function () {
                CourseKeyList = CourseKeyList + $(this).val() + ",0";
            })
        });


        JsonModel["AcademicTermKey"] = $("#AcademicTermKey").val();
        JsonModel["CourseTypeKey"] = $("#CourseTypeKey").val();
        JsonModel["CourseKeyList"] = CourseKeyList;
        JsonModel["UniversityMasterKey"] = $("#UniversityMasterKey").val();
        JsonModel["BatchKey"] = $("#BatchKey").val();
        JsonModel["SecondLanguageKey"] = $("#SecondLanguageKey").val();
        JsonModel["ReligionKey"] = $("#ReligionKey").val();
        JsonModel["IncomeKey"] = $("#IncomeKey").val();
        JsonModel["ModeKey"] = $("#ModeKey").val();
        JsonModel["ClassModeKey"] = $("#ClassModeKey").val();
        JsonModel["NatureOfEnquiryKey"] = $("#NatureOfEnquiryKey").val();
        JsonModel["BranchKey"] = $("#BranchKey").val();
        JsonModel["AgentKey"] = $("#AgentKey").val();
        JsonModel["StudentStatusKey"] = $("#StudentStatusKey").val();
        JsonModel["Mediumkey"] = $("#Mediumkey").val();
        JsonModel["ClassKey"] = $("#ClassKey").val();
        JsonModel["DateAdded"] = $("#DateAdded").val();
        JsonModel["rows"] = $(".ui-pg-selbox").val();
        JsonModel["page"] = $(".ui-pg-input").val();
        JsonModel["sidx"] = $("#grid").jqGrid('getGridParam', 'sortname');
        JsonModel["sord"] = $("#grid").jqGrid('getGridParam', 'sortorder');

        $.ajax({
            url: $("#hdnGetExportStudentsSummaryReport").val(),
            dataType: "JSON",
            type: "POST",
            data: JSON.stringify(JsonModel),
            async: false,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                AppCommon.JsonToPrint(result, Columns);
            }
        });



    }

    // Student Summery End


    // Fee Summery Start

    var getStudentFeeSummary = function (json) {

        JsonModel = json;


        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentMobile', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentEmail', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ApplicationNo', index: 'ApplicationNo', headerText: Resources.ApplicationNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CasteName', index: 'CasteName', headerText: Resources.Caste, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CommunityType', index: 'CommunityType', headerText: Resources.CommunityType, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BloodGroup', index: 'BloodGroup', headerText: Resources.BloodGroup, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CatagoryName', index: 'CatagoryName', headerText: Resources.RegistrationCatagory, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'Gender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, name: 'IsTaxText', index: 'IsTaxText', headerText: Resources.IsTax, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'IsInstallmentText', index: 'IsInstallmentText', headerText: Resources.IsInstallment, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'IsConsessionText', index: 'IsConsessionText', headerText: Resources.IsConsession, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'TotalFee', index: 'StudentTotalFee', headerText: Resources.TotalFee, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'TotalPaid', index: 'TotalPaid', headerText: Resources.TotalPaid, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'BalanceFee', index: 'BalanceAmount', headerText: Resources.BalanceAmount, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
        ];

        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });

        $("#grid").jqGrid({

            url: $("#hdnGetStudentsFeeSummaryReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            footerrow: true,
            userDataOnFooter: true,
            rownumbers: true,
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showFeeChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

    function showFeeChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetStudentTotalFeeFeeDetails").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: Resources.Year, name: 'FeeYearText', index: 'FeeYearText' },
                { label: Resources.TotalFee, name: 'TotalAmount', formatter: RupeeIcon },
                { label: Resources.TotalPaid, name: 'FeeAmount', formatter: RupeeIcon },
                { label: Resources.BalanceAmount, name: 'BalanceAmount', formatter: RupeeIcon },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }


    // Fee Summery End

    var getDayToDayFees = (function () {

        $("#DivDayToDayFees").mLoading();
        var JsonData = $("form").serializeToJSON({

        });
        JsonData.page = 1
        JsonData.rows = 999,
            JsonData.sord = "desc",
            JsonData.sidx = "RowKey"
        $.ajax({
            type: "POST",
            url: $("#hdnGetDayToDayFees").val(),
            data: JsonData,
            success: function (result) {
                $("#DivDayToDayFees").html("")
                $("#DivDayToDayFees").html(result);
                $("#DivDayToDayFees").mLoading("destroy");
            }


        })
    })


    // StudyMaterial Issue Summery Start

    var getStudyMaterialIssueSummary = function (json) {

        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentEmail', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentMobile', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, name: 'TotalBooks', index: 'TotalBooks', headerText: Resources.TotalBooks, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AvailableBookCount', index: 'AvailableBookCount', headerText: Resources.AvailableBooks, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'IssuedBookCount', index: 'IssuedBookCount', headerText: Resources.IssuedBooks, editable: true, cellEdit: true, sortable: true, resizable: false },
            //{ key: false, name: 'ChalanAmount', index: 'ChalanAmount', headerText: Resources.ChalanAmount, editable: true, cellEdit: true, sortable: true, resizable: false },
            //{ key: false, name: 'ChalanDate', index: 'ChalanDate', headerText: Resources.ChalanDate, editable: true, cellEdit: true, sortable: true, resizable: false },
            //{ key: false, name: 'ChalanNo', index: 'ChalanNo', headerText: Resources.ChalanNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            //{ key: false, name: 'BankName', index: 'BankName', headerText: Resources.Bank, editable: true, cellEdit: true, sortable: true, resizable: false },
        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudyMaterialIssueReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showStudyMaterialChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }
    function showStudyMaterialChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetStudyMaterialById").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { key: false, hidden: true, name: 'IsIssued', index: 'IsIssued', editable: true },
                { key: false, hidden: true, name: 'IsAvailable', index: 'IsAvailable', editable: true },
                { key: false, hidden: true, name: 'IssuedDate', index: 'IssuedDate', editable: true },
                { key: false, hidden: true, name: 'StudyMaterialStatusBy', index: 'StudyMaterialStatusBy', editable: true },
                { label: "Subject Code", name: 'StudyMaterialCode', index: 'StudyMaterialCode' },
                { label: "Subject Name", name: 'StudyMaterialName' },
                { label: "Year", name: 'SubjectYearText' },
                { label: "Status", name: 'IsAvailable', formatter: GetBookStatus, editable: true },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }
    function GetBookStatus(cellValue, options, rowdata, action) {
        var html = "";
        if (rowdata.IsIssued == true) {

            var issueddate = moment(rowdata.IssuedDate).format('DD MMMM  YYYY');
            return html = '<span class="label label-danger">' + "Issued By " + rowdata.StudyMaterialStatusBy + " On " + issueddate + '</span>';

        }
        else if (rowdata.IsIssued == false && rowdata.IsAvailable == true) {

            return html = '<span class="label label-success">' + "Available" + '</span>';

        }
        else {
            return html = '<span class="label label-warning">' + "Not Available" + '</span>';
        }

    }

    // StudyMaterial Issue Summery End


    // Internal Exam Result Summary Start

    var getInternalExamResultSummary = function (json) {
        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'InternalExamDetailsKey', index: 'InternalExamDetailsKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'InternalExamKey', index: 'InternalExamKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'SubjectKey', index: 'SubjectKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SubjectName', headerText: Resources.Subject, index: 'SubjectName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ExamDate', headerText: Resources.ExamDate, index: 'ExamDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ExamStartTime', headerText: Resources.StartTime, index: 'ExamStartTime', formatter: formatTimeColumn, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ExamEndTime', headerText: Resources.EndTime, index: 'ExamEndTime', formatter: formatTimeColumn, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseName', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MaximumMark', headerText: Resources.MaximumMark, index: 'MaximumMark', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MinimumMark', headerText: Resources.MinimumMark, index: 'MinimumMark', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BatchName', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'InternalExamTermName', headerText: Resources.InternalExamTerm, index: 'InternalExamTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'Affiliations', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassCode', index: 'ClassCode', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'TotalStudents', index: 'TotalStudents', headerText: Resources.TotalStudents, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Passed', index: 'Passed', headerText: Resources.Passed, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Failed', index: 'Failed', headerText: Resources.Failed, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Absent', index: 'Absent', headerText: Resources.Absent, editable: true, cellEdit: true, sortable: true, resizable: false },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetInternalExamResultSummaryReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showStudentMarkListChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

    function showStudentMarkListChildGrid(parentRowID, parentRowKey) {

        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');
        item = $("#grid").jqGrid("getRowData", parentRowKey);
        $("#" + childGridID).jqGrid({
            url: $("#hdnGetStudentMarkDetails").val(),//+ "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            postData: { InternalExamDetailsKey: item.InternalExamDetailsKey, ClassDetailsKey: item.ClassDetailsKey, InternalExamKey: item.InternalExamKey, SubjectKey: item.SubjectKey },
            page: 1,
            colModel: [
                { label: Resources.AdmissionNo, name: 'AdmissionNo', index: 'FeeYearText' },
                { label: Resources.StudentName, name: 'StudentName' },
                { label: Resources.Mark, name: 'Mark' },
                { label: Resources.ResultStatus, name: 'ResultStatus', formatter: formatExamStatusColumn },
                { label: Resources.Remarks, name: 'Remarks' },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }

    function formatTimeColumn(cellValue, options, rowdata, action) {
        // var columnName = options.colModel["name"];
        //var string = options.gid + "_" + columnName;
        //setTimeout(function () {
        //    if (columnName == "InDateTime") {
        //        $("td[aria-describedby=" + string + "]").addClass("text-green")
        //    }
        //    else {
        //        $("td[aria-describedby=" + string + "]").addClass("text-red")
        //    }
        //}, 100)
        if (cellValue != null) {
            //return moment(cellValue).format("hh:mm A");
            return moment(cellValue).format("hh:mm A");


        }
        //else {

        //    if ((columnName == "InDateTime" && rowdata.ClockInStatus == false)) {
        //        setTimeout(function () { $("td[aria-describedby=" + string + "]", $("tr[id=" + options.rowId + "]")).attr("data-digital-clock-date", "") }, 100);
        //    }
        //    else if ((columnName == "OutDateTime" && rowdata.ClockInStatus == true)) {
        //        setTimeout(function () { $("td[aria-describedby=" + string + "]", $("tr[id=" + options.rowId + "]")).attr("data-digital-clock-date", "") }, 100);
        //    }
        //    return "";
        //}


    }

    function formatExamStatusColumn(cellValue, options, rowdata, action) {

        if (rowdata.ResultStatus == "P") {

            return "<span style=color:green>Pass</span>"

        }
        else if (rowdata.ResultStatus == "F") {

            return "<span style=color:red>Fail</span>"
        }
        else if (rowdata.ResultStatus == "A") {
            return "<span style=color:orange>Absent</span>"

        }
        else if (rowdata.ResultStatus === null) {

            return "<span style=color:black>N/A</span>"
        }

    }

    // Internal Exam Result Summary End



    // Student Id Card Issue Summery Start

    var getStudentIdCardIssueSummary = function (json) {

        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentEmail', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentMobile', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'IsReceived', index: 'IsReceived', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'IsIssued', index: 'IsIssued', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'ReceivedBy', index: 'ReceivedBy', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'IssuedBy', index: 'IssuedBy', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'ReceivedDate', index: 'ReceivedDate', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'IssuedDate', index: 'IssuedDate', editable: true },
            { key: false, name: 'ReceivedStatus', index: 'ReceivedStatus', headerText: Resources.IsReceived, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: IdCardRecievedStatus, width: 250 },
            { key: false, name: 'IssuedStatus', index: 'IssuedStatus', headerText: Resources.IsIssued, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: IdCardIssuedStatus, width: 250 },
        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStududentIdCardIssueReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");

    }

    function IdCardRecievedStatus(cellValue, options, rowdata, action) {
        var html = "";
        if (rowdata.IsReceived == 1) {
            var ReceivedDate = moment(rowdata.ReceivedDate).format('DD MMMM  YYYY');
            return html = '<span class="label label-success">' + "Received By " + rowdata.ReceivedBy + " On " + ReceivedDate + '</span>';

        }

        else {
            return html = '<span class="label label-danger">' + "No" + '</span>';
        }

    }
    function IdCardIssuedStatus(cellValue, options, rowdata, action) {
        var html = "";
        if (rowdata.IsIssued == 1) {

            var issueddate = moment(rowdata.IssuedDate).format('DD MMMM  YYYY');
            return html = '<span class="label label-info">' + "Issued By " + rowdata.IssuedBy + " On " + issueddate + '</span>';

        }

        else {
            return html = '<span class="label label-danger">' + "No" + '</span>';
        }

    }

    // Student Id Card Issue Summery End



    // University Fee Summary Start

    var getStudentUniversityFeeSummary = function (json) {
        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentEmail', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentMobile', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            //{ key: false, name: 'TotalFee', index: 'StudentTotalFee', headerText: Resources.TotalFee, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'TotalPaid', index: 'TotalPaid', headerText: Resources.FeeAmountPaid, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'TotalUniversityPaid', index: 'TotalUniversityPaid', headerText: Resources.UniversityAmountPaid, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false,width:250 },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudentsUniversityFeeReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',
            colNames: colNames,
            colModel: colModelList,
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            footerrow: true,
            userDataOnFooter: true,
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: false, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showUniversityFeeChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");

    }

    function showUniversityFeeChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetStudentUniversityFeeDetails").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: Resources.Year, name: 'FeeYearText', index: 'FeeYearText' },
                { label: Resources.TotalFee, name: 'TotalAmount', formatter: RupeeIcon },
                { label: Resources.FeeAmountPaid, name: 'FeePaid', formatter: RupeeIcon },
                { label: Resources.UniversityAmountPaid, name: 'UniversityFeePaid', formatter: RupeeIcon },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }

    // Fee Summery End




    // Student Certificate  Summery Start

    var getStudentCertificateSummary = function (json) {



        JsonModel = json;


        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentEmail', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentMobile', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, name: 'TotalCertificates', index: 'TotalCertificates', headerText: Resources.NoOfCertificate, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            //{ key: false, name: 'NoOfRecieved', index: 'NoOfRecieved', headerText: Resources.NoOfRecived, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            { key: false, name: 'NoOfVerified', index: 'NoOfVerified', headerText: Resources.NoOfVerified, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            { key: false, name: 'NoOfTempReturned', index: 'NoOfTempReturned', headerText: "No Of Temp Return", editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            { key: false, name: 'NoOfReturned', index: 'NoOfReturned', headerText: Resources.NoOfReturned, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudentCertificateReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showStudentCertificateChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }
    function showStudentCertificateChildGrid(parentRowID, parentRowKey) {
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
                { key: false, hidden: true, name: 'CertificateStatusName', index: 'CertificateStatusName', editable: true },
                { key: false, hidden: true, name: 'CertificateStatusBy', index: 'CertificateStatusBy', editable: true },
                { key: false, hidden: true, name: 'IssuedDate', index: 'IssuedDate', editable: true },
                { label: "Certificate Name", name: 'EducationQualificationName', index: 'EducationQualificationName' },
                { label: "Affiliation / Tieup ", name: 'EducationQualificationUniversity' },
                { label: "Current Status", name: 'CertificateStatusName', formatter: GetCertificateStatus, editable: true },

            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }
    function GetCertificateStatus(cellValue, options, rowdata, action) {

        var html = "";
        var issueddate = moment(rowdata.IssuedDate).format('DD MMMM  YYYY');
        // return html = '<span class="label label-danger">' + rowdata.CertificateStatusName + " By " + rowdata.CertificateStatusBy + " On " + issueddate + '</span>';
        if (rowdata.CertificateStatusName.toLowerCase() == "temporary returned") {

            var issueddate = moment(rowdata.IssuedDate).format('DD MMMM  YYYY');
            return html = '<span class="label label-danger">' + rowdata.CertificateStatusName + " By " + rowdata.CertificateStatusBy + " On " + issueddate + '</span>';

        }
        else if (rowdata.CertificateStatusName.toLowerCase() == "returned") {

            return html = '<span class="label label-success">' + rowdata.CertificateStatusName + " By " + rowdata.CertificateStatusBy + " On " + issueddate + '</span>';

        }
        else if (rowdata.CertificateStatusName.toLowerCase() == "verified") {

            return html = '<span class="label label-info">' + rowdata.CertificateStatusName + " By " + rowdata.CertificateStatusBy + " On " + issueddate + '</span>';

        }
        else {
            return html = '<span class="label label-warning">' + rowdata.CertificateStatusName + " By " + rowdata.CertificateStatusBy + " On " + issueddate + '</span>';
        }

    }

    // Student Certificate Summery End




    // University Certificate  Summery Start

    var getUniversityCertificateSummary = function (json) {

        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentEmail', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentMobile', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, name: 'TotalCertificates', index: 'TotalCertificates', headerText: Resources.NoOfCertificate, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            { key: false, name: 'NoOfRecieved', index: 'NoOfRecieved', headerText: Resources.NoOfRecived, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            { key: false, name: 'NoOfIssued', index: 'NoOfIssued', headerText: Resources.NoOfVerified, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            //{ key: false, name: 'NoOfTempReturned', index: 'NoOfTempReturned', headerText: "No Of Temp Return", editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            //{ key: false, name: 'NoOfReturned', index: 'NoOfReturned', headerText: Resources.NoOfReturned, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetUniversityCertificateReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {
                StudentSummary.GetCustomizedColumns();
            },
            onPaging: function () {
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showUniversityCertificateChildGrid,

            subGridOptions: {

                reloadOnExpand: false,

                selectOnExpand: true

            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }
    function showUniversityCertificateChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetUniversityCertificateDetailsByApplication").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { key: false, hidden: true, name: 'IsReceived', index: 'IsReceived', editable: true },
                { key: false, hidden: true, name: 'IsIssued', index: 'IsIssued', editable: true },
                { key: false, hidden: true, name: 'ReceivedDate', index: 'ReceivedDate', editable: true },
                { key: false, hidden: true, name: 'IssuedDate', index: 'IssuedDate', editable: true },
                { label: "Certificate Name", name: 'CertificateTypeName', index: 'CertificateTypeName' },
                { label: "Affiliation / Tieup ", name: 'UniversityCertificateDescription' },
                { label: "ReceivedStatus", name: 'ReceivedByName', formatter: GetUniversityCertificateRecievedStatus, editable: true },
                { label: "Issued Status", name: 'IssuedByName', formatter: GetUniversityCertificateIssuedStatus, editable: true },

            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }
    function GetUniversityCertificateRecievedStatus(cellValue, options, rowdata, action) {


        var html = "";
        if (rowdata.IsReceived == true) {

            var receivedDate = moment(rowdata.ReceivedDate).format('DD MMMM  YYYY');
            return html = '<span class="label label-info">' + "Recived By " + rowdata.ReceivedByName + " On " + receivedDate + '</span>';

        }

        else {
            return html = '<span class="label label-danger">' + "No" + '</span>';
        }


    }
    function GetUniversityCertificateIssuedStatus(cellValue, options, rowdata, action) {

        var html = "";
        if (rowdata.IsIssued == true) {

            var issueddate = moment(rowdata.IssuedDate).format('DD MMMM  YYYY');
            return html = '<span class="label label-info">' + "Issued By " + rowdata.IssuedByName + " On " + issueddate + '</span>';

        }

        else {
            return html = '<span class="label label-danger">' + "No" + '</span>';
        }


    }

    // University Certificate Summery End



    // Unit Test Exam Result Summary Start

    var getUnitTestExamResultSummary = function (json) {

        JsonModel = json;


        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'UnitTestScheduledKey', index: 'UnitTestScheduledKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'SubjectKey', index: 'SubjectKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SubjectName', headerText: Resources.Subject, index: 'SubjectName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ModuleName', headerText: Resources.SubjectModule, index: 'ModuleName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ExamDate', headerText: Resources.ExamDate, index: 'ExamDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MaximumMark', headerText: Resources.MaximumMark, index: 'MaximumMark', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MinimumMark', headerText: Resources.MinimumMark, index: 'MinimumMark', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'Affiliations', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            //{ key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.AcademicTerm, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'TotalTopics', index: 'TotalTopics', headerText: Resources.TotalTopics, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'TotalStudents', index: 'TotalStudents', headerText: Resources.TotalStudents, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Passed', index: 'Passed', headerText: Resources.Passed, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Failed', index: 'Failed', headerText: Resources.Failed, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Absent', index: 'Absent', headerText: Resources.Absent, editable: true, cellEdit: true, sortable: true, resizable: false },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetUnitTestExamResultSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showUnitTestStudentMarkListChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

    function showUnitTestStudentMarkListChildGrid(parentRowID, parentRowKey) {

        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');
        item = $("#grid").jqGrid("getRowData", parentRowKey);
        $("#" + childGridID).jqGrid({
            url: $("#hdnGetUnitTestStudentMarkDetails").val(),//+ "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            postData: { UnitTestScheduledKey: item.UnitTestScheduledKey, ClassDetailsKey: item.ClassDetailsKey, SubjectKey: item.SubjectKey },
            page: 1,
            colModel: [
                { label: Resources.AdmissionNo, name: 'AdmissionNo', index: 'FeeYearText' },
                { label: Resources.StudentName, name: 'StudentName' },
                { label: Resources.Mark, name: 'Mark' },
                { label: Resources.ResultStatus, name: 'ResultStatus', formatter: formatExamStatusColumn },
                { label: Resources.Remarks, name: 'Remarks' },
                { label: Resources.ModuleTopics, name: 'TopicNames' },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }

    // Unit Test Exam Result Summary End


    // Student Exam Schedule  Summary Start

    var getExamScheduleSummary = function (json) {
        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentEmail', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentMobile', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, name: 'TotalSubjects', index: 'TotalSubjects', headerText: Resources.TotalSubject, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250 },
            { key: false, name: 'AppliedSubject', index: 'AppliedSubject', headerText: Resources.Applied, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250, formatter: TextHilightwithbadge },
            { key: false, name: 'Passed', index: 'Passed', headerText: Resources.Passed, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250, formatter: TextHilightwithbadge },
            { key: false, name: 'Failed', index: 'Failed', headerText: Resources.Failed, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250, formatter: TextHilightwithbadge },
            { key: false, name: 'Absent', index: 'Absent', headerText: Resources.Absent, editable: true, cellEdit: true, sortable: true, resizable: false, width: 250, formatter: TextHilightwithbadge },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudentExamScheduleSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showStudentExamSchdeuleDetailsChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

    function showStudentExamSchdeuleDetailsChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetExamSchdeuleDetailsByApplication").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { key: false, hidden: true, name: 'ExamRegisterNumber', index: 'ExamRegisterNumber', editable: true },

                { label: "Subject Name", name: 'SubjectName', index: 'SubjectName' },
                { label: "Subject Year ", name: 'SubjectYearName' },
                { label: "Exam Term Name ", name: 'ExamTermName' },
                { label: "Exam Date", name: 'ExamDate', formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { label: "Mark", name: 'Mark' },
                { label: "ResultStatus", name: 'ResultStatus', formatter: formatExamStatusColumn },
                { label: "Appearence", name: 'AppearenceCount', formatter: AppearenceCountName },
                { label: "Status ", name: 'AppliedStatus', formatter: formatAppliedStatusColumn },

            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }

    function formatAppliedStatusColumn(cellValue, options, rowdata, action) {

        if (rowdata.AppliedStatus == "A") {

            return "<span style=color:green>Applied</span>"

        }
        else if (rowdata.AppliedStatus == "NA") {

            return "<span style=color:red>Not Applied</span>"
        }
        else if (rowdata.AppliedStatus == "C") {
            return "<span style=color:orange>Complete</span>"

        }
        else if (rowdata.AppliedStatus === null) {

            return "<span style=color:blue>N/A</span>"
        }

    }

    function AppearenceCountName(cellValue, options, rowdata, action) {

        if (rowdata.AppearenceCount == 1) {

            return "<span style=color:black>1st Appearence</span>"

        }
        else if (rowdata.AppearenceCount == 2) {

            return "<span style=color:black>2nd Appearence</span>"

        }
        else if (rowdata.AppearenceCount == 3) {

            return "<span style=color:black>3rd Appearence</span>"

        }
        else if (rowdata.AppearenceCount == 4) {

            return "<span style=color:black>4th Appearence</span>"

        }
        else if (rowdata.AppearenceCount == null || rowdata.AppearenceCount == "") {

            return "<span style=color:black>1st Appearence</span>"

        }
        else {
            return "<span style=color:black>" + rowdata.AppearenceCount + "th Appearence</span>"

        }

    }
    // Student Certificate Summary End


    // Teacher Work Schedule Summary Start

    var getTeacherWorkScheduleSummary = function (json) {
        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'BranchKey', index: 'BranchKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'BatchKey', index: 'BatchKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'SubjectModuleKey', index: 'SubjectModuleKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'ClassDetailsKey', index: 'ClassDetailsKey', editable: true },
            { key: false, hidden: true, headerText: Resources.RowKey, name: 'SubjectKey', index: 'SubjectKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SubjectName', headerText: Resources.Subject, index: 'SubjectName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ModuleName', headerText: Resources.SubjectModule, index: 'ModuleName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'Course', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'Affiliations', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassCode', index: 'ClassCode', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Duration', index: 'Duration', headerText: Resources.Duration, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatMinToHrs },
            { key: false, name: 'ProgressStatus', index: 'ProgressStatus', headerText: Resources.Status, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatProgressStatus },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetTeacherWorkScheduleSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {

            },
            subGrid: true,
            subGridWidth: 40,
            subGridRowExpanded: showHistoryWorkScheduleChildGrid,

            subGridOptions: {

                reloadOnExpand: false,

                selectOnExpand: true

            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }
    function showHistoryWorkScheduleChildGrid(parentRowID, parentRowKey) {

        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');
        item = $("#grid").jqGrid("getRowData", parentRowKey);
        $("#" + childGridID).jqGrid({
            url: $("#hdnGetHistoryWorkSchedule").val(),//+ "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            postData: { BatchKey: item.BatchKey, ClassDetailsKey: item.ClassDetailsKey, SubjectKey: item.SubjectKey, BranchKey: item.BranchKey, SubjectModuleKey: item.SubjectModuleKey },
            page: 1,
            colModel: [
                { key: false, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },
                { label: Resources.Topic, name: 'TopicName' },
                { label: Resources.Duration, name: 'Duration', formatter: formatMinToHrs },
                { label: Resources.Status, name: 'CurrentProgressStatus' },
                { name: 'edit', search: false, index: 'RowKey', sortable: false, formatter: editLink, resizable: false },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }
    function editLink(cellValue, options, rowdata, action) {

        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-sm mx-1" href="#"   onclick="javascript:workScheduleHistoryCustomPopup(' + rowdata.RowKey + ');return false;"><i class="fa fa-history" aria-hidden="true"></i></a></div>';
    }

    function formatMinToHrs(cellValue, option, rowdata, action) {
        var TotalWorkDuration = cellValue;
        TotalWorkDuration = parseInt(TotalWorkDuration) ? parseInt(TotalWorkDuration) : 0;
        var num = TotalWorkDuration;
        var hours = (num / 60);
        var rhours = Math.floor(hours);
        var minutes = (hours - rhours) * 60;
        var rminutes = Math.round(minutes);
        var TotalWorkDurationText = rhours + " HR : " + rminutes + " Min.";
        return '<i  class="fa fa-clock-o" aria-hidden="true"></i>' + " " + TotalWorkDurationText;
    }
    function formatProgressStatus(cellValue, option, rowdata, action) {

        var ProgressStatus = cellValue

        ProgressStatus = parseInt(ProgressStatus) ? parseInt(ProgressStatus) : 0;

        if (ProgressStatus < 100) {

            return "<span class='badge badge-pill badge-danger'>" + "Pending (" + ProgressStatus + " % )" + "</span>"
        }
        else if (ProgressStatus == 0) {
            return "<span class='badge badge-pill badge-success'>" + "Pending (" + ProgressStatus + " % )" + "</span>"
        }
        else if (ProgressStatus >= 100) {
            return "<span class='badge badge-pill badge-success'>" + "Completed (" + ProgressStatus + " % )" + "</span>"
        }
    }

    // Teacher Work Schedule Summary End


    // Student Late Summery Start

    var getStudentLateSummary = function (json) {
        //var CourseKeyList = "";


        JsonModel = json;
        var newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
        $.extend(newPostData, JsonData)
        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: newPostData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentMobile', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentEmail', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'RollNoCode', index: 'RollNoCode', headerText: Resources.RollNumber, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'LateDate', index: 'LateDate', headerText: Resources.LateDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'LateMinutes', index: 'LateMinutes', headerText: Resources.LateMinutes, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'LateRemarks', index: 'LateRemarks', headerText: Resources.Remarks, editable: true, cellEdit: true, sortable: true, resizable: false },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudentsLateReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: newPostData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }



        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");


    }

    // Student Late Summery End


    // Student Leave Summery Start

    var getStudentLeaveSummary = function (json) {
        //var CourseKeyList = "";


        JsonModel = json;
        var newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
        $.extend(newPostData, JsonData)
        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: newPostData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentMobile', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentEmail', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'RollNoCode', index: 'RollNoCode', headerText: Resources.RollNumber, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'LeaveDateFrom', index: 'LeaveDateFrom', headerText: Resources.LeaveDateFrom, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'LeaveDateTo', index: 'LeaveDateTo', headerText: Resources.LeaveDateTo, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'LeaveRemarks', index: 'LeaveRemarks', headerText: Resources.Remarks, editable: true, cellEdit: true, sortable: true, resizable: false },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudentsLeaveReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: newPostData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }



        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");


    }

    // Student Leave Summery End

    // Student Absconders Summery Start

    var getStudentAbscondersSummary = function (json) {
        //var CourseKeyList = "";


        JsonModel = json;
        var newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
        $.extend(newPostData, JsonData)
        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: newPostData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentMobile', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentEmail', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'RollNoCode', index: 'RollNoCode', headerText: Resources.RollNumber, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AbscondersDate', index: 'AbscondersDate', headerText: Resources.AbscondersDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'IsAbsconders', index: 'IsAbsconders', headerText: Resources.Pardonit, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentsAbscondersRemarks', index: 'StudentsAbscondersRemarks', headerText: Resources.Remarks, editable: true, cellEdit: true, sortable: true, resizable: false },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudentsAbscondersReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: newPostData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }



        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");


    }

    // Student Absconders Summery End

    // Student EarlyDeparture Summery Start

    var getStudentEarlyDepartureSummary = function (json) {
        //var CourseKeyList = "";


        JsonModel = json;
        var newPostData = $.extend(true, {}, json);

        var JsonData = $("form").serializeToJSON({

        });
        $.extend(newPostData, JsonData)
        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: newPostData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentMobile', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentEmail', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'RollNoCode', index: 'RollNoCode', headerText: Resources.RollNumber, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EarlyDepartureDate', index: 'EarlyDepartureDate', headerText: Resources.EarlyDepartureDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'EarlyDepartureTime', index: 'EarlyDepartureTime', headerText: Resources.EarlyDepartureTime, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EarlyDepartureRemarks', index: 'EarlyDepartureRemarks', headerText: Resources.Remarks, editable: true, cellEdit: true, sortable: true, resizable: false },

        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetStudentsEarlyDepartureReport").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: newPostData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
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
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }



        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");


    }

    // Student EarlyDeparture Summery End


    // Fee Refund Summary Start


    var getStudentFeeRefundSummary = function (json) {
        JsonModel = json;
        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'Name', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Batch', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ClassMode', headerText: Resources.ClassModeName, index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentEmail', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentMobile', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', headerText: Resources.EnrollmentNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'GuardianName', index: 'StudentGuardian', headerText: Resources.StudentGuardian, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MotherName', index: 'StudentMotherName', headerText: Resources.StudentMother, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', headerText: Resources.StudentPermanentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PresentAddress', index: 'StudentPresentAddress', headerText: Resources.PresentAddress, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Medium', index: 'Medium', headerText: Resources.Medium, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Income', index: 'IncomeName', headerText: Resources.Income, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DOB', index: 'StudentDOB', headerText: Resources.DateOfBirth, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'SecondLanguage', index: 'SecondLanguageName', headerText: Resources.SecondLanguage, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', headerText: Resources.AdmissionDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ClassRequired', index: 'StudentClassRequired', headerText: Resources.ClassRequired, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'StudentGender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, name: 'IsTaxText', index: 'IsTaxText', headerText: Resources.IsTax, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'IsInstallmentText', index: 'IsInstallmentText', headerText: Resources.IsInstallment, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'IsConsessionText', index: 'IsConsessionText', headerText: Resources.IsConsession, editable: true, cellEdit: true, formatter: formatYesorNO, sortable: true, resizable: false },
            { key: false, name: 'StudentTotalFee', index: 'StudentTotalFee', headerText: Resources.TotalFee, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'TotalPaid', index: 'TotalPaid', headerText: Resources.TotalPaid, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'BalanceAmount', index: 'BalanceAmount', headerText: Resources.BalanceAmount, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'FeeTypeName', index: 'FeeTypeName', headerText: Resources.FeeType, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'TotalAmount', index: 'TotalAmount', headerText: "Paid Amount", editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },

            { key: false, name: 'RefundDate', index: 'RefundDate', headerText: "Refund Date", editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'RefundAmount', index: 'RefundAmount', headerText: "Refund Amount", editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'ProcessStatus', index: 'ProcessStatus', headerText: Resources.Status, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'RefundBy', index: 'RefundBy', headerText: "Refund By", editable: true, cellEdit: true, sortable: true, resizable: false },


        ];
        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetFeeRefundSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                //userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            footerrow: true,
            // userDataOnFooter: true,
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: false, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showFeeRefundChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        if ($("#FeeRefundTabKey").val() == 2) {

            json["DefaultColumns"] = ["RowNumber", "AdmissionNo", "Name", "CourseType", "Course", "Affiliations", "CurrentYearText", "StudentStatus", "FeeTypeName", "TotalAmount", "RefundDate", "RefundAmount", "ProcessStatus", "RefundBy"]
        }
        else {
            json["DefaultColumns"] = ["RowNumber", "AdmissionNo", "Name", "CourseType", "Course", "Affiliations", "CurrentYearText", "StudentStatus", "RefundDate", "RefundAmount", "ProcessStatus", "RefundBy"]
        }
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

    function showFeeRefundChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetStudentRefundDetails").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: Resources.Year, name: 'FeeYearText', index: 'FeeYearText' },
                { label: Resources.TotalFee, name: 'TotalAmount', formatter: RupeeIcon },
                { label: Resources.TotalPaid, name: 'FeeAmount', formatter: RupeeIcon },
                { label: Resources.BalanceAmount, name: 'BalanceAmount', formatter: RupeeIcon },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }


    var getExportPrintDynamicData = function (type, url, filename, title) {

        var sortColumnName = $("#grid").jqGrid('getGridParam', 'sortname');
        var sortOrder = $("#grid").jqGrid('getGridParam', 'sortorder'); // 'desc' or 'asc'
        var rows = $("#grid").getRowData().length;
        //var row = $("#grid").jqGrid('getGridParam', 'records'); // All Records
        var page = $('#grid').getGridParam('page');


        var obj = {};
        obj.ajaxData = $("form").serializeToJSON({

        });
        obj.beforeProcessing = function () {
            var data = this;
            data.rows = $(data.rows).map(function (n, item) {
                var obj = {};
                $(item).each(function () {
                    obj[this.Key] = this.Value;
                });
                return obj;
            });
        }
        obj.ajaxData.sidx = sortColumnName;
        obj.ajaxData.sord = sortOrder;
        obj.ajaxData.page = page;
        obj.ajaxData.rows = rows;
        obj.ajaxType = "POST";
        obj.ajaxUrl = $(url).val();
        obj.ContainerId = "#grid";
        obj.FileName = filename;
        obj.Title = title;
        AppCommon.ExportPrintAjax(obj, type)


    }

    // Fee Refund Summary End



    // CashFlow Summery Start


    var getCashFlowSummary = function (json) {

        JsonModel = json;

        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'VoucherNumber', headerText: Resources.VoucherNumber, index: 'VoucherNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AccountheadName', headerText: Resources.AccountHead, index: 'AccountheadName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CashFlowTypeName', headerText: Resources.CashFlowType, index: 'CashFlowTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CashFlowDate', headerText: Resources.Date, index: 'CashFlowDate', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Amount', headerText: Resources.Amount, index: 'Amount', editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'PaymentMode', headerText: Resources.PaymentMode, index: 'PaymentMode', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BankAccount', headerText: Resources.Bank, index: 'BankAccount', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ChequeOrDDNumber', headerText: Resources.ChequeOrDDNumber, index: 'ChequeOrDDNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ChequeOrDDDate', index: 'ChequeOrDDDate', headerText: Resources.ChequeOrDDDate, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'ReferenceNumber', index: 'ReferenceNumber', headerText: "Reference Number", editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Remarks', index: 'Remarks', headerText: Resources.Remarks, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Purpose', index: 'Purpose', headerText: Resources.Purpose, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PaidBy', index: 'PaidBy', headerText: Resources.PaidBy, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AuthorizedBy', index: 'AuthorizedBy', headerText: Resources.AuthorizedBy, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'ReceivedBy', index: 'ReceivedBy', headerText: Resources.ReceivedBy, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'OnBehalfOf', index: 'OnBehalfOf', headerText: Resources.OnBehalfOf, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DateAdded', index: 'DateAdded', headerText: "Date Added", editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },

        ];

        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetCashFlowSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',
            colNames: colNames,
            colModel: colModelList,
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'DateAdded',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            //rownumbers: true,
            //rownumWidth:50,
            footerrow: true,
            userDataOnFooter: true,
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }
        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "rn", "Sl.No");


    }


    // CashFlow Summary End


    // Fee Installment Summary Start

    var getFeeInstallmentSummary = function (json) {
        JsonModel = json;

        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'AdmissionNo', headerText: Resources.AdmissionNo, index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Name', headerText: Resources.Name, index: 'name', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CourseType', headerText: Resources.CourseType, index: 'CourseType', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Course', headerText: Resources.Course, index: 'Course', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Affiliations', headerText: Resources.AffiliationsTieUps, index: 'Affiliations', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AcademicTerm', headerText: Resources.AcademicTerm, index: 'AcademicTerm', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BatchName', headerText: Resources.Batch, index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mode', headerText: Resources.Mode, index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Mobile', index: 'StudentEmail', headerText: Resources.MobileNo, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Email', index: 'StudentMobile', headerText: Resources.Email, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'StudentStatus', index: 'StudentStatusName', headerText: Resources.StudentStatus, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Class', index: 'Class', headerText: Resources.ClassCode, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Agent', index: 'AgentName', headerText: Resources.Agent, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Religion', index: 'ReligionName', headerText: Resources.Religion, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'CurrentYearText', index: 'CurrentYearText', headerText: Resources.CurrentYear, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'Gender', index: 'Gender', headerText: Resources.Gender, editable: true, cellEdit: true, sortable: true, resizable: false, formatter: formatGender },
            { key: false, name: 'InstallmentNo', index: 'InstallmentNo', headerText: "Installment No", editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'InstallmentMonth', index: 'InstallmentMonth', headerText: "Installment Month", formatter: formatInstallmentMonth, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'InstallmentYear', index: 'InstallmentYear', headerText: "Installment Year", editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'InstallmentAmount', index: 'InstallmentAmount', headerText: "Installment Amount", editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'InstallmentPaid', index: 'InstallmentPaid', headerText: "Installment Paid", editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'BalanceDue', index: 'BalanceDue', headerText: "Installment Due", editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'DueDate', index: 'DueDate', headerText: "Due Date", editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'TotalFee', index: 'StudentTotalFee', headerText: Resources.TotalFee, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'TotalPaid', index: 'TotalPaid', headerText: Resources.TotalPaid, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'BalanceFee', index: 'BalanceAmount', headerText: Resources.BalanceAmount, editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },

        ];

        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });

        $("#grid").jqGrid({

            url: $("#hdnGetFeeInstallmentSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',


            colNames: colNames,
            colModel: colModelList,

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            footerrow: true,
            userDataOnFooter: true,
            rownumbers: true,

            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 40,
            subGridRowExpanded: showInstallmentFeeChildGrid,
            //subGridBeforeExpand: function (subgridDivId, rowid) {

            //    var rowIds = $("#grid").getDataIDs();
            //    $.each(rowIds, function (index, rowId) {
            //        $("#grid").collapseSubGridRow(rowId);
            //    });

            //},// javascript function that will take care of showing the child grid
            subGridOptions: {
                // load the subgrid data only once
                // and the just show/hide
                reloadOnExpand: false,
                // select the row when the expand column is clicked
                selectOnExpand: true
                //url from which subgrid data should be requested
            },

        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");
        //$("#grid").jqGrid("showHideColumnMenu");



    }

    function showInstallmentFeeChildGrid(parentRowID, parentRowKey) {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetStudentFeeInstallmentDetails").val() + "?id=" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: Resources.Year, name: 'InstallmentYear', index: 'InstallmentYear' },
                { label: Resources.Month, name: 'InitialPaymentMonth', index: 'InitialPaymentMonth' },
                { label: Resources.TotalFee, name: 'InitialPaymentAmount', formatter: RupeeIcon },
                { label: Resources.TotalPaid, name: 'InitialPaymentAmountPaid', formatter: RupeeIcon },
                { label: Resources.BalanceAmount, name: 'InitialPaymentbalanceDue', formatter: RupeeIcon },
            ],
            loadonce: true,
            width: 500,
            height: '100%',
            pager: false,
            footer: false
        });

    }

    function formatInstallmentMonth(cellValue, option, rowdata, action) {
        cellValue = cellValue ? cellValue : 0;
        if (rowdata.InstallmentYear != null) {
            return '<span  >' + rowdata.InstallmentMonth + " - " + rowdata.InstallmentYear + '</span>';
        }
        return cellValue;
    }


    // Fee Installment Summary End


    // Employee Salary Summary Start


    var getEmployeeSalarySummary = function (json) {

        JsonModel = json;

        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'SalaryMasterKey', index: 'SalaryMasterKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'VoucherNumber', headerText: "Pay Slip No", index: 'VoucherNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BranchName', headerText: Resources.Branch, index: 'BranchName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'EmployeeName', headerText: Resources.Employee, index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MobileNumber', headerText: Resources.MobileNumber, index: 'MobileNumber', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'PaySlipFileName', headerText: "Salary Month", index: 'PaySlipFileName', editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'MonthlySalary', headerText: "Monthly Salary", index: 'MonthlySalary', formatter: RupeeIcon, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'TotalSalary', headerText: Resources.TotalSalary, index: 'TotalSalary', formatter: RupeeIcon, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'TotalPaid', index: 'TotalPaid', headerText: "Total Paid", editable: true, formatter: RupeeIcon, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BalanceAmount', index: 'BalanceAmount', headerText: "BalanceAmount", editable: true, formatter: RupeeIcon, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'LOP', headerText: "LOP", index: 'LOP', editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'AditionAmount', index: 'AditionAmount', headerText: "Adition Amount", formatter: RupeeIcon, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'DeductionAmount', index: 'DeductionAmount', headerText: "Deduction Amount", formatter: RupeeIcon, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdvanceAmount', index: 'AdvanceAmount', headerText: "Advance Amount", formatter: RupeeIcon, editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdditionalDayAmount', index: 'AdditionalDayAmount', headerText: "Additional Day Amount", editable: true, formatter: RupeeIcon, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AdditionalDayWorked', index: 'AdditionalDayWorked', headerText: "Additional Day Worked", editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'BaseWorkingDays', index: 'BaseWorkingDays', headerText: "Base Working Days", editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'AbsentDays', index: 'AbsentDays', headerText: "Absent Days", editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'OvertimeTotalHour', index: 'OvertimeTotalHour', headerText: "Over Time Hours", editable: true, cellEdit: true, sortable: true, resizable: false },
            { key: false, name: 'OverTimeTotalAmount', index: 'OverTimeTotalAmount', headerText: "Over Time Amount", editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false },
            { key: false, name: 'LastPaymentDate', index: 'LastPaymentDate', headerText: "Last Payment Date", editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Remarks', index: 'Remarks', headerText: "Remarks", editable: true, cellEdit: true, sortable: true, resizable: false },

        ];

        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetEmployeeSalarySummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',
            colNames: colNames,
            colModel: colModelList,
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'SalaryMasterKey',
            sortorder: 'desc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            //rownumbers: true,
            //rownumWidth:50,
            footerrow: true,
            userDataOnFooter: true,
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }
        })
        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", "rn", "Sl.No");


    }


    // Employee Salary Summary End


    // AccountHead Income And Expense Summary Start


    var getAccountHeadIncomeExpenseSummary = function (json) {

        JsonModel = json;

        $('#grid').jqGrid('GridUnload');

        var fromdate = $("#DateAddedFrom").val();
        var Todate = $("#DateAddedTo").val();
        var IncomeByDate = 'Income';
        var ExpenseByDate = 'Expense';
        var BalanceByDate = 'Balance';
        if (fromdate == Todate) {
            IncomeByDate = IncomeByDate + ' on ' + Todate;
            ExpenseByDate = ExpenseByDate + ' on ' + Todate;
            BalanceByDate = BalanceByDate + ' on ' + Todate;
        }
        else {
            IncomeByDate = IncomeByDate + ' b / w ' + fromdate + ' - ' + Todate;
            ExpenseByDate = ExpenseByDate + ' b / w ' + fromdate + ' - ' + Todate;
            BalanceByDate = BalanceByDate + ' b / w ' + fromdate + ' - ' + Todate;
        }


        var JsonData = $("form").serializeToJSON({

        });

        $("#grid").setGridParam({ postData: null });
        $("#grid").jqGrid('setGridParam', {
            postData: JsonData
        }).trigger('reloadGrid');

        var colModelList = [
            { key: true, hidden: true, headerText: Resources.RowKey, name: 'RowKey', index: 'RowKey', editable: true },
            { key: false, name: 'RowNumber', headerText: Resources.SlNo, index: 'RowNumber', editable: true, cellEdit: true,  resizable: false },
            { key: false, name: 'AccountHeadName', headerText: Resources.AccountHead, index: 'AccountHeadName', editable: true, cellEdit: true, sortable: true, resizable: false, width: 300},
            { key: false, name: 'TotalIncome', headerText: "Total Income", index: 'TotalIncome', editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false, width: 200 },
            { key: false, name: 'Income', headerText: IncomeByDate, index: 'Income', editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false, width: 350 },
            { key: false, name: 'TotalExpense', headerText: "Total Expense", index: 'TotalExpense', editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false, width: 200 },
            { key: false, name: 'Expense', headerText: ExpenseByDate, index: 'Expense', editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false, width: 350 },
            { key: false, name: 'TotalBalance', headerText: "Total Balance", index: 'TotalBalance', editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false, width: 200 },
            { key: false, name: 'Balance', headerText: BalanceByDate, index: 'Balance', editable: true, cellEdit: true, formatter: RupeeIcon, sortable: true, resizable: false, width: 350 },

        ];

        var colNames = colModelList.map(function (item) {
            return item.headerText;
        });
        $("#grid").jqGrid({

            url: $("#hdnGetAccountHeadIncomeExpenseSummary").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonData,
            contenttype: 'application/json; charset=utf-8',
            colNames: colNames,
            colModel: colModelList,
            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: griddatacount,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            width: '100%',
            height: '100%',
            viewrecords: true,
            emptyrecords: Resources.NoRecordsToDisplay,
            jsonReader:
            {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                userdata: "userData",
                repeatitems: false,
                Id: "0"
            },
            multiselect: false,
            loadonce: false,
            sortname: 'RowKey',
            sortorder: 'asc',
            ignoreCase: true,
            altRows: true,
            altclass: 'jqgrid-altrow',
            //rownumbers: true,
            //rownumWidth:50,
            footerrow: true,
            userDataOnFooter: true,
            beforeProcessing: function (data) {
                data.rows = $(data.rows).map(function (n, item) {
                    var obj = {};
                    $(item).each(function () {
                        obj[this.Key] = this.Value;
                    });
                    return obj;
                });
            },
            loadComplete: function (data) {

                StudentSummary.GetCustomizedColumns();

            },
            onPaging: function () {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            }
        })

        //jQuery("#grid").jqGrid('setGroupHeaders', {
        //    useColSpanStyle: true,
        //    groupHeaders: [
        //        { startColumnName: 'TotalIncome', numberOfColumns: 2, titleText: '<em>Income</em>' },
        //        { startColumnName: 'TotalExpense', numberOfColumns: 2, titleText: 'Expense' },
        //        { startColumnName: 'TotalBalance', numberOfColumns: 2, titleText: 'Balance' }
        //    ]
        //});

        GenerateShowHideColumnList(colModelList, json["DefaultColumns"])
        $("#grid").jqGrid("setLabel", 6, "Dynamic Column Name");
        // $("#grid").jqGrid("setLabel", "rn", "Sl.No");




    }


    // AccountHead Income And Expense Summary End




    // Common queries Start

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

    function formatYesorNO(cellValue, option, rowdata, action) {

        if (cellValue == "Yes") {
            return '<i  class="fa fa-check" aria-hidden="true"></i>';
        }
        else {
            return '<i  class="fa fa-times" aria-hidden="true"></i>';
        }
        return cellValue;
    }

    function RupeeIcon(cellValue, option, rowdata, action) {

        var amount = parseFloat(cellValue) ? parseFloat(cellValue) : 0;
        amount = amount.toLocaleString();
        return '<i  class="fa fa-inr" aria-hidden="true"></i>' + amount;
    }

    var getCustomizedColumns = function (obj) {
        var removeCols = ["subgrid", "edit"]
        var colList = $("#grid").jqGrid('getGridParam', 'colModel');
        colList = $(colList).filter(function (n, item) {
            return removeCols.indexOf(item.name) === -1;
        }).map(function (n, item) {
            return item.name;
        });
        var SelectedList = $('#ShowHideColumns').selectpicker('val');
        var CountSelection = SelectedList.length;


        $("#grid").hideCol($(colList).not(SelectedList));
        $("#grid").showCol(SelectedList);


        if (CountSelection <= 6) {
            jQuery("#grid").setGridWidth("1000");
        }
        else {
            jQuery("#grid").setGridWidth(CountSelection * 150);
        }

    }

    function TextHilightwithbadge(cellValue, options, rowdata, action) {

        if (cellValue != 0) {

            return "<span class='badge badge-pill badge-success'>" + cellValue + "</span>"
        }
        else {
            return cellValue
        }


    }

  

    // Common queries End



    return {
        GetStudentSummary: getStudentSummary,
        GetCustomizedColumns: getCustomizedColumns,
        GetExportPrintData: getExportPrintData,
        GetPrintData: getPrintData,
        GetStudentFeeSummary: getStudentFeeSummary,
        GetStudyMaterialIssueSummary: getStudyMaterialIssueSummary,
        GetStudentIdCardIssueSummary: getStudentIdCardIssueSummary,
        GetInternalExamResultSummary: getInternalExamResultSummary,
        GetStudentUniversityFeeSummary: getStudentUniversityFeeSummary,
        GetStudentCertificateSummary: getStudentCertificateSummary,
        GetUniversityCertificateSummary: getUniversityCertificateSummary,
        GetUnitTestExamResultSummary: getUnitTestExamResultSummary,
        GetExamScheduleSummary: getExamScheduleSummary,
        GetTeacherWorkScheduleSummary: getTeacherWorkScheduleSummary,
        GetDayToDayFees: getDayToDayFees,
        GetStudentLateSummary: getStudentLateSummary,
        GetStudentLeaveSummary: getStudentLeaveSummary,
        GetStudentAbscondersSummary: getStudentAbscondersSummary,
        GetStudentEarlyDepartureSummary: getStudentEarlyDepartureSummary,
        GetStudentFeeRefundSummary: getStudentFeeRefundSummary,
        GetExportPrintDynamicData: getExportPrintDynamicData,
        GetCashFlowSummary: getCashFlowSummary,
        GetFeeInstallmentSummary: getFeeInstallmentSummary,
        GetEmployeeSalarySummary: getEmployeeSalarySummary,
        GetAccountHeadIncomeExpenseSummary: getAccountHeadIncomeExpenseSummary
    }

}());



function GenerateShowHideColumnList(data, DefaultColumns) {

    data = data.filter(function (n, p) {
        return !n.hidden
    })
    var ddl = $("#ShowHideColumns");
    $(ddl).html("")
    $(ddl).val('default').selectpicker("refresh");
    //$(ddl).append(
    //    $('<option ' + ("selected=true") + '></option>').val("rn").html("Sl.No"));
    $.each(data, function (i, item) {
        $(ddl).append(
            $('<option ' + (DefaultColumns.indexOf(item.name) > -1 ? "selected=true" : "") + '></option>').val(item.name).html(item.headerText));
    });

    $(ddl).selectpicker("refresh");
}

function workScheduleHistoryCustomPopup(MasterRowKey) {



    var url = $("#hdnGetWorkScheduleTopicHistory").val();
    $.customPopupform.CustomPopup({
        modalsize: "modal-lg",
        ajaxType: "GET",
        ajaxData: { MasterRowKey: MasterRowKey },
        load: function () {
            setTimeout(function () {


            }, 500)
        },

        rebind: function (result) {


        }
    }, url);
}