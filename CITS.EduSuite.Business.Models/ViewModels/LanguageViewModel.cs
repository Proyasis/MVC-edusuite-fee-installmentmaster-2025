using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class LanguageViewModel : BaseModel
    {

        public LanguageViewModel()
        {
            IsActive = true;
        }
        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LanguageNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LanguageNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LanguageRegularExpressionErrorMessage")]

        public string LanguageName { get; set; }

        [StringLength(5, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LanguageShortNameLengthErrorMessage")]
        public string LanguageShortName { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

    }
}