var JsonModel = [], request = null;

function ShowHideEmptyRecords()
{
    for(i=0;i<100;i++)
    {
        var CellValue = $('#grid').jqGrid('getCell', i, '9')               
        if ($("#ShowEmptyRecord").is(':checked'))
        {
            if (CellValue == 0) {
                $("#" + i).css("display", "none");
            }                      
        }
        else
        {
            $("#" + i).removeAttr("style");
        }
    }

    //$("#grid").jqGrid('setGridParam', { datatype: 'json', postData: JsonModel }).trigger('reloadGrid');
}

function FillDate(parentId) {
    var StartDay = "01/";
    var Month = parseInt($(parentId+" .MonthSelect").prop('selectedIndex')) + parseInt(1) + "/";
    var EndDay = $(parentId+" .MonthSelect").val() + "/";
    var Year = $(parentId+" .YearSelect").val();

    $("#SearchFromDate").val(StartDay + Month + Year);
    $("#SearchToDate").val(EndDay + Month + Year);

    //$(".YearSelect").prop('selectedIndex', $(".YearSelect").prop('selectedIndex'));
}


var date_diff_indays = function (date1, date2) {
    var today = date1;
    today = new Date(today.split('/')[2],today.split('/')[1]-1,today.split('/')[0]);
    var date2 = date2;
    date2 = new Date(date2.split('/')[2],date2.split('/')[1]-1,date2.split('/')[0]);
    var timeDiff = Math.abs(date2.getTime() - today.getTime());
    var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24)); 

    return diffDays;
}



//Enquiry Counselling 

function FillTodaysCounselling() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling"); 
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });



    //$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);

    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(2);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());

    $("#ReportDescriptions").html("Counselling Scheduled to Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");
 
    CallReports.GetApplication(jsonData);
}

function FillTomorrowsCounselling()
{
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {
       
        if ($(this).val() == 2)
        {
            
            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }

     
    });


    //$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#SearchFromDate").val($("#DateTomorrow").val());
    $("#SearchToDate").val($("#DateTomorrow").val());
    $("#SearchDateTypeKey").val(2);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());
    
    $("#ReportDescriptions").html("Counselling Scheduled to Date  between <span id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillYesterdayCounselling() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterdays <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });

    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    //$('#ScheduleTypeKeys').multiselect('refresh');

    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(2);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());

    $("#ReportDescriptions").html("Counselling Scheduled to Date  between <span id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillPendingCounselling() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });

    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    //$('#ScheduleTypeKeys').multiselect('refresh');

    //$("#SearchFromDate").val($("#MonthStartDate").val());
    $("#SearchFromDate").val($("").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(2);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());

    $("#ReportDescriptions").html("Counselling Scheduled to Date  between <span id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillUpcomingCounselling() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);

    //$('#ScheduleTypeKeys').multiselect('refresh');

    $("#SearchFromDate").val($("#DateTomorrow").val());
    $("#SearchToDate").val("");
    $("#SearchDateTypeKey").val(2);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());

    $("#ReportDescriptions").html("Counselling Scheduled to Date  between <span id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillEnquiryRescheduledCounselling(dateValue)
{

    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Rescheduled <i class='fa fa-hand-o-right' aria-hidden='true'></i> " + dateValue+" Days");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    //$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    //$("#ReportOnCallStatusVise").prop("checked", true);
  


    $("#SearchFromDate").val($(dateValue).val());

    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillEnquiryMonthlyCounsellingByCounsellingDateViseReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    //$("#ReminderActive").prop("checked", true);
    //$('#ScheduleTypeKeys').multiselect('refresh');

    FillDate("#menu_enquirycounselling");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(2);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());

    $("#ReportDescriptions").html("Enquiry   Counselling Report- Employee called for Counselling  Scheduled Date  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");


    CallReports.GetApplication(jsonData);
}

function FillEnquiryMonthlyCounsellingByCalledDateViseReport()
{
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#ReminderActive").prop("checked", true);
    //$('#ScheduleTypeKeys').multiselect('refresh');

    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    FillDate("#menu_enquirycounselling");
    $("#SearchDateTypeKey").val(1);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());

    $("#ReportDescriptions").html("Enquiry   Counselling Report- Employee called for Counselling Called  Date  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");


    CallReports.GetApplication(jsonData);
}

//Enquiry Counselling  Called

function FillTodaysCounsellingCalled() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });

    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    //$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ReminderActive").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());

    $("#ReportDescriptions").html("Counselling Scheduled to Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");
    CallReports.GetApplication(jsonData);
}
function FillYesterdayCounsellingCalled()
{
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterdays <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called");
    ResetSearch(false);
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });

    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ReminderActive").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#SearchCallStatusKey").val($("#hdnCounsellingCallStatusKey").val());

    $("#ReportDescriptions").html("Counselling Scheduled to Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");
    CallReports.GetApplication(jsonData);
}

