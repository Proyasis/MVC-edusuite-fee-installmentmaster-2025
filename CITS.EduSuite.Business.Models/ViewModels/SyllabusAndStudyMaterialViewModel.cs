using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class SyllabusAndStudyMaterialViewModel : BaseModel
    {
        public SyllabusAndStudyMaterialViewModel()
        {
            StudyMaterials = new List<StudyMaterialModel>();
            SubjectModulesModel = new List<SubjectModulesModel>();

            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();          
            CourseYear = new List<SelectListModel>();
        }
        public long? SubjectKey { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string IsElective { get; set; }
        public string HasStudyMaterial { get; set; }
        public string IsCommonSubject { get; set; }
        public short? StudyMaterialCount { get; set; }
        public List<StudyMaterialModel> StudyMaterials { get; set; }
        public List<SubjectModulesModel> SubjectModulesModel { get; set; }
        public string Course { get; set; }
        public long? CourseKey { get; set; }
        public string University { get; set; }
        public short? UniversityMasterKey { get; set; }
        public string SubjectYearText { get; set; }
        public short? SubjectYear { get; set; }
        public int? CourseDuration { get; set; }
        public short? AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }
        public long? CourseSubjectMasterKey { get; set; }

        public string UniversityCourse { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> CourseYear { get; set; }
    }

    public class SubjectModulesModel : BaseModel
    {
        public SubjectModulesModel()
        {
            ModulesTopicModel = new List<ModulesTopicModel>();
            IsActive = true;
            HasTopics = false;
        }
        public long RowKey { get; set; }
        public long SubjectKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ModuleNameRequired")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ModuleNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ModuleNameRegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckModuleNameExist", "SyllabusAndStudyMaterial", AdditionalFields = "SubjectKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ModuleNameExistsErrorMessage")]
        public string ModuleName { get; set; }
        public bool IsActive { get; set; }
        public bool HasTopics { get; set; }
        public int? Duration { get; set; }
        public long? SubjectModuleKey { get; set; }
        public List<ModulesTopicModel> ModulesTopicModel { get; set; }

    }

    public class ModulesTopicModel
    {
        public long RowKey { get; set; }
        public long SubjectModuleKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TopicNameRequired")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TopicNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TopicNameRegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckTopicNameExist", "SyllabusAndStudyMaterial", AdditionalFields = "SubjectModuleKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TopicNameExistsErrorMessage")]
        public string TopicName { get; set; }


    }
}
