using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeTaskService
    {
        List<EmployeeTaskViewModel> GetEmployeeTasks(EmployeeTaskViewModel model);

        EmployeeTaskViewModel GetEmployeeTaskById(EmployeeTaskViewModel model);

        EmployeeTaskViewModel CreateEmployeeTask(EmployeeTaskViewModel model);

        EmployeeTaskViewModel UpdateEmployeeTask(EmployeeTaskViewModel model);

        EmployeeTaskViewModel DeleteEmployeeTask(EmployeeTaskViewModel model);

        EmployeeTaskViewModel GetBranches(EmployeeTaskViewModel model);
        EmployeeTaskViewModel GetEmployeesByBranchId(EmployeeTaskViewModel model);
    }
}
