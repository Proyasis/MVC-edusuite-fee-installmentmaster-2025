using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeUserPermissionService
    {
        bool CheckUserPermission(UserPermissionViewModel model);

        EmployeeUserPermissionViewModel GetUserPermissionsById(Int64 UserKey);

        EmployeeUserPermissionViewModel UpdateEmployeeUserPermission(EmployeeUserPermissionViewModel model);
        void FillMenuTypes(EmployeeUserPermissionViewModel model);

        bool CheckDashBoardPermission(DashBoardPermissionViewModel model);
    }
}
