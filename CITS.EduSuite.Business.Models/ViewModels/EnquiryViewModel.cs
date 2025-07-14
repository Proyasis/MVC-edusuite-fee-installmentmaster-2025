using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using CITS.EduSuite.Business.Models.Security;
using System.Threading;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EnquiryViewModel : BaseModel
    {
        public EnquiryViewModel()
        {
            Branches = new List<SelectListModel>();
            Departments = new List<SelectListModel>();
            CallTypes = new List<SelectListModel>();
            CallStatuses = new List<SelectListModel>();
            EnquiryStatuses = new List<SelectListModel>();
            LastEnquiryCallStatuses = new List<SelectListModel>();
            Countries = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            NatureOfEnquiries = new List<SelectListModel>();
            CourseDuration = new List<SelectListModel>();
            ConcellingTimes = new List<SelectListModel>();
            TelephoneCodes = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public long UserKey { get; set; }
        public int ModuleKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AcademicTermRequired")]
        public short? AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryRequired")]
        public short? CountryKey { get; set; }
        public string CountryName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseType_Required")]
        public short? CourseTypeKey { get; set; }
        public string CourseTypeName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Course_Required")]
        public long? CourseKey { get; set; }
        public string CourseName { get; set; }
        public string EducationQualification { get; set; }
        public DateTime? DateOfBirth { get; set; }

        //[RequiredIf("ServiceTypeKey", DbConstants.ServiceType.Study, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "UniversityRequired")]
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "University_Required")]
        public short? UniversityKey { get; set; }
        public string UniversityName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryNameRegularExpressionErrorMessage")]
        public string EnquiryName { get; set; }

        [StringLength(150, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryAddressLengthErrorMessage")]
        public string EnquiryAddress { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LandPhoneExpressionErrorMessage")]
        [StringLength(15, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LandPhoneExpressionErrorMessage")]
        public string PhoneNumber { get; set; }

        public int? MinPhoneLength { get; set; }
        public int? MaxPhoneLength { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberRequired")]
        [StringLengthDynamic("MaxPhoneLength", "MinPhoneLength", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ValidExpressionErrorMessage")]
        //[StringLength(15, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckMobileNumberExists", "Enquiry", AdditionalFields = "RowKey,EnquiryLeadKey,AcademicTermKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckApplicationPhoneExists")]
        [Display(Name = "MobileNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string MobileNumber { get; set; }

        public int? MinPhoneLengthOptional { get; set; }
        public int? MaxPhoneLengthOptional { get; set; }
        //[StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        //[StringLength(15, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberLengthErrorMessage")]
        [StringLengthDynamic("MaxPhoneLengthOptional", "MinPhoneLengthOptional", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ValidExpressionErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        [Display(Name = "MobileNumberOptional", ResourceType = typeof(EduSuiteUIResources))]
        public string MobileNumberOptional { get; set; }
        public short TelephoneCodeKey { get; set; }
        public string TelephoneCodeName { get; set; }
        public short? TelephoneCodeOptionalKey { get; set; }
        public string TelephoneCodeOptionalName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressLengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckEmailAddressExists", "Enquiry", AdditionalFields = "RowKey,EnquiryLeadKey,AcademicTermKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckEnquiryEmailExists")]
        public string EmailAddress { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "QualificationLengthErrorMessage")]
        public string EnquiryEducationQualification { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NatureOfEnquiryRequired")]
        public short? NatureOfEnquiryKey { get; set; }
        public string NatureOfEnquiryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DepartmentRequired")]
        //public short DepartmentKey { get; set; }
        public string DepartmentName { get; set; }
        public string CouncellingTime { get; set; }
        //public short UserDepartmentKey { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadFeedbackLengthErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeedbackRequired")]
        public string EnquiryFeedback { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateOfBirthRequired")]
        //[LessThan("DateTimeNow", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateOfBirthCompareErrorMessage")]
        //public DateTime? DateOfBirth { get; set; }
        public DateTime? DateTimeNow { get { return DateTimeUTC.Now; } }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GenderRequired")]
        public byte Gender { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryStatusRequired")]
        public short EnquiryStatusKey { get; set; }

        [RequiredIfNot("EnquiryStatusKey", DbConstants.EnquiryStatus.Closed, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NextCallScheduleRequired")]
        //[ShortListDateRestriction("IsShortListed", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ShortListDateRestrictionErrorMessage")]
        public DateTime? NextCallSchedule { get; set; }
        public bool ProcessingStatus { get; set; }
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExpectedJoinDateRequired")]
        //public DateTime? EnquiryFeedbackReminderDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallTypeRequired")]
        public byte CallTypeKey { get; set; }
        public string CallTypeName { get; set; }
        public string EnquiryStatusesName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadCallStatusRequired")]
        public int? EnquiryCallStatusKey { get; set; }

        [RequiredIf("EnquiryCallStatusKey", DbConstants.EnquiryCallStatus.Counselling, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ConcellingTimeRequired")]
        public string ConcellingTimeKey { get; set; }
        public long? EnquiryKey { get; set; }

        [RequiredIfTrue("IsDuration", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallDurationRequired")]
        public TimeSpan? EnquiryDuration { get; set; }
        public long? EnquiryLeadKey { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Feedback { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictNameLengthErrorMessage")]
        public string DistrictName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LocationNameLengthErrorMessage")]
        public string LocationName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameRequired")]
        //[RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameLengthErrorMessage")]
        //[StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameRegularExpressionErrorMessage")]
        public string CompanyName { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryRemarksLengthErrorMessage")]
        public string Remarks { get; set; }
        public string SearchLocation { get; set; }
        public string LastUpdatedBy { get; set; }
        public int? LastCallStatusKey { get; set; }
        public DateTime? LastCallScheduleDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
        public string SearchName { get; set; }
        public short? SearchAcademicTermKey { get; set; }
        public string SearchPhone { get; set; }
        public string SearchEmail { get; set; }
        public DateTime? SearchFromDate { get; set; }
        public DateTime? SearchToDate { get; set; }
        public short? SearchBranchKey { get; set; }
        //public short? SearchDepartmentKey { get; set; }
        public int? SearchCallStatusKey { get; set; }
        public int? SearchEnquiryStatusKey { get; set; }
        public bool IsProccessed { get; set; }
        public int TabKey { get; set; }
        public bool IsEditable { get; set; }
        public string EnquiryInboxCount { get; set; }
        public string EnquiryOutboxCount { get; set; }
        public string EnquiryProccessingCount { get; set; }
        public long AddedEmployeeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public long? EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public string ScheduledBy { get; set; }
        public string CounselledBy { get; set; }
        public short? CourseDurationKey { get; set; }
        public List<SelectListModel> CourseDuration { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Departments { get; set; }
        public List<SelectListModel> CallTypes { get; set; }
        public List<SelectListModel> LastEnquiryCallStatuses { get; set; }
        public List<SelectListModel> Countries { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> NatureOfEnquiries { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> CallStatuses { get; set; }
        public List<SelectListModel> EnquiryStatuses { get; set; }
        public List<SelectListModel> ConcellingTimes { get; set; }
        public List<SelectListModel> TelephoneCodes { get; set; }
        public string EnquiryCallStatusName { get; set; }
        public DateTime? EnquiryLastUpdatedDate { get; set; }
        public long? SearchEmployeeKey { get; set; }
        public bool IsDuration { get; set; }
        public List<GroupSelectListModel> Employees { get; set; }
        public string SearchText { get; set; }
        public long? ScholershipKey { get; set; }
        public bool IsShortListed { get; set; }
        public bool? isCounsellingCompleted { get; set; }
    
    }

    public class EnquiryFeedbackViewModel : BaseModel
    {
        public EnquiryFeedbackViewModel()
        {
            EnquiryCallStatuses = new List<SelectListModel>();
            CallTypes = new List<SelectListModel>();
            EnquiryStatuses = new List<SelectListModel>();
            ConcellingTimes = new List<SelectListModel>();
            UserKeys = new List<long>();
            Branches = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public long UserKey { get; set; }
        public int ModuleKey { get; set; }
        public List<SelectListModel> Employees { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadFeedbackLengthErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeedbackRequired")]
        public string Feedback { get; set; }
        public string PostedBy { get; set; }
        public long? EmployeeKey { get; set; }

        [RequiredIfTrue("IsDuration", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallDurationRequired")]
        public TimeSpan? CallDuration { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadCallStatusRequired")]
        public int? EnquiryCallStatusKey { get; set; }

        [RequiredIfNot("EnquiryStatusKey", DbConstants.EnquiryStatus.Closed, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NextCallScheduleRequired")]
        //[ShortListDateRestriction("IsShortListed", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ShortListDateRestrictionErrorMessage")]
        public DateTime? NextCallSchedule { get; set; }
        public long EnquiryKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallTypeRequired")]
        public byte CallTypeKey { get; set; }
        public string CallTypeName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryStatusRequired")]
        public short EnquiryStatusKey { get; set; }
        public short? OtherBranchKey { get; set; }
        public short BranchKey { get; set; }
        public string EnquiryCallStatusName { get; set; }
        public bool IsDuration { get; set; }

        [RequiredIf("EnquiryCallStatusKey", DbConstants.EnquiryCallStatus.Counselling, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ConcellingTimeRequired")]
        public string ConcellingTimeKey { get; set; }
        public DateTime? DateAdded { get; set; }
        public string NotificationEmails { get; set; }
        public string NotificationMobileNo { get; set; }
        public List<SelectListModel> CallTypes { get; set; }
        public List<SelectListModel> EnquiryCallStatuses { get; set; }
        public List<SelectListModel> EnquiryStatuses { get; set; }
        public List<SelectListModel> ConcellingTimes { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<long> UserKeys { get; set; }
        public string CouncellingTime { get; set; }
        public DateTime? CallDate { get; set; }
        public bool IsClosedNotification { get; set; }
        public bool IsUserBlocked { get; set; }
        public bool IsShortListed { get; set; }

    }
}
