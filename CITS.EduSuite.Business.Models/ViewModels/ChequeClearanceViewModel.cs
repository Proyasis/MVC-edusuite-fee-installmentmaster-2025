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
    public class ChequeClearanceViewModel:BaseModel
    {
        public ChequeClearanceViewModel()
        {
            PaymentModes = new List<SelectListModel>();
            BankAccounts = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            PaymentModeKey = DbConstants.PaymentMode.Bank;
            ClearanceDate = DateTimeUTC.Now;
            IsApproved = true;
            
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClearanceDateRequired")]
        [GreaterThanOrEqualTo("ChequeOrDDDate", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LesThanClearanceDateErrorMessage")]
        public DateTime ClearanceDate { get; set; }

        //[LessThanOrEqualToIfNot("BankAccountBalance", "IsApproved", false, "CashFlowTypeKey", DbConstants.CashFlowType.In, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BalanceErrorMessage")]
        public decimal? Amount { get; set; }
        public decimal? BankAccountBalance { get; set; }
        public string ChequeOrDDNumber { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RemarksLengthErrorMessage")]
        public string Remark { get; set; }
        public DateTime? ChequeOrDDDate { get; set; }
        public string TransactionTypeName { get; set; }
        public byte CashFlowTypeKey { get; set; }
        public short BranchKey { get; set; }
        public string AccountHeadName { get; set; }
        public string Purpose { get; set; }
        public long AccountHeadKey { get; set; }
        public bool IsApproved { get; set; }
        public bool IsOrderPayment { get; set; }
        public bool IsAdvance { get; set; }
        public string AccountHeadCode { get; set; }

        

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PaymentModeKeyRequired")]

        public short PaymentModeKey { get; set; }
        public string PaymentModeName { get; set; }

        [RequiredIf("PaymentModeKey", 3, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankAccountRequired")]
        public long? BankAccountKey { get; set; }
        public long? OldBankAccountKey { get; set; }
        
        public byte TransactionTypeKey { get; set; }

        public long TransactionKey { get; set; }
        public string BranchName { get; set; }
        public string CashFlowTypeName { get; set; }

        
        public List<SelectListModel> PaymentModes { get; set; }
        public List<SelectListModel> BankAccounts { get; set; }
        public List<SelectListModel> Branches { get; set; }

    }
}
