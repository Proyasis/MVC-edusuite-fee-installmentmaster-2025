using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class ApplicationScheduleCallStatusViewModel : BaseModel
    {
        public ApplicationScheduleCallStatusViewModel()
        {
            IsActive = true;
            IsDuration = false;
            IsSystem = false;
            ShowInMenuKeys = new List<int>();
            MenuList = new List<SelectListModel>();
        }

        public short RowKey { get; set; }


        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "CallStatus", ResourceType = typeof(EduSuiteUIResources))]
        public string ApplicationCallStatusName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDuration { get; set; }
        public bool IsSystem { get; set; }
        public string ShowInMenuKeysList { get; set; }
        public List<SelectListModel> MenuList { get; set; }
        public List<Int32> ShowInMenuKeys { get; set; }
    }
}
