using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeUserAccountViewModel : BaseModel
    {
        public EmployeeUserAccountViewModel()
        {
            Roles = new List<SelectListModel>();
            IsActive = true;
        }
        public long RowKey { get; set; }
        public long EmployeeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Username_Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Username_Length")]
        [System.Web.Mvc.Remote("CheckUserNameExists", "EmployeeUserAccount", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckUserNameExists")]

        public string UserName { get; set; }

        [RequiredIf("RowKey", 0, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Length")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ConfirmPasswordErrorMessage")]
        public string ConfirmPassword { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PasswordHintLengthErrorMessage")]
        public string PasswordHint { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RoleRequired")]
        public short RoleKey { get; set; }
        public string RoleName { get; set; }

        public bool IsActive { get; set; }    
        public string IsActiveName { get; set; }
        public string EmployeeCode { get; set; }


        public List<SelectListModel> Roles { get; set; }
    }
}
