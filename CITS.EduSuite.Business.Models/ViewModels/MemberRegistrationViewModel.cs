using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class MemberRegistrationViewModel : BaseModel
    {
        public MemberRegistrationViewModel()
        {
            MemberTitle = new List<SelectListModel>();
            IdentificationType = new List<SelectListModel>();
            MemberType = new List<SelectListModel>();
            BorrowerType = new List<SelectListModel>();
            State = new List<SelectListModel>();
            MemberPhotoUrl = Resources.ModelResources.DefaultPhotoUrl;
            RegistrationDate = DateTime.Today;
            Branches = new List<SelectListModel>();

        }
        public long RowKey { get; set; }
        public int? MemberTitleKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "FirstName", ResourceType = typeof(EduSuiteUIResources))]
        public string MemberFirstName { get; set; }
        public HttpPostedFileBase MFile { get; set; }
        public string MemberPhotoUrl { get; set; }

        [CustomRequired("LastNameRequired", EnableProprety = "LastNameEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "LastName", ResourceType = typeof(EduSuiteUIResources))]
        public string MemberLastName { get; set; }

        [CustomRequired("MemberDOBRequired", EnableProprety = "MemberDOBEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "DOB", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? MemberDOB { get; set; }
        public string MemberPhoto { get; set; }

        [CustomRequired("IdentificationTypeRequired", EnableProprety = "IdentificationTypeEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "IdentificationType", ResourceType = typeof(EduSuiteUIResources))]
        public int? IdentificationTypeKey { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [RegularExpressionIf(@"^\d{4}\s\d{4}\s\d{4}$", "IdentificationTypeKey", 2, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AdharNumberRegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckIdentificationExists", "MemberRegistration", AdditionalFields = "IdentificationTypeKey,RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [CustomRequired("IdentificationNumberRequired", EnableProprety = "IdentificationNumberEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "IdentificationNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string IdentificationNumber { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [CustomRequired("StreetNameRequired", EnableProprety = "StreetNameEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Street", ResourceType = typeof(EduSuiteUIResources))]
        public string StreetName { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [CustomRequired("CityNameRequired", EnableProprety = "CityNameEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "City", ResourceType = typeof(EduSuiteUIResources))]
        public string CityName { get; set; }

        [CustomRequired("StateRequired", EnableProprety = "StateEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "State", ResourceType = typeof(EduSuiteUIResources))]
        public int? StateKey { get; set; }

        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [CustomRequired("ZipCodeRequired", EnableProprety = "ZipCodeEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "ZipCode", ResourceType = typeof(EduSuiteUIResources))]
        public string ZipCode { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [System.Web.Mvc.Remote("CheckPhoneExists", "MemberRegistration", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [CustomRequired("PhoneNoRequired", EnableProprety = "PhoneNoEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Phone", ResourceType = typeof(EduSuiteUIResources))]
        public string PhoneNo { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckEmailExists", "MemberRegistration", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [CustomRequired("EmailAddressRequired", EnableProprety = "EmailAddressEnabled", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Email", ResourceType = typeof(EduSuiteUIResources))]
        public string EmailAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Gender", ResourceType = typeof(EduSuiteUIResources))]
        public string Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "MemberType", ResourceType = typeof(EduSuiteUIResources))]
        public byte MemberTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "BorrowerType", ResourceType = typeof(EduSuiteUIResources))]
        public byte BorrowerTypeKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "RegistrationDate", ResourceType = typeof(EduSuiteUIResources))]
        public DateTime? RegistrationDate { get; set; }
        public bool IsBlockMember { get; set; }
        public byte ApplicationTypeKey { get; set; }
        public long ApplicationKey { get; set; }
        public string CardId { get; set; }
        public long? CardSerialNo { get; set; }
        public string BorrowerTypeName { get; set; }
        public string MemberTypeName { get; set; }
        public string MemberFullName { get; set; }
        public decimal RegistrationFee { get; set; }
        public decimal MemberShipFee { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Branch", ResourceType = typeof(EduSuiteUIResources))]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }
        public List<SelectListModel> MemberTitle { get; set; }
        public List<SelectListModel> IdentificationType { get; set; }
        public List<SelectListModel> MemberType { get; set; }
        public List<SelectListModel> BorrowerType { get; set; }
        public List<SelectListModel> State { get; set; }
        public List<SelectListModel> Branches { get; set; }

    }
}
