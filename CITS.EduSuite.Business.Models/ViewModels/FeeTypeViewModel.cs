using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class FeeTypeViewModel : BaseModel
    {
        public FeeTypeViewModel()
        {
            IsActive = true;
            CashFlowTypeList = new List<SelectListModel>();
            AccountHeadType = new List<SelectListModel>();
            AccountGroup = new List<SelectListModel>();
            FeeTypeMode = new List<SelectListModel>();
            HSNCodes = new List<SelectListModel>();
            ReceiptTypes = new List<SelectListModel>();
        }
        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeNameExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckFeeTypeExist", "FeeType", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeNameExistsErrorMessage")]
        public string FeeTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CashFlowTypeRequired")]
        public byte CashFlowTypeKey { get; set; }
        public string CashFlowTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountHeadTypeRequired")]
        public short AccountHeadTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountGroupRequired")]
        public byte AccountGroupKey { get; set; }
        public string AccountGroupName { get; set; }
        public short DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FeeTypeModeRequired")]
        public short? FeeTypeModeKey { get; set; }
        public string FeeTypeModeName { get; set; }
        public bool IsUniverisity { get; set; }
        public string IsUniverisityName { get; set; }
        public bool IsTax { get; set; }
        public string IsTaxName { get; set; }

        [RequiredIfTrue("IsTax", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GSTMasterRequired")]
        public short? GSTMasterkey { get; set; }
        public long? AccountHeadKey { get; set; }

        [CustomRequired("MultipleReceiptRequired", EnableProprety = "MultipleReceiptEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ReceiptType", ResourceType = typeof(EduSuiteUIResources))]
        public short? ReceiptNumberConfigurationKey { get; set; }
        public string ReceiptNumberConfigurationName { get; set; }
        public bool AllowFeetypeOnlyIncome { get; set; }
        public bool IsDeduct { get; set; }
        public bool AllowHideFeeBalance { get; set; }
        public List<SelectListModel> CashFlowTypeList { get; set; }
        public List<SelectListModel> AccountHeadType { get; set; }
        public List<SelectListModel> AccountGroup { get; set; }
        public List<SelectListModel> FeeTypeMode { get; set; }
        public List<SelectListModel> HSNCodes { get; set; }
        public List<SelectListModel> ReceiptTypes { get; set; }

    }
}
