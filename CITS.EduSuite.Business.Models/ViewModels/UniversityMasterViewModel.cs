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
    public class UniversityMasterViewModel : BaseModel
    {
        public UniversityMasterViewModel()
        {
            IsActive = true;
            AccountHeadType = new List<SelectListModel>();
            AccountGroup = new List<SelectListModel>();
        }
        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckUniversityMasterCodeExists", "UniversityMaster", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "AffiliationsTieUpsCode", ResourceType = typeof(EduSuiteUIResources))]
        public string UniversityMasterCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(80, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckUniversityMasterNameExists", "UniversityMaster", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Display(Name = "AffiliationsTieUpsName", ResourceType = typeof(EduSuiteUIResources))]
        public string UniversityMasterName { get; set; }
        public bool IsActive { get; set; }
        public bool AllowUniversityAccountHead { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }


        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadTypeRequired")]
        [RequiredIfTrue("AllowUniversityAccountHead", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadTypeRequired")]
        public short? AccountHeadTypeKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountGroupRequired")]
        [RequiredIfTrue("AllowUniversityAccountHead", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountGroupRequired")]
        public byte? AccountGroupKey { get; set; }

        public string AccountGroupName { get; set; }

        public List<SelectListModel> AccountHeadType { get; set; }
        public List<SelectListModel> AccountGroup { get; set; }
        public long? AccountHeadKey { get; set; }
        
    }
}
