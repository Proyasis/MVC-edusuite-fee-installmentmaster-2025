using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class AccountHeadOpeningBalanceViewModel : BaseModel
    {
        public AccountHeadOpeningBalanceViewModel()
        {
            Branches = new List<SelectListModel>();
            OpeningBalanceDetails = new List<OpeningBalanceDetailViewModel>();
            OpeningDate = DateTimeUTC.Now;
        }

        


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "OpeningDateRequired")]
        public DateTime OpeningDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short BranchKey { get; set; }
        
        public List<SelectListModel> Branches { get; set; }
        public string BranchName { get; set; }
        public List<OpeningBalanceDetailViewModel> OpeningBalanceDetails { get; set; }
        public decimal? AccountReceivable { get; set; }
        public decimal? AccountPayable { get; set; }
        public decimal? AdvanceReceivable { get; set; }
        public decimal? AdvancePayable { get; set; }
        public decimal? Inventory { get; set; }

        public decimal? TotalDebit { get; set; }
        public decimal? TotalCredit { get; set; }


    }

    public class OpeningBalanceDetailViewModel : BaseModel
    {
        public int RowKey { get; set; }

        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowAmountRegularExpressionErrorMessage")]
        public decimal? DebitAmount { get; set; }

        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowAmountRegularExpressionErrorMessage")]
        public decimal? CreditAmount { get; set; }
        public long AccountHeadKey { get; set; }
        public byte? AccountGroupKey { get; set; }
        public string AccountHeadName { get; set; }
    }
}
