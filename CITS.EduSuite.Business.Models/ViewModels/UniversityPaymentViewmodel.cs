using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class UniversityPaymentViewmodel : BaseModel
    {
        public UniversityPaymentViewmodel()
        {
            PaymentModes = new List<SelectListModel>();
            BankAccounts = new List<SelectListModel>();
            UniversityFeePaymentDetails = new List<UniversityPaymentDetailsmodel>();
            UniversityPaymentDate = DateTimeUTC.Now;
            FeeTypes = new List<SelectListModel>();

            PaymentModeKey = DbConstants.PaymentMode.Bank;
            FeeYears = new List<SelectListModel>();
            TotalFeeDetails = new List<ApplicationFeePaymentViewModel>();
            PaymentModeSub = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            PurposeList = new List<string>();
            PaidBranches = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public string RecieptNo { get; set; }
        public long ApplicationKey { get; set; }
        public string ApplicantName { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentMobile { get; set; }
        public string StudentEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeDateRequired")]
        public DateTime? UniversityPaymentDate { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeRemarksLengthErrorMessage")]

        public string UniversityPaymentNote { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeRequired")]
        public short PaymentModeKey { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankTransaction", ResourceType = typeof(EduSuiteUIResources))]
        public short? PaymentModeSubKey { get; set; }
        public string PaymentModeName { get; set; }
        public string PaymentModeSubName { get; set; }

        //[RequiredIf("PaymentModeSubKey", DbConstants.PaymentModeSub.Card, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberLengthErrorMessage")]
        public string CardNumber { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "ReferenceNo", ResourceType = typeof(EduSuiteUIResources))]
        public string ReferenceNumber { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public long? BankAccountKey { get; set; }
        public string BankAccountName { get; set; }

        public decimal? BankAccountBalance { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberRequired")]
        [StringLength(25, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberLengthErrorMessage")]

        public string ChequeOrDDNumber { get; set; }

        [RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDDateRequired")]
        public DateTime? ChequeClearanceDate { get; set; }
        public short BranchKey { get; set; }
        public short? BatchKey { get; set; }

        public List<SelectListModel> PaymentModeSub { get; set; }
        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<SelectListModel> FeeTypes { get; set; }
        public List<UniversityPaymentDetailsmodel> UniversityFeePaymentDetails { get; set; }

        public List<ApplicationFeePaymentViewModel> TotalFeeDetails { get; set; }
        public List<SelectListModel> FeeYears { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public string UniversityPaymentChalanDDNumber { get; set; }

        public decimal? CenterShareAmount { get; set; }

        public string VoucherNo { get; set; }

        public long? SerialNumber { get; set; }
        public DateTime? FeeDate { get; set; }

        public string FeeDescription { get; set; }

        // Bulk University Payment

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectCourseType")]
        public short SearchCourseTypeKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectCourse")]
        public long SearchCourseKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectUniversity")]
        public short SearchUniversityKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectBatch")]
        public short SearchBatchKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectYear")]
        public int SearchYearKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectSyllabus")]
        public short SearchAcademicTermKey { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }

        public short? ChequeStatusKey { get; set; }
        public DateTime? ChequeApprovedRejectedDate { get; set; }
        public string ChequeRejectedRemarks { get; set; }
        public string ChequeAction { get; set; }

        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> CourseYears { get; set; }

        public List<string> PurposeList { get; set; }
        public short? PaidBranchKey { get; set; }
        public List<SelectListModel> PaidBranches { get; set; }
        public bool? IsCancel { get; set; }
    }

    public class UniversityPaymentDetailsmodel
    {
        public long RowKey { get; set; }
        public long UniversiyPaymenMasterKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeRequired")]
        [System.Web.Mvc.Remote("CheckFeeTypeExists", "UniversityPayment", AdditionalFields = "UniversityPaymentYear", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeExists")]
        public short FeeTypeKey { get; set; }
        public string FeeTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeAmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? UniversityPaymentAmount { get; set; }

        [RequiredIfFalse("IsFeeTypeYear", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeYearRequired")]
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeYearRequired")]
        //[System.Web.Mvc.Remote("CheckFeeTypeExists", "UniversityPayment", AdditionalFields = "FeeYear", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeYearExists")]
        public int? UniversityPaymentYear { get; set; }

        public byte CashFlowTypeKey { get; set; }
        public bool IsFeeTypeYear { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? FeePaid { get; set; }
        public string FeeYearText { get; set; }
        public decimal? UniversityFeePaid { get; set; }

    }

    public class UniversityFeePrintViewModel
    {
        public UniversityFeePrintViewModel()
        {
            UniversityPaymentViewmodel = new UniversityPaymentViewmodel();
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
        public UniversityPaymentViewmodel UniversityPaymentViewmodel { get; set; }



    }
}
