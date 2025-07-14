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
    public class AccountTransactionViewModel : BaseModel
    {
        public AccountTransactionViewModel()
        {
            Parties = new List<SelectListModel>();
            PartyTypes = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            AccountLedgers = new List<SelectListModel>();
            TransactionDate = DateTimeUTC.Now;
        }
        public long RowKey { get; set; }
        public byte TransactionTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountLedgerRequired")]
        public int AccountLedgerKey { get; set; }
        public string AccountLedgerName { get; set; }
        public DateTime? TransactionDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PartyKeyRequired")]
        public long PartyKey { get; set; }
        public string PartyName { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PartyTypeKeyRequired")]
        public byte PartyTypeKey { get; set; }
        public string PartyTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountRequired")]
        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? Amount { get; set; }

        [GreaterThan("TransactionDate", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TransactionDueDateCompareErrorMessage")]
        public DateTime? TransactionDueDate { get; set; }

        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? DueFineAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public short TransactionStatusKey { get; set; }
        public string TransactionStatusName { get; set; }
        public string Remarks { get; set; }

        public List<SelectListModel> AccountLedgers { get; set; }
        public List<SelectListModel> PartyTypes { get; set; }

        public List<SelectListModel> Parties { get; set; }

        public List<SelectListModel> Branches { get; set; }
    }
}
