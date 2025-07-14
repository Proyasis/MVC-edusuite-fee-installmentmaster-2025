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
    public class AssetTypeViewModel : BaseModel
    {
        public AssetTypeViewModel()
        {
            HSNCodes = new List<SelectListModel>();
            IsActive = true;
            HaveDepreciation = true;
            DepreciationMethods = new List<SelectListModel>();
        }
        public int RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "AssetType", ResourceType = typeof(EduSuiteUIResources))]
        public string AssetTypeName { get; set; }
        public bool IsActive { get; set; }
        public string AccountHeadCode { get; set; }
        public bool IsTax { get; set; }
        [RequiredIf("IsTax", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        [Display(Name = "CGSTPer", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? CGSTPer { get; set; }
        [RequiredIf("IsTax", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,3}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal3RegularExpressionErrorMessage")]
        [Display(Name = "SGSTPer", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? SGSTPer { get; set; }

        [RequiredIf("IsTax", true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^-*[0-9,\.]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NumericExpressionErrorMessage")]
        [Display(Name = "IGSTPer", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? IGSTPer { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [RequiredIfTrue("IsTax", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "HSNCode", ResourceType = typeof(EduSuiteUIResources))]
        public string HSNCode { get; set; }

        public List<SelectListModel> HSNCodes { get; set; }
        public long? HSNCodeKey { get; set; }
        public string HSNCodeName { get; set; }
        public bool HaveDepreciation { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "DepreciationMethod", ResourceType = typeof(EduSuiteUIResources))]
        public byte DepreciationMethodKey { get; set; }
        public List<SelectListModel> DepreciationMethods { get; set; }
    }
}
