using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ClassModeViewModel : BaseModel
    {
        public ClassModeViewModel()
        {
            IsActive = true;
        }

        public short RowKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassModeNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassModeNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-:,/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ClassModeNameExpressionErrorMessage")]
        public string ClassModeName { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
    }
}
