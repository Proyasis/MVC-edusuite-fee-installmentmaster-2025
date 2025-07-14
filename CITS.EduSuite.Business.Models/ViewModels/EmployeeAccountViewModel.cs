using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeAccountViewModel : BaseModel
    {

        public EmployeeAccountViewModel()
        {
            Banks = new List<SelectListModel>();
            AccountTypes = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeNameRequired")]
        public long EmployeeKey { get; set; }
        public string EmployeeName { get; set; }
       
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BankRequired")]
        public short BankKey { get; set; }
        public string BankName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchLocationRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchLocationLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchLocationRegularExpressionErrorMessage")]
        public string BranchLocation { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IFSCCodeRequired")]
        [StringLength(11, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IFSCCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9  a-zA-Z]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IFSCCodeLengthRegularExpressionErrorMessage")]
        public string IFSCCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountNumberRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountNumberLengthRegularExpressionErrorMessage")]

        [System.Web.Mvc.Remote("CheckAccountNumberExists", "EmployeeAccount", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountNumberExists")]
        public string AccountNumber { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NameInAccountLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NameInAccountRegularExpressionErrorMessage")]
        public string NameInAccount { get; set; }
        [StringLength(9, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MICRCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MICRCodeRegularExpressionErrorMessage")]
        public string MICRCode { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AccountTypeRequired")]
        public short AccountTypeKey { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdharNumberRequired")] 
        [RegularExpression(@"^\d{4}\s\d{4}\s\d{4}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdharNumberRegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckAdharNumberExists", "EmployeeAccount", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdharNumberExists")]
        public string AdharNumber { get; set; }
        public string UANNumber { get; set; }
        public List<SelectListModel> AccountTypes { get; set; }
        public List<SelectListModel> Banks { get; set; }
    }
}






