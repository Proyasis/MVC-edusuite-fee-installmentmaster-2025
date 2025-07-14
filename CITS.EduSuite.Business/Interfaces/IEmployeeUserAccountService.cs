using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IEmployeeUserAccountService
    {
        EmployeeUserAccountViewModel GetEmployeeUserAccountById(Int64 id);

        EmployeeUserAccountViewModel CreateEmployeeUserAccount(EmployeeUserAccountViewModel model);


        EmployeeUserAccountViewModel UpdateEmployeeUserAccount(EmployeeUserAccountViewModel model);


        EmployeeUserAccountViewModel DeleteEmployeeUserAccount(EmployeeUserAccountViewModel model);

        EmployeeUserAccountViewModel CheckUserNameExists(string UserName, int RowKey);
    }
}
