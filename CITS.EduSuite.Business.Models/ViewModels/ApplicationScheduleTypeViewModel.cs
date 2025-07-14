using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationScheduleTypeViewModel : BaseModel
    {
        public ApplicationScheduleTypeViewModel()
        {
            IsActive = true;           
            IsSystem = false;          
        }

        public short RowKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "ScheduleType", ResourceType = typeof(EduSuiteUIResources))]
        public string ScheduleTypeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }
}
