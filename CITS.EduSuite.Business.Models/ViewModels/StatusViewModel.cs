using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class StatusViewModel : BaseModel
    {
        public StatusViewModel()
        {
            IsActive = true;
        }

        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StatusNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StatusLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z ]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "StatusRegularExpressionErrorMessage")]
        public string StatusName { get; set; }
        
        public bool IsActive { get; set; }
        
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }
        
    }
}
