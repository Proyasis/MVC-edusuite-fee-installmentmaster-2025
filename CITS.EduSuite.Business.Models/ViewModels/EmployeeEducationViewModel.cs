using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.ComponentModel.DataAnnotations;
using System.Web;
namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeEducationViewModel : BaseModel
    {
        public EmployeeEducationViewModel()
        {
            EmployeeEducations = new List<EducationViewModel>();   
        }
        public long EmployeeKey { get; set; }
        public List<EducationViewModel> EmployeeEducations { get; set; }       
    }
    public class EducationViewModel
    {
        public EducationViewModel()
        {
            EducationType = new List<SelectListModel>();
            GradigSystem = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public long EmployeeKey { get; set; }
        public string EducationName { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectNameRegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SubjectNameRequired")]
        public string SubjectName { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CertifiedByLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CertifiedByRegularExpressionErrorMessage")]
        public string CertifiedBy { get; set; }

        [RegularExpression(@"^(100(\.[0]{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionMarkMessage")]
        [Display(Name = "Mark", ResourceType = typeof(EduSuiteUIResources))]
        public string Mark { get; set; }
        public string EducationTypeName { get; set; }
        public string GradeName { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "CompletedYear", ResourceType = typeof(EduSuiteUIResources))]
        public short? CompletedYear { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EducationTypeRequired")]
        [System.Web.Mvc.Remote("CheckEducationTypeExists", "EmployeeEducation", AdditionalFields = "EmployeeKey,RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeEducationExists")]
        public short EducationTypeKey { get; set; }
        public short? GradigSystemKey { get; set; }     
        public HttpPostedFileBase AttanchedFile { get; set; }
        public string AttanchedFileName { get; set; }
        public string AttanchedFileNamePath { get; set; }
        public List<SelectListModel> EducationType { get; set; }
        public List<SelectListModel> GradigSystem { get; set; }
    }
}