//Added Enquiry
function FillTodaysAddedEnquiryReport()
{
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Added Enquiry");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(6);
    $("#EmployeeFilterTypeKey").val("1");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysAddedEnquiryReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Added Enquiry");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(6);
    $("#EmployeeFilterTypeKey").val("1");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillMonthlyAddedEnquiryReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Added Enquiry");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    FillDate("#menu_AddedToenquiry");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    //$("#SearchFromDate").val($("#DateYesterday").val());
    //$("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(6);
    $("#EmployeeFilterTypeKey").val("1");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

//New Lead
function FillTodaysLeadReport() {
    ResetSearch(false);

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("1");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysLeadReport() {
    ResetSearch(false);

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("1");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}


//NewEnquiry
function FillTodaysEnquiryReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> New Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays New Enquiry ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("1");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysEnquiryReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> New Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterdays New Enquiry ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("1");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillMonthlyEnquiryReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> New Enquiry<i class='fa fa-hand-o-right' aria-hidden='true'></i>Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i>  New Enquiries ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    FillDate("#menu_newenquiry");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    //$("#SearchFromDate").val($("#DateYesterday").val());
    //$("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("1");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

//callReport Today
function FillTodaysCallReport()
{
    ResetSearch(false);
    $("#SearchFromDate").val($("#DateToday").val());

    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}


function FillTodaysEnquiryEnquiryLeadCallReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry/EnquiryLead Call History");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2 || $(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysEnquiryLeadCallReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead Call History");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysEnquiryCallReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry Call History");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysApplicationCallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Application Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysDocumentsCallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Documents Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 4) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysOfferLetterCallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Offer Letter Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 7) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysVisaCallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Visa Documentation Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 8) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysVisa2CallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Visa Documentation Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 10) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysTravelArragementsCallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Travel Arragements Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 11) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

//callReport Yesterday
function FillYesterdaysEnquiryEnquiryLeadCallReport(data) {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry EnquiryLead Call History");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2 || $(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });

    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysEnquiryLeadCallReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead Call History");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 1) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });

    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysEnquiryCallReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry Call History");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 2) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ShowSpamed,#ShowClosePending").prop("checked", false);
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysApplicationCallReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Application Call History");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysDocumentsCallReport() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Documents Call History");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 4) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysOfferLetterCallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterdays <i class='fa fa-hand-o-right' aria-hidden='true'></i> Offer Letter Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 7) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysVisaCallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterdays <i class='fa fa-hand-o-right' aria-hidden='true'></i> Visa Documentation Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 8) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysVisa2CallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterdays <i class='fa fa-hand-o-right' aria-hidden='true'></i> Visa Documentation Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 10) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysTravelArragementsCallReport() {
    ResetSearch(false);

    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Call History <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterdays <i class='fa fa-hand-o-right' aria-hidden='true'></i> Travel Arragements Call History");

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 11) {

            $(this).prop("selected", true);
            $("#TypeEnquiry").prop("checked", true);
        }


    });
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#ReportOnCallStatusVise_section").removeAttr("style");
    ////$('#ScheduleTypeKeys').multiselect('refresh');
    //$("#ReportOnCallStatusVise").prop("checked", true);
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(1);
    $("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Call Report from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

//Schedule
function FillTomorrowsCallScheduleReport()
{
    ResetSearch(false);
    $("#SearchFromDate").val($("#DateTomorrow").val());
    $("#SearchToDate").val($("#DateTomorrow").val());
    $("#SearchDateTypeKey").val(2);
    //$("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillTodaysCallScheduleReport() {
    ResetSearch(false);
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(2);
    //$("#EmployeeFilterTypeKey").val("3");
    $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}



//Menu1 Enquiry/EnquiryLead
    function FillEnquiryEnquiryLeadTodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry/EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2 || $(this).val() == 1) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });

        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillEnquiryEnquiryLeadTomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry/EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2 || $(this).val() == 1) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ReminderActive").prop("checked", true);
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillEnquiryEnquiryLeadPendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry/EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2 || $(this).val() == 1) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ReminderActive").prop("checked", true);
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillEnquiryEnquiryLeadUpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry/EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2 || $(this).val() == 1) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ReminderActive").prop("checked", true);
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());
       
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

//Menu2 EnquiryLead
    function FillEnquiryLeadTodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 1) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillEnquiryLeadTomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 1) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        $("#ReminderActive").prop("checked", true);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }
    function FillEnquiryLeadPendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 1) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        $("#ReminderActive").prop("checked", true);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillEnquiryLeadUpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> EnquiryLead <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 1) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        $("#ReminderActive").prop("checked", true);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());

        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }


    //Menu2 Enquiry
    function FillEnquiryTodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillEnquiryTomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        $("#ReminderActive").prop("checked", true);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillEnquiryPendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillEnquiryUpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ShowSpamed,#ShowClosePending").prop("checked", false);
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());

        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    //Menu2 Application
    function FillApplicationTodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 3) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillApplicationTomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 3) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }
    function FillApplicationPendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 3) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillApplicationUpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 3) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());

        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }


