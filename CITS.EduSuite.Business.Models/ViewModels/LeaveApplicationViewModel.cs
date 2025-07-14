using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class LeaveApplicationViewModel : BaseModel
    {
        public LeaveApplicationViewModel()
        {
            LeaveStatuses = new List<SelectListModel>();
            LeaveTypes = new List<SelectListModel>();
            LeaveDurationTypes = new List<SelectListModel>();
            Employees = new List<GroupSelectListModel>();
            Branches = new List<SelectListModel>();
            LeaveDurationTypeKey = DbConstants.LeaveDurationType.MultipleDays;
        }
        public Int64 RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public long EmployeeKey { get; set; }

        public string EmployeeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveDateRequired")]
        public DateTime? LeaveFrom { get; set; }

        [RequiredIf("LeaveDurationTypeKey",DbConstants.LeaveDurationType.MultipleDays, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveDateRequired")]
        [GreaterThan("LeaveFrom",PassOnNull=true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveToCompareErrorMessage")]
        public DateTime? LeaveTo { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveTypeRequired")]
        public Int16 LeaveTypeKey { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveReasonLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveReasonRegularExpressionErrorMessage")]
        public string LeaveReason { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveStatusRequired")]
        public Int16 LeaveStatusKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LeaveDurationTypeRequired")]
        public byte LeaveDurationTypeKey { get; set; }

        public string LeaveTypeName { get; set; }
        public string LeaveDurationTypeName { get; set; }
        public string LeaveStatusName { get; set; }

        public int? LeaveTypeCount { get; set; }
        public int? LeaveUsedCount { get; set; }
        public int? LeaveAvailableCount { get; set; }
        public bool SalaryDeductionForAdditional { get; set; }



        public List<SelectListModel> LeaveTypes { get; set; }
        public List<SelectListModel> LeaveDurationTypes { get; set; }
        public List<SelectListModel> LeaveStatuses { get; set; }
        public List<GroupSelectListModel> Employees { get; set; }
        public List<SelectListModel> Branches { get; set; }
    }

}
