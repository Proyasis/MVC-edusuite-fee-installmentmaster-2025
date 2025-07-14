using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ProvinceViewModel:BaseModel
    {
        public ProvinceViewModel()
        {
            CountryNames = new List<SelectListModel>();
            LanguageNames = new List<SelectListModel>();
            IsActive = true;
        }

        public int RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvinceNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvinceNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvinceNameRegularExpressionErrorMessage")]
        public string Provincename { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvincenameLocalRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvincenameLocalLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ProvincenameLocalRegularExpressionErrorMessage")]
        public string ProvincenameLocal { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CountryRequired")]
        public short CountryKey { get; set; }
        public string CountryName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LanguageNameRequired")]
        public short LanguageKey { get; set; }
        public string LanguageName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get; set; }
        public List<SelectListModel> CountryNames { get; set; }
        public List<SelectListModel> LanguageNames { get; set; }
    }
}












