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
    public class BankStatementMasterViewModel : BaseModel
    {
        public BankStatementMasterViewModel()
        {
            BankStatementDetails = new List<BankStatementDetailsViewModel>();
            BankAccounts = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
        }

        public long RowKey { get; set; }
        
        public string BankAccountName { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankAccountRequired")]
        public long BankAccountKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short BranchKey { get; set; }
        public byte? MonthKey { get; set; }
        public int? YearKey { get; set; }
        public List<BankStatementDetailsViewModel> BankStatementDetails { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<SelectListModel> Branches { get; set; }

        public DateTime? BankStatementMonth
        {
            get
            {
                if ((YearKey ?? 0) != 0 || (MonthKey ?? 0) != 0)
                {
                    return new DateTime(YearKey ?? 0, MonthKey ?? 0, 01);
                }
                else
                {
                    return DateTimeUTC.Now;
                }
            }
            set
            {

            }
        }
    }
    public class BankStatementDetailsViewModel : BaseModel
    {
        public BankStatementDetailsViewModel()
        {
            BankTransactionTypes = new List<GroupSelectListModel>();
            CashFlowTypes = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        public string ReferenceKey { get; set; }
        public string Particulars { get; set; }
        public byte? BankTransactionTypeKey { get; set; }
        public byte UpdateType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowTypeKeyRequired")]
        public byte CashFlowTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowAmountRequired")]
        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowAmountRegularExpressionErrorMessage")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankTransactionDateRequired")]
        public DateTime? TransactionDate { get; set; }
        public List<GroupSelectListModel> BankTransactionTypes { get; set; }
        public List<SelectListModel> CashFlowTypes { get; set; }
    }
}
