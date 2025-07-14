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
    public class UniversityPaymentBulkViewmodel : BaseModel
    {
        public UniversityPaymentBulkViewmodel()
        {
            Batches = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            Universities = new List<SelectListModel>();
            Years = new List<SelectListModel>();
            AcademicTerms = new List<SelectListModel>();
            FeeTypes = new List<SelectListModel>();
            BankAccounts = new List<SelectListModel>();

            PaymentModes = new List<SelectListModel>();
            PaymentModeSub = new List<SelectListModel>();
            SearchPaymentModeKey = DbConstants.PaymentMode.Cash;


        }

        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Branch", ResourceType = typeof(EduSuiteUIResources))]
        public short? SearchBranchKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectCourseType")]
        public short? SearchCourseTypeKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectCourse")]
        public long? SearchCourseKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectUniversity")]
        public short? SearchUniversityKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectBatch")]
        public short? SearchBatchKey { get; set; }
        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectYear")]
        [RequiredIf("IsFeeTypeYear", false, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectYear")]
        public int? SearchYearKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectSyllabus")]
        public short? SearchAcademicTermKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SelectFeeType")]
        public short FeeTypeKey { get; set; }

        [RequiredIf("SearchPaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public short? SearchBankKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeRequired")]
        public short? SearchPaymentModeKey { get; set; }

        [RequiredIf("SearchPaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankTransaction", ResourceType = typeof(EduSuiteUIResources))]
        public short? SearchPaymentModeSubKey { get; set; }

        [RequiredIfTrue("IsActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeDateRequired")]
        public DateTime? SearchUniversityPaymentDate { get; set; }


        public bool IsActive { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
        public decimal? CenterShareAmount { get; set; }

        public string VoucherNo { get; set; }

        public long? SerialNumber { get; set; }

        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> Universities { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> Years { get; set; }
        public List<SelectListModel> FeeTypes { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }

        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> PaymentModeSub { get; set; }
        public List<SelectListModel> Branches { get; set; }

        public List<UniversityPaymentBulklist> UniversityPaymentBulklist { get; set; }

        public bool IsFeeTypeYear { get; set; }

    }

    public class UniversityPaymentBulklist
    {
        public UniversityPaymentBulklist()
        {
            BankAccounts = new List<SelectListModel>();
            PaymentModes = new List<SelectListModel>();
            PaymentModeSub = new List<SelectListModel>();
            PurposeList = new List<string>();
        }
        public bool IsBank { get { return ((PaymentModeKey == DbConstants.PaymentMode.Bank ? true : false) && IsActive); } set { } }
        public bool IsCheque { get { return ((PaymentModeKey == DbConstants.PaymentMode.Cheque ? true : false) && IsActive); } set { } }

        public List<SelectListModel> BankAccounts { get; set; }

        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> PaymentModeSub { get; set; }
        public long? UniversityPaymentDetailsKey { get; set; }
        public long ApplicationKey { get; set; }

        public long? Rowkey { get; set; }

        public long? UniversiyPaymentMasterKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeRequired")]
        [System.Web.Mvc.Remote("CheckFeeTypeExists", "ApplicationFeePayment", AdditionalFields = "FeeYear", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeExists")]
        public short FeeTypeKey { get; set; }
        public string FeeTypeName { get; set; }

        [RequiredIfTrue("IsActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeAmountRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AmountExpressionErrorMessage")]
        public decimal? UniversityPaymentAmount { get; set; }

        public int? UniversityPaymentYear { get; set; }
        public byte CashFlowTypeKey { get; set; }

        public string UniversityPaymentChalanDDNumber { get; set; }

        public DateTime? FeeDate { get; set; }

        public string FeeDescription { get; set; }

        [RequiredIfTrue("IsActive", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeDateRequired")]
        // [RequiredIf("IsActive", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeDateRequired")]
        // [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeDateRequired")]
        public DateTime? UniversityPaymentDate { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeRemarksLengthErrorMessage")]

        public string UniversityPaymentNote { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeRequired")]
        public short PaymentModeKey { get; set; }



        //[RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RequiredIfTrue("IsBank", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankTransaction", ResourceType = typeof(EduSuiteUIResources))]
        public short? PaymentModeSubKey { get; set; }
        public string PaymentModeName { get; set; }
        public string PaymentModeSubName { get; set; }


        //[RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Bank, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        [RequiredIfTrue("IsBank", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public long? BankAccountKey { get; set; }
        public string BankAccountName { get; set; }

        public decimal? BankAccountBalance { get; set; }

        //[RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberRequired")]
        [RequiredIfTrue("IsCheque", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberRequired")]
        [StringLength(25, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDNumberLengthErrorMessage")]
        public string ChequeOrDDNumber { get; set; }

        //[RequiredIf("PaymentModeKey", DbConstants.PaymentMode.Cheque, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDDateRequired")]
        [RequiredIfTrue("IsCheque", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ChequeOrDDDateRequired")]
        public DateTime? ChequeClearanceDate { get; set; }
        public short BranchKey { get; set; }


        //[RequiredIf("PaymentModeSubKey", DbConstants.PaymentModeSub.Card, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CardNumberLengthErrorMessage")]
        public string CardNumber { get; set; }


        [StringLength(30, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "ReferenceNo", ResourceType = typeof(EduSuiteUIResources))]
        public string ReferenceNumber { get; set; }
        public bool IsActive { get; set; }
        public int DataPageIndex { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public short CourseTypeKey { get; set; }
        public long CourseKey { get; set; }
        public short UniversityKey { get; set; }
        public short BatchKey { get; set; }
        public short? CurrentYear { get; set; }
        public List<string> PurposeList { get; set; }
    }
}
