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
    public class AssetPurchaseMasterViewModel : BaseModel
    {
        public AssetPurchaseMasterViewModel()
        {
            Parties = new List<SelectListModel>();
            AssetPurchaseDetails = new List<AssetPurchaseDetailsViewModel>();
            PaymentModes = new List<SelectListModel>();
            BankAccounts = new List<SelectListModel>();
            BillDate = DateTimeUTC.Now.Date;
            Branches = new List<SelectListModel>();

            PaymentModeKey = DbConstants.PaymentMode.Cash;
            CashFlowTypeKey = DbConstants.CashFlowType.Out;
        }

        public long RowKey { get; set; }
        public string OrderNumber { get; set; }
        public string PartyName { get; set; }
        public string PartyMobileNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short BranchKey { get; set; }
        public List<SelectListModel> Branches { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PartyRequired")]
        public long PartyKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BillNumberRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BillNumberLengthErrorMessage")]
        [System.Web.Mvc.Remote("CheckBillNumberExists", "AssetPurchase", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BillNumberExistsErrorMessage")]
        public string BillNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BillDateRequired")]
        public DateTime BillDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TotalAmountRequired")]
        // [RegularExpression(@"^\d{0,18}(\.\d{1,5})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DecimalRegularExpressionErrorMessage")]
        public decimal? TotalAmount { get; set; }

        public List<SelectListModel> Parties { get; set; }
        public decimal? OldAdvanceBalance { get; set; }

        public List<AssetPurchaseDetailsViewModel> AssetPurchaseDetails { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountPaidRequired")]
        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DecimalRegularExpressionErrorMessage")]
        [LessThanOrEqualToIfNot("BankAccountBalance", "PaymentModeKey", DbConstants.PaymentMode.Cheque, "CashFlowTypeKey", DbConstants.CashFlowType.In, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BalanceErrorMessage")]
        public decimal? AmountPaid { get; set; }
        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeRequired")]
        public short PaymentModeKey { get; set; }

        [RequiredIf("PaymentModeKey", 2, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberLengthErrorMessage")]

        public string CardNumber { get; set; }

        [RequiredIfNot("PaymentModeKey", DbConstants.PaymentMode.Cash, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankAccountRequired")]

        public long? BankAccountKey { get; set; }
        public byte? CashFlowTypeKey { get; set; }
        public byte StateKey { get; set; }
        public decimal? RoundOff { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal10RegularExpressionErrorMessage")]
        public decimal? Discount { get; set; }
        public byte PartyStateKey { get; set; }
        public string BankAccountName { get; set; }

        public decimal? BankAccountBalance { get; set; }

        [RequiredIf("PaymentModeKey", 4, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberRequired")]
        [StringLength(25, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberLengthErrorMessage")]

        public string ChequeOrDDNumber { get; set; }

        [RequiredIf("PaymentModeKey", 4, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDDateRequired")]
        public DateTime? ChequeOrDDDate { get; set; }

        public long AssetPurchasePaymentRowKey { get; set; }


        public byte PartyTypeKey { get; set; }
        public decimal? SubTotal { get; set; }

        public decimal? BalanceAmount { get; set; }

        public decimal? TaxableAmount { get; set; }
        public decimal? NonTaxableAmount { get; set; }

        public decimal? CGSTAmt { get; set; }
        public decimal? CGSTPer { get; set; }
        public decimal? IGSTAmt { get; set; }
        public decimal? IGSTPer { get; set; }

        public decimal? SGSTAmt { get; set; }
        public decimal? SGSTPer { get; set; }
        public decimal? PartyTotalAmount { get; set; }
        public decimal? PartyTotalAmountPaid { get; set; }
        public decimal? PartyTotalBalanceAmount { get; set; }
        public List<PartyViewModel> PartyDetails_Order { get; set; }
        public List<PaymentWindowViewModel> PurchasePayments { get; set; }
        public int TotalItems { get; set; }

      
        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

    }

    public class AssetPurchaseDetailsViewModel : BaseModel
    {

        public AssetPurchaseDetailsViewModel()
        {
            AssetTypes = new List<SelectListModel>();
            RateTypes = new List<SelectListModel>();
            PeriodType = new List<SelectListModel>();
            DepreciationMethods = new List<SelectListModel>();
        }

        public long? RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AssetTypeRequired")]
        public int AssetTypeKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "QuantityRequired")]
        [RegularExpression(@"^\s*-?[0-9]{1,5}\s*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "QuantityRegularExpressionErrorMessage")]
        //[GreaterThan("QuantityMinValue", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckLessEqalToZerroErrorMessage")]
        [Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckLessEqalToZerroErrorMessage")]
        public int? Quantity { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountPerQtyRequired")]
        [RegularExpression(@"^\d{0,8}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal8RegularExpressionErrorMessage")]
        public decimal? Amount { get; set; }
        public long OrderMasterKey { get; set; }

        public List<SelectListModel> AssetTypes { get; set; }
        public string MaterialName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DescriptionRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DescriptionLengthErrorMessage")]
        public string Description { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ReferenceNumberLengthErrorMessage")]
        public string ReferenceNumber { get; set; }
        public decimal? RowTotal { get; set; }

        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        public decimal? CGSTPer { get; set; }
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        public decimal? SGSTPer { get; set; }

        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        public decimal? IGSTPer { get; set; }
        public decimal? CGSTAmt { get; set; }
        public decimal? SGSTAmt { get; set; }
        public decimal? IGSTAmt { get; set; }
        public bool IsTax { get; set; }
        public decimal? RowTotalGST { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PeriodTypeRequired")]
        public byte PeriodTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LifePeriodRequired")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NoOfCopiesRegularExpressionErrorMessage")]
        public short? LifePeriod { get; set; }
        public string Period { get; set; }
        public bool? IsUsed { get; set; }
        public byte RateTypeKey { get; set; }
        public string RateTypeCode { get; set; }
        public List<SelectListModel> RateTypes { get; set; }
        public List<SelectListModel> PeriodType { get; set; }
        public decimal? RateTypeUnitLength { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DepreciationMethodRequired")]
        public byte DepreciationMethodKey { get; set; }

        [RequiredIf("DepreciationMethodKey", DbConstants.DepreciationMethod.Unitsofproduction, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProductionUnitRequired")]
        public decimal? ProductionLimit { get; set; }
        public List<SelectListModel> DepreciationMethods { get; set; }
    }
}
