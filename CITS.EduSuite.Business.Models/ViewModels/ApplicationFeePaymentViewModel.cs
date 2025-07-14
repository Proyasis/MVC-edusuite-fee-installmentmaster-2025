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
    public class ApplicationFeePaymentViewModel : BaseModel
    {
        public ApplicationFeePaymentViewModel()
        {
            PaymentModes = new List<SelectListModel>();
            BankAccounts = new List<SelectListModel>();
            ApplicationFeePaymentDetails = new List<ApplicationFeePaymentDetailViewModel>();
            FeeDate = DateTimeUTC.Now;
            FeeTypes = new List<SelectListModel>();
            IsActive = true;
            PaymentModeKey = DbConstants.PaymentMode.Cash;
            ReceiptNumberConfigurationKey = DbConstants.PaymentReceiptConfigType.Receipt;
            FeeYears = new List<SelectListModel>();
            TaxRateTypes = new List<SelectListModel>();
            PaymentModeSub = new List<SelectListModel>();
            PurposeList = new List<string>();
            PaidBranches = new List<SelectListModel>();
            TaxRateTypeKey = DbConstants.TaxRateType.Inclusive;
            ScheduleFeeTypes = new List<SelectListModel>();
            IsSchedule = false;
        }
        public long RowKey { get; set; }
        public string RecieptNo { get; set; }
        public long ApplicationKey { get; set; }
        public string ApplicantName { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentMobile { get; set; }
        public string StudentEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeDateRequired")]
        public DateTime? FeeDate { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeRemarksLengthErrorMessage")]
        public string FeeDescription { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeRequired")]
        public short? PaymentModeKey { get; set; }
        public string PaymentModeName { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankTransaction", ResourceType = typeof(EduSuiteUIResources))]
        public short? PaymentModeSubKey { get; set; }
        public string PaymentModeSubName { get; set; }

        //[RequiredIf("PaymentModeSubKey", DbConstants.PaymentModeSub.Card, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberLengthErrorMessage")]
        public string CardNumber { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "ReferenceNo", ResourceType = typeof(EduSuiteUIResources))]
        public string ReferenceNumber { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public long? BankAccountKey { get; set; }
        public string BankAccountName { get; set; }
        public decimal? BankAccountBalance { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberRequired")]
        [StringLength(25, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberLengthErrorMessage")]
        public string ChequeOrDDNumber { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDDateRequired")]
        public DateTime? ChequeClearanceDate { get; set; }
        public short BranchKey { get; set; }
        public decimal? TotalFee { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? BalanceFee { get; set; }
        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> PaymentModeSub { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<SelectListModel> FeeTypes { get; set; }
        public List<ApplicationFeePaymentDetailViewModel> ApplicationFeePaymentDetails { get; set; }
        public List<SelectListModel> TaxRateTypes { get; set; }
        public List<SelectListModel> ReceiptTypes { get; set; }

        //New By Khaleefa

        public DateTime? ChequeApprovedRejectedDate { get; set; }
        public string ChequeAction { get; set; }
        public bool IsChequeProcessCompleted { get; set; }
        public bool IsActive { get; set; }
        public string ChequeRejectedRemarks { get; set; }
        public List<SelectListModel> FeeYears { get; set; }

        //[LessThanOrEqualTo("TotalPaid", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "InitialPaymentLessThanErrorMessage")]
        public decimal? InitialPayment { get; set; }
        public int? FeeYearForTotal { get; set; }
        public decimal? FeeAmountForTotal { get; set; }
        public decimal? FeePaidForTotal { get; set; }
        public decimal? BalanceFeeForTotal { get; set; }
        public string FeeYearTextForTotal { get; set; }
        public string InitialPaymentMonth { get; set; }
        public decimal? InitialPaymentAmount { get; set; }
        public decimal? InitialPaymentAmountPaid { get; set; }
        public decimal? InitialPaymentbalanceDue { get; set; }
        public DateTime? InitialPaymentDueDate { get; set; }
        public int? InitialPaymentYear { get; set; }
        public int? InstallmentYear { get; set; }
        public string InitialPaymentYearText { get; set; }
        public bool HasInstallment { get; set; }
        public long? SerialNumber { get; set; }
        public bool IsTax { get; set; }
        public byte? TaxRateTypeKey { get; set; }
        public short? ChequeStatusKey { get; set; }
        public List<string> PurposeList { get; set; }

        // In this belowed two variable are declared for Student Fees are goes to  Other Branch Accounts
        public short? PaidBranchKey { get; set; }
        public List<SelectListModel> PaidBranches { get; set; }
        public bool? IsRefunded { get; set; }

        [CustomRequired("MultipleReceiptRequired", EnableProprety = "MultipleReceiptEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ReceiptType", ResourceType = typeof(EduSuiteUIResources))]
        public short? ReceiptNumberConfigurationKey { get; set; }
        public string ReceiptNumberConfigurationName { get; set; }
        public bool IsSchedule { get; set; }

        [RequiredIfTrue("IsSchedule", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Date", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? ScheduleFollowupDate { get; set; }

        [RequiredIfTrue("IsSchedule", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "FeeType", ResourceType = typeof(EduSuiteUIResources))]
        public List<short?> ScheduleFeeTypeKeys { get; set; }
        public string ScheduleFeeRemarks { get; set; }
        public List<SelectListModel> ScheduleFeeTypes { get; set; }
    }

    public class ApplicationFeePaymentDetailViewModel
    {
        public long RowKey { get; set; }
        public long ApplicationFeePaymentKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeRequired")]
        [System.Web.Mvc.Remote("CheckFeeTypeExists", "ApplicationFeePayment", AdditionalFields = "FeeYear", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeExists")]
        public short FeeTypeKey { get; set; }
        public string FeeTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeAmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        //[LessThanOrEqualTo("TotalPaid", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeAmountLessthanErrorMessage")]
        public decimal? FeeAmount { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeYearRequired")]
        // [System.Web.Mvc.Remote("CheckFeeTypeExists", "UniversityPayment", AdditionalFields = "FeeYear", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeYearExists")]
        [RequiredIfFalse("IsFeeTypeYear", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeYearRequired")]
        public int? FeeYear { get; set; }
        public string FeeYearText { get; set; }
        public byte CashFlowTypeKey { get; set; }
        public bool IsFeeTypeYear { get; set; }
        public decimal? CGSTRate { get; set; }
        public decimal? SGSTRate { get; set; }
        public decimal? IGSTRate { get; set; }
        public decimal? CessRate { get; set; }
        public decimal? CGSTAmount { get; set; }
        public decimal? SGSTAmount { get; set; }
        public decimal? IGSTAmount { get; set; }
        public decimal? CessAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeAmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? TaxableAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? GSTAmount { get; set; }
        public decimal? GSTRate { get; set; }
        public string HSNCode { get; set; }
        public bool? IsInitial { get; set; }
        public decimal? TotalItemAmount { get; set; }
        public decimal? TotalCessAmount { get; set; }
        public decimal? TotalCessRate { get; set; }
        public decimal? TotalPaid { get; set; }
        public byte? TaxRateTypeKey { get; set; }
        public decimal? OldPaid { get; set; }
        public bool IsDeduct { get; set; }


        //New Changes by Khaleefa on 31 Jul 2019 End
    }

    public class ApplicationFeePrintViewModel
    {
        public ApplicationFeePrintViewModel()
        {
            ApplicationFeePaymentViewModel = new ApplicationFeePaymentViewModel();
            PrintDownload = false;
            IsFranchise = false;
        }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string StudentMobile { get; set; }
        public string ProgramName { get; set; }
        public string UniversityName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string DistrictName { get; set; }
        public string AmountInWords { get; set; }
        public ApplicationFeePaymentViewModel ApplicationFeePaymentViewModel { get; set; }
        public string GSTINNumber { get; set; }
        public long? ApplicationKey { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyLogoPath { get; set; }
        public bool PrintDownload { get; set; }
        public string BranchName { get; set; }
        public bool IsFranchise { get; set; }
        public string Branchmail { get; set; }
        public string CompanyWebsite { get; set; }
        public string BillAddedBy { get; set; }
        public bool AllowHideFeeBalance { get; set; }


    }





}
