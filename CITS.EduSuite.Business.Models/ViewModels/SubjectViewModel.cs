using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class SubjectViewModel : BaseModel
    {
        public SubjectViewModel()
        {
            StudyMaterials = new List<StudyMaterialModel>();
            StudyMaterialCount = 1;
        }


        [System.Web.Mvc.Remote("CheckSubjectCodeExist", "CourseSubject", AdditionalFields = "CourseYearKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]

        public string SubjectCode { get; set; }

        [System.Web.Mvc.Remote("CheckSubjectNameExist", "CourseSubject", AdditionalFields = "CourseYearKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]
        public string SubjectName { get; set; }
        public bool IsElective { get; set; }
        public bool HasStudyMaterial { get; set; }

        public short? StudyMaterialCount { get; set; }
        public bool IsCommonSubject { get; set; }

        public List<StudyMaterialModel> StudyMaterials { get; set; }

    }

    public class StudyMaterialModel
    {
        public StudyMaterialModel()
        {
            IsActive = true;
        }
        public long RowKey { get; set; }
        public long SubjectKey { get; set; }


        [System.Web.Mvc.Remote("CheckStudyMaterialCodeExist", "CourseSubject", AdditionalFields = "SubjectKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]

        public string StudyMaterialCode { get; set; }

        [System.Web.Mvc.Remote("CheckStudyMaterialNameExist", "CourseSubject", AdditionalFields = "SubjectKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]
        public string StudyMaterialName { get; set; }
        public bool IsActive { get; set; }

    }
}
