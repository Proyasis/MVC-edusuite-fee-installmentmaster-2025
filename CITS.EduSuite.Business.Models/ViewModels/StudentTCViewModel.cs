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
    public class StudentTCViewModel : BaseModel
    {
        public StudentTCViewModel()
        {
            StudentTCDetailsViewModel = new List<StudentTCDetailsViewModel>();
            ReasonMaster = new List<SelectListModel>();
        }
        public long ApplicationKey { get; set; }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "DateOfApplication", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? DateOfApplicationForTC { get; set; }
        public bool IsIssued { get; set; }
        public long? IssuedBy { get; set; }

        [RequiredIf("IsIssued", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "IssueDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? IssuedDate { get; set; }
        public bool IsGenerate { get; set; }
        public long? GeneratedBy { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "GenerateDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? GeneratedDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "LeaveReason", ResourceType = typeof(EduSuiteUIResources))]
        public short? ReasonMasterKey { get; set; }
        public string ReasonMasterName { get; set; }
        public bool IsActive { get; set; }

        public short? BranchKey { get; set; }
        public short? BatchKey { get; set; }

        public List<SelectListModel> ReasonMaster { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Batches { get; set; }

        public List<StudentTCDetailsViewModel> StudentTCDetailsViewModel { get; set; }

        public string StudentEmail { get; set; }
        public string StudentMobile { get; set; }
        public string StudentGuardianPhone { get; set; }
    }

    public class StudentTCDetailsViewModel : BaseModel
    {
        public StudentTCDetailsViewModel()
        {
            PersonalDetails = new ApplicationPersonalViewModel();
            OptionalValues = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public long TCMasterKey { get; set; }
        public short? TCConfigColumnKey { get; set; }

        [RequiredIf("IsMandatory", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Answer", ResourceType = typeof(EduSuiteUIResources))]
        public string Value { get; set; }
        public bool IsActive { get; set; }

        public string TCConfigColumnName { get; set; }

        public short? CondentTypeKey { get; set; }

        public string TCConfigDescreption { get; set; }

        public bool? IsMandatory { get; set; }
        public bool? IsDeletable { get; set; }

        public string ColumnValue { get; set; }


        public List<SelectListModel> OptionalValues { get; set; }
        public ApplicationPersonalViewModel PersonalDetails { get; set; }
    }

}
