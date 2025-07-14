using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class JournalViewModel : BaseModel
    {
        public JournalViewModel()
        {
            JournalDetails = new List<JournalDetailsViewModel>();
            Branches = new List<SelectListModel>();
            JournalDate = DateTimeUTC.Now;
        }

        public long RowKey { get; set; }
        

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Branch_Required")]
        public short BranchKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "JournalDateRequired")]
        //[DateRestriction("RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateDifferenceErrorMessage")]
        public DateTime JournalDate { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage200")]
        public string Remark { get; set; }
        public long? UserKey { get; set; }
        public List<JournalDetailsViewModel> JournalDetails { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public string BranchName { get; set; }

        public Decimal? TotalDebitAmount { get; set; }
        public Decimal? TotalCreditAmount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

    }
    public class JournalDetailsViewModel
    {
        public JournalDetailsViewModel()
        {
            AccountHeads = new List<SelectListModel>();
            AccountGroups = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        public long MasterKey { get; set; }

 

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountGroupRequired")]
        public long? AccountGroupKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadRequired")]
        [System.Web.Mvc.Remote("CheckAccountHeadExists", "Journal", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadSelectExists")]
        public long AccountHeadKey { get; set; }

        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DecimalRegularExpressionErrorMessage")]
        public decimal? Debit { get; set; }

        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DecimalRegularExpressionErrorMessage")]
        public decimal? Credit { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage200")]
        public string Remark { get; set; }

        public List<SelectListModel> AccountGroups { get; set; }
        public List<SelectListModel> AccountHeads { get; set; }
    }
}
