using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BankTransactionViewModel : BaseModel
    {
        public BankTransactionViewModel()
        {
            BankTransactionTypes = new List<SelectListModel>();
            FromBankAccounts = new List<SelectListModel>();
            ToBankAccounts = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            SearchBankAccounts = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "SalaryComponentTypeRequired")]
        public byte BankTransactionTypeKey { get; set; }
        public string BankTransactionTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankTransactionDateRequired")]
        public DateTime? TransactionDate { get; set; }
                
        [RequiredIf("CheckFromBank", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankAccount", ResourceType = typeof(EduSuiteUIResources))]
        public long? FromBankAccountKey { get; set; }
        public string FromBankAccountName { get; set; }
        public decimal? FromBankAccountBalance { get; set; }
        
        [RequiredIf("CheckToBank", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BankAccount", ResourceType = typeof(EduSuiteUIResources))]
        public long? ToBankAccountKey { get; set; }
        public string ToBankAccountName { get; set; }
        public decimal? ToBankAccountBalance { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankTransactionAmountRequired")]
        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankTransactionAmountRegularExpressionErrorMessage")]
        public decimal? Amount { get; set; }

        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankChargeRegularExpressionErrorMessage")]
        public decimal? BankCharge { get; set; }

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

        [StringLength(1000, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankTransactionRemarksLengthErrorMessage")]
        public string Remarks { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [NotMapped]
        public bool CheckFromBank
        {
            get
            {
                return (this.BankTransactionTypeKey == 2 || this.BankTransactionTypeKey == 3);
            }
        }
        [NotMapped]
        public bool CheckToBank
        {
            get
            {
                return (this.BankTransactionTypeKey == 1 || this.BankTransactionTypeKey == 3);
            }
        }

        public List<SelectListModel> BankTransactionTypes { get; set; }
        public List<SelectListModel> FromBankAccounts { get; set; }
        public List<SelectListModel> ToBankAccounts { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string SearchText { get; set; }
        public long? OldFromBankAccountKey { get; set; }
        public long? OldToBankAccountKey { get; set; }
        public decimal? AccountHeadBalance { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime? SearchDate { get; set; }
        public long? SearchBankAccountKey { get; set; }
        public List<SelectListModel> SearchBankAccounts { get; set; }

    }
}
