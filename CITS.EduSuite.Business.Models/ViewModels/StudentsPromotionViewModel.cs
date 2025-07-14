using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using System.ComponentModel.DataAnnotations;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class StudentsPromotionViewModel : BaseModel
    {
        public StudentsPromotionViewModel()
        {
            CourseTypes = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            PromotionStatus = new List<SelectListModel>();
            StudentsPromotionDetails = new List<StudentsPromotionDetailsViewModel>();
            PromotedClasses = new List<SelectListModel>();
            ClassDetails = new List<SelectListModel>();

        }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> PromotionStatus { get; set; }
        public List<StudentsPromotionDetailsViewModel> StudentsPromotionDetails { get; set; }
        public List<SelectListModel> PromotedClasses { get; set; }

        public List<SelectListModel> ClassDetails { get; set; }

        public long RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassDetailsRequired")]
        public long ClassDetailsKey { get; set; }
        public string ClassDetailsName { get; set; }

        public short? CourseYear { get; set; }
        public string CourseYearName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        public int? EmployeeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeRequired")]
        public short CourseTypeKey { get; set; }

        public string UniversityName { get; set; }

        public string CourseName { get; set; }

        public short? AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }

        public bool IsTeacher { get; set; }

          [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BatchRequired")]
        public short BatchKey { get; set; }
        public string BatchName { get; set; }

        public int PromotedCount { get; set; }
        public int CompletedCount { get; set; }
        public int DiscontinuedCount { get; set; }
        public int TotalStudentCount { get; set; }


        public int? CourseDuration { get; set; }

        public bool? IfEdit { get; set; }
        public bool? IsUpdate { get; set; }


        public short? AllPromotionStatusKey { get; set; }
        public short? AllPromotedClassDetailsKey { get; set; }
        public long? UniversityCourseKey { get; set; }

        public long? CurrentClassDetailsKey { get; set; }
        public short? CurrentYear { get; set; }
    }

    public class StudentsPromotionDetailsViewModel
    {

        public long RowKey { get; set; }
        public long ApplicationKey { get; set; }
        public short? PromotionStatusKey { get; set; }

        [RequiredIf("PromotionStatusKey", DbConstants.PromotionStatus.Promoted, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PromotedDivisionRequired")]
        public long? PromotedClassDetailsKey { get; set; }
        public short? PromotedYear { get; set; }

        public bool IsActive { get; set; }

        public int RollNumber { get; set; }
        public short? CurrentYear { get; set; }
        public string CurrentYearText { get; set; }
        public string AdmissionNo { get; set; }
        public string Name { get; set; }

        public string ClassCode { get; set; }
        public short? AcademicTermKey { get; set; }
        public int? CourseDuration { get; set; }
    }
}