//Menu 3 Documents
    function FillDocumentsTodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Documents <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 4) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillDocumentsTomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Documents <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 4) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }
    function FillDocumentsPendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Documents <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 4) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillDocumentsUpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Documents <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 4) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());

        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }



    //Menu 4 OfferLetter
    function FillOfferLetterTodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Offer Letter <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 7) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillOfferLetterTomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Offer Letter <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 7) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }
    function FillOfferLetterPendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Offer Letter <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 7) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillOfferLetterUpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Offer Letter <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 7) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());

        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }


    //Menu 5 Visa
    function FillVisaTodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Visa Documentation <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 8) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillVisaTomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Visa Documentation  <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 8) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }
    function FillVisaPendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Visa Documentation  <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 8) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillVisaUpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Visa Documentation  <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 8) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());

        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }



    //Menu 6 Visa2
    function FillVisa2TodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i> Visa Documentation <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 10) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillVisa2TomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Visa Documentation  <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 10) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }
    function FillVisa2PendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Visa Documentation  <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 10) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillVisa2UpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Visa Documentation  <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 10) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());

        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }


    //Menu 6 Travel Arragements
    function FillTravelArragementsTodaysCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Travel Arragements <i class='fa fa-hand-o-right' aria-hidden='true'></i> Todays Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 11) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateToday").val());
        $("#SearchToDate").val($("#DateToday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillTravelArragementsTomorrowsCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Travel Arragements <i class='fa fa-hand-o-right' aria-hidden='true'></i> Tomorrows Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 11) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val($("#DateTomorrow").val());
        $("#SearchToDate").val($("#DateTomorrow").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }
    function FillTravelArragementsPendingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Travel Arragements <i class='fa fa-hand-o-right' aria-hidden='true'></i> Pending Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 11) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        ////$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchFromDate").val("");
        $("#SearchToDate").val($("#DateYesterday").val());
        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }

    function FillTravelArragementsUpcomingCallScheduleReport() {
        ResetSearch(false);
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Schedule <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Travel Arragements <i class='fa fa-hand-o-right' aria-hidden='true'></i> Upcoming Schedule");
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 11) {

                $(this).prop("selected", true);
                $("#TypeEnquiry").prop("checked", true);
            }


        });
        $("#ApplicationSubFilter_section").removeAttr("style");
        //$('#ScheduleTypeKeys').multiselect('refresh');
        $("#ReminderActive").prop("checked", true);
        $("#SearchToDate").val("");
        $("#SearchFromDate").val($("#DateUpcoming").val());

        $("#SearchDateTypeKey").val(2);
        //$("#EmployeeFilterTypeKey").val("3");
        $("#ReportDescriptions").html("Report of Call Scheduled  to  Date between <span  id='spanFromDate' class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

        CallReports.GetApplication(jsonData);
    }



    


//Application Counselling monthly vise
function FillApplicationCounsellingByCreatedDate()
{
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> All Applications");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    FillDate("#menu_application");
  
    $("#ApplicationSubFilter_section").removeAttr("style");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(3);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillApplicationCounsellingByCounsellingDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    FillDate("#menu_application");
    $("#ApplicationSubFilter_section").removeAttr("style");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(4);

    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where Counselling Scheduled to date from Enquiry  between <span  id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillApplicationCounsellingByCalledDate() {
    ResetSearch(false);

    $('#ScheduleTypeKeys > option').each(function () {
        $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called ");
        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    FillDate("#menu_application");
    $("#ApplicationSubFilter_section").removeAttr("style");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(5);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillApplicationCounsellingByFetchDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Counselled");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    FillDate("#menu_application");
    $("#ApplicationSubFilter_section").removeAttr("style");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(7);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}


//Application Scheduled monthly vise 
function FillApplicationScheduleViseByCreatedDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> All Applications ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {
            $("#TypeApplication").prop("checked", true);
            $(this).prop("selected", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    FillDate("#menu_application");
    $("#ApplicationSubFilter_section").removeAttr("style");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("2");
    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillApplicationScheduledByCalledDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    FillDate("#menu_application");
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#EmployeeFilterTypeKey").val("2");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(5);

    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillApplicationScheduleViseByCounsellingDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    FillDate("#menu_application");
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#EmployeeFilterTypeKey").val("2");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(4);


    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on where Counselling Scheduled to date from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillApplicationScheduleViseByFetchDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Monthly <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Counselled ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ShowRefund").prop("checked", false);
    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#EmployeeFilterTypeKey").val("2");
    FillDate("#menu_application");
    //$("#SearchFromDate").val($("#MonthStartDate").val());
    //$("#SearchToDate").val($("#MonthEndDate").val());
    $("#SearchDateTypeKey").val(7);


    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on where Counselling Scheduled to date from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}


//Application Counselling Today vise
function FillTodaysApplicationCounsellingByCreatedDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> All Applications ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });



    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(3);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillTodaysApplicationCounsellingByCounsellingDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(4);

    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where Counselling Scheduled to date from Enquiry  between <span  id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillTodaysApplicationCounsellingByCalledDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(5);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillTodaysApplicationCounsellingByFetchDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Counselled ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(7);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}


//Application Scheduled Today vise 

function FillTodaysApplicationScheduleViseByCreatedDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> All Applications ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {
            $("#TypeApplication").prop("checked", true);
            $(this).prop("selected", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("2");
    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillTodaysApplicationScheduledByCalledDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#EmployeeFilterTypeKey").val("2");
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(5);

    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillTodaysApplicationScheduleViseByCounsellingDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#EmployeeFilterTypeKey").val("2");
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(4);


    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on where Counselling Scheduled to date from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillTodaysApplicationScheduleViseByFetchDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Today <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Counselled ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {
            $("#TypeApplication").prop("checked", true);
            $(this).prop("selected", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());
    $("#SearchDateTypeKey").val(7);
    $("#EmployeeFilterTypeKey").val("2");
    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}




//Application Counselling Yesterday vise
function FillYesterdaysApplicationCounsellingByCreatedDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> All Applications ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });




    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(3);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillYesterdaysApplicationCounsellingByCounsellingDate() {
    ResetSearch(false);
     $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(4);

    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where Counselling Scheduled to date from Enquiry  between <span  id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillYesterdaysApplicationCounsellingByCalledDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(5);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}
function FillYesterdaysApplicationCounsellingByFetchDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Counselled ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(7);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}


