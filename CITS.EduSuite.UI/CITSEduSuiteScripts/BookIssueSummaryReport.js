var JsonModel = [], request = null;
var GvWidth;
var GbSubGrid = [];
var StudentSummary = (function ()
{
    var getStudentSummary = function (json)
    {
        var CourseKeyList="";
        GbSubGrid = [];
        
        JsonModel = json;      
        JsonModel["AcademicTermKey"] = $("#AcademicTermKey").val();
        JsonModel["CourseTypeKeyList"] = GenerateListBoxString("#CourseTypeKeyMultiSelect");
        JsonModel["CourseKeyList"] = GenerateListBoxString("#CourseKeyMultiSelect");
        JsonModel["UniversityMasterKeyList"] = GenerateListBoxString("#UniversityMasterKeyMultiSelect");
        JsonModel["CourseYearsKeyList"] = GenerateListBoxString("#CourseYearsKeyMultiSelect");
        JsonModel["BatchKeyList"] = GenerateListBoxString("#BatchKeyMultiSelect");
        JsonModel["SecondLanguageKeyList"] = GenerateListBoxString("#SecondLanguageKeyMultiSelect");
        JsonModel["ReligionKeyList"] = GenerateListBoxString("#ReligionKeyMultiSelect");
        JsonModel["ModeKeyList"] = GenerateListBoxString("#ModeKeyMultiSelect");
        JsonModel["ClassModeKeyList"] = GenerateListBoxString("#ClassModeKeyMultiSelect");
        JsonModel["NatureOfEnquiryKeyList"] = GenerateListBoxString("#NatureOfEnquiryKeyMultiSelect");
        JsonModel["BranchKeyList"] = GenerateListBoxString("#BranchKeyMultiSelect");
        JsonModel["AgentKeyList"] = GenerateListBoxString("#AgentKeyMultiSelect");
        JsonModel["StudentStatusKeyList"] = GenerateListBoxString("#StudentStatusKeyMultiSelect");
        JsonModel["MeadiumKeyList"] = GenerateListBoxString("#MeadiumKeyMultiSelect");
        JsonModel["ClassKeyList"] = GenerateListBoxString("#ClassKeyMultiSelect");
        JsonModel["CourseYearsKey"] = GenerateListBoxString("#CourseYearsKeyMultiSelect");
        JsonModel["IncomeGroupKeyList"] = GenerateListBoxString("#IncomeGroupKeyMultiSelect");
        JsonModel["ClassRequiredKey"] = $("#ClassRequiredKey").val();
        JsonModel["GenderKey"] = $("#GenderKey").val();
        JsonModel["DateOfAdmission"] = $("#DateOfAdmission").val();
        JsonModel["DateAddedFrom"] = $("#DateAddedFrom").val();
        JsonModel["DateAddedTo"] = $("#DateAddedTo").val();


        

        $("#grid").jqGrid('setGridParam', { datatype: 'json', postData: JsonModel }).trigger('reloadGrid');


        $("#grid").jqGrid({

            url: $("#hdnGetStudentsSummaryReports").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonModel,
            async: false,
            contenttype: 'application/json; charset=utf-8',


            colNames:
               [Resources.RowKey, Resources.Name, Resources.AdmissionNo, /*Resources.CourseType,*/ Resources.Course,
                Resources.NotAvailable, Resources.Available, Resources.Issued
               /* Resources.AffiliationsTieUps, Resources.AcademicTerm, Resources.Batch, Resources.Mode, Resources.ClassModeName,
                Resources.MobileNo, Resources.Email, Resources.EnrollmentNo, Resources.StudentGuardian,
                Resources.StudentMother, Resources.StudentPermanentAddress, Resources.PresentAddress, Resources.MediumName,
                Resources.StudentStatus, Resources.ClassCode, Resources.Agent, Resources.Income, Resources.DateOfBirth, Resources.Religion,
                Resources.CurrentYear, Resources.SecondLanguage, Resources.AdmissionDate, Resources.ClassRequired, Resources.Gender, Resources.TotalFee*/],
            colModel: [
               { key: true, hidden: true, name: 'RowKey', index: 'RowKey', editable: true },                             
               { key: false, name: 'Name', index: 'StudentName', editable: true, cellEdit: true, sortable: true, resizable: false },
               { key: false, name: 'AdmissionNo', index: 'AdmissionNo', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'CourseType', index: 'CourseTypeName', editable: true, cellEdit: true, sortable: true, resizable: false },
               { key: false, name: 'Course', index: 'CourseName', editable: true, cellEdit: true, sortable: true, resizable: false },
               { key: false, name: 'NotAvailable', index: 'NotAvailable', editable: true, cellEdit: true, sortable: true, resizable: false },
               { key: false, name: 'Available', index: 'Available', editable: true, cellEdit: true, sortable: true, resizable: false },
               { key: false, name: 'Issued', index: 'Issued', editable: true, cellEdit: true, sortable: true, resizable: false }

            //{ key: false, name: 'Affiliations', index: 'UniversityMasterName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'AcademicTerm', index: 'AcademicTermName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Batch', index: 'BatchName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Mode', index: 'ModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'ClassMode', index: 'ClassModeName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Mobile', index: 'StudentEmail', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Email', index: 'StudentMobile', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'EnrollmentNo', index: 'StudentEnrollmentNo', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'GuardianName', index: 'StudentGuardian', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'MotherName', index: 'StudentMotherName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'PermanentAddress', index: 'StudentPermanentAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'PresentAddress', index: 'StudentPresentAddress', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Medium', index: 'MediumName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'StudentStatus', index: 'StudentStatusName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Class', index: 'Class', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Agent', index: 'AgentName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Income', index: 'IncomeName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'DOB', index: 'StudentDOB', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
               //{ key: false, name: 'Religion', index: 'ReligionName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'CurrentYear', index: 'CurrentYear', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'SecondLanguage', index: 'SecondLanguageName', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'DateOfAdmission', index: 'StudentDateOfAdmission', editable: true, cellEdit: true, sortable: true, resizable: false, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
               //{ key: false, name: 'ClassRequired', index: 'StudentClassRequired', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'Gender', index: 'StudentGender', editable: true, cellEdit: true, sortable: true, resizable: false },
               //{ key: false, name: 'TotalFee', index: 'StudentTotalFee', editable: true, cellEdit: true, sortable: true, resizable: false },

            ],

            pager: jQuery('#pager'),
            rowNum: 10,
            rowList: [ 10, 15,50,100],
              hidegrid: false,
            shrinkToFit: true,
            //autoResizing: { minColWidth: 80 },
            width:'100%',
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
            loadComplete: function (data)
            {
                //GvWidth = $(".ui-jqgrid-htable").width();
                StudentSummary.GetCustomizedColumns();
                


            },
            onPaging: function ()
            {
                //var CurrPage = $(".ui-pg-input", $("#pager")).val();
                //var Rows = $(".ui-pg-selbox").val();                
                // $("#grid").setGridParam({ datatype: 'json', page: CurrPage, rows: Rows });                           
            },
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridWidth: 50,
           subGridRowExpanded: showChildGrid,  
            subGridOptions: {
            // load the subgrid data only once
            // and the just show/hide
            reloadOnExpand: false,
            // select the row when the expand column is clicked
            selectOnExpand: true,

            openicon: false

            //url from which subgrid data should be requested
        },





        })

        $("#grid").jqGrid("setLabel", "BranchName", "", "thBranchName");





    }




    function showChildGrid(parentRowID, parentRowKey)
    {
        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";

        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = parentRowKey + ".json";
        //childGridURL = childGridURL + "&parentRowID=" + encodeURIComponent(parentRowKey)

        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table colspan="15" id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: $("#hdnGetBookDetailsById").val() + "/" + parentRowKey,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel:[
                { label: "Book Code", name: 'BookCode', index: 'BookCode', width: 100 },
                { label: "Subject Name", name: 'SubjectName', width: 100 },
                { label: "Year", name: 'Year', width: 100 },
                { label: "Status", name: 'AvailableCount', width: 100,formatter:GetBookStatus },
            ],
            loadonce: true,
            height: '100%',
            width: $(".ui-jqgrid-htable").width(),
            pager: "#" + childGridPagerID,
            footer: false,
              hidegrid: false,
            shrinkToFit: true,
            loadComplete: function (data)
            {
                $(".subgrid-cell").remove();
                $(".ubgrid-data .ui-jqgrid-pager").remove();
   
            }
        });


     

    }


    function GetBookStatus(cellValue, options, rowdata, action)
    {
        if(rowdata.AvailableCount==false)
        {
            return "Not Available";
        }
        else if(rowdata.Issued==true)
        {
            return "Issued"
        }
        else
        {
            return "Not Issued"
        }
    }



    var getCustomizedColumns = function (obj)
    {
   
        var CountSelection = $('#Columns :selected').length;

        $('#Columns option').each(function (index, value)
        {
            if (this.selected == true)
            {
                    $("#grid").showCol(this.value);
                }
            else
            {             
                    $("#grid").hideCol(this.value);
                }

            });       
       
        //if (CountSelection <= 6) {
        //    jQuery("#grid").setGridWidth("1000");
        //}
        //else {
        //    jQuery("#grid").setGridWidth(CountSelection * 150);
        //}
       
    }

    var getExcelImportData = function ()
    {
        Columns = []
        $('#Columns option').each(function (index, value)
        {
            if (this.selected == true)
            {
                Columns.push(this.value);
            }
        });


        var CourseKeyList = "";


        $("#CourseKeyMultiSelect").each(function ()
        {
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
            success: function (result)
            {                
                AppCommon.JsonToExcel(result, Columns);
            }
        });

      
     
    }

    var getPrintData = function ()
    {
      
        Columns = []
        $('#Columns option').each(function (index, value) {
            if (this.selected == true)
            {
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
            success: function (result)
            {
                AppCommon.JsonToPrint(result, Columns);
            }
        });


        
    }
 

   
    function editLink(cellValue, options, rowdata, action)
    {
        var temp = "'" + rowdata.RowKey + "'";
        GbSubGrid.push(rowdata);
        return rowdata.Issued
    }

    return {
        GetStudentSummary: getStudentSummary,
        GetCustomizedColumns: getCustomizedColumns,
        GetExcelImportData: getExcelImportData,
        GetPrintData: getPrintData,
       
    
    }

}());

