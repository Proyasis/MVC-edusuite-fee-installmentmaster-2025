using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class CountryViewModel : BaseModel
    {
        public CountryViewModel()
        {
            LanguageNames = new List<SelectListModel>();
            Currencies = new List<SelectListModel>();
            IsActive = true;

        }
        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryNameRegularExpressionErrorMessage")]
        public string CountryName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryNameLocalErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryNameLocalRegularExpressionErrorMessage")]
        public string CountryNameLocal { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryShortNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryShortNameRegularExpressionErrorMessage")]
        public string CountryShortName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NationalityNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NationalityNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "NationalityNameRegularExpressionErrorMessage")]
        public string NationalityName { get; set; }


        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CapitalCityNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CapitalCityNameRegularExpressionErrorMessage")]
        public string CapitalCityName { get; set; }


        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TelephoneCodeLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TelephoneCodeRegularExpressionErrorMessage")]
        public string TelephoneCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LanguageNameRequired")]
        public short LanguageKey { get; set; }
        public string LanguageName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CurrencyRequired")]
        public short CurrencyKey { get; set; }
        public string CurrencyName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "IsActiveRequired")]
        public bool IsActive { get; set; }
        public string IsActiveText { get; set; }
        public int DisplayOrder { get; set; }
        public List<SelectListModel> LanguageNames { get; set; }
        public List<SelectListModel> Currencies { get; set; }

    }


}

