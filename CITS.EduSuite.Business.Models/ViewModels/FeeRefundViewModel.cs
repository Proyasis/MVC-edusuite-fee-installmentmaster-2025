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
    public class FeeRefundViewModel : BaseModel
    {

        public FeeRefundViewModel()
        {
            PaymentModes = new List<SelectListModel>();
            BankAccounts = new List<SelectListModel>();
            FeeRefundDetails = new List<FeeRefundDetailViewModel>();
            RefundDate = DateTimeUTC.Now;
            FeeTypes = new List<SelectListModel>();
            IsActive = true;
            PaymentModeKey = DbConstants.PaymentMode.Cash;
            FeeYears = new List<SelectListModel>();
            PaymentModeSub = new List<SelectListModel>();
            PurposeList = new List<string>();
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            PaidBranches = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public string VoucherNo { get; set; }
        public long ApplicationKey { get; set; }
        public string ApplicantName { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentMobile { get; set; }
        public string StudentEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeDateRequired")]
        public DateTime? RefundDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeRequired")]
        public short? PaymentModeKey { get; set; }
        public short? OldPaymentModeKey { get; set; }
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
        public short? BranchKey { get; set; }
        public string BranchName { get; set; }

        [EqualTo("TotalRefundedAmount", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeRefundLessThanErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AmountPaid", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? RefundAmount { get; set; }
        public decimal? BalanceFee { get; set; }
        public short? BatchKey { get; set; }
        public string BatchName { get; set; }
        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> PaymentModeSub { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<SelectListModel> FeeTypes { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Branches { get; set; }

        public List<FeeRefundDetailViewModel> FeeRefundDetails { get; set; }

        //New By Khaleefa

        
        public decimal? TotalRefundedAmount { get; set; }
        public DateTime? ChequeApprovedRejectedDate { get; set; }
        public string ChequeAction { get; set; }
        public bool IsChequeProcessCompleted { get; set; }
        public bool IsActive { get; set; }
        public string ChequeRejectedRemarks { get; set; }
        public List<SelectListModel> FeeYears { get; set; }
        public int? FeeYearForTotal { get; set; }
        public decimal? FeeAmountForTotal { get; set; }
        public decimal? FeePaidForTotal { get; set; }
        public decimal? BalanceFeeForTotal { get; set; }
        public string FeeYearTextForTotal { get; set; }
        public long? SerialNumber { get; set; }
        public short? ChequeStatusKey { get; set; }
        public List<string> PurposeList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string Remarks { get; set; }
        public int? TotalRecords { get; set; }
        public short? AcademicTermKey { get; set; }
        public short? CurrentYear { get; set; }
        public int? CourseDuration { get; set; }
        public string CourseName { get; set; }
        public string AcademicTermName { get; set; }
        public string UniversityName { get; set; }
        public short? ProcessStatus { get; set; }
        public string ProcessStatusName { get; set; }
        public short? PaidBranchKey { get; set; }
        public List<SelectListModel> PaidBranches { get; set; }
        public long? CourseKey { get; set; }
        public short? UniversityKey { get; set; }
    }

    public class FeeRefundDetailViewModel
    {
        public long RowKey { get; set; }
        public long FeeRefundMasterKey { get; set; }
        public long? FeePaymentDetailKey { get; set; }

        public short? FeeTypeKey { get; set; }
        public string FeeTypeName { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? ReturnAmount { get; set; }

        public int? FeeYear { get; set; }
        public string FeeYearText { get; set; }
        public bool IsFeeTypeYear { get; set; }

        public decimal? TotalFee { get; set; }
        public decimal? TotalPaidAmount { get; set; }


        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? TotalAmount { get; set; }

        public decimal? BeforeTakenAdvance { get; set; }

        public bool IsDeduct { get; set; }
        public decimal? AdvanceBalance
        {
            get
            {
                return (BeforeTakenAdvance) - (ReturnAmount ?? 0);
            }
            set { }
        }
        public string RecieptNo { get; set; }
        public DateTime? FeeRecieptDate { get; set; }
    }

    public class FeeRefundPrintViewModel
    {
        public FeeRefundPrintViewModel()
        {
            FeeRefundViewModel = new FeeRefundViewModel();
        }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
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
        public string StudentMobile { get; set; }
        public string GSTINNumber { get; set; }
        public FeeRefundViewModel FeeRefundViewModel { get; set; }



    }
}