//Application Scheduled Yesterday vise 

function FillYesterdaysApplicationScheduleViseByCreatedDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> All Applications ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {
            $("#TypeApplication").prop("checked", true);
            $(this).prop("selected", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("2");
    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillYesterdaysApplicationScheduledByCalledDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling Called ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#EmployeeFilterTypeKey").val("2");
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(5);

    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillYesterdaysApplicationScheduleViseByCounsellingDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i> Enquiry <i class='fa fa-hand-o-right' aria-hidden='true'></i> Counselling ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#EmployeeFilterTypeKey").val("2");
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(4);


    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on where Counselling Scheduled to date from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillYesterdaysApplicationScheduleViseByFetchDate() {
    ResetSearch(false);
    $("#ReportPath").html("<i class='fa fa-hand-o-right' aria-hidden='true'></i> Application <i class='fa fa-hand-o-right' aria-hidden='true'></i> Yesterday <i class='fa fa-hand-o-right' aria-hidden='true'></i> Scheduled Vise <i class='fa fa-hand-o-right' aria-hidden='true'></i>  Counselled ");
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {
            $("#TypeApplication").prop("checked", true);
            $(this).prop("selected", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("#DateYesterday").val());
    $("#SearchToDate").val($("#DateYesterday").val());
    $("#SearchDateTypeKey").val(7);
    $("#EmployeeFilterTypeKey").val("2");
    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}



//All Applications Counselling vise

function FillAllApplicationCounsellingByCreatedDate() {
    ResetSearch(false);

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });




    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("").val());
    $("#SearchToDate").val($("").val());
    $("#SearchDateTypeKey").val(3);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillAllApplicationCounsellingByFetchDate() {
    ResetSearch(false);

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });


    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("").val());
    $("#SearchToDate").val($("").val());
    $("#SearchDateTypeKey").val(7);
    $("#ReportDescriptions").html("Application Counselling vise Report from application based on where called date for Counselling from Enquiry  between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}


//All Applications Scheduled vise

function FillAllApplicationScheduleViseByCreatedDate() {
    ResetSearch(false);

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {
            $("#TypeApplication").prop("checked", true);
            $(this).prop("selected", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("").val());
    $("#SearchToDate").val($("").val());
    $("#SearchDateTypeKey").val(3);
    $("#EmployeeFilterTypeKey").val("2");
    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}

function FillAllApplicationScheduleViseByFetchDate() {
    ResetSearch(false);

    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {
            $("#TypeApplication").prop("checked", true);
            $(this).prop("selected", true);
            //$('#ScheduleTypeKeys').multiselect('refresh');
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    $("#SearchFromDate").val($("").val());
    $("#SearchToDate").val($("").val());
    $("#SearchDateTypeKey").val(7);
    $("#EmployeeFilterTypeKey").val("2");
    $("#ReportDescriptions").html("Application Scheduled vise Report from application based on Application Fetch date from Enquiry from Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}


//Application Counselled
function FillTodayCounselled() {
    ResetSearch(false);
    $('#ScheduleTypeKeys > option').each(function () {

        if ($(this).val() == 3) {

            $(this).prop("selected", true);
            $("#TypeApplication").prop("checked", true);
        }
    });

    $("#ApplicationSubFilter_section").removeAttr("style");
    //$('#ScheduleTypeKeys').multiselect('refresh');

    $("#SearchDateTypeKey").val(3);

    $("#SearchFromDate").val($("#DateToday").val());
    $("#SearchToDate").val($("#DateToday").val());

    $("#ReportDescriptions").html("Counselled Report from Application Date between <span id='spanFromDate'  class='red'> " + $("#SearchFromDate").val() + " </span> and  <span  id='spanToDate' class='red'>" + $("#SearchToDate").val() + "</span>");

    CallReports.GetApplication(jsonData);
}







function LeadClick(_this)
{

    if ($(_this).is(':checked'))
    {
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 1)
            {
                
                
                $(this).prop("selected", true);
               
            }
        });
    }
    else
    {
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 1) {

                $(this).prop("selected", false);
               
            }
        });
    }

    //$('#ScheduleTypeKeys').multiselect('refresh');
}


function EnquiryClick(_this)
{
    if ($(_this).is(':checked')) {
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 2) {

                $(this).prop("selected", true);

            }
        });
    }
    else {
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() ==2) {

                $(this).prop("selected", false);

            }
        });
    }

    //$('#ScheduleTypeKeys').multiselect('refresh');
}


function ApplicationClick(_this)
{
    if ($(_this).is(':checked')) {
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 3) {

                $(this).prop("selected", true);

            }
        });
    }
    else {
        $('#ScheduleTypeKeys > option').each(function () {

            if ($(this).val() == 3) {

                $(this).prop("selected", false);

            }
        });
    }

    //$('#ScheduleTypeKeys').multiselect('refresh');
}

function ShowApplicationRefresh(_this)
{       
        //$("#BtnApplicationCounsellingVise").trigger("click");    
}



