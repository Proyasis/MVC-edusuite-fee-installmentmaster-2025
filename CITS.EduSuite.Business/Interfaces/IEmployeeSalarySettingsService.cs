using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IEmployeeSalarySettingsService
    {

        EmployeeSalaryMasterViewModel GetEmployeeSalarySettingsById(long EmployeeId);

        EmployeeSalarySettingsViewModel UpdateEmployeeSalarySettings(EmployeeSalarySettingsViewModel model);

        //EmployeeSalarySettingsViewModel CheckEmployeeSalarySettingsTypeExists(short PayHeadKey, long EmployeeKey, long RowKey);

        EmployeeSalarySettingsViewModel DeleteEmployeeSalarySettings(AdditionalSalaryComponentViewModel model);


    }
}
