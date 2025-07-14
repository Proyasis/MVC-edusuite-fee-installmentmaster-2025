using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IRoleService
    {
        List<RolesViewModel> GetRoles(string searchText);

        RolesViewModel GetRoleById(Int16? id);

        RolesViewModel CreateRole(RolesViewModel model);

        RolesViewModel UpdateRole(RolesViewModel model);

        RolesViewModel DeleteRole(RolesViewModel model);
    }
}
