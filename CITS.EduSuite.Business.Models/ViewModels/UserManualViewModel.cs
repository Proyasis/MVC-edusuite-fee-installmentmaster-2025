using CITS.EduSuite.Business.Models.Resources;
using CITS.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class UserManualViewModel : BaseModel
    {
        public UserManualViewModel()
        {
            UserManualDetails = new List<UserManualDetailsViewModel>();
            UserManualTypes = new List<SelectListModel>();
            DashBoardTypes = new List<SelectListModel>();
            MenuTypes = new List<SelectListModel>();
            Menus = new List<SelectListModel>();
            DocumentPath = Resources.ModelResources.DefaultPhotoUrl;
        }
        public long RowKey { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "UserManualType", ResourceType = typeof(EduSuiteUIResources))]
        public short UserManualTypeKey { get; set; }
        public string UserManalTypeName { get; set; }

        [RequiredIf("UserManualTypeKey", DbConstants.UserManualType.DashBoard, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "DashBoardType", ResourceType = typeof(EduSuiteUIResources))]
        public short? DashBoardTypeKey { get; set; }
        public string DashBoardTypeName { get; set; }

        [RequiredIf("UserManualTypeKey", DbConstants.UserManualType.Menu, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "MenuType", ResourceType = typeof(EduSuiteUIResources))]
        public short? MenuTypeKey { get; set; }
        public string MenuTypeName { get; set; }

        [RequiredIf("UserManualTypeKey", DbConstants.UserManualType.Menu, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Menu", ResourceType = typeof(EduSuiteUIResources))]
        public int? MenuKey { get; set; }
        public string MenuName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Descreption", ResourceType = typeof(EduSuiteUIResources))]
        public string Descreption { get; set; }
        public string DocumentPath { get; set; }
        public string Document { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public HttpPostedFileBase DocumentFile { get; set; }
        public List<UserManualDetailsViewModel> UserManualDetails { get; set; }
        public List<SelectListModel> UserManualTypes { get; set; }
        public List<SelectListModel> DashBoardTypes { get; set; }
        public List<SelectListModel> MenuTypes { get; set; }
        public List<SelectListModel> Menus { get; set; }

    }
    public class UserManualDetailsViewModel : BaseModel
    {       
        public long RowKey { get; set; }
        public long UserManualMasterKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Descreption", ResourceType = typeof(EduSuiteUIResources))]
        public string Descreption { get; set; }
        public string DocumentPath { get; set; }
        public string Document { get; set; }
        public HttpPostedFileBase DocumentFileDetails { get; set; }

    }

    public class UserManualAllViewModel
    {
        public UserManualAllViewModel()
        {
            UserManualViewAll = new List<UserManualViewModel>();
        }
        public List<UserManualViewModel> UserManualViewAll { get; set; }
    }
}
