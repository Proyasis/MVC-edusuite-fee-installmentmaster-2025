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
    public class EnquiryLeadViewModel : BaseModel
    {
        public EnquiryLeadViewModel()
        {
            Branches = new List<SelectListModel>();
            Departments = new List<SelectListModel>();
            Employees = new List<GroupSelectListModel>();
            EnquiryStatuses = new List<SelectListModel>();
            TelephoneCodes = new List<SelectListModel>();
            EnquiryLeadCallStatuses = new List<SelectListModel>();
            EnquiryLeadStatuses = new List<SelectListModel>();
            CallTypes = new List<SelectListModel>();
            //EnquiryLeadStatusKey = DbConstants.EnquiryStatus.FollowUp;
            LeadDate = DateTimeUTC.Now;
            EnquiryLeadKeys = new List<long>();
            NatureOfEnquiries = new List<SelectListModel>();

        }
        public string AdName { get; set; }
        public string platformName { get; set; }
        public long? LeadApiKey { get; set; }
        public long? AdsAPIKey { get; set; }
        public long? AdKey { get; set; }
        public long RowKey { get; set; }
        public long? EnquiryLeadFeedbackKey { get; set; }
        public short? NatureOfEnquiryKey { get; set; }
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadNameRegularExpressionErrorMessage")]
        public string Name { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictNameLengthErrorMessage")]
        public string District { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LocationNameLengthErrorMessage")]
        public string Location { get; set; }
        public short? TelephoneCodeKey { get; set; }
        public string TelephoneCodeName { get; set; }

        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameLengthErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CompanyNameRegularExpressionErrorMessage")]
        public string CompanyName { get; set; }
        public string LeadReference { get; set; }
        public string SearchLocation { get; set; }

        public int? MinPhoneLength { get; set; }
        public int? MaxPhoneLength { get; set; }
        //[StringLength(15, MinimumLength =10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        //[StringLength(15, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberLengthErrorMessage")]
        [StringLengthDynamic("MaxPhoneLength", "MinPhoneLength", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ValidExpressionErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberRequired")]
        //[RegularExpression(@"^[0-9]{10}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckMobileNumberExists", "EnquiryLead", AdditionalFields = "TelephoneCodeKey,RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckApplicationPhoneExists")]
        [Display(Name = "MobileNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string MobileNumber { get; set; }
        public short? TelephoneCodeOptionalKey { get; set; }
        public string TelephoneCodeOptionalName { get; set; }

        public int? MinPhoneLengthOptional { get; set; }
        public int? MaxPhoneLengthOptional { get; set; }

        [StringLengthDynamic("MaxPhoneLengthOptional", "MinPhoneLengthOptional", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ValidExpressionErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        [Display(Name = "MobileNumberOptional", ResourceType = typeof(EduSuiteUIResources))]
        public string MobileNumberOptional { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LandPhoneExpressionErrorMessage")]
        [StringLength(12, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LandPhoneExpressionErrorMessage")]
        public string PhoneNumber { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressLengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckEmailAddressExists", "EnquiryLead", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckEnquiryEmailExists")]
        public string EmailAddress { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "QualificationLengthErrorMessage")]
        public string Qualification { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DepartmentRequired")]
        public short? DepartmentKey { get; set; }
        public string DepartmentName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public long? EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string LeadFrom { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadDateRequired")]
        //[GreaterThanOrEqualTo("DateTimeNow", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadDateCompareErrorMessage")]
        public DateTime? LeadDate { get; set; }
        public DateTime DateTimeNow { get { return DateTimeUTC.Now; } }
        public DateTime? DateAdded { get; set; }
        public string DateAddedTxt { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadRemarksLengthErrorMessage")]
        public string Remarks { get; set; }
        public byte? IsNewLead { get; set; }
        public long EnquiryKey { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateOn { get; set; }

        //Feedback

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadFeedbackLengthErrorMessage")]
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadFeedbackRequired")]
        public string Feedback { get; set; }

        //[RequiredIfTrue("IsDuration", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallDurationRequired")]
        public TimeSpan? CallDuration { get; set; }

        //[RequiredIfNot("EnquiryLeadStatusKey", DbConstants.EnquiryStatus.Closed, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NextCallScheduleRequired")]
        public DateTime? NextCallSchedule { get; set; }
        public long EnquiryLeadKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallTypeRequired")]
        public byte? CallTypeKey { get; set; }
        public string CallTypeName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadStatusRequired")]
        public short? EnquiryLeadStatusKey { get; set; }
        public string EnquiryLeadStatusName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadCallStatusRequired")]
        public int? EnquiryLeadCallStatusKey { get; set; }
        public string EnquiryLeadCallStatusName { get; set; }
        public bool IsDuration { get; set; }
        public string NotificationEmails { get; set; }
        public string NotificationMobileNo { get; set; }
        public bool IsUserBlocked { get; set; }

        //Bind EnquiryLead
        public long TotalRecords { get; set; }
        public string SearchName { get; set; }
        public string SearchPhone { get; set; }
        public string SearchEmail { get; set; }
        public string SearchCallStatus { get; set; }
        public DateTime? SearchFromDate { get; set; }
        public DateTime? SearchToDate { get; set; }
        public long? SearchEmployeeKey { get; set; }
        public short? SearchBranchKey { get; set; }
        public short? SearchAllocateBranchKey { get; set; }
        public short? SearchEnquiryLeadStatusKey { get; set; }
        public int? SearchCallStatusKey { get; set; }
        public short? SearchAcademicTermKey { get; set; }
        public long UserKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchText { get; set; }
        public bool IsEditable { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public DateTime? LeadLastUpdatedDate { get; set; }
        public string NatureOfEnquiryName { get; set; }
        public string BranchCode { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Departments { get; set; }
        public List<GroupSelectListModel> Employees { get; set; }
        public List<SelectListModel> EnquiryStatuses { get; set; }
        public List<SelectListModel> TelephoneCodes { get; set; }
        public List<SelectListModel> CallTypes { get; set; }
        public List<SelectListModel> EnquiryLeadCallStatuses { get; set; }
        public List<SelectListModel> EnquiryLeadStatuses { get; set; }
        public List<EnquiryLeadViewModel> LeadsList { get; set; }
        public List<SelectListModel> NatureOfEnquiries { get; set; }
        public List<long> EnquiryLeadKeys { get; set; }
                    
    }
    public class EnquiryLeadFeedbackViewModel : BaseModel
    {
        public EnquiryLeadFeedbackViewModel()
        {
            EnquiryLeadCallStatuses = new List<SelectListModel>();
            EnquiryLeadStatuses = new List<SelectListModel>();
            CallTypes = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            EnquiryLeadStatusKey = DbConstants.EnquiryStatus.FollowUp;
            UserKeys = new List<long>();
            ReferenceList = new List<ReferenceList>();
        }

        public long RowKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AcademicTermRequired")]
        public short AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeadFeedbackLengthErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadFeedbackRequired")]
        public string Feedback { get; set; }

        [RequiredIfTrue("IsDuration", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallDurationRequired")]
        public TimeSpan? CallDuration { get; set; }

        [RequiredIfNot("EnquiryLeadStatusKey", DbConstants.EnquiryStatus.Closed, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NextCallScheduleRequired")]
        public DateTime? NextCallSchedule { get; set; }
        public DateTime? DateAdded { get; set; }
        public long EnquiryLeadKey { get; set; }
        public long? EnquiryKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CallTypeRequired")]
        public byte CallTypeKey { get; set; }
        public string CallTypeName { get; set; }
        public string PostedBy { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadCallStatusRequired")]
        public int? EnquiryLeadCallStatusKey { get; set; }
        public string EnquiryLeadCallStatusName { get; set; }
        public long? EmployeeKey { get; set; }
        public long? LeadEmployeeKey { get; set; }
        public List<SelectListModel> CallTypes { get; set; }
        public List<SelectListModel> EnquiryLeadCallStatuses { get; set; }
        public List<SelectListModel> EnquiryLeadStatuses { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryLeadStatusRequired")]
        public short? EnquiryLeadStatusKey { get; set; }
        public DateTime? CallDate { get; set; }
        public bool IsDuration { get; set; }
        public bool IsNewLeadNotification { get; set; }
        public bool IsClosedNotification { get; set; }
        public string NotificationEmails { get; set; }
        public string NotificationMobileNo { get; set; }
        public string ReferenceName { get; set; }
        public long UserKey { get; set; }
        public bool IsUserBlocked { get; set; }
        public List<long> UserKeys { get; set; }
        public List<ReferenceList> ReferenceList { get; set; }
    }

    public class ReferenceList
    {
        public ReferenceList()
        {
            TelephoneCodes = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
        }
        public string ReferenceName { get; set; }

        //[DisplayFormat(ConvertEmptyStringToNull = false)]
        [System.Web.Mvc.Remote("CheckMobileNumberExists", "EnquiryLead", AdditionalFields = "TelephoneCodeKey,RowKey,AcademicTermKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckApplicationPhoneExists")]
        [RequiredIfNot("AcademicTermKey", "", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnquiryPhoneRequired")]
        public string MobileNumber { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public short TelephoneCodeKey { get; set; }
        [RequiredIfNot("MobileNumber", "", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AcademicTermRequired")]
        public short? AcademicTermKey { get; set; }
        public long RowKey { get; set; }
        public List<SelectListModel> TelephoneCodes { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
    }

}
