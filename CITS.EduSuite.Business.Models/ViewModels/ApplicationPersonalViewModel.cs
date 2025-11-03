using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationPersonalViewModel : BaseModel
    {
        public string ScheduledEmployeeName;

        public ApplicationPersonalViewModel()
        {
            Branches = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            Religions = new List<SelectListModel>();
            Agents = new List<SelectListModel>();
            Countries = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            NatureOfEnquiries = new List<SelectListModel>();
            Medium = new List<SelectListModel>();
            Modes = new List<SelectListModel>();
            ClassModes = new List<SelectListModel>();
            IncomeGroups = new List<SelectListModel>();
            AdmittedYear = new List<SelectListModel>();
            SecondLanguages = new List<SelectListModel>();
            AdmissionFees = new List<AdmissionFeeModel>();
            ClassDetails = new List<SelectListModel>();
            Caste = new List<SelectListModel>();
            BloodGroups = new List<SelectListModel>();
            CommunityTypes = new List<SelectListModel>();
            CurrentYears = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
            EducationTypes = new List<SelectListModel>();
            RegistrationCatagory = new List<SelectListModel>();
            StudentBioMetrics = new List<SelectListModel>();
            DateOfApplication = DateTimeUTC.Now;
            AgeLimitFrom = 0;
            AgeLimitTo = 100;
            StudentClassRequired = true;
            AllowLogin = false;
            CertificateKeys = new List<short>();            
        }

        public List<SelectListModel> NatureOfEnquiries1 { get; set; }

        public long RowKey { get; set; }
        public string AdmissionNo { get; set; }

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

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ApplicantMotherNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ApplicantMotherNameRegularExpressionErrorMessage")]
        public string ApplicantMotherName { get; set; }

        [CustomRequired("PermenantAddressRequired", EnableProprety = "PermenantAddressEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(150, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "PermenantAddress", ResourceType = typeof(EduSuiteUIResources))]
        public string PermenantAddress { get; set; }

        [CustomRequired("PresentAddressRequired", EnableProprety = "PresentAddressEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(150, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "PresentAddress", ResourceType = typeof(EduSuiteUIResources))]
        public string PresentAddress { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [StringLength(15, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "PhoneNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string StudentPhone { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        //[StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        //[RegularExpression(@"^[0-9 +]{10}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [StringLength(15, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "MobileNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string MobileNumber { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ValidExpressionErrorMessage")]
        //[System.Web.Mvc.Remote("CheckEmailExists", "ApplicationPersonal", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckApplicationEmailExists")]
        [Display(Name = "EmailAddress", ResourceType = typeof(EduSuiteUIResources))]
        public string StudentEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "DateOfBirth", ResourceType = typeof(EduSuiteUIResources))]
        // [LessThan("DateTimeNow", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateOfBirthCompareErrorMessage")]
        public DateTime? DateOfBirth { get; set; }

        //[Range(0,100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ApplicantAgeRangeErrorMessage")]
        public int? Age { get; set; }

        public DateTime? DateTimeNow { get { return DateTimeUTC.Now; } }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Gender", ResourceType = typeof(EduSuiteUIResources))]
        public byte Gender { get; set; }


        [CustomRequired("ReligionRequired", EnableProprety = "ReligionEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Religion", ResourceType = typeof(EduSuiteUIResources))]
        public short? ReligionKey { get; set; }
        public string ReligionName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Batch", ResourceType = typeof(EduSuiteUIResources))]
        public short BatchKey { get; set; }
        public string BatchName { get; set; }


        [CustomRequired("NatureOfEnquiryRequired", EnableProprety = "NatureOfEnquiryEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "NatureOfEnquiry", ResourceType = typeof(EduSuiteUIResources))]
        public short? NatureOfEnquiryKey { get; set; }
        public string NatureOfEnquiryName { get; set; }
        public string ApplicantPhoto { get; set; }

        //[LessThan("DateTimeNow", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateOfApplicationCompareErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "DateOfApplication", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? DateOfApplication { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Branch", ResourceType = typeof(EduSuiteUIResources))]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }


        [CustomRequired("AgentRequired", EnableProprety = "AgentEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Agent", ResourceType = typeof(EduSuiteUIResources))]
        public int? AgentKey { get; set; }
        public string AgentName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TotalAmountRequired")]
        //[RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? TotalFeeAmount { get; set; }

        // [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        //[RegularExpressionIf(@"^[^0]+$", "GrandTotalFeeAmount", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TotalAmountRupeeRequired")]
        public decimal? GrandTotalFeeAmount { get; set; }

        [RegularExpressionIf(@"^[^0]+$", "TotalAdmissionFeeAmount", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TotalAmountRupeeRequired")]
        public decimal? TotalAdmissionFeeAmount { get; set; }
        public long? EnquiryKey { get; set; }
        public short ApplicationStatusKey { get; set; }
        public string ApplicationStatusName { get; set; }
        public string Remarks { get; set; }
        public short TelephoneCodeKey { get; set; }
        public string TelephoneCodeName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public int AgeLimitFrom { get; set; }
        public int AgeLimitTo { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public long SerialNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Mode", ResourceType = typeof(EduSuiteUIResources))]
        public short ModeKey { get; set; }
        public int? StartYear { get; set; }

        [CustomRequired("ClassModeRequired", EnableProprety = "ClassModeEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ClassMode", ResourceType = typeof(EduSuiteUIResources))]
        public short? ClassModeKey { get; set; }

        [CustomRequired("IncomeGroupRequired", EnableProprety = "IncomeGroupEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "IncomeGroup", ResourceType = typeof(EduSuiteUIResources))]
        public short? IncomeKey { get; set; }

        [CustomRequired("SecondLanguageRequired", EnableProprety = "SecondLanguageEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "SecoundLanguage", ResourceType = typeof(EduSuiteUIResources))]
        public short? SecondLanguageKey { get; set; }

        [CustomRequired("MediumRequired", EnableProprety = "MediumEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Medium", ResourceType = typeof(EduSuiteUIResources))]
        public short? Mediumkey { get; set; }
        public bool StudentClassRequired { get; set; }
        public bool HasOffer { get; set; }
        public bool HasConcession { get; set; }
        public bool HasInstallment { get; set; }
        public int CourseDuration { get; set; }
        public string OfferName { get; set; }
        public decimal? OfferValue { get; set; }
        public DateTime? OfferDate { get; set; }
        public int? OfferKey { get; set; }
        public bool HasElective { get; set; }
        public string ModeName { get; set; }
        public string ClassRequired { get; set; }
        public string MediumName { get; set; }
        public string ConsessionName { get; set; }

        [CustomRequired("ClassCodeRequired", EnableProprety = "ClassCodeEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ClassCode", ResourceType = typeof(EduSuiteUIResources))]
        public long? ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }
        public int? RollNumber { get; set; }
        public bool ifClassRequired { get; set; }
        public long? UniversityCourseKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CurrentYear", ResourceType = typeof(EduSuiteUIResources))]
        public short? AdmissionCurrentYear { get; set; }
        public short? CurrentYear { get; set; }

        [CustomRequired("CasteRequired", EnableProprety = "CasteEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Caste", ResourceType = typeof(EduSuiteUIResources))]
        public short? CasteKey { get; set; }
        public string CasteName { get; set; }

        [CustomRequired("BloodGroupRequired", EnableProprety = "BloodGroupEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BloodGroup", ResourceType = typeof(EduSuiteUIResources))]
        public byte? BloodGroupKey { get; set; }
        public string BloodGroupName { get; set; }
        public string ApplicationNo { get; set; }
        public bool IsTax { get; set; }
        public string StudentPhotoPath { get; set; }
        public string CurrentYearText { get; set; }
        public bool AllowLogin { get; set; }

        [CustomRequired("CommunityTypeRequired", EnableProprety = "CommunityTypeEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CommunityType", ResourceType = typeof(EduSuiteUIResources))]
        public byte? CommunityTypeKey { get; set; }
        public string CommunityTypeName { get; set; }
        public long? ApplicationKey { get; set; }
        public string RollNoCode { get; set; }
        public string StudentEnrollmentNo { get; set; }
        public string ExamRegisterNo { get; set; }
        public long? EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
        public List<SelectListModel> Employees { get; set; }
        public List<SelectListModel> CurrentYears { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Agents { get; set; }       
        public List<SelectListModel> Religions { get; set; }
        public List<SelectListModel> Countries { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> NatureOfEnquiries { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }        
        public List<SelectListModel> Medium { get; set; }
        public List<SelectListModel> IncomeGroups { get; set; }
        public List<SelectListModel> SecondLanguages { get; set; }
        public List<SelectListModel> Modes { get; set; }
        public List<SelectListModel> ClassModes { get; set; }
        public List<AdmissionFeeModel> AdmissionFees { get; set; }
        public List<SelectListModel> AdmittedYear { get; set; }
        public List<SelectListModel> ClassDetails { get; set; }
        public List<SelectListModel> Caste { get; set; }
        public List<SelectListModel> BloodGroups { get; set; }
        public List<SelectListModel> CommunityTypes { get; set; }
        public List<SelectListModel> EducationTypes { get; set; }
        public List<SelectListModel> RegistrationCatagory { get; set; }
        public bool AllowOldPaid { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "EducationType", ResourceType = typeof(EduSuiteUIResources))]
        public byte EducationTypeKey { get; set; }

        [CustomRequired("RegistrationCatagoryRequired", EnableProprety = "RegistrationCatagoryEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "RegistrationCatagory", ResourceType = typeof(EduSuiteUIResources))]
        public short? RegistrationCatagoryKey { get; set; }
        public long? ApplicationWebFormKey { get; set; }
        public bool AllowTransportation { get; set; }

        [CustomRequired("TransportLocationRequired", EnableProprety = "TransportLocationEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "TransportLocation", ResourceType = typeof(EduSuiteUIResources))]
        public string TransportLocation { get; set; }

        [CustomRequired("PassPortNumberRequired", EnableProprety = "PassPortNumberEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "PassPortNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string PassPortNumber { get; set; }

        [CustomRequired("EmiratesIdRequired", EnableProprety = "EmiratesIdEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "EmiratesId", ResourceType = typeof(EduSuiteUIResources))]
        public string EmiratesId { get; set; }
        public List<SelectListModel> Certificates { get; set; }

        [CustomRequired("CertificatesRequired", EnableProprety = "CertificatesEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Certificate", ResourceType = typeof(EduSuiteUIResources))]
        public List<short> CertificateKeys { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyLogoPath { get; set; }

        [CustomRequired("GuardianMobileRequired", EnableProprety = "GuardianMobileEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(15, MinimumLength = 10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "GuardianMobile", ResourceType = typeof(EduSuiteUIResources))]
        public string GuardianMobile { get; set; }

        public bool HasBioMetric { get; set; }

        [CustomRequired("BioMetricsRequired", EnableProprety = "BioMetricsEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BiomatricID", ResourceType = typeof(EduSuiteUIResources))]
        public long? BioMetricsId { get; set; }
        public List<SelectListModel> StudentBioMetrics { get; set; }
    }
    public class AdmissionFeeModel
    {
        public long RowKey { get; set; }
        public int? AdmissionFeeYear { get; set; }
        public string AdmissionFeeYearText { get; set; }

        public decimal? AdmissionFeeAmount { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ConsessionFeeAmountExpressionErrorMessage")]
        [RequiredIfTrue("HasConcession", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ConsessionFeeAmountRequired")]
        public decimal? ConcessionAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdmissionFeeAmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdmissionFeeAmountExpressionErrorMessage")]
        public decimal? ActualAmount { get; set; }
        public bool HasConcession { get; set; }

        public short? FeeTypeKey { get; set; }
        public string FeeTypeName { get; set; }
        //public decimal? CGSTRate { get; set; }
        //public decimal? SGSTRate { get; set; }
        //public decimal? IGSTRate { get; set; }
        //public decimal? CessRate { get; set; }


        //public decimal? CGSTAmount { get; set; }
        //public decimal? SGSTAmount { get; set; }
        //public decimal? IGSTAmount { get; set; }
        //public decimal? CessAmount { get; set; }

        public bool IsUniversity { get; set; }
        public decimal? CenterShareAmountPer { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "ExistingPaidAmount", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? OldPaid { get; set; }

    }




}
