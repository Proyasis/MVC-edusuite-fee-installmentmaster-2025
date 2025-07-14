using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BranchViewModel : BaseModel
    {
        public BranchViewModel()
        {
            IsActive = true;
            Countries = new List<SelectListModel>();
            Provinces = new List<SelectListModel>();
            Districts = new List<SelectListModel>();
            Designations = new List<SelectListModel>();
            Departments = new List<SelectListModel>();
            DepartmentKeys = new List<short>();
            IsFranchise = false;
            BranchLogoPath = Resources.ModelResources.DefaultPhotoUrl;
        }

        public short RowKey { get; set; }

        [System.Web.Mvc.Remote("CheckBranchCodeExists", "Branch", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ExistsErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]
        public string BranchCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]

        public string BranchName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "Name", ResourceType = typeof(EduSuiteUIResources))]
        public string BranchNameLocal { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "AddressLine1", ResourceType = typeof(EduSuiteUIResources))]
        public string AddressLine1 { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "AddressLine2", ResourceType = typeof(EduSuiteUIResources))]

        public string AddressLine2 { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "AddressLine3", ResourceType = typeof(EduSuiteUIResources))]
        public string AddressLine3 { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "City", ResourceType = typeof(EduSuiteUIResources))]
        public string CityName { get; set; }


        [StringLength(6, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 +]{6}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PostalCodeExpressionErrorMessage")]
        [Display(Name = "PostalCode", ResourceType = typeof(EduSuiteUIResources))]
        public string PostalCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 +]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "PhoneNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string PhoneNumber1 { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 +]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "PhoneNumber2", ResourceType = typeof(EduSuiteUIResources))]
        public string PhoneNumber2 { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "FaxNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string FaxNumber { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ValidExpressionErrorMessage")]
        [Display(Name = "Email", ResourceType = typeof(EduSuiteUIResources))]
        public string EmailAddress { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonNameExpressionErrorMessage")]
        [Display(Name = "ContactPerson", ResourceType = typeof(EduSuiteUIResources))]
        public string ContactPersonName { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 +]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "Phone", ResourceType = typeof(EduSuiteUIResources))]
        public string ContactPersonPhone { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Country", ResourceType = typeof(EduSuiteUIResources))]
        public int CountryKey { get; set; }
        public string CountryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Province", ResourceType = typeof(EduSuiteUIResources))]
        public int ProvinceKey { get; set; }
        public string ProvinceName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "District", ResourceType = typeof(EduSuiteUIResources))]
        public int DistrictKey { get; set; }
        public string DistrictName { get; set; }

        public short? DesignationKey { get; set; }
        public string DesignationName { get; set; }

        public string TelephoneCode { get; set; }

        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionDigitErrorMessage")]
        [Display(Name = "OpeningBalance", ResourceType = typeof(EduSuiteUIResources))]
        public decimal? OpeningCashBalance { get; set; }

        public decimal? CurrentCashBalance { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DepartmentRequired")]
        public List<short> DepartmentKeys { get; set; }
        public int DepartmentKey { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentCount { get; set; }
        public short DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
        public bool IsFranchise { get; set; }

        public HttpPostedFileBase PhotoFile { get; set; }
        public string BranchLogo { get; set; }
        public string BranchLogoPath { get; set; }

        public List<SelectListModel> Designations { get; set; }
        public List<SelectListModel> Countries { get; set; }
        public List<SelectListModel> Provinces { get; set; }
        public List<SelectListModel> Districts { get; set; }
        public List<SelectListModel> Departments { get; set; }
    }
}
