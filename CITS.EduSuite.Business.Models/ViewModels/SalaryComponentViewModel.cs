using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class SalaryComponentViewModel:BaseModel
    {
        public SalaryComponentViewModel()
        {
            SalaryComponentTypes = new List<GroupSelectListModel>();
            Employees = new List<GroupSelectListModel>();
            SalaryComponentDate = DateTimeUTC.Now;
            Branches = new List<SelectListModel>();
        }

        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public Int64 EmployeeKey { get; set; }

        public string EmployeeName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SalaryComponentTypeRequired")]
        public short SalaryComponentTypeKey { get; set; }
        public string SalaryComponentTypeName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ComponentTitleRequired")]
        //[StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ComponentTitleLengthErrorMessage")]
        //[RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ComponentTitleRegularExpressionErrorMessage")]
       
        //public string ComponentTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? AmountUnit { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SalaryComponentDateRequired")]
        public DateTime? SalaryComponentDate { get; set; }
        public string OperationType { get; set; }

        public short ApprovalStatusKey { get; set; }
        public string ApprovalStatusName { get; set; }


        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SalaryComponentRemarksLengthErrorMessage")]
        
        public string Remarks { get; set; }

        public List<GroupSelectListModel> SalaryComponentTypes { get; set; }
        public List<GroupSelectListModel> Employees { get; set; }
        public List<SelectListModel> Branches { get; set; }
    }
}
