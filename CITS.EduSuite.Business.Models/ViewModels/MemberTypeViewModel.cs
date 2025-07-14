using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class MemberTypeViewModel : BaseModel
    {
        public MemberTypeViewModel()
        {
            Statuses = new List<SelectListModel>();
            NumberOfBooksAllowed = 1;
            IsActive = true;
        }


        public byte RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]        
        [Display(Name = "MemberType", ResourceType = typeof(EduSuiteUIResources))]
        public string MemberTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Range(1, Byte.MaxValue, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NumberOfBooksAllowedRangeErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "NumberOfBooksAllowed", ResourceType = typeof(EduSuiteUIResources))]
        public byte? NumberOfBooksAllowed { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "RegistrationFee", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? RegistrationFee { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "MembershipFee", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? MemberShipFee { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "LateFeePerDay", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? LateFeePerDay { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "ReturnPeriod", ResourceType = typeof(EduSuiteUIResources))]
        public int? ReturnPeriod { get; set; }

        [System.Web.Mvc.Remote("CheckMemberTypeCodeExist", "MemberType", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]
        public string MemberTypeCode { get; set; }

        public List<SelectListModel> Statuses { get; set; }


    }
}