function ResetSearch(IsResetScheduleType)
{
    $("#ReminderActive").prop("checked", false);

   
    if (IsResetScheduleType == false)
    {
        $('#ScheduleTypeKeys > option').each(function ()
        {
            $(this).prop("selected", false);
            //$('#ScheduleTypeKeys').multiselect('refresh');

        });
    }

    //$("#ReportOnCallStatusVise_section").css("display", "none");
    $("#ApplicationSubFilter_section").css("display", "none");
    
    $("#TypeApplication").prop("checked", false);
    $("#TypeEnquiry").prop("checked", false);
    $("#TypeEnquiryLead").prop("checked", false);
    $("#SearchBranchKey").val("");
    $("#SearchCounselllingBranchKey").val("");
    $("#SearchApplicationStatusKey").val("");
    $("#SearchCallTypeKey").val("");
    $("#EmployeeFilterTypeKey").val("1");

    $("#SearchCallStatusKey").val("");
}

function IsReset()
{

    var isApplicationSelected=false;
    $('#ScheduleTypeKeys > option').each(function ()
    {
        if ($(this).val() == 3 && $(this).prop("selected")==true)
        {
            isApplicationSelected = true;
        }        
    });

    if(isApplicationSelected==false)
    {
        $("#ShowRefund").prop("checked", false);
    }
    

}





