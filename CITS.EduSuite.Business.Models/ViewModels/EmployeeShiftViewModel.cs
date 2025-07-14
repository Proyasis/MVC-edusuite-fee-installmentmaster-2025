using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeShiftViewModel : BaseModel
    {
        public EmployeeShiftViewModel()
        {
            EmployeeShiftDetailsModel = new List<EmployeeShiftDetailsModel>();
            Shifts = new List<SelectListModel>();
            FromDate = DateTimeUTC.Now;
            ToDate = DateTimeUTC.Now;
        }
        public long RowKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Shift", ResourceType = typeof(EduSuiteUIResources))]
        public int ShiftKey { get; set; }
        public string ShiftName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "FromDate", ResourceType = typeof(EduSuiteUIResources))]

        public DateTime FromDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ToDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime ToDate { get; set; }

        public List<EmployeeShiftDetailsModel> EmployeeShiftDetailsModel { get; set; }
        public List<SelectListModel> Shifts { get; set; }

        [Range(1, short.MaxValue, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "RecordCount", ResourceType = typeof(EduSuiteUIResources))]
        public short RecordCount { get; set; }
        public int? EmployeeCount { get; set; }

    }
    public class EmployeeShiftDetailsModel
    {
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Employee", ResourceType = typeof(EduSuiteUIResources))]
        public long EmployeeKey { get; set; }
        public long EmployeeShiftKey { get; set; }

        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public bool IsActive { get; set; }

        public long? RefKey { get; set; }

      

    }
}
