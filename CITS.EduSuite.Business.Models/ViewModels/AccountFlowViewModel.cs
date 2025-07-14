using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class AccountFlowViewModel : BaseModel
    {
        public AccountFlowViewModel()
        {
            CreditAccountFlow = new List<AccountFlowViewModel>();
            DebitAccountFlow = new List<AccountFlowViewModel>();
            AccountHead = new List<SelectListModel>();
            Date = DateTimeUTC.Now;
            FromDate = DateTimeUTC.Now;
            ToDate = DateTimeUTC.Now;
            BankAccounts = new List<SelectListModel>();
            GSTEFilingList = new List<GSTEFilingViewModel>();
            GSTEFilingTotalList = new List<GSTEFilingTotalViewModel>();
            Branches = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public short? BranchKey { get; set; }
        //public string AccountHeadCode { get; set; }
      
        public long? OldAccountHeadKey { get; set; }
        public string AccountHeadName { get; set; }
        public string Purpose { get; set; }
        public string OrderNumber { get; set; }
        public string OtherChargeName { get; set; }
        public string PartyName { get; set; }
       
        public string MobileNumber { get; set; }
        public string Status { get; set; }
        public string OldPurpose { get; set; }
        public decimal Amount { get; set; }
        public decimal? TotalDebitAmount { get; set; }
        public decimal? TotalCreditAmount { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? ClosingBalance { get; set; }
        public byte OpeningBalanceTypeKey { get; set; }
        public byte ClosingBalanceKey { get; set; }
        public byte CashFlowTypeKey { get; set; }
        public long? AppUserKey { get; set; }
        public byte TransactionTypeKey { get; set; }
        public long TransactionKey { get; set; }
        public short oldCashFlowTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadRequired")]
        public long AccountHeadKey { get; set; }
        public List<long> TransactionKeyList { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime ClosingBalanceDate { get; set; }
        public DateTime OpeningBalanceDate { get; set; }
        public DateTime NextDate { get; set; }
        public long ExtraUpdateKey { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? Date { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankAccountRequired")]
        //public long? AccountHeadKey { get; set; }

        [RequiredIfNot("ToDate", null, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FromDateRequired")]
        public DateTime? FromDate { get; set; }

        [GreaterThanOrEqualTo("FromDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GreaterThanFromDateErrorMessage")]
        public DateTime? ToDate { get; set; }

        public byte VoucherTypeKey { get; set; }
        public string VoucherTypeName { get; set; }
        public string AccountHeadTypeName { get; set; }
        public int AccountHeadTypeKey { get; set; }
        public int? DisplayOrder { get; set; }

        public bool? IsDummy { get; set; }
        public List<AccountFlowViewModel> CreditAccountFlow { get; set; }
        public List<AccountFlowViewModel> DebitAccountFlow { get; set; }
        public List<SelectListModel> AccountHead { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<GSTEFilingViewModel> GSTEFilingList { get; set; }
        public List<GSTEFilingTotalViewModel> GSTEFilingTotalList { get; set; }
        public List<SelectListModel> Branches { get; set; }

    }
    public class GSTEFilingViewModel
    {
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string PartyName { get; set; }
        public string GSTINNumber { get; set; }
        public int GSTFlow { get; set; }
        public string HSNCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Total { get; set; }
        public decimal? CGSTPer { get; set; }
        public decimal? SGSTPer { get; set; }
        public decimal? IGSTPer { get; set; }
        public decimal? CGSTAmt { get; set; }
        public decimal? SGSTAmt { get; set; }
        public decimal? IGSTAmt { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? NetAmount { get; set; }


    }

    public class GSTEFilingTotalViewModel : BaseModel
    {
        public GSTEFilingTotalViewModel()
        {
            Branches = new List<SelectListModel>();
        }
        public decimal? OutputCGSTAmt { get; set; }
        public decimal? OutputSGSTAmt { get; set; }
        public decimal? OutputIGSTAmt { get; set; }
        public decimal? InputCGSTAmt { get; set; }
        public decimal? InputSGSTAmt { get; set; }
        public decimal? InputIGSTAmt { get; set; }
        public decimal? TotalInputCGSTAmt { get; set; }
        public decimal? TotalInputSGSTAmt { get; set; }
        public decimal? TotalInputIGSTAmt { get; set; }
        public decimal? BalanceICGSTAmt { get; set; }
        public decimal? BalanceISGSTAmt { get; set; }
        public decimal? BalanceIIGSTAmt { get; set; }
        public decimal? BalancePaidCGSTAmt { get; set; }
        public decimal? BalancePaidSGSTAmt { get; set; }
        public decimal? BalancePaidIGSTAmt { get; set; }
        public decimal? RecievableIGSTAmt { get; set; }
        public decimal? RecievableSGSTAmt { get; set; }
        public decimal? RecievableCGSTAmt { get; set; }

        #region GetBranch
        
        public List<SelectListModel> Branches { get; set; }
        public short BranchKey { get; set; }
        #endregion
    }

    public class ProfitAndLossAccountViewModel : BaseModel
    {
        public ProfitAndLossAccountViewModel()
        {
            IndirectExpenses = new List<ProfitAndLossAccountDetailsViewModel>();
            IndirectIncomes = new List<ProfitAndLossAccountDetailsViewModel>();
            DirectExpenses = new List<ProfitAndLossAccountDetailsViewModel>();
            DirectIncomes = new List<ProfitAndLossAccountDetailsViewModel>();
        }
        public decimal Net { get; set; }
        public decimal Gross { get; set; }
        public short? BranchKey { get; set; }
        public long? UserKey { get; set; }

        [RequiredIfNot("ToDate", null, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FromDateRequired")]
        public DateTime? FromDate { get; set; }

        [GreaterThanOrEqualTo("FromDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GreaterThanFromDateErrorMessage")]
        public DateTime? ToDate { get; set; }
        public List<ProfitAndLossAccountDetailsViewModel> IndirectIncomes { get; set; }
        public List<ProfitAndLossAccountDetailsViewModel> DirectIncomes { get; set; }
        public List<ProfitAndLossAccountDetailsViewModel> IndirectExpenses { get; set; }
        public List<ProfitAndLossAccountDetailsViewModel> DirectExpenses { get; set; }
    }

    public class ProfitAndLossAccountDetailsViewModel
    {
        public string AccountHeadName { get; set; }
        public short AccountHeadTypeKey { get; set; }
        public byte AccountGroupKey { get; set; }
        public decimal Amount { get; set; }
    }

}

