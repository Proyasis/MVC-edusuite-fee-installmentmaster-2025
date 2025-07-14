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
    public class FutureTransactionViewModel : BaseModel
    {
        public FutureTransactionViewModel()
        {
            BankAccounts = new List<SelectListModel>();
            AccountHeads = new List<SelectListModel>();
            CashFlowTypes = new List<SelectListModel>();
            FutureTransactionOtherAmountTypes = new List<FutureTransactionOtherAmountTypeViewModel>();
            FutureTransactionPaidAmounts = new List<PaymentWindowViewModel>();
            FutureTransactionRecievedAmounts = new List<PaymentWindowViewModel>();
            HSNCodes = new List<SelectListModel>();
            InstallmentTypes = new List<SelectListModel>();
            BillDate = DateTimeUTC.Now;
            InstallmentPeriod = 1;
            AccountGroups = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            States = new List<SelectListModel>();
        }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadRequired")]
        public long AccountHeadKey { get; set; }
        public string AccountHeadName { get; set; }
        public List<SelectListModel> AccountHeads { get; set; }
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DecimalRegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountRequired")]
        public decimal? Amount { get; set; }
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DecimalRegularExpressionErrorMessage")]
        public decimal? AmountPaid { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? BankAccountBalance { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public long? BankAccountKey { get; set; }
        public string BankAccountName { get; set; }
        public bool IsContra { get; set; }
        public bool IsInstallment { get; set; }
        public bool IsOpeningBalance { get; set; }
        public bool IsTax { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BillDateRequired")]
        public DateTime BillDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BillNumberRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BillNumberLengthErrorMessage")]
        public string BillNo { get; set; }
        public List<SelectListModel> Branches { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [RequiredIf("PaymentModeSubKey", DbConstants.PaymentModeSub.Card, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberLengthErrorMessage")]
        public string CardNumber { get; set; }

        public byte? CashFlowTypeKey { get; set; }
        public string CashFlowTypeName { get; set; }
        public List<SelectListModel> CashFlowTypes { get; set; }
        public decimal? CGSTAmt { get; set; }

        [RequiredIfTrue("CGSTvalidationActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaxPercentageRequired")]
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        public decimal? CGSTPer { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDDateRequired")]
        public DateTime? ChequeOrDDDate { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberRequired")]
        [StringLength(25, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberLengthErrorMessage")]
        public string ChequeOrDDNumber { get; set; }
        public int CompanyStateKey { get; set; }
        public decimal? DeductAmount { get; set; }

        [RegularExpression(@"^\d{0,8}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal8RegularExpressionErrorMessage")]
        public decimal? DownPayment { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? EarningAmount { get; set; }
        public List<FutureTransactionOtherAmountTypeViewModel> FutureTransactionOtherAmountTypes { get; set; }
        public List<PaymentWindowViewModel> FutureTransactionPaidAmounts { get; set; }
        public long FutureTransactionPaymentRowKey { get; set; }
        public List<PaymentWindowViewModel> FutureTransactionRecievedAmounts { get; set; }

        [RegularExpression(@"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TinNumberExpressionErrorMessage")]
        public string GSTINNumber { get; set; }


        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage50")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HSNCodeNumberRegularExpressionErrorMessage")]
        [RequiredIfTrue("IsTax", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "HSNCodeRequired")]
        public string HSNCode { get; set; }
        public long? HSNCodeKey { get; set; }
        public List<SelectListModel> HSNCodes { get; set; }
        public decimal? IGSTAmt { get; set; }

        [RequiredIfTrue("IGSTvalidationActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaxPercentageRequired")]
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        public decimal? IGSTPer { get; set; }

        [RequiredIfTrue("IsInstallment", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InstallmentAmountRequired")]
        [RegularExpression(@"^\d{0,8}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal8RegularExpressionErrorMessage")]
        public decimal? InstallmentAmount { get; set; }

        [RequiredIfTrue("IsInstallment", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InstallmentFlowRequired")]
        public byte? InstallmentFlowKey { get; set; }

        [RequiredIfTrue("IsInstallment", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InstallmentPeriodRequired")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IntegerRegularExpressionErrorMessage")]
        //[RegularExpressionIf(@"^[^0]+$", "InstallmentPeriod", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GreaterThanZero")]
        public int? InstallmentPeriod { get; set; }

        [RequiredIfTrue("IsInstallment", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InstallmentTypeRequired")]
        public byte? InstallmentTypeKey { get; set; }
        public string InstallmentTypeName { get; set; }
        public List<SelectListModel> InstallmentTypes { get; set; }

        public bool IGSTvalidationActive { get; set; }
        public bool CGSTvalidationActive { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowTypeKeyRequired")]
        public byte? MasterCashFlowTypeKey { get; set; }
        public decimal? NonTaxableAmount { get; set; }

        [RequiredIfTrue("IsInstallment", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NoOfInstallmentRequired")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IntegerRegularExpressionErrorMessage")]
        //[RegularExpressionIf(@"^[^0]+$", "NoOfInstallment", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GreaterThanZero")]
        public int? NoOfInstallment { get; set; }

        [RegularExpression(@"^\d{0,8}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal8RegularExpressionErrorMessage")]
        public decimal? OpeningPaidAmount { get; set; }

        [RegularExpression(@"^\d{0,8}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal8RegularExpressionErrorMessage")]
        public decimal? OpeningReceivedAmount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeRequired")]
        public short PaymentModeKey { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankTransaction", ResourceType = typeof(EduSuiteUIResources))]
        public short? PaymentModeSubKey { get; set; }
        public string PaymentModeName { get; set; }
        public string PaymentModeSubName { get; set; }

      
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ReferenceNumberLengthErrorMessage")]
        public string ReferenceNumber { get; set; }
        
        public decimal? RoundOff { get; set; }
        public long RowKey { get; set; }
        public decimal? SGSTAmt { get; set; }

        [RequiredIfTrue("CGSTvalidationActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TaxPercentageRequired")]
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        public decimal? SGSTPer { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StateRequired")]
        public int StateKey { get; set; }
        public string StateName { get; set; }
        public List<SelectListModel> States { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxableAmount { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TotalAmountRequired")]
        public decimal? TotalAmount { get; set; }
        public decimal? TotalInAmount { get; set; }
        public int TotalItems { get; set; }
        public decimal? TotalOutAmount { get; set; }
        public int TotalRecords { get; set; }
        

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountGroupRequired")]
        public long? AccountGroupKey { get; set; }
        public List<SelectListModel> AccountGroups { get; set; }
        public string ProductDescription { get; set; }
        public string ProductName { get; set; }

        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> PaymentModeSub { get; set; }
    }
    public class FutureTransactionOtherAmountTypeViewModel : BaseModel
    {
        public FutureTransactionOtherAmountTypeViewModel()
        {
            AccountHeads = new List<SelectListModel>();
        }

        public long? AccountHeadKey { get; set; }
        public string AccountHeadName { get; set; }
        public List<SelectListModel> AccountHeads { get; set; }

        public decimal? Amount { get; set; }

        public decimal? AmountPer { get; set; }
        public long? FutureTransactionKey { get; set; }
        public long? FutureTransactionPaymentKey { get; set; }
        public bool IsAddition { get; set; }
        public long? RowKey { get; set; }
    }
}
