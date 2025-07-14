using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationFamilyDetailsViewModel : BaseModel
    {
        public ApplicationFamilyDetailsViewModel()
        {
            FamilyDetailsModel = new List<FamilyDetailsModel>();
            IfchkLogin = false;

        }
        public long ApplicationKey { get; set; }
        public string AdmissionNo { get; set; }

        public List<FamilyDetailsModel> FamilyDetailsModel { get; set; }

        [RequiredIfTrue("IfchkLogin", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Username_Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Username_Length")]
        [System.Web.Mvc.Remote("CheckUserNameExists", "EmployeeUserAccount", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CheckUserNameExists")]

        public string UserName { get; set; }

        [RequiredIfTrue("IfchkLogin", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Password_Length")]
        public string Password { get; set; }

        //[Compare("Password", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ConfirmPasswordErrorMessage")]
        public string ConfirmPassword { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PasswordHintLengthErrorMessage")]
        public string PasswordHint { get; set; }

        public bool IsActive { get; set; }
        public bool IfchkLogin { get; set; }



    }
    public class FamilyDetailsModel
    {

        public FamilyDetailsModel()
        {

        }
        public long RowKey { get; set; }
        public short FamilyMemberTypeKey { get; set; }
        public string FamilymemberTypeName { get; set; }

        [RequiredIfTrue("IfLogin", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NameRequired")]      
        public string Name { get; set; }

        [RequiredIfTrue("IfLogin", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ApplicationPhoneRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PhoneNumberLengthErrorMessage")]
        [RegularExpression(@"^[0-9 +]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PhoneNumberExpressionErrorMessage")]
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Occupation { get; set; }
        public bool SendMail { get; set; }
        public bool sendSMS { get; set; }
        public long ApplicationKey { get; set; }
        public long? AppuserKey { get; set; }
        public bool IsActive { get; set; }
        public bool IfLogin { get; set; }

    }

}
