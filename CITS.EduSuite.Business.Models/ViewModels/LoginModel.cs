using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.Security;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class LoginModel : BaseModel 
    {
        public LoginModel()
        {

        }

        public bool LoginSuccess { get; set; }

        public bool NeedsPasswordChange { get; set; }

        public CITSEduSuitePrincipalData UserPrincipalData { get; set; }

        public AppUserViewModel User { get; set; }

        public bool FailedLogin { get; set; }

        public string TopContent { get; set; }

        public string BottomContent { get; set; }

        public bool DisplayMobileLinks { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Required")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Length")]
        [Display(Name = "Password")]
        public string LoginPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Username_Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Username_Length")]
        [Display(Name = "Username")]
        public string LoginUsername { get; set; }

        public string ReturnUrl { get; set; }

        public string TimeZoneTime { get; set; }

        //Change password
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "OldPassword_Required")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Length")]
        [Display(Name = "Password")]
        [System.Web.Mvc.Remote("CheckPasswordExists", "Login", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckPasswordExistsErrorMessage")]
        public string OldPassword { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NewPassword_Required")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Length")]
        public string NewPassword { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ConfirmPassword_Required")]
        [StringLength(200, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Length")]
        [CompareAttribute("NewPassword",ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PasswordCompareMessage")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnterRecoverInformation")]
        [StringLength(255, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RecoverLengthErrorMessage")]
      
        public string ForgotPasswordName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public long UserKey { get; set; }
        public int RoleKey { get; set; }
        public long ForgotPasswordKey { get; set; }

        [RequiredIfTrue("IsSuccessful", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EnterOTP")]          
        public string OTP { get; set; }
        public bool? IsOTPSuccess { get; set; }
        public byte OTPType { get; set; }
     
    }
}
