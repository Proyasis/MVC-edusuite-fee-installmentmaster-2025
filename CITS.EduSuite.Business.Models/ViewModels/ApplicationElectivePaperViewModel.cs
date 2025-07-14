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
    public class ApplicationElectivePaperViewModel : BaseModel
    {
        public ApplicationElectivePaperViewModel()
        {
            ElectivePapers = new List<ElectivePaperViewModel>();
        }
        public long ApplicationKey { get; set; }

        public long CourseKey { get; set; }
        public short AcademicTermKey { get; set; }
        public short UniversityKey { get; set; }


        public List<ElectivePaperViewModel> ElectivePapers { get; set; }
    }
    public class ElectivePaperViewModel
    {
        public ElectivePaperViewModel()
        {
           
        }

        public long RowKey { get; set; }

        public long SubjectKey { get; set; }

        public bool IsActive { get; set; }

        public string SubjectName { get; set; }

        public string SubjectCode { get; set; }
        public short? AcademicTermKey { get; set; }
        public int? CourseDuration { get; set; }
        public int? SubjectYear { get; set; }
        public string SubjectYearText { get; set; }
    }
}