var CallReports = (function ()
{
    var getApplication = function (json)
    {
        
        if (date_diff_indays($("#SearchFromDate").val(), $("#SearchToDate").val()) > 30) {
            alert("FromDate and ToDate between cannot Longer than 30 days");
            return false;
        }

         TotalFollowUpCount = 0;
         TotalIntrestedCount = 0;
         TotalAdmissionTakenCount = 0;
         TotalClosedCount = 0;
         TotalCount = 0;
         TotalProductiveCallsCount = 0;
         TotalRefundCount = 0;
         TotalReScheduledCount = 0;
         TotalCallsCount = 0;
         IsReset();

        
        JsonModel = json;
        JsonModel["SearchBranchKey"] = $("#SearchBranchKey").val();
        JsonModel["SearchEmployeeKey"] = $("#SearchEmployeeKey").val();
        JsonModel["SearchScheduledEmployeeKey"] = $("#SearchScheduledEmployeeKey").val();
        JsonModel["SearchCountryKey"] = $("#SearchCountryKey").val();
        JsonModel["SearchInTakeKey"] = $("#SearchInTakeKey").val();
        //JsonModel["SearchApplicationStatusKey"] = $("#SearchApplicationStatusKey").val();
        JsonModel["SearchLocation"] = $("#SearchLocation").val();
        JsonModel["ScheduleTypeKey"] = $("#ScheduleTypeKey").val();
        JsonModel["SearchAnyText"] = $("#SearchAnyText").val();
        JsonModel["SearchFromDate"] = $("#SearchFromDate").val();
        JsonModel["SearchToDate"] = $("#SearchToDate").val();
        JsonModel["SearchCallStatusKey"] = $("#SearchCallStatusKey").val();
        JsonModel["SearchDateTypeKey"] = $("#SearchDateTypeKey").val();
        JsonModel["SearchCallTypeKey"] = $("#SearchCallTypeKey").val();
        JsonModel["SearchCounselllingBranchKey"] = $("#SearchCounselllingBranchKey").val();
        JsonModel["EmployeeFilterTypeKey"] = $("#EmployeeFilterTypeKey").val();
        JsonModel["SearchServiceTypeKey"] = $("#SearchServiceTypeKey").val();
        JsonModel["SearchSubEnquiryStatusKey"] = $("#SearchSubEnquiryStatusKey").val();
          
        $("#spanFromDate").html($("#SearchFromDate").val());
        $("#spanToDate").html($("#SearchToDate").val());
        
        JsonModel["ScheduleTypeKeysList"] = $("#ScheduleTypeKeys option:selected").map(function () {
            return $(this).val();
        }).get().join(',');
    
        JsonModel["ApplicationStatusKeysList"] = $("#ApplicationStatusKeys option:selected").map(function () {
            return $(this).val();
        }).get().join(',');

        
        if ($("#ReminderActive").is(':checked'))
        {
            JsonModel["ReminderStatusKey"] = 1;
        }
        else
        {
            JsonModel["ReminderStatusKey"] = 0;
        }

        if($("#ShowClosePending").is(':checked')==true)
        {
            JsonModel["SearchIsClosePending"] =1;
        }
        else
        {
            JsonModel["SearchIsClosePending"] = 0;
        }


        if ($("#ShowSpamed").is(':checked') == true)
        {
            JsonModel["SearchIsSpamed"] = 1;
        }
        else
        {
            JsonModel["SearchIsSpamed"] = 0;
        }


        if ($("#ShowRefund").is(':checked') == true) {
            JsonModel["SearchIsRefund"] = 1;
        }
        else {
            JsonModel["SearchIsRefund"] = 0;
        }

        


        if ($("#ReportOnCallStatusVise").is(':checked')) {
            JsonModel["SearchIsOnCallStatusVise"] = 1;
        }
        else {
            JsonModel["SearchIsOnCallStatusVise"] = 0;
        }

        $(".lblDate").remove();
        $("#ReportPath").append(" <span class='lblDate'> Date from " + $("#SearchFromDate").val() + " to " + $("#SearchToDate").val() + "</span>");

        $("#grid").jqGrid('setGridParam', { datatype: 'json', postData: JsonModel }).trigger('reloadGrid');



        $("#grid").jqGrid({
            url: $("#hdnGetCallReports").val(),
            datatype: 'json',
            mtype: 'Post',
            postData: JsonModel,
            async: false,
            contenttype: 'application/json; charset=utf-8',
            //TotalProductiveCallsCount
            colNames: [Resources.Employee, "Follow Up", "Intrested", "Admission Taken", "Closed", "Σ Total No of Records", /*Resources.TotalRecords*/ "Repeated Calls", "Total Calls", "Productive Calls", "", ""],
            colModel: [
               
                { key: false, name: 'EmployeeName', index: 'EmployeeName', editable: true, cellEdit: true, sortable: true, resizable: false },               
                { key: false, name: 'TotalFollowUpCount', index: 'TotalFollowUpCount', editable: true,sorttype:"number", cellEdit: true, sortable: true, resizable: false, formatter: FC },
                { key: false, name: 'TotalIntrestedCount', index: 'TotalIntrestedCount', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: IC },
                { key: false, name: 'TotalAdmissionTakenCount', index: 'TotalAdmissionTakenCount', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: AC },
                { key: false, name: 'TotalClosedCount', index: 'TotalClosedCount', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: CC },
                { key: false, name: 'TotalEnquiryCount', index: 'TotalEnquiryCount', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: TR },
                { key: false, name: 'TotalCallsCount', index: 'TotalRepeatedCallsCount', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: RS },
                { key: false, name: 'TotalCallsCount', index: 'TotalCallsCount', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: TC },
                { key: false, name: 'TotalProductiveCallsCount', index: 'TotalProductiveCallsCount', editable: true, sorttype: "number", cellEdit: true, sortable: true, resizable: false, formatter: PC },
                //{ key: false, name: 'TotalRefundCount', index: 'TotalRefundCount', editable: true, cellEdit: true, sorttype: "number", sortable: true, resizable: false, formatter: RF },
                { key: false, name: 'TotalCallsCount', index: 'TotalCallsCount', hidden: true, editable: true, cellEdit: true, sorttype: "number", sortable: false, resizable: false },
                { key: false, name: 'EmployeeKey', index: 'EmployeeKey', hidden: true, editable: true, cellEdit: true, sorttype: "number", sortable: false, resizable: false },
            ],
            pager: jQuery('#pager'),
            rowNum: 200,
            rowList: [200],
            autowidth: true,
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
            sortname: 'TotalFollowUpCount',
            sortorder: 'desc',
            altRows: true,
            altclass: 'jqgrid-altrow',
            gridComplete: function ()
            {
                
                $("#TotalFollowUpCount").html(TotalFollowUpCount);
                $("#TotalIntrestedCount").html(TotalIntrestedCount);
                $("#TotalAdmissionTakenCount").html(TotalAdmissionTakenCount);
                $("#TotalClosedCount").html(TotalClosedCount);
                $("#TotalCount").html(TotalCount);
                $("#TotalProductiveCallsCount").html(TotalProductiveCallsCount);
                $("#TotalRefundCount").html(TotalRefundCount);
                $("#TotalReScheduledCount").html(TotalReScheduledCount);
                $("#TotalCallsCount").html(TotalCallsCount);
                

                TotalFollowUpCount = 0;
                TotalIntrestedCount = 0;
                TotalAdmissionTakenCount = 0;
                TotalClosedCount = 0;
                TotalCount = 0;
                TotalProductiveCallsCount = 0;
                TotalRefundCount = 0;
                TotalReScheduledCount = 0;
                TotalCallsCount = 0;

                ShowHideEmptyRecords();

              
            }
        });

        $("#grid").jqGrid("setLabel", "ApplicationName", "", "thApplicationName");
    }


    var getCallReportDetailed = function (_this, pageIndex, resetPagination)
    {
        var URL = $("#hdnGetCallReportsDetailed").val();
        

            $.customPopupform.CustomPopup({
                modalsize: "modal-xl  mw-100 w-100",
                ajaxType: "Post",
                load: function ()
                {

                   
                    CallReports.GetCallReportDetailedData(_this, pageIndex, resetPagination);
                    
                },


            }, URL);
        
    }

    var getCallReportDetailedData = function (_this, pageIndex, resetPagination)
    {
        $(".modal-body").mLoading();
        var URL = $("#hdnGetCallReportsData").val();


        JsonModel["SearchEnquiryStatusKey"] = $(_this).attr("data-val");

        //JsonModel["SearchScheduledEmployeeKey"] = "";
        
        if ($("#EmployeeFilterTypeKey").val() == 1 || $("#EmployeeFilterTypeKey").val() == 3)
        {
            var EmployeeKey = $(_this).attr("employeekey");
            if (EmployeeKey != "")
            {
                JsonModel["SearchEmployeeKey"] = EmployeeKey;
            }
            else
            {
                JsonModel["SearchEmployeeKey"] = $("#SearchEmployeeKey").val();
            }
        }
        else
        {

            var ScheduledEmployeeKey = $(_this).attr("employeekey");
            if (ScheduledEmployeeKey != "") {
                JsonModel["SearchScheduledEmployeeKey"] = ScheduledEmployeeKey;
            }
            else {
                JsonModel["SearchScheduledEmployeeKey"] = $("#SearchScheduledEmployeeKey").val();
            }
            //if (JsonModel["SearchScheduledEmployeeKey"] == null || JsonModel["SearchScheduledEmployeeKey"]=="")
            //{
            //    var ScheduledEmployeeKey = $(_this).attr("employeekey");
            //    if (ScheduledEmployeeKey != "") {
            //        JsonModel["SearchScheduledEmployeeKey"] = ScheduledEmployeeKey;
            //    }
            //    else
            //    {
            //        JsonModel["SearchScheduledEmployeeKey"]=  $("#SearchScheduledEmployeeKey").val();
            //    }
            //}
        }
       



        JsonModel["PageIndex"] = pageIndex ? pageIndex : 1;
        JsonModel["PageSize"] = 10;


        request = $.ajax({
            url: URL,
            data: JsonModel,
            datatype: "json",
            type: "post",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            beforeSend: function () {
                if (request != null) {
                    request.abort();
                }
            },
            success: function (data)
            {
                $("#reportData").html(data);

                //JsonModel["SearchEmployeeKey"] = "";
                //JsonModel["SearchScheduledEmployeeKey"] = "";
                //$("#TotalRecords").html($("#hdnTotalRecords").val());
                if (resetPagination)
                {
                    CallReportPagination(_this);
                }
                $(".modal-body").mLoading("destroy");
            },
            error: function (xhr) {
                console.log(xhr.responseText);
                //  $("#dvScheduleContainer").mLoading("destroy");
            }
        });



    }

   


    function editLink(cellValue, options, rowdata, action) {
        var temp = "'" + rowdata.RowKey + "'";
        return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-xs context-menu"><i class="fa fa-bars" />' + Resources.Action + '</a></div>'
        //return '<div class="divEditDelete"><a class="btn btn-outline-primary btn-xs" href="AddEditApplication' + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-pencil" aria-hidden="true"></i>' + Resources.Edit + '</a>' + '<a class="btn btn-outline-danger btn-xs"  onclick="javascript:deleteApplication(' + temp + ');return false;" >  <i class="fa fa-trash pointer" aria-hidden="true"></i> ' + Resources.Delete + ' </a><a class="btn btn-warning btn-xs" href="' + $("#hdnViewApplication").val() + AppCommon.EncodeQueryString("id=" + rowdata.RowKey) + '"><i class="fa fa-eye" aria-hidden="true"></i>' + Resources.View + '</a></div>';
    }

    var getEmployeesByBranchId = function (Id, ddl, ddl2)
    {
        $(ddl).html("");
        $(ddl2).html("");

  
          
        $(ddl).append($('<option></option>').val("").html(Resources.BlankSpace + Resources.CounsellingEmployee));
        $(ddl).select2("val", "");

        $(ddl2).append($('<option></option>').val("").html(Resources.BlankSpace + Resources.ScheduledEmployee));
        $(ddl2).select2("val", "");

                         

        var response = AjaxHelper.ajax("GET", $("#hdnGetEmployeesByBranchId").val() + "/" + Id);
        var $optgroup;
        $.each(response, function (i, Employee)
        {

            //if (response[i - 1] == undefined || Employee.GroupName != response[i - 1]["GroupName"]) {
            //    $optgroup = "";
            //    $optgroup = $("<optgroup>", {
            //        label: Employee.GroupName
            //    });

            //    $optgroup.appendTo(ddl);
            //}
            var $option = $("<option>", {
                text: Employee.Text,
                value: Employee.RowKey
            });

            $option.appendTo(ddl);
         

        });

        $.each(response, function (i, Employee) {

            //if (response[i - 1] == undefined || Employee.GroupName != response[i - 1]["GroupName"]) {
            //    $optgroup = "";
            //    $optgroup = $("<optgroup>", {
            //        label: Employee.GroupName
            //    });

            //    $optgroup.appendTo(ddl);
            //}
            var $option = $("<option>", {
                text: Employee.Text,
                value: Employee.RowKey
            });

            $option.appendTo(ddl2);

        });
       


    }

    var TotalFollowUpCount = 0;
    var TotalIntrestedCount = 0;
    var TotalAdmissionTakenCount = 0;
    var TotalClosedCount = 0;
    var TotalCount = 0;
    var TotalProductiveCallsCount = 0;
    var TotalRefundCount = 0;
    var TotalReScheduledCount = 0;
  

    function FC(cellValue, options, rowdata, action)
    {
        

        TotalFollowUpCount = TotalFollowUpCount + cellValue;
        if (cellValue != 0) {
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount FollowUpCallsCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            //return ''
 
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
      
        }
    }

    function IC(cellValue, options, rowdata, action) {
        TotalIntrestedCount = TotalIntrestedCount + cellValue;
        if (cellValue != 0) {
            return '<a data-key="4" data-val="3" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount IntrestedCallsCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else
        {
            //return ''
         return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function AC(cellValue, options, rowdata, action) {
        TotalAdmissionTakenCount = TotalAdmissionTakenCount + cellValue;
        if (cellValue != 0)
        {
            return '<a data-key="4" data-val="2" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount AdmissionTakenCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else
        {
            //return ''
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function CC(cellValue, options, rowdata, action)
    {
        TotalClosedCount = TotalClosedCount + cellValue;
        if (cellValue != 0) {
            return '<a data-key="4" data-val="4" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount ClosedCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else
        {
            //return ''
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }
    function TR(cellValue, options, rowdata, action)
    {
      var TotalRecords=  rowdata.TotalFollowUpCount + rowdata.TotalAdmissionTakenCount + rowdata.TotalIntrestedCount + rowdata.TotalClosedCount;
      rowdata.TotalEnquiryCount = TotalRecords;
      TotalCount = TotalCount + TotalRecords;
     
      if (TotalRecords != 0) {
          return '<a data-key="4" data-val="5"  EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount TotalRecordsCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + TotalRecords + '</span>';
      }
      else {
          //return ''
          return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
      }
    }

    //function RS(cellValue, options, rowdata, action) {
        
    //    if (cellValue != 0)
    //    {
    //        TotalReScheduledCount = TotalReScheduledCount + cellValue;

    //        return '<a data-key="4" data-val="6" EmployeeKey=' + rowdata.EmployeeKey + '   class="callsCount RescheduleCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
    //    }

    //    else
    //    {
    //        //return ''
    //        return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
    //    }
    //}

    function RS(cellValue, options, rowdata, action)
    {    
        var DuplicateCalls = parseInt(rowdata.TotalCallsCount) - parseInt(rowdata.TotalEnquiryCount);
        if (DuplicateCalls < 0)
        {
            DuplicateCalls = 0;
        }
        if (DuplicateCalls != 0)
            {
                TotalReScheduledCount = TotalReScheduledCount + DuplicateCalls;
           
                return '<a data-key="4" data-val="6" EmployeeKey=' + rowdata.EmployeeKey + '   class="callsCount RescheduleCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + DuplicateCalls + '</span>';
            }
        

        else {
            //return ''
         return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
        }
    


    function TC(cellValue, options, rowdata, action)
    {

        TotalCallsCount = TotalCallsCount + cellValue;
        if (cellValue != 0) {


            return '<a data-key="4" data-val="7" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount TotalCallsCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }

        else {
            //return ''
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function PC(cellValue, options, rowdata, action) {
        TotalProductiveCallsCount = TotalProductiveCallsCount + cellValue;
        if (cellValue != 0) {
            return '<a data-key="4" data-val="8" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount ProductiveCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            //return ''
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

    function RF(cellValue, options, rowdata, action) {
        TotalRefundCount = TotalRefundCount + cellValue;
        if (cellValue != 0) {
            return '<a data-key="4" data-val="9" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount FollowUpCallsCount" onclick="CallReports.GetCallReportDetailed(this,1,true)" >' + cellValue + '</span>';
        }
        else {
            //return ''
            return '<a data-key="4" data-val="1" EmployeeKey=' + rowdata.EmployeeKey + '  class="callsCount NoRecordCount"  >0</span>';
        }
    }

 return {
     GetApplication: getApplication,
     GetEmployeesByBranchId: getEmployeesByBranchId,
     GetCallReportDetailed: getCallReportDetailed,
     GetCallReportDetailedData: getCallReportDetailedData
   
}
}());



function CallReportPagination(_this)
{
    
    $('#page-selection-up,#page-selection-down').empty();
    var totalRecords = $("#hdnTotalRecords").val();
    totalRecords = totalRecords != "" ? parseInt(totalRecords) : 0;
    var Size = jsonData["PageSize"];
    var totalPages = Math.floor(totalRecords % Size == 0 ? totalRecords / Size : (totalRecords / Size) + 1);

    $('#page-selection-up,#page-selection-down').bootpag({
        total: totalPages,
        page: 1,
        maxVisible: 30
    });

    $('#page-selection-up,#page-selection-down').on("page", function (event, num) {
       
        CallReports.GetCallReportDetailedData(_this, num)
    });
}




function GetHistoryByMobileNumber(obj)
{

    var SelectedCount = 0;
    $('#CustomizeColumn option').each(function (value, object) {
        if ($(object).prop("selected") == true)
        {
            SelectedCount = SelectedCount + 1;
        }
    })

    $('#ReportTable > tbody > tr').eq(obj.RowIndex).after("<tr class='tempRow'> <td id='FeedbackCol' colspan=" + SelectedCount +5+ ">  <table class='Feedback'>   <span style='position: absolute; right: 0px;  left: 0px; margin: auto; width: 107px;margin-top: -8px; font-size: 12px;  color: #277919;'><i style='color: #417d5c; margin-left: -30px !important; margin: auto; right: 0px; left: 0px; margin-top: -5px;position: absolute; font-size: 24px;' class='fa fa-spinner fa-spin fa-3x fa-fw'></i> Loading all History...</span> </table></td></tr>");

    $("tr.tempRow").animate({ height: 44 }, 150);

    SharedRequest = $.ajax({
        url: $("#hdnGetReportHistory").val(),
        data: obj,
        datatype: "json",
        type: "post",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        beforeSend: function () {
            if (SharedRequest != null) {
                SharedRequest.abort();
            }
        },
        success: function (data) {

            $(".tempRow").remove();
            $('#ReportTable > tbody > tr').eq(obj.RowIndex).after("<tr class='tempRow'> <td id='FeedbackCol' colspan=" + SelectedCount + 5 + ">  <i onclick='RemoveSubTable()' class='fa fa-remove removeSubTable'></i> <table class='Feedback'>" + data + "</table></td></tr>");

           

        }
    });

}

function RemoveSubTable()
{
    $(".tempRow").remove();
}