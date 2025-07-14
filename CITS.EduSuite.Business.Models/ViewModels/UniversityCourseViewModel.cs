using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class UniversityCourseViewModel : BaseModel
    {
        public UniversityCourseViewModel()
        {
            IsActive = true;
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            DivisionList = new List<SelectListModel>();
            BuildingDetailsList = new List<SelectListModel>();
            YearList = new List<SelectListModel>();
            FeeTypes = new List<SelectListModel>();
            IsUpdate = false;
        }

        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CourseType", ResourceType = typeof(EduSuiteUIResources))]
        public short CourseTypeKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AffiliationsTieUps", ResourceType = typeof(EduSuiteUIResources))]
        public short UniversityMasterKey { get; set; }
        public string UniversityName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Course", ResourceType = typeof(EduSuiteUIResources))]
        public long CourseKey { get; set; }
        public string CourseName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AcademicTerm", ResourceType = typeof(EduSuiteUIResources))]
        public short AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }

        public bool IsActive { get; set; }
        public bool? IsUpdate { get; set; }

        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> YearList { get; set; }
        public List<SelectListModel> DivisionList { get; set; }
        public List<SelectListModel> BuildingDetailsList { get; set; }
        public List<SelectListModel> FeeTypes { get; set; }
        public short? StudentYear { get; set; }
        public List<ClassDetailsModel> ClassDetailsModel { get; set; }
        public List<UniversityCourseFeeModel> UniversityCourseFeeModel { get; set; }
        public List<UniversityCourseFeeInstallmentModel> UniversityCourseFeeInstallmentModel { get; set; }
        public string ClassName { get; set; }
        public long? ClassDetailsKey { get; set; }
        public decimal? TotalUniversityCoursefee { get; set; }
        public decimal? TotalCenterShareAmountPer { get; set; }
        public string Duration { get; set; }
        public int? CourseDuration { get; set; }
        public bool AllowCenterShare { get; set; }
        public short? SearchCourseTypeKey { get; set; }
        public short? SearchUniversityMasterKey { get; set; }
        public long? SearchCourseKey { get; set; }
        public short? SearchAcademicTermKey { get; set; }
        public string SearchText { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public short? DurationTypeKey { get; set; }
        public int? DurationCount { get; set; }

    }

    public class ClassDetailsModel
    {
        public ClassDetailsModel()
        {
            IsActive = true;
        }

        public long RowKey { get; set; }
        public long UniversityCourseKey { get; set; }

        [System.Web.Mvc.Remote("CheckDivisionKey", "UniversityCourse", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "Division", ResourceType = typeof(EduSuiteUIResources))]
        public short? DivisionKey { get; set; }
        public short StudentYear { get; set; }
        public string StudentYearText { get; set; }

        [System.Web.Mvc.Remote("CheckClassCodeExists", "UniversityCourse", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassCodeExists")]

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ClassCode", ResourceType = typeof(EduSuiteUIResources))]
        public string ClassCode { get; set; }

        [System.Web.Mvc.Remote("CheckRoomNoExists", "UniversityCourse", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "Building", ResourceType = typeof(EduSuiteUIResources))]
        public long? BuildingDetailsKey { get; set; }

        public bool IsActive { get; set; }
        public long ClassDetialsKey { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? LateMinutes { get; set; }

    }
    public class UniversityCourseFeeModel
    {
        public UniversityCourseFeeModel()
        {

        }

        public long RowKey { get; set; }
        public long UniversityCourseKey { get; set; }


        [System.Web.Mvc.Remote("CheckFeeTypeExists", "ApplicationFeePayment", AdditionalFields = "FeeYear", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "FeeType", ResourceType = typeof(EduSuiteUIResources))]
        public short? FeeTypeKey { get; set; }
        public short? FeeYear { get; set; }
        public string StudentYearText { get; set; }

        [RequiredIfTrue("IsActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "Amount", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? FeeAmount { get; set; }

        public bool IsCenterShare { get { return (IsUniversity && IsActive && AllowCenterShare); } set { } }


        [RequiredIfTrue("IsCenterShare", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        //[RequiredIf("IsUniversity", "IsActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"(^100(\.0{1,2})?$)|(^([1-9]([0-9])?|0)(\.[0-9]{1,2})?$)", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "UniversityAmountPerExpressionErrorMessage")]
        [Display(Name = "CenterShare", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? CenterShareAmountPer { get; set; }
        public decimal? CenterShareAmount { get; set; }
        public bool IsActive { get; set; }
        public string FeeYearText { get; set; }
        public string FeeTypeName { get; set; }
        public bool IsUniversity { get; set; }
        public bool AllowCenterShare { get; set; }

    }

    public class UniversityCourseFeeInstallmentModel : BaseModel
    {
        public UniversityCourseFeeInstallmentModel()
        {
            FeeYears = new List<SelectListModel>();
            UniversityCourseFeeInstallments = new List<FeeInstallmentModel>();
            InstallMentMonth = new List<SelectListModel>();
        }

        public long UniversityCourseKey { get; set; }
        public List<SelectListModel> FeeYears { get; set; }
        public int? FeeYear { get; set; }
        public int? StartYear { get; set; }

        public List<FeeInstallmentModel> UniversityCourseFeeInstallments { get; set; }
        public decimal? FeeAmount { get; set; }
        public decimal? BalancePayment { get; set; }

        public decimal? TotalFeeAmount { get; set; }

        [EqualTo("TotalFeeAmount", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InitialPaymentLessThanErrorMessage")]
        public decimal? TotalInstallmentFee { get; set; }


        public List<SelectListModel> InstallMentMonth { get; set; }
    }
}
