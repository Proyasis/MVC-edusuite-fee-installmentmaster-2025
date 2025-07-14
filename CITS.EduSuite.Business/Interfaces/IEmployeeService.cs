using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeService
    {
        List<EmployeePersonalViewModel> GetEmployees(EmployeePersonalViewModel model);
        EmployeePersonalViewModel DeleteEmployee(EmployeePersonalViewModel model);
        EmployeePersonalViewModel GetSearchDropdownLists(EmployeePersonalViewModel model);

        List<EmployeePersonalViewModel> GetOrganizationChart();

        //List<EmployeeFileHandOverViewModel> GetEmployeeFileHandOvers(EmployeeFileHandOverViewModel model);
        EmployeeFileHandOverViewModel GetFileHandoverDropdownLists(EmployeeFileHandOverViewModel model);

        EmployeeFileHandOverViewModel UpdateEmployeesFileHandover(List<EmployeeFileHandOverViewModel> modelList);

        EmployeeFileHandOverViewModel DeleteHandover(List<long> Keys);
        EmployeePersonalViewModel GetEmployeePhotoById(long Id);
        EmployeePersonalViewModel UpdateEmployeePhoto(EmployeePersonalViewModel model);
        EmployeePersonalViewModel DeleteEmployeePhoto(long Id);
        EmployeeViewModel GetEmployeeDetailsById(long? Id);
       
    }
}
