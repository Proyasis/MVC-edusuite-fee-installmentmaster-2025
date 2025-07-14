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
    public class CashFlowViewModel : BaseModel
    {
        public CashFlowViewModel()
        {
            AccountGroups = new List<SelectListModel>();
            PaymentModes = new List<SelectListModel>();
            AccountHeadTypes = new List<SelectListModel>();
            AccountHeads = new List<SelectListModel>();
            BankAccounts = new List<SelectListModel>();
            CashFlowTypes = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            PendingAccount = new List<PendingAccountViewModel>();
            CashFlowDate = DateTimeUTC.Now;
            FromDate = DateTimeUTC.Now;
            ToDate = DateTimeUTC.Now;
            IsOrderPayment = false;
            PaymentModeSub = new List<SelectListModel>();
            IsContra = false;
        }
        public long RowKey { get; set; }
        public long CashFlowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowDateRequired")]
        //[DateRestriction("CashFlowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateDifferenceErrorMessage")]
        public DateTime CashFlowDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowTypeKeyRequired")]
        public byte CashFlowTypeKey { get; set; }
        public string CashFlowTypeName { get; set; }
        public short? AccountHeadTypeKey { get; set; }
        public string AccountHeadTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadRequired")]
        public long AccountHeadKey { get; set; }
        public long OldAccountHeadKey { get; set; }
        public long OldBankKey { get; set; }
        public short OldPaymentModeKey { get; set; }
        public byte OldCashFlowTypeKey { get; set; }
        public long PartyKey { get; set; }
        public string AccountHeadName { get; set; }
        public string AccountHeadCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountGroupRequired")]
        public byte? AccountGroupKey { get; set; }
        public string AccountGroupName { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowVoucherNumberLengthErrorMessage")]
        public string VoucherNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowAmountRequired")]
        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowAmountRegularExpressionErrorMessage")]
        public decimal? Amount { get; set; }
        public decimal? Excess { get; set; }
        public decimal? TotalAmountToBePaid { get; set; }
        public decimal? AdvanceAmount { get; set; }
        public decimal? TotalBalance { get; set; }
        public decimal? ConversionSize { get; set; }
        public string PaperSize { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeKeyRequired")]
        public short PaymentModeKey { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankTransaction", ResourceType = typeof(EduSuiteUIResources))]
        public short? PaymentModeSubKey { get; set; }
        public string PaymentModeName { get; set; }
        public string PaymentModeSubName { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberLengthErrorMessage")]
        public string CardNumber { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public long? BankAccountKey { get; set; }
        public string BankAccountName { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberRequired")]
        [StringLength(25, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberLengthErrorMessage")]
        public string ChequeOrDDNumber { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDDateRequired")]
        public DateTime? ChequeOrDDDate { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "ReferenceNo", ResourceType = typeof(EduSuiteUIResources))]
        public string ReferenceNumber { get; set; }
        public byte? TransactionTypeKey { get; set; }
        public short? ChequeStatusKey { get; set; }
        public long? TransactionKey { get; set; }
        public string TransactionTypeName { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PurposeLengthErrorMessage")]
        public string Purpose { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaidByLengthErrorMessage")]
        public string PaidBy { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AuthorizedByLengthErrorMessage")]
        public string AuthorizedBy { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ReceivedByLengthErrorMessage")]
        public string ReceivedBy { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "OnBehalfOfLengthErrorMessage")]
        public string OnBehalfOf { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RemarksLengthErrorMessage")]
        public string Remarks { get; set; }
        public decimal? BankAccountBalance { get; set; }

        //[GreaterThanOrEqualToIfNot("Amount", "IsOrderPayment", true, "CashFlowTypeKey", DbConstants.CashFlowType.Out, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BalanceErrorMessage")]
        public decimal? AccountHeadBalance { get; set; }
        public bool IsOrderPayment { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountBalace { get; set; }
        public decimal AccountAmount
        {
            get
            {
                return AmountBalace + AmountPaid;
            }
            set { }
        }
        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> PaymentModeSub { get; set; }
        public List<SelectListModel> AccountHeads { get; set; }
        public List<SelectListModel> AccountHeadTypes { get; set; }
        public List<SelectListModel> AccountGroups { get; set; }
        public List<SelectListModel> CashFlowTypes { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<PendingAccountViewModel> PendingAccount { get; set; }
        public string CompanyImageUrl { get; set; }
        public string CompanyEmailId { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyName { get; set; }
        public bool ShowAdminBankBalance { get; set; }
        public long TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string ReceiptNumber { get; set; }
        public bool IsContra { get; set; }
        public bool IsPayment { get; set; }
        public string ChequeAction { get; set; }
        public DateTime? SearchDate { get; set; }

    }

    public class PendingAccountViewModel : BaseModel
    {
        public DateTime PaymentDate { get; set; }
        public long PaymentKey { get; set; }
        public long PendingAccountKey { get; set; }
        public string ReferenceNumber { get; set; }
        public string OrderNumber { get; set; }
        public decimal AmountTotal { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? PaidUpdate { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal10RegularExpressionErrorMessage")]
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountTotal { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public DateTime OrderDate { get; set; }
        public int TransactionTypeKey { get; set; }
        public decimal? AmountBalace
        {
            get
            {
                return AmountTotal - TotalAmountPaid;
            }
            set { }
        }
        public decimal? BillBalanceAmount { get; set; }
    }
}
