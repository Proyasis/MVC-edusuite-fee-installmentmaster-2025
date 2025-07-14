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
    public class FutureTransactionPaymentViewModel:BaseModel
    {
        public FutureTransactionPaymentViewModel()
        {
            PaymentModes = new List<SelectListModel>();
            BankAccounts = new List<SelectListModel>();
            PaymentDate = DateTimeUTC.Now;
            PaymentModeKey = DbConstants.PaymentMode.Cash;
            Branches = new List<SelectListModel>();
            Employees = new List<SelectListModel>();
            CashFlowTypes = new List<SelectListModel>();
            FutureTransactionOtherAmountTypes = new List<FutureTransactionOtherAmountTypeViewModel>();
            PaymentModeSub = new List<SelectListModel>();
        }
        public long PaymentKey { get; set; }
        public long? PartyKey { get; set; }
        public long? AccountHeadKey { get; set; }
        public long? OldAccountHeadKey { get; set; }
        public long? UserKey { get; set; }
        public short? BranchKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowTypeKeyRequired")]
        public byte? CashFlowTypeKey { get; set; }
        public long? OldPartyKey { get; set; }
        public long MasterKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeRequired")]
        public short PaymentModeKey { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankTransaction", ResourceType = typeof(EduSuiteUIResources))]
        public short? PaymentModeSubKey { get; set; }
        public string PaymentModeName { get; set; }
        public string PaymentModeSubName { get; set; }
        public string PartyName { get; set; }
        public string EmployeeName { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ReferenceNumberLengthErrorMessage")]
        public string ReferenceNumber { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal ConversionSize { get; set; }
        public string PaperSize { get; set; }
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaidAmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        //[LessThanOrEqualToIfNot("BankAccountBalance", "PaymentModeKey", DbConstants.PaymentMode.Cheque, "CashFlowTypeKey", DbConstants.CashFlowType.In, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BalanceErrorMessage")]
        [RegularExpressionIf(@"^[^0]+$", "PaidAmount", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GreaterThanZero")]
        public decimal? PaidAmount { get; set; }
        public decimal? TotalReceivedAmount { get; set; }
        public decimal AmountToPay { get; set; }
        public byte StateKey { get; set; }
        public byte OrderStatusKey { get; set; }
        public decimal? OldAdvanceBalance { get; set; }
        public byte? PartyStateKey { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal10RegularExpressionErrorMessage")]
        public decimal? Discount { get; set; }

        public decimal? BalanceAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentDateRequired")]
        //[DateRestriction("PaymentKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateDifferenceErrorMessage")]
        public DateTime PaymentDate { get; set; }



        //[RequiredIf("PaymentModeSubKey", DbConstants.PaymentModeSub.Card, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberLengthErrorMessage")]
        public string CardNumber { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public long? BankAccountKey { get; set; }
        public short OldPaymentModeKey { get; set; }
        public string BankAccountName { get; set; }

        public decimal? BankAccountBalance { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberRequired")]
        [StringLength(25, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberLengthErrorMessage")]
        public string ChequeOrDDNumber { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDDateRequired")]
        public DateTime? ChequeOrDDDate { get; set; }

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


        [StringLength(1000, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SalaryPaymentRemarksLengthErrorMessage")]

        public string Remarks { get; set; }
        public decimal? TaxableAmount { get; set; }
        public decimal? DeductAmount { get; set; }
        public decimal? EarningAmount { get; set; }
        public decimal AccountAmount
        {
            get
            {
                return BalanceAmount + PaidAmount ?? 0;
            }
            set { }
        }
        public List<SelectListModel> PaymentModeSub { get; set; }
        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Employees { get; set; }
        public List<SelectListModel> CashFlowTypes { get; set; }
        public List<FutureTransactionOtherAmountTypeViewModel> FutureTransactionOtherAmountTypes { get; set; }

        public bool IsAdavance { get; set; }
        public bool IsContra { get; set; }
        public bool IsMasterAdavance { get; set; }

        public string CompanyImageUrl { get; set; }
        public string CompanyEmailId { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyWebsite { get; set; }

        public string CompanyName { get; set; }
        public bool IsUpdate { get; set; }
        public decimal? OldAmount { get; set; }
        public decimal? BillBalanceAmount { get; set; }
        public string BankAccountDetails { get; set; }

        public string OrderNumber { get; set; }
        public string CashSalePartyMobile { get; set; }
        public bool ShowAdminBankBalance { get; set; }
    }
    
}
