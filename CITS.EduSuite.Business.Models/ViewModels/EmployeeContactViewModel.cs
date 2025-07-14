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
    public class EmployeeContactViewModel:BaseModel
    {
        public EmployeeContactViewModel()
        {
            EmployeeContacts = new List<ContactViewModel>();           
        }

        public long EmployeeKey { get; set; }
        public List<ContactViewModel> EmployeeContacts { get; set; }
    }

    public class ContactViewModel
    {
        public ContactViewModel()
        {
            AddressTypes = new List<SelectListModel>();
            Countries = new List<SelectListModel>();
            Provinces = new List<SelectListModel>();
            Districts = new List<SelectListModel>();
        }
        public long RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressTypeRequired")]
        [System.Web.Mvc.Remote("CheckAddressTypeExists", "EmployeeContact", AdditionalFields = "EmployeeKey,RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeContactExists")]
      
        public short AddressTypeKey { get; set; }
        public string AddressTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressLine1Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressLine1LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressLine1ExpressionErrorMessage")]
        public string AddressLine1 { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressLine2LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressLine2ExpressionErrorMessage")]
        public string AddressLine2 { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressLine3LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "AddressLine3ExpressionErrorMessage")]
        public string AddressLine3 { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CityNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ,()&-/\s]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CityNameExpressionErrorMessage")]

        public string FullAddress { get; set; }
        public string CityName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryRequired")]
        public int CountryKey { get; set; }
        public string CountryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvinceRequired")]
        public int ProvinceKey { get; set; }
        public string ProvinceName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DistrictRequired")]
        public int DistrictKey { get; set; }
        public string DistrictName { get; set; }

        [StringLength(6, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 +]{6}$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PostalCodeExpressionErrorMessage")]
        [Display(Name = "PostalCode", ResourceType = typeof(EduSuiteUIResources))]
        public string PostalCode { get; set; }     


        public List<SelectListModel> AddressTypes { get; set; }
        public List<SelectListModel> Countries { get; set; }
        public List<SelectListModel> Provinces { get; set; }
        public List<SelectListModel> Districts { get; set; }
    }
}
