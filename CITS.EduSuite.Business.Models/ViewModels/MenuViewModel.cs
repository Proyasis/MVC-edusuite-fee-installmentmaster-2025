using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class MenuViewModel : BaseModel
    {
        public MenuViewModel()
        {
            MenuDetails = new List<MenuDetailViewModel>();
        }
        public string MenuTypeName { get; set; }
        public string MenuTypeIconClassName { get; set; }
        public List<MenuDetailViewModel> MenuDetails { get; set; }
    }
    public class MenuDetailViewModel : BaseModel
    {
        public MenuDetailViewModel()
        {
            IsActive = true;
            MenuTypes = new List<SelectListModel>();
            Actions = new List<SelectListModel>();
            MenuActionsList = new List<MenuActionModel>();
            MenuCatagories = new List<SelectListModel>();
        }

        public int RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "MenuType", ResourceType = typeof(EduSuiteUIResources))]
        public short? MenuTypeKey { get; set; }
        public string MenuTypeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "MenuCode", ResourceType = typeof(EduSuiteUIResources))]
        public string MenuCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "MenuName", ResourceType = typeof(EduSuiteUIResources))]
        public string MenuName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "ActionName", ResourceType = typeof(EduSuiteUIResources))]
        public string ActionName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "ControllerName", ResourceType = typeof(EduSuiteUIResources))]
        public string ControllerName { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "OptionalParameter", ResourceType = typeof(EduSuiteUIResources))]
        public string OptionalParameter { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "IconClassName", ResourceType = typeof(EduSuiteUIResources))]
        public string IconClassName { get; set; }
        public string MenucatagoryName { get; set; }

        [Display(Name = "Menucatagory", ResourceType = typeof(EduSuiteUIResources))]
        public byte? MenucatagoryKey { get; set; }

        [Display(Name = "BlankSpace", ResourceType = typeof(EduSuiteUIResources))]
        public bool IsActive { get; set; }
        public string IsActiveText { get { return IsActive ? EduSuiteUIResources.Yes : EduSuiteUIResources.No; } }

        [Display(Name = "DisplayOrder", ResourceType = typeof(EduSuiteUIResources))]
        public int DisplayOrder { get; set; }
        public List<SelectListModel> MenuTypes { get; set; }
        public List<SelectListModel> Actions { get; set; }
        public List<SelectListModel> MenuCatagories { get; set; }
        public List<MenuActionModel> MenuActionsList { get; set; }
    }

    public class MenuActionModel : BaseModel
    {
        public MenuActionModel()
        {
            IsActive = true;
        }

        public short? RowKey { get; set; }


        [Display(Name = "Menu", ResourceType = typeof(EduSuiteUIResources))]
        public int MenuKey { get; set; }
        public string MenuName { get; set; }

        [Display(Name = "Action", ResourceType = typeof(EduSuiteUIResources))]
        public short ActionKey { get; set; }
        public string ActionName { get; set; }

        [Display(Name = "BlankSpace", ResourceType = typeof(EduSuiteUIResources))]
        public bool IsActive { get; set; }
    }
}
