using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CITS.EduSuite.Business.Models.ViewModels
{
   public class EnquiryReportViewModel:BaseModel
    {

       public EnquiryReportViewModel()
        {
            Branches = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            InTakes = new List<SelectListModel>();
            Religions = new List<SelectListModel>();
            Agents = new List<SelectListModel>();
            Countries = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            ProgramTypes = new List<SelectListModel>();
            Programs = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            NatureOfEnquiries = new List<SelectListModel>();
            TelephoneCodes = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
            SearchEmployees = new List<SelectListModel>();
            ApplicationStatuses = new List<SelectListModel>();
            ScheduleTypes = new List<SelectListModel>();
            EnquiryCallStatuses = new List<SelectListModel>();
            DateTypes = new List<SelectListModel>();
            EmployeeFilterTypes = new List<SelectListModel>();
            CallReportsDetailsList = new List<EnquiryReportViewModel>();
            FeedbackList = new List<CallFeedbacksList>();
            CallTypes = new List<SelectListModel>();
            EnquiryStatus = new List<SelectListModel>();
            Enquiries = new List<SelectListModel>();

        }
       public  List<EnquiryReportViewModel> CallReportsDetailsList = new List<EnquiryReportViewModel>();
       public List<SelectListModel> Branches { get; set; }
       public List<SelectListModel> CallTypes { get; set; }
       public List<SelectListModel> Batches { get; set; }
       public List<SelectListModel> Agents { get; set; }
       public List<SelectListModel> InTakes { get; set; }
       public List<SelectListModel> Religions { get; set; }
       public List<SelectListModel> Countries { get; set; }
       public List<SelectListModel> ProgramTypes { get; set; }
       public List<SelectListModel> Programs { get; set; }
       public List<SelectListModel> Universities { get; set; }
       public List<SelectListModel> NatureOfEnquiries { get; set; }
       public List<SelectListModel> AcademicTerms { get; set; }
       public List<SelectListModel> TelephoneCodes { get; set; }
       public List<SelectListModel> Employees { get; set; }
       public List<SelectListModel> SearchEmployees { get; set; }
       public List<SelectListModel> ApplicationStatuses { get; set; }
       public List<SelectListModel> ScheduleTypes { get; set; }
       public List<SelectListModel> DateTypes { get; set; }
       public List<SelectListModel> EnquiryCallStatuses { get; set; }
       public List<SelectListModel> EmployeeFilterTypes { get; set; }
       public List<SelectListModel> EnquiryStatus { get; set; }
      
       public List<Int32> ScheduleTypeKeys { get; set; }
        public string ScheduleTypeKeysList { get; set; }
        public List<Int32> ApplicationStatusKeys { get; set; }
        public string ApplicationStatusKeysList { get; set; }   
        public long TotalEnquiryCount {get;set;}
        public long TotalFollowUpCount {get;set;}
        public long TotalIntrestedCount {get;set;}
        public long TotalAdmissionTakenCount{get;set;}
        public  long TotalClosedCount{get;set;}
        public long TotalProductiveCallsCount{get;set;}
        public long TotalRefundCount { get; set; }
        public long TotalCallsCount { get; set; }
        public long TotalRepeatedCallsCount { get; set; }
        public int ScheduleTypeKey{get;set;}
        public short FollowUpKey {get;set;}
        public short AdmissionTakenKey{get;set;}

        public short IntrestedKey {get;set;}
        public short ClosedKey{get;set;}
        public short ProductiveCallsLimit{get;set;}
        public short? SearchSubEnquiryStatusKey { get; set; }
        public short? SearchAcademicTermKey { get; set; }
        public long? SearchEmployeeKey { get; set; }
        public long? SearchScheduledEmployeeKey {get;set;}
        public short? SearchBranchKey {get;set;}
        public short SearchApplicationStatusKey { get; set; }
        public short SearchEnquiryStatusKey { get; set; }
        public short? TelephoneCodeKey { get; set; }
        public short BranchKey { get; set; }
        public int? SearchScheduleTypeKey { get; set; }
        public int ReminderStatusKey { get; set; }
        public int? SearchDateTypeKey { get; set; }
        public int? SearchCountryKey { get; set; }
        public int? SearchCallStatusKey { get; set; }
        public int? SearchCounselllingBranchKey { get; set; }
        public int? SearchCallTypeKey { get; set; }
        public short? SearchInTakeKey { get; set; }
        public string SearchLocation { get; set; }
        public string SearchAnyText { get; set; }
        public string PostedBy { get; set; }
        public DateTime? SearchFromDate{ get; set; }
        public DateTime? SearchToDate { get; set; }
        public int? EmployeeFilterTypeKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        
        

        public long RowKey { get; set; }
    
        public string EmployeeName { get; set; }
        public string ScheduledEmployeeName { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Qualificatin { get; set; }
        public string Branch { get; set; }
        public string EnquiryStatusOnCall { get; set; }
        public string CounsellingBranchName { get; set; }
        public string AcademicTermName { get; set; }
        public string Country { get; set; }
        public string Program { get; set; }
        public string District { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string CallStatusName { get; set; }

        public long TotalRecordsCount { get; set; }
        public long RowNumber { get; set; }
        public List<CallFeedbacksList> FeedbackList { get; set; }
        public long EmployeeKey { get; set; }
        public string Feedback { get; set; }
        public string CallTypeName { get; set; }
        public string ScheduleTypeName { get; set; }
        public string AddedBy { get; set; }
        public DateTime? NextCallSchedule { get; set; }
        public DateTime? EnquiryCounsellingDate { get; set; }
        public DateTime? EnquiryCounsellingCalledDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public short? ApplicationStatusKey { get; set; }

    //NextCallSchedule as EnquiryCounsellingCalledDate,
    //NextCallSchedule as CreatedOn,
    //CAST(0 as smallint) ApplicationStatusKey

        public DateTime? LastCallSchedule { get; set; }
        public string ApplicationStatusName { get; set; }
        public DateTime? LastCalledDate { get; set; }
        public DateTime? CalledDate { get; set; }
        public DateTime? DateTomorrow { get; set; }
        public DateTime? DateToday { get; set; }
        public DateTime? MonthStartDate { get; set; }
        public DateTime? MonthEndDate { get; set; }
        public DateTime? DateYesterday { get; set; }
        public DateTime? DateUpcoming { get; set; }
        public TimeSpan? CallDuration { get; set; }
        public string StatusName { get; set; }
        public string CounsellingTime { get; set; }
        public bool IsClose { get; set; }
        public bool IsSpamed { get; set; }
        public bool IsClosePending { get; set; }
   
   
        public short? EnquiryStatusKey { get; set; }
        public int SearchIsRefund { get; set; }
        public int SearchIsClose { get; set; }
        public int SearchIsSpamed { get; set; }
        public int SearchIsOnCallStatusVise { get; set; }
        public int SearchIsClosePending { get; set; }
        //public string ScheduleTypesName { get; set; }


       //ScheduleTransfer variables
        public List<SelectListModel> Enquiries { get; set; }
        public bool IsSelected { get; set; }
        public long SelectedEnquiryKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public int? TransferEmployeeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public int? TransferEmployeeBranchKey { get; set; }
        
        public int? ScheduleTransferTypeKey { get; set; }
    }


   public class CallReportCountData
   {
       public long EmployeeKey { get; set; }
       public string EmployeeName { get; set; }
       public long TotalEnquiryCount { get; set; }
       public long TotalFollowUpCount { get; set; }
       public long TotalIntrestedCount { get; set; }
       public long TotalAdmissionTakenCount { get; set; }
       public long TotalClosedCount { get; set; }
       public long TotalProductiveCallsCount { get; set; }
       public long TotalRefundCount { get; set; }
       public long TotalCallsCount { get; set; }
       public long TotalRepeatedCallsCount { get; set; }
   }

   public class CallFeedbacksList
   {

       public string CallStatusName { get; set; }
       public string CallTypeName { get; set; }
       public string ScheduleTypeName { get; set; }
       public string AddedBy { get; set; }
       public DateTime? NextCallSchedule { get; set; }
       public DateTime CalledDate { get; set; }
       public TimeSpan? CallDuration { get; set; }
   }
}
