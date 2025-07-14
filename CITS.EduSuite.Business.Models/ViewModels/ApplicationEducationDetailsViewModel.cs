using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationEducationDetailsViewModel : BaseModel
    {
        public ApplicationEducationDetailsViewModel()
        {
            DocumentTypes = new List<SelectListModel>();
            ApplicationEducationalDetails = new List<EducationDetailViewModel>();
        }
        public long ApplicationKey { get; set; }
        public string AdmissionNo { get; set; }
        public List<SelectListModel> DocumentTypes { get; set; }
        public List<EducationDetailViewModel> ApplicationEducationalDetails { get; set; }

    }
    public class EducationDetailViewModel
    {
        public EducationDetailViewModel()
        {
            
        }
        public long RowKey { get; set; }
        public string DocumentTypeName { get; set; }
        public bool IsOriginal { get; set; }
        public bool IsReturn { get; set; }
        public DateTime? DocumentIssuedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Remarks { get; set; }
        public HttpPostedFileBase DocumentFile { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Course", ResourceType = typeof(EduSuiteUIResources))]
        public string EducationQualificationCourse { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "Board", ResourceType = typeof(EduSuiteUIResources))]
        public string EducationQualificationUniversity { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "CompletedYear", ResourceType = typeof(EduSuiteUIResources))]
        public int? EducationQualificationYear { get; set; }
        public bool EducationQualificationResult { get; set; }
        public long ApplicationKey { get; set; }

        [RegularExpression(@"^(100(\.[0]{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionMarkMessage")]
        [Display(Name = "Mark", ResourceType = typeof(EduSuiteUIResources))]
        public int? EducationQualificationPercentage { get; set; }
        public string EducationQualificationCertificatePath { get; set; }
        public string EducationQualificationCertificatePathText { get; set; }
        public bool IsOriginalIssued { get; set; }
        public bool IsCopyIssued { get; set; }
        public bool IsOriginalReturn { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? OriginalIssuedDate { get; set; }
        public DateTime? CopyIssuedDate { get; set; }
        public DateTime? OriginalReturnDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }

}
