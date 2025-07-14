using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class StudyMaterialViewModel : BaseModel
    {
        public StudyMaterialViewModel()
        {
            SubjectYears = new List<SelectListModel>();
            StudyMaterialList = new List<StudyMaterialDetailsModel>();
            Branches = new List<SelectListModel>();
        }

        public List<SelectListModel> SubjectYears { get; set; }
        public long ApplicationKey { get; set; }

        public long CourseKey { get; set; }
        public List<StudyMaterialDetailsModel> StudyMaterialList { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string StudyMaterialName { get; set; }
        public int? SubjectYear { get; set; }
        public string StudyMaterialKeys { get; set; }
        public short? RoleKey { get; set; }
        public long? UserKey { get; set; }
        public List<SelectListModel> Branches { get; set; }
    }
    public class StudyMaterialDetailsModel
    {
        public StudyMaterialDetailsModel()
        {

        }
        public long RowKey { get; set; }
        public string StudyMaterialName { get; set; }
        public int? SubjectYear { get; set; }
        public string StudyMaterialCode { get; set; }
        public long StudyMaterialKey { get; set; }
        public bool IsAvailable { get; set; }
        public long? AvailableBy { get; set; }
        public DateTime? AvailableDate { get; set; }
        public bool IsIssued { get; set; }
        public long? IssuedBy { get; set; }
        public DateTime? IssuedDate { get; set; }

        public string StudyMaterialStatusName { get; set; }
        public string StudyMaterialStatusBy { get; set; }
        public string SubjectYearText { get; set; }
        public string SubjectType { get; set; }
        public string IssuedByText { get; set; }
        public string AvailableByText { get; set; }


        public short? AcademicTermKey { get; set; }
        public int? CourseDuration { get; set; }

        public long? ApplicationKey { get; set; }

    }
}
