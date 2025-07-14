using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CITS.EduSuite.Business.Models.ViewModels
{
  public  class StudentStudyMaterialViewModel:BaseModel
    {
        public StudentStudyMaterialViewModel()
        {

            Subjects = new List<SelectListModel>();
            SubjectsList = new List<StudyMaterialCategoryList>();
            VisibilityTypes = new List<SelectListModel>();
            StudentStudyMaterialDetailsList = new List<StudentStudyMaterialDetailsViewModel>();
        }
        public long RowKey { get; set; }
        //public long StudyMaterialDetailsKey { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        public string SubjectName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public long SubjectKey { get; set; }
        public int StudyMaterialCount { get; set; }
    

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        public string StudyMaterialTitle { get; set; }

        [StringLength(1000, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        public string StudyMaterialDecription { get; set; }
        //public string StudyMaterialFileName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
       
        public string SearchText { get; set; }
        public string PlanKeysList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int UserKey { get; set; }
        public short RoleKey { get; set; }
        public bool IsActive { get; set; }
       

        
        public List<SelectListModel> Subjects { get; set; }
        public List<StudyMaterialCategoryList> SubjectsList { get; set; }

        public List<StudentStudyMaterialViewModel> StudyMaterials { get; set; }
        public List<SelectListModel> VisibilityTypes { get; set; }

        public List<StudyMaterialsList> StudyMaterialsList { get; set; }
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public List<byte> PlanKeys { get; set; }

        public List<StudentStudyMaterialDetailsViewModel> StudentStudyMaterialDetailsList { get; set; }

    }
    public class StudyMaterialsList
    {
        public string StudyMaterialDecription { get; set; }
    }




    public class StudyMaterialCategoryList
    {
        public long SubjectKey { get; set; }
        public string SubjectName { get; set; }
        public bool IsActive { get; set; }
    }

    public class StudentStudyMaterialDetailsViewModel
    {      
        public long RowKey { get; set; }
        public long StudentStudyMaterialKey { get; set; }
        public string StudyMaterialFilePath { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public string StudyMaterialName { get; set; }
        public short? VisibilityTypeKey { get; set; }
        public string SubjectName { get; set; }
        public bool IsActive { get; set; }
        public string VideoTitle { get; set; }
        public string VisibilityName { get; set; }
        public long? SubjectKey { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsAllowDownload { get; set; }

        [RequiredIf("RowKey", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        public HttpPostedFileBase StudyMaterialFileAttachment { get; set; }

        public bool IsAllowPreview { get; set; }

        public int? StudyMaterialViewCount { get; set; }
        public int? StudyMaterialDownloadCount { get; set; }
    }


}
