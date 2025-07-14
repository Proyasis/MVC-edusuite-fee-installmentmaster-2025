using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class CourseViewModel : BaseModel
    {
        public CourseViewModel()
        {
            IsActive = true;
            CourseTypeList = new List<SelectListModel>();
            CourseDurationTypeList = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckCourseCodeExists", "Course", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]
        public string CourseCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckCourseNameExists", "Course", AdditionalFields = "RowKey,CourseTypeKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]
        public string CourseName { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseYearExpressionErrorMessage")]
        public int? CourseYear { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CourseType", ResourceType = typeof(EduSuiteUIResources))]
        public short CourseTypeKey { get; set; }               
        public int? CourseDuration { get; set; }
        public string CourseTypeName { get; set; }
        public List<SelectListModel> CourseTypeList { get; set; }
        public long? UniversityCourseKey { get; set; }
        public short? SearchCourseTypeKey { get; set; }
        public string SearchText { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "DurationType", ResourceType = typeof(EduSuiteUIResources))]
        public short? DurationTypeKey { get; set; }

        [RegularExpression(@"^[1-9][0-9]?$|^200$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Duration", ResourceType = typeof(EduSuiteUIResources))]
        public int? DurationCount { get; set; }
        public List<SelectListModel> CourseDurationTypeList { get; set; }


    }
}
