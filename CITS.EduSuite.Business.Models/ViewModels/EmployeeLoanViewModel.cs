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
    public class EmployeeLoanViewModel : BaseModel
    {
        public EmployeeLoanViewModel()
        {
            Employees = new List<GroupSelectListModel>();
            Branches = new List<SelectListModel>();
        }

        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeRequired")]
        public long EmployeeKey { get; set; }
        public string EmployeeName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RemarksLengthErrorMessage")]
        public string Remarks { get; set; }


        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LoanAmountExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LoanAmount_Required")]
        public decimal? Amount { get; set; }
     
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LoanInPaySlip_Required")]
        public bool LoanInPaySlip { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MonthlyRepaymentAmount_Required")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MonthlyRepaymentAmountExpressionErrorMessage")]
        public decimal? MonthlyRepaymentAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LoanDate_Required")]
        public DateTime? LoanDate { get; set; }

        [GreaterThanOrEqualTo("LoanDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RepaymentStartDateCompareErrorMessage")]
        public DateTime? RepaymentStartDate { get; set; }
        public short LoanStatusKey { get; set; }

        public string LoanStatusName { get; set; }


        public List<GroupSelectListModel> Employees { get; set; }
        public List<SelectListModel> Branches { get; set; }

    }
}
