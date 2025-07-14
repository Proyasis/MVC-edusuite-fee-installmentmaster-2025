using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class UniversityCancelationViewModel : BaseModel
    {
        public UniversityCancelationViewModel()
        {
            Branches = new List<SelectListModel>();
            AccountHead = new List<SelectListModel>();
        }

        public long RowKey { get; set; }
        public long ApplicationKey { get; set; }
        public long? UniversityPaymentDetailsKey { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool IfServiceCharge { get; set; }

        [RequiredIfTrue("IfServiceCharge", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AccountHeadCode", ResourceType = typeof(EduSuiteUIResources))]
        public long? AccountHeadKey { get; set; }

        [RequiredIfTrue("IfServiceCharge", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ServiceCharge", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? ServiceFee { get; set; }

        public decimal? TotalDeductionFee { get; set; }

        public string Remarks { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Date", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? CancelDate { get; set; }

        public string StudentName { get; set; }
        public string VoucherNo { get; set; }
        public string PaymentModeName { get; set; }
        public string PaymentModeSubName { get; set; }
        public string ChequeOrDDNumber { get; set; }
        public DateTime? DDDate { get; set; }

        public short? BranchKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string SearchText { get; set; }

        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> AccountHead { get; set; }
    }

}
