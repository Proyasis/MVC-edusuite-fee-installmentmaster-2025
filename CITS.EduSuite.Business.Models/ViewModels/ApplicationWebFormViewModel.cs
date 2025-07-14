using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationWebFormViewModel : BaseModel
    {
        public ApplicationWebFormViewModel()
        {
            Branches = new List<SelectListModel>();
            Religions = new List<SelectListModel>();
            Agents = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            NatureOfEnquiries = new List<SelectListModel>();
            SecondLanguages = new List<SelectListModel>();
            Caste = new List<SelectListModel>();
            CommunityTypes = new List<SelectListModel>();
            CurrentYears = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
            WebFormStatus = new List<SelectListModel>();
            WebEnquiryStatus = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AcademicTerm", ResourceType = typeof(EduSuiteUIResources))]
        public short? AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CourseType", ResourceType = typeof(EduSuiteUIResources))]
        public short CourseTypeKey { get; set; }
        public string CourseTypeName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Course", ResourceType = typeof(EduSuiteUIResources))]
        public long CourseKey { get; set; }
        public string CourseName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AffiliationsTieUps", ResourceType = typeof(EduSuiteUIResources))]
        public short UniversityKey { get; set; }
        public string UniversityName { get; set; }


        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ApplicantName", ResourceType = typeof(EduSuiteUIResources))]
        public string ApplicantName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ApplicantGuardianNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ApplicantGuardianNameRegularExpressionErrorMessage")]
        public string ApplicantGuardianName { get; set; }


        [CustomRequired("PermenantAddressRequired", EnableProprety = "PermenantAddressEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(150, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "PermenantAddress", ResourceType = typeof(EduSuiteUIResources))]
        public string PermenantAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 +]{10}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [System.Web.Mvc.Remote("CheckPhoneExists", "ApplicationWebForm", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckApplicationPhoneExists")]

        [Display(Name = "MobileNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string MobileNumber { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ValidExpressionErrorMessage")]
        [Display(Name = "EmailAddress", ResourceType = typeof(EduSuiteUIResources))]
        public string StudentEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "DateOfBirth", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public DateTime? DateTimeNow { get { return DateTimeUTC.Now; } }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Gender", ResourceType = typeof(EduSuiteUIResources))]
        public byte Gender { get; set; }


        [CustomRequired("ReligionRequired", EnableProprety = "ReligionEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Religion", ResourceType = typeof(EduSuiteUIResources))]
        public short? ReligionKey { get; set; }
        public string ReligionName { get; set; }

        [CustomRequired("NatureOfEnquiryRequired", EnableProprety = "NatureOfEnquiryEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "NatureOfEnquiry", ResourceType = typeof(EduSuiteUIResources))]
        public short? NatureOfEnquiryKey { get; set; }
        public string NatureOfEnquiryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Branch", ResourceType = typeof(EduSuiteUIResources))]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        [CustomRequired("AgentRequired", EnableProprety = "AgentEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Agent", ResourceType = typeof(EduSuiteUIResources))]
        public int? AgentKey { get; set; }
        public string AgentName { get; set; }
        public long? EnquiryKey { get; set; }
        public string Remarks { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        [CustomRequired("SecondLanguageRequired", EnableProprety = "SecondLanguageEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "SecoundLanguage", ResourceType = typeof(EduSuiteUIResources))]
        public short? SecondLanguageKey { get; set; }
        public bool ConvertedToEnquiry { get; set; }
        public bool ConvertedToApplication { get; set; }
        public bool IsDroped { get; set; }

        [CustomRequired("CasteRequired", EnableProprety = "CasteEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Caste", ResourceType = typeof(EduSuiteUIResources))]
        public short? CasteKey { get; set; }
        public string CasteName { get; set; }

        [CustomRequired("CommunityTypeRequired", EnableProprety = "CommunityTypeEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CommunityType", ResourceType = typeof(EduSuiteUIResources))]
        public byte? CommunityTypeKey { get; set; }
        public string CommunityTypeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? ApplicationKey { get; set; }
        public long? EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public string EnquiryStatusName { get; set; }
        public short? WebFormStatusKey { get; set; }
        public short? EnquiryStatusKey { get; set; }

        [StringLength(15, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "GuardianMobile", ResourceType = typeof(EduSuiteUIResources))]
        public string GuardianMobile { get; set; }
        public List<SelectListModel> Employees { get; set; }
        public List<SelectListModel> CurrentYears { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Agents { get; set; }
        public List<SelectListModel> Religions { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> NatureOfEnquiries { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> SecondLanguages { get; set; }
        public List<SelectListModel> Caste { get; set; }
        public List<SelectListModel> CommunityTypes { get; set; }
        public List<SelectListModel> WebFormStatus { get; set; }
        public List<SelectListModel> WebEnquiryStatus { get; set; }


    }
}
