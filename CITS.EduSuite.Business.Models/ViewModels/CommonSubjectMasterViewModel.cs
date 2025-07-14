using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class CommonSubjectMasterViewModel : SubjectViewModel
    {
        public CommonSubjectMasterViewModel()
        {

        }
        public long RowKey { get; set; }
        public long SubjectKey { get; set; }
        public bool IsActive { get; set; }

    }
    public class CommonSubjectDetailViewModel
    {

        public long RowKey { get; set; }
        public long CommonSubjectMasterKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseKeyRequired")]
        public long CourseKey { get; set; }
        public string CourseName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeRequired")]
        public short CourseTypeKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "UniversityKeyRequired")]
        public short UniversityKey { get; set; }
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
    }
}
