using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EnquiryScheduleViewModel : BaseModel
    {
        public EnquiryScheduleViewModel()
        {
            Branches = new List<SelectListModel>();
            Departments = new List<SelectListModel>();
            Employees = new List<GroupSelectListModel>();
            CallTypes = new List<SelectListModel>();
            ScheduleCallStatuses = new List<SelectListModel>();
            ScheduleStatuses = new List<SelectListModel>();
            TelephoneCodes = new List<SelectListModel>();
            UserKeys = new List<Int64>();
            EnquiryScheduleList = new List<EnquiryScheduleViewModel>();
            MobileNumberSearch = new List<MobileNumberSearchViewModel>();
            EnquiryScheduleKeys = new List<long>();
            IfFeedback = false;
            FHCallStatuses = new List<SelectListModel>();
            FHEnquiryStatuses = new List<SelectListModel>();
        }
        public List<Int64> UserKeys { get; set; }
        public long? RowNumber { get; set; }
        public long RowKey { get; set; }
        public long FeedbackKey { get; set; }
        public long UserKey { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }
        public string InTakeName { get; set; }
        public string EmailAddress { get; set; }
        public string Qualification { get; set; }
        public short BranchKey { get; set; }
        public short DepartmentKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Employee", ResourceType = typeof(EduSuiteUIResources))]
        public long EmployeeKey { get; set; }
        public string Feedback { get; set; }
        public string MobileNumber { get; set; }
        public string Remarks { get; set; }
        public byte IsNewLead { get; set; }
        public DateTime? LeadDate { get; set; }
        public long EnquiryKey { get; set; }
        public long EnquiryLeadKey { get; set; }
        public string SearchName { get; set; }
        public string SearchPhone { get; set; }
        public string SearchEmail { get; set; }
        public string EmployeeName { get; set; }
        public string CallTypeName { get; set; }
        public string ApplicationStatusName { get; set; }
        public bool IsSpamed { get; set; }
        public string ScheduledBy { get; set; }
        public string CouncellingBy { get; set; }
        public int ApplicationStatusKey { get; set; }
        public string CallStatusName { get; set; }
        public TimeSpan? CallDuration { get; set; }
        public DateTime? NextCallScheduleDate { get; set; }
        public int LastCallStatusKey { get; set; }
        public DateTime? FeedbackCreatedDate { get; set; }
        public byte? CallTypeKey { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateOn { get; set; }
        public bool IsShortListed { get; set; }
        public bool IsNew { get; set; }
        public bool IsInBox { get; set; }
        public bool IsEditable { get; set; }
        public int? ScheduleStatusKey { get; set; }
        public bool EnquiryFeedbackReminderStatus { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
        public long? SearchEmployeeKey { get; set; }
        public short? SearchBranchKey { get; set; }
        public short? SearchScheduleStatusKey { get; set; }
        public int? SearchCallStatusKey { get; set; }
        public byte? SearchCallTypeKey { get; set; }
        public DateTime? SearchFromDate { get; set; }
        public DateTime? SearchToDate { get; set; }
        public short EnquiryStatusKey { get; set; }
        public int ScheduleTypeKey { get; set; }
        public int ScheduleSelectTypeKey { get; set; }
        public long TodaysScheduleCount { get; set; }
        public long TomorrowCount { get; set; }
        public long PendingScheduleCount { get; set; }
        public long UpcomingScheduleCount { get; set; }
        public long HistoryCount { get; set; }
        public long NewLeadCount { get; set; }
        public string AllScheduleCount { get; set; }
        public string EnquiryScheduleCount { get; set; }
        public string LeadScheduleCount { get; set; }
        public long TodaysRecheduleCount { get; set; }
        public long CouncellingScheduleCount { get; set; }
        public long UnallocatedLeadCount { get; set; }
        public string CouncellingTime { get; set; }
        public string LocationName { get; set; }
        public string BranchName { get; set; }
        public string SearchLocation { get; set; }
        public string ScheduleTypeName { get; set; }
        public int SearchHistoryTelephoneCodeKey { get; set; }
        public int TelephoneCodeKey { get; set; }
        public string LeadAllocationStaffKey { get; set; }
        public long EmployeeAllocateKey { get; set; }
        public long ShortlistedCount { get; set; }
        public long ShortlistPendingCount { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Departments { get; set; }
        public List<GroupSelectListModel> Employees { get; set; }
        public List<SelectListModel> CallTypes { get; set; }
        public List<SelectListModel> ScheduleCallStatuses { get; set; }
        public List<SelectListModel> ScheduleStatuses { get; set; }
        public List<EnquiryViewModel> CallCountList { get; set; }
        public List<EnquiryViewModel> CallDurationList { get; set; }
        public List<EnquiryViewModel> IntrestedList { get; set; }
        public List<EnquiryViewModel> TodaysCouncelling { get; set; }
        public List<EnquiryViewModel> ProductiveCallList { get; set; }
        public List<EnquiryViewModel> ProductiveIncomingCallList { get; set; }
        public List<EnquiryViewModel> ProductiveOutgoingCallList { get; set; }
        public List<EnquiryViewModel> ProductiveWalkingList { get; set; }
        public List<SelectListModel> TelephoneCodes { get; set; }
        public List<EnquiryScheduleViewModel> EnquiryScheduleList { get; set; }
        public List<MobileNumberSearchViewModel> MobileNumberSearch { get; set; }
        public int ModuleKey { get; set; }
        public long ClosedCount { get; set; }
        public List<long> EnquiryScheduleKeys { get; set; }

        public List<SelectListModel> FHCallStatuses { get; set; }
        public List<SelectListModel> FHEnquiryStatuses { get; set; }
        public bool IfFeedback { get; set; }

        [RequiredIfTrue("IfFeedback", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CallStatus", ResourceType = typeof(EduSuiteUIResources))]
        public int? FHCallStatusKey { get; set; }

        [RequiredIfTrue("IfFeedback", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryStatusRequired")]
        public short? FHEnquiryStatusKey { get; set; }
       
        [RequiredIfNot("FHEnquiryStatusKey", DbConstants.EnquiryStatus.Closed, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NextCallScheduleRequired")]
        public DateTime? FHNextCallScheduleDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(500, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "Remarks", ResourceType = typeof(EduSuiteUIResources))]
        public string FHRemarks { get; set; }
    }



    public class MobileNumberSearchViewModel
    {

        public MobileNumberSearchViewModel()
        {
            EnquiryLeadSchedule = new List<EnquiryScheduleViewModel>();
            EnquiryNewLeadSchedule = new List<EnquiryScheduleViewModel>();
            EnquirySchedule = new List<EnquiryScheduleViewModel>();
            ApplicationSchedule = new List<EnquiryScheduleViewModel>();
            DocumentSchedule = new List<EnquiryScheduleViewModel>();
        }

        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string LocationName { get; set; }
        public string ApplicationStatusName { get; set; }
        public string InTakeName { get; set; }
        public string CouncellingBy { get; set; }
        public string ScheduledBy { get; set; }
        public int ApplicationStatusKey { get; set; }
        public long ApplicationKey { get; set; }
        public bool IsSpamed { get; set; }
        public string AcademicTermName { get; set; }


        public List<EnquiryScheduleViewModel> EnquiryLeadSchedule { get; set; }
        public List<EnquiryScheduleViewModel> EnquirySchedule { get; set; }
        public List<EnquiryScheduleViewModel> ApplicationSchedule { get; set; }
        public List<EnquiryScheduleViewModel> DocumentSchedule { get; set; }

        public List<EnquiryScheduleViewModel> EnquiryNewLeadSchedule { get; set; }

    }



}
