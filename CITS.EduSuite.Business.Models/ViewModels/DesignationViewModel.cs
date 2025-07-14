using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class DesignationViewModel : BaseModel
    {
        public DesignationViewModel()
        {
            DesignationPermissions = new List<UserPermissionViewModel>();
            IsActive = true;
        }

        public short RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DesignationNameRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DesignationNameLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DesignationNameRegularExpressionErrorMessage")]
        public string DesignationName { get; set; }
        public short? HigherDesignationKey { get; set; }
        public string HigherDesignationName { get; set; }

        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        public List<SelectListModel> HigherDesignations { get; set; }
        public List<UserPermissionViewModel> DesignationPermissions { get; set; }
    }
}
