using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeAccountService
    {
        //List<EmployeeAccountViewModel> GetEmployeeAccount(string searchText);

        EmployeeAccountViewModel GetEmployeeAccountById(Int64 id);

        EmployeeAccountViewModel CreateEmployeeAccount(EmployeeAccountViewModel model);


        EmployeeAccountViewModel UpdateEmployeeAccount(EmployeeAccountViewModel model);


        EmployeeAccountViewModel DeleteEmployeeAccount(EmployeeAccountViewModel model);

        EmployeeAccountViewModel CheckAccountNumberExists(string AccountNumber, Int64 RowKey);

        EmployeeAccountViewModel CheckAdharNumberExists(string AdharNumber, Int64 RowKey);

    }
}
