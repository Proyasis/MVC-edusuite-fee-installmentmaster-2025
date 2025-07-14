using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeTaskViewModel : BaseModel
    {
        public EmployeeTaskViewModel()
        {
            Employees = new List<GroupSelectListModel>();
            EmployeeNames = new List<SelectListModel>();
            Priorities = new List<SelectListModel>();
            TaskStatuses = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
        }

        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public Int64 EmployeeKey { get; set; }
        public string EmployeeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaskTitleRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaskTitleLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaskTitleRegularExpressionErrorMessage")]
        public string TaskTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StartDateRequired")]
        public DateTime? StartDate { get; set; }

        //[System.Web.Mvc.Remote("CheckEmployeeTaskDate", "EmployeeTask", AdditionalFields = "StartDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeTaskDateCompareMessage")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PriorityRequired")]
        public byte PriorityKey { get; set; }

        public string PriorityName { get; set; }

        public string TaskStatusName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaskStatusRequired")]
        public byte TaskStatusKey { get; set; }

        public short ApprovalStatusKey { get; set; }
        public string ApprovalStatusName { get; set; }

        public List<GroupSelectListModel> Employees { get; set; }

        public List<SelectListModel> Priorities { get; set; }

        public List<SelectListModel> TaskStatuses { get; set; }
        public List<SelectListModel> ApproveStatuses { get; set; }
        public List<SelectListModel> EmployeeNames { get; set; }
        public List<SelectListModel> Branches { get; set; }
    }
}
