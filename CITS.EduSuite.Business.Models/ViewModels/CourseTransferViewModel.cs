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
    public class CourseTransferViewModel : BaseModel
    {
        public CourseTransferViewModel()
        {
            AcademicTerms = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            IsChangeAdmissionNo = false;
        }

        public long RowKey { get; set; }

        public long? FromCourseKey { get; set; }
        public short? FromUniverisityKey { get; set; }
        public short? FromAcademicTermKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Remarks", ResourceType = typeof(EduSuiteUIResources))]
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public long? ApplicationKey { get; set; }
        public string AdmissionNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AcademicTerm", ResourceType = typeof(EduSuiteUIResources))]
        public short? ToAcademicTermKey { get; set; }
        public string ToAcademicTermName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "CourseType", ResourceType = typeof(EduSuiteUIResources))]
        public short? CourseTypeKey { get; set; }
        public string CourseTypeName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Course", ResourceType = typeof(EduSuiteUIResources))]
        public long? ToCourseKey { get; set; }
        public string ToCourseName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AffiliationsTieUps", ResourceType = typeof(EduSuiteUIResources))]
        public short? ToUniverisityKey { get; set; }
        public string ToUniversityName { get; set; }

        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public short? BranchKey { get; set; }
        public short? BatchKey { get; set; }
        public string StudentMobile { get; set; }
        public string StudentEmail { get; set; }
        public bool IsChangeAdmissionNo { get; set; }
        public string FromAcademicTermName { get; set; }
        public string FromUniversityName { get; set; }
        public string FromCourseName { get; set; }
        public DateTime? TransferDate { get; set; }
    }
}
