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
    public class CourseSubjectMasterViewModel : BaseModel
    {
        public CourseSubjectMasterViewModel()
        {
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            CourseYear = new List<SelectListModel>();
            CourseSubjectDetailViewModel = new List<CourseSubjectDetailViewModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseKeyRequired")]
        public long CourseKey { get; set; }
        public string CourseName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeRequired")]
        public short CourseTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "UniversityKeyRequired")]
        public short UniversityMasterKey { get; set; }
        public string UniversityName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SyllabusRequired")]
        public short AcademicTermKey { get; set; }
        public string AcademicTermName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BookYearRequired")]
        public short CourseYearKey { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> CourseYear { get; set; }
        public List<CourseSubjectDetailViewModel> CourseSubjectDetailViewModel { get; set; }

        public int? NoOfSubject { get; set; }
    }
    public class CourseSubjectDetailViewModel : SubjectViewModel
    {
        public long RowKey { get; set; }
        public long CourseSubjectMasterKey { get; set; }
        public long SubjectKey { get; set; }
        public bool IsActive { get; set; }

    }
}
