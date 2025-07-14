using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
   public class BankAccountViewModel:BaseModel
    {
        public BankAccountViewModel()
        {
            AccountTypes = new List<SelectListModel>();
            Banks = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            BankBranches = new List<SelectListModel>();
            BranchKeys = new List<short>();
            IsActive = true;
        }

        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public short BankKey { get; set; }
        public string BankName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchLocationRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchLocationLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchLocationRegularExpressionErrorMessage")]
        public string BranchLocation { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IFSCCodeRequired")]
        [StringLength(11, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IFSCCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9  a-zA-Z]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IFSCCodeLengthRegularExpressionErrorMessage")]
        public string IFSCCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountNumberRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountNumberLengthRegularExpressionErrorMessage")]
        public string AccountNumber { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NameInAccountLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NameInAccountRegularExpressionErrorMessage")]
        public string NameInAccount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountTypeRequired")]
        public short AccountTypeKey { get; set; }
        public string AccountTypeName { get; set; }

        [StringLength(9, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MICRCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MICRCodeRegularExpressionErrorMessage")]
        public string MICRCode { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? OpeningAccountBalance { get; set; }
        public decimal? CurrentAccountBalance { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
        public decimal? Amount { get; set; }
        public List<SelectListModel> AccountTypes { get; set; }
        public List<SelectListModel> Banks { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> BankBranches { get; set; }                
        public List<short> BranchKeys { get; set; }
        public string BranchBank { get; set; }
        public string searchText { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

    }
}
