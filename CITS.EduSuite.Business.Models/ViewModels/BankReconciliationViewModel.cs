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
    public class BankReconciliationViewModel : BaseModel
    {
        public BankReconciliationViewModel()
        {
            BankStatementDetails = new List<BankReconciliationDetailViewModel>();
            DefaultBankPaymentDetails = new List<BankReconciliationDetailViewModel>();
            BankReconciliationDetails = new List<BankReconciliationSelectViewModel>();
            BankAccounts = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            AccountHeads = new List<SelectListModel>();
        }
        
        
        public string BankAccountName { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankAccountRequired")]
        public long BankAccountKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short BranchKey { get; set; }
        public int? ReconcileCount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<BankReconciliationDetailViewModel> BankStatementDetails { get; set; }
        public List<BankReconciliationSelectViewModel> BankReconciliationDetails { get; set; }
        public List<BankReconciliationDetailViewModel> DefaultBankPaymentDetails { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> AccountHeads { get; set; }

    }

    public class BankReconciliationDetailViewModel : BaseModel
    {
        public BankReconciliationDetailViewModel()
        {
        }
        public long RowKey { get; set; }
        public byte TransactionTypeKey { get; set; }
        public long TransactionKey { get; set; }
        public byte ProcessStatusKey { get; set; }
        public long? BankStatementKey { get; set; }

        [RequiredIf("TransactionTypeKey", DbConstants.TransactionType.OtherBankTransaction, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadRequired")]
        public long? AccountHeadKey { get; set; }
        public string ReferenceKey { get; set; }
        public string Particulars { get; set; }
        public string BankTransactionTypeName { get; set; }
        public string CashFlowTypeName { get; set; }
        public byte? BankTransactionTypeKey { get; set; }
        public byte CashFlowTypeKey { get; set; }
        public short? PaymentModeKey { get; set; }
        public decimal Amount { get; set; }
        public bool IsReconcile { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Remark { get; set; }
        public bool? IsAdvance { get; set; }
        public bool? IsOrderPayment { get; set; }
        
    }

    public class BankReconciliationSelectViewModel
    {
        public string Status { get; set; }
        public short ProcessStatusKey { get; set; }
        public string DefaultReferenceKey { get; set; }
        public string DefaultParticulars { get; set; }
        public string DefaultBankTransactionTypeName { get; set; }
        public string DefaultCashFlowTypeName { get; set; }
        public decimal DefaultAmount { get; set; }
        public DateTime DefaultTransactionDate { get; set; }
        public string Remark { get; set; }
        public string StatementReferenceKey { get; set; }
        public string StatementParticulars { get; set; }
        public string StatementBankTransactionTypeName { get; set; }
        public string StatementCashFlowTypeName { get; set; }
        public decimal? StatementAmount { get; set; }
        public DateTime? StatementTransactionDate { get; set; }
    }
}
