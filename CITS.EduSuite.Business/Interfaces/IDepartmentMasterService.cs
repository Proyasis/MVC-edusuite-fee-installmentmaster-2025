using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IDepartmentMasterService
    {
        DepartmentViewModel GetDepartmentById(int? id);
        DepartmentViewModel CreateDepartment(DepartmentViewModel model);
        DepartmentViewModel UpdateDepartment(DepartmentViewModel model);
        DepartmentViewModel DeleteDepartment(DepartmentViewModel model);
        List<DepartmentViewModel> GetDepartment(string searchText);
        DepartmentViewModel CheckDepartmentCodeExists(DepartmentViewModel model);
        DepartmentViewModel CheckDepartmentNameExists(DepartmentViewModel model);
    }
}
