using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface ISalaryComponentService
    {
        List<SalaryComponentViewModel> GetSalaryComponentsByType(SalaryComponentViewModel model);
        SalaryComponentViewModel GetSalaryComponentById(SalaryComponentViewModel model);

        SalaryComponentViewModel CreateSalaryComponent(SalaryComponentViewModel model);

        SalaryComponentViewModel UpdateSalaryComponent(SalaryComponentViewModel model);

        SalaryComponentViewModel DeleteSalaryComponent(SalaryComponentViewModel model);

        SalaryComponentViewModel GetComponentTypes(SalaryComponentViewModel model);

        SalaryComponentViewModel GetBranches(SalaryComponentViewModel model);
        SalaryComponentViewModel GetEmployeesByBranchId(SalaryComponentViewModel model);
    }
}
