using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class CourseCompletionCertificateViewModel : BaseModel
    {

        public long ApplicationKey { get; set; }
        public long RowKey { get; set; }


        public bool IsIssued { get; set; }
        public long? IssuedBy { get; set; }

        [RequiredIf("IsIssued", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "IssueDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? IssuedDate { get; set; }

        public string Remarks { get; set; }
        public string StudentMobile { get; set; }
        public string StudentEmail { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public short? BranchKey { get; set; }
        public short? BatchKey { get; set; }
    }
}
