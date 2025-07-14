using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class CourseTypeViewModel : BaseModel
    {
        public CourseTypeViewModel()
        {
            IsActive = true;
            HasSecondLanguage = true;
        }
        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CourseTypeNameExpressionErrorMessage")]
        public string CourseTypeName { get; set; }
        public bool HasSecondLanguage { get; set; }
        public bool IsActive { get; set; }
        public string HasSecondLanguageText { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
    }
}
