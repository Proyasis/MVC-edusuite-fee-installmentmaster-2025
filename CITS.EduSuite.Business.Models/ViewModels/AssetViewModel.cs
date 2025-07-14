using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class AssetViewModel : BaseModel
    {
        public AssetViewModel()
        {
            AssetTypes = new List<SelectListModel>();
            AssetDetailList = new List<AssetDetailsViewModel>();
            Branches = new List<SelectListModel>();
        }
        public long MasterKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "AssetType", ResourceType = typeof(EduSuiteUIResources))]
        public int AssetTypeKey { get; set; }
        public int AccountHeadTypeKey { get; set; }
        public long AccountHeadKey { get; set; }
        public string AccountHeadName { get; set; }
        public int AssetCount { get; set; }
        public short BranchKey { get; set; }
     
        public List<SelectListModel> AssetTypes { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<AssetDetailsViewModel> AssetDetailList { get; set; }
    }

    public class AssetDetailsViewModel
    {
        public AssetDetailsViewModel()
        {
            PeriodTypeKey = DbConstants.PeriodType.Year;
            PeriodType = new List<SelectListModel>();
            PurchasingDate = DateTimeUTC.Now;
            IsActive = true;
        }
        public long? RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "PeriodType", ResourceType = typeof(EduSuiteUIResources))]
        public byte PeriodTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "PurchasingDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime PurchasingDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]        
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "LifePeriod", ResourceType = typeof(EduSuiteUIResources))]
        public short? LifePeriod { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]        
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]
        public string AssetDetailName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]        
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckAssetDetailCode", "Asset", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]
        public string AssetDetailCode { get; set; }
        public string Period { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]        
        [RegularExpression(@"^\d{0,18}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "Amount", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? Amount { get; set; }
        public decimal? AccumulateDepreciation { get; set; }
        public decimal? BookValue { get; set; }
        public bool IsActive { get; set; }
        public List<SelectListModel> PeriodType { get; set; }

        public string PeriodTypeName { get; set; }
    }
}
