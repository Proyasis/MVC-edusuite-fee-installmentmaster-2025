using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeeUserPermissionViewModel : BaseModel
    {
        public EmployeeUserPermissionViewModel()
        {
            Countries = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            UserPermissions = new List<UserPermissionViewModel>();
            MenuTypes = new List<SelectListModel>();
        }


        public string BranchAccess { get; set; }
        public string CountryAccess { get; set; }
        public string SearchText { get; set; }

        public long EmployeeKey { get; set; }
        public short? MenuTypeKey { get; set; }

        public List<UserPermissionViewModel> UserPermissions { get; set; }
        public List<SelectListModel> Countries { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> MenuTypes { get; set; }

        public List<DashBoardPermissionViewModel> DashBoardPermission { get; set; }
    }
    public class UserPermissionViewModel
    {
        public long? RowKey { get; set; }
        public int? MenuKey { get; set; }
        public string MenuName { get; set; }
        public string MenuCode { get; set; }

        public short? ActionKey { get; set; }
        public string ActionName { get; set; }
        public string ActionCode { get; set; }

        public string MenuControllerName { get; set; }
        public string MenuActionName { get; set; }

        public string MenuOptionalParam { get; set; }
        public bool? IsActive { get; set; }

        public short? MenuTypeKey { get; set; }
        public string MenuTypeName { get; set; }

        public long DisplayOrder { get; set; }
    }

    public class DashBoardPermissionViewModel
    {
        public long? RowKey { get; set; }
        public short? DashBoardTypeKey { get; set; }
        public string DashBoardTypeName { get; set; }
        public string DashBoardTypeCode { get; set; }
        public short? DashBoardContentKey { get; set; }
        public string DashBoardContentName { get; set; }     
        public bool? IsActive { get; set; }
        public long DisplayOrder { get; set; }
    }
}
