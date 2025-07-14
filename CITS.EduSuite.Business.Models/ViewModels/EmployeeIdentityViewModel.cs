using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeIdentityViewModel:BaseModel
    {
        public EmployeeIdentityViewModel()
        {
            EmployeeIdentities = new List<IdentityViewModel>();
           
          
        }
        public long EmployeeKey { get; set; }
        public List<IdentityViewModel> EmployeeIdentities { get; set; }
       
    }
    public class IdentityViewModel
    {

        public IdentityViewModel()
        {
            IdentityTypes = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IdentityTypeRequired")]
        [System.Web.Mvc.Remote("CheckIdentityTypeExists", "EmployeeIdentity", AdditionalFields = "EmployeeKey,RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeIdentityExists")]
        public int IdentityTypeKey { get; set; }
        public string IdentityTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IdentyUniqueIDRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IdentyUniqueIDLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IdentyUniqueIDExpressionErrorMessage")]
        [RegularExpressionIfAttribute(@"^\d{4}\s\d{4}\s\d{4}$", "IdentityTypeKey", DbConstants.IdentityType.AdharNumber, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdharNumberRegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckAdharNumberExists", "EmployeeIdentity", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdharNumberExists")]
       
        public string IdentyUniqueID { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IdentityIssuedDateRequired")]
        public DateTime? IdentityIssuedDate { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IdentityExpiryDateRequired")]
        [GreaterThanOrEqualTo("IdentityIssuedDate", PassOnNull = true, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IdentityExpiryDateCompareErrorMessage")]
      
        public DateTime? IdentityExpiryDate { get; set; }
        public HttpPostedFileBase AttanchedFile { get; set; }
        public string AttanchedFileName { get; set; }
        public string AttanchedFileNamePath { get; set; }
        public List<SelectListModel> IdentityTypes { get; set; }
     
    }

}

